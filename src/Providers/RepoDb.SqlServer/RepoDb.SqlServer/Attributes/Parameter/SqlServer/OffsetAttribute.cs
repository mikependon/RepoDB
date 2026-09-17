#region Copyright Attributions

// Copyright (c) 2021 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.Data.SqlClient;

namespace RepoDb.Attributes.Parameter.SqlServer
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="SqlParameter.Offset"/> property via an entity property
    /// before the actual execution.
    /// </summary>
    /// <remarks>
    /// Creates a new instance of <see cref="OffsetAttribute"/> class.
    /// </remarks>
    /// <param name="offset">The offset value.</param>
    [System.AttributeUsage(System.AttributeTargets.All)]
    public class OffsetAttribute(int offset) : PropertyValueAttribute(typeof(SqlParameter), nameof(SqlParameter.Offset), offset)
    {

        /// <summary>
        /// Gets the mapped offset value of the parameter.
        /// </summary>
        public int Offset => (int)Value;
    }
}