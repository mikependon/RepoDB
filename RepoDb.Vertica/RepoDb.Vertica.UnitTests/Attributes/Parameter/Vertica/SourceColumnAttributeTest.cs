using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vertica.Data.VerticaClient;
using RepoDb.Attributes.Parameter.Vertica;
using RepoDb.DbSettings;
using RepoDb.Extensions;

namespace RepoDb.Vertica.UnitTests.Attributes.Parameter.Vertica
{
    [TestClass]
    public class SourceColumnAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<VerticaConnection>(new VerticaDbSetting(), true);
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
            using var connection = new VerticaConnection();
            using var command = connection.CreateCommand();

            DbCommandExtension
                .CreateParameters(command, new SourceColumnAttributeTestClass
                {
                    ColumnName = "Test"
                });

            // Assert
            Assert.AreEqual(1, command.Parameters.Count);

            // Assert
            var parameter = (VerticaParameter)command.Parameters["@ColumnName"];
            Assert.AreEqual("MappedColumnName", parameter.SourceColumn);
        }

        [TestMethod]
        public void TestSourceColumnAttributeViaAnonymousViaCreateParameters()
        {
            // Act
            using var connection = new VerticaConnection();
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
            var parameter = (VerticaParameter)command.Parameters["@ColumnName"];
            Assert.AreEqual("MappedColumnName", parameter.SourceColumn);
        }
    }
}
