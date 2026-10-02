#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.Enumerations.SapHana;

namespace RepoDb.SapHana.BulkOperations.UnitTests
{
    [TestClass]
    public class SapHanaBulkDbSettingTest
    {
        [TestMethod]
        public void TestSapHanaBulkDbSettingWriteToServerExecutionDefaultsToAsyncOverSync()
        {
            // Setup
            var setting = new SapHanaBulkDbSetting();

            // Act
            var actual = setting.WriteToServerExecution;

            // Assert - async Bulk*Async calls must default to the genuine HanaBulkCopy-backed path
            // (issue #1363): the row-by-row SapHanaCommandBatcher fallback must be opt-in only.
            Assert.AreEqual(SapHanaWriteToServerExecution.AsyncOverSync, actual);
        }

        [TestMethod]
        public void TestSapHanaWriteToServerExecutionOrdinalsAreStable()
        {
            // Assert - these are public, persistable enum values; guards against an accidental
            // reorder silently flipping the meaning of a previously-serialized/configured value.
            Assert.AreEqual(0, (short)SapHanaWriteToServerExecution.SapHanaCommandBatcher);
            Assert.AreEqual(1, (short)SapHanaWriteToServerExecution.AsyncOverSync);
        }
    }
}
