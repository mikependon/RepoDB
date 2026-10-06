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
    /// A class that is being used to resolve the .NET CLR types into the equivalent CockroachDB type names.
    /// </summary>
    public class ClientTypeToCockroachDbTypeNameResolver : IResolver<Type, string>
    {
        /// <summary>
        /// Returns the equivalent CockroachDB type name of the .NET CLR type.
        /// </summary>
        /// <param name="type">The .NET CLR type.</param>
        /// <returns>The equivalent CockroachDB type name (<c>text</c> if the type is not known).</returns>
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
            if (type == typeof(short) || type == typeof(byte)) return "smallint";
            if (type == typeof(bool)) return "boolean";
            if (type == typeof(decimal)) return "numeric";
            if (type == typeof(double)) return "double precision";
            if (type == typeof(float)) return "real";
            if (type == typeof(DateTime)) return "timestamp";
            if (type == typeof(DateTimeOffset)) return "timestamptz";
            if (type == typeof(TimeSpan)) return "time";
            if (type == typeof(Guid)) return "uuid";
            if (type == typeof(byte[])) return "bytea";
            return "text";
        }
    }
}
