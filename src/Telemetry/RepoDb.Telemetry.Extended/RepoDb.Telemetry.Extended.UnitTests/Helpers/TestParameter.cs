#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Data;

namespace RepoDb.Telemetry.Extended.UnitTests
{
    /// <summary>
    /// Provides a command parameter for capture tests.
    /// </summary>
    public sealed class TestParameter : IDbDataParameter
    {
        #region Properties

        /// <summary>
        /// Gets or sets the database type.
        /// </summary>
        public DbType DbType { get; set; } = DbType.Int32;

        /// <summary>
        /// Gets or sets the parameter direction.
        /// </summary>
        public ParameterDirection Direction { get; set; } = ParameterDirection.Input;

        /// <summary>
        /// Gets whether null is supported.
        /// </summary>
        public bool IsNullable => true;

        /// <summary>
        /// Gets or sets the parameter name.
        /// </summary>
        public string ParameterName { get; set; } = "@Id";

        /// <summary>
        /// Gets or sets the source column.
        /// </summary>
        public string SourceColumn { get; set; } = "Id";

        /// <summary>
        /// Gets or sets the row version.
        /// </summary>
        public DataRowVersion SourceVersion { get; set; } = DataRowVersion.Current;

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public object Value { get; set; } = 42;

        /// <summary>
        /// Gets or sets the precision.
        /// </summary>
        public byte Precision { get; set; } = 8;

        /// <summary>
        /// Gets or sets the scale.
        /// </summary>
        public byte Scale { get; set; } = 2;

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        public int Size { get; set; } = 4;

        #endregion
    }
}
