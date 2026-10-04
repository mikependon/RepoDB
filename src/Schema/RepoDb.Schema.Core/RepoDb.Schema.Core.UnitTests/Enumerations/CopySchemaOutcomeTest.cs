#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.Core.UnitTests.Enumerations
{
    [TestClass]
    public class CopySchemaOutcomeTest
    {
        #region Methods

        [TestMethod]
        public void TestCopySchemaOutcomeDefaultValue()
        {
            // Act
            var actual = default(CopySchemaOutcome);
            var expected = CopySchemaOutcome.Created;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCopySchemaOutcomeValues()
        {
            // Act
            var actual = Enum.GetNames(typeof(CopySchemaOutcome));
            var expected = new[] { "Created", "Skipped", "Aligned", "Dropped", "Failed" };

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        #endregion
    }
}
