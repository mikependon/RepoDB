using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes.Parameter.Sqlite;
using RepoDb.DbSettings;
using RepoDb.Extensions;

namespace RepoDb.SqLite.UnitTests.Attributes.Parameter.Sqlite
{
    [TestClass]
    public class SqliteTypeAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<SqliteConnection>(new SqLiteDbSetting(), true);
        }

        #region Classes

        private class SqliteTypeAttributeTestClass
        {
            [SqliteType(SqliteType.Real)]
            public object ColumnName { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestSqliteTypeAttributeViaEntityViaCreateParameters()
        {
            // Act
            using (var connection = new SqliteConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    DbCommandExtension
                        .CreateParameters(command, new SqliteTypeAttributeTestClass
                        {
                            ColumnName = "Test"
                        });

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = command.Parameters["@ColumnName"];
                    Assert.AreEqual(SqliteType.Real, parameter.SqliteType);
                }
            }
        }

        [TestMethod]
        public void TestSqliteTypeAttributeViaAnonymousViaCreateParameters()
        {
            // Act
            using (var connection = new SqliteConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    DbCommandExtension
                        .CreateParameters(command, new
                        {
                            ColumnName = "Test"
                        },
                        typeof(SqliteTypeAttributeTestClass));

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = command.Parameters["@ColumnName"];
                    Assert.AreEqual(SqliteType.Real, parameter.SqliteType);
                }
            }
        }
    }
}
