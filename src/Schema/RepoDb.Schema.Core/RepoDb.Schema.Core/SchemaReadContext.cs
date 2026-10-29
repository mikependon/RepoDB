#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Threading;
using RepoDb.Interfaces;

namespace RepoDb.Schema
{
    /// <summary>
    /// Contains the query settings for a scoped schema extraction. Custom readers may consult <see cref="Current"/>.
    /// </summary>
    public sealed class SchemaReadContext : IDisposable
    {
        #region Private Variables

        /// <summary>
        /// Holds the active schema extraction context for the current execution context.
        /// </summary>
        private static readonly AsyncLocal<SchemaReadContext> Slot = new AsyncLocal<SchemaReadContext>();

        /// <summary>
        /// Holds the enclosing schema extraction context.
        /// </summary>
        private readonly SchemaReadContext previous;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates and activates a schema extraction context.
        /// </summary>
        /// <param name="commandTimeout">The catalog query timeout in seconds.</param>
        /// <param name="traceKey">The tracking key for catalog queries.</param>
        /// <param name="trace">The catalog query trace.</param>
        internal SchemaReadContext(int? commandTimeout,
            string traceKey,
            ITrace trace)
        {
            previous = Slot.Value;
            CommandTimeout = commandTimeout;
            TraceKey = traceKey;
            Trace = trace;
            Slot.Value = this;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the command timeout in seconds.
        /// </summary>
        public int? CommandTimeout { get; }

        /// <summary>
        /// Gets the active extraction settings, or null.
        /// </summary>
        public static SchemaReadContext Current => Slot.Value;

        /// <summary>
        /// Gets the query trace.
        /// </summary>
        public ITrace Trace { get; }

        /// <summary>
        /// Gets the operation trace key.
        /// </summary>
        public string TraceKey { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Restores the enclosing context.
        /// </summary>
        public void Dispose() =>
            Slot.Value = previous;

        #endregion
    }
}
