using RepoDb.Interfaces;
using System;
using System.Data;

namespace RepoDb.Requests
{
    /// <summary>
    /// A base class for all operational request.
    /// </summary>
    internal abstract class BaseRequest : IEquatable<BaseRequest>
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
            IStatementBuilder? statementBuilder = null)
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

        #region Equality and comparers

        private int? hashCode = null;

        /// <summary>
        /// Returns the hashcode for this <see cref="BaseRequest"/>.
        /// </summary>
        /// <returns>The hashcode value.</returns>
        public override int GetHashCode()
        {
            // Make sure to return if it is already provided
            if (this.hashCode != null)
            {
                return this.hashCode.Value;
            }

            // Set and return the hashcode
            return (this.hashCode = GetType().GetHashCode()).Value;
        }

        /// <summary>
        /// Compares the <see cref="BaseRequest"/> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equals.</returns>
        public override bool Equals(object obj) =>
            (obj is null) ? false : obj.GetHashCode() == GetHashCode();

        /// <summary>
        /// Compares the <see cref="BaseRequest"/> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(BaseRequest other) =>
            (other is null) ? false : other.GetHashCode() == GetHashCode();

        /// <summary>
        /// Compares the equality of the two <see cref="BaseRequest"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="BaseRequest"/> object.</param>
        /// <param name="objB">The second <see cref="BaseRequest"/> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(BaseRequest objA,
            BaseRequest objB) =>
            (objA is null) ? objB is null : objA.Equals(objB);

        /// <summary>
        /// Compares the inequality of the two <see cref="BaseRequest"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="BaseRequest"/> object.</param>
        /// <param name="objB">The second <see cref="BaseRequest"/> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(BaseRequest objA,
            BaseRequest objB) =>
            (objA == objB) == false;

        #endregion
    }
}
