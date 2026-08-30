#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Vertica.Data.VerticaClient;

namespace RepoDb.Attributes.Parameter.Vertica
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="VerticaParameter.Type"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class VerticaTypeAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="VerticaTypeAttribute"/> class.
        /// </summary>
        /// <param name="verticaType">A target <see cref="global::Vertica.Data.VerticaClient.VerticaType"/> value.</param>
        public VerticaTypeAttribute(VerticaType verticaType)
            : base(typeof(VerticaParameter), nameof(VerticaParameter.Type), verticaType)
        { }

        /// <summary>
        /// Gets the mapped <see cref="global::Vertica.Data.VerticaClient.VerticaType"/> value of the parameter.
        /// </summary>
        public VerticaType Type => (VerticaType)Value;
    }
}
