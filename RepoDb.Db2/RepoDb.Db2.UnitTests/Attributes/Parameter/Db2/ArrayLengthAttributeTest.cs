using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Data.Db2;
using RepoDb.Attributes.Parameter.Db2;
using RepoDb.DbSettings;
using RepoDb.Extensions;

namespace RepoDb.Db2.UnitTests.Attributes.Parameter.Db2
{
    [TestClass]
    public class ArrayLengthAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<DB2Connection>(new Db2DbSetting(), true);
        }

        #region Classes

        private class ArrayLengthAttributeTestClass
        {
            [ArrayLength(4000)]
            public object ColumnName { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestArrayLengthAttributeViaEntityViaCreateParameters()
        {
            // Act
            using var connection = new DB2Connection();
            using var command = connection.CreateCommand();

            DbCommandExtension
                .CreateParameters(command, new ArrayLengthAttributeTestClass
                {
                    ColumnName = "Test"
                });

            // Assert
            Assert.AreEqual(1, command.Parameters.Count);

            // Assert
            var parameter = (DB2Parameter)command.Parameters[":ColumnName"];
            Assert.AreEqual(4000, parameter.ArrayLength);
        }

        [TestMethod]
        public void TestArrayLengthAttributeViaAnonymousViaCreateParameters()
        {
            // Act
            using var connection = new DB2Connection();
            using var command = connection.CreateCommand();

            DbCommandExtension
                .CreateParameters(command, new
                {
                    ColumnName = "Test"
                },
                typeof(ArrayLengthAttributeTestClass));

            // Assert
            Assert.AreEqual(1, command.Parameters.Count);

            // Assert
            var parameter = (DB2Parameter)command.Parameters[":ColumnName"];
            Assert.AreEqual(4000, parameter.ArrayLength);
        }
    }
}
