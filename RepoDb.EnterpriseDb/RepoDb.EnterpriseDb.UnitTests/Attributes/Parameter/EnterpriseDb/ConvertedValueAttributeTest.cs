using Microsoft.VisualStudio.TestTools.UnitTesting;
using EnterpriseDB.EDBClient;
using RepoDb.Attributes.Parameter.EnterpriseDb;
using RepoDb.DbSettings;
using RepoDb.Extensions;

namespace RepoDb.EnterpriseDb.UnitTests.Attributes.Parameter.EnterpriseDb
{
    [TestClass]
    public class ConvertedValueAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<EDBConnection>(new EnterpriseDbDbSetting(), true);
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
            using (var connection = new EDBConnection())
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
                    Assert.AreEqual("NameColumn", parameter.Value);
                }
            }
        }

        [TestMethod]
        public void TestConvertedValueAttributeViaAnonymousViaCreateParameters()
        {
            // Act
            using (var connection = new EDBConnection())
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
                    Assert.AreEqual("NameColumn", parameter.Value);
                }
            }
        }
    }
}
