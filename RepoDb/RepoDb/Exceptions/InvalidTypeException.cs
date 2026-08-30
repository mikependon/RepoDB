#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown when the type is not valid.
    /// </summary>
    public class InvalidTypeException : Exception
    {
        /// <summary>
        /// Creates a new instance of <see cref="InvalidTypeException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public InvalidTypeException(string message)
            : base(message) { }
    }
}
