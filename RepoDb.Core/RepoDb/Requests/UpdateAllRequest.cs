using RepoDb.Extensions;
using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace RepoDb.Requests
{
    /// <summary>
    /// A class that holds the value of the 'UpdateAll' operation arguments.
    /// </summary>
    internal class UpdateAllRequest : BaseRequest
    {
        private int? hashCode = null;

        /// <summary>
        /// Creates a new instance of <see cref="UpdateAllRequest"/> object.
        /// </summary>
        /// <param name="type">The target type.</param>
        /// <param name="connection">The connection object.</param>
        /// <param name="transaction">The transaction object.</param>
        /// <param name="fields">The list of the target fields.</param>
        /// <param name="qualifiers">The list of the qualifier <see cref="Field"/> objects.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="hints">The hints for the table.</param>
        /// <param name="statementBuilder">The statement builder.</param>
        public UpdateAllRequest(Type type,
            IDbConnection connection,
            IDbTransaction transaction,
            IEnumerable<Field> fields = null,
            IEnumerable<Field>? qualifiers = null,
            int batchSize = Constant.DefaultBatchOperationSize,
            string? hints = null,
            IStatementBuilder? statementBuilder = null)
            : this(ClassMappedNameCache.Get(type),
                connection,
                transaction,
                fields,
                qualifiers,
                batchSize,
                hints,
                statementBuilder)
        {
            Type = type;
        }

        /// <summary>
        /// Creates a new instance of <see cref="UpdateAllRequest"/> object.
        /// </summary>
        /// <param name="name">The name of the request.</param>
        /// <param name="connection">The connection object.</param>
        /// <param name="transaction">The transaction object.</param>
        /// <param name="fields">The list of the target fields.</param>
        /// <param name="qualifiers">The list of the qualifier <see cref="Field"/> objects.</param>
        /// <param name="batchSize">The batch size of the update operation.</param>
        /// <param name="hints">The hints for the table.</param>
        /// <param name="statementBuilder">The statement builder.</param>
        public UpdateAllRequest(string name,
            IDbConnection connection,
            IDbTransaction transaction,
            IEnumerable<Field> fields = null,
            IEnumerable<Field>? qualifiers = null,
            int batchSize = Constant.DefaultBatchOperationSize,
            string? hints = null,
            IStatementBuilder? statementBuilder = null)
            : base(name,
                connection,
                transaction,
                statementBuilder)
        {
            Fields = fields?.AsList();
            Qualifiers = qualifiers?.AsList();
            BatchSize = batchSize;
            Hints = hints;
        }

        /// <summary>
        /// Gets the target fields.
        /// </summary>
        public IEnumerable<Field> Fields { get; set; }

        /// <summary>
        /// Gets the qualifiers fields.
        /// </summary>
        public IEnumerable<Field> Qualifiers { get; set; }

        /// <summary>
        /// Gets the size batch of the update operation.
        /// </summary>
        public int BatchSize { get; set; }

        /// <summary>
        /// Gets the hints for the table.
        /// </summary>
        public string Hints { get; }

        #region Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <see cref="UpdateAllRequest"/>.
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
            var hashCode = HashCode.Combine(base.GetHashCode(), Name, ".UpdateAll");

            // Get the fields
            if (Fields != null)
            {
                foreach (var field in Fields)
                {
                    hashCode = HashCode.Combine(hashCode, field);
                }
            }

            // Get the qualifier <see cref="Field"/> objects
            if (Fields != null)
            {
                foreach (var field in Qualifiers)
                {
                    hashCode = HashCode.Combine(hashCode, field);
                }
            }

            // Get the batch size
            if (BatchSize > 0)
            {
                hashCode = HashCode.Combine(hashCode, BatchSize);
            }

            // Add the hints
            if (!string.IsNullOrWhiteSpace(Hints))
            {
                hashCode = HashCode.Combine(hashCode, Hints);
            }

            // Set and return the hashcode
            return (this.hashCode = hashCode).Value;
        }

        #endregion
    }
}
