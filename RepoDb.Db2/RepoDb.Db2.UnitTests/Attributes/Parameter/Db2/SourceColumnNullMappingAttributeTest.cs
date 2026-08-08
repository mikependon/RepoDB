using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Data.Db2;
using RepoDb.Attributes.Parameter.Db2;
using RepoDb.DbSettings;
using RepoDb.Extensions;

namespace RepoDb.Db2.UnitTests.Attributes.Parameter.Db2
{
    [TestClass]
    public class SourceColumnNullMappingAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<DB2Connection>(new Db2DbSetting(), true);
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
            using var connection = new DB2Connection();
            using var command = connection.CreateCommand();

            DbCommandExtension
                .CreateParameters(command, new SourceColumnNullMappingAttributeTestClass
                {
                    ColumnName = "Test"
                });

            // Assert
            Assert.AreEqual(1, command.Parameters.Count);

            // Assert
            var parameter = (DB2Parameter)command.Parameters[":ColumnName"];
            Assert.IsTrue(parameter.SourceColumnNullMapping);
        }

        [TestMethod]
        public void TestSourceColumnNullMappingAttributeViaAnonymousViaCreateParameters()
        {
            // Act
            using var connection = new DB2Connection();
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
            var parameter = (DB2Parameter)command.Parameters[":ColumnName"];
            Assert.IsTrue(parameter.SourceColumnNullMapping);
        }
    }
}
