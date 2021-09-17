using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes.Parameter.SqlServer;
using RepoDb.DbSettings;
using RepoDb.Extensions;

namespace RepoDb.SqlServer.UnitTests.Attributes.Parameter.SqlServer
{
    [TestClass]
    public class LocaleIdAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<SqlConnection>(new SqlServerDbSetting(), true);
        }

        #region Classes

        private class LocaleIdAttributeTestClass
        {
            [LocaleId(1000)]
            public object ColumnName { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestLocaleIdAttributeViaEntityViaCreateParameters()
        {
            // Act
            using (var connection = new SqlConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    DbCommandExtension
                        .CreateParameters(command, new LocaleIdAttributeTestClass
                        {
                            ColumnName = "Test"
                        });

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = command.Parameters["@ColumnName"];
                    Assert.AreEqual(1000, parameter.LocaleId);
                }
            }
        }

        [TestMethod]
        public void TestLocaleIdAttributeViaAnonymousViaCreateParameters()
        {
            // Act
            using (var connection = new SqlConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    DbCommandExtension
                        .CreateParameters(command, new
                        {
                            ColumnName = "Test"
                        },
                        typeof(LocaleIdAttributeTestClass));

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = command.Parameters["@ColumnName"];
                    Assert.AreEqual(1000, parameter.LocaleId);
                }
            }
        }
    }
}
