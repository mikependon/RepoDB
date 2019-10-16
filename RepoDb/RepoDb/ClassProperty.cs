using RepoDb.Attributes;
using RepoDb.Extensions;
using RepoDb.Interfaces;
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
        /// <param name="dbSetting">The database setting to be used.</param>
        public ClassProperty(PropertyInfo property,
            IDbSetting dbSetting)
        {
            PropertyInfo = property;
            DbSetting = dbSetting;
        }

        #region Properties

        /// <summary>
        /// Gets the wrapped property of this object.
        /// </summary>
        public PropertyInfo PropertyInfo { get; }

        /// <summary>
        /// Gets the database setting that is currently in used.
        /// </summary>
        public IDbSetting DbSetting { get; }

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
            m_field = new Field(GetUnquotedMappedName(), PropertyInfo.PropertyType);

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

            // Id Property
            m_isPrimary = string.Equals(PropertyInfo.Name, "id", StringComparison.OrdinalIgnoreCase);
            if (m_isPrimary == true)
            {
                return m_isPrimary;
            }

            // Type.Name + Id
            m_isPrimary = string.Equals(PropertyInfo.Name, string.Concat(PropertyInfo.DeclaringType.Name, "id"), StringComparison.OrdinalIgnoreCase);
            if (m_isPrimary == true)
            {
                return m_isPrimary;
            }

            // Mapping.Name + Id
            m_isPrimary = string.Equals(PropertyInfo.Name, string.Concat(ClassMappedNameCache.Get(PropertyInfo.DeclaringType, true, DbSetting), "id"), StringComparison.OrdinalIgnoreCase);
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
            return m_isIdentity = (GetIdentityAttribute() != null);
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
                TypeMapper.Get(propertyType);

            // Try to resolve if not found
            if (m_dbType == null && propertyType.IsEnum == false)
            {
                m_dbType = m_clientTypeToSqlDbTypeResolver.Resolve(PropertyInfo.PropertyType);
            }

            // Return the value
            return m_dbType;
        }

        /*
         * GetQuotedMappedName
         */

        private string m_quotedMappedName;

        /// <summary>
        /// Gets the quoted mapped-name for the current property.
        /// </summary>
        /// <returns>The quoted mapped-name value.</returns>
        public string GetQuotedMappedName()
        {
            if (m_quotedMappedName != null)
            {
                return m_quotedMappedName;
            }
            return m_quotedMappedName = PropertyMappedNameCache.Get(PropertyInfo, true, DbSetting);
        }

        /*
         * GetUnquotedMappedName
         */

        private string m_unquotedMappedName;

        /// <summary>
        /// Gets the unquoted mapped-name for the current property.
        /// </summary>
        /// <returns>The unquoted mapped-name value.</returns>
        public string GetUnquotedMappedName()
        {
            if (m_unquotedMappedName != null)
            {
                return m_unquotedMappedName;
            }
            return m_unquotedMappedName = PropertyMappedNameCache.Get(PropertyInfo, false, DbSetting);
        }

        /// <summary>
        /// Returns the string that represent the current <see cref="ClassProperty"/> object.
        /// </summary>
        /// <returns>The unquoted name.</returns>
        public override string ToString()
        {
            return string.Concat(m_quotedMappedName ?? PropertyInfo.Name, " (", PropertyInfo.PropertyType.Name, ")");
        }

        #endregion

        #region Comparers

        /// <summary>
        /// Returns the hashcode of the <see cref="PropertyInfo"/> object of this instance.
        /// </summary>
        /// <returns>The hash code value.</returns>
        public override int GetHashCode()
        {
            return PropertyInfo.GetHashCode();
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
