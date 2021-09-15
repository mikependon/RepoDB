using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes.Parameter.SqlServer;
using RepoDb.DbSettings;
using RepoDb.Extensions;

namespace RepoDb.SqlServer.UnitTests.Attributes.Parameter.SqlServer
{
    [TestClass]
    public class OffsetAttributeTest
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

        private class OffsetAttributeTestClass
        {
            [Offset(1000)]
            public object ColumnName { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestOffsetAttributeViaEntityViaCreateParameters()
        {
            // Act
            using (var connection = new SqlConnection("Server=.;"))
            {
                using (var command = connection.CreateCommand())
                {
                    DbCommandExtension
                        .CreateParameters(command, new OffsetAttributeTestClass
                        {
                            ColumnName = "Test"
                        });

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = command.Parameters["@ColumnName"];
                    Assert.AreEqual(1000, parameter.Offset);
                }
            }
        }

        [TestMethod]
        public void TestOffsetAttributeViaAnonymousViaCreateParameters()
        {
            // Act
            using (var connection = new SqlConnection("Server=.;"))
            {
                using (var command = connection.CreateCommand())
                {
                    DbCommandExtension
                        .CreateParameters(command, new
                        {
                            ColumnName = "Test"
                        },
                        typeof(OffsetAttributeTestClass));

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = command.Parameters["@ColumnName"];
                    Assert.AreEqual(1000, parameter.Offset);
                }
            }
        }
    }
}
