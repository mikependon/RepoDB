#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using ClickHouse.Driver.ADO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.ClickHouse.BulkOperations.IntegrationTests.Models;
using RepoDb.DbSettings;
using RepoDb.Enumerations.ClickHouse;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Setup;

namespace RepoDb.ClickHouse.BulkOperations.IntegrationTests.Operations
{
    /// <summary>
    /// Tests for the <see cref="ClickHouseBulkDbSetting.BulkColumnMappingsBehavior"/> setting
    /// (https://github.com/mikependon/RepoDB/issues/1360). The behavior is only applied when no explicit
    /// mappings were passed to the operation. The destination columns are read from the <see cref="DbFieldCache"/>
    /// (in the order of the table columns), so the tests do not depend on the column list of the table.
    /// </summary>
    [TestClass]
    [DoNotParallelize]
    public class ClickHouseBulkColumnMappingsBehaviorTest
    {
        private const string UnknownColumn = "UnknownColumn";

        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            // The setting is global, so always revert to the default one for the other test classes
            GlobalConfiguration
                .Setup()
                .UseClickHouse(new ClickHouseBulkDbSetting { IsWaitForMutationsEnabled = true });
            Database.Cleanup();
        }

        #region Helpers

        private static string TableName =>
            ClassMappedNameCache.Get<BulkOperationIdentityTable>();

        private static ClickHouseConnection CreateConnection() =>
            new ClickHouseConnection(Database.ConnectionString);

        private static void UseBehavior(ClickHouseBulkColumnMappingsBehavior behavior) =>
            GlobalConfiguration
                .Setup()
                .UseClickHouse(new ClickHouseBulkDbSetting
                {
                    IsWaitForMutationsEnabled = true,
                    BulkColumnMappingsBehavior = behavior
                });

        /// <summary>
        /// Gets the columns of the destination table, in the order of the table columns.
        /// </summary>
        private static List<string> GetDestinationColumns(ClickHouseConnection connection) =>
            DbFieldCache.Get(connection, TableName, null)
                .GetItems()
                .Select(dbField => dbField.Name)
                .ToList();

        private static string GetKeyColumn(ClickHouseConnection connection)
        {
            var dbFields = DbFieldCache.Get(connection, TableName, null);
            return (dbFields.GetPrimary() ?? dbFields.GetIdentity()).Name;
        }

        private static int Seed(ClickHouseConnection connection,
            int count = 10)
        {
            connection.InsertAll(Helper.CreateBulkOperationIdentityTables(count));
            return count;
        }

        private static long CountRows(ClickHouseConnection connection) =>
            connection.CountAll<BulkOperationIdentityTable>();

        /// <summary>
        /// Creates an in-memory <see cref="DataTable"/> with the given columns and a single row. It is only used by the
        /// tests in which the operation must throw before any data is written.
        /// </summary>
        private static DataTable CreateDataTable(IEnumerable<string> columns)
        {
            var table = new DataTable();
            foreach (var column in columns)
            {
                table.Columns.Add(column, typeof(object));
            }
            table.Rows.Add(table.NewRow());
            return table;
        }

        /// <summary>
        /// Creates a single dynamic entity with the given properties (in the given order).
        /// </summary>
        private static List<ExpandoObject> CreateEntities(IEnumerable<string> properties)
        {
            var entity = new ExpandoObject();
            var dictionary = (IDictionary<string, object>)entity;
            foreach (var property in properties)
            {
                dictionary[property] = null;
            }
            return [entity];
        }

        /// <summary>
        /// Loads the rows of the table (SELECT *, so the columns are in the order of the table columns).
        /// </summary>
        private static DataTable LoadDataTable(ClickHouseConnection connection)
        {
            var table = new DataTable();
            using (var reader = connection.ExecuteReader($"SELECT * FROM {TableName.AsQuoted(true, connection.GetDbSetting())}"))
            {
                table.Load(reader);
            }
            foreach (DataColumn column in table.Columns)
            {
                column.ReadOnly = false;
            }
            return table;
        }

        private static List<string> GetColumns(DataTable table) =>
            table.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToList();

        #endregion

        #region DbSetting

        [TestMethod]
        public void TestBulkColumnMappingsBehaviorDefaultValueIsAutomatic()
        {
            // Act
            var setting = new ClickHouseBulkDbSetting();

            // Assert
            Assert.AreEqual(ClickHouseBulkColumnMappingsBehavior.Automatic, setting.BulkColumnMappingsBehavior);
            Assert.IsInstanceOfType<ClickHouseDbSetting>(setting);
        }

        [TestMethod]
        public void TestBulkColumnMappingsBehaviorSettingIsRegistered()
        {
            // Act
            UseBehavior(ClickHouseBulkColumnMappingsBehavior.StrictBypass);

            // Assert
            using var connection = CreateConnection();
            var setting = connection.GetDbSetting() as ClickHouseBulkDbSetting;
            Assert.IsNotNull(setting);
            Assert.AreEqual(ClickHouseBulkColumnMappingsBehavior.StrictBypass, setting.BulkColumnMappingsBehavior);
        }

        #endregion

        #region DbHelper (column order)

        [TestMethod]
        public void TestDbFieldsAreInTheOrderOfTheTableColumns()
        {
            // Setup
            using var connection = CreateConnection();
            Seed(connection, 1);

            // Act
            var columns = GetColumns(LoadDataTable(connection));

            // Assert (SELECT * returns the columns in the order of the table)
            CollectionAssert.AreEqual(columns, GetDestinationColumns(connection));
        }

        #endregion

        #region Strict

        [TestMethod]
        public void TestStrictBulkInsertForDataTableWithAllColumnsInOrder()
        {
            // Setup
            using var connection = CreateConnection();
            var count = Seed(connection);
            var table = LoadDataTable(connection);
            connection.DeleteAll<BulkOperationIdentityTable>();
            UseBehavior(ClickHouseBulkColumnMappingsBehavior.Strict);

            // Act
            var result = connection.BulkInsert(TableName, table);

            // Assert
            Assert.AreEqual(count, result);
            Assert.AreEqual(count, CountRows(connection));
        }

        [TestMethod]
        public void TestStrictBulkUpdateForDataTableWithAllColumnsInOrder()
        {
            // Setup
            using var connection = CreateConnection();
            var count = Seed(connection);
            var table = LoadDataTable(connection);
            UseBehavior(ClickHouseBulkColumnMappingsBehavior.Strict);

            // Act
            var result = connection.BulkUpdate(TableName, table);

            // Assert
            Assert.AreEqual(count, result);
        }

        [TestMethod]
        public void TestStrictBulkDeleteForDataTableWithAllColumnsInOrder()
        {
            // Setup
            using var connection = CreateConnection();
            var count = Seed(connection);
            var table = LoadDataTable(connection);
            UseBehavior(ClickHouseBulkColumnMappingsBehavior.Strict);

            // Act
            var result = connection.BulkDelete(TableName, table);

            // Assert
            Assert.AreEqual(count, result);
            Assert.AreEqual(0, CountRows(connection));
        }

        [TestMethod]
        public void ThrowExceptionOnStrictBulkInsertForDataTableIfTheOrderIsDifferent()
        {
            // Setup
            using var connection = CreateConnection();
            var columns = GetDestinationColumns(connection);
            var reversed = Enumerable.Reverse(columns).ToList();
            UseBehavior(ClickHouseBulkColumnMappingsBehavior.Strict);

            // Act
            var exception = Assert.Throws<ClickHouseBulkColumnMappingsException>(() =>
                connection.BulkInsert(TableName, CreateDataTable(reversed)));

            // Assert
            Assert.IsTrue(exception.IsOrderMismatch);
            Assert.AreEqual(ClickHouseBulkColumnMappingsBehavior.Strict, exception.Behavior);
            Assert.AreEqual(TableName, exception.TableName);
            Assert.IsEmpty(exception.SourceColumnsNotInDestination);
            Assert.IsEmpty(exception.DestinationColumnsNotInSource);
            Assert.AreEqual(0, CountRows(connection));
        }

        [TestMethod]
        public void ThrowExceptionOnStrictBulkInsertForDataTableIfADestinationColumnIsMissing()
        {
            // Setup
            using var connection = CreateConnection();
            var columns = GetDestinationColumns(connection);
            var missing = columns.Last();
            UseBehavior(ClickHouseBulkColumnMappingsBehavior.Strict);

            // Act
            var exception = Assert.Throws<ClickHouseBulkColumnMappingsException>(() =>
                connection.BulkInsert(TableName, CreateDataTable(columns.Take(columns.Count - 1))));

            // Assert
            CollectionAssert.AreEqual(new[] { missing }, exception.DestinationColumnsNotInSource.ToArray());
            Assert.IsEmpty(exception.SourceColumnsNotInDestination);
            Assert.IsFalse(exception.IsOrderMismatch);
            Assert.AreEqual(0, CountRows(connection));
        }

        [TestMethod]
        public void ThrowExceptionOnStrictBulkInsertForDataTableIfASourceColumnDoesNotExist()
        {
            // Setup
            using var connection = CreateConnection();
            var columns = GetDestinationColumns(connection);
            UseBehavior(ClickHouseBulkColumnMappingsBehavior.Strict);

            // Act
            var exception = Assert.Throws<ClickHouseBulkColumnMappingsException>(() =>
                connection.BulkInsert(TableName, CreateDataTable(columns.Append(UnknownColumn))));

            // Assert
            CollectionAssert.AreEqual(new[] { UnknownColumn }, exception.SourceColumnsNotInDestination.ToArray());
            StringAssert.Contains(exception.Message, UnknownColumn);
            Assert.AreEqual(0, CountRows(connection));
        }

        [TestMethod]
        public void ThrowExceptionOnStrictBulkMergeForEntitiesIfTheOrderIsDifferent()
        {
            // Setup
            using var connection = CreateConnection();
            var count = Seed(connection);
            var reversed = Enumerable.Reverse(GetDestinationColumns(connection)).ToList();
            UseBehavior(ClickHouseBulkColumnMappingsBehavior.Strict);

            // Act
            var exception = Assert.Throws<ClickHouseBulkColumnMappingsException>(() =>
                connection.BulkMerge(TableName, CreateEntities(reversed)));

            // Assert
            Assert.IsTrue(exception.IsOrderMismatch);
            Assert.AreEqual(count, CountRows(connection));
        }

        [TestMethod]
        public void ThrowExceptionOnStrictBulkUpdateForDataReaderIfADestinationColumnIsMissing()
        {
            // Setup
            using var connection = CreateConnection();
            var key = GetKeyColumn(connection);
            using var table = CreateDataTable([key]);
            using var reader = table.CreateDataReader();
            UseBehavior(ClickHouseBulkColumnMappingsBehavior.Strict);

            // Act
            var exception = Assert.Throws<ClickHouseBulkColumnMappingsException>(() =>
                connection.BulkUpdate(TableName, reader));

            // Assert
            Assert.HasCount(GetDestinationColumns(connection).Count - 1, exception.DestinationColumnsNotInSource);
        }

        [TestMethod]
        public void ThrowExceptionOnStrictBulkDeleteForDataTableIfADestinationColumnIsMissing()
        {
            // Setup
            using var connection = CreateConnection();
            var count = Seed(connection);
            var key = GetKeyColumn(connection);
            UseBehavior(ClickHouseBulkColumnMappingsBehavior.Strict);

            // Act
            Assert.Throws<ClickHouseBulkColumnMappingsException>(() =>
                connection.BulkDelete(TableName, CreateDataTable([key])));

            // Assert (nothing was deleted)
            Assert.AreEqual(count, CountRows(connection));
        }

        [TestMethod]
        public async Task ThrowExceptionOnStrictBulkInsertAsyncForEntitiesIfTheOrderIsDifferent()
        {
            // Setup
            using var connection = CreateConnection();
            var reversed = Enumerable.Reverse(GetDestinationColumns(connection)).ToList();
            UseBehavior(ClickHouseBulkColumnMappingsBehavior.Strict);

            // Act
            var exception = await Assert.ThrowsAsync<ClickHouseBulkColumnMappingsException>(() =>
                connection.BulkInsertAsync(TableName, CreateEntities(reversed)));

            // Assert
            Assert.IsTrue(exception.IsOrderMismatch);
            Assert.AreEqual(0, CountRows(connection));
        }

        [TestMethod]
        public async Task ThrowExceptionOnStrictBulkDeleteAsyncForDataReaderIfTheOrderIsDifferent()
        {
            // Setup
            using var connection = CreateConnection();
            var count = Seed(connection);
            var reversed = Enumerable.Reverse(GetDestinationColumns(connection)).ToList();
            using var table = CreateDataTable(reversed);
            using var reader = table.CreateDataReader();
            UseBehavior(ClickHouseBulkColumnMappingsBehavior.Strict);

            // Act
            var exception = await Assert.ThrowsAsync<ClickHouseBulkColumnMappingsException>(() =>
                connection.BulkDeleteAsync(TableName, reader));

            // Assert
            Assert.IsTrue(exception.IsOrderMismatch);
            Assert.AreEqual(count, CountRows(connection));
        }

        #endregion

        #region StrictBypass

        [TestMethod]
        public void TestStrictBypassBulkUpdateForDataTableWithASubsetOfColumnsInADifferentOrder()
        {
            // Setup
            using var connection = CreateConnection();
            var count = Seed(connection);
            var key = GetKeyColumn(connection);
            var notNullables = DbFieldCache.Get(connection, TableName, null)
                .GetItems()
                .Where(dbField => !dbField.IsNullable)
                .Select(dbField => dbField.Name)
                .ToList();
            var table = LoadDataTable(connection);
            var kept = GetColumns(table).Last(column => !string.Equals(column, key, StringComparison.OrdinalIgnoreCase));

            // Bypass the nullable columns (except one), as the destination columns that are not supplied by the source are bypassed
            foreach (var column in GetColumns(table).Where(column => column != key && column != kept && !notNullables.Contains(column)))
            {
                table.Columns.Remove(column);
            }
            table.Columns[key].SetOrdinal(table.Columns.Count - 1);
            UseBehavior(ClickHouseBulkColumnMappingsBehavior.StrictBypass);

            // Act
            var result = connection.BulkUpdate(TableName, table);

            // Assert
            Assert.AreEqual(count, result);
        }

        [TestMethod]
        public async Task TestStrictBypassBulkUpdateAsyncForDataTableWithAllColumnsInADifferentOrder()
        {
            // Setup
            using var connection = CreateConnection();
            var count = Seed(connection);
            var table = LoadDataTable(connection);
            table.Columns[0].SetOrdinal(table.Columns.Count - 1);
            UseBehavior(ClickHouseBulkColumnMappingsBehavior.StrictBypass);

            // Act
            var result = await connection.BulkUpdateAsync(TableName, table);

            // Assert
            Assert.AreEqual(count, result);
        }

        [TestMethod]
        public void ThrowExceptionOnStrictBypassBulkInsertForDataTableIfASourceColumnDoesNotExist()
        {
            // Setup
            using var connection = CreateConnection();
            var columns = GetDestinationColumns(connection);
            UseBehavior(ClickHouseBulkColumnMappingsBehavior.StrictBypass);

            // Act
            var exception = Assert.Throws<ClickHouseBulkColumnMappingsException>(() =>
                connection.BulkInsert(TableName, CreateDataTable(columns.Skip(1).Append(UnknownColumn))));

            // Assert
            Assert.AreEqual(ClickHouseBulkColumnMappingsBehavior.StrictBypass, exception.Behavior);
            CollectionAssert.AreEqual(new[] { UnknownColumn }, exception.SourceColumnsNotInDestination.ToArray());
            Assert.IsEmpty(exception.DestinationColumnsNotInSource);
            Assert.AreEqual(0, CountRows(connection));
        }

        [TestMethod]
        public void ThrowExceptionOnStrictBypassBulkMergeForEntitiesIfASourcePropertyDoesNotExist()
        {
            // Setup
            using var connection = CreateConnection();
            var count = Seed(connection);
            UseBehavior(ClickHouseBulkColumnMappingsBehavior.StrictBypass);

            // Act
            var exception = Assert.Throws<ClickHouseBulkColumnMappingsException>(() =>
                connection.BulkMerge(TableName, CreateEntities([GetKeyColumn(connection), UnknownColumn])));

            // Assert
            CollectionAssert.AreEqual(new[] { UnknownColumn }, exception.SourceColumnsNotInDestination.ToArray());
            Assert.AreEqual(count, CountRows(connection));
        }

        [TestMethod]
        public async Task ThrowExceptionOnStrictBypassBulkUpdateAsyncForDataReaderIfASourceColumnDoesNotExist()
        {
            // Setup
            using var connection = CreateConnection();
            using var table = CreateDataTable([GetKeyColumn(connection), UnknownColumn]);
            using var reader = table.CreateDataReader();
            UseBehavior(ClickHouseBulkColumnMappingsBehavior.StrictBypass);

            // Act
            var exception = await Assert.ThrowsAsync<ClickHouseBulkColumnMappingsException>(() =>
                connection.BulkUpdateAsync(TableName, reader));

            // Assert
            CollectionAssert.AreEqual(new[] { UnknownColumn }, exception.SourceColumnsNotInDestination.ToArray());
        }

        [TestMethod]
        public async Task ThrowExceptionOnStrictBypassBulkMergeAsyncForDataTableIfASourceColumnDoesNotExist()
        {
            // Setup
            using var connection = CreateConnection();
            UseBehavior(ClickHouseBulkColumnMappingsBehavior.StrictBypass);

            // Act
            await Assert.ThrowsAsync<ClickHouseBulkColumnMappingsException>(() =>
                connection.BulkMergeAsync(TableName, CreateDataTable([GetKeyColumn(connection), UnknownColumn])));

            // Assert
            Assert.AreEqual(0, CountRows(connection));
        }

        [TestMethod]
        public void ThrowExceptionOnStrictBypassBulkDeleteForEntitiesIfASourcePropertyDoesNotExist()
        {
            // Setup
            using var connection = CreateConnection();
            var count = Seed(connection);
            UseBehavior(ClickHouseBulkColumnMappingsBehavior.StrictBypass);

            // Act
            Assert.Throws<ClickHouseBulkColumnMappingsException>(() =>
                connection.BulkDelete(TableName, CreateEntities([GetKeyColumn(connection), UnknownColumn])));

            // Assert (nothing was deleted)
            Assert.AreEqual(count, CountRows(connection));
        }

        #endregion

        #region Explicit Mappings / Automatic

        [TestMethod]
        public void TestStrictIsNotAppliedIfMappingsArePassed()
        {
            // Setup
            using var connection = CreateConnection();
            var count = Seed(connection);
            var table = LoadDataTable(connection);
            var mappings = GetColumns(table)
                .Select(column => new ClickHouseBulkInsertMapItem(column, column))
                .ToList();
            table.Columns.Add(UnknownColumn, typeof(string));
            table.Columns[0].SetOrdinal(table.Columns.Count - 1);
            UseBehavior(ClickHouseBulkColumnMappingsBehavior.Strict);

            // Act
            var result = connection.BulkUpdate(TableName, table, mappings: mappings);

            // Assert
            Assert.AreEqual(count, result);
        }

        [TestMethod]
        public void TestAutomaticBulkUpdateForDataTableWithAllColumnsInADifferentOrder()
        {
            // Setup (the default setting, i.e. Automatic)
            using var connection = CreateConnection();
            var count = Seed(connection);
            var table = LoadDataTable(connection);
            table.Columns[0].SetOrdinal(table.Columns.Count - 1);

            // Act
            var result = connection.BulkUpdate(TableName, table);

            // Assert
            Assert.AreEqual(count, result);
        }

        [TestMethod]
        public void TestBulkDeleteByKeyIsNotAffectedByStrict()
        {
            // Setup
            using var connection = CreateConnection();
            Seed(connection);
            var key = GetKeyColumn(connection);
            var keys = connection.QueryAll<BulkOperationIdentityTable>()
                .Select(entity => entity.Id)
                .ToList();
            UseBehavior(ClickHouseBulkColumnMappingsBehavior.Strict);

            // Act
            var result = connection.BulkDeleteByKey(TableName, keys);

            // Assert
            Assert.AreEqual(keys.Count, result);
            Assert.AreEqual(0, CountRows(connection));
            Assert.IsNotNull(key);
        }

        #endregion
    }
}
