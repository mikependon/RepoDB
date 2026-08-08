using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Data.Db2;
using RepoDb.Attributes.Parameter.Db2;
using RepoDb.DbSettings;
using RepoDb.Extensions;

namespace RepoDb.Db2.UnitTests.Attributes.Parameter.Db2
{
    [TestClass]
    public class SkipConversionToLocalTimeAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<Db2Connection>(new Db2DbSetting(), true);
        }

        #region Classes

        private class SkipConversionToLocalTimeAttributeTestClass
        {
            [SkipConversionToLocalTime(true)]
            public object ColumnName { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestSkipConversionToLocalTimeAttributeViaEntityViaCreateParameters()
        {
            // Act
            using var connection = new Db2Connection();
            using var command = connection.CreateCommand();

            DbCommandExtension
                .CreateParameters(command, new SkipConversionToLocalTimeAttributeTestClass
                {
                    ColumnName = "Test"
                });

            // Assert
            Assert.AreEqual(1, command.Parameters.Count);

            // Assert
            var parameter = (Db2Parameter)command.Parameters[":ColumnName"];
            Assert.IsTrue(parameter.SkipConversionToLocalTime);
        }

        [TestMethod]
        public void TestSkipConversionToLocalTimeAttributeViaAnonymousViaCreateParameters()
        {
            // Act
            using var connection = new Db2Connection();
            using var command = connection.CreateCommand();

            DbCommandExtension
                .CreateParameters(command, new
                {
                    ColumnName = "Test"
                },
                typeof(SkipConversionToLocalTimeAttributeTestClass));

            // Assert
            Assert.AreEqual(1, command.Parameters.Count);

            // Assert
            var parameter = (Db2Parameter)command.Parameters[":ColumnName"];
            Assert.IsTrue(parameter.SkipConversionToLocalTime);
        }
    }
}
