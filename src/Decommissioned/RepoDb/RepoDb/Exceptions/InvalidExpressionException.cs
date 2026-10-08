#region Copyright Attributions

// Copyright (c) 2018 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown when the query expression passed is not valid.
    /// </summary>
    public class InvalidExpressionException : Exception
    {
        /// <summary>
        /// Creates a new instance of <see cref="InvalidExpressionException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public InvalidExpressionException(string message)
            : base(message) { }
    }
}
