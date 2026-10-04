#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
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
            var schema = new TableSchema();
            var actual = schema.Table.Name;

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestTableSchemaTableNameProperty()
        {
            // Act
            var schema = new TableSchema
            {
                Table = new TableInfo { Name = "Person" }
            };
            var actual = schema.Table.Name;
            var expected = "Person";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestTableSchemaSchemaNamePropertyDefaultValue()
        {
            // Act
            var schema = new TableSchema();
            var actual = schema.Table.Schema;

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestTableSchemaSchemaNameProperty()
        {
            // Act
            var schema = new TableSchema
            {
                Table = new TableInfo { Schema = "dbo" }
            };
            var actual = schema.Table.Schema;
            var expected = "dbo";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestTableSchemaPrimaryKeyPropertyDefaultValue()
        {
            // Act
            var schema = new TableSchema();
            var actual = schema.PrimaryKey;

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestTableSchemaPrimaryKeyProperty()
        {
            // Setup
            var primaryKey = new PrimaryKeyInfo { Name = "PK_Person" };

            // Act
            var schema = new TableSchema
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
            var schema = new TableSchema();
            var actual = schema.Columns;

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void TestTableSchemaIndexesPropertyDefaultValue()
        {
            // Act
            var schema = new TableSchema();
            var actual = schema.Indexes;

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void TestTableSchemaForeignKeysPropertyDefaultValue()
        {
            // Act
            var schema = new TableSchema();
            var actual = schema.ForeignKeys;

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void TestTableSchemaUniqueConstraintsPropertyDefaultValue()
        {
            // Act
            var schema = new TableSchema();
            var actual = schema.UniqueConstraints;

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void TestTableSchemaCheckConstraintsPropertyDefaultValue()
        {
            // Act
            var schema = new TableSchema();
            var actual = schema.CheckConstraints;

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void TestTableSchemaCollectionsAreNotShared()
        {
            // Act
            var first = new TableSchema();
            var second = new TableSchema();
            first.Columns.Add(new ColumnInfo());

            // Assert
            Assert.AreEqual(1, first.Columns.Count);
            Assert.AreEqual(0, second.Columns.Count);
        }

        #endregion
    }
}
