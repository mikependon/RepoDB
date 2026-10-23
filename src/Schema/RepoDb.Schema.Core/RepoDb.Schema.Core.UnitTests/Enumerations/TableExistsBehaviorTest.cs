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
    public class TableExistsBehaviorTest
    {
        #region Methods

        [TestMethod]
        public void TestCopyTableSchemaExistsBehaviorDefaultValue()
        {
            // Act
            var actual = default(TableExistsBehavior);
            var expected = TableExistsBehavior.Skip;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCopyTableSchemaExistsBehaviorValues()
        {
            // Act
            var actual = Enum.GetNames(typeof(TableExistsBehavior));
            var expected = new[] { "Skip", "Align", "Throw", "Drop" };

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        #endregion
    }
}
