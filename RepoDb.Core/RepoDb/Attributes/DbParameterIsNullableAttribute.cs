using System.Data;
using System.Data.Common;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="DbParameter.IsNullable"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class DbParameterIsNullableAttribute : ParameterPropertyValueSetterAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="DbParameterDbTypeAttribute"/> class.
        /// </summary>
        /// <param name="dbType">The value that defines whether the parameter accepts a null value.</param>
        public DbParameterIsNullableAttribute(DbType dbType)
            : base(typeof(DbParameter), nameof(DbParameter.IsNullable), dbType)
        { }

        /// <summary>
        /// Gets the mapped value that defines whether the parameter accepts a null value.
        /// </summary>
        public bool IsNullable => (bool)Value;
    }
}