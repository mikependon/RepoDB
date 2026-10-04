#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests.Models
{
    [TestClass]
    public class RelationshipInfoTest
    {
        #region Methods

        [TestMethod]
        public void TestRelationshipInfoEqualsWithSameValues()
        {
            // Act
            var a = Create();
            var b = Create();

            // Assert
            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(a.Equals((object)b));
            Assert.IsTrue(a == b);
            Assert.IsFalse(a != b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [TestMethod]
        public void TestRelationshipInfoEqualsWithSameInstance()
        {
            // Act
            var a = Create();

            // Assert
            Assert.IsTrue(a.Equals(a));
            Assert.IsTrue(a == a);
        }

        [TestMethod]
        public void TestRelationshipInfoEqualsWithNull()
        {
            // Act
            var a = Create();

            // Assert
            Assert.IsFalse(a.Equals((object)null));
            Assert.IsFalse(a.Equals(null));
            Assert.IsFalse(a == null);
            Assert.IsFalse(null == a);
            Assert.IsTrue(a != null);
            Assert.IsTrue(((RelationshipInfo)null) == null);
        }

        [TestMethod]
        public void TestRelationshipInfoEqualsWithOtherType()
        {
            // Act
            var a = Create();

            // Assert
            Assert.IsFalse(a.Equals(new object()));
        }

        [TestMethod]
        public void TestRelationshipInfoNotEqualsWithDifferentSchema()
        {
            // Act
            var a = Create();
            var b = Create();
            b.Schema = new TableSchema("Other", "dbo");

            // Assert
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a == b);
            Assert.IsTrue(a != b);
        }

        [TestMethod]
        public void TestRelationshipInfoTablePropertyDefaultValue()
        {
            // Act
            var relationship = new RelationshipInfo();
            var actual = relationship.Schema;

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestRelationshipInfoTableProperty()
        {
            // Setup
            var table = new TableSchema("Person", null);

            // Act
            var relationship = new RelationshipInfo
            {
                Schema = table
            };
            var actual = relationship.Schema;

            // Assert
            Assert.AreSame(table, actual);
        }

        [TestMethod]
        public void TestRelationshipInfoParentsPropertyDefaultValue()
        {
            // Act
            var relationship = new RelationshipInfo();
            var actual = relationship.Parents;

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void TestRelationshipInfoParentsProperty()
        {
            // Setup
            var parent = new RelationshipInfo { Schema = new TableSchema("Country", null) };

            // Act
            var relationship = new RelationshipInfo();
            relationship.Parents.Add(parent);
            var actual = relationship.Parents;

            // Assert
            Assert.AreEqual(1, actual.Count);
            Assert.AreSame(parent, actual[0]);
        }

        [TestMethod]
        public void TestRelationshipInfoChildrenPropertyDefaultValue()
        {
            // Act
            var relationship = new RelationshipInfo();
            var actual = relationship.Children;

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void TestRelationshipInfoChildrenProperty()
        {
            // Setup
            var child = new RelationshipInfo { Schema = new TableSchema("Person", null) };

            // Act
            var relationship = new RelationshipInfo();
            relationship.Children.Add(child);
            var actual = relationship.Children;

            // Assert
            Assert.AreEqual(1, actual.Count);
            Assert.AreSame(child, actual[0]);
        }

        [TestMethod]
        public void TestRelationshipInfoCollectionsAreNotShared()
        {
            // Act
            var first = new RelationshipInfo();
            var second = new RelationshipInfo();
            first.Parents.Add(new RelationshipInfo());
            first.Children.Add(new RelationshipInfo());

            // Assert
            Assert.AreEqual(0, second.Parents.Count);
            Assert.AreEqual(0, second.Children.Count);
        }

        [TestMethod]
        public void TestRelationshipInfoCanReferenceItselfCircularly()
        {
            // Setup
            var a = new RelationshipInfo { Schema = new TableSchema("CycleA", null) };
            var b = new RelationshipInfo { Schema = new TableSchema("CycleB", null) };

            // Act
            a.Parents.Add(b);
            b.Parents.Add(a);

            // Assert
            Assert.AreSame(b, a.Parents[0]);
            Assert.AreSame(a, a.Parents[0].Parents[0]);
        }

        #endregion

        #region Helpers

        private static RelationshipInfo Create() =>
            new RelationshipInfo { Schema = new TableSchema("Person", "dbo") };

        #endregion
    }
}
