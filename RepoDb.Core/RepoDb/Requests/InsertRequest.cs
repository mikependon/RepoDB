using RepoDb.Extensions;
using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace RepoDb.Requests
{
    /// <summary>
    /// A class that holds the value of the 'Insert' operation arguments.
    /// </summary>
    internal class InsertRequest : BaseRequest, IEquatable<InsertRequest>
    {
        private int? hashCode = null;

        /// <summary>
        /// Creates a new instance of <see cref="InsertRequest"/> object.
        /// </summary>
        /// <param name="type">The target type.</param>
        /// <param name="connection">The connection object.</param>
        /// <param name="transaction">The transaction object.</param>
        /// <param name="fields">The list of the target fields.</param>
        /// <param name="hints">The hints for the table.</param>
        /// <param name="statementBuilder">The statement builder.</param>
        public InsertRequest(Type type,
            IDbConnection connection,
            IDbTransaction transaction,
            IEnumerable<Field> fields = null,
            string hints = null,
            IStatementBuilder statementBuilder = null)
            : this(ClassMappedNameCache.Get(type),
                connection,
                transaction,
                fields,
                hints,
                statementBuilder)
        {
            Type = type;
        }

        /// <summary>
        /// Creates a new instance of <see cref="InsertRequest"/> object.
        /// </summary>
        /// <param name="name">The name of the request.</param>
        /// <param name="connection">The connection object.</param>
        /// <param name="transaction">The transaction object.</param>
        /// <param name="fields">The list of the target fields.</param>
        /// <param name="hints">The hints for the table.</param>
        /// <param name="statementBuilder">The statement builder.</param>
        public InsertRequest(string name,
            IDbConnection connection,
            IDbTransaction transaction,
            IEnumerable<Field> fields = null,
            string hints = null,
            IStatementBuilder statementBuilder = null)
            : base(name,
                connection,
                transaction,
                statementBuilder)
        {
            Fields = fields?.AsList();
            Hints = hints;
        }

        /// <summary>
        /// Gets the target fields.
        /// </summary>
        public IEnumerable<Field> Fields { get; set; }

        /// <summary>
        /// Gets the hints for the table.
        /// </summary>
        public string Hints { get; }

        #region Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <see cref="InsertRequest"/>.
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
            var hashCode = HashCode.Combine(Name, ".Insert");

            // Get the qualifier <see cref="Field"/> objects
            if (Fields != null)
            {
                foreach (var field in Fields)
                {
                    hashCode += field.GetHashCode();
                }
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
        /// Compares the <see cref="InsertRequest"/> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equals.</returns>
        public override bool Equals(object obj) =>
            obj?.GetHashCode() == GetHashCode();

        /// <summary>
        /// Compares the <see cref="InsertRequest"/> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(InsertRequest other) =>
            other?.GetHashCode() == GetHashCode();

        /// <summary>
        /// Compares the equality of the two <see cref="InsertRequest"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="InsertRequest"/> object.</param>
        /// <param name="objB">The second <see cref="InsertRequest"/> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(InsertRequest objA,
            InsertRequest objB)
        {
            if (objA is null)
            {
                return objB is null;
            }
            return objB?.GetHashCode() == objA.GetHashCode();
        }

        /// <summary>
        /// Compares the inequality of the two <see cref="InsertRequest"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="InsertRequest"/> object.</param>
        /// <param name="objB">The second <see cref="InsertRequest"/> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(InsertRequest objA,
            InsertRequest objB) =>
            (objA == objB) == false;

        #endregion
    }
}
