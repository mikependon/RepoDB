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
    public class CopyDataProgressTest
    {
        [TestMethod]
        public void TestCopyDataProgressDefaultValues()
        {
            // Act
            var actual = new CopyDataProgress();

            // Assert
            Assert.AreEqual(0, actual.BatchNumber);
            Assert.AreEqual(0, actual.RowCount);
            Assert.AreEqual(0, actual.TotalCopiedRowCount);
            Assert.AreEqual(default(DateTime), actual.StartTime);
            Assert.AreEqual(default(DateTime), actual.EndTime);
        }

        [TestMethod]
        public void TestCopyDataProgressPropertiesCanBeSet()
        {
            // Setup
            var start = new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc);
            var end = start.AddMinutes(5);

            // Act
            var actual = new CopyDataProgress
            {
                BatchNumber = 3,
                RowCount = 1000,
                TotalCopiedRowCount = 2500,
                StartTime = start,
                EndTime = end
            };

            // Assert
            Assert.AreEqual(3, actual.BatchNumber);
            Assert.AreEqual(1000, actual.RowCount);
            Assert.AreEqual(2500, actual.TotalCopiedRowCount);
            Assert.AreEqual(start, actual.StartTime);
            Assert.AreEqual(end, actual.EndTime);
        }
    }
}
