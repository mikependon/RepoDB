#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using RepoDb.Data.Enumerations;

namespace RepoDb.Data.Models
{
    /// <summary>
    /// A class that holds a log (an information, a warning, an error or a critical information) of a copy operation.
    /// </summary>
    public class CopyDataLog
    {
        #region Properties

        /// <summary>
        /// Gets or sets the type of the database provider (the type of the destination connection) where the activity has happened.
        /// </summary>
        public Type DatabaseType { get; set; }

        /// <summary>
        /// Gets or sets the level (the kind) of the log: information, warning, error or critical.
        /// </summary>
        public CopyDataLogLevel Level { get; set; }

        /// <summary>
        /// Gets or sets the text of the log.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the time (in UTC) when the activity has happened.
        /// </summary>
        public DateTime Timestamp { get; set; }

        #endregion

        /// <summary>
        /// Returns the string representation of the log.
        /// </summary>
        /// <returns>The string representation of this object.</returns>
        public override string ToString() =>
            $"{Timestamp:O} [{Level}] {DatabaseType?.Name}: {Text}";
    }
}
