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
    /// A class that is being used to resolve the .NET CLR Types into the name of its equivalent SQLite type
    /// (i.e.: <see cref="string"/> into <c>text</c>). A nullable type is resolved as its underlying type.
    /// </summary>
    public class ClientTypeToSqliteTypeNameResolver : IResolver<Type, string>
    {
        /// <summary>
        /// Resolves a .NET CLR Type into the name of its equivalent SQLite type.
        /// </summary>
        /// <param name="type">The .NET CLR Type to be resolved.</param>
        /// <returns>The name of the equivalent SQLite type, or <c>text</c> if the type is not known.</returns>
        public virtual string Resolve(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            type = Nullable.GetUnderlyingType(type) ?? type;
            if (type == typeof(string)) return "text";
            if (type == typeof(int) || type == typeof(uint)) return "integer";
            if (type == typeof(long) || type == typeof(ulong)) return "bigint";
            if (type == typeof(short) || type == typeof(ushort)) return "smallint";
            if (type == typeof(byte) || type == typeof(sbyte)) return "tinyint";
            if (type == typeof(bool)) return "boolean";
            if (type == typeof(decimal)) return "decimal";
            if (type == typeof(double)) return "double";
            if (type == typeof(float)) return "real";
            if (type == typeof(DateTime) || type == typeof(DateTimeOffset)) return "datetime";
            if (type == typeof(TimeSpan)) return "time";
            if (type == typeof(Guid)) return "uuid";
            if (type == typeof(byte[])) return "blob";
            return "text";
        }
    }
}
