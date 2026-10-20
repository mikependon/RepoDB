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
    public class CopyTableSchemaExistsBehaviorTest
    {
        #region Methods

        [TestMethod]
        public void TestCopyTableSchemaExistsBehaviorDefaultValue()
        {
            // Act
            var actual = default(CopyTableSchemaExistsBehavior);
            var expected = CopyTableSchemaExistsBehavior.Skip;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCopyTableSchemaExistsBehaviorValues()
        {
            // Act
            var actual = Enum.GetNames(typeof(CopyTableSchemaExistsBehavior));
            var expected = new[] { "Skip", "Align", "Throw", "Drop" };

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        #endregion
    }
}
