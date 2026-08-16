using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using RepoDb.Attributes;
using RepoDb.Attributes.Parameter.MariaDb;
using RepoDb.DbSettings;
using RepoDb.Extensions;

namespace RepoDb.MariaDb.UnitTests.Attributes
{
    [TestClass]
    public class MariaDbTypeMapAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<MySqlConnection>(new MariaDbDbSetting(), true);
        }

        #region Classes

        private class MySqlDbTypeAttributeTestClass
        {
            [MySqlDbType(MySqlDbType.Geometry)]
            public object ColumnName { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestMariaDbTypeMapAttributeViaEntityViaCreateParameters()
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
        public void TestMariaDbTypeMapAttributeViaAnonymousViaCreateParameters()
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
