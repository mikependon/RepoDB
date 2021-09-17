using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes.Parameter.SqlServer;
using RepoDb.DbSettings;
using RepoDb.Extensions;

namespace RepoDb.SqlServer.UnitTests.Attributes.Parameter.SqlServer
{
    [TestClass]
    public class UdtTypeNameAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<SqlConnection>(new SqlServerDbSetting(), true);
        }

        #region Classes

        private class UdtTypeNameAttributeTestClass
        {
            [UdtTypeName("UdtTypeName")]
            public object ColumnName { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestUdtTypeNameAttributeViaEntityViaCreateParameters()
        {
            // Act
            using (var connection = new SqlConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    DbCommandExtension
                        .CreateParameters(command, new UdtTypeNameAttributeTestClass
                        {
                            ColumnName = "Test"
                        });

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = command.Parameters["@ColumnName"];
                    Assert.AreEqual("UdtTypeName", parameter.UdtTypeName);
                }
            }
        }

        [TestMethod]
        public void TestUdtTypeNameAttributeViaAnonymousViaCreateParameters()
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
                        typeof(UdtTypeNameAttributeTestClass));

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = command.Parameters["@ColumnName"];
                    Assert.AreEqual("UdtTypeName", parameter.UdtTypeName);
                }
            }
        }
    }
}
