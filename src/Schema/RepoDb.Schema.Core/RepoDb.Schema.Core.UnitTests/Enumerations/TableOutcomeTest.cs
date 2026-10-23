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
    public class TableOutcomeTest
    {
        #region Methods

        [TestMethod]
        public void TestCopyTableSchemaOutcomeDefaultValue()
        {
            // Act
            var actual = default(TableOutcome);
            var expected = TableOutcome.Created;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCopyTableSchemaOutcomeValues()
        {
            // Act
            var actual = Enum.GetNames(typeof(TableOutcome));
            var expected = new[] { "Created", "Skipped", "Aligned", "Dropped", "Failed" };

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        #endregion
    }
}
