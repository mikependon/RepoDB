using Npgsql;
using NpgsqlTypes;
using System;
using System.Data;
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
        /// (This is being used by the compiler)
        /// </summary>
        /// <param name="parameter">The instance of the <see cref="NpgsqlParameter"/> object.</param>
        private static void SetToUnknownNpgsqlParameter(this DbParameter parameter)
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
