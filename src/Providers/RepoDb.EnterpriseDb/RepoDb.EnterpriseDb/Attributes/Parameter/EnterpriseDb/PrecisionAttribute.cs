#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Connector.EnterpriseDb;

namespace RepoDb.Attributes.Parameter.EnterpriseDb
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="EDBParameter.Precision"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class PrecisionAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="PrecisionAttribute"/> class.
        /// </summary>
        /// <param name="precision">The precision of the parameter.</param>
        public PrecisionAttribute(byte precision)
            : base(typeof(EDBParameter), nameof(EDBParameter.Precision), precision)
        { }

        /// <summary>
        /// Gets the mapped precision value of the parameter.
        /// </summary>
        public byte Precision => (byte)Value;
    }
}
