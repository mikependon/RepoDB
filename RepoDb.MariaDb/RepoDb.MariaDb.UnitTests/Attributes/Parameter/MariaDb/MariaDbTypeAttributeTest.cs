using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes.Parameter.MariaDb;
using RepoDb.Connector.MariaDb;
using RepoDb.DbSettings;
using RepoDb.Extensions;

namespace RepoDb.MariaDb.UnitTests.Attributes.Parameter.MariaDb
{
    [TestClass]
    public class MariaDbTypeAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<MariaDbConnection>(new MariaDbDbSetting(), true);
        }

        #region Classes

        private class MariaDbTypeAttributeTestClass
        {
            [MariaDbType(MariaDbType.Geometry)]
            public object ColumnName { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestMariaDbTypeAttributeViaEntityViaCreateParameters()
        {
            // Act
            using (var connection = new MariaDbConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    DbCommandExtension
                        .CreateParameters(command, new MariaDbTypeAttributeTestClass
                        {
                            ColumnName = "Test"
                        });

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = (MariaDbParameter)command.Parameters["@ColumnName"];
                    Assert.AreEqual(MariaDbType.Geometry, parameter.MariaDbType);
                }
            }
        }

        [TestMethod]
        public void TestMariaDbTypeAttributeViaAnonymousViaCreateParameters()
        {
            // Act
            using (var connection = new MariaDbConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    DbCommandExtension
                        .CreateParameters(command, new
                        {
                            ColumnName = "Test"
                        },
                        typeof(MariaDbTypeAttributeTestClass));

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = (MariaDbParameter)command.Parameters["@ColumnName"];
                    Assert.AreEqual(MariaDbType.Geometry, parameter.MariaDbType);
                }
            }
        }
    }
}
