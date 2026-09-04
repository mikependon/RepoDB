#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.EnterpriseDb;
using RepoDb.Attributes;
using RepoDb.Attributes.Parameter.EnterpriseDb;
using RepoDb.Extensions;
using RepoDb.EnterpriseDb.IntegrationTests.Setup;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoDb.EnterpriseDb.IntegrationTests
{
    [TestClass]
    public class AttributeTest
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

        [Table("CompleteTable")]
        public class AttributeTable
        {
            public int Id { get; set; }

            [EnterpriseDbType(EDBType.Bytea)]
            public byte[] ColumnByteA { get; set; }

            [EnterpriseDbType(EDBType.BigInt)]
            public long ColumnBigInt { get; set; }

            [EnterpriseDbType(EDBType.Date)]
            public DateTime ColumnDate { get; set; }

            [EnterpriseDbType(EDBType.Text)]
            public string ColumnText { get; set; }
        }

        #endregion

        #region Helpers

        private IEnumerable<AttributeTable> CreateAttributeTables(int count = 10)
        {
            var random = new Random();
            for (var i = 0; i < count; i++)
            {
                yield return new AttributeTable
                {
                    Id = i,
                    ColumnBigInt = Convert.ToInt64(random.Next(int.MaxValue)),
                    ColumnByteA = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()),
                    ColumnDate = DateTime.UtcNow.AddDays(-random.Next(100)).Date,
                    ColumnText = $"ColumnNVarChar-{i}-{Guid.NewGuid()}"
                };
            }
        }

        #endregion

        #region Methods

        [TestMethod]
        public void TestEDBConnectionForInsertForEnterpriseDbTypeMapAttribute()
        {
            // Setup
            var table = CreateAttributeTables(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<AttributeTable>(table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<AttributeTable>());

                // Query
                var queryResult = connection.QueryAll<AttributeTable>().First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestEDBConnectionForInsertAllForEnterpriseDbTypeMapAttribute()
        {
            // Setup
            var tables = CreateAttributeTables(10).AsList();

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<AttributeTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<AttributeTable>());

                // Query
                var queryResult = connection.QueryAll<AttributeTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestEDBConnectionForQueryForEnterpriseDbTypeMapAttribute()
        {
            // Setup
            var table = CreateAttributeTables(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var id = connection.Insert<AttributeTable>(table);

                // Query
                var queryResult = connection.Query<AttributeTable>(id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestEDBConnectionForQueryAllForEnterpriseDbTypeMapAttribute()
        {
            // Setup
            var tables = CreateAttributeTables(10).AsList();

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<AttributeTable>(tables);

                // Query
                var queryResult = connection.QueryAll<AttributeTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
            }
        }

        [TestMethod]
        public async Task TestEDBConnectionForInsertAsyncForEnterpriseDbTypeMapAttribute()
        {
            // Setup
            var table = CreateAttributeTables(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAsync<AttributeTable>(table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<AttributeTable>());

                // Query
                var queryResult = connection.QueryAll<AttributeTable>().First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public async Task TestEDBConnectionForInsertAllAsyncForEnterpriseDbTypeMapAttribute()
        {
            // Setup
            var tables = CreateAttributeTables(10).AsList();

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAllAsync<AttributeTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<AttributeTable>());

                // Query
                var queryResult = connection.QueryAll<AttributeTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public async Task TestEDBConnectionForQueryAsyncForEnterpriseDbTypeMapAttribute()
        {
            // Setup
            var table = CreateAttributeTables(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var id = connection.Insert<AttributeTable>(table);

                // Query
                var queryResult = (await connection.QueryAsync<AttributeTable>(id)).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public async Task TestEDBConnectionForQueryAllAsyncForEnterpriseDbTypeMapAttribute()
        {
            // Setup
            var tables = CreateAttributeTables(10).AsList();

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<AttributeTable>(tables);

                // Query
                var queryResult = await connection.QueryAllAsync<AttributeTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
            }
        }

        #endregion
    }
}
