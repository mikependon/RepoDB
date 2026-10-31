#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;

namespace RepoDb.UnitTests.CustomObjects
{
    /// <summary>
    /// A connection that cannot tell its database (i.e.: a provider whose connection is not usable), so the tracing can be tested against it.
    /// </summary>
    public class CustomThrowingDatabaseDbConnection : CustomDbConnection
    {
        #region Properties

        /// <summary>
        /// Gets the name of the database of the connection. It always throws.
        /// </summary>
        public override string Database => throw new InvalidOperationException("The database is not available.");

        #endregion
    }
}
