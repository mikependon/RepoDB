#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Interfaces;
using RepoDb.Schema.Core.UnitTests.CustomObjects;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests.Extensions
{
    /// <summary>
    /// Contains the tests that show that reading and loading a definition file validate the definition, and fail with an <see cref="InvalidDataException"/> if it is not valid.
    /// </summary>
    [TestClass]
    public class ReadDefinitionValidationTest
    {
        #region Private Variables

        /// <summary>
        /// The name of the definition file that is created for a test.
        /// </summary>
        private string _filename;

        #endregion

        #region Public Methods

        [TestInitialize]
        public void Initialize()
        {
            _filename = Path.Combine(Path.GetTempPath(), "repodb-read-validation-core-" + Guid.NewGuid().ToString("N"));
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(_filename))
            {
                File.Delete(_filename);
            }
        }

        [TestMethod]
        [DataRow("json", false)]
        [DataRow("json", true)]
        [DataRow("xml", false)]
        [DataRow("xml", true)]
        [DataRow("yaml", false)]
        [DataRow("yaml", true)]
        public async Task ReadReturnsAValidDefinition(string format,
            bool asynchronous)
        {
            // Setup
            var table = CreateTable();
            WriteDefinition(format, table);
            using var connection = new CustomDbConnection();

            // Act
            var result = await Read(connection, format, asynchronous).ConfigureAwait(false);

            // Assert
            Assert.AreEqual("Child", result.Tables.Single().Table.Name);
        }

        [TestMethod]
        [DataRow("json", false)]
        [DataRow("json", true)]
        [DataRow("xml", false)]
        [DataRow("xml", true)]
        [DataRow("yaml", false)]
        [DataRow("yaml", true)]
        public async Task ReadThrowsForADefinitionWithAStructuralError(string format,
            bool asynchronous)
        {
            // Setup
            var table = CreateTable();
            TableSchemaFactory.SetPrimaryKey(table, "PK_Child", "Missing");
            WriteDefinition(format, table);
            using var connection = new CustomDbConnection();

            // Act/Assert
            var exception = await Assert.ThrowsExactlyAsync<InvalidDataException>(() => Read(connection, format, asynchronous)).ConfigureAwait(false);
            StringAssert.Contains(exception.Message, "primary key 'PK_Child'", StringComparison.Ordinal);
        }

        [TestMethod]
        [DataRow("json", false)]
        [DataRow("json", true)]
        [DataRow("xml", false)]
        [DataRow("xml", true)]
        [DataRow("yaml", false)]
        [DataRow("yaml", true)]
        public async Task ReadThrowsForADefinitionWithAStrictError(string format,
            bool asynchronous)
        {
            // Setup
            var parent = TableSchemaFactory.CreateTable("Parent", "Id", "Code");
            TableSchemaFactory.SetPrimaryKey(parent, "PK_Parent", "Id");
            var child = TableSchemaFactory.CreateTable("Child", "Id", "ParentCode");
            TableSchemaFactory.SetPrimaryKey(child, "PK_Child", "Id");
            TableSchemaFactory.AddForeignKey(child, "FK_Child", parent.Table, new[] { "ParentCode" }, new[] { "Code" });
            WriteDefinition(format, child, parent);
            using var connection = new CustomDbConnection();

            // Act/Assert
            var exception = await Assert.ThrowsExactlyAsync<InvalidDataException>(() => Read(connection, format, asynchronous)).ConfigureAwait(false);
            StringAssert.Contains(exception.Message, "not its primary key or a unique key", StringComparison.Ordinal);
        }

        [TestMethod]
        [DataRow("json", false)]
        [DataRow("json", true)]
        [DataRow("xml", false)]
        [DataRow("xml", true)]
        [DataRow("yaml", false)]
        [DataRow("yaml", true)]
        public async Task ReadReturnsADefinitionThatHasOnlyWarnings(string format,
            bool asynchronous)
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Child");
            table.Columns.Add(TableSchemaFactory.CreateColumn("Code", typeof(string), 1, isNullable: true));
            TableSchemaFactory.SetPrimaryKey(table, "PK_Child", "Code");
            WriteDefinition(format, table);
            using var connection = new CustomDbConnection();

            // Act
            var result = await Read(connection, format, asynchronous).ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ReadTracesOnlyTheReadAndDoesNotTellTheTraceThatItCompletedWhenTheDefinitionIsNotValid(bool asynchronous)
        {
            // Setup
            var table = CreateTable();
            TableSchemaFactory.SetPrimaryKey(table, "PK_Child", "Missing");
            WriteDefinition("json", table);
            using var connection = new CustomDbConnection();
            var trace = new RecordingTrace(asynchronous);

            // Act/Assert
            await Assert.ThrowsExactlyAsync<InvalidDataException>(() => Read(connection, "json", asynchronous, trace)).ConfigureAwait(false);
            Assert.AreEqual(1, trace.Keys.Count);
            Assert.AreEqual("read-validation-test", trace.Keys.Single());
            Assert.AreEqual(0, trace.AfterCount);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ReadTracesTheReadOnceWhenTheDefinitionIsValid(bool asynchronous)
        {
            // Setup
            WriteDefinition("json", CreateTable());
            using var connection = new CustomDbConnection();
            var trace = new RecordingTrace(asynchronous);

            // Act
            await Read(connection, "json", asynchronous, trace).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(1, trace.Keys.Count);
            Assert.AreEqual(1, trace.AfterCount);
        }

        [TestMethod]
        [DataRow("json", false)]
        [DataRow("json", true)]
        [DataRow("xml", false)]
        [DataRow("xml", true)]
        [DataRow("yaml", false)]
        [DataRow("yaml", true)]
        public async Task LoadThrowsForAnInvalidDefinitionWithoutAccessingTheDatabase(string format,
            bool asynchronous)
        {
            // Setup
            var table = CreateTable();
            TableSchemaFactory.SetPrimaryKey(table, "PK_Child", "Missing");
            WriteDefinition(format, table);
            using var connection = new CustomDbConnection();

            // Act/Assert
            await Assert.ThrowsExactlyAsync<InvalidDataException>(() => Load(connection, format, asynchronous)).ConfigureAwait(false);
            Assert.AreEqual(0, connection.Details.Count);
            Assert.AreEqual(0, connection.ExecutedCommands.Count);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates a table that has a primary key, and no problems.
        /// </summary>
        /// <returns>The table.</returns>
        private static TableSchema CreateTable()
        {
            var table = TableSchemaFactory.CreateTable("Child", "Id", "Name");
            TableSchemaFactory.SetPrimaryKey(table, "PK_Child", "Id");
            return table;
        }

        /// <summary>
        /// Writes a definition file of the given tables, in the given format. The first table is the root table.
        /// </summary>
        /// <param name="format">The format of the file: json, xml or yaml.</param>
        /// <param name="tables">The tables of the definition.</param>
        private void WriteDefinition(string format,
            params TableSchema[] tables)
        {
            var definition = SchemaDefinition.Create(tables[0].Table, tables, RelatedTableBehavior.ParentsAndChildren);
            File.WriteAllText(_filename, SchemaDefinitionSerializer.Serialize(definition, format));
        }

        /// <summary>
        /// Reads the definition file with the extension method of the format, in the sync or async way.
        /// </summary>
        /// <param name="connection">The connection that the extension method is invoked on.</param>
        /// <param name="format">The format of the file: json, xml or yaml.</param>
        /// <param name="asynchronous">Whether the asynchronous extension method is used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <returns>A task that represents the operation. The task result is the definition that was read.</returns>
        private async Task<SchemaDefinitionInfo> Read(IDbConnection connection,
            string format,
            bool asynchronous,
            ITrace trace = null)
        {
            const string key = "read-validation-test";
            switch (format)
            {
                case "json":
                    return asynchronous
                        ? await connection.ReadDefinitionFromJsonAsync(_filename, key, trace).ConfigureAwait(false)
                        : connection.ReadDefinitionFromJson(_filename, key, trace);
                case "xml":
                    return asynchronous
                        ? await connection.ReadDefinitionFromXmlAsync(_filename, key, trace).ConfigureAwait(false)
                        : connection.ReadDefinitionFromXml(_filename, key, trace);
                default:
                    return asynchronous
                        ? await connection.ReadDefinitionFromYamlAsync(_filename, key, trace).ConfigureAwait(false)
                        : connection.ReadDefinitionFromYaml(_filename, key, trace);
            }
        }

        /// <summary>
        /// Loads the definition file with the extension method of the format, in the sync or async way.
        /// </summary>
        /// <param name="connection">The connection that the extension method is invoked on.</param>
        /// <param name="format">The format of the file: json, xml or yaml.</param>
        /// <param name="asynchronous">Whether the asynchronous extension method is used.</param>
        /// <returns>A task that represents the operation. The task result is the outcome of the load.</returns>
        private async Task<LoadDefinitionResult> Load(IDbConnection connection,
            string format,
            bool asynchronous)
        {
            switch (format)
            {
                case "json":
                    return asynchronous
                        ? await connection.LoadDefinitionFromJsonAsync(_filename).ConfigureAwait(false)
                        : connection.LoadDefinitionFromJson(_filename);
                case "xml":
                    return asynchronous
                        ? await connection.LoadDefinitionFromXmlAsync(_filename).ConfigureAwait(false)
                        : connection.LoadDefinitionFromXml(_filename);
                default:
                    return asynchronous
                        ? await connection.LoadDefinitionFromYamlAsync(_filename).ConfigureAwait(false)
                        : connection.LoadDefinitionFromYaml(_filename);
            }
        }

        #endregion
    }
}
