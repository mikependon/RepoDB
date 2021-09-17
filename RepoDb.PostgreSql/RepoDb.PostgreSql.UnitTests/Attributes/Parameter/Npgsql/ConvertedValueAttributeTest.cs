using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Attributes.Parameter.Npgsql;
using RepoDb.DbSettings;
using RepoDb.Extensions;

namespace RepoDb.PostgreSql.UnitTests.Attributes.Parameter.Npgsql
{
    [TestClass]
    public class ConvertedValueAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<NpgsqlConnection>(new PostgreSqlDbSetting(), true);
        }

        #region Classes

        private class ConvertedValueAttributeTestClass
        {
            [ConvertedValue("NameColumn")]
            public object ColumnName { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestConvertedValueAttributeViaEntityViaCreateParameters()
        {
            // Act
            using (var connection = new NpgsqlConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    DbCommandExtension
                        .CreateParameters(command, new ConvertedValueAttributeTestClass
                        {
                            ColumnName = "Test"
                        });

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = command.Parameters["@ColumnName"];
                    Assert.AreEqual("NameColumn", parameter.ConvertedValue);
                }
            }
        }

        [TestMethod]
        public void TestConvertedValueAttributeViaAnonymousViaCreateParameters()
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
                        typeof(ConvertedValueAttributeTestClass));

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = command.Parameters["@ColumnName"];
                    Assert.AreEqual("NameColumn", parameter.ConvertedValue);
                }
            }
        }
    }
}
