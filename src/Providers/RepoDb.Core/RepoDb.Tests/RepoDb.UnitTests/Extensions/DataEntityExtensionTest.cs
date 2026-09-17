#region Copyright Attributions

// Copyright (c) 2022 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Extensions
{
    [TestClass]
    public class DataEntityExtensionTest
    {
        [TestCleanup]
        public void Cleanup()
        {
            PropertyValueAttributeMapper.Clear();
        }

        #region GetSchema

        [TestMethod]
        public void TestDataEntityExtensionGetSchema()
        {
            // Prepare
            var dbSetting = new CustomDbSetting();

            // Act
            var schema = DataEntityExtension.GetSchema("SchemaName.TableName", dbSetting);

            // Assert
            Assert.AreEqual("SchemaName", schema, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDataEntityExtensionGetSchemaFromQuoted()
        {
            // Prepare
            var dbSetting = new CustomDbSetting();

            // Act
            var schema = DataEntityExtension.GetSchema("[SchemaName].[TableName]", dbSetting);

            // Assert
            Assert.AreEqual("[SchemaName]", schema, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDataEntityExtensionGetSchemaFromWhitespaced()
        {
            // Prepare
            var dbSetting = new CustomDbSetting();

            // Act
            var schema = DataEntityExtension.GetSchema("Schema Name.TableName", dbSetting);

            // Assert
            Assert.AreEqual("Schema Name", schema, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDataEntityExtensionGetSchemaFromQuotedAndIsDotted()
        {
            // Prepare
            var dbSetting = new CustomDbSetting();

            // Act
            var schema = DataEntityExtension.GetSchema("[Schema.Name].[TableName]", dbSetting);

            // Assert
            Assert.AreEqual("[Schema.Name]", schema, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDataEntityExtensionGetSchemaFromQuotedAndIsWhitespaced()
        {
            // Prepare
            var dbSetting = new CustomDbSetting();

            // Act
            var schema = DataEntityExtension.GetSchema("[Schema Name].[TableName]", dbSetting);

            // Assert
            Assert.AreEqual("[Schema Name]", schema, StringComparer.Ordinal);
        }

        #endregion

        #region GetTableName

        [TestMethod]
        public void TestDataEntityExtensionGetTableName()
        {
            // Prepare
            var dbSetting = new CustomDbSetting();

            // Act
            var schema = DataEntityExtension.GetTableName("TableName", dbSetting);

            // Assert
            Assert.AreEqual("TableName", schema, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDataEntityExtensionGetTableNameFromQuoted()
        {
            // Prepare
            var dbSetting = new CustomDbSetting();

            // Act
            var schema = DataEntityExtension.GetTableName("[TableName]", dbSetting);

            // Assert
            Assert.AreEqual("[TableName]", schema, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDataEntityExtensionGetTableNameFromWhitespaced()
        {
            // Prepare
            var dbSetting = new CustomDbSetting();

            // Act
            var schema = DataEntityExtension.GetTableName("Table Name", dbSetting);

            // Assert
            Assert.AreEqual("Table Name", schema, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDataEntityExtensionGetTableNameFromDotted()
        {
            // Prepare
            var dbSetting = new CustomDbSetting();

            // Act
            var schema = DataEntityExtension.GetTableName("[Table.Name]", dbSetting);

            // Assert
            Assert.AreEqual("[Table.Name]", schema, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDataEntityExtensionGetTableNameFromQuotedAndIsWhitespaced()
        {
            // Prepare
            var dbSetting = new CustomDbSetting();

            // Act
            var schema = DataEntityExtension.GetTableName("[Table Name]", dbSetting);

            // Assert
            Assert.AreEqual("[Table Name]", schema, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDataEntityExtensionGetTableNameWithSchema()
        {
            // Prepare
            var dbSetting = new CustomDbSetting();

            // Act
            var schema = DataEntityExtension.GetTableName("SchemaName.TableName", dbSetting);

            // Assert
            Assert.AreEqual("TableName", schema, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDataEntityExtensionGetTableNameWithSchemaAndIsWhitespaced()
        {
            // Prepare
            var dbSetting = new CustomDbSetting();

            // Act
            var schema = DataEntityExtension.GetTableName("SchemaName.Table Name", dbSetting);

            // Assert
            Assert.AreEqual("Table Name", schema, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDataEntityExtensionGetTableNameWithSchemaFromQuoted()
        {
            // Prepare
            var dbSetting = new CustomDbSetting();

            // Act
            var schema = DataEntityExtension.GetTableName("[SchemaName].[TableName]", dbSetting);

            // Assert
            Assert.AreEqual("[TableName]", schema, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDataEntityExtensionGetTableNameWithSchemaFromQuotedAndIsDotted()
        {
            // Prepare
            var dbSetting = new CustomDbSetting();

            // Act
            var schema = DataEntityExtension.GetTableName("[SchemaName].[Table.Name]", dbSetting);

            // Assert
            Assert.AreEqual("[Table.Name]", schema, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDataEntityExtensionGetTableNameWithSchemaFromQuotedAndIsWhitespaced()
        {
            // Prepare
            var dbSetting = new CustomDbSetting();

            // Act
            var schema = DataEntityExtension.GetTableName("[SchemaName].[Table Name]", dbSetting);

            // Assert
            Assert.AreEqual("[Table Name]", schema, StringComparer.Ordinal);
        }

        #endregion
    }
}
