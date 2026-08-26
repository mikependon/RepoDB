using Microsoft.VisualStudio.TestTools.UnitTesting;
using EnterpriseDB.EDBClient;
using EDBTypes;
using RepoDb.Attributes;
using RepoDb.Attributes.Parameter.EnterpriseDb;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.EnterpriseDb.IntegrationTests.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.EnterpriseDb.IntegrationTests
{
    [TestClass]
    public class EDBDbTypeTests
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
            GlobalConfiguration
                .Setup(new() { ConversionType = ConversionType.Default });
            Database.Cleanup();
        }

        #region SubClasses

        [Map("CompleteTable")]
        private class CompleteTableForJson
        {
            public System.Int64 Id { get; set; }
            [EnterpriseDbType(EDBDbType.Json)]
            public System.String ColumnJson { get; set; }
        }

        [Map("CompleteTable")]
        private class CompleteTableForDateTime
        {
            public System.Int64 Id { get; set; }
            [EnterpriseDbType(EDBDbType.TimestampTz)]
            public System.DateTimeOffset ColumnTimestampWithTimeZone { get; set; }
            [EnterpriseDbType(EDBDbType.Timestamp)]
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
                    ColumnTimestampWithTimeZone = DateTimeOffset.Now.Date.AddSeconds(random.Next(60)).ToUniversalTime(),
                    ColumnTimestampWithoutTimeZone = DateTime.Now.Date.AddSeconds(random.Next(60))
                };
            }
        }

        #endregion

        #region JSON

        [TestMethod]
        public void TestInsertAndQueryForJson()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
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
            using (var connection = new EDBConnection(Database.ConnectionString))
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

        [TestMethod]
        public async Task TestInsertAndQueryAsyncForJson()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                var entity = GetCompleteTableForJsons(1).First();

                // Act
                await connection.InsertAsync(entity);

                // Act
                var queryResult = (await connection.QueryAsync<CompleteTableForJson>(entity.Id)).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestInsertAndQueryAsyncForJsons()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                var entities = GetCompleteTableForJsons(10).AsList();

                // Act
                await connection.InsertAllAsync(entities);

                // Act
                var queryResult = await connection.QueryAllAsync<CompleteTableForJson>();

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
            using (var connection = new EDBConnection(Database.ConnectionString))
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
            using (var connection = new EDBConnection(Database.ConnectionString))
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
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                var entity = GetCompleteTableForDateTimes(1).First();

                // Act
                connection.Insert(entity);

                // Setup
                DateTimeOffset startDate = DateTimeOffset.Now.Date.AddHours(-5).ToUniversalTime();
                DateTimeOffset endDate = DateTimeOffset.Now.Date.AddHours(5).ToUniversalTime();

                // Act
                var queryResult = connection.Query<CompleteTableForDateTime>(e =>
                    e.ColumnTimestampWithTimeZone >= startDate && e.ColumnTimestampWithTimeZone <= endDate).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestInsertAndQueryForDateTimeAsWhereExpressionFromVariable()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                var entity = GetCompleteTableForDateTimes(1).First();

                // Act
                connection.Insert(entity);

                // Setup
                DateTimeOffset startDate = DateTimeOffset.Now.Date.AddHours(-5).ToUniversalTime();
                DateTimeOffset endDate = DateTimeOffset.Now.Date.AddHours(5).ToUniversalTime();

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
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration
                    .Setup(new() { ConversionType = ConversionType.Automatic });
                var entity = GetCompleteTableForDateTimes(1).First();

                // Act
                connection.Insert(entity);

                // Setup
                DateTimeOffset startDate = DateTimeOffset.Now.Date.AddHours(-5).ToUniversalTime();
                DateTimeOffset endDate = DateTimeOffset.Now.Date.AddHours(5).ToUniversalTime();

                // Act
                var queryResult = connection.Query<CompleteTableForDateTime>(e =>
                    e.ColumnTimestampWithTimeZone >= startDate && e.ColumnTimestampWithTimeZone <= endDate).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestInsertAndQueryForDateTimeAsWhereExpressionFromVariableWithAutomaticConversion()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration
                    .Setup(new() { ConversionType = ConversionType.Automatic });
                var entity = GetCompleteTableForDateTimes(1).First();

                // Act
                connection.Insert(entity);

                // Setup
                DateTimeOffset startDate = DateTimeOffset.Now.Date.AddHours(-5).ToUniversalTime();
                DateTimeOffset endDate = DateTimeOffset.Now.Date.AddHours(5).ToUniversalTime();

                // Act
                var queryResult = connection.Query<CompleteTableForDateTime>(e =>
                    e.ColumnTimestampWithTimeZone >= startDate && e.ColumnTimestampWithTimeZone <= endDate).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestInsertAndQueryAsyncForDateTime()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                var entity = GetCompleteTableForDateTimes(1).First();

                // Act
                await connection.InsertAsync(entity);

                // Act
                var queryResult = (await connection.QueryAsync<CompleteTableForDateTime>(entity.Id)).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestInsertAndQueryAsyncForDateTimes()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                var entities = GetCompleteTableForDateTimes(10).AsList();

                // Act
                await connection.InsertAllAsync(entities);

                // Act
                var queryResult = await connection.QueryAllAsync<CompleteTableForDateTime>();

                // Assert
                entities.ForEach(e =>
                    Helper.AssertPropertiesEquality(e, queryResult.First(item => item.Id == e.Id)));
            }
        }

        [TestMethod]
        public async Task TestInsertAndQueryAsyncForDateTimeAsWhereExpression()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                var entity = GetCompleteTableForDateTimes(1).First();

                // Act
                await connection.InsertAsync(entity);

                // Setup
                DateTimeOffset startDate = DateTimeOffset.Now.Date.AddHours(-5).ToUniversalTime();
                DateTimeOffset endDate = DateTimeOffset.Now.Date.AddHours(5).ToUniversalTime();

                // Act
                var queryResult = (await connection.QueryAsync<CompleteTableForDateTime>(e =>
                    e.ColumnTimestampWithTimeZone >= startDate && e.ColumnTimestampWithTimeZone <= endDate)).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestInsertAndQueryAsyncForDateTimeAsWhereExpressionFromVariable()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                var entity = GetCompleteTableForDateTimes(1).First();

                // Act
                await connection.InsertAsync(entity);

                // Setup
                DateTimeOffset startDate = DateTimeOffset.Now.Date.AddHours(-5).ToUniversalTime();
                DateTimeOffset endDate = DateTimeOffset.Now.Date.AddHours(5).ToUniversalTime();

                // Act
                var queryResult = (await connection.QueryAsync<CompleteTableForDateTime>(e =>
                    e.ColumnTimestampWithTimeZone >= startDate && e.ColumnTimestampWithTimeZone <= endDate)).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestInsertAndQueryAsyncForDateTimeAsWhereExpressionWithAutomaticConversion()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration
                    .Setup(new() { ConversionType = ConversionType.Automatic });
                var entity = GetCompleteTableForDateTimes(1).First();

                // Act
                await connection.InsertAsync(entity);

                // Setup
                DateTimeOffset startDate = DateTimeOffset.Now.Date.AddHours(-5).ToUniversalTime();
                DateTimeOffset endDate = DateTimeOffset.Now.Date.AddHours(5).ToUniversalTime();

                // Act
                var queryResult = (await connection.QueryAsync<CompleteTableForDateTime>(e =>
                    e.ColumnTimestampWithTimeZone >= startDate && e.ColumnTimestampWithTimeZone <= endDate)).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestInsertAndQueryAsyncForDateTimeAsWhereExpressionFromVariableWithAutomaticConversion()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                GlobalConfiguration
                    .Setup(new() { ConversionType = ConversionType.Automatic });
                var entity = GetCompleteTableForDateTimes(1).First();

                // Act
                await connection.InsertAsync(entity);

                // Setup
                DateTimeOffset startDate = DateTimeOffset.Now.Date.AddHours(-5).ToUniversalTime();
                DateTimeOffset endDate = DateTimeOffset.Now.Date.AddHours(5).ToUniversalTime();

                // Act
                var queryResult = (await connection.QueryAsync<CompleteTableForDateTime>(e =>
                    e.ColumnTimestampWithTimeZone >= startDate && e.ColumnTimestampWithTimeZone <= endDate)).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        #endregion
    }
}
