using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes.Parameter;
using RepoDb.Extensions;
using RepoDb.UnitTests.CustomObjects;
using System.Data;

namespace RepoDb.UnitTests.Attributes.Parameter
{
    [TestClass]
    public class DbTypeAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<CustomDbConnection>(new CustomDbSetting(), true);
        }

        [TestCleanup]
        public void Cleanup()
        {
            DbSettingMapper.Clear();
        }

        #region Classes

        private class DbTypeAttributeTestClass
        {
            [DbType(DbType.AnsiStringFixedLength)]
            public object ColumnName { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestDbTypeAttributeViaEntityViaCreateParameters()
        {
            // Act
            using (var connection = new CustomDbConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    DbCommandExtension
                        .CreateParameters(command, new DbTypeAttributeTestClass
                        {
                            ColumnName = "Test"
                        });

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = command.Parameters["@ColumnName"];
                    Assert.AreEqual(DbType.AnsiStringFixedLength, ((CustomDbParameter)parameter).DbType);
                }
            }
        }

        [TestMethod]
        public void TestDbTypeAttributeViaAnonymousViaCreateParameters()
        {
            // Act
            using (var connection = new CustomDbConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    DbCommandExtension
                        .CreateParameters(command, new
                        {
                            ColumnName = "Test"
                        },
                        typeof(DbTypeAttributeTestClass));

                    // Assert
                    Assert.AreEqual(1, command.Parameters.Count);

                    // Assert
                    var parameter = command.Parameters["@ColumnName"];
                    Assert.AreEqual(DbType.AnsiStringFixedLength, ((CustomDbParameter)parameter).DbType);
                }
            }
        }
    }
}
