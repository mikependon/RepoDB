#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Interfaces;
using RepoDb.Schema.Core.UnitTests.CustomObjects;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests.Extensions
{
    [TestClass]
    public class LoadDefinitionDefaultValueTest
    {
        #region Private Variables

        private List<TableSchema> _composed;
        private string _filename;

        #endregion

        #region Setup

        [TestInitialize]
        public void Initialize()
        {
            _composed = new List<TableSchema>();
            _filename = Path.Combine(Path.GetTempPath(), "repodb-load-default-" + Guid.NewGuid().ToString("N"));
            using var initializeConnection = new CustomDbConnection();
            DbSettingMapper.Add<CustomDbConnection>(CreateSetting(), true);
            SchemaReaderMapper.Clear();
            SchemaComposerMapper.Add<CustomDbConnection>(CreateComposer(), true);
            WriteDefinition();
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(_filename))
            {
                File.Delete(_filename);
            }

            SchemaReaderMapper.Clear();
            SchemaComposerMapper.Clear();
        }

        #endregion

        #region Private Methods

        private ISchemaComposer CreateComposer()
        {
            var composer = new Mock<ISchemaComposer>();
            composer.Setup(c => c.ComposeName(It.IsAny<TableInfo>())).Returns<TableInfo>(table => table.Name);
            composer.Setup(c => c.ComposeTableExists(It.IsAny<string>())).Returns<string>(table => "EXISTS " + table);
            composer.Setup(c => c.ComposeSchema(It.IsAny<TableSchema>())).Returns<TableSchema>(table => new[] { "CREATE " + table.Table.Name });
            composer.Setup(c => c.ComposeSchemas(It.IsAny<IEnumerable<TableSchema>>())).Returns<IEnumerable<TableSchema>>(tables =>
            {
                var list = tables.ToList();
                _composed.AddRange(list);
                return list.Select(table => "CREATE " + table.Table.Name);
            });
            composer.As<ISchemaValueSupport>().SetupGet(item => item.ValueFormatter).Returns(new CustomSchemaValueFormatter());
            return composer.Object;
        }

        private static IDbSetting CreateSetting()
        {
            var setting = new Mock<IDbSetting>();
            setting.SetupGet(item => item.IsSchemaSupported).Returns(true);
            setting.SetupGet(item => item.Provider).Returns("PGSQL");
            return setting.Object;
        }

        private void WriteDefinition()
        {
            var schema = new TableSchema(new TableInfo("Products",
                "dbo",
                "Northwind",
                "MSSQL"));
            schema.Columns.Add(new ColumnInfo
            {
                Ordinal = 1,
                Field = new DbField("Id", true, false, false, typeof(int), null, null, null, "int")
            });
            schema.Columns.Add(new ColumnInfo
            {
                Ordinal = 2,
                NativeType = "bit",
                DefaultValue = ColumnDefaultValue.Literal,
                DefaultExpression = "((0))",
                Field = new DbField("Discontinued", false, false, false, typeof(bool), null, null, null, "bit", true, "MSSQL")
            });
            schema.Columns.Add(new ColumnInfo
            {
                Ordinal = 3,
                NativeType = "bit",
                DefaultValue = ColumnDefaultValue.Literal,
                DefaultExpression = "'maybe'",
                Field = new DbField("Active", false, false, false, typeof(bool), null, null, null, "bit", true, "MSSQL")
            });
            schema.PrimaryKey = new PrimaryKeyInfo("PK_Products") { Columns = new[] { "Id" } };
            var definition = SchemaDefinition.Create(schema.Table,
                new[] { schema },
                RelatedTableBehavior.TableOnly);
            File.WriteAllText(_filename,
                SchemaDefinitionSerializer.Serialize(definition,
                    "json"));
        }

        private static CustomDbConnection CreateConnection() =>
            new CustomDbConnection
            {
                ScalarResult = text => null
            };

        private ColumnInfo ComposedColumn(string name) =>
            _composed.Single().Columns.Single(column => column.Field.Name == name);

        #endregion

        #region Public Methods

        [TestMethod]
        public void TestLoadDefinitionFromJsonWritesTheDefaultValueInTheFormatOfTheDestination()
        {
            // Setup
            using var connection = CreateConnection();

            // Act
            connection.LoadDefinitionFromJson(_filename);

            // Assert
            Assert.AreEqual("FALSE", ComposedColumn("Discontinued").DefaultExpression);
        }

        [TestMethod]
        public void TestLoadDefinitionFromJsonLeavesOutTheDefaultValueThatIsNotUnderstood()
        {
            // Setup
            using var connection = CreateConnection();

            // Act
            connection.LoadDefinitionFromJson(_filename);

            // Assert
            Assert.IsNull(ComposedColumn("Active").DefaultExpression);
        }

        [TestMethod]
        public void TestLoadDefinitionFromJsonReportsTheDefaultValueThatIsNotUnderstood()
        {
            // Setup
            using var connection = CreateConnection();

            // Act
            var result = connection.LoadDefinitionFromJson(_filename);

            // Assert
            StringAssert.Contains(result.CopyResult.Items.Single().Warnings.Single(), "Skipped the default value of the column 'Active'");
        }

        [TestMethod]
        public void TestLoadDefinitionFromJsonDoesNotChangeTheLoadedDefinition()
        {
            // Setup
            using var connection = CreateConnection();

            // Act
            var result = connection.LoadDefinitionFromJson(_filename);

            // Assert
            Assert.AreEqual("((0))", result.Definition.Tables.Single().Columns.Single(column => column.Field.Name == "Discontinued").DefaultExpression);
        }

        [TestMethod]
        public async Task TestLoadDefinitionFromJsonAsyncWritesTheDefaultValueInTheFormatOfTheDestination()
        {
            // Setup
            using var connection = CreateConnection();

            // Act
            await connection.LoadDefinitionFromJsonAsync(_filename);

            // Assert
            Assert.AreEqual("FALSE", ComposedColumn("Discontinued").DefaultExpression);
        }

        [TestMethod]
        public async Task TestLoadDefinitionFromJsonAsyncReportsTheDefaultValueThatIsNotUnderstood()
        {
            // Setup
            using var connection = CreateConnection();

            // Act
            var result = await connection.LoadDefinitionFromJsonAsync(_filename);

            // Assert
            StringAssert.Contains(result.CopyResult.Items.Single().Warnings.Single(), "Skipped the default value of the column 'Active'");
        }

        #endregion
    }
}
