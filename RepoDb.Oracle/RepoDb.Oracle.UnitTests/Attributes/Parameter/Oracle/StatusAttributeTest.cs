using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Attributes.Parameter.Oracle;
using RepoDb.DbSettings;
using RepoDb.Extensions;

namespace RepoDb.Oracle.UnitTests.Attributes.Parameter.Oracle
{
    [TestClass]
    public class StatusAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<OracleConnection>(new OracleDbSetting(), true);
        }

        #region Classes

        private class StatusAttributeTestClass
        {
            [Status(OracleParameterStatus.NullFetched)]
            public object ColumnName { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestStatusAttributeViaEntityViaCreateParameters()
        {
            // Act
            using var connection = new OracleConnection();
            using var command = connection.CreateCommand();

            DbCommandExtension
                .CreateParameters(command, new StatusAttributeTestClass
                {
                    ColumnName = "Test"
                });

            // Assert
            Assert.AreEqual(1, command.Parameters.Count);

            // Assert
            var parameter = (OracleParameter)command.Parameters[":ColumnName"];
            Assert.AreEqual(OracleParameterStatus.NullFetched, parameter.Status);
        }

        [TestMethod]
        public void TestStatusAttributeViaAnonymousViaCreateParameters()
        {
            // Act
            using var connection = new OracleConnection();
            using var command = connection.CreateCommand();

            DbCommandExtension
                .CreateParameters(command, new
                {
                    ColumnName = "Test"
                },
                typeof(StatusAttributeTestClass));

            // Assert
            Assert.AreEqual(1, command.Parameters.Count);

            // Assert
            var parameter = (OracleParameter)command.Parameters[":ColumnName"];
            Assert.AreEqual(OracleParameterStatus.NullFetched, parameter.Status);
        }
    }
}
