using RepoDb.Interfaces;
using System;
using System.Data;

namespace RepoDb.Requests
{
    /// <summary>
    /// A class that holds the value of the 'Truncate' operation arguments.
    /// </summary>
    internal class TruncateRequest : BaseRequest, IEquatable<TruncateRequest>
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
            IStatementBuilder statementBuilder = null)
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
            IStatementBuilder statementBuilder = null)
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
            var hashCode = HashCode.Combine(Name, ".Truncate");

            // Set and return the hashcode
            return (this.hashCode = hashCode).Value;
        }

        /// <summary>
        /// Compares the <see cref="TruncateRequest"/> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equals.</returns>
        public override bool Equals(object obj) =>
            obj?.GetHashCode() == GetHashCode();

        /// <summary>
        /// Compares the <see cref="TruncateRequest"/> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(TruncateRequest other) =>
            other?.GetHashCode() == GetHashCode();

        /// <summary>
        /// Compares the equality of the two <see cref="TruncateRequest"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="TruncateRequest"/> object.</param>
        /// <param name="objB">The second <see cref="TruncateRequest"/> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(TruncateRequest objA,
            TruncateRequest objB)
        {
            if (objA is null)
            {
                return objB is null;
            }
            return objB?.GetHashCode() == objA.GetHashCode();
        }

        /// <summary>
        /// Compares the inequality of the two <see cref="TruncateRequest"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="TruncateRequest"/> object.</param>
        /// <param name="objB">The second <see cref="TruncateRequest"/> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(TruncateRequest objA,
            TruncateRequest objB) =>
            (objA == objB) == false;

        #endregion
    }
}
