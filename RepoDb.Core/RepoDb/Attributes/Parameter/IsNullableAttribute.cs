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
    /// An attribute that is being used to define a value to the <see cref="DbParameter.IsNullable"/>
    /// property via a class property mapping.
    /// </summary>
    public class IsNullableAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="IsNullableAttribute"/> class.
        /// </summary>
        /// <param name="isNullable">The value that defines whether the parameter accepts a null value.</param>
        public IsNullableAttribute(bool isNullable)
            : base(typeof(DbParameter), nameof(DbParameter.IsNullable), isNullable)
        { }

        /// <summary>
        /// Gets the mapped value that defines whether the parameter accepts a null value.
        /// </summary>
        public bool IsNullable => (bool)Value;
    }
}