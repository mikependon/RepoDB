using System.Collections.Generic;
using System.Data;

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
        /// Creates a SQL Statement for repository batch query operation.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The data entity object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="where">The query expression for SQL statement.</param>
        /// <param name="page">The page of the batch.</param>
        /// <param name="rowsPerBatch">The number of rows per batch.</param>
        /// <param name="orderBy">The list of fields used for ordering.</param>
        /// <param name="hints">The hints to be used to optimze the query operation.</param>
        /// <returns>A string containing the composed SQL Statement for batch query operation.</returns>
        string CreateBatchQuery<TEntity>(QueryBuilder<TEntity> queryBuilder,
            QueryGroup where = null,
            int? page = null,
            int? rowsPerBatch = null,
            IEnumerable<OrderField> orderBy = null,
            string hints = null)
            where TEntity : class;

        /// <summary>
        /// Creates a SQL Statement for repository count operation.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The data entity object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="where">The query expression for SQL statement.</param>
        /// <param name="hints">The hints to be used to optimze the query operation.</param>
        /// <returns>A string containing the composed SQL Statement for count operation.</returns>
        string CreateCount<TEntity>(QueryBuilder<TEntity> queryBuilder,
            QueryGroup where = null,
            string hints = null)
            where TEntity : class;

        /// <summary>
        /// Creates a SQL Statement for repository delete operation.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The data entity object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="where">The query expression for SQL statement.</param>
        /// <returns>A string containing the composed SQL Statement for delete operation.</returns>
        string CreateDelete<TEntity>(QueryBuilder<TEntity> queryBuilder,
            QueryGroup where = null)
            where TEntity : class;

        /// <summary>
        /// Creates a SQL Statement for repository delete-all operation.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The data entity object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <returns>A string containing the composed SQL Statement for delete-all operation.</returns>
        string CreateDeleteAll<TEntity>(QueryBuilder<TEntity> queryBuilder)
            where TEntity : class;

        /// <summary>
        /// Creates a SQL Statement for repository inline-insert operation.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The data entity object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="fields">The list of fields to be a part of the inline insert operation in SQL Statement composition.</param>
        /// <returns>A string containing the composed SQL Statement for inline-insert operation.</returns>
        string CreateInlineInsert<TEntity>(QueryBuilder<TEntity> queryBuilder,
            IEnumerable<Field> fields = null)
            where TEntity : class;

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
        string CreateInlineMerge<TEntity>(QueryBuilder<TEntity> queryBuilder,
            IEnumerable<Field> fields = null,
            IEnumerable<Field> qualifiers = null)
            where TEntity : class;

        /// <summary>
        /// Creates a SQL Statement for repository inline-update operation.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The data entity object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="fields">The list of fields to be a part of the inline update operation in SQL Statement composition.</param>
        /// <param name="where">The query expression for SQL statement.</param>
        /// <returns>A string containing the composed SQL Statement for inline-update operation.</returns>
        string CreateInlineUpdate<TEntity>(QueryBuilder<TEntity> queryBuilder,
            IEnumerable<Field> fields = null,
            QueryGroup where = null)
            where TEntity : class;

        /// <summary>
        /// Creates a SQL Statement for repository insert operation.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The data entity object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <returns>A string containing the composed SQL Statement for insert operation.</returns>
        string CreateInsert<TEntity>(QueryBuilder<TEntity> queryBuilder)
            where TEntity : class;

        /// <summary>
        /// Creates a SQL Statement for repository merge operation.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The data entity object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="qualifiers">The list of qualifier fields to be used for the merge operation in SQL Statement composition.</param>
        /// <returns>A string containing the composed SQL Statement for merge operation.</returns>
        string CreateMerge<TEntity>(QueryBuilder<TEntity> queryBuilder,
            IEnumerable<Field> qualifiers = null)
            where TEntity : class;

        /// <summary>
        /// Creates a SQL Statement for repository query operation.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The data entity object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="where">The query expression for SQL statement.</param>
        /// <param name="orderBy">The list of fields to be used for ordering in SQL Statement composition.</param>
        /// <param name="top">The number of rows to be returned by the query operation in SQL Statement composition.</param>
        /// <param name="hints">The hints to be used to optimze the query operation.</param>
        /// <returns>A string containing the composed SQL Statement for query operation.</returns>
        string CreateQuery<TEntity>(QueryBuilder<TEntity> queryBuilder,
            QueryGroup where = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = null,
            string hints = null)
            where TEntity : class;

        /// <summary>
        /// Creates a SQL Statement for repository truncate operation.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The data entity object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <returns>A string containing the composed SQL Statement for truncate operation.</returns>
        string CreateTruncate<TEntity>(QueryBuilder<TEntity> queryBuilder)
            where TEntity : class;

        /// <summary>
        /// Creates a SQL Statement for repository update operation.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The data entity object bound for the SQL Statement to be created.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="where">The query expression for SQL statement.</param>
        /// <returns>A string containing the composed SQL Statement for update operation.</returns>
        string CreateUpdate<TEntity>(QueryBuilder<TEntity> queryBuilder,
            QueryGroup where = null)
            where TEntity : class;
    }
}
