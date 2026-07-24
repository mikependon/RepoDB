using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Attributes.Parameter.Oracle;
using RepoDb.DbSettings;
using RepoDb.Extensions;

namespace RepoDb.Oracle.UnitTests.Attributes.Parameter.Oracle
{
    [TestClass]
    public class UdtTypeNameAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<OracleConnection>(new OracleDbSetting(), true);
        }

        #region Classes

        private class UdtTypeNameAttributeTestClass
        {
            [UdtTypeName("SCHEMA.MY_TYPE")]
            public object ColumnName { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestUdtTypeNameAttributeViaEntityViaCreateParameters()
        {
            // Act
            using var connection = new OracleConnection();
            using var command = connection.CreateCommand();

            DbCommandExtension
                .CreateParameters(command, new UdtTypeNameAttributeTestClass
                {
                    ColumnName = "Test"
                });

            // Assert
            Assert.AreEqual(1, command.Parameters.Count);

            // Assert
            var parameter = (OracleParameter)command.Parameters[":ColumnName"];
            Assert.AreEqual("SCHEMA.MY_TYPE", parameter.UdtTypeName);
        }

        [TestMethod]
        public void TestUdtTypeNameAttributeViaAnonymousViaCreateParameters()
        {
            // Act
            using var connection = new OracleConnection();
            using var command = connection.CreateCommand();

            DbCommandExtension
                .CreateParameters(command, new
                {
                    ColumnName = "Test"
                },
                typeof(UdtTypeNameAttributeTestClass));

            // Assert
            Assert.AreEqual(1, command.Parameters.Count);

            // Assert
            var parameter = (OracleParameter)command.Parameters[":ColumnName"];
            Assert.AreEqual("SCHEMA.MY_TYPE", parameter.UdtTypeName);
        }
    }
}
