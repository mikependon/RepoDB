#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.UnitTests.Trace
{
    /// <summary>
    /// Verifies the memory and process I/O usage that is recorded for an execution.
    /// </summary>
    [TestClass]
    public class ResourceUsageTest
    {
        #region Public Methods

        [TestMethod]
        public void TestResourceUsageKeepsTheValuesThatAreGiven()
        {
            // Act
            var actual = new ResourceUsage(1, 2, 3);

            // Assert
            Assert.AreEqual(1L, actual.MemoryBytes);
            Assert.AreEqual(2L, actual.ReadBytes);
            Assert.AreEqual(3L, actual.WriteBytes);
        }

        [TestMethod]
        public void TestResourceUsageKeepsTheValuesThatAreNotAvailableAsNull()
        {
            // Act
            var actual = new ResourceUsage(null, null, null);

            // Assert
            Assert.IsNull(actual.MemoryBytes);
            Assert.IsNull(actual.ReadBytes);
            Assert.IsNull(actual.WriteBytes);
        }

        [TestMethod]
        public void TestResourceUsageSnapshotSubtractGivesTheDifferenceOfEveryValue()
        {
            // Setup
            var start = new ResourceUsageSnapshot(10, 20, 30);
            var end = new ResourceUsageSnapshot(15, 28, 39);

            // Act
            var actual = end.Subtract(start);

            // Assert
            Assert.AreEqual(5L, actual.MemoryBytes);
            Assert.AreEqual(8L, actual.ReadBytes);
            Assert.AreEqual(9L, actual.WriteBytes);
        }

        [TestMethod]
        public void TestResourceUsageSnapshotSubtractNeverGivesNegativeIo()
        {
            // Setup
            var start = new ResourceUsageSnapshot(10, 10, 10);
            var end = new ResourceUsageSnapshot(4, 4, 4);

            // Act
            var actual = end.Subtract(start);

            // Assert
            Assert.AreEqual(0L, actual.ReadBytes);
            Assert.AreEqual(0L, actual.WriteBytes);
        }

        [TestMethod]
        public void TestResourceUsageSnapshotSubtractGivesNegativeMemoryIfTheWorkingSetShrinks()
        {
            // Setup
            var start = new ResourceUsageSnapshot(10, 0, 0);
            var end = new ResourceUsageSnapshot(4, 0, 0);

            // Act
            var actual = end.Subtract(start);

            // Assert
            Assert.AreEqual(-6L, actual.MemoryBytes);
        }

        [TestMethod]
        public void TestResourceUsageSnapshotSubtractGivesNullForAValueThatIsNotAvailable()
        {
            // Setup
            var start = new ResourceUsageSnapshot(1, null, 1);
            var end = new ResourceUsageSnapshot(2, 2, null);

            // Act
            var actual = end.Subtract(start);

            // Assert
            Assert.AreEqual(1L, actual.MemoryBytes);
            Assert.IsNull(actual.ReadBytes);
            Assert.IsNull(actual.WriteBytes);
        }

        [TestMethod]
        public void TestResourceUsageSnapshotCaptureTakesTheWorkingSetOfTheProcess()
        {
            // Setup
            using var process = System.Diagnostics.Process.GetCurrentProcess();

            // Act
            var actual = ResourceUsageSnapshot.Capture();

            // Assert
            Assert.IsTrue(actual.MemoryBytes > 0);
            Assert.IsTrue(actual.MemoryBytes < process.PeakWorkingSet64 * 2);
        }

        [TestMethod]
        public void TestProcessIoParseReadsTheReadAndWrittenBytes()
        {
            // Setup
            var text = "rchar: 100\nwchar: 200\nsyscr: 1\nread_bytes: 5\n";

            // Act
            var actual = ProcessIo.TryParse(text, out var read, out var write);

            // Assert
            Assert.IsTrue(actual);
            Assert.AreEqual(100L, read);
            Assert.AreEqual(200L, write);
        }

        [TestMethod]
        public void TestProcessIoParseFailsIfALineIsMissing()
        {
            // Act
            var actual = ProcessIo.TryParse("rchar: 100\n", out _, out _);

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void TestProcessIoParseFailsForNullAndForAValueThatIsNotANumber()
        {
            // Act/Assert
            Assert.IsFalse(ProcessIo.TryParse(null, out _, out _));
            Assert.IsFalse(ProcessIo.TryParse("rchar: x\nwchar: 1\n", out _, out _));
        }

        [TestMethod]
        public void TestProcessIoTryReadReadsTheCountersOfTheProcessOnWindowsLinuxAndMac()
        {
            // Act
            var actual = ProcessIo.TryRead(out var read, out var write);

            // Assert
            if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
            {
                Assert.IsTrue(actual);
                Assert.IsTrue(read >= 0);
                Assert.IsTrue(write >= 0);
            }
            else
            {
                Assert.IsFalse(actual);
            }
        }

        #endregion
    }
}
