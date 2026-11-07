#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Attributes;

namespace RepoDb.PostgreSql.IntegrationTests.Models
{
    /// <summary>
    /// Represents a row without an identity or primary key.
    /// </summary>
    public class KeylessRow
    {
        #region Properties

        /// <summary>
        /// Gets or sets the tote identifier.
        /// </summary>
        [Map("tote_id")]
        public int ToteId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Map("name")]
        public string Name { get; set; }

        #endregion
    }
}
