using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Attributes.Parameter.Npgsql;
using RepoDb.DbSettings;
using RepoDb.Extensions;

namespace RepoDb.PostgreSql.UnitTests.Attributes.Parameter.Npgsql
{
    [TestClass]
    public class DataTypeNameAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<NpgsqlConnection>(new PostgreSqlDbSetting(), true);
        }

        [TestCleanup]
        public void Cleanup()
        {
            DbSettingMapper.Clear();
        }

        #region Classes

        private class DataTypeNameAttributeTestClass
        {
            [DataTypeName("DataTypeName")]
            public object ColumnName { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestDataTypeNameAttributeViaEntityViaCreateParameters()
        {
            // Act
            using (var connection = new NpgsqlConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    DbCommandExtension
                        .CreateParameters(command, new DataTypeNameAttributeTestClass
                        {
                            ColumnName = "Test"
                        });

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = command.Parameters["@ColumnName"];
                    Assert.AreEqual("DataTypeName", parameter.DataTypeName);
                }
            }
        }

        [TestMethod]
        public void TestDataTypeNameAttributeViaAnonymousViaCreateParameters()
        {
            // Act
            using (var connection = new NpgsqlConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    DbCommandExtension
                        .CreateParameters(command, new
                        {
                            ColumnName = "Test"
                        },
                        typeof(DataTypeNameAttributeTestClass));

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = command.Parameters["@ColumnName"];
                    Assert.AreEqual("DataTypeName", parameter.DataTypeName);
                }
            }
        }
    }
}
