#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;

namespace RepoDb.Schema.Models
{
    /// <summary>
    /// A class that holds the details of a schema copy operation: when it ran, the tables that it copied, and the errors and warnings that it raised.
    /// </summary>
    public class CopyTableSchemaResult
    {
        #region Properties

        /// <summary>
        /// Gets the duration of the schema copy operation.
        /// </summary>
        public TimeSpan Duration => EndTime - StartTime;

        /// <summary>
        /// Gets the time (in UTC) when the schema copy operation has ended.
        /// </summary>
        public DateTime EndTime { get; internal set; }

        /// <summary>
        /// Gets the errors that were raised while the schema was being created (see <see cref="ExecutionError"/>), for all the tables.
        /// </summary>
        public IList<ExecutionError> Errors { get; internal set; } = new List<ExecutionError>();

        /// <summary>
        /// Gets the tables that were copied (see <see cref="CopyTableSchemaResultItem"/>): each one has its source table and the target table that was created from it.
        /// </summary>
        public IList<CopyTableSchemaResultItem> Items { get; internal set; } = new List<CopyTableSchemaResultItem>();

        /// <summary>
        /// Gets the time (in UTC) when the schema copy operation has started.
        /// </summary>
        public DateTime StartTime { get; internal set; }

        /// <summary>
        /// Gets the warnings raised during the copy (i.e.: an unmapped column type or an unsupported feature that was skipped), for all the tables.
        /// </summary>
        public IList<string> Warnings { get; internal set; } = new List<string>();

        #endregion
    }
}
