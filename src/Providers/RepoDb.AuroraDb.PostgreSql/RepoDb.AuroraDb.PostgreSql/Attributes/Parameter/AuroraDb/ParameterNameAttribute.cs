#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Connector.AuroraDb.Npgsql;

namespace RepoDb.Attributes.Parameter.AuroraDb
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="AuroraDbParameter.ParameterName"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class ParameterNameAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="ParameterNameAttribute"/> class.
        /// </summary>
        /// <param name="parameterName">The name of the parameter.</param>
        public ParameterNameAttribute(string parameterName)
            : base(typeof(AuroraDbParameter), nameof(AuroraDbParameter.ParameterName), parameterName)
        { }

        /// <summary>
        /// Gets the mapped name of the parameter.
        /// </summary>
        public string ParameterName => (string)Value;
    }
}
