using System.Collections.Generic;

namespace RepoDb.Interfaces
{
    /// <summary>
    /// An interface used to mark a class to be a statement builder. The statement builder is an object being mapped or injected into the
    /// repositories to be used for composing the SQL Statements. Implement this interface if the caller would likely to support the different
    /// statement building approach, or by supporting the other data providers like Oracle, OleDb, MySql, etc.
    /// </summary>
    public interface IStatementBuilder
    {
        /// <summary>
        /// Creates a SQL Statement for repository <i>BatchQuery</i> operation.
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
        string CreateBatchQuery<TEntity>(QueryBuilder<TEntity> queryBuilder, QueryGroup where, int page, int rowsPerBatch, IEnumerable<OrderField> orderBy)
            where TEntity : DataEntity;

        /// <summary>
        /// Creates a SQL Statement for repository <i>Count</i> operation.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The <i>DataEntity</i> object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="where">The query expression for SQL statement.</param>
        /// <returns>A string containing the composed SQL Statement for <i>Count</i> operation.</returns>
        string CreateCount<TEntity>(QueryBuilder<TEntity> queryBuilder, QueryGroup where)
            where TEntity : DataEntity;

        /// <summary>
        /// Creates a SQL Statement for repository <i>Delete</i> operation.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The <i>DataEntity</i> object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="where">The query expression for SQL statement.</param>
        /// <returns>A string containing the composed SQL Statement for <i>Delete</i> operation.</returns>
        string CreateDelete<TEntity>(QueryBuilder<TEntity> queryBuilder, QueryGroup where)
            where TEntity : DataEntity;

        /// <summary>
        /// Creates a SQL Statement for repository <i>DeleteAll</i> operation.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The <i>DataEntity</i> object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <returns>A string containing the composed SQL Statement for <i>DeleteAll</i> operation.</returns>
        string CreateDeleteAll<TEntity>(QueryBuilder<TEntity> queryBuilder)
            where TEntity : DataEntity;

        /// <summary>
        /// Creates a SQL Statement for repository <i>InlineInsert</i> operation.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The <i>DataEntity</i> object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="fields">The list of fields to be a part of the inline insert operation in SQL Statement composition.</param>
        /// <param name="overrideIgnore">
        /// Set to <i>true</i> if the defined <i>RepoDb.Attributes.IgnoreAttribute</i> would likely 
        /// be ignored on the inline insert operation in SQL Statement composition.
        /// </param>
        /// <returns>A string containing the composed SQL Statement for <i>InlineInsert</i> operation.</returns>
        string CreateInlineInsert<TEntity>(QueryBuilder<TEntity> queryBuilder, IEnumerable<Field> fields, bool? overrideIgnore = false)
            where TEntity : DataEntity;

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
        /// be ignored on the inline insert operation in SQL Statement composition.
        /// </param>
        /// <returns>A string containing the composed SQL Statement for <i>InlineMerge</i> operation.</returns>
        string CreateInlineMerge<TEntity>(QueryBuilder<TEntity> queryBuilder, IEnumerable<Field> fields, IEnumerable<Field> qualifiers, bool? overrideIgnore = false)
            where TEntity : DataEntity;

        /// <summary>
        /// Creates a SQL Statement for repository <i>InlineUpdate</i> operation.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The <i>DataEntity</i> object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="fields">The list of fields to be a part of the inline update operation in SQL Statement composition.</param>
        /// <param name="where">The query expression for SQL statement.</param>
        /// <param name="overrideIgnore">
        /// Set to <i>true</i> if the defined <i>RepoDb.Attributes.IgnoreAttribute</i> would likely 
        /// be ignored on the inline update operation in SQL Statement composition.
        /// </param>
        /// <returns>A string containing the composed SQL Statement for <i>InlineUpdate</i> operation.</returns>
        string CreateInlineUpdate<TEntity>(QueryBuilder<TEntity> queryBuilder, IEnumerable<Field> fields, QueryGroup where, bool? overrideIgnore = false)
            where TEntity : DataEntity;

        /// <summary>
        /// Creates a SQL Statement for repository <i>Insert</i> operation.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The <i>DataEntity</i> object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <returns>A string containing the composed SQL Statement for <i>Insert</i> operation.</returns>
        string CreateInsert<TEntity>(QueryBuilder<TEntity> queryBuilder)
            where TEntity : DataEntity;

        /// <summary>
        /// Creates a SQL Statement for repository <i>Merge</i> operation.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The <i>DataEntity</i> object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="qualifiers">The list of qualifier fields to be used for the <i>Merge</i> operation in SQL Statement composition.</param>
        /// <returns>A string containing the composed SQL Statement for <i>Merge</i> operation.</returns>
        string CreateMerge<TEntity>(QueryBuilder<TEntity> queryBuilder, IEnumerable<Field> qualifiers)
            where TEntity : DataEntity;

        /// <summary>
        /// Creates a SQL Statement for repository <i>Query</i> operation.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The <i>DataEntity</i> object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="where">The query expression for SQL statement.</param>
        /// <param name="top">The number of rows to be returned by the <i>Query</i> operation in SQL Statement composition.</param>
        /// <param name="orderBy">The list of fields  to be used for ordering in SQL Statement composition.</param>
        /// <returns>A string containing the composed SQL Statement for <i>Query</i> operation.</returns>
        string CreateQuery<TEntity>(QueryBuilder<TEntity> queryBuilder, QueryGroup where, int? top = 0, IEnumerable<OrderField> orderBy = null)
            where TEntity : DataEntity;
        /// <summary>
        /// Creates a SQL Statement for repository <i>Truncate</i> operation that is meant for SQL Server.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The <i>DataEntity</i> object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <returns>A string containing the composed SQL Statement for <i>Truncate</i> operation.</returns>
        string CreateTruncate<TEntity>(QueryBuilder<TEntity> queryBuilder) where TEntity : DataEntity;

        /// <summary>
        /// Creates a SQL Statement for repository <i>Update</i> operation.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The <i>DataEntity</i> object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="where">The query expression for SQL statement.</param>
        /// <returns>A string containing the composed SQL Statement for <i>Update</i> operation.</returns>
        string CreateUpdate<TEntity>(QueryBuilder<TEntity> queryBuilder, QueryGroup where)
            where TEntity : DataEntity;
    }
}
