using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using NpgsqlTypes;
using RepoDb.Attributes.Parameter.Npgsql;
using RepoDb.DbSettings;
using RepoDb.Extensions;

namespace RepoDb.PostgreSql.UnitTests.Attributes.Parameter.Npgsql
{
    [TestClass]
    public class NpgsqlDbTypeAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<NpgsqlConnection>(new PostgreSqlDbSetting(), true);
        }

        #region Classes

        private class NpgsqlDbTypeAttributeTestClass
        {
            [NpgsqlDbType(NpgsqlDbType.Box)]
            public object ColumnName { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestNpgsqlDbTypeAttributeViaEntityViaCreateParameters()
        {
            // Act
            using (var connection = new NpgsqlConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    DbCommandExtension
                        .CreateParameters(command, new NpgsqlDbTypeAttributeTestClass
                        {
                            ColumnName = "Test"
                        });

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = command.Parameters["@ColumnName"];
                    Assert.AreEqual(NpgsqlDbType.Box, parameter.NpgsqlDbType);
                }
            }
        }

        [TestMethod]
        public void TestNpgsqlDbTypeAttributeViaAnonymousViaCreateParameters()
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
                        typeof(NpgsqlDbTypeAttributeTestClass));

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = command.Parameters["@ColumnName"];
                    Assert.AreEqual(NpgsqlDbType.Box, parameter.NpgsqlDbType);
                }
            }
        }
    }
}
