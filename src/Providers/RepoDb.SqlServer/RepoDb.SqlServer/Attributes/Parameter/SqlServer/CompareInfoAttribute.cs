#region Copyright Attributions

// Copyright (c) 2021 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.Data.SqlClient;
using System.Data.SqlTypes;

namespace RepoDb.Attributes.Parameter.SqlServer
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="SqlParameter.CompareInfo"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    /// <remarks>
    /// Creates a new instance of <see cref="CompareInfoAttribute"/> class.
    /// </remarks>
    /// <param name="compareInfo">The value that determines how the string comparission is being defined.</param>
    [System.AttributeUsage(System.AttributeTargets.All)]
    public class CompareInfoAttribute(SqlCompareOptions compareInfo) : PropertyValueAttribute(typeof(SqlParameter), nameof(SqlParameter.CompareInfo), compareInfo)
    {

        /// <summary>
        /// Gets the mapped value that determines how the string comparission is being defined on the parameter.
        /// </summary>
        public SqlCompareOptions CompareInfo => (SqlCompareOptions)Value;
    }
}