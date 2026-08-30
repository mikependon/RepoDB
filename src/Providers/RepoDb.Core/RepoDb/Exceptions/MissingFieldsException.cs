#region Copyright Attributions

// Copyright (c) 2018 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown when the operation extraction of the <see cref="System.Data.Common.DbDataReader"/> into data entity object 
    /// does not matched at least one of the field from the result set.
    /// </summary>
    public class MissingFieldsException : Exception
    {
        /// <summary>
        /// Creates a new instance of <see cref="MissingFieldsException"/> class.
        /// </summary>
        public MissingFieldsException()
            : this(string.Empty) { }

        /// <summary>
        /// Creates a new instance of <see cref="MissingFieldsException"/> class.
        /// </summary>
        /// <param name="fields">The list of fields that is missing.</param>
        public MissingFieldsException(IEnumerable<string> fields)
            : this(fields?.Any() == true ? $"The fields '{fields.Join(", ")}' are missing." : null) { }

        /// <summary>
        /// Creates a new instance of <see cref="MissingFieldsException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public MissingFieldsException(string message)
            : base(!string.IsNullOrEmpty(message) ? message : "The fields cannot be null or empty.") { }
    }
}
