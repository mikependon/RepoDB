using Npgsql;
using NpgsqlTypes;
using System;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used to define a mapping of .NET CLR <see cref="Type"/> into its equivalent <see cref="NpgsqlDbType"/> value.
    /// </summary>
    public class NpgsqlTypeMapAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="NpgsqlTypeMapAttribute"/> class.
        /// </summary>
        /// <param name="dbType">A target <see cref="NpgsqlDbType"/> value.</param>
        public NpgsqlTypeMapAttribute(NpgsqlDbType dbType)
        {
            DbType = dbType;
            ParameterType = typeof(NpgsqlParameter);
        }

        /// <summary>
        /// Gets a <see cref="NpgsqlDbType"/> that is currently mapped.
        /// </summary>
        public NpgsqlDbType DbType { get; }

        /// <summary>
        /// Gets the represented <see cref="Type"/> of the <see cref="NpgsqlParameter"/>.
        /// </summary>
        public Type ParameterType { get; }
    }
}