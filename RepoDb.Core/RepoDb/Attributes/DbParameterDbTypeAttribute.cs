using System.Data;
using System.Data.Common;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="DbParameter.DbType"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class DbParameterDbTypeAttribute : ParameterPropertyValueSetterAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="DbParameterDbTypeAttribute"/> class.
        /// </summary>
        /// <param name="dbType">The equivalent <see cref="System.Data.DbType"/> value of the parameter.</param>
        public DbParameterDbTypeAttribute(DbType dbType)
            : base(typeof(DbParameter), nameof(DbParameter.DbType), dbType)
        { }

        /// <summary>
        /// Gets the mapped <see cref="DbParameter.DbType"/> value of the parameter.
        /// </summary>
        public DbType DbType => (DbType)Value;
    }
}