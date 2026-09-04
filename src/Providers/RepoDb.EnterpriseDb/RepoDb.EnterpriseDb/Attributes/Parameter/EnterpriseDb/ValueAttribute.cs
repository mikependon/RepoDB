#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Connector.EnterpriseDb;

namespace RepoDb.Attributes.Parameter.EnterpriseDb
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="EDBParameter.Value"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class ValueAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="ValueAttribute"/> class.
        /// </summary>
        /// <param name="value">The value of the parameter.</param>
        public ValueAttribute(object value)
            : base(typeof(EDBParameter), nameof(EDBParameter.Value), value)
        { }

        /// <summary>
        /// Gets the mapped value of the parameter.
        /// </summary>
        /// <remarks>
        /// Hides <see cref="PropertyValueAttribute.Value"/> (which is <c>protected internal</c>) with a
        /// same-named, same-typed public accessor, consistent with how every other attribute in this
        /// folder exposes its configured value.
        /// </remarks>
        public new object Value => base.Value;
    }
}
