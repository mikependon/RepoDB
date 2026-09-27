#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Connector.AuroraDb.Npgsql;

namespace RepoDb.Attributes.Parameter.AuroraDb
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="AuroraDbParameter.AuroraDbType"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class AuroraDbTypeAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="AuroraDbTypeAttribute"/> class.
        /// </summary>
        /// <param name="auroraDbType">The target <see cref="AuroraDbType"/> value.</param>
        public AuroraDbTypeAttribute(AuroraDbType auroraDbType)
            : base(typeof(AuroraDbParameter), nameof(AuroraDbParameter.AuroraDbType), auroraDbType)
        { }

        /// <summary>
        /// Gets the mapped <see cref="AuroraDbType"/> value of the parameter.
        /// </summary>
        public AuroraDbType AuroraDbType => (AuroraDbType)Value;
    }
}
