#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Schema;
using RepoDb.Schema.Core.UnitTests.CustomObjects;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests.Extensions
{
    [TestClass]
    public class CopyTableSchemasToDefaultValuesTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            SchemaReaderMapper.Clear();
            SchemaComposerMapper.Clear();
        }

        #region Helpers

        private static TableSchema GetOrders()
        {
            var schema = new TableSchema("Orders", "dbo");
            foreach (var (name, kind, expression) in new[]
            {
                ("Id", ColumnDefaultValue.None, (string)null),
                ("Created", ColumnDefaultValue.CurrentTimestamp, "(getdate())"),
                ("Number", ColumnDefaultValue.SequenceNextValue, "(NEXT VALUE FOR [dbo].[OrderSeq])"),
                ("Token", ColumnDefaultValue.NewGuid, "(newid())")
            })
            {
                var column = new ColumnInfo();
                typeof(ColumnInfo).GetProperty(nameof(ColumnInfo.Field)).SetValue(column, new DbField(name, false, false, true, typeof(string), null, null, null, "text", expression != null, "Test"));
                typeof(ColumnInfo).GetProperty(nameof(ColumnInfo.DefaultValue)).SetValue(column, kind);
                typeof(ColumnInfo).GetProperty(nameof(ColumnInfo.DefaultExpression)).SetValue(column, expression);
                schema.Columns.Add(column);
            }
            return schema;
        }

        private static void MapReader(params TableSchema[] schemas)
        {
            var relationships = schemas.Select(s => new RelationshipInfo { Schema = s }).ToList();
            var reader = new Mock<ISchemaReader>();
            reader.Setup(r => r.GetDependencyOrder(It.IsAny<IEnumerable<string>>())).Returns(relationships);
            reader.Setup(r => r.GetDependencyOrderAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>())).ReturnsAsync(relationships);
            SchemaReaderMapper.Add<CustomDbConnection>(reader.Object, true);
        }

        private static void MapComposer(Func<ColumnInfo, string> unsupportedDefaultReason = null)
        {
            var composer = new Mock<ISchemaComposer>();
            composer.Setup(c => c.ComposeName(It.IsAny<TableInfo>())).Returns<TableInfo>(t => t.Name);
            composer.Setup(c => c.ComposeSchemas(It.IsAny<IEnumerable<TableSchema>>())).Returns<IEnumerable<TableSchema>>(schemas => schemas.Select(s => $"CREATE TABLE {s.Table.Name};").ToList());
            composer.Setup(c => c.ComposeSchema(It.IsAny<TableSchema>())).Returns<TableSchema>(s => new[] { $"CREATE TABLE {s.Table.Name};" });
            composer.Setup(c => c.ComposeTableExists(It.IsAny<string>())).Returns<string>(n => $"TABLE_EXISTS {n}");
            if (unsupportedDefaultReason != null)
            {
                composer.As<IColumnDefaultSupport>().Setup(c => c.GetUnsupportedDefaultReason(It.IsAny<ColumnInfo>())).Returns(unsupportedDefaultReason);
            }
            SchemaComposerMapper.Add<CustomDbConnection>(composer.Object, true);
        }

        private static string Refuse(ColumnInfo column) =>
            column.DefaultValue == ColumnDefaultValue.SequenceNextValue ? "its value comes from a sequence, which the destination can not use as a default value" : null;

        #endregion

        #region Warnings

        [TestMethod]
        public void TestCopyTableSchemasToReportsTheDefaultValuesThatTheComposerDoesNotCreate()
        {
            // Setup
            MapReader(GetOrders());
            MapComposer(Refuse);
            var results = new List<CopyTableSchemaResult>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Orders" }, new CustomDbConnection(), createdCallback: results.Add);

            // Assert
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(1, results[0].Warnings.Count);
            StringAssert.Contains(results[0].Warnings[0], "Skipped the default value of the column 'Number'", StringComparison.Ordinal);
            StringAssert.Contains(results[0].Warnings[0], "sequence", StringComparison.Ordinal);
            Assert.AreEqual(0, results[0].Errors.Count);
        }

        [TestMethod]
        public async Task TestCopyTableSchemasToAsyncReportsTheDefaultValuesThatTheComposerDoesNotCreate()
        {
            // Setup
            MapReader(GetOrders());
            MapComposer(Refuse);
            var results = new List<CopyTableSchemaResult>();

            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Orders" }, new CustomDbConnection(), createdCallback: results.Add);

            // Assert
            Assert.AreEqual(1, results[0].Warnings.Count);
            StringAssert.Contains(results[0].Warnings[0], "'Number'", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestCopyTableSchemasToReportsOneWarningForEachDefaultValueThatTheComposerDoesNotCreate()
        {
            // Setup
            MapReader(GetOrders());
            MapComposer(column => column.DefaultValue == ColumnDefaultValue.None ? null : $"the destination refuses {column.DefaultValue}");
            var results = new List<CopyTableSchemaResult>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Orders" }, new CustomDbConnection(), createdCallback: results.Add);

            // Assert
            Assert.AreEqual(3, results[0].Warnings.Count);
            StringAssert.Contains(results[0].Warnings[0], "'Created'", StringComparison.Ordinal);
            StringAssert.Contains(results[0].Warnings[2], "'Token'", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestCopyTableSchemasToDoesNotReportTheDefaultValuesIfTheComposerCreatesThemAll()
        {
            // Setup
            MapReader(GetOrders());
            MapComposer(column => null);
            var results = new List<CopyTableSchemaResult>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Orders" }, new CustomDbConnection(), createdCallback: results.Add);

            // Assert
            Assert.AreEqual(0, results[0].Warnings.Count);
        }

        [TestMethod]
        public void TestCopyTableSchemasToDoesNotReportTheDefaultValuesIfTheComposerDoesNotSayWhichOnesItRefuses()
        {
            // Setup
            MapReader(GetOrders());
            MapComposer();
            var results = new List<CopyTableSchemaResult>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Orders" }, new CustomDbConnection(), createdCallback: results.Add);

            // Assert
            Assert.AreEqual(0, results[0].Warnings.Count);
        }

        [TestMethod]
        public void TestCopyTableSchemasToDoesNotReportTheDefaultValuesOfATableThatIsNotCreated()
        {
            // Setup
            MapReader(GetOrders());
            MapComposer(Refuse);
            var destination = new CustomDbConnection { ScalarResult = statement => statement == "TABLE_EXISTS Orders" ? 1 : 0 };
            var results = new List<CopyTableSchemaResult>();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Orders" }, destination, createdCallback: results.Add);

            // Assert
            Assert.AreEqual(0, results[0].Warnings.Count);
        }

        [TestMethod]
        public void TestCopyTableSchemasToKeepsTheDefaultValueOfTheColumnsOfTheSchemaOfTheSource()
        {
            // Setup
            var orders = GetOrders();
            MapReader(orders);
            MapComposer(Refuse);

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Orders" }, new CustomDbConnection());

            // Assert
            Assert.AreEqual(ColumnDefaultValue.SequenceNextValue, orders.Columns.Single(column => column.Field.Name == "Number").DefaultValue);
        }

        #endregion
    }
}
