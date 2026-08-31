#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sap.Data.Hana;
using RepoDb.Attributes.Parameter.SapHana;
using RepoDb.Extensions;
using RepoDb.SapHana.IntegrationTests.Setup;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace RepoDb.SapHana.IntegrationTests
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

        /// <summary>
        /// The properties here are declared in the same relative order as "CompleteTable"'s physical
        /// columns (Id, ColumnVarchar, ColumnBlob, ColumnDateTime2, ColumnBigint). That's required for
        /// SAP HANA specifically: unlike every other ADO.NET provider in this codebase, Sap.Data.Hana
        /// binds command parameters by their position in the <see cref="System.Data.IDataParameterCollection"/>
        /// rather than by their <c>:Name</c>, while RepoDb.Core builds the generated SQL text's column
        /// list from this class's declared property order but populates the parameter values by iterating
        /// the table's actual physical column order - so the two only line up when this class's properties
        /// are declared in that same physical order.
        /// </summary>
        [Table("CompleteTable")]
        public class AttributeTable
        {
            public int Id { get; set; }

            [SapHanaDbType(HanaDbType.VarChar)]
            public string ColumnVarchar { get; set; }

            [SapHanaDbType(HanaDbType.Blob)]
            public byte[] ColumnBlob { get; set; }

            [SapHanaDbType(HanaDbType.TimeStamp)]
            public DateTime ColumnDateTime2 { get; set; }

            [SapHanaDbType(HanaDbType.BigInt)]
            public long ColumnBigint { get; set; }
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
        public void TestHanaConnectionForInsertForSapHanaTypeMapAttribute()
        {
            // Setup
            var table = CreateAttributeTables(1).First();

            using (var connection = new HanaConnection(Database.ConnectionString))
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
        public void TestHanaConnectionForInsertAllForSapHanaTypeMapAttribute()
        {
            // Setup
            var tables = CreateAttributeTables(10).AsList();

            using (var connection = new HanaConnection(Database.ConnectionString))
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
        public void TestHanaConnectionForQueryForSapHanaTypeMapAttribute()
        {
            // Setup
            var table = CreateAttributeTables(1).First();

            using (var connection = new HanaConnection(Database.ConnectionString))
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
        public void TestHanaConnectionForQueryAllForSapHanaTypeMapAttribute()
        {
            // Setup
            var tables = CreateAttributeTables(10).AsList();

            using (var connection = new HanaConnection(Database.ConnectionString))
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
