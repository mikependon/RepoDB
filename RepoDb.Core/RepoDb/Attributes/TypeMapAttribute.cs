using System;
using System.Data;
using System.Data.Common;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute that is used to define a mapping between the .NET CLR <see cref="Type"/> and the <see cref="System.Data.DbType"/>.
    /// </summary>
    public class TypeMapAttribute : ParameterPropertyValueSetterAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="TypeMapAttribute"/> class.
        /// </summary>
        /// <param name="dbType">A target <see cref="System.Data.DbType"/> value.</param>
        public TypeMapAttribute(DbType dbType)
            : base(typeof(DbParameter), nameof(DbParameter.DbType), dbType)
        { }

        /// <summary>
        /// Gets the mapped <see cref="DbParameter.DbType"/> value of the parameter.
        /// </summary>
        public DbType DbType => (DbType)Value;
    }
}