using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes.Parameter.SqlServer;
using RepoDb.DbSettings;
using RepoDb.Extensions;

namespace RepoDb.SqlServer.UnitTests.Attributes.Parameter.SqlServer
{
    [TestClass]
    public class XmlSchemaCollectionDatabaseAttributeTest
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

        private class XmlSchemaCollectionDatabaseAttributeTestClass
        {
            [XmlSchemaCollectionDatabase("XmlSchemaCollectionDatabase")]
            public object ColumnName { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestXmlSchemaCollectionDatabaseAttributeViaEntityViaCreateParameters()
        {
            // Act
            using (var connection = new SqlConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    DbCommandExtension
                        .CreateParameters(command, new XmlSchemaCollectionDatabaseAttributeTestClass
                        {
                            ColumnName = "Test"
                        });

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = command.Parameters["@ColumnName"];
                    Assert.AreEqual("XmlSchemaCollectionDatabase", parameter.XmlSchemaCollectionDatabase);
                }
            }
        }

        [TestMethod]
        public void TestXmlSchemaCollectionDatabaseAttributeViaAnonymousViaCreateParameters()
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
                        typeof(XmlSchemaCollectionDatabaseAttributeTestClass));

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = command.Parameters["@ColumnName"];
                    Assert.AreEqual("XmlSchemaCollectionDatabase", parameter.XmlSchemaCollectionDatabase);
                }
            }
        }
    }
}
