#region Copyright Attributions

// Copyright (c) 2020 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using FirebirdSql.Data.FirebirdClient;
using RepoDb.Attributes.Parameter.Firebird;
using RepoDb.Extensions;
using RepoDb.Firebird.IntegrationTests.Setup;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace RepoDb.Firebird.IntegrationTests
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

            [FbDbType(FbDbType.Binary)]
            public byte[] ColumnBlob { get; set; }

            [FbDbType(FbDbType.BigInt)]
            public long ColumnBigint { get; set; }

            [FbDbType(FbDbType.TimeStamp)]
            public DateTime ColumnDateTime2 { get; set; }

            [FbDbType(FbDbType.VarChar)]
            public string ColumnVarchar { get; set; }
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
                    ColumnBigint = Convert.ToInt64(random.Next(int.MaxValue)),
                    ColumnBlob = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()),
                    ColumnDateTime2 = DateTime.UtcNow.Date.AddDays(-random.Next(100)),
                    ColumnVarchar = $"ColumnNVarChar-{i}-{Guid.NewGuid()}"
                };
            }
        }

        #endregion

        #region Methods

        [TestMethod]
        public void TestFirebirdConnectionForInsertForFirebirdMapAttribute()
        {
            // Setup
            var table = CreateAttributeTables(1).First();

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionForInsertAllForFirebirdMapAttribute()
        {
            // Setup
            var tables = CreateAttributeTables(10).AsList();

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionForQueryForFirebirdMapAttribute()
        {
            // Setup
            var table = CreateAttributeTables(1).First();

            using (var connection = new FbConnection(Database.ConnectionString))
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
        public void TestFirebirdConnectionForQueryAllForFirebirdMapAttribute()
        {
            // Setup
            var tables = CreateAttributeTables(10).AsList();

            using (var connection = new FbConnection(Database.ConnectionString))
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
