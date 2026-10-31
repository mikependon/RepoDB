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


        /// <summary>
        /// Creates a test command with one parameter, and the context that the library captures when an operation starts.
        /// </summary>
        /// <param name="context">The context of the execution.</param>
        /// <param name="operation">The operation being traced.</param>
        internal TestCommandLog(TraceContext context,
            string operation = "Delete")
            : base(Guid.NewGuid(),
                operation,
                "DELETE FROM dbo.Warehouse WHERE Id = @Id",
                new IDbDataParameter[] { null },
                context)
        {
        }

        #endregion
    }
}

