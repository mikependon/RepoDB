#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;

namespace RepoDb.Telemetry.Extended.UnitTests
{
    /// <summary>
    /// Provides a constructible result trace for pipeline tests.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    internal sealed class TestResultLog<TResult> : ResultTraceLog<TResult>
    {
        #region Constructors

        /// <summary>
        /// Creates a result paired with its command trace.
        /// </summary>
        /// <param name="log">The original command.</param>
        /// <param name="result">The returned result.</param>
        public TestResultLog(CancellableTraceLog log,
            TResult result)
            : base(log.SessionId,
                log.Key,
                TimeSpan.FromMilliseconds(320),
                result,
                log)
        {
        }

        #endregion
    }
}

