using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySqlConnector;
using RepoDb.Attributes.Parameter.MySqlConnector;
using RepoDb.DbSettings;
using RepoDb.Extensions;

namespace RepoDb.MySqlConnector.UnitTests.Attributes.Parameter.MySqlConnector
{
    [TestClass]
    public class MySqlDbTypeAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<MySqlConnection>(new MySqlConnectorDbSetting(), true);
        }

        [TestCleanup]
        public void Cleanup()
        {
            DbSettingMapper.Clear();
        }

        #region Classes

        private class MySqlDbTypeAttributeTestClass
        {
            [MySqlDbType(MySqlDbType.Geometry)]
            public object ColumnName { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestMySqlDbTypeAttributeViaEntityViaCreateParameters()
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
        public void TestMySqlDbTypeAttributeViaAnonymousViaCreateParameters()
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
