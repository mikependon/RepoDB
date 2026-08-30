#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Data;
using Vertica.Data.VerticaClient;

namespace RepoDb.Attributes.Parameter.Vertica
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="VerticaParameter.SourceVersion"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class SourceVersionAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="SourceVersionAttribute"/> class.
        /// </summary>
        /// <param name="sourceVersion">The value of the target <see cref="DataRowVersion"/>.</param>
        public SourceVersionAttribute(DataRowVersion sourceVersion)
            : base(typeof(VerticaParameter), nameof(VerticaParameter.SourceVersion), sourceVersion)
        { }

        /// <summary>
        /// Gets the mapped <see cref="DataRowVersion"/> value of the parameter.
        /// </summary>
        public DataRowVersion SourceVersion => (DataRowVersion)Value;
    }
}
