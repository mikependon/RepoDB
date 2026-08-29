using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Attributes.Parameter.Oracle;
using RepoDb.DbSettings;
using RepoDb.Extensions;

namespace RepoDb.Oracle.UnitTests.Attributes.Parameter.Oracle
{
    [TestClass]
    public class SourceColumnAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<OracleConnection>(new OracleDbSetting(), true);
        }

        #region Classes

        private class SourceColumnAttributeTestClass
        {
            [SourceColumn("MappedColumnName")]
            public object ColumnName { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestSourceColumnAttributeViaEntityViaCreateParameters()
        {
            // Act
            using var connection = new OracleConnection();
            using var command = connection.CreateCommand();

            DbCommandExtension
                .CreateParameters(command, new SourceColumnAttributeTestClass
                {
                    ColumnName = "Test"
                });

            // Assert
            Assert.AreEqual(1, command.Parameters.Count);

            // Assert
            var parameter = (OracleParameter)command.Parameters[":ColumnName"];
            Assert.AreEqual("MappedColumnName", parameter.SourceColumn);
        }

        [TestMethod]
        public void TestSourceColumnAttributeViaAnonymousViaCreateParameters()
        {
            // Act
            using var connection = new OracleConnection();
            using var command = connection.CreateCommand();

            DbCommandExtension
                .CreateParameters(command, new
                {
                    ColumnName = "Test"
                },
                typeof(SourceColumnAttributeTestClass));

            // Assert
            Assert.AreEqual(1, command.Parameters.Count);

            // Assert
            var parameter = (OracleParameter)command.Parameters[":ColumnName"];
            Assert.AreEqual("MappedColumnName", parameter.SourceColumn);
        }
    }
}
