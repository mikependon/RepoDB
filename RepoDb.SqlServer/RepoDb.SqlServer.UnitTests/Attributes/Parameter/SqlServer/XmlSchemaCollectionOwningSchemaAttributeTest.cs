using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes.Parameter.SqlServer;
using RepoDb.DbSettings;
using RepoDb.Extensions;

namespace RepoDb.SqlServer.UnitTests.Attributes.Parameter.SqlServer
{
    [TestClass]
    public class XmlSchemaCollectionOwningSchemaAttributeTest
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

        private class XmlSchemaCollectionOwningSchemaAttributeTestClass
        {
            [XmlSchemaCollectionOwningSchema("XmlSchemaCollectionOwningSchema")]
            public object ColumnName { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestXmlSchemaCollectionOwningSchemaAttributeViaEntityViaCreateParameters()
        {
            // Act
            using (var connection = new SqlConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    DbCommandExtension
                        .CreateParameters(command, new XmlSchemaCollectionOwningSchemaAttributeTestClass
                        {
                            ColumnName = "Test"
                        });

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = command.Parameters["@ColumnName"];
                    Assert.AreEqual("XmlSchemaCollectionOwningSchema", parameter.XmlSchemaCollectionOwningSchema);
                }
            }
        }

        [TestMethod]
        public void TestXmlSchemaCollectionOwningSchemaAttributeViaAnonymousViaCreateParameters()
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
                        typeof(XmlSchemaCollectionOwningSchemaAttributeTestClass));

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = command.Parameters["@ColumnName"];
                    Assert.AreEqual("XmlSchemaCollectionOwningSchema", parameter.XmlSchemaCollectionOwningSchema);
                }
            }
        }
    }
}
