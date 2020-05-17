using RepoDb.Attributes;
using RepoDb.Extensions;
using System;
using System.ComponentModel.DataAnnotations;
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
        /// Gets the <see cref="PrimaryAttribute"/> if present.
        /// </summary>
        /// <returns>The instance of <see cref="PrimaryAttribute"/>.</returns>
        public PrimaryAttribute GetPrimaryAttribute()
        {
            if (m_isPrimaryAttributeWasSet)
            {
                return m_primaryAttribute;
            }
            m_isPrimaryAttributeWasSet = true;
            return m_primaryAttribute = PropertyInfo.GetCustomAttribute<PrimaryAttribute>() ??
                (PropertyInfo.GetCustomAttribute<KeyAttribute>() != null ? new PrimaryAttribute() : null);
        }

        /*
         * GetIdentityAttribute
         */
        private bool m_isIdentityAttributeWasSet;
        private IdentityAttribute m_identityAttribute;

        /// <summary>
        /// Gets the <see cref="IdentityAttribute"/> if present.
        /// </summary>
        /// <returns>The instance of <see cref="IdentityAttribute"/>.</returns>
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
         * GetTypeMapAttribute
         */
        private bool m_isTypeMapAttributeWasSet;
        private TypeMapAttribute m_typeMapAttribute;

        /// <summary>
        /// Gets the <see cref="TypeMapAttribute"/> if present.
        /// </summary>
        /// <returns>The instance of <see cref="TypeMapAttribute"/>.</returns>
        public TypeMapAttribute GetTypeMapAttribute()
        {
            if (m_isTypeMapAttributeWasSet)
            {
                return m_typeMapAttribute;
            }
            m_isTypeMapAttributeWasSet = true;
            return m_typeMapAttribute = PropertyInfo.GetCustomAttribute(typeof(TypeMapAttribute)) as TypeMapAttribute;
        }

        /*
         * GetPropertyHandlerAttribute
         */
        private bool m_isPropertyHandlerAttributeWasSet;
        private PropertyHandlerAttribute m_propertyHandlerAttribute;

        /// <summary>
        /// Gets the <see cref="PropertyHandlerAttribute"/> if present.
        /// </summary>
        /// <returns>The instance of <see cref="PropertyHandlerAttribute"/>.</returns>
        public PropertyHandlerAttribute GetPropertyHandlerAttribute()
        {
            if (m_isPropertyHandlerAttributeWasSet)
            {
                return m_propertyHandlerAttribute;
            }
            m_isPropertyHandlerAttributeWasSet = true;
            return m_propertyHandlerAttribute = PropertyInfo.GetCustomAttribute(typeof(PropertyHandlerAttribute)) as PropertyHandlerAttribute;
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

            // Get the value from the cache
            return (m_isPrimary = PrimaryCache.Get(GetDeclaringType()) != null);
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

            // Get the value from the cache
            return (m_isIdentity = IdentityCache.Get(GetDeclaringType()) != null);
        }

        /*
         * GetDbType
         */
        private bool m_isDbTypeWasSet;
        private DbType? m_dbType;

        /// <summary>
        /// Gets the mapped <see cref="DbType"/> for the current property.
        /// </summary>
        /// <returns>The mapped <see cref="DbType"/> value.</returns>
        public DbType? GetDbType()
        {
            if (m_isDbTypeWasSet == true)
            {
                return m_dbType;
            }

            // Set the flag
            m_isDbTypeWasSet = true;

            // Return the value
            return m_dbType = TypeMapCache.Get(GetDeclaringType(), PropertyInfo);
        }

        /*
         * GetPropertyHandler
         */
        private bool m_propertyHandlerWasSet;
        private object m_propertyHandler;

        /// <summary>
        /// Gets the mapped property handler object for the current property.
        /// </summary>
        /// <returns>The mapped property handler object.</returns>
        public object GetPropertyHandler() =>
            GetPropertyHandler<object>();

        /// <summary>
        /// Gets the mapped property handler object for the current property.
        /// </summary>
        /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
        /// <returns>The mapped property handler object.</returns>
        public TPropertyHandler GetPropertyHandler<TPropertyHandler>()
        {
            if (m_propertyHandlerWasSet == true)
            {
                return Converter.ToType<TPropertyHandler>(m_propertyHandler);
            }

            // Set the flag
            m_propertyHandlerWasSet = true;

            // Set the instance
            m_propertyHandler = PropertyHandlerCache.Get<TPropertyHandler>(GetDeclaringType(), PropertyInfo);

            // Return the value
            return Converter.ToType<TPropertyHandler>(m_propertyHandler);
        }

        /*
         * GetMappedName
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
