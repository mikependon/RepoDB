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
    public class CopySchemaRelationshipBehaviorTest
    {
        #region Methods

        [TestMethod]
        public void TestCopySchemaRelationshipBehaviorDefaultValue()
        {
            // Act
            var actual = default(CopySchemaRelationshipBehavior);
            var expected = CopySchemaRelationshipBehavior.TableOnly;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCopySchemaRelationshipBehaviorValues()
        {
            // Act
            var actual = Enum.GetNames(typeof(CopySchemaRelationshipBehavior));
            var expected = new[] { "TableOnly", "Parents", "Children", "All" };

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        #endregion
    }
}
