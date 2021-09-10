using Npgsql;
using NpgsqlTypes;
using System;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used to define a mapping of .NET CLR <see cref="Type"/> into its equivalent <see cref="NpgsqlDbType"/> value.
    /// </summary>
    public class NpgsqlTypeMapAttribute : ParameterPropertyValueSetterAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="NpgsqlTypeMapAttribute"/> class.
        /// </summary>
        /// <param name="dbType">A target <see cref="NpgsqlDbType"/> value.</param>
        public NpgsqlTypeMapAttribute(NpgsqlDbType dbType)
            : base(typeof(NpgsqlParameter), nameof(NpgsqlParameter.NpgsqlDbType), dbType)
        { }

        /// <summary>
        /// Gets the actual database type value.
        /// </summary>
        public NpgsqlDbType DbType => (NpgsqlDbType)Value;
    }
}