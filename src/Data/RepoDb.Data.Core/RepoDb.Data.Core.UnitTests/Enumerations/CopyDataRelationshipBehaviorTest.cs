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
    public class CopyDataRelationshipBehaviorTest
    {
        #region Methods

        [TestMethod]
        public void TestCopyDataRelationshipBehaviorDefaultValue()
        {
            // Act
            var actual = default(CopyDataRelationshipBehavior);
            var expected = CopyDataRelationshipBehavior.TableOnly;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCopyDataRelationshipBehaviorValues()
        {
            // Act
            var actual = Enum.GetNames(typeof(CopyDataRelationshipBehavior));
            var expected = new[] { "TableOnly", "Parents", "Children", "ParentsAndChildren" };

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCopyDataRelationshipBehaviorHasTheSameValuesAsTheCopySchemaRelationshipBehavior()
        {
            // Act
            var data = Enum.GetNames(typeof(CopyDataRelationshipBehavior)).Select(name => (name, (int)Enum.Parse(typeof(CopyDataRelationshipBehavior), name)));
            var schema = Enum.GetNames(typeof(CopySchemaRelationshipBehavior)).Select(name => (name, (int)Enum.Parse(typeof(CopySchemaRelationshipBehavior), name)));

            // Assert
            CollectionAssert.AreEqual(schema.ToArray(), data.ToArray());
        }

        #endregion
    }
}
