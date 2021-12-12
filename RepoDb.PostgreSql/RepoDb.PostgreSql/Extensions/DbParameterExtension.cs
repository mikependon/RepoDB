using Npgsql;
using NpgsqlTypes;
using System;
using System.Data.Common;

namespace RepoDb.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    internal static partial class DbParameterExtension
    {
        /// <summary>
        /// Sets the instance of <see cref="NpgsqlParameter"/> object <see cref="NpgsqlParameter.NpgsqlDbType"/> property to <see cref="NpgsqlDbType.Unknown"/>.
        /// This method is being used by the compiler to compliment the needs for the customized PGSQL objects.
        /// </summary>
        /// <param name="parameter">The instance of the <see cref="NpgsqlParameter"/> object.</param>
        internal static void SetToUnknown(this DbParameter parameter)
        {
            if (parameter is NpgsqlParameter p)
            {
                p.NpgsqlDbType = NpgsqlDbType.Unknown;
            }
            else
            {
                throw new InvalidOperationException($"The passed instance of parameter is not of type '{typeof(NpgsqlParameter)}'.");
            }
        }
    }
}
