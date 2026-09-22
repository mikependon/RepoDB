#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using DuckDB.NET.Data;
using RepoDb.Attributes;
using RepoDb.Extensions;
using RepoDb.DuckDb.IntegrationTests.Setup;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;

namespace RepoDb.DuckDb.IntegrationTests
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

        // DuckDB has no provider-specific parameter-type enum (unlike, e.g., MySqlConnector's MySqlDbType or
        // Npgsql's NpgsqlDbType - DuckDBParameter only exposes the standard System.Data.DbType), so this
        // exercises the generic, provider-agnostic RepoDb.Attributes.TypeMapAttribute instead.
        [Table("CompleteTable")]
        public class AttributeTable
        {
            public int Id { get; set; }

            [TypeMap(DbType.Binary)]
            public byte[] ColumnBlob { get; set; }

            [TypeMap(DbType.Int64)]
            public long ColumnBigInt { get; set; }

            [TypeMap(DbType.DateTime)]
            public DateTime ColumnDateTime2 { get; set; }

            [TypeMap(DbType.String)]
            public string ColumnVarChar { get; set; }
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
                    ColumnBlob = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()),
                    ColumnDateTime2 = DateTime.UtcNow.Date.AddDays(-random.Next(100)),
                    ColumnVarChar = $"ColumnNVarChar-{i}-{Guid.NewGuid()}"
                };
            }
        }

        #endregion

        #region Methods

        [TestMethod]
        public void TestDuckDBConnectionForInsertForTypeMapAttribute()
        {
            // Setup
            var table = CreateAttributeTables(1).First();

            using (var connection = new DuckDBConnection(Database.ConnectionString))
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
        public void TestDuckDBConnectionForInsertAllForTypeMapAttribute()
        {
            // Setup
            var tables = CreateAttributeTables(10).AsList();

            using (var connection = new DuckDBConnection(Database.ConnectionString))
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
        public void TestDuckDBConnectionForQueryForTypeMapAttribute()
        {
            // Setup
            var table = CreateAttributeTables(1).First();

            using (var connection = new DuckDBConnection(Database.ConnectionString))
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
        public void TestDuckDBConnectionForQueryAllForTypeMapAttribute()
        {
            // Setup
            var tables = CreateAttributeTables(10).AsList();

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<AttributeTable>(tables);

                // Query
                var queryResult = connection.QueryAll<AttributeTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
            }
        }

        #endregion
    }
}
