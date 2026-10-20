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
    public class CopyTableSchemaRelationshipBehaviorTest
    {
        #region Methods

        [TestMethod]
        public void TestCopyTableSchemaRelationshipBehaviorDefaultValue()
        {
            // Act
            var actual = default(CopyTableSchemaRelationshipBehavior);
            var expected = CopyTableSchemaRelationshipBehavior.TableOnly;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCopyTableSchemaRelationshipBehaviorValues()
        {
            // Act
            var actual = Enum.GetNames(typeof(CopyTableSchemaRelationshipBehavior));
            var expected = new[] { "TableOnly", "Parents", "Children", "ParentsAndChildren" };

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        #endregion
    }
}
