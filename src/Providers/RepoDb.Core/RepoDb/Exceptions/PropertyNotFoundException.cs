#region Copyright Attributions

// Copyright (c) 2020 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown if the target property is not found.
    /// </summary>
    /// <remarks>
    /// Creates a new instance of <see cref="PropertyNotFoundException"/> class.
    /// </remarks>
    /// <param name="message">The exception message.</param>
    public class PropertyNotFoundException(string message) : Exception(message)
    {
    }
}
