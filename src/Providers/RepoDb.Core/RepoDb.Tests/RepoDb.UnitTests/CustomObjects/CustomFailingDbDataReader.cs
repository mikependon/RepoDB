#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;

namespace RepoDb.UnitTests.CustomObjects
{
    /// <summary>
    /// Fails during result consumption rather than command execution.
    /// </summary>
    public class CustomFailingDbDataReader : CustomDbDataReader
    {
        #region Constructors

        /// <summary>
        /// Creates a reader that propagates the supplied exception.
        /// </summary>
        /// <param name="failure">The exception raised while reading.</param>
        public CustomFailingDbDataReader(Exception failure)
        {
            Failure = failure;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the exception raised while reading.
        /// </summary>
        public Exception Failure { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Fails when advancing to another result set.
        /// </summary>
        /// <returns>No result is returned.</returns>
        public override bool NextResult() => throw Failure;

        /// <summary>
        /// Fails when reading a result row.
        /// </summary>
        /// <returns>No result is returned.</returns>
        public override bool Read() => throw Failure;

        #endregion
    }
}

