using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using NpgsqlTypes;
using RepoDb.Attributes;
using RepoDb.DbSettings;
using RepoDb.Extensions;

namespace RepoDb.PostgreSql.UnitTests.Attributes
{
    [TestClass]
    public class NpgsqlTypeMapAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<NpgsqlConnection>(new PostgreSqlDbSetting(), true);
        }

        #region Classes

        private class NpgsqlTypeMapAttributeTestClass
        {
            [NpgsqlTypeMap(NpgsqlDbType.Box)]
            public object ColumnName { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestNpgsqlTypeMapAttributeViaEntityViaCreateParameters()
        {
            // Act
            using (var connection = new NpgsqlConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    DbCommandExtension
                        .CreateParameters(command, new NpgsqlTypeMapAttributeTestClass
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
        public void TestNpgsqlTypeMapAttributeViaAnonymousViaCreateParameters()
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
                        typeof(NpgsqlTypeMapAttributeTestClass));

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
