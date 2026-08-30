#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Oracle.ManagedDataAccess.Client;

namespace RepoDb.Attributes.Parameter.Oracle
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="OracleParameter.SkipConversionToLocalTime"/>
    /// property via an entity property before the actual execution. Specifies whether the value
    /// bound to this parameter should skip conversion to local time.
    /// </summary>
    public class SkipConversionToLocalTimeAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="SkipConversionToLocalTimeAttribute"/> class.
        /// </summary>
        /// <param name="skipConversionToLocalTime">The value that indicates whether the conversion to local time is skipped.</param>
        public SkipConversionToLocalTimeAttribute(bool skipConversionToLocalTime)
            : base(typeof(OracleParameter), nameof(OracleParameter.SkipConversionToLocalTime), skipConversionToLocalTime)
        { }

        /// <summary>
        /// Gets the mapped value that indicates whether the conversion to local time is skipped.
        /// </summary>
        public bool SkipConversionToLocalTime => (bool)Value;
    }
}
