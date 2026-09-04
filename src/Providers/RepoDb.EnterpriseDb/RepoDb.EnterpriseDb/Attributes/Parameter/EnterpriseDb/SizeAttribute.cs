#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Connector.EnterpriseDb;

namespace RepoDb.Attributes.Parameter.EnterpriseDb
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="EDBParameter.Size"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class SizeAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="SizeAttribute"/> class.
        /// </summary>
        /// <param name="size">The size of the parameter.</param>
        public SizeAttribute(int size)
            : base(typeof(EDBParameter), nameof(EDBParameter.Size), size)
        { }

        /// <summary>
        /// Gets the mapped size value of the parameter.
        /// </summary>
        public int Size => (int)Value;
    }
}
