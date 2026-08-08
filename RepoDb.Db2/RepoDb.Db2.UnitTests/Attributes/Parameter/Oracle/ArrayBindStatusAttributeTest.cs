using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Data.Db2;
using RepoDb.Attributes.Parameter.Db2;
using RepoDb.DbSettings;
using RepoDb.Extensions;

namespace RepoDb.Db2.UnitTests.Attributes.Parameter.Db2
{
    [TestClass]
    public class ArrayBindStatusAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<Db2Connection>(new Db2DbSetting(), true);
        }

        #region Classes

        private class ArrayBindStatusAttributeTestClass
        {
            [ArrayBindStatus(new[] { Db2ParameterStatus.Success, Db2ParameterStatus.NullInsert })]
            public object ColumnName { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestArrayBindStatusAttributeViaEntityViaCreateParameters()
        {
            // Act
            using var connection = new Db2Connection();
            using var command = connection.CreateCommand();

            DbCommandExtension
                .CreateParameters(command, new ArrayBindStatusAttributeTestClass
                {
                    ColumnName = "Test"
                });

            // Assert
            Assert.AreEqual(1, command.Parameters.Count);

            // Assert
            var parameter = (Db2Parameter)command.Parameters[":ColumnName"];
            CollectionAssert.AreEqual(new[] { Db2ParameterStatus.Success, Db2ParameterStatus.NullInsert }, parameter.ArrayBindStatus);
        }

        [TestMethod]
        public void TestArrayBindStatusAttributeViaAnonymousViaCreateParameters()
        {
            // Act
            using var connection = new Db2Connection();
            using var command = connection.CreateCommand();

            DbCommandExtension
                .CreateParameters(command, new
                {
                    ColumnName = "Test"
                },
                typeof(ArrayBindStatusAttributeTestClass));

            // Assert
            Assert.AreEqual(1, command.Parameters.Count);

            // Assert
            var parameter = (Db2Parameter)command.Parameters[":ColumnName"];
            CollectionAssert.AreEqual(new[] { Db2ParameterStatus.Success, Db2ParameterStatus.NullInsert }, parameter.ArrayBindStatus);
        }
    }
}
