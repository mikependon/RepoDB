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
    /// A class that is being used to resolve the .NET CLR Types into the name of its equivalent Vertica type
    /// (i.e.: <see cref="string"/> into <c>varchar</c>). A nullable type is resolved as its underlying type.
    /// </summary>
    public class ClientTypeToVerticaTypeNameResolver : IResolver<Type, string>
    {
        /// <summary>
        /// Resolves a .NET CLR Type into the name of its equivalent Vertica type.
        /// </summary>
        /// <param name="type">The .NET CLR Type to be resolved.</param>
        /// <returns>The name of the equivalent Vertica type, or <c>long varchar</c> if the type is not known.</returns>
        public virtual string Resolve(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            type = Nullable.GetUnderlyingType(type) ?? type;
            if (type == typeof(string)) return "varchar";
            if (type == typeof(int) || type == typeof(long) || type == typeof(short) || type == typeof(byte) || type == typeof(sbyte) ||
                type == typeof(uint) || type == typeof(ushort)) return "int";
            if (type == typeof(ulong)) return "numeric";
            if (type == typeof(bool)) return "boolean";
            if (type == typeof(decimal)) return "numeric";
            if (type == typeof(double) || type == typeof(float)) return "float";
            if (type == typeof(DateTime)) return "timestamp";
            if (type == typeof(DateTimeOffset)) return "timestamp with timezone";
            if (type == typeof(TimeSpan)) return "time";
            if (type == typeof(Guid)) return "uuid";
            if (type == typeof(byte[])) return "long varbinary";
            return "long varchar";
        }
    }
}
