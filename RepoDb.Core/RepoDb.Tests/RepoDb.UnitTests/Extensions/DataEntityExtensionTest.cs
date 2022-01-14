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
            Assert.AreEqual("SchemaName", schema);
        }

        [TestMethod]
        public void TestDataEntityExtensionGetSchemaFromQuoted()
        {
            // Prepare
            var dbSetting = new CustomDbSetting();

            // Act
            var schema = DataEntityExtension.GetSchema("[SchemaName].[TableName]", dbSetting);

            // Assert
            Assert.AreEqual("[SchemaName]", schema);
        }

        [TestMethod]
        public void TestDataEntityExtensionGetSchemaFromWhitespaced()
        {
            // Prepare
            var dbSetting = new CustomDbSetting();

            // Act
            var schema = DataEntityExtension.GetSchema("Schema Name.TableName", dbSetting);

            // Assert
            Assert.AreEqual("Schema Name", schema);
        }

        [TestMethod]
        public void TestDataEntityExtensionGetSchemaFromQuotedAndIsDotted()
        {
            // Prepare
            var dbSetting = new CustomDbSetting();

            // Act
            var schema = DataEntityExtension.GetSchema("[Schema.Name].[TableName]", dbSetting);

            // Assert
            Assert.AreEqual("[Schema.Name]", schema);
        }

        [TestMethod]
        public void TestDataEntityExtensionGetSchemaFromQuotedAndIsWhitespaced()
        {
            // Prepare
            var dbSetting = new CustomDbSetting();

            // Act
            var schema = DataEntityExtension.GetSchema("[Schema Name].[TableName]", dbSetting);

            // Assert
            Assert.AreEqual("[Schema Name]", schema);
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
            Assert.AreEqual("TableName", schema);
        }

        [TestMethod]
        public void TestDataEntityExtensionGetTableNameFromQuoted()
        {
            // Prepare
            var dbSetting = new CustomDbSetting();

            // Act
            var schema = DataEntityExtension.GetTableName("[TableName]", dbSetting);

            // Assert
            Assert.AreEqual("[TableName]", schema);
        }

        [TestMethod]
        public void TestDataEntityExtensionGetTableNameFromWhitespaced()
        {
            // Prepare
            var dbSetting = new CustomDbSetting();

            // Act
            var schema = DataEntityExtension.GetTableName("Table Name", dbSetting);

            // Assert
            Assert.AreEqual("Table Name", schema);
        }

        [TestMethod]
        public void TestDataEntityExtensionGetTableNameFromDotted()
        {
            // Prepare
            var dbSetting = new CustomDbSetting();

            // Act
            var schema = DataEntityExtension.GetTableName("[Table.Name]", dbSetting);

            // Assert
            Assert.AreEqual("[Table.Name]", schema);
        }

        [TestMethod]
        public void TestDataEntityExtensionGetTableNameFromQuotedAndIsWhitespaced()
        {
            // Prepare
            var dbSetting = new CustomDbSetting();

            // Act
            var schema = DataEntityExtension.GetTableName("[Table Name]", dbSetting);

            // Assert
            Assert.AreEqual("[Table Name]", schema);
        }

        [TestMethod]
        public void TestDataEntityExtensionGetTableNameWithSchema()
        {
            // Prepare
            var dbSetting = new CustomDbSetting();

            // Act
            var schema = DataEntityExtension.GetTableName("SchemaName.TableName", dbSetting);

            // Assert
            Assert.AreEqual("TableName", schema);
        }

        [TestMethod]
        public void TestDataEntityExtensionGetTableNameWithSchemaAndIsWhitespaced()
        {
            // Prepare
            var dbSetting = new CustomDbSetting();

            // Act
            var schema = DataEntityExtension.GetTableName("SchemaName.Table Name", dbSetting);

            // Assert
            Assert.AreEqual("Table Name", schema);
        }

        [TestMethod]
        public void TestDataEntityExtensionGetTableNameWithSchemaFromQuoted()
        {
            // Prepare
            var dbSetting = new CustomDbSetting();

            // Act
            var schema = DataEntityExtension.GetTableName("[SchemaName].[TableName]", dbSetting);

            // Assert
            Assert.AreEqual("[TableName]", schema);
        }

        [TestMethod]
        public void TestDataEntityExtensionGetTableNameWithSchemaFromQuotedAndIsDotted()
        {
            // Prepare
            var dbSetting = new CustomDbSetting();

            // Act
            var schema = DataEntityExtension.GetTableName("[SchemaName].[Table.Name]", dbSetting);

            // Assert
            Assert.AreEqual("[Table.Name]", schema);
        }

        [TestMethod]
        public void TestDataEntityExtensionGetTableNameWithSchemaFromQuotedAndIsWhitespaced()
        {
            // Prepare
            var dbSetting = new CustomDbSetting();

            // Act
            var schema = DataEntityExtension.GetTableName("[SchemaName].[Table Name]", dbSetting);

            // Assert
            Assert.AreEqual("[Table Name]", schema);
        }

        #endregion
    }
}
