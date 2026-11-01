#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Data;
using System.Text.Json;

namespace RepoDb.Telemetry.Extended
{
    /// <summary>
    /// A snapshot of command parameter metadata and its optional value.
    /// </summary>
    public class TelemetryParameter
    {
        #region Properties

        /// <summary>
        /// The parameter name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The database type.
        /// </summary>
        public DbType DbType { get; set; }

        /// <summary>
        /// The parameter direction.
        /// </summary>
        public ParameterDirection Direction { get; set; }

        /// <summary>
        /// Whether the parameter accepts null.
        /// </summary>
        public bool IsNullable { get; set; }

        /// <summary>
        /// The parameter size.
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// The numeric precision.
        /// </summary>
        public byte Precision { get; set; }

        /// <summary>
        /// The numeric scale.
        /// </summary>
        public byte Scale { get; set; }

        /// <summary>
        /// The source column.
        /// </summary>
        public string SourceColumn { get; set; }

        /// <summary>
        /// The source row version.
        /// </summary>
        public DataRowVersion SourceVersion { get; set; }

        /// <summary>
        /// The CLR value type.
        /// </summary>
        public string ValueType { get; set; }

        /// <summary>
        /// The captured value, when enabled and supported.
        /// </summary>
        public JsonElement? Value { get; set; }

        #endregion
    }
}
