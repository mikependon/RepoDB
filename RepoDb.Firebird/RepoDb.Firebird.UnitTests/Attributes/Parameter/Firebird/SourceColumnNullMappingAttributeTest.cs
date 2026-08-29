using Microsoft.VisualStudio.TestTools.UnitTesting;
using FirebirdSql.Data.FirebirdClient;
using RepoDb.Attributes.Parameter.Firebird;
using RepoDb.DbSettings;
using RepoDb.Extensions;

namespace RepoDb.Firebird.UnitTests.Attributes.Parameter.Firebird
{
    [TestClass]
    public class SourceColumnNullMappingAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<FbConnection>(new FirebirdDbSetting(), true);
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
            using var connection = new FbConnection();
            using var command = connection.CreateCommand();

            DbCommandExtension
                .CreateParameters(command, new SourceColumnNullMappingAttributeTestClass
                {
                    ColumnName = "Test"
                });

            // Assert
            Assert.AreEqual(1, command.Parameters.Count);

            // Assert
            var parameter = (FbParameter)command.Parameters["@ColumnName"];
            Assert.IsTrue(parameter.SourceColumnNullMapping);
        }

        [TestMethod]
        public void TestSourceColumnNullMappingAttributeViaAnonymousViaCreateParameters()
        {
            // Act
            using var connection = new FbConnection();
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
            var parameter = (FbParameter)command.Parameters["@ColumnName"];
            Assert.IsTrue(parameter.SourceColumnNullMapping);
        }
    }
}
