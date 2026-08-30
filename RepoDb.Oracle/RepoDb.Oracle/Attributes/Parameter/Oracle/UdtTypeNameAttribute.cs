#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Oracle.ManagedDataAccess.Client;

namespace RepoDb.Attributes.Parameter.Oracle
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="OracleParameter.UdtTypeName"/>
    /// property via an entity property before the actual execution. Required whenever the
    /// parameter binds an Oracle user-defined type (object type or collection).
    /// </summary>
    public class UdtTypeNameAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="UdtTypeNameAttribute"/> class.
        /// </summary>
        /// <param name="udtTypeName">The name of the Oracle user-defined type, e.g. "SCHEMA.MY_TYPE".</param>
        public UdtTypeNameAttribute(string udtTypeName)
            : base(typeof(OracleParameter), nameof(OracleParameter.UdtTypeName), udtTypeName)
        { }

        /// <summary>
        /// Gets the name of the mapped user-defined type of the parameter.
        /// </summary>
        public string UdtTypeName => (string)Value;
    }
}
