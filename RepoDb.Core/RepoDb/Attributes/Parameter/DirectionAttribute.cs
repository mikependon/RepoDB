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
    public class DirectionAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="DirectionAttribute"/> class.
        /// </summary>
        /// <param name="direction">The value that indicates the direction of the parameter.</param>
        public DirectionAttribute(ParameterDirection direction)
            : base(typeof(DbParameter), nameof(DbParameter.Direction), direction)
        { }

        /// <summary>
        /// Gets the mapped value that indicates whether the parameter is input, output, bidirectional 
        /// or a return value from the stored procedure.
        /// </summary>
        public ParameterDirection Direction => (ParameterDirection)Value;
    }
}