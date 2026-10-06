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
    /// A class that is being used to resolve the .NET CLR Types into the name of its equivalent Oracle type
    /// (i.e.: <see cref="string"/> into <c>varchar2</c>). A nullable type is resolved as its underlying type.
    /// </summary>
    public class ClientTypeToOracleTypeNameResolver : IResolver<Type, string>
    {
        /// <summary>
        /// Resolves a .NET CLR Type into the name of its equivalent Oracle type.
        /// </summary>
        /// <param name="type">The .NET CLR Type to be resolved.</param>
        /// <returns>The name of the equivalent Oracle type, or <c>clob</c> if the type is not known.</returns>
        public virtual string Resolve(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            type = Nullable.GetUnderlyingType(type) ?? type;
            if (type == typeof(string)) return "varchar2";
            if (type == typeof(int)) return "number(10)";
            if (type == typeof(long)) return "number(19)";
            if (type == typeof(short)) return "number(5)";
            if (type == typeof(byte) || type == typeof(sbyte)) return "number(3)";
            if (type == typeof(uint)) return "number(10)";
            if (type == typeof(ulong)) return "number(20)";
            if (type == typeof(ushort)) return "number(5)";
            if (type == typeof(bool)) return "number(1)";
            if (type == typeof(decimal)) return "number";
            if (type == typeof(double)) return "binary_double";
            if (type == typeof(float)) return "binary_float";
            if (type == typeof(DateTime)) return "timestamp";
            if (type == typeof(DateTimeOffset)) return "timestamp with time zone";
            if (type == typeof(TimeSpan)) return "interval day to second";
            if (type == typeof(Guid)) return "raw(16)";
            if (type == typeof(byte[])) return "raw";
            return "clob";
        }
    }
}
