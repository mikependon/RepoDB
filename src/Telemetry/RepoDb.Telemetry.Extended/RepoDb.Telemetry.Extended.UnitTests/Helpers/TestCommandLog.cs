#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Data;

namespace RepoDb.Telemetry.Extended.UnitTests
{
    /// <summary>
    /// Provides a constructible command trace for pipeline tests.
    /// </summary>
    internal sealed class TestCommandLog : CancellableTraceLog
    {
        #region Constructors

        /// <summary>
        /// Creates a test command with one parameter.
        /// </summary>
        /// <param name="operation">The operation being traced.</param>
        public TestCommandLog(string operation = "Delete")
            : base(Guid.NewGuid(),
                operation,
                "DELETE FROM dbo.Warehouse WHERE Id = @Id",
                new IDbDataParameter[] { null })
        {
        }

        #endregion
    }
}

