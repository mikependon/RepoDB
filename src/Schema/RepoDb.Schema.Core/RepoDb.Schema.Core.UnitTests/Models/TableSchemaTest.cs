#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests.Models
{
    [TestClass]
    public class TableSchemaTest
    {
        #region Methods

        [TestMethod]
        public void TestTableSchemaEqualsWithSameValues()
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
        public void TestTableSchemaEqualsWithSameInstance()
        {
            // Act
            var a = Create();

            // Assert
            Assert.IsTrue(a.Equals(a));
            Assert.IsTrue(a == a);
        }

        [TestMethod]
        public void TestTableSchemaEqualsWithNull()
        {
            // Act
            var a = Create();

            // Assert
            Assert.IsFalse(a.Equals((object)null));
            Assert.IsFalse(a.Equals(null));
            Assert.IsFalse(a == null);
            Assert.IsFalse(null == a);
            Assert.IsTrue(a != null);
            Assert.IsTrue(((TableSchema)null) == null);
        }

        [TestMethod]
        public void TestTableSchemaEqualsWithOtherType()
        {
            // Act
            var a = Create();

            // Assert
            Assert.IsFalse(a.Equals(new object()));
        }

        [TestMethod]
        public void TestTableSchemaNotEqualsWithDifferentTable()
        {
            // Act
            var a = Create();
            var b = Create();
            b.Table = new TableInfo("Other", "dbo");

            // Assert
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a == b);
            Assert.IsTrue(a != b);
        }

        [TestMethod]
        public void TestTableSchemaNotEqualsWithDifferentPrimaryKey()
        {
            // Act
            var a = Create();
            var b = Create();
            b.PrimaryKey = null;

            // Assert
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a == b);
            Assert.IsTrue(a != b);
        }

        [TestMethod]
        public void TestTableSchemaNotEqualsWithDifferentColumns()
        {
            // Act
            var a = Create();
            var b = Create();
            b.Columns = new List<ColumnInfo>();

            // Assert
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a == b);
            Assert.IsTrue(a != b);
        }

        [TestMethod]
        public void TestTableSchemaNotEqualsWithDifferentIndexes()
        {
            // Act
            var a = Create();
            var b = Create();
            b.Indexes = new List<IndexInfo>();

            // Assert
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a == b);
            Assert.IsTrue(a != b);
        }

        [TestMethod]
        public void TestTableSchemaNotEqualsWithDifferentForeignKeys()
        {
            // Act
            var a = Create();
            var b = Create();
            b.ForeignKeys = new List<ForeignKeyInfo>();

            // Assert
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a == b);
            Assert.IsTrue(a != b);
        }

        [TestMethod]
        public void TestTableSchemaNotEqualsWithDifferentUniqueConstraints()
        {
            // Act
            var a = Create();
            var b = Create();
            b.UniqueConstraints = new List<UniqueConstraintInfo>();

            // Assert
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a == b);
            Assert.IsTrue(a != b);
        }

        [TestMethod]
        public void TestTableSchemaNotEqualsWithDifferentCheckConstraints()
        {
            // Act
            var a = Create();
            var b = Create();
            b.CheckConstraints = new List<CheckConstraintInfo>();

            // Assert
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a == b);
            Assert.IsTrue(a != b);
        }

        [TestMethod]
        public void TestTableSchemaConstructorWithNameAndSchema()
        {
            // Act
            var actual = new TableSchema("Person", "dbo");

            // Assert
            Assert.IsNotNull(actual.Table);
            Assert.AreEqual("Person", actual.Table.Name, StringComparer.Ordinal);
            Assert.AreEqual("dbo", actual.Table.Schema, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestTableSchemaTableNamePropertyDefaultValue()
        {
            // Act
            var schema = new TableSchema(null, null);
            var actual = schema.Table.Name;

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestTableSchemaTableNameProperty()
        {
            // Act
            var schema = new TableSchema("Person", null);
            var actual = schema.Table.Name;
            var expected = "Person";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestTableSchemaSchemaNamePropertyDefaultValue()
        {
            // Act
            var schema = new TableSchema(null, null);
            var actual = schema.Table.Schema;

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestTableSchemaSchemaNameProperty()
        {
            // Act
            var schema = new TableSchema(null, "dbo");
            var actual = schema.Table.Schema;
            var expected = "dbo";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestTableSchemaPrimaryKeyPropertyDefaultValue()
        {
            // Act
            var schema = new TableSchema(null, null);
            var actual = schema.PrimaryKey;

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestTableSchemaPrimaryKeyProperty()
        {
            // Setup
            var primaryKey = new PrimaryKeyInfo("PK_Person");

            // Act
            var schema = new TableSchema(null, null)
            {
                PrimaryKey = primaryKey
            };
            var actual = schema.PrimaryKey;

            // Assert
            Assert.AreSame(primaryKey, actual);
        }

        [TestMethod]
        public void TestTableSchemaColumnsPropertyDefaultValue()
        {
            // Act
            var schema = new TableSchema(null, null);
            var actual = schema.Columns;

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void TestTableSchemaIndexesPropertyDefaultValue()
        {
            // Act
            var schema = new TableSchema(null, null);
            var actual = schema.Indexes;

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void TestTableSchemaForeignKeysPropertyDefaultValue()
        {
            // Act
            var schema = new TableSchema(null, null);
            var actual = schema.ForeignKeys;

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void TestTableSchemaUniqueConstraintsPropertyDefaultValue()
        {
            // Act
            var schema = new TableSchema(null, null);
            var actual = schema.UniqueConstraints;

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void TestTableSchemaCheckConstraintsPropertyDefaultValue()
        {
            // Act
            var schema = new TableSchema(null, null);
            var actual = schema.CheckConstraints;

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void TestTableSchemaCollectionsAreNotShared()
        {
            // Act
            var first = new TableSchema(null, null);
            var second = new TableSchema(null, null);
            first.Columns.Add(new ColumnInfo());

            // Assert
            Assert.AreEqual(1, first.Columns.Count);
            Assert.AreEqual(0, second.Columns.Count);
        }

        #endregion

        #region Helpers

        private static TableSchema Create() =>
            new TableSchema("Person", "dbo") { PrimaryKey = new PrimaryKeyInfo("PK_Person") { Columns = new List<string> { "Id" } }, Columns = new List<ColumnInfo> { new ColumnInfo { Ordinal = 1 } }, Indexes = new List<IndexInfo> { new IndexInfo("IX") }, ForeignKeys = new List<ForeignKeyInfo> { new ForeignKeyInfo("FK") }, UniqueConstraints = new List<UniqueConstraintInfo> { new UniqueConstraintInfo("UQ") }, CheckConstraints = new List<CheckConstraintInfo> { new CheckConstraintInfo("CK") } };

        #endregion
    }
}
