using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;

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
    }
}
