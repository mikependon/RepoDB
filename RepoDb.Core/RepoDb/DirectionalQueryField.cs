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

        /// <summary>
        /// Creates a new instance of <see cref="DirectionalQueryField"/> object.
        /// </summary>
        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="type">The type of the parameter object.</param>
        /// <param name="direction">The direction to be used for the parameter object.</param>
        public DirectionalQueryField(string fieldName,
            Type type,
            ParameterDirection direction)
            : this(fieldName,
                  Operation.Equal,
                  null,
                  direction)
        {
            Type = type;
        }

        /// <summary>
        /// Creates a new instance of <see cref="DirectionalQueryField"/> object.
        /// </summary>
        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        /// <param name="direction">The direction to be used for the parameter object.</param>
        public DirectionalQueryField(string fieldName,
            object value,
            ParameterDirection direction)
            : this(fieldName,
                  Operation.Equal,
                  value,
                  direction)
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
            ParameterDirection direction)
            : this(fieldName,
                  operation,
                  value,
                  false,
                  direction)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="DirectionalQueryField"/> object.
        /// </summary>
        /// <param name="field">The actual field for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        /// <param name="direction">The direction to be used for the parameter object.</param>
        public DirectionalQueryField(Field field,
            object value,
            ParameterDirection direction)
            : this(field,
                  Operation.Equal,
                  value,
                  false,
                  direction)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="DirectionalQueryField"/> object.
        /// </summary>
        /// <param name="field">The actual field for the query expression.</param>
        /// <param name="operation">The operation to be used for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        /// <param name="direction">The direction to be used for the parameter object.</param>
        public DirectionalQueryField(Field field,
            Operation operation,
            object value,
            ParameterDirection direction)
            : this(field,
                  operation,
                  value,
                  false,
                  direction)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="DirectionalQueryField"/> object.
        /// </summary>
        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="operation">The operation to be used for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        /// <param name="appendUnderscore">The value to identify whether the underscore prefix will be appended to the parameter name.</param>
        /// <param name="direction">The direction to be used for the parameter object.</param>
        internal DirectionalQueryField(string fieldName,
            Operation operation,
            object value,
            bool appendUnderscore,
            ParameterDirection direction)
            : this(new Field(fieldName),
                  operation,
                  value,
                  appendUnderscore,
                  direction)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="DirectionalQueryField"/> object.
        /// </summary>
        /// <param name="field">The actual field for the query expression.</param>
        /// <param name="operation">The operation to be used for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        /// <param name="appendUnderscore">The value to identify whether the underscore prefix will be appended to the parameter name.</param>
        /// <param name="direction">The direction to be used for the parameter object.</param>
        internal DirectionalQueryField(Field field,
            Operation operation,
            object value,
            bool appendUnderscore,
            ParameterDirection direction)
            : base(field, operation, value, appendUnderscore)
        {
            Direction = direction;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the the value of the parameter direction currently in used.
        /// </summary>
        public ParameterDirection Direction { get; }

        /// <summary>
        /// Gets the type of the parameter.
        /// </summary>
        internal Type Type { get; }

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
            hashCode += base.GetHashCode();

            // Add the parameter direction
            hashCode += Direction.GetHashCode();

            // Set and return the hashcode
            return (this.hashCode = hashCode).Value;
        }

        /// <summary>
        /// Compares the <see cref="DirectionalQueryField"/> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equals.</returns>
        public override bool Equals(object obj) =>
            obj?.GetHashCode() == GetHashCode();

        /// <summary>
        /// Compares the <see cref="DirectionalQueryField"/> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(DirectionalQueryField other) =>
            other?.GetHashCode() == GetHashCode();

        /// <summary>
        /// Compares the equality of the two <see cref="DirectionalQueryField"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="DirectionalQueryField"/> object.</param>
        /// <param name="objB">The second <see cref="DirectionalQueryField"/> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(DirectionalQueryField objA,
            DirectionalQueryField objB)
        {
            if (ReferenceEquals(null, objA))
            {
                return ReferenceEquals(null, objB);
            }
            return objB?.GetHashCode() == objA.GetHashCode();
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
