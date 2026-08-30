#region Copyright Attributions

// Copyright (c) 2018 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown when the operation extraction of the <see cref="System.Data.Common.DbDataReader"/> into data entity object 
    /// does not matched atleast one of the field from the result set.
    /// </summary>
    public class MissingFieldsException : Exception
    {
        /// <summary>
        /// Creates a new instance of <see cref="MissingFieldsException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public MissingFieldsException(string message)
            : base(message) { }
    }
}
