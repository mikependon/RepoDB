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
        public void TestRelationshipInfoTablePropertyDefaultValue()
        {
            // Act
            var relationship = new RelationshipInfo();
            var actual = relationship.Table;

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestRelationshipInfoTableProperty()
        {
            // Setup
            var table = new TableSchema { TableName = "Person" };

            // Act
            var relationship = new RelationshipInfo
            {
                Table = table
            };
            var actual = relationship.Table;

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
            var parent = new RelationshipInfo { Table = new TableSchema { TableName = "Country" } };

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
            var child = new RelationshipInfo { Table = new TableSchema { TableName = "Person" } };

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
            var a = new RelationshipInfo { Table = new TableSchema { TableName = "CycleA" } };
            var b = new RelationshipInfo { Table = new TableSchema { TableName = "CycleB" } };

            // Act
            a.Parents.Add(b);
            b.Parents.Add(a);

            // Assert
            Assert.AreSame(b, a.Parents[0]);
            Assert.AreSame(a, a.Parents[0].Parents[0]);
        }

        #endregion
    }
}
