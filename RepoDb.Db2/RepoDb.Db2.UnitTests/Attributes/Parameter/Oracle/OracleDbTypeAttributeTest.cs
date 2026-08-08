using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Data.Db2;
using RepoDb.Attributes.Parameter.Db2;
using RepoDb.DbSettings;
using RepoDb.Extensions;

namespace RepoDb.Db2.UnitTests.Attributes.Parameter.Db2
{
    [TestClass]
    public class Db2DbTypeAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<Db2Connection>(new Db2DbSetting(), true);
        }

        #region Classes

        private class Db2DbTypeAttributeTestClass
        {
            [Db2DbType(Db2DbType.NVarchar2)]
            public object ColumnName { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestDb2DbTypeAttributeViaEntityViaCreateParameters()
        {
            // Act
            using var connection = new Db2Connection();
            using var command = connection.CreateCommand();

            DbCommandExtension
                .CreateParameters(command, new Db2DbTypeAttributeTestClass
                {
                    ColumnName = "Test"
                });

            // Assert
            Assert.AreEqual(1, command.Parameters.Count);

            // Assert
            var parameter = (Db2Parameter)command.Parameters[":ColumnName"];
            Assert.AreEqual(Db2DbType.NVarchar2, parameter.Db2DbType);
        }

        [TestMethod]
        public void TestDb2DbTypeAttributeViaAnonymousViaCreateParameters()
        {
            // Act
            using var connection = new Db2Connection();
            using var command = connection.CreateCommand();

            DbCommandExtension
                .CreateParameters(command, new
                {
                    ColumnName = "Test"
                },
                typeof(Db2DbTypeAttributeTestClass));

            // Assert
            Assert.AreEqual(1, command.Parameters.Count);

            // Assert
            var parameter = (Db2Parameter)command.Parameters[":ColumnName"];
            Assert.AreEqual(Db2DbType.NVarchar2, parameter.Db2DbType);
        }
    }
}
