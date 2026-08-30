#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown when the parameter is not valid.
    /// </summary>
    public class InvalidParameterException : Exception
    {
        /// <summary>
        /// Creates a new instance of <see cref="InvalidParameterException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public InvalidParameterException(string message)
            : base(message) { }
    }
}
