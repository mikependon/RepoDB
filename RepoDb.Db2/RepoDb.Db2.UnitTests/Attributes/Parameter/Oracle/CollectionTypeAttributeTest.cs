using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Data.Db2;
using RepoDb.Attributes.Parameter.Db2;
using RepoDb.DbSettings;
using RepoDb.Extensions;

namespace RepoDb.Db2.UnitTests.Attributes.Parameter.Db2
{
    [TestClass]
    public class CollectionTypeAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<Db2Connection>(new Db2DbSetting(), true);
        }

        #region Classes

        private class CollectionTypeAttributeTestClass
        {
            [CollectionType(Db2CollectionType.PLSQLAssociativeArray)]
            public object ColumnName { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestCollectionTypeAttributeViaEntityViaCreateParameters()
        {
            // Act
            using var connection = new Db2Connection();
            using var command = connection.CreateCommand();

            DbCommandExtension
                .CreateParameters(command, new CollectionTypeAttributeTestClass
                {
                    ColumnName = "Test"
                });

            // Assert
            Assert.AreEqual(1, command.Parameters.Count);

            // Assert
            var parameter = (Db2Parameter)command.Parameters[":ColumnName"];
            Assert.AreEqual(Db2CollectionType.PLSQLAssociativeArray, parameter.CollectionType);
        }

        [TestMethod]
        public void TestCollectionTypeAttributeViaAnonymousViaCreateParameters()
        {
            // Act
            using var connection = new Db2Connection();
            using var command = connection.CreateCommand();

            DbCommandExtension
                .CreateParameters(command, new
                {
                    ColumnName = "Test"
                },
                typeof(CollectionTypeAttributeTestClass));

            // Assert
            Assert.AreEqual(1, command.Parameters.Count);

            // Assert
            var parameter = (Db2Parameter)command.Parameters[":ColumnName"];
            Assert.AreEqual(Db2CollectionType.PLSQLAssociativeArray, parameter.CollectionType);
        }
    }
}
