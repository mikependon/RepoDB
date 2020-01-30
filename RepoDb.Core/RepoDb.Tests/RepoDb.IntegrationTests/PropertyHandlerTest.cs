using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace RepoDb.IntegrationTests
{
    [TestClass]
    public class PropertyHandlerTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Database.Cleanup();
        }

        #region Classes

        private class TargetModel
        {
            public string Value { get; set; }
        }

        [Map("[dbo].[NonIdentityTable]")]
        private class EntityModelForClass
        {
            public Guid Id { get; set; }

            [Map("ColumnNVarChar")]
            [PropertyHandler(typeof(PropertyToClassHandler))]
            public TargetModel Model { get; set; }
        }

        [Map("[dbo].[NonIdentityTable]")]
        private class EntityModelForIntToStringType
        {
            public Guid Id { get; set; }

            [Map("ColumnInt")]
            [PropertyHandler(typeof(IntToStringTypeHandler))]
            public string IntAsString { get; set; }
        }

        #endregion

        #region Handlers

        private class PropertyToClassHandler : IPropertyHandler<string, TargetModel>
        {
            public TargetModel Get(string input)
            {
                return new TargetModel { Value = input };
            }

            public string Set(TargetModel input)
            {
                return input?.Value;
            }
        }

        public class IntToStringTypeHandler : IPropertyHandler<int?, string>
        {
            public string Get(int? input)
            {
                return Convert.ToString(input);
            }

            public int? Set(string input)
            {
                return Convert.ToInt32(input);
            }
        }

        #endregion

        #region Helpers

        private IEnumerable<EntityModelForClass> CreateEntityModelForClasses(int count, bool isModelNull = false)
        {
            for (var i = 0; i < count; i++)
            {
                yield return new EntityModelForClass
                {
                    Id = Guid.NewGuid(),
                    Model = isModelNull ? null : new TargetModel
                    {
                        Value = $"Address{i}-{Guid.NewGuid()}"
                    }
                };
            }
        }

        private IEnumerable<EntityModelForIntToStringType> CreateEntityModelForIntToStringTypes(int count, bool isIntNull = false)
        {
            for (var i = 0; i < count; i++)
            {
                yield return new EntityModelForIntToStringType
                {
                    Id = Guid.NewGuid(),
                    IntAsString = isIntNull ? null : Convert.ToString(new Random().Next(int.MaxValue))
                };
            }
        }

        #endregion

        #region PropertyToClass

        [TestMethod]
        public void TestPropertyHandlerWithPropertyToClassHandler()
        {
            // Setup
            var models = CreateEntityModelForClasses(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(models);

                // Act
                var result = connection.QueryAll<EntityModelForClass>();

                // Assert
                models.AsList().ForEach(e =>
                {
                    var item = result.First(obj => obj.Id == e.Id);
                    Assert.AreEqual(e.Model.Value, item.Model.Value);
                });
            }
        }

        [TestMethod]
        public void TestPropertyHandlerWithPropertyToClassHandlerAsNull()
        {
            // Setup
            var models = CreateEntityModelForClasses(10, true).AsList();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(models);

                // Act
                var result = connection.QueryAll<EntityModelForClass>();

                // Assert
                models.AsList().ForEach(e =>
                {
                    var item = result.First(obj => obj.Id == e.Id);
                    Assert.IsNull(item.Model.Value);
                });
            }
        }

        #endregion

        #region IntToString

        [TestMethod]
        public void TestPropertyHandlerWithIntToStringTypeHandler()
        {
            // Setup
            var models = CreateEntityModelForIntToStringTypes(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(models);

                // Act
                var result = connection.QueryAll<EntityModelForIntToStringType>();

                // Assert
                models.AsList().ForEach(e =>
                {
                    var item = result.First(obj => obj.Id == e.Id);
                    Assert.AreEqual(e.IntAsString, item.IntAsString);
                });
            }
        }

        [TestMethod]
        public void TestPropertyHandlerWithIntToStringTypeHandlerAsNull()
        {
            // Setup
            var models = CreateEntityModelForIntToStringTypes(10, true).AsList();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(models);

                // Act
                var result = connection.QueryAll<EntityModelForIntToStringType>();

                // Assert
                models.AsList().ForEach(e =>
                {
                    var item = result.First(obj => obj.Id == e.Id);
                    Assert.AreEqual("0", item.IntAsString); // Since the int type is really not nullable
                });
            }
        }

        #endregion
    }
}
