using RepoDb.Enumerations;
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
    public sealed class SqlDbStatementBuilder : IStatementBuilder
    {
        /// <summary>
        /// Creates a new instance of <i>RepoDb.SqlDbStatementBuilder</i> object.
        /// </summary>
        public SqlDbStatementBuilder() { }

        /// <summary>
        /// Creates a SQL Statement for repository <i>BatchQuery</i> operation that is meant for SQL Server.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The <i>DataEntity</i> object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="where">The query expression for SQL statement.</param>
        /// <param name="page">The page of the batch.</param>
        /// <param name="rowsPerBatch">The number of rows per batch.</param>
        /// <param name="orderBy">The list of fields used for ordering.</param>
        /// <returns>A string containing the composed SQL Statement for <i>BatchQuery</i> operation.</returns>
        public string CreateBatchQuery<TEntity>(QueryBuilder<TEntity> queryBuilder, QueryGroup where, int page, int rowsPerBatch, IEnumerable<OrderField> orderBy)
            where TEntity : DataEntity
        {
            var queryProperties = DataEntityExtension.GetPropertiesFor<TEntity>(Command.Query);
            var batchQueryProperties = DataEntityExtension.GetPropertiesFor<TEntity>(Command.BatchQuery)
                .Where(property => queryProperties.Contains(property));
            var fields = batchQueryProperties.Select(property => new Field(property.Name));

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
                .OrderBy(orderBy)
                .CloseParen()
                .As("[RowNumber],")
                .Fields(Command.BatchQuery)
                .From()
                .Table(Command.BatchQuery)
                .Where(where)
                .CloseParen()
                .Select()
                .Fields(fields)
                .From()
                .WriteText("CTE")
                .WriteText($"WHERE ([RowNumber] BETWEEN {(page * rowsPerBatch) + 1} AND {(page + 1) * rowsPerBatch})")
                .OrderBy(orderBy)
                .End();

            // Return the query
            return queryBuilder.GetString();
        }

        /// <summary>
        /// Creates a SQL Statement for repository <i>Count</i> operation that is meant for SQL Server.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The <i>DataEntity</i> object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="where">The query expression for SQL statement.</param>
        /// <returns>A string containing the composed SQL Statement for <i>Count</i> operation.</returns>
        public string CreateCount<TEntity>(QueryBuilder<TEntity> queryBuilder, QueryGroup where)
            where TEntity : DataEntity
        {
            queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
            queryBuilder
                .Clear()
                .Select()
                .CountBig()
                .WriteText("(1) AS [Counted]")
                .From()
                .Table(Command.Count)
                .Where(where)
                .End();
            return queryBuilder.GetString();
        }

        /// <summary>
        /// Creates a SQL Statement for repository <i>Delete</i> operation that is meant for SQL Server.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The <i>DataEntity</i> object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="where">The query expression for SQL statement.</param>
        /// <returns>A string containing the composed SQL Statement for <i>Delete</i> operation.</returns>
        public string CreateDelete<TEntity>(QueryBuilder<TEntity> queryBuilder, QueryGroup where)
            where TEntity : DataEntity
        {
            queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
            queryBuilder
                .Clear()
                .Delete()
                .From()
                .Table(Command.Delete)
                .Where(where)
                .End();
            return queryBuilder.GetString();
        }

        /// <summary>
        /// Creates a SQL Statement for repository <i>DeleteAll</i> operation that is meant for SQL Server.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The <i>DataEntity</i> object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <returns>A string containing the composed SQL Statement for <i>DeleteAll</i> operation.</returns>
        public string CreateDeleteAll<TEntity>(QueryBuilder<TEntity> queryBuilder)
            where TEntity : DataEntity
        {
            queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
            queryBuilder
                .Clear()
                .Delete()
                .From()
                .Table(Command.DeleteAll)
                .End();
            return queryBuilder.GetString();
        }

        /// <summary>
        /// Creates a SQL Statement for repository <i>InlineInsert</i> operation.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The <i>DataEntity</i> object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="fields">The list of the fields to be a part of the inline insert operation in SQL Statement composition.</param>
        /// <param name="overrideIgnore">
        /// Set to <i>true</i> if the defined <i>RepoDb.Attributes.IgnoreAttribute</i> would likely 
        /// be ignored on the inline insert operation in SQL Statement composition.
        /// </param>
        /// <returns>A string containing the composed SQL Statement for <i>InlineInsert</i> operation.</returns>
        public string CreateInlineInsert<TEntity>(QueryBuilder<TEntity> queryBuilder, IEnumerable<Field> fields, bool? overrideIgnore = false)
            where TEntity : DataEntity
        {
            return CreateInlineInsert<TEntity>(queryBuilder, fields, overrideIgnore, false);
        }

        /// <summary>
        /// Creates a SQL Statement for repository <i>InlineInsert</i> operation.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The <i>DataEntity</i> object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="fields">The list of the fields to be a part of the inline insert operation in SQL Statement composition.</param>
        /// <param name="overrideIgnore">
        /// Set to <i>true</i> if the defined <i>RepoDb.Attributes.IgnoreAttribute</i> would likely 
        /// be ignored on the inline insert operation in SQL Statement composition.
        /// </param>
        /// <param name="isPrimaryIdentity">A boolean value indicates whether the primary key is identity in the database.</param>
        /// <returns>A string containing the composed SQL Statement for <i>InlineInsert</i> operation.</returns>
        internal string CreateInlineInsert<TEntity>(QueryBuilder<TEntity> queryBuilder, IEnumerable<Field> fields,
            bool? overrideIgnore = false, bool isPrimaryIdentity = false)
            where TEntity : DataEntity
        {
            var primary = DataEntityExtension.GetPrimaryProperty<TEntity>();
            var hasFields = fields?.Any(field => field.Name.ToLower() != primary?.GetMappedName().ToLower());

            // Check if there are fields
            if (hasFields == false)
            {
                throw new InvalidOperationException($"No inline insertable fields found at type '{typeof(TEntity).FullName}'.");
            }

            // Check for the unmatches
            if (overrideIgnore == false)
            {
                var insertableProperties = DataEntityExtension.GetPropertiesFor<TEntity>(Command.Insert);
                var inlineInsertableProperties = DataEntityExtension.GetPropertiesFor<TEntity>(Command.InlineInsert)
                    .Where(property => property != primary && insertableProperties.Contains(property))
                    .Select(property => property.GetMappedName());
                var unmatchesProperties = fields?.Where(field =>
                    inlineInsertableProperties?.FirstOrDefault(property =>
                        field.Name.ToLower() == property.ToLower()) == null);
                if (unmatchesProperties.Count() > 0)
                {
                    throw new InvalidOperationException($"The fields '{unmatchesProperties.Select(field => field.AsField()).Join(", ")}' are not " +
                        $"inline insertable for object '{DataEntityExtension.GetMappedName<TEntity>(Command.InlineInsert)}'.");
                }
            }
            if (primary != null)
            {
                fields = fields?
                    .Where(field => !(isPrimaryIdentity && field.Name.ToLower() == primary.Name.ToLower()))
                        .Select(field => field);
            }

            // Build the SQL Statement
            queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
            queryBuilder
                .Clear()
                .Insert()
                .Into()
                .Table(Command.Insert)
                .OpenParen()
                .Fields(fields)
                .CloseParen()
                .Values()
                .OpenParen()
                .Parameters(fields)
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
        /// Creates a SQL Statement for repository <i>InlineMerge</i> operation.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The <i>DataEntity</i> object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="fields">The list of the fields to be a part of the inline merge operation in SQL Statement composition.</param>
        /// <param name="qualifiers">The list of the qualifier fields to be used by the inline merge operation on a SQL Statement.</param>
        /// <param name="overrideIgnore">
        /// Set to <i>true</i> if the defined <i>RepoDb.Attributes.IgnoreAttribute</i> would likely 
        /// be ignored on the inline merge operation in SQL Statement composition.
        /// </param>
        /// <returns>A string containing the composed SQL Statement for <i>InlineMerge</i> operation.</returns>
        public string CreateInlineMerge<TEntity>(QueryBuilder<TEntity> queryBuilder, IEnumerable<Field> fields, IEnumerable<Field> qualifiers, bool? overrideIgnore = false)
            where TEntity : DataEntity
        {
            return CreateInlineMerge<TEntity>(queryBuilder, fields, qualifiers, overrideIgnore, false);
        }

        /// <summary>
        /// Creates a SQL Statement for repository <i>InlineMerge</i> operation.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The <i>DataEntity</i> object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="fields">The list of the fields to be a part of the inline merge operation in SQL Statement composition.</param>
        /// <param name="qualifiers">The list of the qualifier fields to be used by the inline merge operation on a SQL Statement.</param>
        /// <param name="overrideIgnore">
        /// Set to <i>true</i> if the defined <i>RepoDb.Attributes.IgnoreAttribute</i> would likely 
        /// be ignored in the inline merge operation in SQL Statement composition.
        /// </param>
        /// <param name="isPrimaryIdentity">A boolean value indicates whether the primary key is identity in the database.</param>
        /// <returns>A string containing the composed SQL Statement for <i>InlineMerge</i> operation.</returns>
        internal string CreateInlineMerge<TEntity>(QueryBuilder<TEntity> queryBuilder, IEnumerable<Field> fields, IEnumerable<Field> qualifiers,
            bool? overrideIgnore = false, bool isPrimaryIdentity = false)
            where TEntity : DataEntity
        {
            var primary = DataEntityExtension.GetPrimaryProperty<TEntity>();

            // Get all target properties
            var mergeableProperties = DataEntityExtension.GetPropertiesFor<TEntity>(Command.Merge);
            var inlineMergeableProperties = DataEntityExtension.GetPropertiesFor<TEntity>(Command.InlineMerge)
                .Where(property => property != primary && mergeableProperties.Contains(property));

            // Check for the unmatches
            if (overrideIgnore == false)
            {
                var unmatchesProperties = fields?.Where(field =>
                    inlineMergeableProperties?.FirstOrDefault(property => field.Name.ToLower() == property.GetMappedName().ToLower()) == null)
                        .Where(property => property.Name.ToLower() != primary?.GetMappedName().ToLower());
                if (unmatchesProperties.Count() > 0)
                {
                    throw new InvalidOperationException($"The fields '{unmatchesProperties.Select(field => field.AsField()).Join(", ")}' are not " +
                        $"inline mergeable for object '{DataEntityExtension.GetMappedName<TEntity>(Command.InlineMerge)}'.");
                }
            }

            // Use the primary for qualifiers if there is no any
            if (qualifiers == null && primary != null)
            {
                qualifiers = Field.From(primary.GetMappedName());
            }

            // Get all target fields
            var insertableProperties = DataEntityExtension.GetPropertiesFor<TEntity>(Command.Insert)
                .Where(property => !(isPrimaryIdentity && property == primary));
            var updateableProperties = DataEntityExtension.GetPropertiesFor<TEntity>(Command.Update)
                .Where(property => property != primary);
            var mergeInsertableFields = mergeableProperties
                .Where(property => insertableProperties.Contains(property) &&
                    mergeableProperties.Contains(property) &&
                        inlineMergeableProperties.Contains(property))
                .Where(property =>
                    fields.Any(field => field.Name.ToLower() == property.GetMappedName().ToLower()))
                .Select(property => new Field(property.Name));
            var mergeUpdateableFields = mergeableProperties
                .Where(property => updateableProperties.Contains(property) &&
                    mergeableProperties.Contains(property) &&
                        inlineMergeableProperties.Contains(property))
                .Where(property =>
                    fields.Any(field => field.Name.ToLower() == property.GetMappedName().ToLower()))
                .Select(property => new Field(property.Name));

            // Check if there are inline mergeable fields (for insert)
            if (!mergeInsertableFields.Any())
            {
                throw new InvalidOperationException($"No inline mergeable fields (for insert) found at type '{typeof(TEntity).FullName}'.");
            }

            // Check if there are inline mergeable fields (for update)
            if (!mergeUpdateableFields.Any())
            {
                throw new InvalidOperationException($"No inline mergeable fields (for update) found at type '{typeof(TEntity).FullName}'.");
            }

            // Build the SQL Statement
            queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
            queryBuilder
                .Clear()
                // MERGE T USING S
                .Merge()
                .Table(Command.Merge)
                .As("T")
                .Using()
                .OpenParen()
                .Select()
                .ParametersAsFields(fields)
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
                .Fields(mergeInsertableFields)
                .CloseParen()
                .Values()
                .OpenParen()
                .Parameters(mergeInsertableFields)
                .CloseParen()
                // WHEN MATCHED THEN UPDATE SET
                .When()
                .Matched()
                .Then()
                .Update()
                .Set()
                .FieldsAndAliasFields(mergeUpdateableFields, "S")
                .End();

            // Return the query
            return queryBuilder.GetString();
        }

        /// <summary>
        /// Creates a SQL Statement for repository <i>InlineUpdate</i> operation that is meant for SQL Server.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The <i>DataEntity</i> object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="fields">The list of the fields to be a part of inline update operation in SQL Statement composition.</param>
        /// <param name="where">The query expression for SQL statement.</param>
        /// <param name="overrideIgnore">
        /// Set to <i>true</i> if the defined <i>RepoDb.Attributes.IgnoreAttribute</i> would likely 
        /// be ignored on the inline update operation in SQL Statement composition.
        /// </param>
        /// <returns>A string containing the composed SQL Statement for <i>InlineUpdate</i> operation.</returns>
        public string CreateInlineUpdate<TEntity>(QueryBuilder<TEntity> queryBuilder, IEnumerable<Field> fields,
            QueryGroup where, bool? overrideIgnore = false)
            where TEntity : DataEntity
        {
            var primary = DataEntityExtension.GetPrimaryProperty<TEntity>();
            var hasFields = fields?.Any(field => field.Name.ToLower() != primary?.GetMappedName().ToLower());

            // Check if there are fields
            if (hasFields == false)
            {
                throw new InvalidOperationException($"No inline updatable fields found at type '{typeof(TEntity).FullName}'.");
            }

            // Get the target properties
            var updateableProperties = DataEntityExtension.GetPropertiesFor<TEntity>(Command.Update);
            var inlineUpdateableProperties = DataEntityExtension.GetPropertiesFor<TEntity>(Command.InlineUpdate)
                .Where(property => property != DataEntityExtension.GetPrimaryProperty<TEntity>() &&
                    updateableProperties.Contains(property))
                .Select(property => property.GetMappedName());

            // Check for the unmatches
            if (overrideIgnore == false)
            {
                var unmatchesProperties = fields?.Where(field =>
                    inlineUpdateableProperties?.FirstOrDefault(property =>
                        field.Name.ToLower() == property.ToLower()) == null);
                if (unmatchesProperties.Count() > 0)
                {
                    throw new InvalidOperationException($"The fields '{unmatchesProperties.Select(field => field.AsField()).Join(", ")}' are not " +
                        $"inline updateable for object '{DataEntityExtension.GetMappedName<TEntity>(Command.InlineUpdate)}'.");
                }
            }

            // Build the SQL Statement
            queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
            queryBuilder
                .Clear()
                .Update()
                .Table(Command.InlineUpdate)
                .Set()
                .FieldsAndParameters(fields)
                .Where(where)
                .End();

            // Return the query
            return queryBuilder.GetString();
        }

        /// <summary>
        /// Creates a SQL Statement for repository <i>Insert</i> operation that is meant for SQL Server.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The <i>DataEntity</i> object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <returns>A string containing the composed SQL Statement for <i>Insert</i> operation.</returns>
        public string CreateInsert<TEntity>(QueryBuilder<TEntity> queryBuilder)
            where TEntity : DataEntity
        {
            return CreateInsert(queryBuilder, false);
        }

        /// <summary>
        /// Creates a SQL Statement for repository <i>Insert</i> operation that is meant for SQL Server.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The <i>DataEntity</i> object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="isPrimaryIdentity">A boolean value indicates whether the primary key is identity in the database.</param>
        /// <returns>A string containing the composed SQL Statement for <i>Insert</i> operation.</returns>
        internal string CreateInsert<TEntity>(QueryBuilder<TEntity> queryBuilder, bool isPrimaryIdentity)
            where TEntity : DataEntity
        {
            var primary = DataEntityExtension.GetPrimaryProperty<TEntity>();
            var fields = DataEntityExtension.GetPropertiesFor<TEntity>(Command.Insert)
                .Where(property => !(isPrimaryIdentity && property == primary))
                .Select(p => new Field(p.Name));

            // Build the SQL Statement
            queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
            queryBuilder
                .Clear()
                .Insert()
                .Into()
                .Table(Command.Insert)
                .OpenParen()
                .Fields(fields)
                .CloseParen()
                .Values()
                .OpenParen()
                .Parameters(fields)
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
        /// Creates a SQL Statement for repository <i>Merge</i> operation that is meant for SQL Server.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The <i>DataEntity</i> object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="qualifiers">The list of qualifier fields to be used for the <i>Merge</i> operation in SQL Statement composition.</param>
        /// <returns>A string containing the composed SQL Statement for <i>Merge</i> operation.</returns>
        public string CreateMerge<TEntity>(QueryBuilder<TEntity> queryBuilder, IEnumerable<Field> qualifiers)
            where TEntity : DataEntity
        {
            return CreateMerge(queryBuilder, qualifiers);
        }

        /// <summary>
        /// Creates a SQL Statement for repository <i>Merge</i> operation that is meant for SQL Server.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The <i>DataEntity</i> object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="qualifiers">The list of qualifier fields to be used for the <i>Merge</i> operation in SQL Statement composition.</param>
        /// <param name="isPrimaryIdentity">A boolean value indicates whether the primary key is identity in the database.</param>
        /// <returns>A string containing the composed SQL Statement for <i>Merge</i> operation.</returns>
        internal string CreateMerge<TEntity>(QueryBuilder<TEntity> queryBuilder, IEnumerable<Field> qualifiers, bool isPrimaryIdentity)
            where TEntity : DataEntity
        {
            var primary = DataEntityExtension.GetPrimaryProperty<TEntity>();

            // Add the primary key as the default qualifier
            if (qualifiers == null && primary != null)
            {
                qualifiers = Field.From(primary.GetMappedName());
            }

            // Get the target properties
            var insertableProperties = DataEntityExtension.GetPropertiesFor<TEntity>(Command.Insert)
                .Where(property => !(isPrimaryIdentity && property == primary));
            var updateableProperties = DataEntityExtension.GetPropertiesFor<TEntity>(Command.Merge)
                .Where(property => property != primary);
            var mergeableProperties = DataEntityExtension.GetPropertiesFor<TEntity>(Command.Merge);
            var mergeInsertableFields = mergeableProperties
                .Where(property => insertableProperties.Contains(property))
                .Select(property => new Field(property.Name));
            var mergeUpdateableFields = mergeableProperties
                .Where(property => updateableProperties.Contains(property))
                .Select(property => new Field(property.Name));

            // Build the SQL Statement
            queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
            queryBuilder
                .Clear()
                // MERGE T USING S
                .Merge()
                .Table(Command.Merge)
                .As("T")
                .Using()
                .OpenParen()
                .Select()
                .ParametersAsFields(Command.None) // All fields must be included for selection
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
                .Fields(mergeInsertableFields)
                .CloseParen()
                .Values()
                .OpenParen()
                .Parameters(mergeInsertableFields)
                .CloseParen()
                // WHEN MATCHED THEN UPDATE SET
                .When()
                .Matched()
                .Then()
                .Update()
                .Set()
                .FieldsAndAliasFields(mergeUpdateableFields, "S")
                .End();

            // Return the query
            return queryBuilder.GetString();
        }

        /// <summary>
        /// Creates a SQL Statement for repository <i>Query</i> operation that is meant for SQL Server.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The <i>DataEntity</i> object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="where">The query expression for SQL statement.</param>
        /// <param name="top">The number of rows to be returned by the <i>Query</i> operation in SQL Statement composition.</param>
        /// <param name="orderBy">The list of fields  to be used for ordering in SQL Statement composition.</param>
        /// <returns>A string containing the composed SQL Statement for <i>Query</i> operation.</returns>
        public string CreateQuery<TEntity>(QueryBuilder<TEntity> queryBuilder, QueryGroup where, int? top = 0, IEnumerable<OrderField> orderBy = null)
            where TEntity : DataEntity
        {
            queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
            queryBuilder
                .Clear()
                .Select()
                .Top(top)
                .Fields(Command.Query)
                .From()
                .Table(Command.Query)
                .Where(where)
                .OrderBy(orderBy)
                .End();
            return queryBuilder.GetString();
        }

        /// <summary>
        /// Creates a SQL Statement for repository <i>Update</i> operation that is meant for SQL Server.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The <i>DataEntity</i> object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="where">The query expression for SQL statement.</param>
        /// <returns>A string containing the composed SQL Statement for <i>Update</i> operation.</returns>
        public string CreateUpdate<TEntity>(QueryBuilder<TEntity> queryBuilder, QueryGroup where)
            where TEntity : DataEntity
        {
            queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
            var fields = DataEntityExtension.GetPropertiesFor<TEntity>(Command.Update)
                .Where(property => property != DataEntityExtension.GetPrimaryProperty<TEntity>())
                .Select(p => new Field(p.Name));
            queryBuilder
                .Clear()
                .Update()
                .Table(Command.Update)
                .Set()
                .FieldsAndParameters(fields)
                .Where(where)
                .End();
            return queryBuilder.GetString();
        }
    }
}
