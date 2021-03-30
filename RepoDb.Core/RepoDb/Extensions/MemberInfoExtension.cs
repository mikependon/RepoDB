using System.Reflection;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <see cref="MemberInfo"/> object.
    /// </summary>
    public static class MemberInfoExtension
    {
        /// <summary>
        /// Gets the name of the current instance of <see cref="MemberInfo"/>. If the instance is <see cref="PropertyInfo"/>, it will try to retrieved the
        /// mapped name of the property.
        /// </summary>
        /// <param name="member">The member where to retrieve a name.</param>
        /// <returns>The name of the <see cref="MemberInfo"/>.</returns>
        internal static string GetMappedName(this MemberInfo member) => 
            member is PropertyInfo memberInfo ? PropertyMappedNameCache.Get(memberInfo) : member.Name;

        /// <summary>
        /// Gets a value from the current instance of <see cref="MemberInfo"/> object.
        /// </summary>
        /// <param name="member">The instance of <see cref="MemberInfo"/> object where the value is to be extracted.</param>
        /// <param name="obj">The object whose member value will be returned.</param>
        /// <param name="parameters">The argument list of parameters if needed.</param>
        /// <returns>The extracted value from <see cref="MemberInfo"/> object.</returns>
        internal static object GetValue(this MemberInfo member,
            object obj,
            object[] parameters = null)
        {
            return member switch
            {
                FieldInfo fieldInfo => fieldInfo.GetValue(obj),
                PropertyInfo propertyInfo => propertyInfo.GetValue(obj),
                MethodInfo methodInfo => methodInfo.Invoke(obj, parameters),
                _ => null
            };
        }

        /// <summary>
        /// Sets the value of an object member based on the retrieved value from the instance of <see cref="MemberInfo"/> object.
        /// </summary>
        /// <param name="member">The instance of <see cref="MemberInfo"/> object where the value is to be retrieved.</param>
        /// <param name="obj">The object whose member value will be set.</param>
        /// <param name="value">The target value of the member.</param>
        internal static void SetValue(this MemberInfo member,
            object obj,
            object value)
        {
            if (member is FieldInfo fieldInfo)
            {
                fieldInfo.SetValue(obj, value);
            }
            else if (member is PropertyInfo propertyInfo)
            {
                propertyInfo.SetValue(obj, value);
            }
        }

        #region Identification and Conversion
        
        /// <summary>
        /// Converts the current instance of <see cref="MemberInfo"/> object into <see cref="PropertyInfo"/> object.
        /// </summary>
        /// <param name="member">The instance of the <see cref="MemberInfo"/> object.</param>
        /// <returns>A converted instance of <see cref="PropertyInfo"/> object.</returns>
        public static PropertyInfo ToPropertyInfo(this MemberInfo member) =>
            (PropertyInfo)member;
        
        #endregion

        #region Helpers

        /// <summary>
        /// Checks whether the arguments length are equal to both members.
        /// </summary>
        /// <param name="member1">The first <see cref="MemberInfo"/>.</param>
        /// <param name="member2">The second <see cref="MemberInfo"/>.</param>
        /// <returns>True if the arguments length of the members are equal.</returns>
        internal static bool IsMemberArgumentLengthEqual(MemberInfo member1,
            MemberInfo member2)
        {
            if (member1 is MethodInfo methodInfo1 && member2 is MethodInfo methodInfo2)
            {
                return methodInfo1.GetParameters().Length == methodInfo2.GetParameters().Length;
            }

            return member1 is PropertyInfo && member2 is PropertyInfo;
        }

        #endregion
    }
}
