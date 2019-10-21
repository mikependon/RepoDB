using RepoDb.Interfaces;
using System;
using System.Data;

namespace RepoDb.Requests
{
    /// <summary>
    /// A class that holds the value of the maximum-all operation arguments.
    /// </summary>
    internal class MaximumAllRequest : BaseRequest, IEquatable<MaximumAllRequest>
    {
        private int? m_hashCode = null;

        /// <summary>
        /// Creates a new instance of <see cref="MaximumAllRequest"/> object.
        /// </summary>
        /// <param name="type">The target type.</param>
        /// <param name="connection">The connection object.</param>
        /// <param name="transaction">The transaction object.</param>
        /// <param name="field">The field object.</param>
        /// <param name="hints">The hints for the table.</param>
        /// <param name="statementBuilder">The statement builder.</param>
        public MaximumAllRequest(Type type,
            IDbConnection connection,
            IDbTransaction transaction,
            Field field = null,
            string hints = null,
            IStatementBuilder statementBuilder = null)
            : this(ClassMappedNameCache.Get(type),
                  connection,
                  transaction,
                  field,
                  hints,
                  statementBuilder)
        {
            Type = type;
        }

        /// <summary>
        /// Creates a new instance of <see cref="MaximumAllRequest"/> object.
        /// </summary>
        /// <param name="name">The name of the request.</param>
        /// <param name="connection">The connection object.</param>
        /// <param name="transaction">The transaction object.</param>
        /// <param name="field">The field object.</param>
        /// <param name="hints">The hints for the table.</param>
        /// <param name="statementBuilder">The statement builder.</param>
        public MaximumAllRequest(string name,
            IDbConnection connection,
            IDbTransaction transaction,
            Field field = null,
            string hints = null,
            IStatementBuilder statementBuilder = null)
            : base(name,
                  connection,
                  transaction,
                  statementBuilder)
        {
            Field = field;
            Hints = hints;
        }

        /// <summary>
        /// Gets the field to be maximumd.
        /// </summary>
        public Field Field { get; }

        /// <summary>
        /// Gets the hints for the table.
        /// </summary>
        public string Hints { get; }

        #region Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <see cref="MaximumAllRequest"/>.
        /// </summary>
        /// <returns>The hashcode value.</returns>
        public override int GetHashCode()
        {
            // Make sure to return if it is already provided
            if (m_hashCode != null)
            {
                return m_hashCode.Value;
            }

            // Get first the entity hash code
            var hashCode = string.Concat(Name, ".MaximumAll").GetHashCode();

            // Add the field
            if (Field != null)
            {
                hashCode += Field.GetHashCode();
            }

            // Add the hints
            if (!string.IsNullOrEmpty(Hints))
            {
                hashCode += Hints.GetHashCode();
            }

            // Set and return the hashcode
            return (m_hashCode = hashCode).Value;
        }

        /// <summary>
        /// Compares the <see cref="MaximumAllRequest"/> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equals.</returns>
        public override bool Equals(object obj)
        {
            return obj?.GetHashCode() == GetHashCode();
        }

        /// <summary>
        /// Compares the <see cref="MaximumAllRequest"/> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(MaximumAllRequest other)
        {
            return other?.GetHashCode() == GetHashCode();
        }

        /// <summary>
        /// Compares the equality of the two <see cref="MaximumAllRequest"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="MaximumAllRequest"/> object.</param>
        /// <param name="objB">The second <see cref="MaximumAllRequest"/> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(MaximumAllRequest objA, MaximumAllRequest objB)
        {
            if (ReferenceEquals(null, objA))
            {
                return ReferenceEquals(null, objB);
            }
            return objB?.GetHashCode() == objA.GetHashCode();
        }

        /// <summary>
        /// Compares the inequality of the two <see cref="MaximumAllRequest"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="MaximumAllRequest"/> object.</param>
        /// <param name="objB">The second <see cref="MaximumAllRequest"/> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(MaximumAllRequest objA, MaximumAllRequest objB)
        {
            return (objA == objB) == false;
        }

        #endregion
    }
}
