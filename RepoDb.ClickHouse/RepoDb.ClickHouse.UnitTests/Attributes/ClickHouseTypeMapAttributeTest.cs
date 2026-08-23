using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Attributes.Parameter.ClickHouse;
using ClickHouse.Driver.ADO;
using ClickHouse.Driver.ADO.Parameters;
using RepoDb.DbSettings;
using RepoDb.Extensions;

namespace RepoDb.ClickHouse.UnitTests.Attributes
{
    [TestClass]
    public class ClickHouseTypeMapAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<ClickHouseConnection>(new ClickHouseDbSetting(), true);
        }

        #region Classes

        private class ClickHouseTypeMapAttributeTestClass
        {
            [ClickHouseType("UUID")]
            public object ColumnName { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestClickHouseTypeMapAttributeViaEntityViaCreateParameters()
        {
            // Act
            using (var connection = new ClickHouseConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    DbCommandExtension
                        .CreateParameters(command, new ClickHouseTypeMapAttributeTestClass
                        {
                            ColumnName = "Test"
                        });

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert - bare "ColumnName", not "@ColumnName": ClickHouseDbSetting.ParameterPrefix is
                    // string.Empty, so the real DbParameter.ParameterName carries no prefix.
                    var parameter = (ClickHouseDbParameter)command.Parameters["ColumnName"];
                    Assert.AreEqual("UUID", parameter.ClickHouseType);
                }
            }
        }

        [TestMethod]
        public void TestClickHouseTypeMapAttributeViaAnonymousViaCreateParameters()
        {
            // Act
            using (var connection = new ClickHouseConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    DbCommandExtension
                        .CreateParameters(command, new
                        {
                            ColumnName = "Test"
                        },
                        typeof(ClickHouseTypeMapAttributeTestClass));

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = (ClickHouseDbParameter)command.Parameters["ColumnName"];
                    Assert.AreEqual("UUID", parameter.ClickHouseType);
                }
            }
        }
    }
}
