using NpgsqlTypes;
using RepoDb.Attributes.Parameter.Npgsql;
using System;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used to define a mapping of .NET CLR <see cref="Type"/> into its equivalent <see cref="NpgsqlDbType"/> value.
    /// </summary>
    [Obsolete("Please use the RepoDb.Attributes.Npgsql.NpgsqlDbTypeAttribute instead.")]
    public class NpgsqlTypeMapAttribute : NpgsqlDbTypeAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="NpgsqlTypeMapAttribute"/> class.
        /// </summary>
        /// <param name="dbType">A target <see cref="NpgsqlDbType"/> value.</param>
        public NpgsqlTypeMapAttribute(NpgsqlDbType dbType)
            : base(dbType)
        { }
    }
}