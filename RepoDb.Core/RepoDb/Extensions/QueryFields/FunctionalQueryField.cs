using RepoDb.Enumerations;
using RepoDb.Interfaces;
using System;
using System.Data;

namespace RepoDb.Extensions.QueryFields
{
    /// <summary>
    /// A dynamic functional-based <see cref="QueryField"/> object. This requires a properly constructed
    /// formatted string (for a specific database function) in order to work properly.
    /// </summary>
    /// <example>
    /// See sample code below that uses a TRIM function.
    /// <code>
    ///     var where = new FunctionalQueryField("ColumnName", "Value", "TRIM({0})");
    ///     var result = connection.Query&lt;Entity&gt;(where);
    /// </code>
    /// </example>
    public class FunctionalQueryField : QueryField, IEquatable<FunctionalQueryField>
    {
        private int? hashCode = null;

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="FunctionalQueryField"/> object.
        /// </summary>
        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="operation">The operation to be used for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        /// <param name="dbType">The database type to be used for the query expression.</param>
        /// <param name="format">The properly constructed format of the target function to be used.</param>
        public FunctionalQueryField(string fieldName,
            Operation operation,
            object value,
            DbType? dbType,
            string format = null)
            : base(fieldName, operation, value, dbType)
        {
            Format = format;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the properly constructed format of the target function.
        /// </summary>
        public string Format { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the string representations (column-value pairs) of the current <see cref="QueryField"/> object with the formatted-function transformations.
        /// </summary>
        /// <param name="index">The target index.</param>
        /// <param name="dbSetting">The database setting currently in used.</param>
        /// <returns>The string representations of the current <see cref="QueryField"/> object using the LOWER function.</returns>
        public override string GetString(int index,
            IDbSetting dbSetting) =>
            base.GetString(index, Format, dbSetting);

        #endregion

        #region Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <see cref="FunctionalQueryField"/>.
        /// </summary>
        /// <returns>The hashcode value.</returns>
        public override int GetHashCode()
        {
            if (this.hashCode != null)
            {
                return this.hashCode.Value;
            }

            // FullName: This is to ensure that even the user has created an identical formatting 
            //  on the derived class with the existing classes, the Type.FullName could still 
            // differentiate the instances
            var hashCode = GetType().FullName.GetHashCode();

            // Base
            hashCode = HashCode.Combine(hashCode, base.GetHashCode());

            // Format
            if (Format != null)
            {
                hashCode = HashCode.Combine(hashCode, Format);
            }

            // Return
            return (this.hashCode = hashCode).Value;
        }

        /// <summary>
        /// Compares the <see cref="FunctionalQueryField"/> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equals.</returns>
        public override bool Equals(object obj)
        {
            if (obj is null) return false;

            return obj.GetHashCode() == GetHashCode();
        }

        /// <summary>
        /// Compares the <see cref="FunctionalQueryField"/> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(FunctionalQueryField other)
        {
            if (other is null) return false;

            return other.GetHashCode() == GetHashCode();
        }

        /// <summary>
        /// Compares the equality of the two <see cref="FunctionalQueryField"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="FunctionalQueryField"/> object.</param>
        /// <param name="objB">The second <see cref="FunctionalQueryField"/> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(FunctionalQueryField objA,
            FunctionalQueryField objB)
        {
            if (objA is null)
            {
                return objB is null;
            }
            return objA.Equals(objB);
        }

        /// <summary>
        /// Compares the inequality of the two <see cref="FunctionalQueryField"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="FunctionalQueryField"/> object.</param>
        /// <param name="objB">The second <see cref="FunctionalQueryField"/> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(FunctionalQueryField objA,
            FunctionalQueryField objB) =>
            (objA == objB) == false;

        #endregion
    }
}
