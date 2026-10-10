#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Data.Core.UnitTests
{
    [TestClass]
    public class DataTraceKeysTest
    {
        [TestMethod]
        public void TestDataTraceKeysCopyDataTo()
        {
            // Assert
            Assert.AreEqual("CopyDataTo", DataTraceKeys.CopyDataTo);
        }

        [TestMethod]
        public void TestDataTraceKeysMoveDataTo()
        {
            // Assert
            Assert.AreEqual("MoveDataTo", DataTraceKeys.MoveDataTo);
        }

        [TestMethod]
        public void TestDataTraceKeysAreDifferent()
        {
            // Assert
            Assert.AreNotEqual(DataTraceKeys.CopyDataTo, DataTraceKeys.MoveDataTo);
        }
    }
}
