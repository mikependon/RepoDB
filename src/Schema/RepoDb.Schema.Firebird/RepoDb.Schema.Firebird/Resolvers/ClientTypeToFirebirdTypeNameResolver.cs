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
    /// A class that is being used to resolve the .NET CLR Types into the name of its equivalent Firebird type
    /// (i.e.: <see cref="string"/> into <c>varchar</c>). A nullable type is resolved as its underlying type.
    /// </summary>
    public class ClientTypeToFirebirdTypeNameResolver : IResolver<Type, string>
    {
        /// <summary>
        /// Resolves a .NET CLR Type into the name of its equivalent Firebird type.
        /// </summary>
        /// <param name="type">The .NET CLR Type to be resolved.</param>
        /// <returns>The name of the equivalent Firebird type, or <c>blob_text</c> if the type is not known.</returns>
        public virtual string Resolve(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            type = Nullable.GetUnderlyingType(type) ?? type;
            if (type == typeof(string)) return "varchar";
            if (type == typeof(int)) return "integer";
            if (type == typeof(long)) return "bigint";
            if (type == typeof(short)) return "smallint";
            if (type == typeof(byte) || type == typeof(sbyte)) return "smallint";
            if (type == typeof(uint)) return "bigint";
            if (type == typeof(ulong)) return "decimal";
            if (type == typeof(ushort)) return "integer";
            if (type == typeof(bool)) return "boolean";
            if (type == typeof(decimal)) return "decimal";
            if (type == typeof(double)) return "double precision";
            if (type == typeof(float)) return "float";
            if (type == typeof(DateTime)) return "timestamp";
            if (type == typeof(DateTimeOffset)) return "timestamp_tz";
            if (type == typeof(TimeSpan)) return "time";
            if (type == typeof(Guid)) return "binary(16)";
            if (type == typeof(byte[])) return "blob_binary";
            return "blob_text";
        }
    }
}
