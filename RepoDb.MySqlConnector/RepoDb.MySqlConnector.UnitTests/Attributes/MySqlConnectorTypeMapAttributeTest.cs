using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySqlConnector;
using RepoDb.Attributes;
using RepoDb.DbSettings;
using RepoDb.Extensions;

namespace RepoDb.MySqlConnector.UnitTests.Attributes
{
    [TestClass]
    public class MySqlTypeMapAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<MySqlConnection>(new MySqlConnectorDbSetting(), true);
        }

        #region Classes

        private class MySqlDbTypeAttributeTestClass
        {
            [MySqlConnectorTypeMap(MySqlDbType.Geometry)]
            public object ColumnName { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestMySqlConnectorTypeMapAttributeViaEntityViaCreateParameters()
        {
            // Act
            using (var connection = new MySqlConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    DbCommandExtension
                        .CreateParameters(command, new MySqlDbTypeAttributeTestClass
                        {
                            ColumnName = "Test"
                        });

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = command.Parameters["@ColumnName"];
                    Assert.AreEqual(MySqlDbType.Geometry, parameter.MySqlDbType);
                }
            }
        }

        [TestMethod]
        public void TestMySqlConnectorTypeMapAttributeViaAnonymousViaCreateParameters()
        {
            // Act
            using (var connection = new MySqlConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    DbCommandExtension
                        .CreateParameters(command, new
                        {
                            ColumnName = "Test"
                        },
                        typeof(MySqlDbTypeAttributeTestClass));

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = command.Parameters["@ColumnName"];
                    Assert.AreEqual(MySqlDbType.Geometry, parameter.MySqlDbType);
                }
            }
        }
    }
}
