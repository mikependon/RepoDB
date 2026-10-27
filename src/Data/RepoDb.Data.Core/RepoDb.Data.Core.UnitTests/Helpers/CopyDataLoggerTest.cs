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
            CopyDataLogger.Add(progress, new FakeDbConnection(), LogLevel.Warning, "Text");

            // Assert
            Assert.AreEqual(1, progress.Logs.Count);
            Assert.AreEqual(1, item.Logs.Count);
            Assert.AreSame(progress.Logs[0], item.Logs[0]);
            Assert.AreEqual(LogLevel.Warning, item.Logs[0].Level);
            Assert.AreEqual("Text", item.Logs[0].Text);
            Assert.AreEqual(typeof(FakeDbConnection), item.Logs[0].DatabaseType);
        }

        [TestMethod]
        public void TestAddAddsTheLogToTheProgressOnlyIfThereIsNoCurrentItem()
        {
            // Setup
            var progress = new CopyProgress();

            // Act
            CopyDataLogger.Add(progress, new FakeDbConnection(), LogLevel.Info, "Text");

            // Assert
            Assert.AreEqual(1, progress.Logs.Count);
        }

        [TestMethod]
        public void TestAddDoesNothingIfThereIsNoProgress()
        {
            // Act/Assert
            CopyDataLogger.Add(null, new FakeDbConnection(), LogLevel.Info, "Text");
        }

        #endregion

        [TestMethod]
        public void TestLogBulkInsertReportsRowsSentWithoutInferringSkippedRows()
        {
            var item = new CopyProgressItem();
            var progress = new CopyProgress { Current = item };
            CopyDataLogger.LogBulkInsert(progress, new FakeDbConnection(), "Person", 10);
            Assert.AreEqual(1, progress.Logs.Count);
            Assert.AreEqual(LogLevel.Info, progress.Logs[0].Level);
            StringAssert.Contains(progress.Logs[0].Text, "10 row(s)", System.StringComparison.Ordinal);
            Assert.AreSame(progress.Logs[0], item.Logs[0]);
            Assert.IsNull(item.SkippedRowCount);
        }
    }
}
