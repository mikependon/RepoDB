using RepoDb.Attributes;
using RepoDb.Extensions;
using RepoDb.Resolvers;
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
        /// <summary>
        /// Creates a new instance of <see cref="ClassProperty"/> object.
        /// </summary>
        /// <param name="property">The wrapped property.</param>
        public ClassProperty(PropertyInfo property) :
            this(property.DeclaringType, property)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="ClassProperty"/> object.
        /// </summary>
        /// <param name="parentType">The declaring type (avoiding the interface collision).</param>
        /// <param name="property">The wrapped property.</param>
        public ClassProperty(Type parentType,
            PropertyInfo property)
        {
            DeclaringType = parentType;
            PropertyInfo = property;
        }

        #region Properties

        /// <summary>
        /// Gets the original declaring type (avoiding the interface collision).
        /// </summary>
        public Type DeclaringType { get; }

        /// <summary>
        /// Gets the wrapped property of this object.
        /// </summary>
        public PropertyInfo PropertyInfo { get; }

        #endregion

        #region Methods

        /*
         * AsField
         */

        private Field m_field;

        /// <summary>
        /// Convert the <see cref="ClassProperty"/> into a <see cref="Field"/> objects.
        /// </summary>
        /// <returns>An instance of <see cref="string"/> object.</returns>
        public Field AsField()
        {
            if (m_field != null)
            {
                return m_field;
            }

            // Set the properties
            m_field = new Field(GetMappedName(), PropertyInfo.PropertyType);

            // Return the value
            return m_field;
        }

        /*
         * GetPrimaryAttribute
         */

        private bool m_isPrimaryAttributeWasSet;
        private PrimaryAttribute m_primaryAttribute;

        /// <summary>
        /// Gets the primary attribute if present.
        /// </summary>
        /// <returns>The primary attribute if present.</returns>
        public PrimaryAttribute GetPrimaryAttribute()
        {
            if (m_isPrimaryAttributeWasSet)
            {
                return m_primaryAttribute;
            }
            m_isPrimaryAttributeWasSet = true;
            return m_primaryAttribute = PropertyInfo.GetCustomAttribute(typeof(PrimaryAttribute)) as PrimaryAttribute;
        }

        /*
         * GetIdentityAttribute
         */
        private bool m_isIdentityAttributeWasSet;
        private IdentityAttribute m_identityAttribute;

        /// <summary>
        /// Gets the identity attribute if present.
        /// </summary>
        /// <returns>The identity attribute if present.</returns>
        public IdentityAttribute GetIdentityAttribute()
        {
            if (m_isIdentityAttributeWasSet)
            {
                return m_identityAttribute;
            }
            m_isIdentityAttributeWasSet = true;
            return m_identityAttribute = PropertyInfo.GetCustomAttribute(typeof(IdentityAttribute)) as IdentityAttribute;
        }

        /*
         * IsPrimary
         */
        private bool? m_isPrimary;

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

            // Primary Attribute
            m_isPrimary = (GetPrimaryAttribute() != null);
            if (m_isPrimary == true)
            {
                return m_isPrimary;
            }

            // PrimaryMapper
            var classProperty = PrimaryMapper.Get(GetDeclaringType());
            m_isPrimary = (classProperty == this);
            if (m_isPrimary == true)
            {
                return m_isPrimary;
            }

            // Id Property
            m_isPrimary = string.Equals(PropertyInfo.Name, "id", StringComparison.OrdinalIgnoreCase);
            if (m_isPrimary == true)
            {
                return m_isPrimary;
            }

            // Type.Name + Id
            m_isPrimary = string.Equals(PropertyInfo.Name, string.Concat(GetDeclaringType().Name, "id"), StringComparison.OrdinalIgnoreCase);
            if (m_isPrimary == true)
            {
                return m_isPrimary;
            }

            // Mapping.Name + Id
            m_isPrimary = string.Equals(PropertyInfo.Name, string.Concat(ClassMappedNameCache.Get(GetDeclaringType()), "id"), StringComparison.OrdinalIgnoreCase);
            if (m_isPrimary == true)
            {
                return m_isPrimary;
            }

            // Return false
            return (m_isPrimary = false);
        }

        /*
         * IsIdentity
         */

        private bool? m_isIdentity;

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

            // Identity Attribute
            m_isIdentity = (GetIdentityAttribute() != null);
            if (m_isIdentity == true)
            {
                return m_isIdentity;
            }

            // PrimaryMapper
            var classProperty = IdentityMapper.Get(GetDeclaringType());
            m_isIdentity = (classProperty == this);
            if (m_isIdentity == true)
            {
                return m_isIdentity;
            }

            // Return false
            return (m_isIdentity = false);
        }

        /*
         * GetDbType
         */
        private ClientTypeToDbTypeResolver m_clientTypeToSqlDbTypeResolver = new ClientTypeToDbTypeResolver();
        private bool m_isDbTypeWasSet;
        private DbType? m_dbType;

        /// <summary>
        /// Gets the mapped <see cref="DbType"/> for the current property.
        /// </summary>
        /// <returns>The mapped <see cref="DbType"/> value.</returns>
        public DbType? GetDbType()
        {
            // We cannot use the NULL comparer for m_dbType object
            // as the value could actually be null in reality
            if (m_isDbTypeWasSet)
            {
                return m_dbType;
            }

            // Set the flag
            m_isDbTypeWasSet = true;

            // Get the type (underlying type)
            var propertyType = PropertyInfo.PropertyType.GetUnderlyingType();

            // Property and Type level mapping
            m_dbType = PropertyInfo.GetCustomAttribute<TypeMapAttribute>()?.DbType ??
                TypeMapper.Get(GetDeclaringType(), PropertyInfo) ?? // Property Level
                TypeMapper.Get(propertyType); // Type Level

            // Try to resolve if not found
            if (m_dbType == null && propertyType.IsEnum == false)
            {
                m_dbType = m_clientTypeToSqlDbTypeResolver.Resolve(propertyType);
            }

            // Return the value
            return m_dbType;
        }

        /*
         * GetName
         */

        private string m_mappedName;

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
            return m_mappedName = PropertyMappedNameCache.Get(GetDeclaringType(), PropertyInfo);
        }

        /// <summary>
        /// Returns the string that represent the current <see cref="ClassProperty"/> object.
        /// </summary>
        /// <returns>The unquoted name.</returns>
        public override string ToString()
        {
            return string.Concat(GetMappedName(), " (", PropertyInfo.PropertyType.Name, ")");
        }

        /// <summary>
        /// Gets the declaring parent type of the current property info. If the class inherits an interface, then this will return 
        /// the derived class type instead (if there is), otherwise the <see cref="PropertyInfo.DeclaringType"/> property.
        /// </summary>
        /// <returns>The declaring type.</returns>
        public Type GetDeclaringType()
        {
            return (DeclaringType ?? PropertyInfo.DeclaringType);
        }

        #endregion

        #region Comparers

        /// <summary>
        /// Returns the hashcode of the <see cref="PropertyInfo"/> object of this instance.
        /// </summary>
        /// <returns>The hash code value.</returns>
        public override int GetHashCode()
        {
            return GetDeclaringType().FullName.GetHashCode() ^ PropertyInfo.GenerateCustomizedHashCode();
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
                return PropertyInfo.Equals(((ClassProperty)obj).PropertyInfo);
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
            return PropertyInfo.Equals(other.PropertyInfo);
        }


        /// <summary>
        /// Compares the equality of the two <see cref="ClassProperty"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="ClassProperty"/> object.</param>
        /// <param name="objB">The second <see cref="ClassProperty"/> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(ClassProperty objA, ClassProperty objB)
        {
            if (ReferenceEquals(null, objA))
            {
                return ReferenceEquals(null, objB);
            }
            return objB?.GetHashCode() == objA.GetHashCode();
        }

        /// <summary>
        /// Compares the inequality of the two <see cref="ClassProperty"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="ClassProperty"/> object.</param>
        /// <param name="objB">The second <see cref="ClassProperty"/> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(ClassProperty objA, ClassProperty objB)
        {
            return (objA == objB) == false;
        }

        #endregion
    }
}
