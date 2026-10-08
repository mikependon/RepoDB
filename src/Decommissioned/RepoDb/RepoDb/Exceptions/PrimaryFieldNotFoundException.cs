#region Copyright Attributions

// Copyright (c) 2018 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown of the validation for primary key has been called and the primary key is not
    /// found from the data entity.
    /// </summary>
    public class PrimaryFieldNotFoundException : Exception
    {
        /// <summary>
        /// Creates a new instance of <see cref="PrimaryFieldNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public PrimaryFieldNotFoundException(string message)
            : base(message) { }
    }
}
