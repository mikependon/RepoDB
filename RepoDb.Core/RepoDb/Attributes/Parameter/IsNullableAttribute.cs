using System.Data;
using System.Data.Common;

namespace RepoDb.Attributes.Parameter
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="DbParameter.IsNullable"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class IsNullableAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="IsNullableAttribute"/> class.
        /// </summary>
        /// <param name="isNUllable">The value that defines whether the parameter accepts a null value.</param>
        public IsNullableAttribute(bool isNUllable)
            : base(typeof(DbParameter), nameof(DbParameter.IsNullable), isNUllable)
        { }

        /// <summary>
        /// Gets the mapped value that defines whether the parameter accepts a null value.
        /// </summary>
        public bool IsNullable => (bool)Value;
    }
}