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
    public class CopyProgressTest
    {
        [TestMethod]
        public void TestCopyProgressDefaultValues()
        {
            // Act
            var actual = new CopyProgress();

            // Assert
            Assert.AreEqual(0, actual.BatchCount);
            Assert.AreEqual(0, actual.CompletedTableCount);
            Assert.AreEqual(0, actual.RowCount);
            Assert.AreEqual(0, actual.TableCount);
            Assert.AreEqual(default(DateTime), actual.StartTime);
            Assert.AreEqual(default(DateTime), actual.EndTime);
            Assert.AreEqual(TimeSpan.Zero, actual.Elapsed);
            Assert.AreEqual(0d, actual.RowsPerSecond);
            Assert.IsNull(actual.Current);
            Assert.IsNull(actual.CurrentBatch);
        }

        [TestMethod]
        public void TestCopyProgressItemsAndLogsAreEmptyByDefault()
        {
            // Act
            var actual = new CopyProgress();

            // Assert
            Assert.IsNotNull(actual.Items);
            Assert.AreEqual(0, actual.Items.Count);
            Assert.IsNotNull(actual.Logs);
            Assert.AreEqual(0, actual.Logs.Count);
        }

        [TestMethod]
        public void TestCopyProgressItemsAndLogsAreNotShared()
        {
            // Act
            var first = new CopyProgress();
            var second = new CopyProgress();
            first.Items.Add(new CopyProgressItem());
            first.Logs.Add(new Log());

            // Assert
            Assert.AreEqual(0, second.Items.Count);
            Assert.AreEqual(0, second.Logs.Count);
        }

        [TestMethod]
        public void TestCopyProgressPropertiesCanBeSet()
        {
            // Setup
            var start = new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc);
            var end = start.AddMinutes(5);

            // Act
            var actual = new CopyProgress
            {
                BatchCount = 3,
                CompletedTableCount = 1,
                RowCount = 2500,
                TableCount = 4,
                StartTime = start,
                EndTime = end
            };

            // Assert
            Assert.AreEqual(3, actual.BatchCount);
            Assert.AreEqual(1, actual.CompletedTableCount);
            Assert.AreEqual(2500, actual.RowCount);
            Assert.AreEqual(4, actual.TableCount);
            Assert.AreEqual(start, actual.StartTime);
            Assert.AreEqual(end, actual.EndTime);
        }

        [TestMethod]
        public void TestCopyProgressElapsedIsTheTimeBetweenTheStartAndTheEnd()
        {
            // Setup
            var start = new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc);

            // Act
            var actual = new CopyProgress
            {
                StartTime = start,
                EndTime = start.AddSeconds(8)
            };

            // Assert
            Assert.AreEqual(TimeSpan.FromSeconds(8), actual.Elapsed);
        }

        [TestMethod]
        public void TestCopyProgressRowsPerSecondIsTheRowsOverTheElapsedTime()
        {
            // Setup
            var start = new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc);

            // Act
            var actual = new CopyProgress
            {
                RowCount = 1000,
                StartTime = start,
                EndTime = start.AddSeconds(4)
            };

            // Assert
            Assert.AreEqual(250d, actual.RowsPerSecond);
        }

        [TestMethod]
        public void TestCopyProgressRowsPerSecondIsZeroIfNoTimePassed()
        {
            // Act
            var actual = new CopyProgress
            {
                RowCount = 1000
            };

            // Assert
            Assert.AreEqual(0d, actual.RowsPerSecond);
        }

        [TestMethod]
        public void TestCopyProgressCurrentBatchIsTheLastBatchOfTheCurrentItem()
        {
            // Setup
            var first = new CopyBatch { Number = 1, RowCount = 500 };
            var second = new CopyBatch { Number = 2, RowCount = 120 };
            var item = new CopyProgressItem();
            item.Batches.Add(first);
            item.Batches.Add(second);

            // Act
            var actual = new CopyProgress
            {
                Current = item
            };

            // Assert
            Assert.AreSame(second, actual.CurrentBatch);
        }

        [TestMethod]
        public void TestCopyProgressCurrentBatchIsNullIfTheCurrentItemHasNoBatch()
        {
            // Act
            var actual = new CopyProgress
            {
                Current = new CopyProgressItem()
            };

            // Assert
            Assert.IsNull(actual.CurrentBatch);
        }
    }
}
