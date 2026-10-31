#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.UnitTests.Trace
{
    /// <summary>
    /// Verifies shared bulk cleanup preserves execution errors without hiding cleanup-only errors.
    /// </summary>
    [TestClass]
    public class BulkCleanupTest
    {
        #region Public Methods

        /// <summary>
        /// Verifies rollback or disposal failures do not replace an existing failure.
        /// </summary>
        [TestMethod]
        public void CleanupPreservesExecutionFailure()
        {
            var attempted = false;
            Tracer.Cleanup(() =>
            {
                attempted = true;
                throw new NotSupportedException("Rollback failed.");
            },
                new InvalidOperationException("Bulk execution failed."));
            Assert.IsTrue(attempted);
        }

        /// <summary>
        /// Verifies cleanup failures propagate after successful execution.
        /// </summary>
        [TestMethod]
        public void CleanupFailureAfterSuccessIsThrown()
        {
            var failure = new NotSupportedException("Disposal failed.");
            var thrown = Assert.ThrowsExactly<NotSupportedException>(() =>
                Tracer.Cleanup(() => throw failure,
                    null));
            Assert.AreSame(failure,
                thrown);
        }

        /// <summary>
        /// Verifies asynchronous cleanup failures do not replace an existing failure.
        /// </summary>
        [TestMethod]
        public async Task AsyncCleanupPreservesExecutionFailure()
        {
            var attempted = false;
            await Tracer.CleanupAsync(() =>
            {
                attempted = true;
                return Task.FromException(new NotSupportedException("Drop failed."));
            },
                new InvalidOperationException("Bulk execution failed.")).ConfigureAwait(false);
            Assert.IsTrue(attempted);
        }

        /// <summary>
        /// Verifies asynchronous cleanup failures propagate after successful execution.
        /// </summary>
        [TestMethod]
        public async Task AsyncCleanupFailureAfterSuccessIsThrown()
        {
            var failure = new NotSupportedException("Drop failed.");
            var thrown = await Assert.ThrowsExactlyAsync<NotSupportedException>(() =>
                Tracer.CleanupAsync(() => Task.FromException(failure),
                    null)).ConfigureAwait(false);
            Assert.AreSame(failure,
                thrown);
        }

        #endregion
    }
}

