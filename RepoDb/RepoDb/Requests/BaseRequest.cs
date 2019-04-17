using RepoDb.Interfaces;
using System;
using System.Data;

namespace RepoDb.Requests
{
    /// <summary>
    /// A base class for all operational request.
    /// </summary>
    internal abstract class BaseRequest
    {
        /// <summary>
        /// Creates a new instance of <see cref="BaseRequest"/> object.
        /// </summary>
        /// <param name="entityType">The entity type.</param>
        /// <param name="connection">The connection object.</param>
        /// <param name="statementBuilder">The statement builder.</param>
        public BaseRequest(Type entityType, IDbConnection connection, IStatementBuilder statementBuilder = null)
            : this(entityType.FullName, connection, statementBuilder)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="BaseRequest"/> object.
        /// </summary>
        /// <param name="name">The name of request.</param>
        /// <param name="connection">The connection object.</param>
        /// <param name="statementBuilder">The statement builder.</param>
        public BaseRequest(string name, IDbConnection connection, IStatementBuilder statementBuilder = null)
        {
            Name = name;
            Connection = connection;
            StatementBuilder = statementBuilder;
        }

        /// <summary>
        /// Gets the entity type.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the connection object.
        /// </summary>
        public IDbConnection Connection { get; }

        /// <summary>
        /// Gets the statement builder.
        /// </summary>
        public IStatementBuilder StatementBuilder { get; }
    }
}
