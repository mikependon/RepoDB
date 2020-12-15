using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Setup;
using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
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
            PropertyHandlerMapper.Remove(typeof(float));
            PropertyHandlerMapper.Remove(typeof(decimal));
            PropertyHandlerMapper.Remove(typeof(DateTime));
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
                return input > 0 ? Convert.ToString(input) : null;
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
                if (input > 0)
                {
                    return Convert.ToInt64(input);
                }
                else
                {
                    if (property.PropertyInfo.PropertyType.IsNullable())
                    {
                        return null;
                    }
                    else
                    {
                        return default(long);
                    }
                }
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
                var value = Convert.ToInt64(input);
                if (value > 0)
                {
                    return value;
                }
                else
                {
                    if (property.PropertyInfo.PropertyType.IsNullable())
                    {
                        return null;
                    }
                    else
                    {
                        return default(long);
                    }
                }
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

        /// <summary>
        /// A class used to handle the property transformation of <see cref="IDictionary{TKey, TValue}" /> property.
        /// </summary>
        public class DictionaryPropertyHandlerForString : IPropertyHandler<string, IDictionary<string, string>>
        {
            public IDictionary<string, string> Get(string input, ClassProperty property)
            {
                if (input == null)
                {
                    return null;
                }
                return new Dictionary<string, string>() { { "MyKey", input } };
            }

            public string Set(IDictionary<string, string> input, ClassProperty property)
            {
                if (input == null)
                {
                    return null;
                }
                return input.First().Value;
            }
        }

        #endregion

        #region Classes

        private class TargetModel
        {
            public string Value { get; set; }
        }

        [Map("[dbo].[PropertyHandler]")]
        private class EntityModelForClass
        {
            public long Id { get; set; }

            [Map("ColumnNVarChar"), PropertyHandler(typeof(PropertyToClassHandler))]
            public TargetModel NVarCharAsClass { get; set; }

            // Other non-nullables

            public int ColumnIntNotNull { get; set; } = 0;

            public decimal ColumnDecimalNotNull { get; set; } = 0;

            public short ColumnFloatNotNull { get; set; } = 0;

            public DateTime ColumnDateTimeNotNull { get; set; } = System.DateTime.UtcNow.Date;

            public DateTime ColumnDateTime2NotNull { get; set; } = System.DateTime.UtcNow;
        }

        [Map("[dbo].[PropertyHandler]")]
        private class EntityModelForIntToStringType
        {
            public long Id { get; set; }

            [Map("ColumnInt"), PropertyHandler(typeof(IntToStringTypeHandler))]
            public string IntAsString { get; set; }

            [Map("ColumnIntNotNull"), PropertyHandler(typeof(IntToStringTypeHandler))]
            public string IntNotNullAsString { get; set; }

            // Other non-nullables

            public decimal ColumnDecimalNotNull { get; set; } = 0;

            public short ColumnFloatNotNull { get; set; } = 0;

            public DateTime ColumnDateTimeNotNull { get; set; } = System.DateTime.UtcNow.Date;

            public DateTime ColumnDateTime2NotNull { get; set; } = System.DateTime.UtcNow;
        }

        [Map("[dbo].[PropertyHandler]")]
        private class EntityModelForNumberPropertiesToLongType
        {
            public long Id { get; set; }

            [Map("ColumnDecimal")]
            public long? DecimalAsLong { get; set; }

            [Map("ColumnFloat"), PropertyHandler(typeof(PropertiesToLongTypeHandler))]
            public long? FloatAsLong { get; set; }

            // Other non-nullables

            public int ColumnIntNotNull { get; set; } = 0;

            public decimal ColumnDecimalNotNull { get; set; } = 0;

            public short ColumnFloatNotNull { get; set; } = 0;

            public DateTime ColumnDateTimeNotNull { get; set; } = System.DateTime.UtcNow.Date;

            public DateTime ColumnDateTime2NotNull { get; set; } = System.DateTime.UtcNow;
        }

        [Map("[dbo].[PropertyHandler]")]
        private class EntityModelForDateTimeKind
        {
            public long Id { get; set; }

            [Map("ColumnDateTime")]
            public DateTime? DateTime { get; set; }

            [Map("ColumnDateTimeNotNull")]
            public DateTime DateTimeNotNull { get; set; } = System.DateTime.UtcNow.Date;

            [Map("ColumnDateTime2"), TypeMap(DbType.DateTime2)]
            public DateTime? DateTime2 { get; set; }

            [Map("ColumnDateTime2NotNull"), TypeMap(DbType.DateTime2)]
            public DateTime DateTime2NotNull { get; set; }

            // Other non-nullables

            public int ColumnIntNotNull { get; set; } = 0;

            public decimal ColumnDecimalNotNull { get; set; } = 0;

            public float ColumnFloatNotNull { get; set; } = 0;
        }

        [Map("[dbo].[CompleteTable]")]
        public class CompleteTableWithPropertyHandlerForDictionary
        {
            public Guid SessionId { get; set; }
            [PropertyHandler(typeof(DictionaryPropertyHandlerForString))]
            public IDictionary<string, string> ColumnNVarChar { get; set; }
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
                    NVarCharAsClass = isModelNull ? null : new TargetModel
                    {
                        Value = $"Value-{i}-{Guid.NewGuid()}",
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
                    IntAsString = isIntNull ? null : Convert.ToString(new Random().Next(int.MaxValue)),
                    IntNotNullAsString = isIntNull ? null : Convert.ToString(new Random().Next(int.MaxValue))
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
                    DateTime = isDateTimeNull ? null : (DateTime?)DateTime.UtcNow.Date,
                    DateTime2 = isDateTimeNull ? null : (DateTime?)DateTime.UtcNow
                };
            }
        }

        private IEnumerable<dynamic> CreateEntityModelForDateTimeKindForAnonymousTypes(int count,
            bool isDateTimeNull = false)
        {
            for (var i = 0; i < count; i++)
            {
                yield return new
                {
                    Id = (i + 1),
                    ColumnDateTime = isDateTimeNull ? null : (DateTime?)DateTime.UtcNow.Date,
                    ColumnDateTimeNotNull = (DateTime?)DateTime.UtcNow.Date,
                    ColumnDateTime2 = isDateTimeNull ? null : (DateTime?)DateTime.UtcNow,
                    ColumnDateTime2NotNull = (DateTime?)DateTime.UtcNow.Date,
                    ColumnIntNotNull = default(int),
                    ColumnDecimalNotNull = default(decimal),
                    ColumnFloatNotNull = default(float)
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
                    DecimalAsLong = isIntNull ? null : (long?)100,
                    FloatAsLong = isIntNull ? null : (long?)200
                };
            }
        }

        public CompleteTableWithPropertyHandlerForDictionary CreateCompleteTableWithPropertyHandlerForDictionary()
        {
            return new CompleteTableWithPropertyHandlerForDictionary
            {
                SessionId = Guid.NewGuid(),
                ColumnNVarChar = new Dictionary<string, string>() { { "MyKey", $"Value-{Guid.NewGuid()}" } }
            };
        }

        public IEnumerable<CompleteTableWithPropertyHandlerForDictionary> CreateompleteTableWithPropertyHandlerForDictionaries(int count = 0)
        {
            for (var i = 0; i < count; i++)
            {
                yield return new CompleteTableWithPropertyHandlerForDictionary
                {
                    SessionId = Guid.NewGuid(),
                    ColumnNVarChar = new Dictionary<string, string>() { { $"Key-{i}", $"Value-{Guid.NewGuid()}" } }
                };
            }
        }

        #endregion

        #region Expression

        [TestMethod]
        public void TestPropertyHandlerWithWhereConditionViaExpression()
        {
            // Setup
            var model = CreateEntityModelForIntToStringTypes(1).First();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert<EntityModelForIntToStringType, long>(model);

                // Act
                var result = connection.Query<EntityModelForIntToStringType>(e => e.IntAsString == model.IntAsString).FirstOrDefault();

                // Assert
                Assert.IsNotNull(result);
                Helper.AssertPropertiesEquality(model, result);
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
                    Helper.AssertPropertiesEquality(e.NVarCharAsClass, item.NVarCharAsClass);
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
                    Helper.AssertPropertiesEquality(e.NVarCharAsClass, item.NVarCharAsClass);
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
                    Assert.IsNull(item.NVarCharAsClass.Value);
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
                    Assert.IsNull(item.NVarCharAsClass.Value);
                });
            }
        }

        #endregion

        #region PropertyToDictionary

        [TestMethod]
        public void TestPropertyHandlerWithPropertyToDictionary()
        {
            // Setup
            var entity = CreateCompleteTableWithPropertyHandlerForDictionary();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.ExecuteScalar<Guid>("INSERT INTO [dbo].[CompleteTable] " +
                    "(SessionId, ColumnNVarChar) " +
                    "VALUES " +
                    "(@SessionId, @ColumnNVarChar); " +
                    "SELECT CONVERT(UNIQUEIDENTIFIER, @SessionId);", entity);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTableWithPropertyHandlerForDictionary>());
                Assert.AreNotEqual(id, Guid.Empty);
                Assert.AreEqual(entity.SessionId, id);
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
                    Helper.AssertPropertiesEquality(e, item);
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
                    Helper.AssertPropertiesEquality(e, item);
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
                    Helper.AssertPropertiesEquality(e, item);
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
                    Helper.AssertPropertiesEquality(e, item);
                });
            }
        }

        #endregion

        #region DecimalToLong/NumbersToLong

        [TestMethod]
        public void TestPropertyHandlerWithNumbersToLongHandler()
        {
            // Setup
            PropertyHandlerMapper.Add(typeof(decimal), new DecimalToLongTypeHandler(), true);

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
                    Helper.AssertPropertiesEquality(e, item);
                });
            }
        }

        [TestMethod]
        public void TestPropertyHandlerWithNumbersToLongHandlerAtomic()
        {
            // Setup
            PropertyHandlerMapper.Add(typeof(decimal), new DecimalToLongTypeHandler(), true);

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
                    Helper.AssertPropertiesEquality(e, item);
                });
            }
        }

        [TestMethod]
        public void TestPropertyHandlerWithNumbersToLongHandlerAsNull()
        {
            // Setup
            PropertyHandlerMapper.Add(typeof(decimal), new DecimalToLongTypeHandler(), true);

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
                    Helper.AssertPropertiesEquality(e, item);
                });
            }
        }

        [TestMethod]
        public void TestPropertyHandlerWithNumbersToLongHandlerAsNullAtomic()
        {
            // Setup
            PropertyHandlerMapper.Add(typeof(decimal), new DecimalToLongTypeHandler(), true);

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
                    Helper.AssertPropertiesEquality(e, item);
                });
            }
        }

        #endregion

        #region DateTimeKind

        [TestMethod]
        public void TestPropertyHandlerForDateTimeKind()
        {
            // Setup
            PropertyHandlerMapper.Add(typeof(DateTime), new DateTimeToUtcKindHandler(), true);

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
                    Helper.AssertPropertiesEquality(e, item);
                });
            }
        }

        [TestMethod]
        public void TestPropertyHandlerForDateTimeKindAtomic()
        {
            // Setup
            PropertyHandlerMapper.Add(typeof(DateTime), new DateTimeToUtcKindHandler(), true);

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
                    Helper.AssertPropertiesEquality(e, item);
                });
            }
        }

        [TestMethod]
        public void TestPropertyHandlerForDateTimeKindAsNull()
        {
            // Setup
            PropertyHandlerMapper.Add(typeof(DateTime), new DateTimeToUtcKindHandler(), true);

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
                    Helper.AssertPropertiesEquality(e, item);
                });
            }
        }

        [TestMethod]
        public void TestPropertyHandlerForDateTimeKindAsNullAtomic()
        {
            // Setup
            PropertyHandlerMapper.Add(typeof(DateTime), new DateTimeToUtcKindHandler(), true);

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
                    Helper.AssertPropertiesEquality(e, item);
                });
            }
        }

        [TestMethod]
        public void TestPropertyHandlerForDateTimeKindForAnonymousTypes()
        {
            // Setup
            PropertyHandlerMapper.Add(typeof(DateTime), new DateTimeToUtcKindHandler(), true);

            // Setup
            var models = CreateEntityModelForDateTimeKindForAnonymousTypes(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll("[dbo].[PropertyHandler]", models);

                // Act
                var result = connection.QueryAll<EntityModelForDateTimeKind>();

                // Assert
                models.ForEach(e =>
                {
                    var item = result.First(obj => obj.Id == e.Id);
                    Helper.AssertPropertiesEquality(e, item);
                });
            }
        }

        [TestMethod]
        public void TestPropertyHandlerForDateTimeKindAtomicForAnonymousTypes()
        {
            // Setup
            PropertyHandlerMapper.Add(typeof(DateTime), new DateTimeToUtcKindHandler(), true);

            // Setup
            var models = CreateEntityModelForDateTimeKindForAnonymousTypes(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                models.ForEach(e => connection.Insert<int>("[dbo].[PropertyHandler]", (object)e));

                // Act
                var result = connection.QueryAll<EntityModelForDateTimeKind>();

                // Assert
                models.ForEach(e =>
                {
                    var item = result.First(obj => obj.Id == e.Id);
                    Helper.AssertPropertiesEquality(e, item);
                });
            }
        }

        #endregion
    }
}
