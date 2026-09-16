#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown if the qualifier <see cref="Field"/> objects passed in the operation are not valid.
    /// </summary>
    /// <remarks>
    /// Creates a new instance of <see cref="InvalidQualifiersException"/> class.
    /// </remarks>
    /// <param name="message">The exception message.</param>
    public class InvalidQualifiersException(string message) : Exception(message)
    {
    }
}
