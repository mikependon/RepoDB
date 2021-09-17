using System.Data;
using System.Data.Common;

namespace RepoDb.Attributes.Parameter
{
    /// <summary>
    /// An attribute that is being used to define a value to the <see cref="DbParameter.DbType"/>
    /// property via a class property mapping..
    /// </summary>
    public class DbTypeAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="DbTypeAttribute"/> class.
        /// </summary>
        /// <param name="dbType">The equivalent <see cref="System.Data.DbType"/> value of the parameter.</param>
        public DbTypeAttribute(DbType dbType)
            : base(typeof(DbParameter), nameof(DbParameter.DbType), dbType)
        { }

        /// <summary>
        /// Gets the mapped <see cref="DbParameter.DbType"/> value of the parameter.
        /// </summary>
        public DbType DbType => (DbType)Value;
    }
}