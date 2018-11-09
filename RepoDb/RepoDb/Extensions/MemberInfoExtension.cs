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
        /// <typeparam name="T">The custom attribute that is defined into the property.</typeparam>
        /// <param name="member">The type of where the custom attribute is defined.</param>
        /// <returns>The custom attribute.</returns>
        public static T GetCustomAttribute<T>(this MemberInfo member) where T : Attribute
        {
            return (T)GetCustomAttribute(member, typeof(T));
        }

        /// <summary>
        /// Gets a custom attribute defined on the member.
        /// </summary>
        /// <param name="member">The type of where the custom attribute is defined.</param>
        /// <param name="type">The custom attribute that is defined into the member.</param>
        /// <returns>The custom attribute.</returns>
        public static Attribute GetCustomAttribute(this MemberInfo member, Type type)
        {
            var attributes = member.GetCustomAttributes(type, false);
            return attributes?.Any() == true ? (Attribute)attributes[0] : null;
        }

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
        public static FieldInfo AsFieldInfo(this MemberInfo member)
        {
            return member as FieldInfo;
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
        public static PropertyInfo AsPropertyInfo(this MemberInfo member)
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
        public static MethodInfo AsMethodInfo(this MemberInfo member)
        {
            return (MethodInfo)member;
        }
        
        /// <summary>
        /// Gets a value from the current instance of <see cref="MemberInfo"/> object.
        /// </summary>
        /// <param name="member">The instance of <see cref="MemberInfo"/> object where the value is to be extracted.</param>
        /// <param name="obj">The object whose member value will be returned.</param>
        /// <param name="parameters">The argument list of parameters if needed.</param>
        /// <returns>The extracted value from <see cref="MemberInfo"/> object.</returns>
        private static object GetValue(this MemberInfo member, object obj, object[] parameters = null)
        {
            if (member.IsFieldInfo())
            {
                return member.AsFieldInfo().GetValue(obj);
            }
            else if (member.IsPropertyInfo())
            {
                return member.AsPropertyInfo().GetValue(obj);
            }
            else if (member.IsMethodInfo())
            {
                return member.AsMethodInfo().Invoke(obj, parameters);
            }
            return null;
        }
    }
}
