using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes.Parameter.SqlServer;
using RepoDb.DbSettings;
using RepoDb.Extensions;
using System.Data;

namespace RepoDb.SqlServer.UnitTests.Attributes.Parameter.SqlServer
{
    [TestClass]
    public class SqlDbTypeAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<SqlConnection>(new SqlServerDbSetting(), true);
        }

        [TestCleanup]
        public void Cleanup()
        {
            DbSettingMapper.Clear();
        }

        #region Classes

        private class SqlDbTypeAttributeTestClass
        {
            [SqlDbType(SqlDbType.DateTime2)]
            public object ColumnName { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestSqlDbTypeAttributeViaEntityViaCreateParameters()
        {
            // Act
            using (var connection = new SqlConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    DbCommandExtension
                        .CreateParameters(command, new SqlDbTypeAttributeTestClass
                        {
                            ColumnName = "Test"
                        });

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = command.Parameters["@ColumnName"];
                    Assert.AreEqual(SqlDbType.DateTime2, parameter.SqlDbType);
                }
            }
        }

        [TestMethod]
        public void TestSqlDbTypeAttributeViaAnonymousViaCreateParameters()
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
                        typeof(SqlDbTypeAttributeTestClass));

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = command.Parameters["@ColumnName"];
                    Assert.AreEqual(SqlDbType.DateTime2, parameter.SqlDbType);
                }
            }
        }
    }
}
