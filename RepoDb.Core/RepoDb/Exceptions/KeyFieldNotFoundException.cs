#region Copyright Attributions

// Copyright (c) 2020 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown if the primary key and identity key is not found from the data entity.
    /// </summary>
    public class KeyFieldNotFoundException : Exception
    {
        /// <summary>
        /// Creates a new instance of <see cref="KeyFieldNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public KeyFieldNotFoundException(string message)
            : base(message) { }
    }
}
