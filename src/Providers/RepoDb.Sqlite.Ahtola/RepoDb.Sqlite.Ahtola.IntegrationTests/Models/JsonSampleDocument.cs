#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Ahtola.IntegrationTests.Models
{
    /// <summary>
    /// The object stored in the JSON column of a <see cref="JsonSampleEntity"/>.
    /// </summary>
    public class JsonSampleDocument
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        public JsonSampleAddress Address { get; set; }
    }
}
