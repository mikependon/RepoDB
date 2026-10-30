#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Exceptions;
using RepoDb.Interfaces;

namespace RepoDb.Schema
{
    /// <summary>
    /// Applies the trace cancellation contract of the schema operations that read or validate a definition.
    /// </summary>
    internal static class SchemaTraceCancellation
    {
        #region Internal Methods

        /// <summary>
        /// Checks whether the trace cancelled the operation. A trace that cancels with an exception makes the operation throw it.
        /// </summary>
        /// <param name="log">The trace log that the trace may have cancelled.</param>
        /// <param name="message">The message of the exception that is thrown when the trace asks for one.</param>
        /// <returns><c>true</c> if the trace cancelled the operation without requesting an exception; otherwise, <c>false</c>.</returns>
        internal static bool IsCancelled(CancellableTraceLog log,
            string message)
        {
            if (log.IsCancelled && log.IsThrowException)
            {
                throw new CancelledExecutionException(message);
            }
            return log.IsCancelled;
        }

        #endregion
    }
}
