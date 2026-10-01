#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.Enumerations.SqlServer;
using RepoDb.Exceptions;
using RepoDb.IntegrationTests.Setup;
using RepoDb.SqlServer.BulkOperations.IntegrationTests.Models;
using RepoDb.StatementBuilders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.SqlServer.BulkOperations.IntegrationTests.Operations
{
    /// <summary>
    /// Tests for the <see cref="SqlServerBulkOperationsDbSetting.BulkColumnMappingsBehavior"/> setting
    /// (https://github.com/mikependon/RepoDB/issues/1360). The behavior is only applied when no explicit
    /// mappings were passed to the operation.
    /// </summary>
    [TestClass]
    [DoNotParallelize]
    public class MicrosoftSqlConnectionBulkColumnMappingsBehaviorTest
    {
        private const string TableName = "[dbo].[BulkOperationIdentityTable]";

        /// <summary>
        /// The columns of the [dbo].[BulkOperationIdentityTable] table, in the order of the table.
        /// </summary>
        private static readonly string[] AllColumns =
        [
            nameof(BulkOperationIdentityTable.Id),
            nameof(BulkOperationIdentityTable.RowGuid),
            nameof(BulkOperationIdentityTable.ColumnBit),
            nameof(BulkOperationIdentityTable.ColumnDateTime),
            nameof(BulkOperationIdentityTable.ColumnDateTime2),
            nameof(BulkOperationIdentityTable.ColumnDecimal),
            nameof(BulkOperationIdentityTable.ColumnFloat),
            nameof(BulkOperationIdentityTable.ColumnInt),
            nameof(BulkOperationIdentityTable.ColumnNVarChar)
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
            // The setting is global, so always revert to the default SqlServerDbSetting for the other test classes
            GlobalConfiguration
                .Setup()
                .UseSqlServer(new SqlServerDbSetting());
            Database.Cleanup();
        }

        #region Helpers

        private static void UseBehavior(SqlServerBulkColumnMappingsBehavior behavior) =>
            GlobalConfiguration
                .Setup()
                .UseSqlServer(new SqlServerBulkOperationsDbSetting
                {
                    BulkColumnMappingsBehavior = behavior
                });

        /// <summary>
        /// Creates a <see cref="DataTable"/> with the given columns (in the given order) from the entities. A column that is
        /// not a property of <see cref="BulkOperationIdentityTable"/> is added as a text column with a dummy value.
        /// </summary>
        private static DataTable CreateDataTable(IEnumerable<BulkOperationIdentityTable> entities,
            params string[] columns)
        {
            var table = new DataTable(TableName);
            var properties = columns
                .Select(column => typeof(BulkOperationIdentityTable).GetProperty(column))
                .ToList();

            for (var i = 0; i < columns.Length; i++)
            {
                var type = properties[i]?.PropertyType ?? typeof(string);
                table.Columns.Add(columns[i], Nullable.GetUnderlyingType(type) ?? type);
            }

            foreach (var entity in entities)
            {
                var row = table.NewRow();
                for (var i = 0; i < columns.Length; i++)
                {
                    row[i] = properties[i] != null ? (properties[i].GetValue(entity) ?? DBNull.Value) : "Extra";
                }
                table.Rows.Add(row);
            }

            return table;
        }

        private static long CountAll()
        {
            using var connection = new SqlConnection(Database.ConnectionString);
            return connection.CountAll<BulkOperationIdentityTable>();
        }

        private static List<BulkOperationIdentityTable> InsertAndQueryAll(int count)
        {
            using var connection = new SqlConnection(Database.ConnectionString);
            connection.InsertAll(Helper.CreateBulkOperationIdentityTables(count));
            return connection.QueryAll<BulkOperationIdentityTable>().ToList();
        }

        private static List<SqlServerBulkInsertMapItem> CreateMappings(params string[] columns) =>
            columns.Select(column => new SqlServerBulkInsertMapItem(column, column)).ToList();

        #endregion

        #region Configuration

        [TestMethod]
        public void TestSqlServerBulkOperationsDbSettingDefaultIsAutomatic()
        {
            // Act
            var setting = new SqlServerBulkOperationsDbSetting();

            // Assert
            Assert.AreEqual(SqlServerBulkColumnMappingsBehavior.Automatic, setting.BulkColumnMappingsBehavior);
        }

        [TestMethod]
        public void TestSqlServerBulkOperationsDbSettingInheritsTheSqlServerDbSetting()
        {
            // Setup
            var expected = new SqlServerDbSetting();

            // Act
            var setting = new SqlServerBulkOperationsDbSetting();

            // Assert
            Assert.IsInstanceOfType<SqlServerDbSetting>(setting);
            Assert.AreEqual(expected.OpeningQuote, setting.OpeningQuote);
            Assert.AreEqual(expected.ClosingQuote, setting.ClosingQuote);
            Assert.AreEqual(expected.DefaultSchema, setting.DefaultSchema);
            Assert.AreEqual(expected.ParameterPrefix, setting.ParameterPrefix);
            Assert.AreEqual(expected.MaxParameterCount, setting.MaxParameterCount);
            Assert.AreEqual(expected.AreTableHintsSupported, setting.AreTableHintsSupported);
            Assert.AreEqual(expected.IsInsertAllBatchable, setting.IsInsertAllBatchable);
        }

        [TestMethod]
        public void TestUseSqlServerWithBulkOperationsDbSettingRegistersItAsTheDbSetting()
        {
            // Setup
            var setting = new SqlServerBulkOperationsDbSetting
            {
                BulkColumnMappingsBehavior = SqlServerBulkColumnMappingsBehavior.StrictBypass
            };

            // Act
            var result = GlobalConfiguration
                .Setup()
                .UseSqlServer(setting);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(SqlServerBootstrap.IsInitialized);
            Assert.AreSame(setting, DbSettingMapper.Get<SqlConnection>());
            Assert.IsInstanceOfType<SqlServerStatementBuilder>(StatementBuilderMapper.Get<SqlConnection>());
            using var connection = new SqlConnection(Database.ConnectionString);
            Assert.AreSame(setting, connection.GetDbSetting());
        }

        [TestMethod]
        public void TestUseSqlServerWithBulkOperationsDbSettingKeepsTheOperationsWorking()
        {
            // Setup
            UseBehavior(SqlServerBulkColumnMappingsBehavior.Strict);
            var entities = Helper.CreateBulkOperationIdentityTables(5);

            using var connection = new SqlConnection(Database.ConnectionString);

            // Act (the non-bulk operations use the inherited SQL Server values)
            connection.InsertAll(entities);
            var queryResult = connection.QueryAll<BulkOperationIdentityTable>().ToList();

            // Assert
            Assert.AreEqual(entities.Count, queryResult.Count);
        }

        [TestMethod]
        public void TestDefaultSqlServerDbSettingBehavesAsAutomatic()
        {
            // Setup (only UseSqlServer() was called, so the plain SqlServerDbSetting is registered)
            Assert.IsNotInstanceOfType<SqlServerBulkOperationsDbSetting>(DbSettingMapper.Get<SqlConnection>());
            var entities = Helper.CreateBulkOperationIdentityTables(5);
            using var table = CreateDataTable(entities,
                nameof(BulkOperationIdentityTable.ColumnInt),
                "Nickname",
                nameof(BulkOperationIdentityTable.RowGuid));

            using var connection = new SqlConnection(Database.ConnectionString);

            // Act
            var result = connection.BulkInsert(TableName, table);

            // Assert
            Assert.AreEqual(entities.Count, result);
        }

        [TestMethod]
        public void ThrowExceptionOnUseSqlServerIfTheBulkOperationsDbSettingIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() =>
                GlobalConfiguration
                    .Setup()
                    .UseSqlServer((SqlServerBulkOperationsDbSetting)null));
        }

        #endregion

        #region Automatic

        [TestMethod]
        public void TestBulkInsertAutomaticForDataTableWithSubsetExtraAndDifferentOrder()
        {
            // Setup (no 'Id', a different order, and an extra 'Nickname' column)
            var entities = Helper.CreateBulkOperationIdentityTables(10);
            using var table = CreateDataTable(entities,
                nameof(BulkOperationIdentityTable.ColumnNVarChar),
                "Nickname",
                nameof(BulkOperationIdentityTable.RowGuid),
                nameof(BulkOperationIdentityTable.ColumnInt));

            using var connection = new SqlConnection(Database.ConnectionString);

            // Act
            var result = connection.BulkInsert(TableName, table);

            // Assert
            Assert.AreEqual(entities.Count, result);
            var queryResult = connection.QueryAll<BulkOperationIdentityTable>().ToList();
            Assert.AreEqual(entities.Count, queryResult.Count);
            foreach (var entity in entities)
            {
                var item = queryResult.Single(e => e.RowGuid == entity.RowGuid);
                Assert.AreEqual(entity.ColumnInt, item.ColumnInt);
                Assert.AreEqual(entity.ColumnNVarChar, item.ColumnNVarChar);
                Assert.IsNull(item.ColumnDecimal);
            }
        }

        [TestMethod]
        public void TestBulkInsertAutomaticForEntitiesWithExtraFields()
        {
            // Setup
            var entities = Helper.CreateBulkOperationIdentityTables(10)
                .Select(e => new WithExtraFieldsBulkOperationIdentityTable
                {
                    RowGuid = e.RowGuid,
                    ColumnInt = e.ColumnInt,
                    ColumnNVarChar = e.ColumnNVarChar,
                    ExtraField = "Extra"
                })
                .ToList();

            using var connection = new SqlConnection(Database.ConnectionString);

            // Act
            var result = connection.BulkInsert(entities);

            // Assert
            Assert.AreEqual(entities.Count, result);
            Assert.AreEqual(entities.Count, CountAll());
        }

        [TestMethod]
        public void ThrowExceptionOnBulkInsertAutomaticIfNoColumnMatches()
        {
            // Setup
            var entities = Helper.CreateBulkOperationIdentityTables(3);
            using var table = CreateDataTable(entities, "Nickname", "Alias");

            using var connection = new SqlConnection(Database.ConnectionString);

            // Act/Assert (the behavior of the earlier versions)
            Assert.Throws<MissingFieldException>(() => connection.BulkInsert(TableName, table));
            Assert.AreEqual(0, CountAll());
        }

        #endregion

        #region Strict

        [TestMethod]
        public void TestBulkInsertStrictForEntitiesWithIdenticalColumns()
        {
            // Setup
            UseBehavior(SqlServerBulkColumnMappingsBehavior.Strict);
            var entities = Helper.CreateBulkOperationIdentityTables(10);

            using var connection = new SqlConnection(Database.ConnectionString);

            // Act
            var result = connection.BulkInsert(entities);

            // Assert
            Assert.AreEqual(entities.Count, result);
            Assert.AreEqual(entities.Count, CountAll());
        }

        [TestMethod]
        public void TestBulkInsertStrictForMappedEntitiesWithIdenticalColumns()
        {
            // Setup (the property names differ, but the mapped column names are identical and in the same order)
            UseBehavior(SqlServerBulkColumnMappingsBehavior.Strict);
            var entities = Helper.CreateBulkOperationMappedIdentityTables(10);

            using var connection = new SqlConnection(Database.ConnectionString);

            // Act
            var result = connection.BulkInsert(entities);

            // Assert
            Assert.AreEqual(entities.Count, result);
            Assert.AreEqual(entities.Count, CountAll());
        }

        [TestMethod]
        public void TestBulkInsertStrictForDataTableWithIdenticalColumns()
        {
            // Setup
            UseBehavior(SqlServerBulkColumnMappingsBehavior.Strict);
            var entities = Helper.CreateBulkOperationIdentityTables(10);
            using var table = CreateDataTable(entities, AllColumns);

            using var connection = new SqlConnection(Database.ConnectionString);

            // Act
            var result = connection.BulkInsert(TableName, table);

            // Assert
            Assert.AreEqual(entities.Count, result);
            Assert.AreEqual(entities.Count, CountAll());
        }

        [TestMethod]
        public void TestBulkInsertStrictForDataReaderWithIdenticalColumns()
        {
            // Setup
            UseBehavior(SqlServerBulkColumnMappingsBehavior.Strict);
            var existing = InsertAndQueryAll(10);

            using var sourceConnection = new SqlConnection(Database.ConnectionString);
            using var reader = sourceConnection.ExecuteReader($"SELECT * FROM {TableName};");
            using var connection = new SqlConnection(Database.ConnectionString);

            // Act
            var result = connection.BulkInsert(TableName, reader);

            // Assert
            Assert.AreEqual(existing.Count, result);
            Assert.AreEqual(existing.Count * 2, CountAll());
        }

        [TestMethod]
        public void ThrowExceptionOnBulkInsertStrictForDataTableIfADestinationColumnIsMissing()
        {
            // Setup (no 'Id')
            UseBehavior(SqlServerBulkColumnMappingsBehavior.Strict);
            var entities = Helper.CreateBulkOperationIdentityTables(3);
            using var table = CreateDataTable(entities, AllColumns.Skip(1).ToArray());

            using var connection = new SqlConnection(Database.ConnectionString);

            // Act
            var exception = Assert.Throws<SqlServerBulkColumnMappingsException>(() => connection.BulkInsert(TableName, table));

            // Assert
            Assert.AreEqual(SqlServerBulkColumnMappingsBehavior.Strict, exception.Behavior);
            CollectionAssert.AreEqual(new[] { nameof(BulkOperationIdentityTable.Id) }, exception.DestinationColumnsNotInSource.ToArray());
            Assert.AreEqual(0, exception.SourceColumnsNotInDestination.Count);
            Assert.IsFalse(exception.IsOrderMismatch);
            StringAssert.Contains(exception.Message, "'Id'");
            Assert.AreEqual(0, CountAll());
        }

        [TestMethod]
        public void ThrowExceptionOnBulkInsertStrictForDataTableIfTheOrderIsDifferent()
        {
            // Setup (all the columns, but 'ColumnInt' and 'ColumnNVarChar' are swapped)
            UseBehavior(SqlServerBulkColumnMappingsBehavior.Strict);
            var entities = Helper.CreateBulkOperationIdentityTables(3);
            var columns = AllColumns.ToArray();
            (columns[7], columns[8]) = (columns[8], columns[7]);
            using var table = CreateDataTable(entities, columns);

            using var connection = new SqlConnection(Database.ConnectionString);

            // Act
            var exception = Assert.Throws<SqlServerBulkColumnMappingsException>(() => connection.BulkInsert(TableName, table));

            // Assert
            Assert.IsTrue(exception.IsOrderMismatch);
            Assert.AreEqual(0, exception.SourceColumnsNotInDestination.Count);
            Assert.AreEqual(0, exception.DestinationColumnsNotInSource.Count);
            StringAssert.Contains(exception.Message, "same order");
            Assert.AreEqual(0, CountAll());
        }

        [TestMethod]
        public void ThrowExceptionOnBulkInsertStrictForEntitiesWithExtraFields()
        {
            // Setup
            UseBehavior(SqlServerBulkColumnMappingsBehavior.Strict);
            var entities = new[]
            {
                new WithExtraFieldsBulkOperationIdentityTable { RowGuid = Guid.NewGuid(), ExtraField = "Extra" }
            };

            using var connection = new SqlConnection(Database.ConnectionString);

            // Act
            var exception = Assert.Throws<SqlServerBulkColumnMappingsException>(() => connection.BulkInsert(entities));

            // Assert
            CollectionAssert.Contains(exception.SourceColumnsNotInDestination.ToList(), nameof(WithExtraFieldsBulkOperationIdentityTable.ExtraField));
            Assert.AreEqual(0, CountAll());
        }

        [TestMethod]
        public void ThrowExceptionOnBulkInsertStrictForDataReaderWithSubsetColumns()
        {
            // Setup
            UseBehavior(SqlServerBulkColumnMappingsBehavior.Strict);
            InsertAndQueryAll(3);

            using var sourceConnection = new SqlConnection(Database.ConnectionString);
            using var reader = sourceConnection.ExecuteReader($"SELECT [RowGuid], [ColumnInt] FROM {TableName};");
            using var connection = new SqlConnection(Database.ConnectionString);

            // Act
            var exception = Assert.Throws<SqlServerBulkColumnMappingsException>(() => connection.BulkInsert(TableName, reader));

            // Assert
            Assert.AreEqual(7, exception.DestinationColumnsNotInSource.Count);
            Assert.AreEqual(3, CountAll());
        }

        [TestMethod]
        public async Task ThrowExceptionOnBulkInsertAsyncStrictForDataTableIfADestinationColumnIsMissing()
        {
            // Setup
            UseBehavior(SqlServerBulkColumnMappingsBehavior.Strict);
            var entities = Helper.CreateBulkOperationIdentityTables(3);
            using var table = CreateDataTable(entities, AllColumns.Skip(1).ToArray());

            using var connection = new SqlConnection(Database.ConnectionString);

            // Act/Assert
            await Assert.ThrowsAsync<SqlServerBulkColumnMappingsException>(() => connection.BulkInsertAsync(TableName, table));
            Assert.AreEqual(0, CountAll());
        }

        #endregion

        #region StrictBypass

        [TestMethod]
        public void TestBulkInsertStrictBypassForDataTableWithSubsetInDifferentOrder()
        {
            // Setup (no 'Id' and a different order: both are allowed)
            UseBehavior(SqlServerBulkColumnMappingsBehavior.StrictBypass);
            var entities = Helper.CreateBulkOperationIdentityTables(10);
            using var table = CreateDataTable(entities,
                nameof(BulkOperationIdentityTable.ColumnNVarChar),
                nameof(BulkOperationIdentityTable.RowGuid),
                nameof(BulkOperationIdentityTable.ColumnInt));

            using var connection = new SqlConnection(Database.ConnectionString);

            // Act
            var result = connection.BulkInsert(TableName, table);

            // Assert (the bypassed columns are left to their defaults)
            Assert.AreEqual(entities.Count, result);
            var queryResult = connection.QueryAll<BulkOperationIdentityTable>().ToList();
            Assert.AreEqual(entities.Count, queryResult.Count);
            foreach (var entity in entities)
            {
                var item = queryResult.Single(e => e.RowGuid == entity.RowGuid);
                Assert.IsTrue(item.Id > 0);
                Assert.AreEqual(entity.ColumnInt, item.ColumnInt);
                Assert.AreEqual(entity.ColumnNVarChar, item.ColumnNVarChar);
                Assert.IsNull(item.ColumnDateTime);
            }
        }

        [TestMethod]
        public void TestBulkInsertStrictBypassForDataReaderWithSubsetColumns()
        {
            // Setup
            UseBehavior(SqlServerBulkColumnMappingsBehavior.StrictBypass);
            InsertAndQueryAll(5);

            using var sourceConnection = new SqlConnection(Database.ConnectionString);
            using var reader = sourceConnection.ExecuteReader($"SELECT [ColumnInt], [RowGuid] FROM {TableName};");
            using var connection = new SqlConnection(Database.ConnectionString);

            // Act
            var result = connection.BulkInsert(TableName, reader);

            // Assert
            Assert.AreEqual(5, result);
            Assert.AreEqual(10, CountAll());
        }

        [TestMethod]
        public void ThrowExceptionOnBulkInsertStrictBypassForDataTableIfASourceColumnDoesNotExist()
        {
            // Setup (the scenario of the issue: 'Email', 'Name', 'Nickname' => here 'ColumnNVarChar', 'RowGuid', 'Nickname')
            UseBehavior(SqlServerBulkColumnMappingsBehavior.StrictBypass);
            var entities = Helper.CreateBulkOperationIdentityTables(3);
            using var table = CreateDataTable(entities,
                nameof(BulkOperationIdentityTable.ColumnNVarChar),
                nameof(BulkOperationIdentityTable.RowGuid),
                "Nickname");

            using var connection = new SqlConnection(Database.ConnectionString);

            // Act
            var exception = Assert.Throws<SqlServerBulkColumnMappingsException>(() => connection.BulkInsert(TableName, table));

            // Assert
            Assert.AreEqual(SqlServerBulkColumnMappingsBehavior.StrictBypass, exception.Behavior);
            Assert.AreEqual(TableName, exception.TableName);
            CollectionAssert.AreEqual(new[] { "Nickname" }, exception.SourceColumnsNotInDestination.ToArray());
            Assert.AreEqual(0, exception.DestinationColumnsNotInSource.Count);
            StringAssert.Contains(exception.Message, "'Nickname'");
            StringAssert.Contains(exception.Message, TableName);
            Assert.AreEqual(0, CountAll());
        }

        [TestMethod]
        public void ThrowExceptionOnBulkInsertStrictBypassForEntitiesWithExtraFields()
        {
            // Setup
            UseBehavior(SqlServerBulkColumnMappingsBehavior.StrictBypass);
            var entities = new[]
            {
                new WithExtraFieldsBulkOperationIdentityTable { RowGuid = Guid.NewGuid(), ExtraField = "Extra" }
            };

            using var connection = new SqlConnection(Database.ConnectionString);

            // Act/Assert
            var exception = Assert.Throws<SqlServerBulkColumnMappingsException>(() => connection.BulkInsert(entities));
            CollectionAssert.Contains(exception.SourceColumnsNotInDestination.ToList(), nameof(WithExtraFieldsBulkOperationIdentityTable.ExtraField));
            Assert.AreEqual(0, CountAll());
        }

        #endregion

        #region Explicit Mappings

        [TestMethod]
        public void TestBulkInsertStrictIsBypassedByExplicitMappings()
        {
            // Setup (would violate 'Strict': no 'Id', a different order, and an extra 'Nickname' column)
            UseBehavior(SqlServerBulkColumnMappingsBehavior.Strict);
            var entities = Helper.CreateBulkOperationIdentityTables(10);
            using var table = CreateDataTable(entities,
                nameof(BulkOperationIdentityTable.ColumnInt),
                "Nickname",
                nameof(BulkOperationIdentityTable.RowGuid));
            var mappings = CreateMappings(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.ColumnInt));

            using var connection = new SqlConnection(Database.ConnectionString);

            // Act
            var result = connection.BulkInsert(TableName, table, mappings: mappings);

            // Assert
            Assert.AreEqual(entities.Count, result);
            Assert.AreEqual(entities.Count, CountAll());
        }

        [TestMethod]
        public void TestBulkInsertStrictBypassIsBypassedByExplicitMappingsForEntities()
        {
            // Setup
            UseBehavior(SqlServerBulkColumnMappingsBehavior.StrictBypass);
            var entities = new[]
            {
                new WithExtraFieldsBulkOperationIdentityTable { RowGuid = Guid.NewGuid(), ColumnInt = 1, ExtraField = "Extra" },
                new WithExtraFieldsBulkOperationIdentityTable { RowGuid = Guid.NewGuid(), ColumnInt = 2, ExtraField = "Extra" }
            };
            var mappings = CreateMappings(nameof(BulkOperationIdentityTable.RowGuid), nameof(BulkOperationIdentityTable.ColumnInt));

            using var connection = new SqlConnection(Database.ConnectionString);

            // Act
            var result = connection.BulkInsert(entities, mappings);

            // Assert
            Assert.AreEqual(entities.Length, result);
            Assert.AreEqual(entities.Length, CountAll());
        }

        [TestMethod]
        public void ThrowExceptionOnBulkInsertStrictIfTheExplicitMappingsAreEmpty()
        {
            // Setup (an empty mappings collection is the same as not passing the mappings)
            UseBehavior(SqlServerBulkColumnMappingsBehavior.Strict);
            var entities = Helper.CreateBulkOperationIdentityTables(3);
            using var table = CreateDataTable(entities, AllColumns.Skip(1).ToArray());

            using var connection = new SqlConnection(Database.ConnectionString);

            // Act/Assert
            Assert.Throws<SqlServerBulkColumnMappingsException>(() =>
                connection.BulkInsert(TableName, table, mappings: new List<SqlServerBulkInsertMapItem>()));
            Assert.AreEqual(0, CountAll());
        }

        #endregion

        #region BulkMerge

        [TestMethod]
        public void TestBulkMergeStrictBypassForDataTableWithSubsetColumns()
        {
            // Setup
            var existing = InsertAndQueryAll(10);
            UseBehavior(SqlServerBulkColumnMappingsBehavior.StrictBypass);
            existing.ForEach(e => e.ColumnInt = e.ColumnInt.GetValueOrDefault() + 1000);
            using var table = CreateDataTable(existing,
                nameof(BulkOperationIdentityTable.ColumnInt),
                nameof(BulkOperationIdentityTable.Id));

            using var connection = new SqlConnection(Database.ConnectionString);

            // Act
            var result = connection.BulkMerge(TableName, table);

            // Assert (only the supplied column is changed)
            Assert.AreEqual(existing.Count, result);
            var queryResult = connection.QueryAll<BulkOperationIdentityTable>().ToDictionary(e => e.Id);
            foreach (var entity in existing)
            {
                Assert.AreEqual(entity.ColumnInt, queryResult[entity.Id].ColumnInt);
                Assert.AreEqual(entity.ColumnNVarChar, queryResult[entity.Id].ColumnNVarChar);
            }
        }

        [TestMethod]
        public async Task TestBulkMergeAsyncStrictForEntitiesWithIdenticalColumns()
        {
            // Setup
            var existing = InsertAndQueryAll(10);
            UseBehavior(SqlServerBulkColumnMappingsBehavior.Strict);
            existing.ForEach(e => e.ColumnNVarChar = $"Merged:{e.Id}");

            using var connection = new SqlConnection(Database.ConnectionString);

            // Act
            var result = await connection.BulkMergeAsync(existing);

            // Assert
            Assert.AreEqual(existing.Count, result);
            var queryResult = connection.QueryAll<BulkOperationIdentityTable>().ToDictionary(e => e.Id);
            existing.ForEach(e => Assert.AreEqual($"Merged:{e.Id}", queryResult[e.Id].ColumnNVarChar));
        }

        [TestMethod]
        public void ThrowExceptionOnBulkMergeStrictForDataTableWithSubsetColumns()
        {
            // Setup
            var existing = InsertAndQueryAll(5);
            UseBehavior(SqlServerBulkColumnMappingsBehavior.Strict);
            existing.ForEach(e => e.ColumnInt = -1);
            using var table = CreateDataTable(existing,
                nameof(BulkOperationIdentityTable.Id),
                nameof(BulkOperationIdentityTable.ColumnInt));

            using var connection = new SqlConnection(Database.ConnectionString);

            // Act/Assert
            Assert.Throws<SqlServerBulkColumnMappingsException>(() => connection.BulkMerge(TableName, table));

            // Assert (nothing was changed)
            Assert.IsFalse(connection.QueryAll<BulkOperationIdentityTable>().Any(e => e.ColumnInt == -1));
        }

        #endregion

        #region BulkUpdate

        [TestMethod]
        public void TestBulkUpdateStrictBypassForDataTableWithSubsetColumns()
        {
            // Setup
            var existing = InsertAndQueryAll(10);
            UseBehavior(SqlServerBulkColumnMappingsBehavior.StrictBypass);
            existing.ForEach(e => e.ColumnNVarChar = $"Updated:{e.Id}");
            using var table = CreateDataTable(existing,
                nameof(BulkOperationIdentityTable.ColumnNVarChar),
                nameof(BulkOperationIdentityTable.Id));

            using var connection = new SqlConnection(Database.ConnectionString);

            // Act
            var result = connection.BulkUpdate(TableName, table);

            // Assert (only the supplied column is changed)
            Assert.AreEqual(existing.Count, result);
            var queryResult = connection.QueryAll<BulkOperationIdentityTable>().ToDictionary(e => e.Id);
            foreach (var entity in existing)
            {
                Assert.AreEqual($"Updated:{entity.Id}", queryResult[entity.Id].ColumnNVarChar);
                Assert.AreEqual(entity.ColumnInt, queryResult[entity.Id].ColumnInt);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnBulkUpdateStrictBypassForDataTableIfASourceColumnDoesNotExist()
        {
            // Setup
            var existing = InsertAndQueryAll(5);
            UseBehavior(SqlServerBulkColumnMappingsBehavior.StrictBypass);
            existing.ForEach(e => e.ColumnNVarChar = "Changed");
            using var table = CreateDataTable(existing,
                nameof(BulkOperationIdentityTable.Id),
                nameof(BulkOperationIdentityTable.ColumnNVarChar),
                "Nickname");

            using var connection = new SqlConnection(Database.ConnectionString);

            // Act/Assert
            var exception = Assert.Throws<SqlServerBulkColumnMappingsException>(() => connection.BulkUpdate(TableName, table));
            CollectionAssert.AreEqual(new[] { "Nickname" }, exception.SourceColumnsNotInDestination.ToArray());

            // Assert (nothing was changed)
            Assert.IsFalse(connection.QueryAll<BulkOperationIdentityTable>().Any(e => e.ColumnNVarChar == "Changed"));
        }

        [TestMethod]
        public void ThrowExceptionOnBulkUpdateStrictForDataReaderWithSubsetColumns()
        {
            // Setup
            InsertAndQueryAll(3);
            UseBehavior(SqlServerBulkColumnMappingsBehavior.Strict);

            using var sourceConnection = new SqlConnection(Database.ConnectionString);
            using var reader = sourceConnection.ExecuteReader($"SELECT [Id], [ColumnInt] FROM {TableName};");
            using var connection = new SqlConnection(Database.ConnectionString);

            // Act/Assert
            Assert.Throws<SqlServerBulkColumnMappingsException>(() => connection.BulkUpdate(TableName, reader));
        }

        #endregion

        #region BulkDelete

        [TestMethod]
        public void TestBulkDeleteStrictBypassForDataTableWithTheKeyOnly()
        {
            // Setup
            var existing = InsertAndQueryAll(10);
            UseBehavior(SqlServerBulkColumnMappingsBehavior.StrictBypass);
            using var table = CreateDataTable(existing.Take(4), nameof(BulkOperationIdentityTable.Id));

            using var connection = new SqlConnection(Database.ConnectionString);

            // Act
            var result = connection.BulkDelete(TableName, table);

            // Assert
            Assert.AreEqual(4, result);
            Assert.AreEqual(6, CountAll());
        }

        [TestMethod]
        public void ThrowExceptionOnBulkDeleteStrictBypassForDataTableIfASourceColumnDoesNotExist()
        {
            // Setup
            var existing = InsertAndQueryAll(5);
            UseBehavior(SqlServerBulkColumnMappingsBehavior.StrictBypass);
            using var table = CreateDataTable(existing, nameof(BulkOperationIdentityTable.Id), "Nickname");

            using var connection = new SqlConnection(Database.ConnectionString);

            // Act/Assert
            Assert.Throws<SqlServerBulkColumnMappingsException>(() => connection.BulkDelete(TableName, table));
            Assert.AreEqual(5, CountAll());
        }

        [TestMethod]
        public void ThrowExceptionOnBulkDeleteStrictForDataTableWithTheKeyOnly()
        {
            // Setup
            var existing = InsertAndQueryAll(5);
            UseBehavior(SqlServerBulkColumnMappingsBehavior.Strict);
            using var table = CreateDataTable(existing, nameof(BulkOperationIdentityTable.Id));

            using var connection = new SqlConnection(Database.ConnectionString);

            // Act/Assert
            Assert.Throws<SqlServerBulkColumnMappingsException>(() => connection.BulkDelete(TableName, table));
            Assert.AreEqual(5, CountAll());
        }

        [TestMethod]
        public void TestBulkDeleteByKeyIsNotAffectedByTheStrictBehavior()
        {
            // Setup (the source of a delete-by-key is a list of keys, not a set of columns)
            var existing = InsertAndQueryAll(10);
            UseBehavior(SqlServerBulkColumnMappingsBehavior.Strict);

            using var connection = new SqlConnection(Database.ConnectionString);

            // Act
            var result = connection.BulkDeleteByKey(TableName, existing.Take(3).Select(e => e.Id));

            // Assert
            Assert.AreEqual(3, result);
            Assert.AreEqual(7, CountAll());
        }

        #endregion
    }
}
