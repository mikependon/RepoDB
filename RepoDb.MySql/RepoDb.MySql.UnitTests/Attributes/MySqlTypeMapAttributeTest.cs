using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using RepoDb.Attributes;
using RepoDb.Attributes.Parameter.MySql;
using RepoDb.DbSettings;
using RepoDb.Extensions;

namespace RepoDb.MySql.UnitTests.Attributes
{
    [TestClass]
    public class MySqlTypeMapAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<MySqlConnection>(new MySqlDbSetting(), true);
        }

        #region Classes

        private class MySqlDbTypeAttributeTestClass
        {
            [MySqlDbType(MySqlDbType.Geometry)]
            public object ColumnName { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestMySqlTypeMapAttributeViaEntityViaCreateParameters()
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
        public void TestMySqlTypeMapAttributeViaAnonymousViaCreateParameters()
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
