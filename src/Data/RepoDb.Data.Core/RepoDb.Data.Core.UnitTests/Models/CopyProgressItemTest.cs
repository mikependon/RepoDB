#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.Models;

namespace RepoDb.Data.Core.UnitTests.Models
{
    [TestClass]
    public class CopyProgressItemTest
    {
        [TestMethod]
        public void TestCopyProgressItemDefaultValues()
        {
            // Act
            var actual = new CopyProgressItem();

            // Assert
            Assert.AreEqual(0, actual.RowCount);
            Assert.IsNull(actual.SkippedRowCount);
            Assert.AreEqual(default(DateTime), actual.StartTime);
            Assert.AreEqual(default(DateTime), actual.EndTime);
            Assert.AreEqual(TimeSpan.Zero, actual.Elapsed);
        }

        [TestMethod]
        public void TestCopyProgressItemSourceAndTargetAreNotNullAndNotTheSame()
        {
            // Act
            var actual = new CopyProgressItem();

            // Assert
            Assert.IsNotNull(actual.Source);
            Assert.IsNotNull(actual.Target);
            Assert.AreNotSame(actual.Source, actual.Target);
        }

        [TestMethod]
        public void TestCopyProgressItemBatchesAndLogsAreEmptyByDefault()
        {
            // Act
            var actual = new CopyProgressItem();

            // Assert
            Assert.IsNotNull(actual.Batches);
            Assert.AreEqual(0, actual.Batches.Count);
            Assert.IsNotNull(actual.Logs);
            Assert.AreEqual(0, actual.Logs.Count);
        }

        [TestMethod]
        public void TestCopyProgressItemCollectionsAndEndpointsAreNotShared()
        {
            // Act
            var first = new CopyProgressItem();
            var second = new CopyProgressItem();
            first.Batches.Add(new CopyBatch());
            first.Logs.Add(new Log());

            // Assert
            Assert.AreEqual(0, second.Batches.Count);
            Assert.AreEqual(0, second.Logs.Count);
            Assert.AreNotSame(first.Source, second.Source);
            Assert.AreNotSame(first.Target, second.Target);
        }

        [TestMethod]
        public void TestCopyProgressItemElapsedIsTheTimeBetweenTheStartAndTheEnd()
        {
            // Setup
            var start = new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc);

            // Act
            var actual = new CopyProgressItem
            {
                StartTime = start,
                EndTime = start.AddSeconds(3)
            };

            // Assert
            Assert.AreEqual(TimeSpan.FromSeconds(3), actual.Elapsed);
        }

        [TestMethod]
        public void TestCopyProgressItemPropertiesCanBeSet()
        {
            // Act
            var actual = new CopyProgressItem
            {
                RowCount = 6,
                SkippedRowCount = 2
            };

            // Assert
            Assert.AreEqual(6, actual.RowCount);
            Assert.AreEqual(2L, actual.SkippedRowCount);
        }
    }
}
