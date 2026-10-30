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
using RepoDb.Schema.Core.UnitTests.CustomObjects;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests.Extensions
{
    /// <summary>
    /// Contains the end-to-end tests of the format of a definition: it is saved as <c>RepoDb.Schema.Definition</c>, it is kept by the read, and every
    /// validation (of a file, of a loaded definition, and the validation that the read and the load of a file apply) rejects any other value.
    /// </summary>
    [TestClass]
    public class DefinitionFormatTest
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
            _filename = Path.Combine(Path.GetTempPath(), "repodb-format-core-" + Guid.NewGuid().ToString("N"));
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
        [DataRow("json")]
        [DataRow("xml")]
        [DataRow("yaml")]
        public void SaveWritesTheFormatOfASchemaDefinition(string format)
        {
            // Setup
            WriteDefinition(format);

            // Act
            var content = File.ReadAllText(_filename);

            // Assert
            StringAssert.Contains(content, "RepoDb.Schema.Definition", StringComparison.Ordinal);
            Assert.IsFalse(content.Contains("RepoDB.Schema", StringComparison.Ordinal));
        }

        [TestMethod]
        [DataRow("json", false)]
        [DataRow("json", true)]
        [DataRow("xml", false)]
        [DataRow("xml", true)]
        [DataRow("yaml", false)]
        [DataRow("yaml", true)]
        public async Task ReadKeepsTheFormat(string format,
            bool asynchronous)
        {
            // Setup
            WriteDefinition(format);
            using var connection = new CustomDbConnection();

            // Act
            var definition = await Read(connection, format, asynchronous).ConfigureAwait(false);

            // Assert
            Assert.AreEqual("RepoDb.Schema.Definition", definition.Format, StringComparer.Ordinal);
        }

        [TestMethod]
        [DataRow("json", false)]
        [DataRow("json", true)]
        [DataRow("xml", false)]
        [DataRow("xml", true)]
        [DataRow("yaml", false)]
        [DataRow("yaml", true)]
        public async Task ValidateFromAFileReportsAnotherFormat(string format,
            bool asynchronous)
        {
            // Setup
            WriteDefinition(format, "Other.Format");
            using var connection = new CustomDbConnection();

            // Act
            var result = await Validate(connection, format, asynchronous).ConfigureAwait(false);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsNull(result.Definition);
            Assert.AreEqual(1, result.Errors.Count);
            StringAssert.Contains(result.Errors[0], "must be 'RepoDb.Schema.Definition'", StringComparison.Ordinal);
            StringAssert.Contains(result.Errors[0], "but it is 'Other.Format'", StringComparison.Ordinal);
        }

        [TestMethod]
        [DataRow("json")]
        [DataRow("xml")]
        [DataRow("yaml")]
        public async Task ValidateFromAFileRejectsTheFormatThatWasUsedBefore(string format)
        {
            // Setup
            WriteDefinition(format, "RepoDB.Schema");
            using var connection = new CustomDbConnection();

            // Act
            var result = await Validate(connection, format, true).ConfigureAwait(false);

            // Assert
            Assert.IsFalse(result.IsValid);
            StringAssert.Contains(result.Errors[0], "but it is 'RepoDB.Schema'", StringComparison.Ordinal);
        }

        [TestMethod]
        [DataRow("json")]
        [DataRow("xml")]
        [DataRow("yaml")]
        public async Task ValidateFromAFileReportsTheFormatTogetherWithTheOtherProblems(string format)
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Child", "Id");
            TableSchemaFactory.SetPrimaryKey(table, "PK_Child", "Missing");
            WriteDefinition(format, "Other.Format", table);
            using var connection = new CustomDbConnection();

            // Act
            var result = await Validate(connection, format, true).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(2, result.Errors.Count);
            StringAssert.Contains(result.Errors[0], "its format must be", StringComparison.Ordinal);
            StringAssert.Contains(result.Errors[1], "primary key 'PK_Child'", StringComparison.Ordinal);
        }

        [TestMethod]
        [DataRow("json", false)]
        [DataRow("json", true)]
        [DataRow("xml", false)]
        [DataRow("xml", true)]
        [DataRow("yaml", false)]
        [DataRow("yaml", true)]
        public async Task ReadThrowsForAnotherFormat(string format,
            bool asynchronous)
        {
            // Setup
            WriteDefinition(format, "Other.Format");
            using var connection = new CustomDbConnection();

            // Act/Assert
            var exception = await Assert.ThrowsExactlyAsync<InvalidDataException>(() => Read(connection, format, asynchronous)).ConfigureAwait(false);
            StringAssert.Contains(exception.Message, "The file is not a RepoDB schema definition", StringComparison.Ordinal);
            StringAssert.Contains(exception.Message, "'RepoDb.Schema.Definition'", StringComparison.Ordinal);
        }

        [TestMethod]
        [DataRow("json", false)]
        [DataRow("json", true)]
        [DataRow("xml", false)]
        [DataRow("xml", true)]
        [DataRow("yaml", false)]
        [DataRow("yaml", true)]
        public async Task LoadThrowsForAnotherFormatWithoutAccessingTheDatabase(string format,
            bool asynchronous)
        {
            // Setup
            WriteDefinition(format, "Other.Format");
            using var connection = new CustomDbConnection();

            // Act/Assert
            await Assert.ThrowsExactlyAsync<InvalidDataException>(() => Load(connection, format, asynchronous)).ConfigureAwait(false);
            Assert.AreEqual(0, connection.Details.Count);
            Assert.AreEqual(0, connection.ExecutedCommands.Count);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ValidateAnInMemoryDefinitionReportsAnotherFormat(bool asynchronous)
        {
            // Setup
            var definition = TableSchemaFactory.CreateDefinition(TableSchemaFactory.CreateTable("Child", "Id"));
            definition.Format = "Other.Format";
            using var connection = new CustomDbConnection();

            // Act
            var result = asynchronous ? await connection.ValidateDefinitionAsync(definition).ConfigureAwait(false) : connection.ValidateDefinition(definition);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            StringAssert.Contains(result.Errors[0], "but it is 'Other.Format'", StringComparison.Ordinal);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ValidateAnInMemoryDefinitionReportsAMissingFormat(bool asynchronous)
        {
            // Setup
            var definition = TableSchemaFactory.CreateDefinition(TableSchemaFactory.CreateTable("Child", "Id"));
            definition.Format = null;
            using var connection = new CustomDbConnection();

            // Act
            var result = asynchronous ? await connection.ValidateDefinitionAsync(definition).ConfigureAwait(false) : connection.ValidateDefinition(definition);

            // Assert
            Assert.IsFalse(result.IsValid);
            StringAssert.Contains(result.Errors[0], "but it is missing", StringComparison.Ordinal);
        }

        [TestMethod]
        public void ValidateAnInMemoryDefinitionAcceptsTheFormatOfASchemaDefinition()
        {
            // Setup
            var definition = TableSchemaFactory.CreateDefinition(TableSchemaFactory.CreateTable("Child", "Id"));
            using var connection = new CustomDbConnection();

            // Act
            var result = connection.ValidateDefinition(definition);

            // Assert
            Assert.IsTrue(result.IsValid);
            Assert.AreSame(definition, result.Definition);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Writes a definition file of a table that has a primary key, in the given format. The first table is the root table.
        /// </summary>
        /// <param name="format">The format of the file: json, xml or yaml.</param>
        /// <param name="savedFormat">The value of the <c>format</c> property to be saved instead of the one that the library writes, or <c>null</c> to keep it.</param>
        /// <param name="tables">The tables of the definition (a table with a primary key if none is given).</param>
        private void WriteDefinition(string format,
            string savedFormat = null,
            params TableSchema[] tables)
        {
            if (tables.Length == 0)
            {
                var table = TableSchemaFactory.CreateTable("Child", "Id", "Name");
                TableSchemaFactory.SetPrimaryKey(table, "PK_Child", "Id");
                tables = new[] { table };
            }
            var definition = SchemaDefinition.Create(tables[0].Table, tables, RelatedTableBehavior.ParentsAndChildren);
            var content = SchemaDefinitionSerializer.Serialize(definition, format);
            if (savedFormat != null)
            {
                content = content.Replace(SchemaDefinitionFormat.Name, savedFormat, StringComparison.Ordinal);
            }
            File.WriteAllText(_filename, content);
        }

        /// <summary>
        /// Reads the definition file with the extension method of the format, in the sync or async way.
        /// </summary>
        /// <param name="connection">The connection that the extension method is invoked on.</param>
        /// <param name="format">The format of the file: json, xml or yaml.</param>
        /// <param name="asynchronous">Whether the asynchronous extension method is used.</param>
        /// <returns>A task that represents the operation. The task result is the definition that was read.</returns>
        private async Task<SchemaDefinitionInfo> Read(IDbConnection connection,
            string format,
            bool asynchronous)
        {
            switch (format)
            {
                case "json":
                    return asynchronous
                        ? await connection.ReadDefinitionFromJsonAsync(_filename).ConfigureAwait(false)
                        : connection.ReadDefinitionFromJson(_filename);
                case "xml":
                    return asynchronous
                        ? await connection.ReadDefinitionFromXmlAsync(_filename).ConfigureAwait(false)
                        : connection.ReadDefinitionFromXml(_filename);
                default:
                    return asynchronous
                        ? await connection.ReadDefinitionFromYamlAsync(_filename).ConfigureAwait(false)
                        : connection.ReadDefinitionFromYaml(_filename);
            }
        }

        /// <summary>
        /// Validates the definition file with the extension method of the format, in the sync or async way.
        /// </summary>
        /// <param name="connection">The connection that the extension method is invoked on.</param>
        /// <param name="format">The format of the file: json, xml or yaml.</param>
        /// <param name="asynchronous">Whether the asynchronous extension method is used.</param>
        /// <returns>A task that represents the operation. The task result is the outcome of the validation.</returns>
        private async Task<ValidateDefinitionResult> Validate(IDbConnection connection,
            string format,
            bool asynchronous)
        {
            switch (format)
            {
                case "json":
                    return asynchronous
                        ? await connection.ValidateDefinitionFromJsonAsync(_filename).ConfigureAwait(false)
                        : connection.ValidateDefinitionFromJson(_filename);
                case "xml":
                    return asynchronous
                        ? await connection.ValidateDefinitionFromXmlAsync(_filename).ConfigureAwait(false)
                        : connection.ValidateDefinitionFromXml(_filename);
                default:
                    return asynchronous
                        ? await connection.ValidateDefinitionFromYamlAsync(_filename).ConfigureAwait(false)
                        : connection.ValidateDefinitionFromYaml(_filename);
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
