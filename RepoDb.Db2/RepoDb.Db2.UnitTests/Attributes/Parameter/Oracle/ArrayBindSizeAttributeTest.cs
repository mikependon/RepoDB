using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Data.Db2;
using RepoDb.Attributes.Parameter.Db2;
using RepoDb.DbSettings;
using RepoDb.Extensions;

namespace RepoDb.Db2.UnitTests.Attributes.Parameter.Db2
{
    [TestClass]
    public class ArrayBindSizeAttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<Db2Connection>(new Db2DbSetting(), true);
        }

        #region Classes

        private class ArrayBindSizeAttributeTestClass
        {
            [ArrayBindSize(new[] { 4000, 4000 })]
            public object ColumnName { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestArrayBindSizeAttributeViaEntityViaCreateParameters()
        {
            // Act
            using var connection = new Db2Connection();
            using var command = connection.CreateCommand();

            DbCommandExtension
                .CreateParameters(command, new ArrayBindSizeAttributeTestClass
                {
                    ColumnName = "Test"
                });

            // Assert
            Assert.AreEqual(1, command.Parameters.Count);

            // Assert
            var parameter = (Db2Parameter)command.Parameters[":ColumnName"];
            CollectionAssert.AreEqual(new[] { 4000, 4000 }, parameter.ArrayBindSize);
        }

        [TestMethod]
        public void TestArrayBindSizeAttributeViaAnonymousViaCreateParameters()
        {
            // Act
            using var connection = new Db2Connection();
            using var command = connection.CreateCommand();

            DbCommandExtension
                .CreateParameters(command, new
                {
                    ColumnName = "Test"
                },
                typeof(ArrayBindSizeAttributeTestClass));

            // Assert
            Assert.AreEqual(1, command.Parameters.Count);

            // Assert
            var parameter = (Db2Parameter)command.Parameters[":ColumnName"];
            CollectionAssert.AreEqual(new[] { 4000, 4000 }, parameter.ArrayBindSize);
        }
    }
}
