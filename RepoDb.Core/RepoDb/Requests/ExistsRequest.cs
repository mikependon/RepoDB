using RepoDb.Interfaces;
using System;
using System.Data;

namespace RepoDb.Requests
{
    /// <summary>
    /// A class that holds the value of the 'Exists' operation arguments.
    /// </summary>
    internal class ExistsRequest : BaseRequest, IEquatable<ExistsRequest>
    {
        private int? hashCode = null;

        /// <summary>
        /// Creates a new instance of <see cref="ExistsRequest"/> object.
        /// </summary>
        /// <param name="type">The target type.</param>
        /// <param name="connection">The connection object.</param>
        /// <param name="transaction">The transaction object.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="hints">The hints for the table.</param>
        /// <param name="statementBuilder">The statement builder.</param>
        public ExistsRequest(Type type,
            IDbConnection connection,
            IDbTransaction transaction,
            QueryGroup where = null,
            string hints = null,
            IStatementBuilder statementBuilder = null)
            : this(ClassMappedNameCache.Get(type),
                  connection,
                  transaction,
                  where,
                  hints,
                  statementBuilder)
        {
            Type = type;
        }

        /// <summary>
        /// Creates a new instance of <see cref="ExistsRequest"/> object.
        /// </summary>
        /// <param name="name">The name of the request.</param>
        /// <param name="connection">The connection object.</param>
        /// <param name="transaction">The transaction object.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="hints">The hints for the table.</param>
        /// <param name="statementBuilder">The statement builder.</param>
        public ExistsRequest(string name,
            IDbConnection connection,
            IDbTransaction transaction,
            QueryGroup where = null,
            string hints = null,
            IStatementBuilder statementBuilder = null)
            : base(name,
                  connection,
                  transaction,
                  statementBuilder)
        {
            Where = where;
            Hints = hints;
        }

        /// <summary>
        /// Gets the query expression used.
        /// </summary>
        public QueryGroup Where { get; }

        /// <summary>
        /// Gets the hints for the table.
        /// </summary>
        public string Hints { get; }

        #region Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <see cref="ExistsRequest"/>.
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
            var hashCode = HashCode.Combine(Name, ".Exists");

            // Get the properties hash codes
            if (Where != null)
            {
                hashCode += Where.GetHashCode();
            }

            // Add the hints
            if (!string.IsNullOrWhiteSpace(Hints))
            {
                hashCode += Hints.GetHashCode();
            }

            // Set and return the hashcode
            return (this.hashCode = hashCode).Value;
        }

        /// <summary>
        /// Compares the <see cref="ExistsRequest"/> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equals.</returns>
        public override bool Equals(object obj) =>
            obj?.GetHashCode() == GetHashCode();

        /// <summary>
        /// Compares the <see cref="ExistsRequest"/> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(ExistsRequest other) =>
            other?.GetHashCode() == GetHashCode();

        /// <summary>
        /// Compares the equality of the two <see cref="ExistsRequest"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="ExistsRequest"/> object.</param>
        /// <param name="objB">The second <see cref="ExistsRequest"/> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(ExistsRequest objA,
            ExistsRequest objB)
        {
            if (objA is null)
            {
                return objB is null;
            }
            return objB?.GetHashCode() == objA.GetHashCode();
        }

        /// <summary>
        /// Compares the inequality of the two <see cref="ExistsRequest"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="ExistsRequest"/> object.</param>
        /// <param name="objB">The second <see cref="ExistsRequest"/> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(ExistsRequest objA,
            ExistsRequest objB) =>
            (objA == objB) == false;

        #endregion
    }
}
