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
        /// The data entity object bound for the SQL Statement to be created. This must implement the <i>RepoDb.Interfaces.IDataEntity</i> interface.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="where">The query expression for SQL statement.</param>
        /// <param name="page">The page of the batch.</param>
        /// <param name="rowsPerBatch">The number of rows per batch.</param>
        /// <param name="orderBy">The list of fields used for ordering.</param>
        /// <returns>A string containing the composed SQL Statement for <i>BatchQuery</i> operation.</returns>
        string CreateBatchQuery<TEntity>(IQueryBuilder<TEntity> queryBuilder, IQueryGroup where, int page, int rowsPerBatch, IEnumerable<IOrderField> orderby)
            where TEntity : IDataEntity;

        /// <summary>
        /// Creates a SQL Statement for repository <i>Count</i> operation.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The data entity object bound for the SQL Statement to be created. This must implement the <i>RepoDb.Interfaces.IDataEntity</i> interface.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="where">The query expression for SQL statement.</param>
        /// <returns>A string containing the composed SQL Statement for <i>Count</i> operation.</returns>
        string CreateCount<TEntity>(IQueryBuilder<TEntity> queryBuilder, IQueryGroup where)
            where TEntity : IDataEntity;

        /// <summary>
        /// Creates a SQL Statement for repository <i>CountBig</i> operation.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The data entity object bound for the SQL Statement to be created. This must implement the <i>RepoDb.Interfaces.IDataEntity</i> interface.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="where">The query expression for SQL statement.</param>
        /// <returns>A string containing the composed SQL Statement for <i>CountBig</i> operation.</returns>
        string CreateCountBig<TEntity>(IQueryBuilder<TEntity> queryBuilder, IQueryGroup where)
            where TEntity : IDataEntity;

        /// <summary>
        /// Creates a SQL Statement for repository <i>Delete</i> operation.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The data entity object bound for the SQL Statement to be created. This must implement the <i>RepoDb.Interfaces.IDataEntity</i> interface.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="where">The query expression for SQL statement.</param>
        /// <returns>A string containing the composed SQL Statement for <i>Delete</i> operation.</returns>
        string CreateDelete<TEntity>(IQueryBuilder<TEntity> queryBuilder, IQueryGroup where)
            where TEntity : IDataEntity;

        /// <summary>
        /// Creates a SQL Statement for repository <i>InlineUpdate</i> operation.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The data entity object bound for the SQL Statement to be created. This must implement the <i>RepoDb.Interfaces.IDataEntity</i> interface.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="fields">The list of fields to be a part of inline update operation on SQL Statement composition.</param>
        /// <param name="where">The query expression for SQL statement.</param>
        /// <param name="overrideIgnore">
        /// Set to <i>true</i> if the defined <i>RepoDb.Attributes.IgnoreAttribute</i> would likely 
        /// be ignored on the inline update operation on SQL Statement composition.
        /// </param>
        /// <returns>A string containing the composed SQL Statement for <i>InlineUpdate</i> operation.</returns>
        string CreateInlineUpdate<TEntity>(IQueryBuilder<TEntity> queryBuilder, IEnumerable<IField> fields, IQueryGroup where, bool? overrideIgnore = false)
            where TEntity : IDataEntity;

        /// <summary>
        /// Creates a SQL Statement for repository <i>Insert</i> operation.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The data entity object bound for the SQL Statement to be created. This must implement the <i>RepoDb.Interfaces.IDataEntity</i> interface.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <returns>A string containing the composed SQL Statement for <i>Insert</i> operation.</returns>
        string CreateInsert<TEntity>(IQueryBuilder<TEntity> queryBuilder)
            where TEntity : IDataEntity;

        /// <summary>
        /// Creates a SQL Statement for repository <i>Merge</i> operation.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The data entity object bound for the SQL Statement to be created. This must implement the <i>RepoDb.Interfaces.IDataEntity</i> interface.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="qualifiers">The list of qualifier fields to be used for the <i>Merge</i> operation on SQL Statement composition.</param>
        /// <returns>A string containing the composed SQL Statement for <i>Merge</i> operation.</returns>
        string CreateMerge<TEntity>(IQueryBuilder<TEntity> queryBuilder, IEnumerable<IField> qualifiers)
            where TEntity : IDataEntity;

        /// <summary>
        /// Creates a SQL Statement for repository <i>Query</i> operation.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The data entity object bound for the SQL Statement to be created. This must implement the <i>RepoDb.Interfaces.IDataEntity</i> interface.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="where">The query expression for SQL statement.</param>
        /// <param name="top">The number of rows to be returned by the <i>Query</i> operation on SQL Statement composition.</param>
        /// <param name="orderBy">The list of fields  to be used for ordering on SQL Statement composition.</param>
        /// <returns>A string containing the composed SQL Statement for <i>Query</i> operation.</returns>
        string CreateQuery<TEntity>(IQueryBuilder<TEntity> queryBuilder, IQueryGroup where, int? top = 0, IEnumerable<IOrderField> orderBy = null)
            where TEntity : IDataEntity;

        /// <summary>
        /// Creates a SQL Statement for repository <i>Update</i> operation.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The data entity object bound for the SQL Statement to be created. This must implement the <i>RepoDb.Interfaces.IDataEntity</i> interface.
        /// </typeparam>
        /// <param name="queryBuilder">An instance of query builder used to build the SQL statement.</param>
        /// <param name="where">The query expression for SQL statement.</param>
        /// <returns>A string containing the composed SQL Statement for <i>Update</i> operation.</returns>
        string CreateUpdate<TEntity>(IQueryBuilder<TEntity> queryBuilder, IQueryGroup where)
            where TEntity : IDataEntity;
    }
}
