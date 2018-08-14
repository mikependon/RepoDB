using RepoDb.Attributes;
using RepoDb.Extensions;
using System;
using System.Data;
using System.Reflection;

namespace RepoDb
{
    /// <summary>
    /// A class that wraps the property object.
    /// </summary>
    public class ClassProperty : IEquatable<ClassProperty>
    {
        #region Privates

        private bool? m_isIdentity;
        private bool? m_isPrimary;
        private bool m_isDbTypeWasSet;
        private DbType? m_dbType;
        private string m_mappedName;

        #endregion

        /// <summary>
        /// Creates a new instance of <i>ClassProperty</i> object.
        /// </summary>
        /// <param name="property">The wrapped property.</param>
        public ClassProperty(PropertyInfo property)
        {
            Property = property;
        }

        #region Properties

        /// <summary>
        /// Gets the wrapped property of this object.
        /// </summary>
        public PropertyInfo Property
        {
            get;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a value whether the current property is a primary property.
        /// </summary>
        /// <returns>True if the current property is a primary.</returns>
        public bool? IsPrimary()
        {
            if (m_isPrimary != null)
            {
                return m_isPrimary;
            }
            return m_isPrimary = (Property.GetCustomAttribute(typeof(PrimaryAttribute)) != null);
        }

        /// <summary>
        /// Gets a value whether the current property is an identity or not.
        /// </summary>
        /// <returns>True if the current property is an identity.</returns>
        public bool? IsIdentity()
        {
            if (m_isIdentity != null)
            {
                return m_isIdentity;
            }
            return m_isIdentity = (Property.GetCustomAttribute(typeof(IdentityAttribute)) != null);
        }

        /// <summary>
        /// Gets the mapped <i>DbType</i> for the current property.
        /// </summary>
        /// <returns>The mapped <i>DbType</i> value.</returns>
        public DbType? GetDbType()
        {
            // We cannot use the NULL comparer for m_dbType object
            // as the value could actually be null in reality
            if (m_isDbTypeWasSet)
            {
                return m_dbType;
            }
            m_isDbTypeWasSet = true;
            return m_dbType = Property.GetCustomAttribute<TypeMapAttribute>()?.DbType ??
                TypeMapper.Get(GetUnderlyingType(Property.PropertyType))?.DbType;
        }

        /// <summary>
        /// Gets the mapped-name for the current property.
        /// </summary>
        /// <returns>The mapped-name value.</returns>
        public string GetMappedName()
        {
            if (m_mappedName != null)
            {
                return m_mappedName;
            }
            return m_mappedName = ClassExpression.GetPropertyMappedName(Property);
        }

        /// <summary>
        /// Gets the underlying type if present.
        /// </summary>
        /// <param name="type">The type where to get the underlying type.</param>
        /// <returns>The underlying type.</returns>
        private static Type GetUnderlyingType(Type type)
        {
            return type != null ? Nullable.GetUnderlyingType(type) ?? type : null;
        }

        #endregion

        #region Comparers

        /// <summary>
        /// Returns the hashcode of the <i>Property</i> property of this instance.
        /// </summary>
        /// <returns>The hash code value.</returns>
        public override int GetHashCode()
        {
            return Property.GetHashCode();
        }

        /// <summary>
        /// Compare the current instance to the other object instance.
        /// </summary>
        /// <param name="obj">The object to be compared.</param>
        /// <returns>True if the two instance is the same.</returns>
        public override bool Equals(object obj)
        {
            if (obj is ClassProperty)
            {
                return Property.Equals(((ClassProperty)obj).Property);
            }
            return Equals(obj);
        }

        /// <summary>
        /// Compare the current instance to the other object instance.
        /// </summary>
        /// <param name="other">The object to be compared.</param>
        /// <returns>True if the two instance is the same.</returns>
        public bool Equals(ClassProperty other)
        {
            return Property.Equals(other.Property);
        }


        /// <summary>
        /// Compares the equality of the two <i>ClassProperty</i> objects.
        /// </summary>
        /// <param name="objA">The first <i>ClassProperty</i> object.</param>
        /// <param name="objB">The second <i>ClassProperty</i> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(ClassProperty objA, ClassProperty objB)
        {
            if (ReferenceEquals(null, objA))
            {
                return ReferenceEquals(null, objB);
            }
            return objA?.GetHashCode() == objB?.GetHashCode();
        }

        /// <summary>
        /// Compares the inequality of the two <i>ClassProperty</i> objects.
        /// </summary>
        /// <param name="objA">The first <i>ClassProperty</i> object.</param>
        /// <param name="objB">The second <i>ClassProperty</i> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(ClassProperty objA, ClassProperty objB)
        {
            return (objA == objB) == false;
        }

        #endregion
    }
}
