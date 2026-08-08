using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Data.Db2;
using RepoDb.Attributes.Parameter.Db2;
using RepoDb.DbSettings;
using RepoDb.Extensions;

namespace RepoDb.Db2.UnitTests.Attributes.Parameter.Db2
{
    [TestClass]
    public class Db2TypeOutputAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<DB2Connection>(new Db2DbSetting(), true);
        }

        #region Classes

        private class Db2TypeOutputAttributeTestClass
        {
            [Db2TypeOutput(true)]
            public object ColumnName { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestDb2TypeOutputAttributeViaEntityViaCreateParameters()
        {
            // Act
            using var connection = new DB2Connection();
            using var command = connection.CreateCommand();

            DbCommandExtension
                .CreateParameters(command, new Db2TypeOutputAttributeTestClass
                {
                    ColumnName = "Test"
                });

            // Assert
            Assert.AreEqual(1, command.Parameters.Count);

            // Assert
            var parameter = (DB2Parameter)command.Parameters[":ColumnName"];
            Assert.IsTrue(parameter.DB2TypeOutput);
        }

        [TestMethod]
        public void TestDb2TypeOutputAttributeViaAnonymousViaCreateParameters()
        {
            // Act
            using var connection = new DB2Connection();
            using var command = connection.CreateCommand();

            DbCommandExtension
                .CreateParameters(command, new
                {
                    ColumnName = "Test"
                },
                typeof(Db2TypeOutputAttributeTestClass));

            // Assert
            Assert.AreEqual(1, command.Parameters.Count);

            // Assert
            var parameter = (DB2Parameter)command.Parameters[":ColumnName"];
            Assert.IsTrue(parameter.DB2TypeOutput);
        }
    }
}
