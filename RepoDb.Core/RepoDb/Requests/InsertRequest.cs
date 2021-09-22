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
    internal class InsertRequest : BaseRequest
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
            var hashCode = base.GetHashCode() + HashCode.Combine(Name, ".Insert");

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

        #endregion
    }
}
