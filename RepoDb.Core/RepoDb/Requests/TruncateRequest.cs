using RepoDb.Interfaces;
using System;
using System.Data;

namespace RepoDb.Requests
{
    /// <summary>
    /// A class that holds the value of the 'Truncate' operation arguments.
    /// </summary>
    internal class TruncateRequest : BaseRequest
    {
        private int? hashCode = null;

        /// <summary>
        /// Creates a new instance of <see cref="TruncateRequest"/> object.
        /// </summary>
        /// <param name="type">The target type.</param>
        /// <param name="connection">The connection object.</param>
        /// <param name="transaction">The transaction object.</param>
        /// <param name="statementBuilder">The statement builder.</param>
        public TruncateRequest(Type type,
            IDbConnection connection,
            IDbTransaction transaction,
            IStatementBuilder? statementBuilder = null)
            : this(ClassMappedNameCache.Get(type),
                connection,
                transaction,
                statementBuilder)
        {
            Type = type;
        }

        /// <summary>
        /// Creates a new instance of <see cref="TruncateRequest"/> object.
        /// </summary>
        /// <param name="name">The name of the request.</param>
        /// <param name="connection">The connection object.</param>
        /// <param name="transaction">The transaction object.</param>
        /// <param name="statementBuilder">The statement builder.</param>
        public TruncateRequest(string name,
            IDbConnection connection,
            IDbTransaction transaction,
            IStatementBuilder? statementBuilder = null)
            : base(name,
                  connection,
                  transaction,
                  statementBuilder)
        { }

        #region Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <see cref="TruncateRequest"/>.
        /// </summary>
        /// <returns>The hashcode value.</returns>
        public override int GetHashCode()
        {
            // Make sure to return if it is already provided
            if (this.hashCode != null)
            {
                return this.hashCode.Value;
            }

            // Get first the entity hash code
            var hashCode = HashCode.Combine(base.GetHashCode(), Name, ".Truncate");

            // Set and return the hashcode
            return (this.hashCode = hashCode).Value;
        }
        
        #endregion
    }
}
