using Npgsql;
using NpgsqlTypes;
using System;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used to define a mapping of .NET CLR <see cref="Type"/> into its equivalent <see cref="NpgsqlTypes.NpgsqlDbType"/> value.
    /// </summary>
    public class NpgsqlTypeMapAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="NpgsqlTypeMapAttribute"/> class.
        /// </summary>
        /// <param name="npgsqlDbType">A target <see cref="NpgsqlTypes.NpgsqlDbType"/> value.</param>
        public NpgsqlTypeMapAttribute(NpgsqlDbType npgsqlDbType)
        {
            NpgsqlDbType = npgsqlDbType;
            ParameterType = typeof(NpgsqlParameter);
        }

        /// <summary>
        /// Gets a <see cref="NpgsqlTypes.NpgsqlDbType"/> that is currently mapped.
        /// </summary>
        public NpgsqlDbType NpgsqlDbType { get; }

        /// <summary>
        /// Gets the represented <see cref="Type"/> of the <see cref="NpgsqlParameter"/>.
        /// </summary>
        public Type ParameterType { get; }
    }
}