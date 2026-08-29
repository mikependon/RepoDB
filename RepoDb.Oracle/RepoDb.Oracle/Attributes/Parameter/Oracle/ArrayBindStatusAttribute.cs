#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Oracle.ManagedDataAccess.Client;

namespace RepoDb.Attributes.Parameter.Oracle
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="OracleParameter.ArrayBindStatus"/>
    /// property via an entity property before the actual execution. Only meaningful for
    /// an Array Bind or PL/SQL Associative Array Bind execution.
    /// </summary>
    public class ArrayBindStatusAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="ArrayBindStatusAttribute"/> class.
        /// </summary>
        /// <param name="arrayBindStatus">The array of <see cref="OracleParameterStatus"/> values, one per bound element.</param>
        public ArrayBindStatusAttribute(OracleParameterStatus[] arrayBindStatus)
            : base(typeof(OracleParameter), nameof(OracleParameter.ArrayBindStatus), arrayBindStatus)
        { }

        /// <summary>
        /// Gets the mapped array-bind status values of the parameter.
        /// </summary>
        public OracleParameterStatus[] ArrayBindStatus => (OracleParameterStatus[])Value;
    }
}
