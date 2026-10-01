#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Attributes;
using RepoDb.DbSettings;
using RepoDb.Enumerations.PostgreSql;
using RepoDb.Exceptions;
using RepoDb.IntegrationTests.Setup;
using RepoDb.PostgreSql.BulkOperations.IntegrationTests.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.PostgreSql.BulkOperations.IntegrationTests
{
    /// <summary>
    /// Tests for the <see cref="PostgreSqlBulkOperationsDbSetting.BulkColumnMappingsBehavior"/> setting
    /// (https://github.com/mikependon/RepoDB/issues/1360). The behavior is only applied when no explicit
    /// mappings were passed to the operation.
    /// </summary>
    [TestClass]
    [DoNotParallelize]
    public class BulkColumnMappingsBehaviorTest
    {
        private const string TableName = "BulkOperationIdentityTable";

        /// <summary>
        /// The columns of the "BulkOperationIdentityTable" table, in the order of the table.
        /// </summary>
        private static readonly string[] AllColumns =
        [
            "Id",
            "ColumnChar",
            "ColumnBigInt",
            "ColumnBit",
            "ColumnBoolean",
            "ColumnDate",
            "ColumnInteger",
            "ColumnMoney",
            "ColumnNumeric",
            "ColumnReal",
            "ColumnSerial",
            "ColumnSmallInt",
            "ColumnSmallSerial",
            "ColumnText",
            "ColumnTimeWithTimeZone",
            "ColumnTimeWithoutTimeZone",
            "ColumnTimestampWithTimeZone",
            "ColumnTimestampWithoutTimeZone"
        ];

        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            // The setting is global, so always revert to the default PostgreSqlDbSetting for the other test classes
            GlobalConfiguration
                .Setup()
                .UsePostgreSql(new PostgreSqlDbSetting());
            Database.Cleanup();
        }

        #region SubClasses

        /// <summary>
        /// A model with all the columns of the "BulkOperationIdentityTable" table, in the order of the table.
        /// </summary>
        [Map(TableName)]
        private class FullIdentityTable
        {
            public long Id { get; set; }
            public char? ColumnChar { get; set; }
            public long? ColumnBigInt { get; set; }
            public bool? ColumnBit { get; set; }
            public bool? ColumnBoolean { get; set; }
            public DateTime? ColumnDate { get; set; }
            public int? ColumnInteger { get; set; }
            public decimal? ColumnMoney { get; set; }
            public decimal? ColumnNumeric { get; set; }
            public float? ColumnReal { get; set; }
            public int? ColumnSerial { get; set; }
            public short? ColumnSmallInt { get; set; }
            public short? ColumnSmallSerial { get; set; }
            public string ColumnText { get; set; }
            public DateTimeOffset? ColumnTimeWithTimeZone { get; set; }
            public TimeSpan? ColumnTimeWithoutTimeZone { get; set; }
            public DateTime? ColumnTimestampWithTimeZone { get; set; }
            public DateTime? ColumnTimestampWithoutTimeZone { get; set; }
        }

        #endregion

        #region Helpers

        /*
         * The default identity behavior of this package is KeepIdentity (the identity values of the source are written),
         * so the tests that insert rows next to the seeded ones (or from entities with a default Id) use ReturnIdentity.
         */


        private static void UseBehavior(PostgreSqlBulkColumnMappingsBehavior behavior) =>
            GlobalConfiguration
                .Setup()
                .UsePostgreSql(new PostgreSqlBulkOperationsDbSetting
                {
                    BulkColumnMappingsBehavior = behavior
                });

        private static string Quote(IEnumerable<string> columns) =>
            string.Join(", ", columns.Select(column => $"\"{column}\""));

        /// <summary>
        /// Seeds the table with 'count' rows, and returns them.
        /// </summary>
        private static List<BulkOperationLightIdentityTable> Seed(int count)
        {
            using var connection = new NpgsqlConnection(Database.ConnectionString);
            connection.InsertAll(TableName, Helper.CreateBulkOperationLightIdentityTables(count));
            return connection.QueryAll<BulkOperationLightIdentityTable>(TableName).ToList();
        }

        /// <summary>
        /// Loads a <see cref="DataTable"/> from the given SELECT statement (so the column types are the ones of Npgsql).
        /// </summary>
        private static DataTable LoadDataTable(string commandText)
        {
            using var connection = new NpgsqlConnection(Database.ConnectionString);
            using var reader = connection.ExecuteReader(commandText);
            var table = new DataTable();
            table.Load(reader);

            // Load() marks the identity column as read-only, but ReturnIdentity writes the new identities back into it
            foreach (DataColumn column in table.Columns)
            {
                column.ReadOnly = false;
            }

            return table;
        }

        private static long CountAll()
        {
            using var connection = new NpgsqlConnection(Database.ConnectionString);
            return connection.CountAll(TableName);
        }

        private static List<PostgreSqlBulkInsertMapItem> CreateMappings(params string[] columns) =>
            columns.Select(column => new PostgreSqlBulkInsertMapItem(column, column)).ToList();

        private static FullIdentityTable CreateFullEntity(int index) =>
            new FullIdentityTable
            {
                ColumnBigInt = index,
                ColumnBoolean = true,
                ColumnInteger = index,
                ColumnNumeric = index,
                ColumnReal = index,
                ColumnSmallInt = (short)index,
                ColumnText = $"Text-{index}"
            };

        #endregion

        #region Configuration

        [TestMethod]
        public void TestPostgreSqlBulkOperationsDbSettingDefaultIsAutomatic()
        {
            // Act
            var setting = new PostgreSqlBulkOperationsDbSetting();

            // Assert
            Assert.AreEqual(PostgreSqlBulkColumnMappingsBehavior.Automatic, setting.BulkColumnMappingsBehavior);
        }

        [TestMethod]
        public void TestPostgreSqlBulkOperationsDbSettingInheritsThePostgreSqlDbSetting()
        {
            // Setup
            var expected = new PostgreSqlDbSetting();

            // Act
            var setting = new PostgreSqlBulkOperationsDbSetting();

            // Assert
            Assert.IsInstanceOfType<PostgreSqlDbSetting>(setting);
            Assert.AreEqual(expected.OpeningQuote, setting.OpeningQuote);
            Assert.AreEqual(expected.ClosingQuote, setting.ClosingQuote);
            Assert.AreEqual(expected.DefaultSchema, setting.DefaultSchema);
            Assert.AreEqual(expected.ParameterPrefix, setting.ParameterPrefix);
            Assert.AreEqual(expected.MaxParameterCount, setting.MaxParameterCount);
        }

        [TestMethod]
        public void TestUsePostgreSqlWithBulkOperationsDbSettingRegistersItAsTheDbSetting()
        {
            // Setup
            var setting = new PostgreSqlBulkOperationsDbSetting
            {
                BulkColumnMappingsBehavior = PostgreSqlBulkColumnMappingsBehavior.StrictBypass
            };

            // Act
            var result = GlobalConfiguration
                .Setup()
                .UsePostgreSql(setting);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(PostgreSqlBootstrap.IsInitialized);
            Assert.AreSame(setting, DbSettingMapper.Get<NpgsqlConnection>());
            using var connection = new NpgsqlConnection(Database.ConnectionString);
            Assert.AreSame(setting, connection.GetDbSetting());
        }

        [TestMethod]
        public void TestUsePostgreSqlWithBulkOperationsDbSettingKeepsTheOperationsWorking()
        {
            // Setup
            UseBehavior(PostgreSqlBulkColumnMappingsBehavior.Strict);

            // Act (the non-bulk operations use the inherited PostgreSQL values)
            var seeded = Seed(5);

            // Assert
            Assert.AreEqual(5, seeded.Count);
        }

        [TestMethod]
        public void TestDefaultPostgreSqlDbSettingBehavesAsAutomatic()
        {
            // Setup (only the default PostgreSqlDbSetting is registered)
            Assert.IsNotInstanceOfType<PostgreSqlBulkOperationsDbSetting>(DbSettingMapper.Get<NpgsqlConnection>());
            Seed(5);
            using var table = LoadDataTable($"SELECT \"ColumnText\", 'Extra' AS \"Nickname\", \"ColumnInteger\" FROM \"{TableName}\";");

            using var connection = new NpgsqlConnection(Database.ConnectionString);

            // Act
            var result = connection.BulkInsert(TableName, table);

            // Assert
            Assert.AreEqual(5, result);
            Assert.AreEqual(10, CountAll());
        }

        [TestMethod]
        public void ThrowExceptionOnUsePostgreSqlIfTheBulkOperationsDbSettingIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() =>
                GlobalConfiguration
                    .Setup()
                    .UsePostgreSql((PostgreSqlBulkOperationsDbSetting)null));
        }

        #endregion

        #region Automatic

        [TestMethod]
        public void TestBulkInsertAutomaticForDataTableWithSubsetExtraAndDifferentOrder()
        {
            // Setup (no 'Id', a different order, and an extra 'Nickname' column)
            Seed(10);
            using var table = LoadDataTable($"SELECT \"ColumnText\", 'Extra' AS \"Nickname\", \"ColumnInteger\" FROM \"{TableName}\";");

            using var connection = new NpgsqlConnection(Database.ConnectionString);

            // Act
            var result = connection.BulkInsert(TableName, table);

            // Assert
            Assert.AreEqual(10, result);
            Assert.AreEqual(20, CountAll());
        }

        [TestMethod]
        public void TestBulkInsertAutomaticForSubsetEntities()
        {
            // Setup
            var entities = Helper.CreateBulkOperationLightIdentityTables(10);

            using var connection = new NpgsqlConnection(Database.ConnectionString);

            // Act
            var result = connection.BulkInsert(TableName, entities, identityBehavior: PostgreSqlBulkImportIdentityBehavior.ReturnIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);
            Assert.AreEqual(entities.Count, CountAll());
        }

        #endregion

        #region Strict

        [TestMethod]
        public void TestBulkInsertStrictForEntitiesWithIdenticalColumns()
        {
            // Setup
            UseBehavior(PostgreSqlBulkColumnMappingsBehavior.Strict);
            var entities = Enumerable.Range(1, 10).Select(CreateFullEntity).ToList();

            using var connection = new NpgsqlConnection(Database.ConnectionString);

            // Act (the identity column is part of the source, but it is still not written)
            var result = connection.BulkInsert(TableName, entities, identityBehavior: PostgreSqlBulkImportIdentityBehavior.ReturnIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);
            Assert.AreEqual(entities.Count, CountAll());
        }

        [TestMethod]
        public void TestBulkInsertStrictForDataTableWithIdenticalColumns()
        {
            // Setup
            Seed(10);
            UseBehavior(PostgreSqlBulkColumnMappingsBehavior.Strict);
            using var table = LoadDataTable($"SELECT {Quote(AllColumns)} FROM \"{TableName}\";");

            using var connection = new NpgsqlConnection(Database.ConnectionString);

            // Act
            var result = connection.BulkInsert(TableName, table, identityBehavior: PostgreSqlBulkImportIdentityBehavior.ReturnIdentity);

            // Assert
            Assert.AreEqual(10, result);
            Assert.AreEqual(20, CountAll());
        }

        [TestMethod]
        public void TestBulkInsertStrictForDataReaderWithIdenticalColumns()
        {
            // Setup
            Seed(10);
            UseBehavior(PostgreSqlBulkColumnMappingsBehavior.Strict);

            using var sourceConnection = new NpgsqlConnection(Database.ConnectionString);
            using var reader = sourceConnection.ExecuteReader($"SELECT * FROM \"{TableName}\";");
            using var connection = new NpgsqlConnection(Database.ConnectionString);

            // Act
            var result = connection.BulkInsert(TableName, reader: reader, identityBehavior: PostgreSqlBulkImportIdentityBehavior.ReturnIdentity);

            // Assert
            Assert.AreEqual(10, result);
            Assert.AreEqual(20, CountAll());
        }

        [TestMethod]
        public void ThrowExceptionOnBulkInsertStrictForSubsetEntities()
        {
            // Setup
            UseBehavior(PostgreSqlBulkColumnMappingsBehavior.Strict);
            var entities = Helper.CreateBulkOperationLightIdentityTables(3);

            using var connection = new NpgsqlConnection(Database.ConnectionString);

            // Act
            var exception = Assert.Throws<PostgreSqlBulkColumnMappingsException>(() => connection.BulkInsert(TableName, entities));

            // Assert
            Assert.AreEqual(PostgreSqlBulkColumnMappingsBehavior.Strict, exception.Behavior);
            CollectionAssert.Contains(exception.DestinationColumnsNotInSource.ToList(), "ColumnMoney");
            Assert.AreEqual(0, exception.SourceColumnsNotInDestination.Count);
            Assert.AreEqual(0, CountAll());
        }

        [TestMethod]
        public void ThrowExceptionOnBulkInsertStrictForDataTableIfTheOrderIsDifferent()
        {
            // Setup (all the columns, but 'ColumnText' and 'ColumnSmallSerial' are swapped)
            Seed(3);
            UseBehavior(PostgreSqlBulkColumnMappingsBehavior.Strict);
            var columns = AllColumns.ToArray();
            (columns[12], columns[13]) = (columns[13], columns[12]);
            using var table = LoadDataTable($"SELECT {Quote(columns)} FROM \"{TableName}\";");

            using var connection = new NpgsqlConnection(Database.ConnectionString);

            // Act
            var exception = Assert.Throws<PostgreSqlBulkColumnMappingsException>(() => connection.BulkInsert(TableName, table));

            // Assert
            Assert.IsTrue(exception.IsOrderMismatch);
            StringAssert.Contains(exception.Message, "same order");
            Assert.AreEqual(3, CountAll());
        }

        [TestMethod]
        public void ThrowExceptionOnBulkInsertStrictForDataReaderWithAnExtraColumn()
        {
            // Setup
            Seed(3);
            UseBehavior(PostgreSqlBulkColumnMappingsBehavior.Strict);

            using var sourceConnection = new NpgsqlConnection(Database.ConnectionString);
            using var reader = sourceConnection.ExecuteReader($"SELECT *, 'Extra' AS \"Nickname\" FROM \"{TableName}\";");
            using var connection = new NpgsqlConnection(Database.ConnectionString);

            // Act
            var exception = Assert.Throws<PostgreSqlBulkColumnMappingsException>(() => connection.BulkInsert(TableName, reader: reader));

            // Assert
            CollectionAssert.AreEqual(new[] { "Nickname" }, exception.SourceColumnsNotInDestination.ToArray());
            Assert.AreEqual(3, CountAll());
        }

        [TestMethod]
        public async Task ThrowExceptionOnBulkInsertAsyncStrictForSubsetEntities()
        {
            // Setup
            UseBehavior(PostgreSqlBulkColumnMappingsBehavior.Strict);
            var entities = Helper.CreateBulkOperationLightIdentityTables(3);

            using var connection = new NpgsqlConnection(Database.ConnectionString);

            // Act/Assert
            await Assert.ThrowsAsync<PostgreSqlBulkColumnMappingsException>(() => connection.BulkInsertAsync(TableName, entities));
            Assert.AreEqual(0, CountAll());
        }

        #endregion

        #region StrictBypass

        [TestMethod]
        public void TestBulkInsertStrictBypassForSubsetEntities()
        {
            // Setup
            UseBehavior(PostgreSqlBulkColumnMappingsBehavior.StrictBypass);
            var entities = Helper.CreateBulkOperationLightIdentityTables(10);

            using var connection = new NpgsqlConnection(Database.ConnectionString);

            // Act
            var result = connection.BulkInsert(TableName, entities, identityBehavior: PostgreSqlBulkImportIdentityBehavior.ReturnIdentity);

            // Assert
            Assert.AreEqual(entities.Count, result);
            Assert.AreEqual(entities.Count, CountAll());
        }

        [TestMethod]
        public void TestBulkInsertStrictBypassForDataTableWithSubsetInDifferentOrder()
        {
            // Setup (no 'Id' and a different order: both are allowed)
            Seed(10);
            UseBehavior(PostgreSqlBulkColumnMappingsBehavior.StrictBypass);
            using var table = LoadDataTable($"SELECT \"ColumnText\", \"ColumnInteger\" FROM \"{TableName}\";");

            using var connection = new NpgsqlConnection(Database.ConnectionString);

            // Act
            var result = connection.BulkInsert(TableName, table);

            // Assert (the bypassed columns are left to their defaults)
            Assert.AreEqual(10, result);
            using var queryConnection = new NpgsqlConnection(Database.ConnectionString);
            var inserted = queryConnection.ExecuteScalar<long>($"SELECT COUNT(*) FROM \"{TableName}\" WHERE \"ColumnBigInt\" IS NULL AND \"ColumnText\" IS NOT NULL;");
            Assert.AreEqual(10, inserted);
        }

        [TestMethod]
        public void ThrowExceptionOnBulkInsertStrictBypassForDataTableIfASourceColumnDoesNotExist()
        {
            // Setup (the scenario of the issue: a source with an unknown 'Nickname' column)
            Seed(3);
            UseBehavior(PostgreSqlBulkColumnMappingsBehavior.StrictBypass);
            using var table = LoadDataTable($"SELECT \"ColumnText\", \"ColumnInteger\", 'Extra' AS \"Nickname\" FROM \"{TableName}\";");

            using var connection = new NpgsqlConnection(Database.ConnectionString);

            // Act
            var exception = Assert.Throws<PostgreSqlBulkColumnMappingsException>(() => connection.BulkInsert(TableName, table));

            // Assert
            Assert.AreEqual(PostgreSqlBulkColumnMappingsBehavior.StrictBypass, exception.Behavior);
            Assert.AreEqual(TableName, exception.TableName);
            CollectionAssert.AreEqual(new[] { "Nickname" }, exception.SourceColumnsNotInDestination.ToArray());
            StringAssert.Contains(exception.Message, "'Nickname'");
            Assert.AreEqual(3, CountAll());
        }

        [TestMethod]
        public void ThrowExceptionOnBulkInsertStrictBypassForDictionaryWithAnUnknownKey()
        {
            // Setup
            UseBehavior(PostgreSqlBulkColumnMappingsBehavior.StrictBypass);
            var entities = new List<IDictionary<string, object>>
            {
                new Dictionary<string, object> { ["ColumnText"] = "Text", ["ColumnInteger"] = 1, ["Nickname"] = "Extra" }
            };

            using var connection = new NpgsqlConnection(Database.ConnectionString);

            // Act/Assert
            var exception = Assert.Throws<PostgreSqlBulkColumnMappingsException>(() => connection.BulkInsert(TableName, entities));
            CollectionAssert.AreEqual(new[] { "Nickname" }, exception.SourceColumnsNotInDestination.ToArray());
            Assert.AreEqual(0, CountAll());
        }

        #endregion

        #region Explicit Mappings

        [TestMethod]
        public void TestBulkInsertStrictIsBypassedByExplicitMappings()
        {
            // Setup (would violate 'Strict': a subset, a different order, and an extra 'Nickname' column)
            Seed(10);
            UseBehavior(PostgreSqlBulkColumnMappingsBehavior.Strict);
            using var table = LoadDataTable($"SELECT \"ColumnInteger\", 'Extra' AS \"Nickname\", \"ColumnText\" FROM \"{TableName}\";");
            var mappings = CreateMappings("ColumnText", "ColumnInteger");

            using var connection = new NpgsqlConnection(Database.ConnectionString);

            // Act
            var result = connection.BulkInsert(TableName, table, mappings: mappings);

            // Assert
            Assert.AreEqual(10, result);
            Assert.AreEqual(20, CountAll());
        }

        [TestMethod]
        public void TestBulkInsertStrictIsBypassedByExplicitMappingsForEntities()
        {
            // Setup
            UseBehavior(PostgreSqlBulkColumnMappingsBehavior.Strict);
            var entities = Helper.CreateBulkOperationLightIdentityTables(5);
            var mappings = CreateMappings("ColumnText", "ColumnInteger");

            using var connection = new NpgsqlConnection(Database.ConnectionString);

            // Act
            var result = connection.BulkInsert(TableName, entities, mappings);

            // Assert
            Assert.AreEqual(entities.Count, result);
            Assert.AreEqual(entities.Count, CountAll());
        }

        [TestMethod]
        public void ThrowExceptionOnBulkInsertStrictIfTheExplicitMappingsAreEmpty()
        {
            // Setup (an empty mappings collection is the same as not passing the mappings)
            UseBehavior(PostgreSqlBulkColumnMappingsBehavior.Strict);
            var entities = Helper.CreateBulkOperationLightIdentityTables(3);

            using var connection = new NpgsqlConnection(Database.ConnectionString);

            // Act/Assert
            Assert.Throws<PostgreSqlBulkColumnMappingsException>(() =>
                connection.BulkInsert(TableName, entities, new List<PostgreSqlBulkInsertMapItem>()));
            Assert.AreEqual(0, CountAll());
        }

        #endregion

        #region BulkMerge

        [TestMethod]
        public void TestBulkMergeStrictBypassForSubsetEntities()
        {
            // Setup
            var existing = Seed(10);
            UseBehavior(PostgreSqlBulkColumnMappingsBehavior.StrictBypass);
            existing.ForEach(e => e.ColumnText = $"Merged-{e.Id}");

            using var connection = new NpgsqlConnection(Database.ConnectionString);

            // Act
            var result = connection.BulkMerge(TableName, existing);

            // Assert
            Assert.AreEqual(existing.Count, result);
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(TableName).ToDictionary(e => e.Id);
            existing.ForEach(e => Assert.AreEqual($"Merged-{e.Id}", queryResult[e.Id].ColumnText));
        }

        [TestMethod]
        public async Task ThrowExceptionOnBulkMergeAsyncStrictForSubsetEntities()
        {
            // Setup
            var existing = Seed(5);
            UseBehavior(PostgreSqlBulkColumnMappingsBehavior.Strict);
            existing.ForEach(e => e.ColumnText = "Changed");

            using var connection = new NpgsqlConnection(Database.ConnectionString);

            // Act/Assert
            await Assert.ThrowsAsync<PostgreSqlBulkColumnMappingsException>(() => connection.BulkMergeAsync(TableName, existing));

            // Assert (nothing was changed)
            Assert.IsFalse(connection.QueryAll<BulkOperationLightIdentityTable>(TableName).Any(e => e.ColumnText == "Changed"));
        }

        #endregion

        #region BulkUpdate

        [TestMethod]
        public void TestBulkUpdateStrictBypassForDataTableWithSubsetColumns()
        {
            // Setup
            Seed(10);
            UseBehavior(PostgreSqlBulkColumnMappingsBehavior.StrictBypass);
            using var table = LoadDataTable($"SELECT \"Id\", 'Updated' AS \"ColumnText\" FROM \"{TableName}\";");

            using var connection = new NpgsqlConnection(Database.ConnectionString);

            // Act
            var result = connection.BulkUpdate(TableName, table);

            // Assert (only the supplied column is changed)
            Assert.AreEqual(10, result);
            var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(TableName).ToList();
            Assert.IsTrue(queryResult.All(e => e.ColumnText == "Updated" && e.ColumnInteger != null));
        }

        [TestMethod]
        public void ThrowExceptionOnBulkUpdateStrictBypassForDataTableIfASourceColumnDoesNotExist()
        {
            // Setup
            Seed(5);
            UseBehavior(PostgreSqlBulkColumnMappingsBehavior.StrictBypass);
            using var table = LoadDataTable($"SELECT \"Id\", 'Changed' AS \"ColumnText\", 'Extra' AS \"Nickname\" FROM \"{TableName}\";");

            using var connection = new NpgsqlConnection(Database.ConnectionString);

            // Act/Assert
            Assert.Throws<PostgreSqlBulkColumnMappingsException>(() => connection.BulkUpdate(TableName, table));

            // Assert (nothing was changed)
            Assert.IsFalse(connection.QueryAll<BulkOperationLightIdentityTable>(TableName).Any(e => e.ColumnText == "Changed"));
        }

        #endregion

        #region BulkDelete

        [TestMethod]
        public void TestBulkDeleteStrictBypassForDataTableWithTheKeyOnly()
        {
            // Setup
            Seed(10);
            UseBehavior(PostgreSqlBulkColumnMappingsBehavior.StrictBypass);
            using var table = LoadDataTable($"SELECT \"Id\" FROM \"{TableName}\" ORDER BY \"Id\" LIMIT 4;");

            using var connection = new NpgsqlConnection(Database.ConnectionString);

            // Act
            var result = connection.BulkDelete(TableName, table);

            // Assert
            Assert.AreEqual(4, result);
            Assert.AreEqual(6, CountAll());
        }

        [TestMethod]
        public void ThrowExceptionOnBulkDeleteStrictForDataTableWithTheKeyOnly()
        {
            // Setup
            Seed(5);
            UseBehavior(PostgreSqlBulkColumnMappingsBehavior.Strict);
            using var table = LoadDataTable($"SELECT \"Id\" FROM \"{TableName}\";");

            using var connection = new NpgsqlConnection(Database.ConnectionString);

            // Act/Assert
            Assert.Throws<PostgreSqlBulkColumnMappingsException>(() => connection.BulkDelete(TableName, table));
            Assert.AreEqual(5, CountAll());
        }

        [TestMethod]
        public void TestBulkDeleteByKeyIsNotAffectedByTheStrictBehavior()
        {
            // Setup (the source of a delete-by-key is a list of keys, not a set of columns)
            var existing = Seed(10);
            UseBehavior(PostgreSqlBulkColumnMappingsBehavior.Strict);

            using var connection = new NpgsqlConnection(Database.ConnectionString);

            // Act
            var result = connection.BulkDeleteByKey(TableName, existing.Take(3).Select(e => e.Id));

            // Assert
            Assert.AreEqual(3, result);
            Assert.AreEqual(7, CountAll());
        }

        #endregion
    }
}
