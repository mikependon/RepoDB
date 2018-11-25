using System.Linq;
using System.Reflection;
using System;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <see cref="MemberInfo"/> object.
    /// </summary>
    public static class MemberInfoExtension
    {
        // GetCustomAttribute

        /// <summary>
        /// Gets a custom attribute defined on the member.
        /// </summary>
        /// <param name="member">The type of where the custom attribute is defined.</param>
        /// <param name="type">The custom attribute that is defined into the member.</param>
        /// <returns>The custom attribute.</returns>
        public static Attribute GetCustomAttribute(this MemberInfo member, Type type)
        {
            var attributes = member.GetCustomAttributes(type, false);
            return attributes?.Any() == true ? attributes.OfType<Attribute>().First() : null;
        }

        /// <summary>
        /// Gets the name of the current instance of <see cref="MemberInfo"/>. If the instance is <see cref="PropertyInfo"/>, it will try to retrieved the
        /// mapped name of the property.
        /// </summary>
        /// <param name="member">The member where to retrieve a name.</param>
        /// <returns>The name of the <see cref="MemberInfo"/>.</returns>
        internal static string GetMappedName(this MemberInfo member)
        {
            return member.IsPropertyInfo() ? member.ToPropertyInfo().GetMappedName() : member.Name;
        }

        /// <summary>
        /// Gets a value from the current instance of <see cref="MemberInfo"/> object.
        /// </summary>
        /// <param name="member">The instance of <see cref="MemberInfo"/> object where the value is to be extracted.</param>
        /// <param name="obj">The object whose member value will be returned.</param>
        /// <param name="parameters">The argument list of parameters if needed.</param>
        /// <returns>The extracted value from <see cref="MemberInfo"/> object.</returns>
        internal static object GetValue(this MemberInfo member, object obj, object[] parameters = null)
        {
            if (member.IsFieldInfo())
            {
                return member.ToFieldInfo().GetValue(obj);
            }
            else if (member.IsPropertyInfo())
            {
                return member.ToPropertyInfo().GetValue(obj);
            }
            else if (member.IsMethodInfo())
            {
                return member.ToMethodInfo().Invoke(obj, parameters);
            }
            return null;
        }

        /// <summary>
        /// Sets the value of an object member based on the retrieved value from the instance of <see cref="MemberInfo"/> object.
        /// </summary>
        /// <param name="member">The instance of <see cref="MemberInfo"/> object where the value is to be retrieved.</param>
        /// <param name="obj">The object whose member value will be set.</param>
        /// <param name="value">The target value of the member.</param>
        internal static void SetValue(this MemberInfo member, object obj, object value)
        {
            if (member.IsFieldInfo())
            {
                member.ToFieldInfo().SetValue(obj, value);
            }
            else if (member.IsPropertyInfo())
            {
                member.ToPropertyInfo().SetValue(obj, value);
            }
        }

        #region Identification and Conversion

        // Field

        /// <summary>
        /// Identify whether the current instance of <see cref="MemberInfo"/> is a <see cref="FieldInfo"/> object.
        /// </summary>
        /// <param name="member">The instance of <see cref="MemberInfo"/> to be checked.</param>
        /// <returns>True if the instance of <see cref="MemberInfo"/> is a <see cref="FieldInfo"/> object.</returns>
        public static bool IsFieldInfo(this MemberInfo member)
        {
            return member is FieldInfo;
        }

        /// <summary>
        /// Converts the current instance of <see cref="MemberInfo"/> object into <see cref="FieldInfo"/> object.
        /// </summary>
        /// <param name="member">The instance of the <see cref="MemberInfo"/> object.</param>
        /// <returns>A converted instance of <see cref="FieldInfo"/> object.</returns>
        public static FieldInfo ToFieldInfo(this MemberInfo member)
        {
            return (FieldInfo)member;
        }

        // Property

        /// <summary>
        /// Identify whether the current instance of <see cref="MemberInfo"/> is a <see cref="PropertyInfo"/> object.
        /// </summary>
        /// <param name="member">The instance of <see cref="MemberInfo"/> to be checked.</param>
        /// <returns>True if the instance of <see cref="MemberInfo"/> is a <see cref="PropertyInfo"/> object.</returns>
        public static bool IsPropertyInfo(this MemberInfo member)
        {
            return member is PropertyInfo;
        }

        /// <summary>
        /// Converts the current instance of <see cref="MemberInfo"/> object into <see cref="PropertyInfo"/> object.
        /// </summary>
        /// <param name="member">The instance of the <see cref="MemberInfo"/> object.</param>
        /// <returns>A converted instance of <see cref="PropertyInfo"/> object.</returns>
        public static PropertyInfo ToPropertyInfo(this MemberInfo member)
        {
            return (PropertyInfo)member;
        }

        // Method

        /// <summary>
        /// Identify whether the current instance of <see cref="MemberInfo"/> is a <see cref="MethodInfo"/> object.
        /// </summary>
        /// <param name="member">The instance of <see cref="MemberInfo"/> to be checked.</param>
        /// <returns>True if the instance of <see cref="MemberInfo"/> is a <see cref="MethodInfo"/> object.</returns>
        public static bool IsMethodInfo(this MemberInfo member)
        {
            return member is MethodInfo;
        }

        /// <summary>
        /// Converts the current instance of <see cref="MemberInfo"/> object into <see cref="MethodInfo"/> object.
        /// </summary>
        /// <param name="member">The instance of the <see cref="MemberInfo"/> object.</param>
        /// <returns>A converted instance of <see cref="MethodInfo"/> object.</returns>
        public static MethodInfo ToMethodInfo(this MemberInfo member)
        {
            return (MethodInfo)member;
        }

        #endregion
    }
}
