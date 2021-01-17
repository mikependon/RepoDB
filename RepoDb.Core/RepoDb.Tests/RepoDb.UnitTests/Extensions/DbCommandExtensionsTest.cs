using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb.UnitTests.Extensions
{
    [TestClass]
    public class DbCommandExtensionsTest
    {
        [TestCleanup]
        public void Cleanup()
        {
            PropertyHandlerMapper.Clear();
            PropertyHandlerCache.Flush();
        }

        #region SubClasses

        private class PropertyHandlerConnection : CustomDbConnection { }

        #endregion

        #region SubClasses

        private class TestClass
        {
            public Guid Id { get; set; } = Guid.NewGuid();
        }

        #endregion

        #region PropertyHandlers

        private class StringToGuidPropertyHandler : IPropertyHandler<string, Guid>
        {
            public Guid Get(string input, ClassProperty property)
            {
                Guid.TryParse(input, out Guid output);
                return output;
            }

            public string Set(Guid input, ClassProperty property)
            {
                return input.ToString();
            }
        }

        #endregion

#region PropertyHandlerTests

#region PropertyLevel

        [TestMethod]
        public void TestDbCommandCreateParametersPropertyHandlerPropertyLevelInvocationViaDictionary()
        {
            // Arrange
            var param = new Dictionary<string, object> { { "Id", "9963c864-ab4f-43f8-9dc9-43038565b971" } };
            FluentMapper
                .Entity<TestClass>()
                .PropertyHandler<StringToGuidPropertyHandler>(e => e.Id);

            // Act
            using (var connection = new PropertyHandlerConnection())
            {
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CreateParameters(param);

                    // Assert
                    Assert.AreEqual(1, cmd.Parameters.Count);
                    Assert.AreEqual("9963c864-ab4f-43f8-9dc9-43038565b971", cmd.Parameters[0].Value);
                }
            }
        }

        [TestMethod]
        public void TestDbCommandCreateParametersPropertyHandlerPropertyLevelInvocationViaClass()
        {
            // Arrange
            var param = new TestClass { Id = Guid.Parse("9963c864-ab4f-43f8-9dc9-43038565b971") };
            FluentMapper
                .Entity<TestClass>()
                .PropertyHandler<StringToGuidPropertyHandler>(e => e.Id);

            // Act
            using (var connection = new PropertyHandlerConnection())
            {
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CreateParameters(param);

                    // Assert
                    Assert.AreEqual(1, cmd.Parameters.Count);
                    Assert.AreEqual("9963c864-ab4f-43f8-9dc9-43038565b971", cmd.Parameters[0].Value);
                }
            }
        }

#endregion

#region Type Level

        [TestMethod]
        public void TestDbCommandCreateParametersPropertyHandlerTypeLevelInvocationViaDynamic()
        {
            // Arrange
            var param = new { Id = Guid.Parse("9963c864-ab4f-43f8-9dc9-43038565b971") };
            FluentMapper
                .Type<Guid>()
                .PropertyHandler<StringToGuidPropertyHandler>();

            // Act
            using (var connection = new PropertyHandlerConnection())
            {
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CreateParameters(param);

                    // Assert
                    Assert.AreEqual(1, cmd.Parameters.Count);
                    Assert.AreEqual("9963c864-ab4f-43f8-9dc9-43038565b971", cmd.Parameters[0].Value);
                }
            }
        }

        [TestMethod]
        public void TestDbCommandCreateParametersPropertyHandlerTypeLevelInvocationViaQueryField()
        {
            // Arrange
            var param = new QueryField("Id", Guid.Parse("9963c864-ab4f-43f8-9dc9-43038565b971"));
            FluentMapper
                .Type<Guid>()
                .PropertyHandler<StringToGuidPropertyHandler>();

            // Act
            using (var connection = new PropertyHandlerConnection())
            {
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CreateParameters(param);

                    // Assert
                    Assert.AreEqual(1, cmd.Parameters.Count);
                    Assert.AreEqual("9963c864-ab4f-43f8-9dc9-43038565b971", cmd.Parameters[0].Value);
                }
            }
        }

        [TestMethod]
        public void TestDbCommandCreateParametersPropertyHandlerTypeLevelInvocationViaQueryFields()
        {
            // Arrange
            var param = new QueryField("Id", Guid.Parse("9963c864-ab4f-43f8-9dc9-43038565b971")).AsEnumerable();
            FluentMapper
                .Type<Guid>()
                .PropertyHandler<StringToGuidPropertyHandler>();

            // Act
            using (var connection = new PropertyHandlerConnection())
            {
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CreateParameters(param);

                    // Assert
                    Assert.AreEqual(1, cmd.Parameters.Count);
                    Assert.AreEqual("9963c864-ab4f-43f8-9dc9-43038565b971", cmd.Parameters[0].Value);
                }
            }
        }

        [TestMethod]
        public void TestDbCommandCreateParametersPropertyHandlerTypeLevelInvocationViaQueryGroup()
        {
            // Arrange
            var param = new QueryGroup(new QueryField("Id", Guid.Parse("9963c864-ab4f-43f8-9dc9-43038565b971")));
            FluentMapper
                .Type<Guid>()
                .PropertyHandler<StringToGuidPropertyHandler>();

            // Act
            using (var connection = new PropertyHandlerConnection())
            {
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CreateParameters(param);

                    // Assert
                    Assert.AreEqual(1, cmd.Parameters.Count);
                    Assert.AreEqual("9963c864-ab4f-43f8-9dc9-43038565b971", cmd.Parameters[0].Value);
                }
            }
        }

#endregion

#endregion

        [TestMethod]
        public void TestIsPlainTypeForAnoynmousType()
        {
            var type = (new { Property = "ABC" }).GetType();
            Assert.IsTrue(type.IsPlainType());
            Assert.IsTrue(type.IsAnonymousType());
            Assert.IsFalse(type.IsQueryObjectType());
            Assert.IsFalse(type.IsDictionaryStringObject());
            Assert.IsFalse(type.GetEnumerableClassProperties().Any());
        }

        private class PrivateDbConnection : CustomDbConnection { }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryViaDynamicsWithEmptyArrayParameters()
        {
            DbSettingMapper.Add(typeof(PrivateDbConnection), new CustomDbSetting(), true);
            using (var connection = new PrivateDbConnection())
            {
                var sql = @"
select * from someTable
where id in (@normalArray)
  and id in (@emptyArray)
  and id in (@nullArray)
  and id in (@concat1ArrayA, @concat1ArrayB)
  and id in (@concat2ArrayA, @concat2ArrayB)
  and id in (@concat3ArrayA, @concat3ArrayB)";
                var param = new
                {
                    normalArray = new[] { 5, 6 },
                    emptyArray = Array.Empty<int>(),
                    nullArray = (IEnumerable<int>)null,
                    concat1ArrayA = new[] { 100, 101 }, concat1ArrayB = new[] { 102, 103 },
                    concat2ArrayA = Array.Empty<int>(), concat2ArrayB = new[] { 200, 201 },
                    concat3ArrayA = Array.Empty<int>(), concat3ArrayB = Array.Empty<int>()
                };
                var command = connection.CreateDbCommandForExecution(sql, param, skipCommandArrayParametersCheck: false);

                var expectedSql = @"
select * from someTable
where id in (@normalArray0, @normalArray1)
  and id in ((SELECT @emptyArray WHERE 1 = 0))
  and id in (@nullArray)
  and id in (@concat1ArrayA0, @concat1ArrayA1, @concat1ArrayB0, @concat1ArrayB1)
  and id in ((SELECT @concat2ArrayA WHERE 1 = 0), @concat2ArrayB0, @concat2ArrayB1)
  and id in ((SELECT @concat3ArrayA WHERE 1 = 0), (SELECT @concat3ArrayB WHERE 1 = 0))";
                Assert.AreEqual(expectedSql, command.CommandText);
                Assert.AreEqual(13, command.Parameters.Count);
                Assert.AreEqual(5, command.Parameters["@normalArray0"].Value);
                Assert.AreEqual(6, command.Parameters["@normalArray1"].Value);
                Assert.AreEqual(DBNull.Value, command.Parameters["@emptyArray"].Value);
                Assert.AreEqual(DBNull.Value, command.Parameters["@nullArray"].Value);
                Assert.AreEqual(100, command.Parameters["@concat1ArrayA0"].Value);
                Assert.AreEqual(101, command.Parameters["@concat1ArrayA1"].Value);
                Assert.AreEqual(102, command.Parameters["@concat1ArrayB0"].Value);
                Assert.AreEqual(103, command.Parameters["@concat1ArrayB1"].Value);
                Assert.AreEqual(DBNull.Value, command.Parameters["@concat2ArrayA"].Value);
                Assert.AreEqual(200, command.Parameters["@concat2ArrayB0"].Value);
                Assert.AreEqual(201, command.Parameters["@concat2ArrayB1"].Value);
                Assert.AreEqual(DBNull.Value, command.Parameters["@concat3ArrayA"].Value);
                Assert.AreEqual(DBNull.Value, command.Parameters["@concat3ArrayB"].Value);
            }
        }
    }
}
