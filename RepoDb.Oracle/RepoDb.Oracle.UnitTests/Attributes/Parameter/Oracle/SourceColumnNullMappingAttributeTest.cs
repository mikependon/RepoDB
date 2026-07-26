using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Attributes.Parameter.Oracle;
using RepoDb.DbSettings;
using RepoDb.Extensions;

namespace RepoDb.Oracle.UnitTests.Attributes.Parameter.Oracle
{
    [TestClass]
    public class SourceColumnNullMappingAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<OracleConnection>(new OracleDbSetting(), true);
        }

        #region Classes

        private class SourceColumnNullMappingAttributeTestClass
        {
            [SourceColumnNullMapping(true)]
            public object ColumnName { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestSourceColumnNullMappingAttributeViaEntityViaCreateParameters()
        {
            // Act
            using var connection = new OracleConnection();
            using var command = connection.CreateCommand();

            DbCommandExtension
                .CreateParameters(command, new SourceColumnNullMappingAttributeTestClass
                {
                    ColumnName = "Test"
                });

            // Assert
            Assert.AreEqual(1, command.Parameters.Count);

            // Assert
            var parameter = (OracleParameter)command.Parameters[":ColumnName"];
            Assert.IsTrue(parameter.SourceColumnNullMapping);
        }

        [TestMethod]
        public void TestSourceColumnNullMappingAttributeViaAnonymousViaCreateParameters()
        {
            // Act
            using var connection = new OracleConnection();
            using var command = connection.CreateCommand();

            DbCommandExtension
                .CreateParameters(command, new
                {
                    ColumnName = "Test"
                },
                typeof(SourceColumnNullMappingAttributeTestClass));

            // Assert
            Assert.AreEqual(1, command.Parameters.Count);

            // Assert
            var parameter = (OracleParameter)command.Parameters[":ColumnName"];
            Assert.IsTrue(parameter.SourceColumnNullMapping);
        }
    }
}
