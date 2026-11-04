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
    public class LoadDefinitionSchemaCreationTest
    {
        #region Private Variables

        private string _filename;

        #endregion

        #region Setup

        [TestInitialize]
        public void Initialize()
        {
            _filename = Path.Combine(Path.GetTempPath(), "repodb-load-schema-" + Guid.NewGuid().ToString("N"));
            using var initializeConnection = new CustomDbConnection();
            var setting = new Mock<IDbSetting>();
            setting.SetupGet(s => s.IsSchemaSupported).Returns(true);
            DbSettingMapper.Add<CustomDbConnection>(setting.Object, true);
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

        private static ISchemaComposer CreateComposer()
        {
            var composer = new Mock<ISchemaComposer>();
            composer.Setup(c => c.ComposeName(It.IsAny<TableInfo>())).Returns<TableInfo>(table => table.Name);
            composer.Setup(c => c.ComposeTableExists(It.IsAny<string>())).Returns<string>(table => "EXISTS " + table);
            composer.Setup(c => c.ComposeSchema(It.IsAny<TableSchema>())).Returns<TableSchema>(table => new[] { "CREATE " + table.Table.Name });
            composer.Setup(c => c.ComposeSchemas(It.IsAny<IEnumerable<TableSchema>>())).Returns<IEnumerable<TableSchema>>(tables => tables.Select(table => "CREATE " + table.Table.Name));
            var support = composer.As<ISchemaCreationSupport>();
            support.SetupGet(s => s.SchemaParameterPrefix).Returns("@");
            support.SetupGet(s => s.SchemaExistsSql).Returns("SELECT COUNT(*) FROM schemas WHERE name = {0}");
            support.SetupGet(s => s.CreateSchemaSql).Returns("CREATE SCHEMA {0}");
            support.Setup(s => s.QuoteSchemaName(It.IsAny<string>())).Returns<string>(name => "[" + name + "]");
            return composer.Object;
        }

        private void WriteDefinition()
        {
            var schema = new TableSchema(new TableInfo("FileTable", "FileSchema", "FileDatabase", "FileProvider"));
            schema.Columns.Add(new ColumnInfo { Ordinal = 1, Field = new DbField("Id", true, false, false, typeof(int), null, null, null, null) });
            schema.PrimaryKey = new PrimaryKeyInfo("PK_FileTable") { Columns = new[] { "Id" } };
            var definition = SchemaDefinition.Create(schema.Table, new[] { schema }, RelatedTableBehavior.TableOnly);
            File.WriteAllText(_filename, SchemaDefinitionSerializer.Serialize(definition, "json"));
        }

        private static CustomDbConnection CreateConnection(object count) =>
            new CustomDbConnection { ScalarResult = text => text.StartsWith("SELECT COUNT", StringComparison.Ordinal) ? count : null };

        #endregion

        #region Public Methods

        [TestMethod]
        public void TestLoadDefinitionFromJsonCreatesTheSchemaThatDoesNotExistBeforeTheTable()
        {
            // Setup
            var connection = CreateConnection(0);

            // Act
            connection.LoadDefinitionFromJson(_filename);

            // Assert
            CollectionAssert.AreEqual(new[] { "CREATE SCHEMA [FileSchema]", "CREATE FileTable" }, connection.ExecutedCommands);
        }

        [TestMethod]
        public void TestLoadDefinitionFromJsonDoesNotCreateTheSchemaThatExists()
        {
            // Setup
            var connection = CreateConnection(1);

            // Act
            connection.LoadDefinitionFromJson(_filename);

            // Assert
            CollectionAssert.AreEqual(new[] { "CREATE FileTable" }, connection.ExecutedCommands);
        }

        [TestMethod]
        public void TestLoadDefinitionFromJsonDoesNotCreateTheSchemaIfTheSchemaNamesAreIgnored()
        {
            // Setup
            var connection = CreateConnection(0);

            // Act
            connection.LoadDefinitionFromJson(_filename,
                schemaMappingMode: SchemaMappingMode.Ignore);

            // Assert
            CollectionAssert.AreEqual(new[] { "CREATE FileTable" }, connection.ExecutedCommands);
        }

        [TestMethod]
        public async Task TestLoadDefinitionFromJsonAsyncCreatesTheSchemaThatDoesNotExistBeforeTheTable()
        {
            // Setup
            var connection = CreateConnection(0);

            // Act
            await connection.LoadDefinitionFromJsonAsync(_filename);

            // Assert
            CollectionAssert.AreEqual(new[] { "CREATE SCHEMA [FileSchema]", "CREATE FileTable" }, connection.ExecutedCommands);
        }

        #endregion
    }
}
