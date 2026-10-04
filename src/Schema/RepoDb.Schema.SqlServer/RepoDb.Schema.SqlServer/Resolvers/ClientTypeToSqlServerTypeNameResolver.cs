#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using RepoDb.Interfaces;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class that is being used to resolve the .NET CLR Types into the name of its equivalent SQL Server type
    /// (i.e.: <see cref="string"/> into <c>nvarchar</c>). A nullable type is resolved as its underlying type.
    /// </summary>
    public class ClientTypeToSqlServerTypeNameResolver : IResolver<Type, string>
    {
        /// <summary>
        /// Resolves a .NET CLR Type into the name of its equivalent SQL Server type.
        /// </summary>
        /// <param name="type">The .NET CLR Type to be resolved.</param>
        /// <returns>The name of the equivalent SQL Server type, or <c>sql_variant</c> if the type is not known.</returns>
        public virtual string Resolve(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            type = Nullable.GetUnderlyingType(type) ?? type;
            if (type == typeof(string)) return "nvarchar";
            if (type == typeof(int)) return "int";
            if (type == typeof(long)) return "bigint";
            if (type == typeof(short)) return "smallint";
            if (type == typeof(byte)) return "tinyint";
            if (type == typeof(bool)) return "bit";
            if (type == typeof(decimal)) return "decimal";
            if (type == typeof(double)) return "float";
            if (type == typeof(float)) return "real";
            if (type == typeof(DateTime)) return "datetime2";
            if (type == typeof(DateTimeOffset)) return "datetimeoffset";
            if (type == typeof(TimeSpan)) return "time";
            if (type == typeof(Guid)) return "uniqueidentifier";
            if (type == typeof(byte[])) return "varbinary";
            return "sql_variant";
        }
    }
}
