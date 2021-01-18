using RepoDb.Attributes;
using RepoDb.Extensions;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Reflection;

namespace RepoDb
{
    /// <summary>
    /// A class that wraps the <see cref="PropertyInfo"/> object. This class is used to extract the information from the <see cref="System.Reflection.PropertyInfo"/> object in a fast and efficient manner.
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
            declaringType = parentType;
            PropertyInfo = property;
        }

        #region Properties

        /// <summary>
        /// Gets the original declaring type (avoiding the interface collision).
        /// </summary>
        private readonly Type declaringType;

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
        public override string ToString() =>
            string.Concat("ClassProperty :: Name = ", GetMappedName(), " (", PropertyInfo.PropertyType.FullName, "), ",
                "DeclaringType = ", GetDeclaringType().FullName);

        /// <summary>
        /// Gets the declaring parent type of the current property info. If the class inherits an interface, then this will return 
        /// the derived class type instead (if there is), otherwise the <see cref="PropertyInfo.DeclaringType"/> property.
        /// </summary>
        /// <returns>The declaring type.</returns>
        public Type GetDeclaringType() =>
            (declaringType ?? PropertyInfo.DeclaringType);

        /*
         * AsField
         */

        private Field field;

        /// <summary>
        /// Convert the <see cref="ClassProperty"/> into a <see cref="Field"/> objects.
        /// </summary>
        /// <returns>An instance of <see cref="string"/> object.</returns>
        public Field AsField()
        {
            if (field != null)
            {
                return field;
            }

            // Set the properties
            field = new Field(GetMappedName(), PropertyInfo.PropertyType);

            // Return the value
            return field;
        }

        /*
         * GetPrimaryAttribute
         */

        private bool isPrimaryAttributeWasSet;
        private PrimaryAttribute primaryAttribute;

        /// <summary>
        /// Gets the <see cref="PrimaryAttribute"/> if present.
        /// </summary>
        /// <returns>The instance of <see cref="PrimaryAttribute"/>.</returns>
        public PrimaryAttribute GetPrimaryAttribute()
        {
            if (isPrimaryAttributeWasSet)
            {
                return primaryAttribute;
            }
            isPrimaryAttributeWasSet = true;
            return primaryAttribute = PropertyInfo.GetCustomAttribute<PrimaryAttribute>() ??
                (PropertyInfo.GetCustomAttribute<KeyAttribute>() != null ? new PrimaryAttribute() : null);
        }

        /*
         * GetIdentityAttribute
         */
        private bool isIdentityAttributeWasSet;
        private IdentityAttribute identityAttribute;

        /// <summary>
        /// Gets the <see cref="IdentityAttribute"/> if present.
        /// </summary>
        /// <returns>The instance of <see cref="IdentityAttribute"/>.</returns>
        public IdentityAttribute GetIdentityAttribute()
        {
            if (isIdentityAttributeWasSet)
            {
                return identityAttribute;
            }
            isIdentityAttributeWasSet = true;
            return identityAttribute = PropertyInfo.GetCustomAttribute(StaticType.IdentityAttribute) as IdentityAttribute;
        }

        /*
         * GetTypeMapAttribute
         */
        private bool isTypeMapAttributeWasSet;
        private TypeMapAttribute typeMapAttribute;

        /// <summary>
        /// Gets the <see cref="TypeMapAttribute"/> if present.
        /// </summary>
        /// <returns>The instance of <see cref="TypeMapAttribute"/>.</returns>
        public TypeMapAttribute GetTypeMapAttribute()
        {
            if (isTypeMapAttributeWasSet)
            {
                return typeMapAttribute;
            }
            isTypeMapAttributeWasSet = true;
            return typeMapAttribute = PropertyInfo.GetCustomAttribute(StaticType.TypeMapAttribute) as TypeMapAttribute;
        }

        /*
         * GetPropertyHandlerAttribute
         */
        private bool isPropertyHandlerAttributeWasSet;
        private PropertyHandlerAttribute propertyHandlerAttribute;

        /// <summary>
        /// Gets the <see cref="PropertyHandlerAttribute"/> if present.
        /// </summary>
        /// <returns>The instance of <see cref="PropertyHandlerAttribute"/>.</returns>
        public PropertyHandlerAttribute GetPropertyHandlerAttribute()
        {
            if (isPropertyHandlerAttributeWasSet)
            {
                return propertyHandlerAttribute;
            }
            isPropertyHandlerAttributeWasSet = true;
            return propertyHandlerAttribute = PropertyInfo.GetCustomAttribute(StaticType.PropertyHandlerAttribute) as PropertyHandlerAttribute;
        }

        /*
         * IsPrimary
         */
        private bool? isPrimary;

        /// <summary>
        /// Gets a value whether the current property is a primary property.
        /// </summary>
        /// <returns>True if the current property is a primary.</returns>
        public bool? IsPrimary()
        {
            if (isPrimary != null)
            {
                return isPrimary;
            }

            // Get the value from the cache
            return (isPrimary = PrimaryCache.Get(GetDeclaringType()) != null);
        }

        /*
         * IsIdentity
         */

        private bool? isIdentity;

        /// <summary>
        /// Gets a value whether the current property is an identity or not.
        /// </summary>
        /// <returns>True if the current property is an identity.</returns>
        public bool? IsIdentity()
        {
            if (isIdentity != null)
            {
                return isIdentity;
            }

            // Get the value from the cache
            return (isIdentity = IdentityCache.Get(GetDeclaringType()) != null);
        }

        /*
         * GetDbType
         */
        private bool isDbTypeWasSet;
        private DbType? dbType;

        /// <summary>
        /// Gets the mapped <see cref="DbType"/> for the current property.
        /// </summary>
        /// <returns>The mapped <see cref="DbType"/> value.</returns>
        public DbType? GetDbType()
        {
            if (isDbTypeWasSet == true)
            {
                return dbType;
            }

            // Set the flag
            isDbTypeWasSet = true;

            // Return the value
            return dbType = TypeMapCache.Get(GetDeclaringType(), PropertyInfo) ?? TypeMapCache.Get(PropertyInfo.PropertyType);
        }

        /*
         * GetPropertyHandler
         */
        private bool propertyHandlerWasSet;
        private object propertyHandler;

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
            if (propertyHandlerWasSet == true)
            {
                return Converter.ToType<TPropertyHandler>(propertyHandler);
            }

            // Set the flag
            propertyHandlerWasSet = true;

            // Set the instance
            propertyHandler = PropertyHandlerCache.Get<TPropertyHandler>(GetDeclaringType(), PropertyInfo);

            // Return the value
            return Converter.ToType<TPropertyHandler>(propertyHandler);
        }

        /*
         * GetMappedName
         */

        private string mappedName;

        /// <summary>
        /// Gets the mapped-name for the current property.
        /// </summary>
        /// <returns>The mapped-name value.</returns>
        public string GetMappedName()
        {
            if (mappedName != null)
            {
                return mappedName;
            }
            return mappedName = PropertyMappedNameCache.Get(GetDeclaringType(), PropertyInfo);
        }

        #endregion

        #region Comparers

        /// <summary>
        /// Returns the hashcode of the <see cref="PropertyInfo"/> object of this instance.
        /// </summary>
        /// <returns>The hash code value.</returns>
        public override int GetHashCode() =>
            GetDeclaringType().GetHashCode() ^ PropertyInfo.GenerateCustomizedHashCode(GetDeclaringType());

        /// <summary>
        /// Compare the current instance to the other object instance.
        /// </summary>
        /// <param name="obj">The object to be compared.</param>
        /// <returns>True if the two instance is the same.</returns>
        public override bool Equals(object obj)
        {
            if (obj is ClassProperty property)
            {
                return PropertyInfo.Equals(property.PropertyInfo);
            }
            return Equals(obj);
        }

        /// <summary>
        /// Compare the current instance to the other object instance.
        /// </summary>
        /// <param name="other">The object to be compared.</param>
        /// <returns>True if the two instance is the same.</returns>
        public bool Equals(ClassProperty other) =>
            PropertyInfo.Equals(other.PropertyInfo);


        /// <summary>
        /// Compares the equality of the two <see cref="ClassProperty"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="ClassProperty"/> object.</param>
        /// <param name="objB">The second <see cref="ClassProperty"/> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(ClassProperty objA,
            ClassProperty objB)
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
        public static bool operator !=(ClassProperty objA,
            ClassProperty objB) =>
            (objA == objB) == false;

        #endregion
    }
}
