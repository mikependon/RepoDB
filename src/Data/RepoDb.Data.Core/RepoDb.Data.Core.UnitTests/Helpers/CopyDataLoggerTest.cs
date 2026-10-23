#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.Core.UnitTests.CustomObjects;
using RepoDb.Data.Enumerations;
using RepoDb.Data.Helpers;
using RepoDb.Data.Models;

namespace RepoDb.Data.Core.UnitTests.Helpers
{
    [TestClass]
    public class CopyDataLoggerTest
    {
        #region Add

        [TestMethod]
        public void TestAddAddsTheLogToTheProgressAndToTheCurrentItem()
        {
            // Setup
            var item = new CopyProgressItem();
            var progress = new CopyProgress { Current = item };

            // Act
            CopyDataLogger.Add(progress, new FakeDbConnection(), CopyLogLevel.Warning, "Text");

            // Assert
            Assert.AreEqual(1, progress.Logs.Count);
            Assert.AreEqual(1, item.Logs.Count);
            Assert.AreSame(progress.Logs[0], item.Logs[0]);
            Assert.AreEqual(CopyLogLevel.Warning, item.Logs[0].Level);
            Assert.AreEqual("Text", item.Logs[0].Text);
            Assert.AreEqual(typeof(FakeDbConnection), item.Logs[0].DatabaseType);
        }

        [TestMethod]
        public void TestAddAddsTheLogToTheProgressOnlyIfThereIsNoCurrentItem()
        {
            // Setup
            var progress = new CopyProgress();

            // Act
            CopyDataLogger.Add(progress, new FakeDbConnection(), CopyLogLevel.Info, "Text");

            // Assert
            Assert.AreEqual(1, progress.Logs.Count);
        }

        [TestMethod]
        public void TestAddDoesNothingIfThereIsNoProgress()
        {
            // Act/Assert
            CopyDataLogger.Add(null, new FakeDbConnection(), CopyLogLevel.Info, "Text");
        }

        #endregion

        #region LogBulkInsert

        [TestMethod]
        public void TestLogBulkInsertReturnsNullIfTheRowsOfTheTargetAreNotCounted()
        {
            // Setup
            var progress = new CopyProgress { Current = new CopyProgressItem() };

            // Act
            var actual = CopyDataLogger.LogBulkInsert(progress, new FakeDbConnection(), "Person", 10, null, 10);

            // Assert
            Assert.IsNull(actual);
            Assert.AreEqual(1, progress.Logs.Count);
            Assert.AreEqual(CopyLogLevel.Info, progress.Logs[0].Level);
        }

        [TestMethod]
        public void TestLogBulkInsertReturnsZeroIfAllTheRowsWereLoaded()
        {
            // Setup
            var progress = new CopyProgress { Current = new CopyProgressItem() };

            // Act
            var actual = CopyDataLogger.LogBulkInsert(progress, new FakeDbConnection(), "Person", 10, 5, 15);

            // Assert
            Assert.AreEqual(0L, actual);
            Assert.AreEqual(1, progress.Logs.Count);
        }

        [TestMethod]
        public void TestLogBulkInsertReturnsTheNumberOfRowsThatWereSkippedAndLogsAWarning()
        {
            // Setup
            var item = new CopyProgressItem();
            var progress = new CopyProgress { Current = item };

            // Act
            var actual = CopyDataLogger.LogBulkInsert(progress, new FakeDbConnection(), "Person", 10, 5, 12);

            // Assert
            Assert.AreEqual(3L, actual);
            Assert.AreEqual(2, progress.Logs.Count);
            Assert.AreEqual(CopyLogLevel.Warning, progress.Logs[1].Level);
            StringAssert.Contains(progress.Logs[1].Text, "3 of 10", System.StringComparison.Ordinal);
            Assert.AreEqual(2, item.Logs.Count);
        }

        #endregion
    }
}
