#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.Enumerations;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Data.Core.UnitTests.Enumerations
{
    [TestClass]
    public class CopyRelationshipBehaviorTest
    {
        #region Methods

        [TestMethod]
        public void TestCopyDataRelationshipBehaviorDefaultValue()
        {
            // Act
            var actual = default(CopyRelationshipBehavior);
            var expected = CopyRelationshipBehavior.TableOnly;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCopyDataRelationshipBehaviorValues()
        {
            // Act
            var actual = Enum.GetNames(typeof(CopyRelationshipBehavior));
            var expected = new[] { "TableOnly", "Parents", "Children", "ParentsAndChildren" };

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCopyDataRelationshipBehaviorHasTheSameValuesAsTheCopyTableSchemaRelationshipBehavior()
        {
            // Act
            var data = Enum.GetNames(typeof(CopyRelationshipBehavior)).Select(name => (name, (int)Enum.Parse(typeof(CopyRelationshipBehavior), name)));
            var schema = Enum.GetNames(typeof(RelatedTableBehavior)).Select(name => (name, (int)Enum.Parse(typeof(RelatedTableBehavior), name)));

            // Assert
            CollectionAssert.AreEqual(schema.ToArray(), data.ToArray());
        }

        #endregion
    }
}
