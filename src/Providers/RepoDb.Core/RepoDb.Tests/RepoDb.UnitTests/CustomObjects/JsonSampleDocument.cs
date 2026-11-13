#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RepoDb.UnitTests.CustomObjects
{
    /// <summary>
    /// The object represented by the JSON column of a <see cref="JsonSampleEntity"/>.
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

        /// <summary>
        /// Gets or sets the display name, serialized with a custom property name.
        /// </summary>
        [JsonPropertyName("display-name")]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the names.
        /// </summary>
        public List<string> Names { get; set; }

        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        public string[] Tags { get; set; }
    }
}
