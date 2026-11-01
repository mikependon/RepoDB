#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RepoDb.Telemetry.Extended.UnitTests
{
    /// <summary>
    /// Provides a materialized application row.
    /// </summary>
    public sealed class TestResultRow
    {
        #region Properties

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the excluded deferred value.
        /// </summary>
        [JsonIgnore]
        public IEnumerable<int> Deferred { get; set; }

        #endregion
    }
}
