#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.UnitTests.CustomObjects
{
    /// <summary>
    /// An entity with JSON columns.
    /// </summary>
    public class JsonSampleEntity
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the JSON document.
        /// </summary>
        public DbJsonValue<JsonSampleDocument> Document { get; set; }

        /// <summary>
        /// Gets or sets the JSON tags.
        /// </summary>
        public DbJsonValue<string[]> Tags { get; set; }
    }
}
