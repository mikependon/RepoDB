#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown if the query has returned more than one row when only a single row was expected.
    /// </summary>
    /// <remarks>
    /// Creates a new instance of <see cref="MultipleRowsFoundException"/> class.
    /// </remarks>
    /// <param name="message">The exception message.</param>
    public class MultipleRowsFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MultipleRowsFoundException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public MultipleRowsFoundException(string message)
            : base(message)
        {}
    }
}
