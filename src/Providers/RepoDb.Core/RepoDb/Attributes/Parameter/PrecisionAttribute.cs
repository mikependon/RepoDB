#region Copyright Attributions

// Copyright (c) 2021 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Data.Common;

namespace RepoDb.Attributes.Parameter
{
    /// <summary>
    /// An attribute that is being used to define a value to the <see cref="DbParameter.Precision"/>
    /// property via a class property mapping.
    /// </summary>
    /// <remarks>
    /// Creates a new instance of <see cref="PrecisionAttribute"/> class.
    /// </remarks>
    /// <param name="precision">The precision of the parameter.</param>
    [System.AttributeUsage(System.AttributeTargets.All)]
    public class PrecisionAttribute(byte precision) : PropertyValueAttribute(typeof(DbParameter), nameof(DbParameter.Precision), precision)
    {

        /// <summary>
        /// Gets the mapped precision value of the parameter.
        /// </summary>
        public byte Precision => (byte)Value;
    }
}