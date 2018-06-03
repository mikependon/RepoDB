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
        /// Creates a SQL Statement for repository <i>BatchQuery</i> operation.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The <i>Data Entity</i> object bound for the SQL Statement to be created. This must implement the <i>RepoDb.Interfaces.IDataEntity</i> interface.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="where">The query expression for SQL statement.</param>
        /// <param name="page">The page of the batch.</param>
        /// <param name="rowsPerBatch">The number of rows per batch.</param>
        /// <param name="orderBy">The list of fields used for ordering.</param>
        /// <returns>A string containing the composed SQL Statement for <i>BatchQuery</i> operation.</returns>
        public string CreateBatchQuery<TEntity>(IQueryBuilder<TEntity> queryBuilder, IQueryGroup where, int page, int rowsPerBatch, IEnumerable<IOrderField> orderBy)
            where TEntity : IDataEntity
        {
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
                .Fields(Command.BatchQuery)
                .From()
                .WriteText("CTE")
                .WriteText($"WHERE ([RowNumber] BETWEEN {(page * rowsPerBatch) + 1} AND {(page + 1) * rowsPerBatch})")
                .OrderBy(orderBy)
                .End();
            return queryBuilder.GetString();
        }

        /// <summary>
        /// Creates a SQL Statement for repository <i>Count</i> operation.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The <i>Data Entity</i> object bound for the SQL Statement to be created. This must implement the <i>RepoDb.Interfaces.IDataEntity</i> interface.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="where">The query expression for SQL statement.</param>
        /// <returns>A string containing the composed SQL Statement for <i>Count</i> operation.</returns>
        public string CreateCount<TEntity>(IQueryBuilder<TEntity> queryBuilder, IQueryGroup where)
            where TEntity : IDataEntity
        {
            queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
            queryBuilder
                .Clear()
                .Select()
                .Count()
                .WriteText("(*) AS [Counted]")
                .From()
                .Table(Command.Count)
                .Where(where)
                .End();
            return queryBuilder.GetString();
        }

        /// <summary>
        /// Creates a SQL Statement for repository <i>CountBig</i> operation.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The <i>Data Entity</i> object bound for the SQL Statement to be created. This must implement the <i>RepoDb.Interfaces.IDataEntity</i> interface.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="where">The query expression for SQL statement.</param>
        /// <returns>A string containing the composed SQL Statement for <i>CountBig</i> operation.</returns>
        public string CreateCountBig<TEntity>(IQueryBuilder<TEntity> queryBuilder, IQueryGroup where)
            where TEntity : IDataEntity
        {
            queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
            queryBuilder
                .Clear()
                .Select()
                .CountBig()
                .WriteText("(*) AS [Counted]")
                .From()
                .Table(Command.CountBig)
                .Where(where)
                .End();
            return queryBuilder.GetString();
        }

        /// <summary>
        /// Creates a SQL Statement for repository <i>Delete</i> operation.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The <i>Data Entity</i> object bound for the SQL Statement to be created. This must implement the <i>RepoDb.Interfaces.IDataEntity</i> interface.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="where">The query expression for SQL statement.</param>
        /// <returns>A string containing the composed SQL Statement for <i>Delete</i> operation.</returns>
        public string CreateDelete<TEntity>(IQueryBuilder<TEntity> queryBuilder, IQueryGroup where)
            where TEntity : IDataEntity
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
        /// Creates a SQL Statement for repository <i>InlineUpdate</i> operation.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The <i>Data Entity</i> object bound for the SQL Statement to be created. This must implement the <i>RepoDb.Interfaces.IDataEntity</i> interface.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="fields">The list of fields to be a part of inline update operation on SQL Statement composition.</param>
        /// <param name="where">The query expression for SQL statement.</param>
        /// <param name="overrideIgnore">
        /// Set to <i>true</i> if the defined <i>RepoDb.Attributes.IgnoreAttribute</i> would likely 
        /// be ignored on the inline update operation on SQL Statement composition.
        /// </param>
        /// <returns>A string containing the composed SQL Statement for <i>InlineUpdate</i> operation.</returns>
        public string CreateInlineUpdate<TEntity>(IQueryBuilder<TEntity> queryBuilder, IEnumerable<IField> fields,
            IQueryGroup where, bool? overrideIgnore = false)
            where TEntity : IDataEntity
        {
            if (overrideIgnore == false)
            {
                var properties = PropertyCache.Get<TEntity>(Command.InlineUpdate)
                    .Select(property => property.GetMappedName());
                var unmatches = fields?.Where(field =>
                    properties?.FirstOrDefault(property =>
                        field.Name.ToLower() == property.ToLower()) == null);
                if (unmatches?.Count() > 0)
                {
                    throw new InvalidOperationException($"The following columns ({unmatches.Select(field => field.AsField()).Join(", ")}) " +
                        $"are not updatable for entity ({DataEntityExtension.GetMappedName<TEntity>(Command.InlineUpdate)}).");
                }
            }
            queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
            queryBuilder
                .Clear()
                .Update()
                .Table(Command.InlineUpdate)
                .Set()
                .FieldsAndParameters(fields)
                .Where(where)
                .End();
            return queryBuilder.GetString();
        }

        /// <summary>
        /// Creates a SQL Statement for repository <i>Insert</i> operation.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The <i>Data Entity</i> object bound for the SQL Statement to be created. This must implement the <i>RepoDb.Interfaces.IDataEntity</i> interface.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <returns>A string containing the composed SQL Statement for <i>Insert</i> operation.</returns>
        public string CreateInsert<TEntity>(IQueryBuilder<TEntity> queryBuilder)
            where TEntity : IDataEntity
        {
            queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
            var primary = PrimaryPropertyCache.Get<TEntity>();
            queryBuilder
                .Clear()
                .Insert()
                .Into()
                .Table(Command.Insert)
                .OpenParen()
                .Fields(Command.Insert)
                .CloseParen()
                .Values()
                .OpenParen()
                .Parameters(Command.Insert)
                .CloseParen()
                .End();
            if (primary != null)
            {
                var result = primary.IsIdentity() ? "SCOPE_IDENTITY()" : $"@{primary.GetMappedName()}";
                queryBuilder
                    .Select()
                    .WriteText(result)
                    .As("[Result]")
                    .End();
            }
            return queryBuilder.GetString();
        }

        /// <summary>
        /// Creates a SQL Statement for repository <i>Merge</i> operation.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The <i>Data Entity</i> object bound for the SQL Statement to be created. This must implement the <i>RepoDb.Interfaces.IDataEntity</i> interface.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="qualifiers">The list of qualifier fields to be used for the <i>Merge</i> operation on SQL Statement composition.</param>
        /// <returns>A string containing the composed SQL Statement for <i>Merge</i> operation.</returns>
        public string CreateMerge<TEntity>(IQueryBuilder<TEntity> queryBuilder, IEnumerable<IField> qualifiers)
            where TEntity : IDataEntity
        {
            queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
            if (qualifiers == null)
            {
                var primaryKey = PrimaryPropertyCache.Get<TEntity>();
                if (primaryKey != null)
                {
                    qualifiers = new Field(primaryKey.Name).AsEnumerable();
                }
            }
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
                            .Join($" {Constant.And.ToUpper()} "))
                .CloseParen()
                // WHEN NOT MATCHED THEN INSERT VALUES
                .When()
                .Not()
                .Matched()
                .Then()
                .Insert()
                .OpenParen()
                .Fields(Command.Merge)
                .CloseParen()
                .Values()
                .OpenParen()
                .Parameters(Command.Merge)
                .CloseParen()
                // WHEN MATCHED THEN UPDATE SET
                .When()
                .Matched()
                .Then()
                .Update()
                .Set()
                .FieldsAndAliasFields(Command.Merge, "S")
                .End();
            return queryBuilder.GetString();
        }

        /// <summary>
        /// Creates a SQL Statement for repository <i>Query</i> operation.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The <i>Data Entity</i> object bound for the SQL Statement to be created. This must implement the <i>RepoDb.Interfaces.IDataEntity</i> interface.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="where">The query expression for SQL statement.</param>
        /// <param name="top">The number of rows to be returned by the <i>Query</i> operation on SQL Statement composition.</param>
        /// <param name="orderBy">The list of fields  to be used for ordering on SQL Statement composition.</param>
        /// <returns>A string containing the composed SQL Statement for <i>Query</i> operation.</returns>
        public string CreateQuery<TEntity>(IQueryBuilder<TEntity> queryBuilder, IQueryGroup where, int? top = 0, IEnumerable<IOrderField> orderBy = null)
            where TEntity : IDataEntity
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
        /// Creates a SQL Statement for repository <i>Update</i> operation.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The <i>Data Entity</i> object bound for the SQL Statement to be created. This must implement the <i>RepoDb.Interfaces.IDataEntity</i> interface.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="where">The query expression for SQL statement.</param>
        /// <returns>A string containing the composed SQL Statement for <i>Update</i> operation.</returns>
        public string CreateUpdate<TEntity>(IQueryBuilder<TEntity> queryBuilder, IQueryGroup where)
            where TEntity : IDataEntity
        {
            queryBuilder = queryBuilder ?? new QueryBuilder<TEntity>();
            queryBuilder
                .Clear()
                .Update()
                .Table(Command.Update)
                .Set()
                .FieldsAndParameters(Command.Update)
                .Where(where)
                .End();
            return queryBuilder.GetString();
        }
    }
}
