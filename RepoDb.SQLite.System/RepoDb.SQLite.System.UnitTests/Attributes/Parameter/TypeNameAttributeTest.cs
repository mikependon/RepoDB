using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes.Parameter.SQLite;
using RepoDb.DbSettings;
using RepoDb.Extensions;
using System.Data.SQLite;

namespace RepoDb.SQLite.System.UnitTests.Attributes.Parameter.SQLite
{
    [TestClass]
    public class TypeNameAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<SQLiteConnection>(new SqLiteDbSetting(), true);
        }

        #region Classes

        private class TypeNameAttributeTestClass
        {
            [TypeName("TypeName")]
            public object ColumnName { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestTypeNameAttributeViaEntityViaCreateParameters()
        {
            // Act
            using (var connection = new SQLiteConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    DbCommandExtension
                        .CreateParameters(command, new TypeNameAttributeTestClass
                        {
                            ColumnName = "Test"
                        });

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = command.Parameters["@ColumnName"];
                    Assert.AreEqual("TypeName", parameter.TypeName);
                }
            }
        }

        [TestMethod]
        public void TestTypeNameAttributeViaAnonymousViaCreateParameters()
        {
            // Act
            using (var connection = new SQLiteConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    DbCommandExtension
                        .CreateParameters(command, new
                        {
                            ColumnName = "Test"
                        },
                        typeof(TypeNameAttributeTestClass));

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = command.Parameters["@ColumnName"];
                    Assert.AreEqual("TypeName", parameter.TypeName);
                }
            }
        }
    }
}
