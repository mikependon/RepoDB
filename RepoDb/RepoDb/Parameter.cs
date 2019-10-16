using RepoDb.Extensions;
using RepoDb.Interfaces;
using System;

namespace RepoDb
{
    /// <summary>
    /// An object that holds the value of the field parameter.
    /// </summary>
    public sealed class Parameter : IEquatable<Parameter>
    {
        private int m_hashCode = 0;

        /// <summary>
        /// Creates a new instance of <see cref="Parameter"/> object.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <param name="prependUnderscore">The value to identify whether the underscope prefix will be prepended.</param>
        /// <param name="dbSetting">The database setting that is currently in used.</param>
        public Parameter(string name,
            object value,
            bool prependUnderscore,
            IDbSetting dbSetting)
        {
            // Name is required
            if (string.IsNullOrEmpty(name))
            {
                throw new NullReferenceException(name);
            }

            // Set the properties
            DbSetting = dbSetting;
            Name = name.AsAlphaNumeric(true);
            Value = value;
            if (prependUnderscore)
            {
                PrependAnUnderscore();
            }

            // Set the hashcode
            m_hashCode = Name.GetHashCode();
            //if (dbSetting != null)
            //{
            //    m_hashCode += dbSetting.GetHashCode();
            //}
        }

        #region Properties

        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the value of the parameter.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Gets the database setting currently in used.
        /// </summary>
        public IDbSetting DbSetting { get; }

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
        /// <param name="dbSetting">The database setting that is currently in used.</param>
        internal void SetName(string name,
            IDbSetting dbSetting)
        {
            Name = name.AsUnquoted(dbSetting);
        }

        /// <summary>
        /// Stringify the current object. Will return the format of <b>Name (Value)</b> text.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Concat("@", Name, " (", Value, ")");
        }

        #endregion

        #region Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <see cref="Parameter"/>.
        /// </summary>
        /// <returns>The hashcode value.</returns>
        public override int GetHashCode()
        {
            return m_hashCode;
        }

        /// <summary>
        /// Compares the <see cref="Parameter"/> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equals.</returns>
        public override bool Equals(object obj)
        {
            //var hashCode = obj?.GetHashCode();
            //if (obj is string)
            //{
            //    if (DbSetting != null)
            //    {
            //        hashCode += DbSetting.GetHashCode();
            //    }
            //}
            //return hashCode == GetHashCode();
            return obj?.GetHashCode() == GetHashCode();
        }

        /// <summary>
        /// Compares the <see cref="Parameter"/> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(Parameter other)
        {
            return other?.GetHashCode() == GetHashCode();
        }

        /// <summary>
        /// Compares the equality of the two <see cref="Parameter"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="Parameter"/> object.</param>
        /// <param name="objB">The second <see cref="Parameter"/> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(Parameter objA, Parameter objB)
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
        public static bool operator !=(Parameter objA, Parameter objB)
        {
            return (objA == objB) == false;
        }

        #endregion
    }
}