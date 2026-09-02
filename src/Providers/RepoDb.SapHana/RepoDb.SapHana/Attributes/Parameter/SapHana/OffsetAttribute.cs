#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Sap.Data.Hana;

namespace RepoDb.Attributes.Parameter.SapHana
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="HanaParameter.Offset"/> property via
    /// an entity property before the actual execution. Specifies the offset into the <c>Value</c>
    /// property, for binary/string data.
    /// </summary>
    public class OffsetAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="OffsetAttribute"/> class.
        /// </summary>
        /// <param name="offset">The offset value.</param>
        public OffsetAttribute(int offset)
            : base(typeof(HanaParameter), nameof(HanaParameter.Offset), offset)
        { }

        /// <summary>
        /// Gets the mapped offset value of the parameter.
        /// </summary>
        public int Offset => (int)Value;
    }
}
