using RepoDb.Extensions;
using System;

namespace RepoDb
{
    /// <summary>
    /// A class that holds the value of the field parameter.
    /// </summary>
    public sealed class Parameter : IEquatable<Parameter>
    {
        private int? hashCode = null;

        /// <summary>
        /// Creates a new instance of <see cref="Parameter"/> object.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <param name="prependUnderscore">The value to identify whether the underscope prefix will be prepended.</param>
        public Parameter(string name,
            object value,
            bool prependUnderscore)
        {
            // Name is required
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new NullReferenceException(name);
            }

            // Set the properties
            OriginalName = name.AsAlphaNumeric();
            Name = OriginalName;
            OriginalValue = value;
            Value = value;
            if (prependUnderscore)
            {
                PrependAnUnderscore();
            }
        }

        #region Properties

        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the original name of the parameter.
        /// </summary>
        private string OriginalName { get; }

        /// <summary>
        /// Gets the value of the parameter.
        /// </summary>
        public object Value { get; private set; }

        /// <summary>
        /// Gets the original value of the parameter.
        /// </summary>
        private object OriginalValue { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Prepend an underscore on the current parameter object.
        /// </summary>
        internal void PrependAnUnderscore()
        {
            if (!Name.StartsWith("_"))
            {
                Name = string.Concat("_", Name);
            }
        }

        /// <summary>
        /// Set the name of the parameter.
        /// </summary>
        /// <param name="name">The new name.</param>
        internal void SetName(string name) =>
            Name = name;

        /// <summary>
        /// Set the value of the parameter.
        /// </summary>
        /// <param name="value">The new value.</param>
        internal void SetValue(object value) =>
            Value = value;

        /// <summary>
        /// Resets the <see cref="Parameter"/> object back to its default state (as is newly instantiated).
        /// </summary>
        public void Reset()
        {
            Name = OriginalName;
            Value = OriginalValue;
            hashCode = null;
        }

        /// <summary>
        /// Stringify the current object. Will return the format of <b>Name (Value)</b> text.
        /// </summary>
        /// <returns></returns>
        public override string ToString() =>
            string.Concat(Name, " (", Value, ")");

        #endregion

        #region Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <see cref="Parameter"/>.
        /// </summary>
        /// <returns>The hashcode value.</returns>
        public override int GetHashCode()
        {
            if (this.hashCode != null)
            {
                return this.hashCode.Value;
            }

            // Set and return the hashcode
            return (this.hashCode = OriginalName.GetHashCode()).Value;
        }

        /// <summary>
        /// Compares the <see cref="Parameter"/> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equals.</returns>
        public override bool Equals(object obj) =>
            obj?.GetHashCode() == GetHashCode();

        /// <summary>
        /// Compares the <see cref="Parameter"/> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(Parameter other) =>
            other?.GetHashCode() == GetHashCode();

        /// <summary>
        /// Compares the equality of the two <see cref="Parameter"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="Parameter"/> object.</param>
        /// <param name="objB">The second <see cref="Parameter"/> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(Parameter objA,
            Parameter objB)
        {
            if (ReferenceEquals(null, objA))
            {
                return ReferenceEquals(null, objB);
            }
            return objB?.GetHashCode() == objA.GetHashCode();
        }

        /// <summary>
        /// Compares the inequality of the two <see cref="Parameter"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="Parameter"/> object.</param>
        /// <param name="objB">The second <see cref="Parameter"/> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(Parameter objA,
            Parameter objB) =>
            (objA == objB) == false;

        #endregion
    }
}