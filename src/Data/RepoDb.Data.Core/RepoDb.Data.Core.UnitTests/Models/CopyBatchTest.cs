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
    public class CopyBatchTest
    {
        [TestMethod]
        public void TestCopyBatchDefaultValues()
        {
            // Act
            var actual = new CopyBatch();

            // Assert
            Assert.AreEqual(0, actual.Number);
            Assert.AreEqual(0, actual.RowCount);
            Assert.AreEqual(default(DateTime), actual.StartTime);
            Assert.AreEqual(default(DateTime), actual.EndTime);
            Assert.AreEqual(TimeSpan.Zero, actual.Elapsed);
        }

        [TestMethod]
        public void TestCopyBatchPropertiesCanBeSet()
        {
            // Setup
            var start = new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc);

            // Act
            var actual = new CopyBatch
            {
                Number = 2,
                RowCount = 500,
                StartTime = start,
                EndTime = start.AddMilliseconds(1500)
            };

            // Assert
            Assert.AreEqual(2, actual.Number);
            Assert.AreEqual(500, actual.RowCount);
            Assert.AreEqual(start, actual.StartTime);
            Assert.AreEqual(start.AddMilliseconds(1500), actual.EndTime);
            Assert.AreEqual(TimeSpan.FromMilliseconds(1500), actual.Elapsed);
        }
    }
}
