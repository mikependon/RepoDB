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

        /*
         * Unquoted
         */

        [TestMethod]
        public void TestDataEntityExtensionGetSchema()
        {
            // Act
            var schema = DataEntityExtension.GetSchema("SchemaName.TableName");

            // Assert
            Assert.AreEqual("SchemaName", schema);
        }

        [TestMethod]
        public void TestDataEntityExtensionGetSchemaWithDbSetting()
        {
            // Prepare
            var dbSetting = new CustomDbSetting();

            // Act
            var schema = DataEntityExtension.GetSchema("SchemaName.TableName", dbSetting);

            // Assert
            Assert.AreEqual("SchemaName", schema);
        }

        /*
         * Quoted
         */

        [TestMethod]
        public void TestDataEntityExtensionGetSchemaFromQuoted()
        {
            // Act
            var schema = DataEntityExtension.GetSchema("[SchemaName].[TableName]");

            // Assert
            Assert.AreEqual("[SchemaName]", schema);
        }

        [TestMethod]
        public void TestDataEntityExtensionGetSchemaFromQuotedWithDbSetting()
        {
            // Prepare
            var dbSetting = new CustomDbSetting();

            // Act
            var schema = DataEntityExtension.GetSchema("[SchemaName].[TableName]", dbSetting);

            // Assert
            Assert.AreEqual("[SchemaName]", schema);
        }

        /*
         * Dotted
         */

        [TestMethod]
        public void TestDataEntityExtensionGetSchemaFromDotted()
        {
            // Act
            var schema = DataEntityExtension.GetSchema("[Schema.Name].[TableName]");

            // Assert
            Assert.AreEqual("[Schema.Name]", schema);
        }

        [TestMethod]
        public void TestDataEntityExtensionGetSchemaFromDottedWithDbSetting()
        {
            // Prepare
            var dbSetting = new CustomDbSetting();

            // Act
            var schema = DataEntityExtension.GetSchema("[Schema.Name].[TableName]", dbSetting);

            // Assert
            Assert.AreEqual("[Schema.Name]", schema);
        }

        #endregion

        #region GetTableName

        /*
         * Unquoted
         */

        [TestMethod]
        public void TestDataEntityExtensionGetTableName()
        {
            // Act
            var schema = DataEntityExtension.GetTableName("SchemaName.TableName");

            // Assert
            Assert.AreEqual("TableName", schema);
        }

        [TestMethod]
        public void TestDataEntityExtensionGetTableNameWithDbSetting()
        {
            // Prepare
            var dbSetting = new CustomDbSetting();

            // Act
            var schema = DataEntityExtension.GetTableName("SchemaName.TableName", dbSetting);

            // Assert
            Assert.AreEqual("TableName", schema);
        }

        /*
         * Quoted
         */

        [TestMethod]
        public void TestDataEntityExtensionGetTableNameFromQuoted()
        {
            // Act
            var schema = DataEntityExtension.GetTableName("[SchemaName].[TableName]");

            // Assert
            Assert.AreEqual("[TableName]", schema);
        }

        [TestMethod]
        public void TestDataEntityExtensionGetTableNameFromQuotedWithDbSetting()
        {
            // Prepare
            var dbSetting = new CustomDbSetting();

            // Act
            var schema = DataEntityExtension.GetTableName("[SchemaName].[TableName]", dbSetting);

            // Assert
            Assert.AreEqual("[TableName]", schema);
        }

        /*
         * Dotted
         */

        [TestMethod]
        public void TestDataEntityExtensionGetTableNameFromDotted()
        {
            // Act
            var schema = DataEntityExtension.GetTableName("[SchemaName].[Table.Name]");

            // Assert
            Assert.AreEqual("[Table.Name]", schema);
        }

        [TestMethod]
        public void TestDataEntityExtensionGetTableNameFromDottedWithDbSetting()
        {
            // Prepare
            var dbSetting = new CustomDbSetting();

            // Act
            var schema = DataEntityExtension.GetTableName("[SchemaName].[Table.Name]", dbSetting);

            // Assert
            Assert.AreEqual("[Table.Name]", schema);
        }

        #endregion
    }
}
