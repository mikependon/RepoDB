using RepoDb.Extensions;
using RepoDb.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System;

namespace RepoDb
{
    /// <summary>
    /// A class used to build a SQL Statement for SQL Server. This is the default statement builder used by the library.
    /// </summary>
    public sealed class SqlStatementBuilder : IStatementBuilder
    {
        /// <summary>
        /// Creates a new instance of <see cref="SqlStatementBuilder"/> object.
        /// </summary>
        public SqlStatementBuilder() { }

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
        public string CreateBatchQuery<TEntity>(QueryBuilder<TEntity> queryBuilder,
            QueryGroup where = null,
            int? page = null,
            int? rowsPerBatch = null,
            IEnumerable<OrderField> orderBy = null)
            where TEntity : class
        {
            var fields = PropertyCache.Get<TEntity>().Select(property => new Field(property.GetMappedName()));

            // There should be fields
            if (fields == null || fields?.Any() == false)
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
                .WriteText(string.Concat("WHERE ([RowNumber] BETWEEN ", (page * rowsPerBatch) + 1, " AND ", (page + 1) * rowsPerBatch, ")"))
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
        public string CreateCount<TEntity>(QueryBuilder<TEntity> queryBuilder,
            QueryGroup where = null)
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
        public string CreateDelete<TEntity>(QueryBuilder<TEntity> queryBuilder,
            QueryGroup where = null)
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
        /// <returns>A string containing the composed SQL Statement for inline-insert operation.</returns>
        public string CreateInlineInsert<TEntity>(QueryBuilder<TEntity> queryBuilder,
            IEnumerable<Field> fields = null)
            where TEntity : class
        {
            var primary = PrimaryKeyCache.Get<TEntity>();
            var identity = IdentityCache.Get<TEntity>();
            if (identity != null && identity != primary)
            {
                throw new InvalidOperationException($"Identity property must be the primary property for type '{typeof(TEntity).FullName}'.");
            }
            var isPrimaryIdentity = (identity != null) && identity == primary;
            return CreateInlineInsert<TEntity>(queryBuilder, fields, isPrimaryIdentity);
        }

        /// <summary>
        /// Creates a SQL Statement for repository inline-insert operation.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The data entity object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="fields">The list of the fields to be a part of the inline insert operation in SQL Statement composition.</param>
        /// <param name="isPrimaryIdentity">A boolean value indicates whether the primary key is identity in the database.</param>
        /// <returns>A string containing the composed SQL Statement for inline-insert operation.</returns>
        internal string CreateInlineInsert<TEntity>(QueryBuilder<TEntity> queryBuilder,
            IEnumerable<Field> fields = null,
            bool isPrimaryIdentity = false)
            where TEntity : class
        {
            // Check for the fields presence
            if (fields == null)
            {
                throw new NullReferenceException("The target fields must be present.");
            }

            // Check for all the fields
            var properties = PropertyCache.Get<TEntity>()?
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
            var hasFields = isPrimaryIdentity ? fields?.Any(field => field.Name.ToLower() != primary?.GetMappedName().ToLower()) : fields?.Any();

            // Check if there are fields
            if (hasFields == false)
            {
                throw new InvalidOperationException($"No inline insertable fields for object '{ClassMappedNameCache.Get<TEntity>()}'.");
            }

            // Check for the primary key
            if (primary != null && isPrimaryIdentity)
            {
                fields = fields?
                    .Where(field => field.Name.ToLower() != primary.GetMappedName().ToLower());
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
            var result = isPrimaryIdentity ? "SCOPE_IDENTITY()" : (primary != null) ? string.Concat("@", primary.GetMappedName()) : "NULL";
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
        /// <returns>A string containing the composed SQL Statement for inline-merge operation.</returns>
        public string CreateInlineMerge<TEntity>(QueryBuilder<TEntity> queryBuilder,
            IEnumerable<Field> fields = null,
            IEnumerable<Field> qualifiers = null)
            where TEntity : class
        {
            var primary = PrimaryKeyCache.Get<TEntity>();
            var identity = IdentityCache.Get<TEntity>();
            if (identity != null && identity != primary)
            {
                throw new InvalidOperationException($"Identity property must be the primary property for type '{typeof(TEntity).FullName}'.");
            }
            var isPrimaryIdentity = (identity != null) && identity == primary;
            return CreateInlineMerge<TEntity>(queryBuilder, fields, qualifiers, isPrimaryIdentity);
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
        /// <param name="isPrimaryIdentity">A boolean value indicates whether the primary key is identity in the database.</param>
        /// <returns>A string containing the composed SQL Statement for inline-merge operation.</returns>
        internal string CreateInlineMerge<TEntity>(QueryBuilder<TEntity> queryBuilder,
            IEnumerable<Field> fields = null,
            IEnumerable<Field> qualifiers = null,
            bool? isPrimaryIdentity = false)
            where TEntity : class
        {
            // Variables
            var primary = PrimaryKeyCache.Get<TEntity>();

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
            var properties = PropertyCache.Get<TEntity>()?
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

            // Use the primary for qualifiers if there is no any
            if (qualifiers == null && primary != null)
            {
                qualifiers = Field.From(primary.GetMappedName());
            }

            // Get the updateable fields
            var updateableFields = fields
                .Where(field => qualifiers.Any(f => f == field) == false);

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
                .FieldsFrom(fields)
                .CloseParen()
                .Values()
                .OpenParen()
                .AsAliasFieldsFrom(fields, "S")
                .CloseParen()
                // WHEN MATCHED THEN UPDATE SET
                .When()
                .Matched()
                .Then()
                .Update()
                .Set()
                .FieldsAndAliasFieldsFrom(updateableFields, "S")
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
        /// <returns>A string containing the composed SQL Statement for inline-update operation.</returns>
        public string CreateInlineUpdate<TEntity>(QueryBuilder<TEntity> queryBuilder,
            IEnumerable<Field> fields = null,
            QueryGroup where = null)
            where TEntity : class
        {
            // Check for the fields presence
            if (fields == null)
            {
                throw new NullReferenceException("The target fields must be present.");
            }

            // Check for all the fields
            var properties = PropertyCache.Get<TEntity>()?
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

            // Check if there are fields
            var hasFields = fields?.Any(field => field.Name.ToLower() != primary?.GetMappedName().ToLower()) == true;
            if (hasFields == false)
            {
                throw new InvalidOperationException($"No inline updatable fields for object '{ClassMappedNameCache.Get<TEntity>()}'.");
            }

            // Make sure the primary key is not being updated
            if (fields.Any(field => field.Name.ToLower() == primary?.GetMappedName().ToLower()))
            {
                throw new InvalidOperationException("Primary column is not inline-updateable.");
            }

            // Append prefix to all parameters
            where?.AppendParametersPrefix();

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
        internal string CreateInsert<TEntity>(QueryBuilder<TEntity> queryBuilder,
            bool? isPrimaryIdentity = null)
            where TEntity : class
        {
            var primary = PrimaryKeyCache.Get<TEntity>();
            var fields = PropertyCache.Get<TEntity>()
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
            var result = isPrimaryIdentity == true ? "SCOPE_IDENTITY()" : (primary != null) ? string.Concat("@", primary.GetMappedName()) : "NULL";
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
        public string CreateMerge<TEntity>(QueryBuilder<TEntity> queryBuilder,
            IEnumerable<Field> qualifiers = null)
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
        internal string CreateMerge<TEntity>(QueryBuilder<TEntity> queryBuilder,
            IEnumerable<Field> qualifiers,
            bool isPrimaryIdentity)
            where TEntity : class
        {
            // Check for all the fields
            var fields = PropertyCache.Get<TEntity>()?
                .Select(property => new Field(property.GetMappedName()));
            var unmatchesQualifiers = qualifiers?.Where(field =>
                fields?.FirstOrDefault(f =>
                    field == f) == null);
            if (unmatchesQualifiers?.Count() > 0)
            {
                throw new InvalidOperationException($"The qualifiers '{unmatchesQualifiers.Select(field => field.AsField()).Join(", ")}' are not " +
                    $"present at type '{typeof(TEntity).FullName}'.");
            }

            // Variables
            var primary = PrimaryKeyCache.Get<TEntity>();

            // Add the primary key as the default qualifier
            if (qualifiers == null && primary != null)
            {
                qualifiers = Field.From(primary.GetMappedName());
            }

            // Throw an exception if there is no qualifiers defined
            if (qualifiers == null || qualifiers == null)
            {
                throw new InvalidOperationException("There are no qualifier fields defined.");
            }

            // Get the target properties
            var insertableFields = fields
                .Where(field => !(primary?.GetMappedName().ToLower() == field.Name.ToLower() && primary?.IsIdentity() == true));
            var updateableFields = fields
                .Where(field => field.Name.ToLower() != primary?.GetMappedName().ToLower() && qualifiers.Any(f => f == field) == false);

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
                .ParametersAsFieldsFrom()
                .CloseParen()
                .As("S")
                // QUALIFIERS
                .On()
                .OpenParen()
                .WriteText(qualifiers?
                    .Select(
                        field => field.AsJoinQualifier("S", "T"))
                            .Join(" AND "))
                .CloseParen()
                // WHEN NOT MATCHED THEN INSERT VALUES
                .When()
                .Not()
                .Matched()
                .Then()
                .Insert()
                .OpenParen()
                .FieldsFrom(insertableFields)
                .CloseParen()
                .Values()
                .OpenParen()
                .AsAliasFieldsFrom(insertableFields, "S")
                .CloseParen()
                // WHEN MATCHED THEN UPDATE SET
                .When()
                .Matched()
                .Then()
                .Update()
                .Set()
                .FieldsAndAliasFieldsFrom(updateableFields, "S")
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
        public string CreateQuery<TEntity>(QueryBuilder<TEntity> queryBuilder,
            QueryGroup where = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = null)
            where TEntity : class
        {
            var fields = PropertyCache.Get<TEntity>()?.Select(property => new Field(property.GetMappedName().AsQuoted(true)));

            // There should be fields
            if (fields == null || fields.Any() == false)
            {
                throw new InvalidOperationException($"There are no fields found for type '{typeof(TEntity).Name}'.");
            }

            // Build the SQL Statement
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

            // Return the query
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

            // Build the SQL Statement
            queryBuilder
                .Clear()
                .Truncate()
                .Table()
                .TableName()
                .End();

            // Return the query
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
        public string CreateUpdate<TEntity>(QueryBuilder<TEntity> queryBuilder,
            QueryGroup where = null)
            where TEntity : class
        {
            var properties = PropertyCache.Get<TEntity>();
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
    }
}
