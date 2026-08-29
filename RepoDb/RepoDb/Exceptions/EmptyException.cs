// Copyright (c) 2019 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown if the <see cref="Array"/> or <see cref="Enumerable"/> is empty.
    /// </summary>
    public class EmptyException : Exception
    {
        /// <summary>
        /// Creates a new instance of <see cref="EmptyException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public EmptyException(string message) : base(message) { }
    }
}
