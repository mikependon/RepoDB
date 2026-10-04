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
    public class CopySchemaExistsBehaviorTest
    {
        #region Methods

        [TestMethod]
        public void TestCopySchemaExistsBehaviorDefaultValue()
        {
            // Act
            var actual = default(CopySchemaExistsBehavior);
            var expected = CopySchemaExistsBehavior.SkipOnExists;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCopySchemaExistsBehaviorValues()
        {
            // Act
            var actual = Enum.GetNames(typeof(CopySchemaExistsBehavior));
            var expected = new[] { "SkipOnExists", "AlignOnExists", "ThrowOnExists", "DropOnExists" };

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        #endregion
    }
}
