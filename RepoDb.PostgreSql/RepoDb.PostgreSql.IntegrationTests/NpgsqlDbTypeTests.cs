using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using NpgsqlTypes;
using RepoDb.Attributes;
using RepoDb.Extensions;
using RepoDb.PostgreSql.IntegrationTests.Setup;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb.PostgreSql.IntegrationTests
{
    [TestClass]
    public class NpgsqlDbTypeTests
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
            Converter.ConversionType = Enumerations.ConversionType.Default;
            Database.Cleanup();
        }

        #region SubClasses

        [Map("CompleteTable")]
        private class CompleteTableForJson
        {
            public System.Int64 Id { get; set; }
            [NpgsqlTypeMap(NpgsqlDbType.Json)]
            public System.String ColumnJson { get; set; }
        }

        [Map("CompleteTable")]
        private class CompleteTableForDateTime
        {
            public System.Int64 Id { get; set; }
            [NpgsqlTypeMap(NpgsqlDbType.TimestampTz)]
            public System.DateTime ColumnTimestampWithTimeZone { get; set; }
            [NpgsqlTypeMap(NpgsqlDbType.Timestamp)]
            public System.DateTime ColumnTimestampWithoutTimeZone { get; set; }
        }

        #endregion

        #region Helpers

        private IEnumerable<CompleteTableForJson> GetCompleteTableForJsons(int count = 0)
        {
            for (var i = 0; i < count; i++)
            {
                yield return new CompleteTableForJson
                {
                    Id = 1,
                    ColumnJson = $"{{\"Id\": {i}, \"Field1\": \"Field1Value\", \"Field2\": \"Field2Value\"}}"
                };
            }
        }

        private IEnumerable<CompleteTableForDateTime> GetCompleteTableForDateTimes(int count = 0)
        {
            var random = new Random();
            for (var i = 0; i < count; i++)
            {
                yield return new CompleteTableForDateTime
                {
                    Id = 1,
                    ColumnTimestampWithTimeZone = DateTime.Now.Date.AddSeconds(random.Next(60)),
                    ColumnTimestampWithoutTimeZone = DateTime.UtcNow.Date.AddSeconds(random.Next(60))
                };
            }
        }

        #endregion

        #region JSON

        [TestMethod]
        public void TestInsertAndQueryForJson()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                var entity = GetCompleteTableForJsons(1).First();

                // Act
                connection.Insert(entity);

                // Act
                var queryResult = connection.Query<CompleteTableForJson>(entity.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestInsertAndQueryForJsons()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                var entities = GetCompleteTableForJsons(10).AsList();

                // Act
                connection.InsertAll(entities);

                // Act
                var queryResult = connection.QueryAll<CompleteTableForJson>();

                // Assert
                entities.ForEach(e =>
                    Helper.AssertPropertiesEquality(e, queryResult.First(item => item.Id == e.Id)));
            }
        }

        #endregion

        #region DateTime

        [TestMethod]
        public void TestInsertAndQueryForDateTime()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                var entity = GetCompleteTableForDateTimes(1).First();

                // Act
                connection.Insert(entity);

                // Act
                var queryResult = connection.Query<CompleteTableForDateTime>(entity.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestInsertAndQueryForDateTimes()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                var entities = GetCompleteTableForDateTimes(10).AsList();

                // Act
                connection.InsertAll(entities);

                // Act
                var queryResult = connection.QueryAll<CompleteTableForDateTime>();

                // Assert
                entities.ForEach(e =>
                    Helper.AssertPropertiesEquality(e, queryResult.First(item => item.Id == e.Id)));
            }
        }

        [TestMethod]
        public void TestInsertAndQueryForDateTimeAsWhereExpression()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                var entity = GetCompleteTableForDateTimes(1).First();

                // Act
                connection.Insert(entity);

                // Act
                var queryResult = connection.Query<CompleteTableForDateTime>(e =>
                    e.ColumnTimestampWithTimeZone >= DateTime.UtcNow.Date.AddHours(-1) && e.ColumnTimestampWithTimeZone <= DateTime.UtcNow.Date.AddHours(1)).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestInsertAndQueryForDateTimeAsWhereExpressionFromVariable()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                var entity = GetCompleteTableForDateTimes(1).First();

                // Act
                connection.Insert(entity);

                // Setup
                var startDate = DateTime.UtcNow.Date.AddHours(-1);
                var endDate = DateTime.UtcNow.Date.AddHours(1);

                // Act
                var queryResult = connection.Query<CompleteTableForDateTime>(e =>
                    e.ColumnTimestampWithTimeZone >= startDate && e.ColumnTimestampWithTimeZone <= endDate).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestInsertAndQueryForDateTimeAsWhereExpressionWithAutomaticConversion()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                Converter.ConversionType = Enumerations.ConversionType.Automatic;
                var entity = GetCompleteTableForDateTimes(1).First();

                // Act
                connection.Insert(entity);

                // Act
                var queryResult = connection.Query<CompleteTableForDateTime>(e =>
                    e.ColumnTimestampWithTimeZone >= DateTime.UtcNow.Date.AddHours(-1) && e.ColumnTimestampWithTimeZone <= DateTime.UtcNow.Date.AddHours(1)).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestInsertAndQueryForDateTimeAsWhereExpressionFromVariableWithAutomaticConversion()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                Converter.ConversionType = Enumerations.ConversionType.Automatic;
                var entity = GetCompleteTableForDateTimes(1).First();

                // Act
                connection.Insert(entity);

                // Setup
                var startDate = DateTime.UtcNow.Date.AddHours(-1);
                var endDate = DateTime.UtcNow.Date.AddHours(1);

                // Act
                var queryResult = connection.Query<CompleteTableForDateTime>(e =>
                    e.ColumnTimestampWithTimeZone >= startDate && e.ColumnTimestampWithTimeZone <= endDate).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        #endregion
    }
}
