using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Setup;
using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
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

            // Add the maapings
            PropertyTypeHandlerMapper.Add(typeof(float), new PropertiesToLongTypeHandler(), true);
            PropertyTypeHandlerMapper.Add(typeof(decimal), new DecimalToLongTypeHandler(), true);
            PropertyTypeHandlerMapper.Add(typeof(DateTime), new DateTimeToUtcKindHandler(), true);
        }

        [TestCleanup]
        public void Cleanup()
        {
            PropertyTypeHandlerMapper.Remove(typeof(float), false);
            PropertyTypeHandlerMapper.Remove(typeof(decimal), false);
            PropertyTypeHandlerMapper.Remove(typeof(DateTime), false);
            Database.Cleanup();
        }

        #region Handlers

        /// <summary>
        /// A class used to handle the property transformation from a property to a class.
        /// </summary>
        private class PropertyToClassHandler : IPropertyHandler<string, TargetModel>
        {
            public TargetModel Get(string input,
                ClassProperty property)
            {
                return new TargetModel { Value = input };
            }

            public string Set(TargetModel input,
                ClassProperty property)
            {
                return input?.Value;
            }
        }

        /// <summary>
        /// A class used to handle the property transformation from Int to String.
        /// </summary>
        public class IntToStringTypeHandler : IPropertyHandler<int?, string>
        {
            public string Get(int? input,
                ClassProperty property)
            {
                return Convert.ToString(input);
            }

            public int? Set(string input,
                ClassProperty property)
            {
                return Convert.ToInt32(input);
            }
        }

        /// <summary>
        /// A class used to handle the property transformation from Decimal to Long. The values are nullable.
        /// </summary>
        public class DecimalToLongTypeHandler : IPropertyHandler<decimal?, long?>
        {
            public long? Get(decimal? input,
                ClassProperty property)
            {
                return Convert.ToInt64(input);
            }

            public decimal? Set(long? input,
                ClassProperty property)
            {
                return Convert.ToDecimal(input);
            }
        }

        /// <summary>
        /// A class used to handle the property transformation of any property to type long.
        /// </summary>
        public class PropertiesToLongTypeHandler : IPropertyHandler<object, long?>
        {
            public long? Get(object input,
                ClassProperty property)
            {
                return Convert.ToInt64(input);
            }

            public object Set(long? input,
                ClassProperty property)
            {
                return input;
            }
        }

        /// <summary>
        /// A class used to handle the property transformation of <see cref="DateTime.Kind" /> property. The values are not nullable.
        /// </summary>
        public class DateTimeToUtcKindHandler : IPropertyHandler<DateTime?, DateTime?>
        {
            public DateTime? Get(DateTime? input,
                ClassProperty property)
            {
                return input.HasValue ?
                    DateTime.SpecifyKind(input.Value, DateTimeKind.Utc) :
                    (DateTime?)null;
            }

            public DateTime? Set(DateTime? input,
                ClassProperty property)
            {
                return input.HasValue ?
                    DateTime.SpecifyKind(input.Value, DateTimeKind.Unspecified) :
                    (DateTime?)null;
            }
        }

        #endregion

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

        [Map("[dbo].[NonIdentityTable]")]
        private class EntityModelForNumberPropertiesToLongType
        {
            public Guid Id { get; set; }

            [Map("ColumnDecimal")]
            public long? DecimalAsLong { get; set; }

            [Map("ColumnFloat")]
            public long? FloatAsLong { get; set; }
        }

        [Map("[dbo].[CompleteTable]")]
        private class EntityModelForDateTimeKind
        {
            [Map("SessionId")]
            public Guid Id { get; set; }

            [Map("ColumnDateTime2"), TypeMap(DbType.DateTime2)]
            public DateTime? DateTime { get; set; }
        }

        #endregion

        #region Helpers

        private IEnumerable<EntityModelForClass> CreateEntityModelForClasses(int count,
            bool isModelNull = false)
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

        private IEnumerable<EntityModelForIntToStringType> CreateEntityModelForIntToStringTypes(int count,
            bool isIntNull = false)
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

        private IEnumerable<EntityModelForDateTimeKind> CreateEntityModelForDateTimeKinds(int count,
            bool isDateTimeNull = false)
        {
            for (var i = 0; i < count; i++)
            {
                yield return new EntityModelForDateTimeKind
                {
                    Id = Guid.NewGuid(),
                    DateTime = isDateTimeNull ? null : (DateTime?)DateTime.UtcNow
                };
            }
        }

        private IEnumerable<EntityModelForNumberPropertiesToLongType> CreateEntityModelForNumberPropertiesToLongTypes(int count,
            bool isIntNull = false)
        {
            for (var i = 0; i < count; i++)
            {
                yield return new EntityModelForNumberPropertiesToLongType
                {
                    Id = Guid.NewGuid(),
                    DecimalAsLong = isIntNull ? null : (long?)100,
                    FloatAsLong = isIntNull ? null : (long?)200
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
                models.ForEach(e =>
                {
                    var item = result.First(obj => obj.Id == e.Id);
                    Assert.AreEqual(e.Model.Value, item.Model.Value);
                });
            }
        }

        [TestMethod]
        public void TestPropertyHandlerWithPropertyToClassHandlerAtomic()
        {
            // Setup
            var models = CreateEntityModelForClasses(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                models.ForEach(e => connection.Insert(e));

                // Act
                var result = connection.QueryAll<EntityModelForClass>();

                // Assert
                models.ForEach(e =>
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
                models.ForEach(e =>
                {
                    var item = result.First(obj => obj.Id == e.Id);
                    Assert.IsNull(item.Model.Value);
                });
            }
        }

        [TestMethod]
        public void TestPropertyHandlerWithPropertyToClassHandlerAsNullAtomic()
        {
            // Setup
            var models = CreateEntityModelForClasses(10, true).AsList();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                models.ForEach(e => connection.Insert(e));

                // Act
                var result = connection.QueryAll<EntityModelForClass>();

                // Assert
                models.ForEach(e =>
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
                models.ForEach(e =>
                {
                    var item = result.First(obj => obj.Id == e.Id);
                    Assert.AreEqual(e.IntAsString, item.IntAsString);
                });
            }
        }

        [TestMethod]
        public void TestPropertyHandlerWithIntToStringTypeHandlerAtomic()
        {
            // Setup
            var models = CreateEntityModelForIntToStringTypes(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                models.ForEach(e => connection.Insert(e));

                // Act
                var result = connection.QueryAll<EntityModelForIntToStringType>();

                // Assert
                models.ForEach(e =>
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
                models.ForEach(e =>
                {
                    var item = result.First(obj => obj.Id == e.Id);
                    Assert.AreEqual("0", item.IntAsString);
                });
            }
        }

        [TestMethod]
        public void TestPropertyHandlerWithIntToStringTypeHandlerAsNullAtomic()
        {
            // Setup
            var models = CreateEntityModelForIntToStringTypes(10, true).AsList();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                models.ForEach(e => connection.Insert(e));

                // Act
                var result = connection.QueryAll<EntityModelForIntToStringType>();

                // Assert
                models.ForEach(e =>
                {
                    var item = result.First(obj => obj.Id == e.Id);
                    Assert.AreEqual("0", item.IntAsString);
                });
            }
        }

        #endregion

        #region DecimalToLong/NumbersToLong

        [TestMethod]
        public void TestPropertyHandlerWithNumbersToLongHandler()
        {
            // Setup
            var models = CreateEntityModelForNumberPropertiesToLongTypes(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(models);

                // Act
                var result = connection.QueryAll<EntityModelForNumberPropertiesToLongType>();

                // Assert
                models.ForEach(e =>
                {
                    var item = result.First(obj => obj.Id == e.Id);
                    Assert.AreEqual(e.DecimalAsLong, item.DecimalAsLong);
                    Assert.AreEqual(e.FloatAsLong, item.FloatAsLong);
                });
            }
        }

        [TestMethod]
        public void TestPropertyHandlerWithNumbersToLongHandlerAtomic()
        {
            // Setup
            var models = CreateEntityModelForNumberPropertiesToLongTypes(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                models.ForEach(e => connection.Insert(e));

                // Act
                var result = connection.QueryAll<EntityModelForNumberPropertiesToLongType>();

                // Assert
                models.ForEach(e =>
                {
                    var item = result.First(obj => obj.Id == e.Id);
                    Assert.AreEqual(e.DecimalAsLong, item.DecimalAsLong);
                    Assert.AreEqual(e.FloatAsLong, item.FloatAsLong);
                });
            }
        }

        [TestMethod]
        public void TestPropertyHandlerWithNumbersToLongHandlerAsNull()
        {
            // Setup
            var models = CreateEntityModelForNumberPropertiesToLongTypes(10, true).AsList();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(models);

                // Act
                var result = connection.QueryAll<EntityModelForNumberPropertiesToLongType>();

                // Assert
                models.ForEach(e =>
                {
                    var item = result.First(obj => obj.Id == e.Id);
                    Assert.AreEqual((long)0, item.DecimalAsLong.GetValueOrDefault());
                    Assert.AreEqual((long)0, item.FloatAsLong.GetValueOrDefault());
                });
            }
        }

        [TestMethod]
        public void TestPropertyHandlerWithNumbersToLongHandlerAsNullAtomic()
        {
            // Setup
            var models = CreateEntityModelForNumberPropertiesToLongTypes(10, true).AsList();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                models.ForEach(e => connection.Insert(e));

                // Act
                var result = connection.QueryAll<EntityModelForNumberPropertiesToLongType>();

                // Assert
                models.ForEach(e =>
                {
                    var item = result.First(obj => obj.Id == e.Id);
                    Assert.AreEqual((long)0, item.DecimalAsLong.GetValueOrDefault());
                    Assert.AreEqual((long)0, item.FloatAsLong.GetValueOrDefault());
                });
            }
        }

        #endregion

        #region DateTimeKind

        [TestMethod]
        public void TestPropertyHandlerForDateTimeKind()
        {
            // Setup
            var models = CreateEntityModelForDateTimeKinds(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(models);

                // Act
                var result = connection.QueryAll<EntityModelForDateTimeKind>();

                // Assert
                models.ForEach(e =>
                {
                    var item = result.First(obj => obj.Id == e.Id);
                    Assert.AreEqual(e.DateTime, item.DateTime);
                });
            }
        }

        [TestMethod]
        public void TestPropertyHandlerForDateTimeKindAtomic()
        {
            // Setup
            var models = CreateEntityModelForDateTimeKinds(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                models.ForEach(e => connection.Insert(e));

                // Act
                var result = connection.QueryAll<EntityModelForDateTimeKind>();

                // Assert
                models.ForEach(e =>
                {
                    var item = result.First(obj => obj.Id == e.Id);
                    Assert.AreEqual(e.DateTime, item.DateTime);
                });
            }
        }

        [TestMethod]
        public void TestPropertyHandlerForDateTimeKindAsNull()
        {
            // Setup
            var models = CreateEntityModelForDateTimeKinds(10, true).AsList();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(models);

                // Act
                var result = connection.QueryAll<EntityModelForDateTimeKind>();

                // Assert
                models.ForEach(e =>
                {
                    var item = result.First(obj => obj.Id == e.Id);
                    Assert.AreEqual(e.DateTime, item.DateTime);
                });
            }
        }

        [TestMethod]
        public void TestPropertyHandlerForDateTimeKindAsNullAtomic()
        {
            // Setup
            var models = CreateEntityModelForDateTimeKinds(10, true).AsList();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                models.ForEach(e => connection.Insert(e));

                // Act
                var result = connection.QueryAll<EntityModelForDateTimeKind>();

                // Assert
                models.ForEach(e =>
                {
                    var item = result.First(obj => obj.Id == e.Id);
                    Assert.AreEqual(e.DateTime, item.DateTime);
                });
            }
        }

        #endregion

    }
}
