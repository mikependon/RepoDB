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
    /// A class that is being used to resolve the .NET CLR Types into the name of its equivalent ClickHouse type
    /// (i.e.: <see cref="string"/> into <c>String</c>). A nullable type is resolved as its underlying type.
    /// </summary>
    public class ClientTypeToClickHouseTypeNameResolver : IResolver<Type, string>
    {
        /// <summary>
        /// Resolves a .NET CLR Type into the name of its equivalent ClickHouse type.
        /// </summary>
        /// <param name="type">The .NET CLR Type to be resolved.</param>
        /// <returns>The name of the equivalent ClickHouse type, or <c>String</c> if the type is not known.</returns>
        public virtual string Resolve(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            type = Nullable.GetUnderlyingType(type) ?? type;
            if (type == typeof(string)) return "String";
            if (type == typeof(int)) return "Int32";
            if (type == typeof(long)) return "Int64";
            if (type == typeof(short)) return "Int16";
            if (type == typeof(sbyte)) return "Int8";
            if (type == typeof(byte)) return "UInt8";
            if (type == typeof(uint)) return "UInt32";
            if (type == typeof(ulong)) return "UInt64";
            if (type == typeof(ushort)) return "UInt16";
            if (type == typeof(bool)) return "Bool";
            if (type == typeof(decimal)) return "Decimal";
            if (type == typeof(double)) return "Float64";
            if (type == typeof(float)) return "Float32";
            if (type == typeof(DateTime) || type == typeof(DateTimeOffset)) return "DateTime64";
            if (type == typeof(TimeSpan)) return "String";
            if (type == typeof(Guid)) return "UUID";
            if (type == typeof(byte[])) return "String";
            return "String";
        }
    }
}
