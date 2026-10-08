#region Copyright Attributions

// Copyright (c) 2018 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown when the operation has been cancelled during the tracing.
    /// </summary>
    public class CancelledExecutionException : Exception
    {
        /// <summary>
        /// Creates a new instance of <see cref="CancelledExecutionException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public CancelledExecutionException(string message)
            : base(message) { }
    }
}
