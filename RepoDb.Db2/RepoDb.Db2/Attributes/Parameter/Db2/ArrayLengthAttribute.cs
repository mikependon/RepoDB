#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using IBM.Data.Db2;

namespace RepoDb.Attributes.Parameter.Db2
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="DB2Parameter.ArrayLength"/>
    /// property via an entity property before the actual execution. Specifies the number
    /// of elements of the <c>Value</c> property to use for parameter bind-in.
    /// </summary>
    public class ArrayLengthAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="ArrayLengthAttribute"/> class.
        /// </summary>
        /// <param name="arrayLength">The number of elements of the <c>Value</c> property to bind.</param>
        public ArrayLengthAttribute(int arrayLength)
            : base(typeof(DB2Parameter), nameof(DB2Parameter.ArrayLength), arrayLength)
        { }

        /// <summary>
        /// Gets the mapped array length value of the parameter.
        /// </summary>
        public int ArrayLength => (int)Value;
    }
}
