#region Copyright Attributions

// Copyright (c) 2021 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Npgsql;
using NpgsqlTypes;

namespace RepoDb.Attributes.Parameter.Npgsql
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="NpgsqlParameter.NpgsqlDbType"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class NpgsqlDbTypeAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="NpgsqlDbTypeAttribute"/> class.
        /// </summary>
        /// <param name="npgsqlDbType">The target <see cref="NpgsqlTypes.NpgsqlDbType"/> value.</param>
        public NpgsqlDbTypeAttribute(NpgsqlDbType npgsqlDbType)
            : base(typeof(NpgsqlParameter), nameof(NpgsqlParameter.NpgsqlDbType), npgsqlDbType)
        { }

        /// <summary>
        /// Gets the mapped <see cref="NpgsqlTypes.NpgsqlDbType"/> value of the parameter.
        /// </summary>
        public NpgsqlDbType NpgsqlDbType => (NpgsqlDbType)Value;
    }
}