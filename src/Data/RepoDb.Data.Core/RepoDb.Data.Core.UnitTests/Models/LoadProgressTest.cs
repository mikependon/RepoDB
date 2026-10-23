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
    public class LoadProgressTest
    {
        [TestMethod]
        public void TestLoadProgressDefaultValues()
        {
            // Act
            var actual = new LoadProgress();

            // Assert
            Assert.AreEqual(0, actual.BatchNumber);
            Assert.AreEqual(0L, actual.LoadedRowCount);
            Assert.AreEqual(0L, actual.TotalRowCount);
            Assert.AreEqual(default(DateTime), actual.Start);
        }

        [TestMethod]
        public void TestLoadProgressPropertiesCanBeSet()
        {
            // Setup
            var start = new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc);

            // Act
            var actual = new LoadProgress
            {
                BatchNumber = 3,
                LoadedRowCount = 1500,
                TotalRowCount = 4000,
                Start = start
            };

            // Assert
            Assert.AreEqual(3, actual.BatchNumber);
            Assert.AreEqual(1500L, actual.LoadedRowCount);
            Assert.AreEqual(4000L, actual.TotalRowCount);
            Assert.AreEqual(start, actual.Start);
        }
    }
}
