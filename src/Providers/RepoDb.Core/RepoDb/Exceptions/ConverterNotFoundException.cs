#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown when the converter is not found.
    /// </summary>
    public class ConverterNotFoundException : Exception
    {
        /// <summary>
        /// Creates a new instance of <see cref="ConverterNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public ConverterNotFoundException(string message)
            : base(message) { }
    }
}
