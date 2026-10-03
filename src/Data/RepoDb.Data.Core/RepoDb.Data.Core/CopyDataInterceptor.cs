#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using RepoDb.Data.Interfaces;
using RepoDb.Data.Options;

namespace RepoDb.Data
{
    /// <summary>
    /// A base class for the <see cref="ICopyDataInterceptor"/> implementations. By default, the data is returned as is,
    /// so a derived class only needs to override the transformation it cares about.
    /// </summary>
    public abstract class CopyDataInterceptor : ICopyDataInterceptor
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="CopyDataInterceptor"/> class.
        /// </summary>
        /// <param name="options">The options of the interceptor.</param>
        protected CopyDataInterceptor(CopyDataInterceptorOptions options)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        #endregion

        #region Protected

        /// <summary>
        /// Gets the options of the interceptor.
        /// </summary>
        protected CopyDataInterceptorOptions Options { get; }

        #endregion

        #region Public Methods

        /// <inheritdoc/>
        public virtual CopyDataRow Transform(CopyDataRow row) => row;

        /// <inheritdoc/>
        public virtual CopyDataTable Transform(CopyDataTable table) => table;

        #endregion
    }
}
