#region Copyright Attributions

// Copyright (c) 2021 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Data;
using System.Data.Common;

namespace RepoDb.Attributes.Parameter
{
    /// <summary>
    /// An attribute that is being used to define a value to the <see cref="DbParameter.Direction"/>
    /// property via a class property mapping.
    /// </summary>
    /// <remarks>
    /// Creates a new instance of <see cref="DirectionAttribute"/> class.
    /// </remarks>
    /// <param name="direction">The value that indicates the direction of the parameter.</param>
    [System.AttributeUsage(System.AttributeTargets.All)]
    public class DirectionAttribute(ParameterDirection direction) : PropertyValueAttribute(typeof(DbParameter), nameof(DbParameter.Direction), direction)
    {

        /// <summary>
        /// Gets the mapped value that indicates whether the parameter is input, output, bidirectional 
        /// or a return value from the stored procedure.
        /// </summary>
        public ParameterDirection Direction => (ParameterDirection)Value;
    }
}