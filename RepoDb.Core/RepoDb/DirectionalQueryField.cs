using RepoDb.Enumerations;
using System;
using System.Data;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to define a <see cref="QueryField" /> object query expression that allow the setting of the <see cref="ParameterDirection"/> property
    /// of the <see cref="IDbDataParameter"/> object. This is very useful if you wish to invoke a Stored Procedure that has an output parameter.
    /// </summary>
    public class DirectionalQueryField : QueryField, IEquatable<DirectionalQueryField>
    {
        private int? hashCode = null;

        #region Constructors

        #region NoValues

        /// <summary>
        /// Creates a new instance of <see cref="DirectionalQueryField"/> object.
        /// </summary>
        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="direction">The direction to be used for the parameter object.</param>
        public DirectionalQueryField(string fieldName,
            ParameterDirection? direction)
            : this(fieldName,
                  Operation.Equal,
                  null,
                  direction,
                  null,
                  null,
                  false)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="DirectionalQueryField"/> object.
        /// </summary>
        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="direction">The direction to be used for the parameter object.</param>
        /// <param name="size">The sizeof the parameter value.</param>
        public DirectionalQueryField(string fieldName,
            ParameterDirection? direction,
            int? size)
            : this(fieldName,
                  Operation.Equal,
                  null,
                  direction,
                  size,
                  null,
                  false)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="DirectionalQueryField"/> object.
        /// </summary>
        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="direction">The direction to be used for the parameter object.</param>
        /// <param name="size">The sizeof the parameter value.</param>
        /// <param name="dbType">The database type to be used for the query expression.</param>
        public DirectionalQueryField(string fieldName,
            ParameterDirection? direction,
            int? size,
            DbType? dbType)
            : this(fieldName,
                  Operation.Equal,
                  null,
                  direction,
                  size,
                  dbType,
                  false)
        { }

        #endregion

        #region WithValues

        /// <summary>
        /// Creates a new instance of <see cref="DirectionalQueryField"/> object.
        /// </summary>
        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        /// <param name="direction">The direction to be used for the parameter object.</param>
        public DirectionalQueryField(string fieldName,
            object value,
            ParameterDirection? direction)
            : this(fieldName,
                  Operation.Equal,
                  value,
                  direction,
                  null,
                  null,
                  false)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="DirectionalQueryField"/> object.
        /// </summary>
        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        /// <param name="direction">The direction to be used for the parameter object.</param>
        /// <param name="size">The sizeof the parameter value.</param>
        public DirectionalQueryField(string fieldName,
            object value,
            ParameterDirection? direction,
            int? size)
            : this(fieldName,
                  Operation.Equal,
                  value,
                  direction,
                  size,
                  null,
                  false)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="DirectionalQueryField"/> object.
        /// </summary>
        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        /// <param name="direction">The direction to be used for the parameter object.</param>
        /// <param name="size">The sizeof the parameter value.</param>
        /// <param name="dbType">The database type to be used for the query expression.</param>
        public DirectionalQueryField(string fieldName,
            object value,
            ParameterDirection? direction,
            int? size,
            DbType? dbType)
            : this(fieldName,
                  Operation.Equal,
                  value,
                  direction,
                  size,
                  dbType,
                  false)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="DirectionalQueryField"/> object.
        /// </summary>
        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="operation">The operation to be used for the query expression.</param>
        /// <param name="direction">The direction to be used for the parameter object.</param>
        public DirectionalQueryField(string fieldName,
            Operation operation,
            ParameterDirection? direction)
            : this(fieldName,
                  operation,
                  null,
                  direction,
                  null,
                  null,
                  false)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="DirectionalQueryField"/> object.
        /// </summary>
        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="operation">The operation to be used for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        /// <param name="direction">The direction to be used for the parameter object.</param>
        public DirectionalQueryField(string fieldName,
            Operation operation,
            object value,
            ParameterDirection? direction)
            : this(fieldName,
                  operation,
                  value,
                  direction,
                  null,
                  null,
                  false)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="DirectionalQueryField"/> object.
        /// </summary>
        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="operation">The operation to be used for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        /// <param name="direction">The direction to be used for the parameter object.</param>
        /// <param name="size">The sizeof the parameter value.</param>
        public DirectionalQueryField(string fieldName,
            Operation operation,
            object value,
            ParameterDirection? direction,
            int? size)
            : this(fieldName,
                  operation,
                  value,
                  direction,
                  size,
                  null,
                  false)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="DirectionalQueryField"/> object.
        /// </summary>
        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="operation">The operation to be used for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        /// <param name="direction">The direction to be used for the parameter object.</param>
        /// <param name="size">The sizeof the parameter value.</param>
        /// <param name="dbType">The database type to be used for the query expression.</param>
        public DirectionalQueryField(string fieldName,
            Operation operation,
            object value,
            ParameterDirection? direction,
            int? size,
            DbType? dbType)
            : this(fieldName,
                  operation,
                  value,
                  direction,
                  size,
                  dbType,
                  false)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="DirectionalQueryField"/> object.
        /// </summary>
        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="operation">The operation to be used for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        /// <param name="direction">The direction to be used for the parameter object.</param>
        /// <param name="size">The sizeof the parameter value.</param>
        /// <param name="dbType">The database type to be used for the query expression.</param>
        /// <param name="prependUnderscore">The value to identify whether the underscore prefix will be appended to the parameter name.</param>
        internal DirectionalQueryField(string fieldName,
            Operation operation,
            object value,
            ParameterDirection? direction,
            int? size,
            DbType? dbType,
            bool prependUnderscore = false)
            : base(fieldName, operation, value, dbType, prependUnderscore)
        {
            Direction = direction;
            Size = size;
        }

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// Gets the the value of the parameter direction currently in used.
        /// </summary>
        public ParameterDirection? Direction { get; }

        /// <summary>
        /// Gets the size of the parameter currently in used.
        /// </summary>
        public int? Size { get; }

        #endregion

        #region Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <see cref="DirectionalQueryField"/>.
        /// </summary>
        /// <returns>The hashcode value.</returns>
        public override int GetHashCode()
        {
            if (this.hashCode != null)
            {
                return this.hashCode.Value;
            }

            var hashCode = 0;

            // Get the hashcode of the base query field
            hashCode = HashCode.Combine(hashCode, base.GetHashCode());

            // Add the parameter direction
            hashCode = HashCode.Combine(hashCode, Direction);

            // Add the size
            if (Size.HasValue)
            {
                hashCode = HashCode.Combine(hashCode, Size.Value);
            }

            // Set and return the hashcode
            return (this.hashCode = hashCode).Value;
        }

        /// <summary>
        /// Compares the <see cref="DirectionalQueryField"/> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equals.</returns>
        public override bool Equals(object obj)
        {
            if (obj is null) return false;

            return obj.GetHashCode() == GetHashCode();
        }

        /// <summary>
        /// Compares the <see cref="DirectionalQueryField"/> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(DirectionalQueryField other)
        {
            if (other is null) return false;

            return other.GetHashCode() == GetHashCode();
        }

        /// <summary>
        /// Compares the equality of the two <see cref="DirectionalQueryField"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="DirectionalQueryField"/> object.</param>
        /// <param name="objB">The second <see cref="DirectionalQueryField"/> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(DirectionalQueryField objA,
            DirectionalQueryField objB)
        {
            if (objA is null)
            {
                return objB is null;
            }
            return objA.Equals(objB);
        }

        /// <summary>
        /// Compares the inequality of the two <see cref="DirectionalQueryField"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="DirectionalQueryField"/> object.</param>
        /// <param name="objB">The second <see cref="DirectionalQueryField"/> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(DirectionalQueryField objA,
            DirectionalQueryField objB) =>
            (objA == objB) == false;

        #endregion
    }
}
