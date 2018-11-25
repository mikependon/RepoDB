using RepoDb.Attributes;
using RepoDb.Enumerations;
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
        /// <summary>
        /// Creates a new instance of <see cref="ClassProperty"/> object.
        /// </summary>
        /// <param name="property">The wrapped property.</param>
        public ClassProperty(PropertyInfo property)
        {
            PropertyInfo = property;
        }

        #region Properties

        /// <summary>
        /// Gets the wrapped property of this object.
        /// </summary>
        public PropertyInfo PropertyInfo
        {
            get;
        }

        #endregion

        #region Methods

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
         * GetIgnoreAttribute
         */
        private bool m_isIgnoreAttributeWasSet;
        private IgnoreAttribute m_ignoreAttribute;

        /// <summary>
        /// Gets the ignore attribute if present.
        /// </summary>
        /// <returns>The ignore attribute if present.</returns>
        public IgnoreAttribute GetIgnoreAttribute()
        {
            if (m_isIgnoreAttributeWasSet)
            {
                return m_ignoreAttribute;
            }
            m_isIgnoreAttributeWasSet = true;
            return m_ignoreAttribute = PropertyInfo.GetCustomAttribute(typeof(IgnoreAttribute)) as IgnoreAttribute;
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
            m_isPrimary = (PropertyInfo.Name.ToLower() == StringConstant.Id.ToLower());
            if (m_isPrimary == true)
            {
                return m_isPrimary;
            }

            // Type.Name + Id
            m_isPrimary = (PropertyInfo.Name.ToLower() == string.Concat(PropertyInfo.DeclaringType.Name, StringConstant.Id).ToLower());
            if (m_isPrimary == true)
            {
                return m_isPrimary;
            }

            // Mapping.Name + Id
            m_isPrimary = (PropertyInfo.Name.ToLower() == string.Concat(ClassMappedNameCache.Get(PropertyInfo.DeclaringType), StringConstant.Id).ToLower());
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
            m_isDbTypeWasSet = true;
            return m_dbType = PropertyInfo.GetCustomAttribute<TypeMapAttribute>()?.DbType ??
                TypeMapper.Get(GetUnderlyingType(PropertyInfo.PropertyType))?.DbType;
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
            return m_mappedName = PropertyMappedNameCache.Get(PropertyInfo);
        }

        /*
         * GetUnderlyingType
         */

        /// <summary>
        /// Gets the underlying type if present.
        /// </summary>
        /// <param name="type">The type where to get the underlying type.</param>
        /// <returns>The underlying type.</returns>
        private static Type GetUnderlyingType(Type type)
        {
            return type != null ? Nullable.GetUnderlyingType(type) ?? type : null;
        }

        /*
         * IsIgnored
         */

        /// <summary>
        /// Identifies whether the current property is ignored based on the command.
        /// </summary>
        /// <param name="command">The target command.</param>
        /// <returns>Returns true if the property has been ignored based on the target command.</returns>
        public bool IsIgnored(Command command)
        {
            var ignoreAttribute = GetIgnoreAttribute();
            if (ignoreAttribute != null)
            {
                return (ignoreAttribute.Command & command) == command &&
                    (ignoreAttribute.Command != Command.None);
            }
            return false;
        }

        /*
         * IsRecursive
         */

        private bool? m_isRecursive;

        /// <summary>
        /// Gets the value whether this property is recursive.
        /// </summary>
        /// <returns>The value that defines whether this property is recursive.</returns>
        public bool? IsRecursive()
        {
            if (m_isRecursive != null)
            {
                return m_isRecursive;
            }
            var args = PropertyInfo.PropertyType.GetGenericArguments();
            return m_isRecursive = (args != null && args.Length > 0) &&
                (
                    PropertyInfo.PropertyType != typeof(Nullable<>) &&
                    PropertyInfo.PropertyType.Name != "Nullable`1"
                );
        }

        /*
         * IsQueryable
         */

        private bool? m_isQueryable;

        /// <summary>
        /// Gets the value whether this property is queryable.
        /// </summary>
        /// <returns>The value that defines whether this property is queryable.</returns>
        public bool? IsQueryable()
        {
            if (m_isQueryable != null)
            {
                return m_isQueryable;
            }
            return m_isQueryable = IsIgnored(Command.Query) == false;
        }

        /*
         * IsInsertable
         */

        private bool? m_isInsertable;

        /// <summary>
        /// Gets the value whether this property is insertable.
        /// </summary>
        /// <returns>The value that defines whether this property is insertable.</returns>
        public bool? IsInsertable()
        {
            if (m_isInsertable != null)
            {
                return m_isInsertable;
            }
            return m_isInsertable = IsIgnored(Command.Insert) == false;
        }

        /*
         * IsUpdateable
         */

        private bool? m_isUpdateable;

        /// <summary>
        /// Gets the value whether this property is updateable.
        /// </summary>
        /// <returns>The value that defines whether this property is updateable.</returns>
        public bool? IsUpdateable()
        {
            if (m_isUpdateable != null)
            {
                return m_isUpdateable;
            }
            return m_isUpdateable = IsIgnored(Command.Update) == false;
        }

        /*
         * IsDeletable
         */

        private bool? m_isDeletable;

        /// <summary>
        /// Gets the value whether this property is deletable.
        /// </summary>
        /// <returns>The value that defines whether this property is deletable.</returns>
        public bool? IsDeletable()
        {
            if (m_isDeletable != null)
            {
                return m_isDeletable;
            }
            return m_isDeletable = IsIgnored(Command.Delete) == false;
        }

        /*
         * IsMergeable
         */

        private bool? m_isMergeable;

        /// <summary>
        /// Gets the value whether this property is mergeable.
        /// </summary>
        /// <returns>The value that defines whether this property is mergeable.</returns>
        public bool? IsMergeable()
        {
            if (m_isMergeable != null)
            {
                return m_isMergeable;
            }
            return m_isMergeable = IsIgnored(Command.Merge) == false;
        }

        /*
         * IsBatchQueryable
         */

        private bool? m_isBatchQueryable;

        /// <summary>
        /// Gets the value whether this property is batch-queryable.
        /// </summary>
        /// <returns>The value that defines whether this property is batch-queryable.</returns>
        public bool? IsBatchQueryable()
        {
            if (m_isBatchQueryable != null)
            {
                return m_isBatchQueryable;
            }
            return m_isBatchQueryable = IsIgnored(Command.BatchQuery) == false;
        }

        /*
         * IsBatchQueryable
         */

        private bool? m_isInlineUpdateable;

        /// <summary>
        /// Gets the value whether this property is inline-updateable.
        /// </summary>
        /// <returns>The value that defines whether this property is inline-updateable.</returns>
        public bool? IsInlineUpdateable()
        {
            if (m_isInlineUpdateable != null)
            {
                return m_isInlineUpdateable;
            }
            return m_isInlineUpdateable = IsIgnored(Command.InlineUpdate) == false;
        }

        /*
         * IsInlineMergeable
         */

        private bool? m_isInlineMergeable;

        /// <summary>
        /// Gets the value whether this property is inline-merge.
        /// </summary>
        /// <returns>The value that defines whether this property is inline-merge.</returns>
        public bool? IsInlineMergeable()
        {
            if (m_isInlineMergeable != null)
            {
                return m_isInlineMergeable;
            }
            return m_isInlineMergeable = IsIgnored(Command.InlineMerge) == false;
        }

        /*
         * IsInlineInsertable
         */

        private bool? m_isInlineInsertable;

        /// <summary>
        /// Gets the value whether this property is inline-insert.
        /// </summary>
        /// <returns>The value that defines whether this property is inline-insert.</returns>
        public bool? IsInlineInsertable()
        {
            if (m_isInlineInsertable != null)
            {
                return m_isInlineInsertable;
            }
            return m_isInlineInsertable = IsIgnored(Command.InlineInsert) == false;
        }

        /*
         * IsBulkInsertable
         */

        private bool? m_isBulkInsertable;

        /// <summary>
        /// Gets the value whether this property is bulk-insertable.
        /// </summary>
        /// <returns>The value that defines whether this property is bulk-insertable.</returns>
        public bool? IsBulkInsertable()
        {
            if (m_isBulkInsertable != null)
            {
                return m_isBulkInsertable;
            }
            return m_isBulkInsertable = IsIgnored(Command.BulkInsert) == false;
        }

        /*
         * IsCountable
         */

        private bool? m_isCountable;

        /// <summary>
        /// Gets the value whether this property is countable.
        /// </summary>
        /// <returns>The value that defines whether this property is countable.</returns>
        public bool? IsCountable()
        {
            if (m_isCountable != null)
            {
                return m_isCountable;
            }
            return m_isCountable = IsIgnored(Command.Count) == false;
        }

        /*
         * IsDeletableAll
         */

        private bool? m_isDeletableAll;

        /// <summary>
        /// Gets the value whether this property is deletable (all).
        /// </summary>
        /// <returns>The value that defines whether this property is deletable (all).</returns>
        public bool? IsDeletableAll()
        {
            if (m_isDeletableAll != null)
            {
                return m_isDeletableAll;
            }
            return m_isDeletableAll = IsIgnored(Command.DeleteAll) == false;
        }

        /*
         * IsTruncatable
         */

        private bool? m_isTruncatable;

        /// <summary>
        /// Gets the value whether this property is truncatable.
        /// </summary>
        /// <returns>The value that defines whether this property is truncatable.</returns>
        public bool? IsTruncatable()
        {
            if (m_isTruncatable != null)
            {
                return m_isTruncatable;
            }
            return m_isTruncatable = IsIgnored(Command.Truncate) == false;
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
            return objA?.GetHashCode() == objB?.GetHashCode();
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
