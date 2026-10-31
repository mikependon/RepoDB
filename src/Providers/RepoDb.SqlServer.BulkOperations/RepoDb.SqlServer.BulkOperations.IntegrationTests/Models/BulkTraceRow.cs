#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.SqlServer.BulkOperations.IntegrationTests.Models
{
    /// <summary>
    /// Represents a row in an isolated bulk tracing fixture.
    /// </summary>
    public class BulkTraceRow
    {
        #region Properties

        /// <summary>
        /// Gets or sets the primary key.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the payload.
        /// </summary>
        public int Value { get; set; }

        #endregion
    }
}

