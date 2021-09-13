using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Extensions;
using RepoDb.UnitTests.CustomObjects;
using System;
using System.Data;
using System.Linq;

namespace RepoDb.UnitTests.Attributes
{
    [TestClass]
    public class TypeMapAttributeTest
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

        private class TestTypeMapAttributeUnquotedNameClass
        {
            [TypeMap(DbType.StringFixedLength)]
            public string ColumnString { get; set; }
        }

        #endregion

        private class TypeMapAttributeTestClass
        {
            public int Id { get; set; }
            public string Name { get; set; }
            [TypeMap(DbType.DateTime2)]
            public DateTime ColumnDateTime { get; set; }
        }

        [TestMethod]
        public void TestTypeMapAttribute()
        {
            // Act
            var actual = PropertyCache.Get<TypeMapAttributeTestClass>()
                .First(p => p.PropertyInfo.Name == "ColumnDateTime");
            var result = actual.GetDbType();
            var expected = DbType.DateTime2;

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestTypeMapAttributeUnquotedNameViaEntityViaCreateParameters()
        {
            // Act
            using (var connection = new CustomDbConnection())
            {
                var command = connection.CreateCommand();
                DbCommandExtension
                    .CreateParameters(command, new TestTypeMapAttributeUnquotedNameClass
                    {
                        ColumnString = "ColumnStringValue"
                    });

                // Assert
                Assert.AreEqual(1, command.Parameters.Count);

                // Assert
                var parameter = command.Parameters["@ColumnString"];
                Assert.AreEqual(DbType.StringFixedLength, parameter.DbType);
            }
        }

        [TestMethod]
        public void TestTypeMapAttributeUnquotedNameViaAnonymousViaCreateParameters()
        {
            // Act
            using (var connection = new CustomDbConnection())
            {
                var command = connection.CreateCommand();
                DbCommandExtension
                    .CreateParameters(command, new
                    {
                        ColumnString = "ColumnStringValue"
                    },
                    typeof(TestTypeMapAttributeUnquotedNameClass));

                // Assert
                Assert.AreEqual(1, command.Parameters.Count);

                // Assert
                var parameter = command.Parameters["@ColumnString"];
                Assert.AreEqual(DbType.StringFixedLength, parameter.DbType);
            }
        }

    }
}
