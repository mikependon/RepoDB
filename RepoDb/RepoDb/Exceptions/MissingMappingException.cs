// Copyright (c) 2019 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

using System;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown when the mapping is missing.
    /// </summary>
    public class MissingMappingException : Exception
    {
        /// <summary>
        /// Creates a new instance of <see cref="MissingMappingException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public MissingMappingException(string message)
            : base(message) { }
    }
}
