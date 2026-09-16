#region Copyright Attributions

// Copyright (c) 2018 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown if the primary key is not found from the data entity.
    /// </summary>
    /// <remarks>
    /// Creates a new instance of <see cref="PrimaryFieldNotFoundException"/> class.
    /// </remarks>
    /// <param name="message">The exception message.</param>
    public class PrimaryFieldNotFoundException(string message) : Exception(message)
    {
    }
}
