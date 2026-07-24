using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Attributes.Parameter.Oracle;
using RepoDb.DbSettings;
using RepoDb.Extensions;

namespace RepoDb.Oracle.UnitTests.Attributes.Parameter.Oracle
{
    [TestClass]
    public class OffsetAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<OracleConnection>(new OracleDbSetting(), true);
        }

        #region Classes

        private class OffsetAttributeTestClass
        {
            [Offset(1000)]
            public object ColumnName { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestOffsetAttributeViaEntityViaCreateParameters()
        {
            // Act
            using var connection = new OracleConnection();
            using var command = connection.CreateCommand();

            DbCommandExtension
                .CreateParameters(command, new OffsetAttributeTestClass
                {
                    ColumnName = "Test"
                });

            // Assert
            Assert.AreEqual(1, command.Parameters.Count);

            // Assert
            var parameter = (OracleParameter)command.Parameters[":ColumnName"];
            Assert.AreEqual(1000, parameter.Offset);
        }

        [TestMethod]
        public void TestOffsetAttributeViaAnonymousViaCreateParameters()
        {
            // Act
            using var connection = new OracleConnection();
            using var command = connection.CreateCommand();

            DbCommandExtension
                .CreateParameters(command, new
                {
                    ColumnName = "Test"
                },
                typeof(OffsetAttributeTestClass));

            // Assert
            Assert.AreEqual(1, command.Parameters.Count);

            // Assert
            var parameter = (OracleParameter)command.Parameters[":ColumnName"];
            Assert.AreEqual(1000, parameter.Offset);
        }
    }
}
