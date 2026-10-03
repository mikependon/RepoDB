#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Data.Enumerations;

namespace RepoDb.Data.Options
{
    /// <summary>
    /// A class that is used to define the options of the <see cref="ICopyDataInterceptor"/>.
    /// </summary>
    public class CopyDataInterceptorOptions
    {
        #region Properties

        /// <summary>
        /// Gets or sets the level at which the data is intercepted. The default is <see cref="CopyInterceptionLevel.Row"/>.
        /// </summary>
        public CopyInterceptionLevel CopyInterceptionLevel { get; set; } = CopyInterceptionLevel.Row;

        /// <summary>
        /// Gets or sets the number of times a failed interception is retried. The default is 0 (no retry).
        /// </summary>
        public int RetryCount { get; set; }

        #endregion
    }
}
