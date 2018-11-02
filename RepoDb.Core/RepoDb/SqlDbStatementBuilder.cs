using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System;
using RepoDb.Attributes;

namespace RepoDb
{
    /// <summary>
    /// A class used to build a SQL Statement for SQL Server. This is the default statement builder used by the library.
    /// </summary>
    public sealed class SqlDbStatementBuilder : IStatementBuilder
    {
        /// <summary>
        /// Creates a new instance of <see cref="SqlDbStatementBuilder"/> object.
        /// </summary>
        public SqlDbStatementBuilder() { }

        /// <summary>
        /// Creates a SQL Statement for repository batch-query operation that is meant for SQL Server.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The data entity object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="where">The query expression for SQL statement.</param>
        /// <param name="page">The page of the batch.</param>
        /// <param name="rowsPerBatch">The number of rows per batch.</param>
        /// <param name="orderBy">The list of fields used for ordering.</param>
        /// <returns>A string containing the composed SQL Statement for batch-query operation.</returns>
        public string CreateBatchQuery<TEntity>(QueryBuilder<TEntity> queryBuilder, QueryGroup where = null, int? page = null, int? rowsPerBatch = null, IEnumerable<OrderField> orderBy = null)
            where TEntity : class
        {
            var queryProperties = PropertyCache.Get<TEntity>(Command.Query);
            var batchQueryProperties = PropertyCache.Get<TEntity>(Command.BatchQuery)
                .Where(property => queryProperties.Contains(property));
            var fields = batchQueryProperties.Select(property => new Field(property.GetMappedName()));

            // Validate the fields
            if (fields?.Any() == false)
            {
                throw new InvalidOperationException($"No batch queryable fields found from type '{typeof(TEntity).FullName}'.");
            }

            // Build the SQL Statement
            queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
            queryBuilder
                .Clear()
                .With()
                .WriteText("CTE")
                .As()
                .OpenParen()
                .Select()
                .RowNumber()
                .Over()
                .OpenParen()
                .OrderByFrom(orderBy)
                .CloseParen()
                .As("[RowNumber],")
                .FieldsFrom(fields)
                .From()
                .TableName()
                .WhereFrom(where)
                .CloseParen()
                .Select()
                .FieldsFrom(fields)
                .From()
                .WriteText("CTE")
                .WriteText($"WHERE ([RowNumber] BETWEEN {(page * rowsPerBatch) + 1} AND {(page + 1) * rowsPerBatch})")
                .OrderByFrom(orderBy)
                .End();

            // Return the query
            return queryBuilder.GetString();
        }

        /// <summary>
        /// Creates a SQL Statement for repository count operation that is meant for SQL Server.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The data entity object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="where">The query expression for SQL statement.</param>
        /// <returns>A string containing the composed SQL Statement for count operation.</returns>
        public string CreateCount<TEntity>(QueryBuilder<TEntity> queryBuilder, QueryGroup where = null)
            where TEntity : class
        {
            queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
            queryBuilder
                .Clear()
                .Select()
                .CountBig()
                .WriteText("(1) AS [Counted]")
                .From()
                .TableName()
                .WhereFrom(where)
                .End();
            return queryBuilder.GetString();
        }

        /// <summary>
        /// Creates a SQL Statement for repository delete operation that is meant for SQL Server.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The data entity object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="where">The query expression for SQL statement.</param>
        /// <returns>A string containing the composed SQL Statement for delete operation.</returns>
        public string CreateDelete<TEntity>(QueryBuilder<TEntity> queryBuilder, QueryGroup where = null)
            where TEntity : class
        {
            queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
            queryBuilder
                .Clear()
                .Delete()
                .From()
                .TableName()
                .WhereFrom(where)
                .End();
            return queryBuilder.GetString();
        }

        /// <summary>
        /// Creates a SQL Statement for repository delete-all operation that is meant for SQL Server.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The data entity object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <returns>A string containing the composed SQL Statement for delete-all operation.</returns>
        public string CreateDeleteAll<TEntity>(QueryBuilder<TEntity> queryBuilder)
            where TEntity : class
        {
            queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
            queryBuilder
                .Clear()
                .Delete()
                .From()
                .TableName()
                .End();
            return queryBuilder.GetString();
        }

        /// <summary>
        /// Creates a SQL Statement for repository inline-insert operation.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The data entity object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="fields">The list of the fields to be a part of the inline insert operation in SQL Statement composition.</param>
        /// <param name="overrideIgnore">
        /// Set to true if the defined <see cref="IgnoreAttribute"/> would likely 
        /// be ignored on the inline insert operation in SQL Statement composition.
        /// </param>
        /// <returns>A string containing the composed SQL Statement for inline-insert operation.</returns>
        public string CreateInlineInsert<TEntity>(QueryBuilder<TEntity> queryBuilder, IEnumerable<Field> fields = null, bool? overrideIgnore = false)
            where TEntity : class
        {
            var primary = PrimaryKeyCache.Get<TEntity>();
            var identity = IdentityCache.Get<TEntity>();
            if (identity != null && identity != primary)
            {
                throw new InvalidOperationException($"Identity property must be the primary property for type '{typeof(TEntity).FullName}'.");
            }
            var isPrimaryIdentity = (identity != null) && identity == primary;
            return CreateInlineInsert<TEntity>(queryBuilder, fields, overrideIgnore, isPrimaryIdentity);
        }

        /// <summary>
        /// Creates a SQL Statement for repository inline-insert operation.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The data entity object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="fields">The list of the fields to be a part of the inline insert operation in SQL Statement composition.</param>
        /// <param name="overrideIgnore">
        /// Set to true if the defined <see cref="IgnoreAttribute"/> would likely 
        /// be ignored on the inline insert operation in SQL Statement composition.
        /// </param>
        /// <param name="isPrimaryIdentity">A boolean value indicates whether the primary key is identity in the database.</param>
        /// <returns>A string containing the composed SQL Statement for inline-insert operation.</returns>
        internal string CreateInlineInsert<TEntity>(QueryBuilder<TEntity> queryBuilder, IEnumerable<Field> fields = null,
            bool? overrideIgnore = false, bool isPrimaryIdentity = false)
            where TEntity : class
        {
            // Check for the fields presence
            if (fields == null)
            {
                throw new NullReferenceException("The target fields must be present.");
            }

            // Check for all the fields
            var properties = PropertyCache.Get<TEntity>(Command.None)?
                .Select(property => property.GetMappedName());
            var unmatchesFields = fields?.Where(field =>
                properties?.FirstOrDefault(property =>
                    field.Name.ToLower() == property.ToLower()) == null);
            if (unmatchesFields?.Count() > 0)
            {
                throw new InvalidOperationException($"The fields '{unmatchesFields.Select(field => field.AsField()).Join(", ")}' are not " +
                    $"present at type '{typeof(TEntity).FullName}'.");
            }

            // Variables
            var primary = PrimaryKeyCache.Get<TEntity>();
            var hasFields = isPrimaryIdentity ? fields?.Any(field => field.Name.ToLower() != primary?.GetMappedName().ToLower()) : fields?.Any() == true;

            // Check if there are fields
            if (hasFields == false)
            {
                throw new InvalidOperationException($"No inline insertable fields for object '{ClassMappedNameCache.Get<TEntity>()}'.");
            }

            // Check for the unmatches
            if (overrideIgnore == false)
            {
                var insertableProperties = PropertyCache.Get<TEntity>(Command.Insert)
                    .Select(property => property.GetMappedName()); ;
                var inlineInsertableProperties = PropertyCache.Get<TEntity>(Command.InlineInsert)
                    .Select(property => property.GetMappedName())
                    .Where(property => insertableProperties.Contains(property));
                unmatchesFields = fields?.Where(field =>
                    inlineInsertableProperties?.FirstOrDefault(property =>
                        field.Name.ToLower() == property.ToLower()) == null);
                if (unmatchesFields?.Count() > 0)
                {
                    throw new InvalidOperationException($"The fields '{unmatchesFields.Select(field => field.AsField()).Join(", ")}' are not " +
                        $"inline insertable for object '{ClassMappedNameCache.Get<TEntity>()}'.");
                }
            }

            // Check for the primary key
            if (primary != null && isPrimaryIdentity)
            {
                fields = fields?
                    .Where(field => field.Name.ToLower() != primary.GetMappedName().ToLower())
                        .Select(field => field);
            }

            // Build the SQL Statement
            queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
            queryBuilder
                .Clear()
                .Insert()
                .Into()
                .TableName()
                .OpenParen()
                .FieldsFrom(fields)
                .CloseParen()
                .Values()
                .OpenParen()
                .ParametersFrom(fields)
                .CloseParen()
                .End();
            var result = isPrimaryIdentity ? "SCOPE_IDENTITY()" : (primary != null) ? $"@{primary.GetMappedName()}" : "NULL";
            queryBuilder
                .Select()
                .WriteText(result)
                .As("[Result]")
                .End();

            // Return the query
            return queryBuilder.GetString();
        }

        /// <summary>
        /// Creates a SQL Statement for repository inline-merge operation.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The data entity object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="fields">The list of the fields to be a part of the inline merge operation in SQL Statement composition.</param>
        /// <param name="qualifiers">The list of the qualifier fields to be used by the inline merge operation on a SQL Statement.</param>
        /// <param name="overrideIgnore">
        /// Set to true if the defined <see cref="IgnoreAttribute"/> would likely 
        /// be ignored on the inline merge operation in SQL Statement composition.
        /// </param>
        /// <returns>A string containing the composed SQL Statement for inline-merge operation.</returns>
        public string CreateInlineMerge<TEntity>(QueryBuilder<TEntity> queryBuilder, IEnumerable<Field> fields = null, IEnumerable<Field> qualifiers = null, bool? overrideIgnore = false)
            where TEntity : class
        {
            var primary = PrimaryKeyCache.Get<TEntity>();
            var identity = IdentityCache.Get<TEntity>();
            if (identity != null && identity != primary)
            {
                throw new InvalidOperationException($"Identity property must be the primary property for type '{typeof(TEntity).FullName}'.");
            }
            var isPrimaryIdentity = (identity != null) && identity == primary;
            return CreateInlineMerge<TEntity>(queryBuilder, fields, qualifiers, overrideIgnore, isPrimaryIdentity);
        }

        /// <summary>
        /// Creates a SQL Statement for repository inline-merge operation.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The data entity object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="fields">The list of the fields to be a part of the inline merge operation in SQL Statement composition.</param>
        /// <param name="qualifiers">The list of the qualifier fields to be used by the inline merge operation on a SQL Statement.</param>
        /// <param name="overrideIgnore">
        /// Set to true if the defined <see cref="IgnoreAttribute"/> would likely 
        /// be ignored in the inline merge operation in SQL Statement composition.
        /// </param>
        /// <param name="isPrimaryIdentity">A boolean value indicates whether the primary key is identity in the database.</param>
        /// <returns>A string containing the composed SQL Statement for inline-merge operation.</returns>
        internal string CreateInlineMerge<TEntity>(QueryBuilder<TEntity> queryBuilder, IEnumerable<Field> fields = null, IEnumerable<Field> qualifiers = null,
            bool? overrideIgnore = false, bool? isPrimaryIdentity = false)
            where TEntity : class
        {
            // Variables
            var primary = PrimaryKeyCache.Get<TEntity>();
            var primaryMappedName = primary?.GetMappedName();

            // Check for the fields presence
            if (fields == null)
            {
                throw new NullReferenceException("The target fields must be present.");
            }

            // Check for the qualifiers presence
            if (primary == null && qualifiers == null)
            {
                throw new NullReferenceException("The qualifiers must be present.");
            }

            // Check for all the fields
            var properties = PropertyCache.Get<TEntity>(Command.None)?
                .Select(property => property.GetMappedName());
            var unmatchesFields = fields?.Where(field =>
                properties?.FirstOrDefault(property =>
                    field.Name.ToLower() == property.ToLower()) == null);
            if (unmatchesFields?.Count() > 0)
            {
                throw new InvalidOperationException($"The fields '{unmatchesFields.Select(field => field.AsField()).Join(", ")}' are not " +
                    $"present at type '{typeof(TEntity).FullName}'.");
            }

            // Check for all the qualifiers
            var unmatchesQualifiers = qualifiers?.Where(field =>
                properties?.FirstOrDefault(property =>
                    field.Name.ToLower() == property.ToLower()) == null);
            if (unmatchesQualifiers?.Count() > 0)
            {
                throw new InvalidOperationException($"The qualifiers '{unmatchesQualifiers.Select(field => field.AsField()).Join(", ")}' are not " +
                    $"present at type '{typeof(TEntity).FullName}'.");
            }

            // Check for the unmatches
            if (overrideIgnore == false)
            {
                var mergeableProperties = PropertyCache.Get<TEntity>(Command.Merge)?
                    .Select(property => property.GetMappedName());
                var inlineMergeableProperties = PropertyCache.Get<TEntity>(Command.InlineMerge)?
                    .Select(property => property.GetMappedName())
                    .Where(property => mergeableProperties.Contains(property));
                unmatchesFields = fields?.Where(field =>
                    inlineMergeableProperties?.FirstOrDefault(property => field.Name.ToLower() == property.ToLower()) == null);
                if (unmatchesFields?.Count() > 0)
                {
                    throw new InvalidOperationException($"The fields '{unmatchesFields.Select(field => field.AsField()).Join(", ")}' are not " +
                        $"inline mergeable for object '{ClassMappedNameCache.Get<TEntity>()}'.");
                }
                unmatchesQualifiers = qualifiers?.Where(field =>
                    inlineMergeableProperties?.FirstOrDefault(property => field.Name.ToLower() == property.ToLower()) == null);
                if (unmatchesQualifiers?.Count() > 0)
                {
                    throw new InvalidOperationException($"The qualifiers '{unmatchesQualifiers.Select(field => field.AsField()).Join(", ")}' are not " +
                        $"inline mergeable for object '{ClassMappedNameCache.Get<TEntity>()}'.");
                }
            }

            // Use the primary for qualifiers if there is no any
            if (qualifiers == null && primary != null)
            {
                qualifiers = Field.From(primaryMappedName);
            }

            // Get all target fields
            var insertableFields = PropertyCache.Get<TEntity>(Command.Insert)
                .Select(property => property.GetMappedName())
                .Where(field => !(isPrimaryIdentity == true && field.ToLower() == primaryMappedName?.ToLower()));
            var updateableFields = PropertyCache.Get<TEntity>(Command.Update)
                .Select(property => property.GetMappedName())
                .Where(field => field.ToLower() != primaryMappedName?.ToLower());
            var mergeInsertableFields = fields
                .Where(field => overrideIgnore == true || insertableFields.Contains(field.Name));
            var mergeUpdateableFields = fields
                .Where(field => overrideIgnore == true || updateableFields.Contains(field.Name) &&
                    qualifiers?.FirstOrDefault(qualifier => qualifier.Name.ToLower() == field.Name.ToLower()) == null);

            // Check if there are inline mergeable fields (for insert)
            if (mergeInsertableFields.Any() == false)
            {
                throw new InvalidOperationException($"No inline mergeable fields (for insert) found at type '{typeof(TEntity).FullName}'.");
            }

            // Check if there are inline mergeable fields (for update)
            if (mergeUpdateableFields.Any() == false)
            {
                throw new InvalidOperationException($"No inline mergeable fields (for update) found at type '{typeof(TEntity).FullName}'.");
            }

            // Build the SQL Statement
            queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
            queryBuilder
                .Clear()
                // MERGE T USING S
                .Merge()
                .TableName()
                .As("T")
                .Using()
                .OpenParen()
                .Select()
                .ParametersAsFieldsFrom(fields)
                .CloseParen()
                .As("S")
                // QUALIFIERS
                .On()
                .OpenParen()
                .WriteText(qualifiers?
                    .Select(
                        field => field.AsJoinQualifier("S", "T"))
                            .Join($" {StringConstant.And.ToUpper()} "))
                .CloseParen()
                // WHEN NOT MATCHED THEN INSERT VALUES
                .When()
                .Not()
                .Matched()
                .Then()
                .Insert()
                .OpenParen()
                .FieldsFrom(mergeInsertableFields)
                .CloseParen()
                .Values()
                .OpenParen()
                .AsAliasFieldsFrom(mergeInsertableFields, "S")
                .CloseParen()
                // WHEN MATCHED THEN UPDATE SET
                .When()
                .Matched()
                .Then()
                .Update()
                .Set()
                .FieldsAndAliasFieldsFrom(mergeUpdateableFields, "S")
                .End();

            // Return the query
            return queryBuilder.GetString();
        }

        /// <summary>
        /// Creates a SQL Statement for repository inline update operation that is meant for SQL Server.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The data entity object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="fields">The list of the fields to be a part of inline update operation in SQL Statement composition.</param>
        /// <param name="where">The query expression for SQL statement.</param>
        /// <param name="overrideIgnore">
        /// Set to true if the defined <see cref="IgnoreAttribute"/> would likely 
        /// be ignored on the inline update operation in SQL Statement composition.
        /// </param>
        /// <returns>A string containing the composed SQL Statement for inline-update operation.</returns>
        public string CreateInlineUpdate<TEntity>(QueryBuilder<TEntity> queryBuilder, IEnumerable<Field> fields = null,
            QueryGroup where = null, bool? overrideIgnore = false)
            where TEntity : class
        {
            // Check for the fields presence
            if (fields == null)
            {
                throw new NullReferenceException("The target fields must be present.");
            }

            // Check for all the fields
            var properties = PropertyCache.Get<TEntity>(Command.None)?
                .Select(property => property.GetMappedName());
            var unmatchesFields = fields?.Where(field =>
                properties?.FirstOrDefault(property =>
                    field.Name.ToLower() == property.ToLower()) == null);
            if (unmatchesFields?.Count() > 0)
            {
                throw new InvalidOperationException($"The fields '{unmatchesFields.Select(field => field.AsField()).Join(", ")}' are not " +
                    $"present at type '{typeof(TEntity).FullName}'.");
            }

            // Important fields
            var primary = PrimaryKeyCache.Get<TEntity>();
            var identity = IdentityCache.Get<TEntity>();
            if (identity != null && identity != primary)
            {
                throw new InvalidOperationException($"Identity property must be the primary property for type '{typeof(TEntity).FullName}'.");
            }

            // Variables
            var hasFields = fields?.Any(field => field.Name.ToLower() != primary?.GetMappedName().ToLower()) == true;

            // Check if there are fields
            if (hasFields == false)
            {
                throw new InvalidOperationException($"No inline updatable fields for object '{ClassMappedNameCache.Get<TEntity>()}'.");
            }

            // Append prefix to all parameters
            where?.AppendParametersPrefix();

            // Check for the unmatches
            if (overrideIgnore == false)
            {
                var updateableFields = PropertyCache.Get<TEntity>(Command.Update)
                    .Select(property => property.GetMappedName());
                var inlineUpdateableFields = PropertyCache.Get<TEntity>(Command.InlineUpdate)
                    .Select(property => property.GetMappedName())
                    .Where(field => field.ToLower() != primary?.GetMappedName().ToLower() && updateableFields.Contains(field));
                var unmatchesProperties = fields?.Where(field =>
                    inlineUpdateableFields?.FirstOrDefault(property => field.Name.ToLower() == property.ToLower()) == null);
                if (unmatchesProperties.Count() > 0)
                {
                    throw new InvalidOperationException($"The fields '{unmatchesProperties.Select(field => field.AsField()).Join(", ")}' are not " +
                        $"inline updateable for object '{ClassMappedNameCache.Get<TEntity>()}'.");
                }
            }

            // Build the SQL Statement
            queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
            queryBuilder
                .Clear()
                .Update()
                .TableName()
                .Set()
                .FieldsAndParametersFrom(fields)
                .WhereFrom(where)
                .End();

            // Return the query
            return queryBuilder.GetString();
        }

        /// <summary>
        /// Creates a SQL Statement for repository insert operation that is meant for SQL Server.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The data entity object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <returns>A string containing the composed SQL Statement for insert operation.</returns>
        public string CreateInsert<TEntity>(QueryBuilder<TEntity> queryBuilder)
            where TEntity : class
        {
            var primary = PrimaryKeyCache.Get<TEntity>();
            var identity = IdentityCache.Get<TEntity>();
            if (identity != null && identity != primary)
            {
                throw new InvalidOperationException($"Identity property must be the primary property for type '{typeof(TEntity).FullName}'.");
            }
            var isPrimaryIdentity = (identity != null) && identity == primary;
            return CreateInsert(queryBuilder, isPrimaryIdentity);
        }

        /// <summary>
        /// Creates a SQL Statement for repository insert operation that is meant for SQL Server.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The data entity object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="isPrimaryIdentity">A boolean value indicates whether the primary key is identity in the database.</param>
        /// <returns>A string containing the composed SQL Statement for insert operation.</returns>
        internal string CreateInsert<TEntity>(QueryBuilder<TEntity> queryBuilder, bool? isPrimaryIdentity = null)
            where TEntity : class
        {
            var primary = PrimaryKeyCache.Get<TEntity>();
            var fields = PropertyCache.Get<TEntity>(Command.Insert)
                .Where(property => !(isPrimaryIdentity == true && property.IsPrimary() == true))
                .Select(property => new Field(property.GetMappedName()));

            // Build the SQL Statement
            queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
            queryBuilder
                .Clear()
                .Insert()
                .Into()
                .TableName()
                .OpenParen()
                .FieldsFrom(fields)
                .CloseParen()
                .Values()
                .OpenParen()
                .ParametersFrom(fields)
                .CloseParen()
                .End();
            var result = isPrimaryIdentity == true ? "SCOPE_IDENTITY()" : (primary != null) ? $"@{primary.GetMappedName()}" : "NULL";
            queryBuilder
                .Select()
                .WriteText(result)
                .As("[Result]")
                .End();

            // Return the query
            return queryBuilder.GetString();
        }

        /// <summary>
        /// Creates a SQL Statement for repository merge operation that is meant for SQL Server.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The data entity object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="qualifiers">The list of qualifier fields to be used for the merge operation in SQL Statement composition.</param>
        /// <returns>A string containing the composed SQL Statement for merge operation.</returns>
        public string CreateMerge<TEntity>(QueryBuilder<TEntity> queryBuilder, IEnumerable<Field> qualifiers = null)
            where TEntity : class
        {
            var primary = PrimaryKeyCache.Get<TEntity>();
            var identity = IdentityCache.Get<TEntity>();
            if (identity != null && identity != primary)
            {
                throw new InvalidOperationException($"Identity property must be the primary property for type '{typeof(TEntity).FullName}'.");
            }
            var isPrimaryIdentity = (identity != null) && identity == primary;
            return CreateMerge(queryBuilder, qualifiers, isPrimaryIdentity);
        }

        /// <summary>
        /// Creates a SQL Statement for repository merge operation that is meant for SQL Server.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The data entity object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="qualifiers">The list of qualifier fields to be used for the merge operation in SQL Statement composition.</param>
        /// <param name="isPrimaryIdentity">A boolean value indicates whether the primary key is identity in the database.</param>
        /// <returns>A string containing the composed SQL Statement for merge operation.</returns>
        internal string CreateMerge<TEntity>(QueryBuilder<TEntity> queryBuilder, IEnumerable<Field> qualifiers, bool isPrimaryIdentity)
            where TEntity : class
        {
            // Check for all the fields
            var properties = PropertyCache.Get<TEntity>(Command.Merge)?
                .Select(property => property.GetMappedName());
            var unmatchesQualifiers = qualifiers?.Where(field =>
                properties?.FirstOrDefault(property =>
                    field.Name.ToLower() == property.ToLower()) == null);
            if (unmatchesQualifiers?.Count() > 0)
            {
                throw new InvalidOperationException($"The qualifiers '{unmatchesQualifiers.Select(field => field.AsField()).Join(", ")}' are not " +
                    $"present at type '{typeof(TEntity).FullName}'.");
            }

            // Variables
            var primary = PrimaryKeyCache.Get<TEntity>();
            var primaryKeyName = primary?.GetMappedName();

            // Add the primary key as the default qualifier
            if (qualifiers == null && primary != null)
            {
                qualifiers = Field.From(primaryKeyName);
            }

            // Throw an exception if there is no qualifiers defined
            if (qualifiers == null || qualifiers?.Any() == false)
            {
                throw new InvalidOperationException("There are no qualifier fields defined.");
            }

            // Get the target properties
            var insertableFields = PropertyCache.Get<TEntity>(Command.Insert)
                .Select(property => property.GetMappedName())
                .Where(field => !(isPrimaryIdentity && field.ToLower() == primaryKeyName?.ToLower()));
            var updateableFields = PropertyCache.Get<TEntity>(Command.Update)
                .Select(property => property.GetMappedName())
                .Where(field => field.ToLower() != primaryKeyName?.ToLower());
            var mergeableFields = PropertyCache.Get<TEntity>(Command.Merge)
                .Select(property => property.GetMappedName());
            var mergeInsertableFields = mergeableFields
                .Where(field => insertableFields.Contains(field))
                .Select(field => new Field(field));
            var mergeUpdateableFields = mergeableFields
                .Where(field => updateableFields.Contains(field) &&
                    qualifiers?.FirstOrDefault(qualifier => qualifier.Name.ToLower() == field.ToLower()) == null)
                .Select(field => new Field(field));

            // Build the SQL Statement
            queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
            queryBuilder
                .Clear()
                // MERGE T USING S
                .Merge()
                .TableName()
                .As("T")
                .Using()
                .OpenParen()
                .Select()
                .ParametersAsFieldsFrom(Command.Merge)
                .CloseParen()
                .As("S")
                // QUALIFIERS
                .On()
                .OpenParen()
                .WriteText(qualifiers?
                    .Select(
                        field => field.AsJoinQualifier("S", "T"))
                            .Join($" {StringConstant.And.ToUpper()} "))
                .CloseParen()
                // WHEN NOT MATCHED THEN INSERT VALUES
                .When()
                .Not()
                .Matched()
                .Then()
                .Insert()
                .OpenParen()
                .FieldsFrom(mergeInsertableFields)
                .CloseParen()
                .Values()
                .OpenParen()
                .AsAliasFieldsFrom(mergeInsertableFields, "S")
                .CloseParen()
                // WHEN MATCHED THEN UPDATE SET
                .When()
                .Matched()
                .Then()
                .Update()
                .Set()
                .FieldsAndAliasFieldsFrom(mergeUpdateableFields, "S")
                .End();

            // Return the query
            return queryBuilder.GetString();
        }

        /// <summary>
        /// Creates a SQL Statement for repository query operation that is meant for SQL Server.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The data entity object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="where">The query expression for SQL statement.</param>
        /// <param name="orderBy">The list of fields  to be used for ordering in SQL Statement composition.</param>
        /// <param name="top">The number of rows to be returned by the query operation in SQL Statement composition.</param>
        /// <returns>A string containing the composed SQL Statement for query operation.</returns>
        public string CreateQuery<TEntity>(QueryBuilder<TEntity> queryBuilder, QueryGroup where = null, IEnumerable<OrderField> orderBy = null, int? top = null)
            where TEntity : class
        {
            var properties = PropertyCache.Get<TEntity>(Command.Query);
            if (properties?.Any() == false)
            {
                throw new InvalidOperationException($"No queryable fields found from type '{typeof(TEntity).FullName}'.");
            }
            var fields = properties?.Select(property => new Field(property.GetMappedName().AsQuoted(true)));
            queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
            queryBuilder
                .Clear()
                .Select()
                .TopFrom(top)
                .FieldsFrom(fields)
                .From()
                .TableName()
                .WhereFrom(where)
                .OrderByFrom(orderBy)
                .End();
            return queryBuilder.GetString();
        }

        /// <summary>
        /// Creates a SQL Statement for repository truncate operation that is meant for SQL Server.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The data entity object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <returns>A string containing the composed SQL Statement for truncate operation.</returns>
        public string CreateTruncate<TEntity>(QueryBuilder<TEntity> queryBuilder)
            where TEntity : class
        {
            queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
            queryBuilder
                .Clear()
                .Truncate()
                .Table()
                .TableName()
                .End();
            return queryBuilder.GetString();
        }

        /// <summary>
        /// Creates a SQL Statement for repository update operation that is meant for SQL Server.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The data entity object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="where">The query expression for SQL statement.</param>
        /// <returns>A string containing the composed SQL Statement for update operation.</returns>
        public string CreateUpdate<TEntity>(QueryBuilder<TEntity> queryBuilder, QueryGroup where = null)
            where TEntity : class
        {
            var properties = PropertyCache.Get<TEntity>(Command.Update);
            if (properties?.Any() == false)
            {
                throw new InvalidOperationException($"No updateable fields found from type '{typeof(TEntity).FullName}'.");
            }
            var primary = PrimaryKeyCache.Get<TEntity>();
            var identity = IdentityCache.Get<TEntity>();
            if (identity != null && identity != primary)
            {
                throw new InvalidOperationException($"Identity property must be the primary property for type '{typeof(TEntity).FullName}'.");
            }
            var fields = properties
                .Where(property => property.IsPrimary() == false && property.IsIdentity() == false)
                .Select(p => new Field(p.GetMappedName()));
            where?.AppendParametersPrefix();
            queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
            queryBuilder
                .Clear()
                .Update()
                .TableName()
                .Set()
                .FieldsAndParametersFrom(fields)
                .WhereFrom(where)
                .End();
            return queryBuilder.GetString();
        }
    }
}
