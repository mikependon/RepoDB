using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using NpgsqlTypes;
using RepoDb.Attributes;
using RepoDb.Extensions;
using RepoDb.PostgreSql.IntegrationTests.Setup;
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

        #endregion

        [TestMethod]
        public void TestInsertAndQueryForJson()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                var entity = GetCompleteTableForJsons(1).First();

                // Act
                connection.Insert(entity);

                // Query
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

                // Query
                var queryResult = connection.QueryAll<CompleteTableForJson>();

                // Assert
                entities.ForEach(e =>
                    Helper.AssertPropertiesEquality(e, queryResult.First(item => item.Id == e.Id)));
            }
        }
    }
}
