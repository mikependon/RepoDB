using System.Linq.Expressions;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <see cref="MemberBinding"/> object.
    /// </summary>
    public static class MemberBindingExtension
    {
        /// <summary>
        /// Gets a value from the current instance of <see cref="MemberBinding"/> object.
        /// </summary>
        /// <param name="member">The instance of <see cref="MemberBinding"/> object where the value is to be extracted.</param>
        /// <returns>The extracted value from <see cref="MemberBinding"/> object.</returns>
        public static object GetValue(this MemberBinding member)
        {
            if (member.IsMemberAssignment())
            {
                return member.ToMemberAssignment().Expression.GetValue();
            }
            return null;
        }

        #region Identification and Conversion

        /// <summary>
        /// Identify whether the instance of <see cref="MemberBinding"/> is a <see cref="MemberAssignment"/> object.
        /// </summary>
        /// <param name="member">The instance of <see cref="MemberBinding"/> object to be identified.</param>
        /// <returns>Returns true if the expression is a <see cref="MemberAssignment"/>.</returns>
        public static bool IsMemberAssignment(this MemberBinding member) =>
            member is MemberAssignment;

        /// <summary>
        /// Converts the <see cref="MemberBinding"/> object into <see cref="MemberAssignment"/> object.
        /// </summary>
        /// <param name="member">The instance of <see cref="MemberBinding"/> object to be converted.</param>
        /// <returns>A converted instance of <see cref="MemberAssignment"/> object.</returns>
        public static MemberAssignment ToMemberAssignment(this MemberBinding member) =>
            (MemberAssignment)member;

        #endregion
    }
}
