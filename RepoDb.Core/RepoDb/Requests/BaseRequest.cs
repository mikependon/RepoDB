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
        /// <param name="name">The name of request.</param>
        /// <param name="connection">The connection object.</param>
        /// <param name="transaction">The transaction object.</param>
        /// <param name="statementBuilder">The statement builder.</param>
        public BaseRequest(string name,
            IDbConnection connection,
            IDbTransaction transaction,
            IStatementBuilder statementBuilder = null)
        {
            Name = name;
            Connection = connection;
            Transaction = transaction;
            StatementBuilder = statementBuilder;
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        public Type Type { get; internal set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the connection object.
        /// </summary>
        public IDbConnection Connection { get; }

        /// <summary>
        /// Gets the transaction object.
        /// </summary>
        public IDbTransaction Transaction { get; }

        /// <summary>
        /// Gets the statement builder.
        /// </summary>
        public IStatementBuilder StatementBuilder { get; }
    }
}
