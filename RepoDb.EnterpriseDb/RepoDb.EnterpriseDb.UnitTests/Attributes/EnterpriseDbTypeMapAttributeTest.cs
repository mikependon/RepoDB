using Microsoft.VisualStudio.TestTools.UnitTesting;
using EnterpriseDB.EDBClient;
using EDBTypes;
using RepoDb.Attributes.Parameter.EnterpriseDb;
using RepoDb.DbSettings;
using RepoDb.Extensions;

namespace RepoDb.EnterpriseDb.UnitTests.Attributes
{
    [TestClass]
    public class EnterpriseDbTypeMapAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<EDBConnection>(new EnterpriseDbDbSetting(), true);
        }

        #region Classes

        private class EnterpriseDbTypeMapAttributeTestClass
        {
            [EnterpriseDbType(EDBDbType.Box)]
            public object ColumnName { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestEnterpriseDbTypeMapAttributeViaEntityViaCreateParameters()
        {
            // Act
            using (var connection = new EDBConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    DbCommandExtension
                        .CreateParameters(command, new EnterpriseDbTypeMapAttributeTestClass
                        {
                            ColumnName = "Test"
                        });

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = command.Parameters["@ColumnName"];
                    Assert.AreEqual(EDBDbType.Box, parameter.EDBDbType);
                }
            }
        }

        [TestMethod]
        public void TestEnterpriseDbTypeMapAttributeViaAnonymousViaCreateParameters()
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
                        typeof(EnterpriseDbTypeMapAttributeTestClass));

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = command.Parameters["@ColumnName"];
                    Assert.AreEqual(EDBDbType.Box, parameter.EDBDbType);
                }
            }
        }
    }
}
