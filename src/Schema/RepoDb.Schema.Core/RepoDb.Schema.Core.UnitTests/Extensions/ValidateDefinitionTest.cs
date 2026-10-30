#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Interfaces;
using RepoDb.Schema.Core.UnitTests.CustomObjects;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests.Extensions
{
    /// <summary>
    /// Contains the tests of the <c>ValidateDefinitionFrom*</c> extension methods.
    /// </summary>
    [TestClass]
    public class ValidateDefinitionTest
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
            _filename = Path.Combine(Path.GetTempPath(), "repodb-validate-core-" + Guid.NewGuid().ToString("N"));
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
        public async Task ValidateReportsAValidFile(string format,
            bool asynchronous)
        {
            // Setup
            WriteValidDefinition(format);
            using var connection = new CustomDbConnection();

            // Act
            var result = await Validate(connection, format, asynchronous).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual("FileTable", result.Definition.Tables.Single().Table.Name);
            Assert.IsTrue(result.Definition.FormatVersion > 0);
        }

        [TestMethod]
        [DataRow("json", "{ not json", "malformed", false)]
        [DataRow("json", "{ not json", "malformed", true)]
        [DataRow("json", "[]", "root must be an object", false)]
        [DataRow("json", "[]", "root must be an object", true)]
        [DataRow("json", "{}", "Missing property: format", false)]
        [DataRow("json", "{}", "Missing property: format", true)]
        [DataRow("xml", "<repoDbSchema>", "malformed", false)]
        [DataRow("xml", "<repoDbSchema>", "malformed", true)]
        [DataRow("xml", "<other />", "Invalid schema XML root", false)]
        [DataRow("xml", "<other />", "Invalid schema XML root", true)]
        [DataRow("yaml", "- a", "malformed", false)]
        [DataRow("yaml", "- a", "malformed", true)]
        [DataRow("yaml", "- \"a\"", "Invalid YAML definition root", false)]
        [DataRow("yaml", "- \"a\"", "Invalid YAML definition root", true)]
        [DataRow("yaml", "", "The YAML definition is empty", false)]
        [DataRow("yaml", "", "The YAML definition is empty", true)]
        public async Task ValidateReportsAnInvalidFileInsteadOfThrowing(string format,
            string content,
            string expectedMessage,
            bool asynchronous)
        {
            // Setup
            File.WriteAllText(_filename, content);
            using var connection = new CustomDbConnection();

            // Act
            var result = await Validate(connection, format, asynchronous).ConfigureAwait(false);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsNull(result.Definition);
            Assert.IsTrue(result.Errors.Count > 0);
            StringAssert.Contains(result.Errors[0], expectedMessage, StringComparison.Ordinal);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ValidateIncludesTheUnderlyingCauseOfAMalformedFile(bool asynchronous)
        {
            // Setup
            File.WriteAllText(_filename, "{ not json");
            using var connection = new CustomDbConnection();

            // Act
            var result = await Validate(connection, "json", asynchronous).ConfigureAwait(false);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Count > 1);
            StringAssert.Contains(result.Errors[0], "malformed", StringComparison.Ordinal);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ValidateReportsAnUnsupportedFormatVersion(bool asynchronous)
        {
            // Setup
            WriteValidDefinition("json");
            var document = JsonNode.Parse(File.ReadAllText(_filename)).AsObject();
            document["formatVersion"] = 99;
            File.WriteAllText(_filename, document.ToJsonString());
            using var connection = new CustomDbConnection();

            // Act
            var result = await Validate(connection, "json", asynchronous).ConfigureAwait(false);

            // Assert
            Assert.IsFalse(result.IsValid);
            StringAssert.Contains(result.Errors[0], "version is not supported", StringComparison.Ordinal);
        }

        [TestMethod]
        [DataRow("json", false)]
        [DataRow("json", true)]
        [DataRow("xml", false)]
        [DataRow("xml", true)]
        [DataRow("yaml", false)]
        [DataRow("yaml", true)]
        public async Task ValidateReportsAFileWithInvalidTextEncoding(string format,
            bool asynchronous)
        {
            // Setup
            File.WriteAllBytes(_filename, new byte[] { 0xC3, 0x28 });
            using var connection = new CustomDbConnection();

            // Act
            var result = await Validate(connection, format, asynchronous).ConfigureAwait(false);

            // Assert
            Assert.IsFalse(result.IsValid);
            StringAssert.Contains(result.Errors[0], "invalid text encoding", StringComparison.Ordinal);
        }

        [TestMethod]
        [DataRow("json", false)]
        [DataRow("json", true)]
        [DataRow("xml", false)]
        [DataRow("xml", true)]
        [DataRow("yaml", false)]
        [DataRow("yaml", true)]
        public async Task ValidateThrowsForAMissingFile(string format,
            bool asynchronous)
        {
            // Setup
            using var connection = new CustomDbConnection();

            // Act/Assert
            await Assert.ThrowsExactlyAsync<FileNotFoundException>(() => Validate(connection, format, asynchronous)).ConfigureAwait(false);
        }

        [TestMethod]
        [DataRow("json")]
        [DataRow("xml")]
        [DataRow("yaml")]
        public async Task ValidateThrowsForInvalidArguments(string format)
        {
            // Setup
            using var connection = new CustomDbConnection();

            // Act/Assert
            await Assert.ThrowsExactlyAsync<ArgumentNullException>(() => Validate(null, format, true)).ConfigureAwait(false);
            await Assert.ThrowsExactlyAsync<ArgumentNullException>(() => Validate(null, format, false)).ConfigureAwait(false);
            await Assert.ThrowsExactlyAsync<ArgumentException>(() => Validate(connection, format, true, filename: " ")).ConfigureAwait(false);
            await Assert.ThrowsExactlyAsync<ArgumentException>(() => Validate(connection, format, false, filename: " ")).ConfigureAwait(false);
        }

        [TestMethod]
        [DataRow("json", false)]
        [DataRow("json", true)]
        [DataRow("xml", false)]
        [DataRow("xml", true)]
        [DataRow("yaml", false)]
        [DataRow("yaml", true)]
        public async Task ValidateForwardsTheTraceKeyAndNeverAccessesTheDatabase(string format,
            bool asynchronous)
        {
            // Setup
            WriteValidDefinition(format);
            using var connection = new CustomDbConnection();
            var trace = new RecordingTrace(asynchronous);

            // Act
            await Validate(connection, format, asynchronous, traceKey: "validate-test", trace: trace).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(1, trace.Keys.Count);
            Assert.AreEqual("validate-test", trace.Keys.Single());
            Assert.AreEqual(0, connection.Details.Count);
        }

        [TestMethod]
        [DataRow("json", false, "ValidateDefinitionFromJson")]
        [DataRow("json", true, "ValidateDefinitionFromJson")]
        [DataRow("xml", false, "ValidateDefinitionFromXml")]
        [DataRow("xml", true, "ValidateDefinitionFromXml")]
        [DataRow("yaml", false, "ValidateDefinitionFromYaml")]
        [DataRow("yaml", true, "ValidateDefinitionFromYaml")]
        public async Task ValidateUsesTheDefaultTraceKey(string format,
            bool asynchronous,
            string expectedKey)
        {
            // Setup
            WriteValidDefinition(format);
            using var connection = new CustomDbConnection();
            var trace = new RecordingTrace(asynchronous);

            // Act
            await Validate(connection, format, asynchronous, trace: trace).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(expectedKey, trace.Keys.Single(), StringComparer.Ordinal);
        }

        [TestMethod]
        [DataRow("json", false)]
        [DataRow("json", true)]
        [DataRow("xml", false)]
        [DataRow("xml", true)]
        [DataRow("yaml", false)]
        [DataRow("yaml", true)]
        public async Task ValidateReturnsNullWhenTheTraceCancelsTheRead(string format,
            bool asynchronous)
        {
            // Setup
            using var connection = new CustomDbConnection();
            var trace = new RecordingTrace(asynchronous) { Cancel = true };

            // Act
            var result = await Validate(connection, format, asynchronous, trace: trace).ConfigureAwait(false);

            // Assert
            Assert.IsNull(result);
            Assert.AreEqual(0, connection.Details.Count);
        }

        [TestMethod]
        [DataRow("json")]
        [DataRow("xml")]
        [DataRow("yaml")]
        public async Task ValidateAsyncThrowsWhenTheTokenIsCancelled(string format)
        {
            // Setup
            WriteValidDefinition(format);
            using var connection = new CustomDbConnection();
            using var source = new CancellationTokenSource();
            source.Cancel();

            // Act/Assert
            await Assert.ThrowsExactlyAsync<OperationCanceledException>(() => Validate(connection, format, true, cancellationToken: source.Token)).ConfigureAwait(false);
        }

        [TestMethod]
        [DataRow("json", false)]
        [DataRow("json", true)]
        [DataRow("xml", false)]
        [DataRow("xml", true)]
        [DataRow("yaml", false)]
        [DataRow("yaml", true)]
        public async Task ValidateReportsAllTheStrictErrorsOfAReadableFile(string format,
            bool asynchronous)
        {
            // Setup
            var parent = TableSchemaFactory.CreateTable("Parent", "Id", "Code");
            TableSchemaFactory.SetPrimaryKey(parent, "PK_Parent", "Id");
            var child = TableSchemaFactory.CreateTable("Child", "Id", "ParentCode");
            TableSchemaFactory.SetPrimaryKey(child, "PK_Child", "Id");
            TableSchemaFactory.AddForeignKey(child, "FK_Child", parent.Table, new[] { "ParentCode" }, new[] { "Code" });
            child.Indexes.Add(new IndexInfo("IX_Same") { Columns = new[] { "Id" } });
            child.Indexes.Add(new IndexInfo("IX_Same") { Columns = new[] { "ParentCode" } });
            WriteDefinition(format, RelatedTableBehavior.ParentsAndChildren, child.Table, parent, child);
            using var connection = new CustomDbConnection();

            // Act
            var result = await Validate(connection, format, asynchronous).ConfigureAwait(false);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsNull(result.Definition);
            Assert.AreEqual(2, result.Errors.Count);
            Assert.IsTrue(result.Errors.Any(error => error.Contains("references the columns (Code)", StringComparison.Ordinal)));
            Assert.IsTrue(result.Errors.Any(error => error.Contains("index name 'IX_Same' is used more than once", StringComparison.Ordinal)));
        }

        [TestMethod]
        [DataRow("json", false)]
        [DataRow("json", true)]
        [DataRow("xml", false)]
        [DataRow("xml", true)]
        [DataRow("yaml", false)]
        [DataRow("yaml", true)]
        public async Task ValidateReportsTheWarningsOfAValidFile(string format,
            bool asynchronous)
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Child");
            table.Columns.Add(TableSchemaFactory.CreateColumn("Code", typeof(string), 1, isNullable: true));
            TableSchemaFactory.SetPrimaryKey(table, "PK_Child", "Code");
            WriteDefinition(format, RelatedTableBehavior.TableOnly, table.Table, table);
            using var connection = new CustomDbConnection();

            // Act
            var result = await Validate(connection, format, asynchronous).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result.IsValid);
            Assert.IsNotNull(result.Definition);
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(1, result.Warnings.Count);
            StringAssert.Contains(result.Warnings[0], "primary key column 'Code' is nullable", StringComparison.Ordinal);
        }

        [TestMethod]
        [DataRow("json", false)]
        [DataRow("json", true)]
        [DataRow("xml", false)]
        [DataRow("xml", true)]
        [DataRow("yaml", false)]
        [DataRow("yaml", true)]
        public async Task ValidateWarnsAboutAForeignKeyToATableThatIsNotInTheFile(string format,
            bool asynchronous)
        {
            // Setup
            var parent = TableSchemaFactory.CreateTable("Parent", "Id");
            TableSchemaFactory.SetPrimaryKey(parent, "PK_Parent", "Id");
            var child = TableSchemaFactory.CreateTable("Child", "Id", "ParentId");
            TableSchemaFactory.SetPrimaryKey(child, "PK_Child", "Id");
            TableSchemaFactory.AddForeignKey(child, "FK_Child", parent.Table, new[] { "ParentId" }, new[] { "Id" });
            WriteDefinition(format, RelatedTableBehavior.Parents, child.Table, child);
            using var connection = new CustomDbConnection();

            // Act
            var result = await Validate(connection, format, asynchronous).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(1, result.Warnings.Count);
            StringAssert.Contains(result.Warnings[0], "cannot be verified", StringComparison.Ordinal);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ValidateReportsAValidFileWithKeysThatAreConsistentWithTheirColumns(bool asynchronous)
        {
            // Setup
            var parent = TableSchemaFactory.CreateTable("Parent", "Id", "Code");
            TableSchemaFactory.SetPrimaryKey(parent, "PK_Parent", "Id");
            parent.UniqueConstraints.Add(new UniqueConstraintInfo("UQ_Parent") { Columns = new[] { "Code" } });
            var child = TableSchemaFactory.CreateTable("Child", "Id", "ParentCode");
            TableSchemaFactory.SetPrimaryKey(child, "PK_Child", "Id");
            TableSchemaFactory.AddForeignKey(child, "FK_Child", parent.Table, new[] { "ParentCode" }, new[] { "Code" });
            child.Indexes.Add(new IndexInfo("IX_Child") { Columns = new[] { "ParentCode" } });
            WriteDefinition("json", RelatedTableBehavior.ParentsAndChildren, child.Table, parent, child);
            using var connection = new CustomDbConnection();

            // Act
            var result = await Validate(connection, "json", asynchronous).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(0, result.Warnings.Count);
            Assert.AreEqual(2, result.Definition.Tables.Count);
        }

        [TestMethod]
        [DataRow("json", false)]
        [DataRow("json", true)]
        [DataRow("xml", false)]
        [DataRow("xml", true)]
        [DataRow("yaml", false)]
        [DataRow("yaml", true)]
        public async Task ValidateWarnsAboutNamesThatDifferOnlyByCaseAndConstraintNamesOfMoreThanOneTable(string format,
            bool asynchronous)
        {
            // Setup
            var first = TableSchemaFactory.CreateTable("First", "Name", "name");
            TableSchemaFactory.SetPrimaryKey(first, "PK_Shared", "Name");
            var second = TableSchemaFactory.CreateTable("Second", "Id");
            TableSchemaFactory.SetPrimaryKey(second, "PK_Shared", "Id");
            WriteDefinition(format, RelatedTableBehavior.ParentsAndChildren, first.Table, first, second);
            using var connection = new CustomDbConnection();

            // Act
            var result = await Validate(connection, format, asynchronous).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(2, result.Warnings.Count);
            Assert.IsTrue(result.Warnings.Any(warning => warning.Contains("column names (Name, name) differ only by case", StringComparison.Ordinal)));
            Assert.IsTrue(result.Warnings.Any(warning => warning.Contains("constraint name 'PK_Shared' is used by more than one table", StringComparison.Ordinal)));
        }

        [TestMethod]
        [DataRow("json", false)]
        [DataRow("json", true)]
        [DataRow("xml", false)]
        [DataRow("xml", true)]
        [DataRow("yaml", false)]
        [DataRow("yaml", true)]
        public async Task ValidateThrowsTheFirstProblemOfAReadableFileWhenThrowOnErrorIsTrue(string format,
            bool asynchronous)
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Child", "Id");
            TableSchemaFactory.SetPrimaryKey(table, "PK_Child", "Missing");
            table.Indexes.Add(new IndexInfo("IX_Child") { Columns = new[] { "AlsoMissing" } });
            WriteDefinition(format, RelatedTableBehavior.TableOnly, table.Table, table);
            using var connection = new CustomDbConnection();

            // Act/Assert
            var exception = await Assert.ThrowsExactlyAsync<InvalidDataException>(() => Validate(connection, format, asynchronous, throwOnError: true)).ConfigureAwait(false);
            StringAssert.Contains(exception.Message, "primary key 'PK_Child'", StringComparison.Ordinal);
            Assert.IsFalse(exception.Message.Contains("IX_Child", StringComparison.Ordinal));
        }

        [TestMethod]
        [DataRow("json", false)]
        [DataRow("json", true)]
        [DataRow("xml", false)]
        [DataRow("xml", true)]
        [DataRow("yaml", false)]
        [DataRow("yaml", true)]
        public async Task ValidateReportsTheErrorsInTheResultWhenThrowOnErrorIsFalse(string format,
            bool asynchronous)
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Child", "Id");
            TableSchemaFactory.SetPrimaryKey(table, "PK_Child", "Missing");
            WriteDefinition(format, RelatedTableBehavior.TableOnly, table.Table, table);
            using var connection = new CustomDbConnection();

            // Act
            var result = await Validate(connection, format, asynchronous, throwOnError: false).ConfigureAwait(false);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
        }

        [TestMethod]
        [DataRow("json", "{ not json", "malformed", false)]
        [DataRow("json", "{ not json", "malformed", true)]
        [DataRow("json", "[]", "root must be an object", false)]
        [DataRow("json", "[]", "root must be an object", true)]
        [DataRow("json", "{}", "Missing property: format", false)]
        [DataRow("json", "{}", "Missing property: format", true)]
        [DataRow("xml", "<repoDbSchema>", "malformed", false)]
        [DataRow("xml", "<repoDbSchema>", "malformed", true)]
        [DataRow("xml", "<other />", "Invalid schema XML root", false)]
        [DataRow("xml", "<other />", "Invalid schema XML root", true)]
        [DataRow("yaml", "- a", "malformed", false)]
        [DataRow("yaml", "- a", "malformed", true)]
        [DataRow("yaml", "", "The YAML definition is empty", false)]
        [DataRow("yaml", "", "The YAML definition is empty", true)]
        public async Task ValidateThrowsAFileThatCannotBeReadWhenThrowOnErrorIsTrue(string format,
            string content,
            string expectedMessage,
            bool asynchronous)
        {
            // Setup
            File.WriteAllText(_filename, content);
            using var connection = new CustomDbConnection();

            // Act/Assert
            var exception = await Assert.ThrowsExactlyAsync<InvalidDataException>(() => Validate(connection, format, asynchronous, throwOnError: true)).ConfigureAwait(false);
            StringAssert.Contains(exception.Message, expectedMessage, StringComparison.Ordinal);
        }

        [TestMethod]
        [DataRow("json", false)]
        [DataRow("json", true)]
        [DataRow("xml", false)]
        [DataRow("xml", true)]
        [DataRow("yaml", false)]
        [DataRow("yaml", true)]
        public async Task ValidateThrowsAFileWithAnInvalidTextEncodingWhenThrowOnErrorIsTrue(string format,
            bool asynchronous)
        {
            // Setup
            File.WriteAllBytes(_filename, new byte[] { 0xC3, 0x28 });
            using var connection = new CustomDbConnection();

            // Act/Assert
            var exception = await Assert.ThrowsExactlyAsync<InvalidDataException>(() => Validate(connection, format, asynchronous, throwOnError: true)).ConfigureAwait(false);
            StringAssert.Contains(exception.Message, "invalid text encoding", StringComparison.Ordinal);
        }

        [TestMethod]
        [DataRow("json", false)]
        [DataRow("json", true)]
        [DataRow("xml", false)]
        [DataRow("xml", true)]
        [DataRow("yaml", false)]
        [DataRow("yaml", true)]
        public async Task ValidateReturnsAValidFileWhenThrowOnErrorIsTrue(string format,
            bool asynchronous)
        {
            // Setup
            WriteValidDefinition(format);
            using var connection = new CustomDbConnection();

            // Act
            var result = await Validate(connection, format, asynchronous, throwOnError: true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual("FileTable", result.Definition.Tables.Single().Table.Name);
        }

        [TestMethod]
        [DataRow("json", false)]
        [DataRow("json", true)]
        [DataRow("xml", false)]
        [DataRow("xml", true)]
        [DataRow("yaml", false)]
        [DataRow("yaml", true)]
        public async Task ValidateDoesNotThrowTheWarningsOfAFileWhenThrowOnErrorIsTrue(string format,
            bool asynchronous)
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Child");
            table.Columns.Add(TableSchemaFactory.CreateColumn("Code", typeof(string), 1, isNullable: true));
            TableSchemaFactory.SetPrimaryKey(table, "PK_Child", "Code");
            WriteDefinition(format, RelatedTableBehavior.TableOnly, table.Table, table);
            using var connection = new CustomDbConnection();

            // Act
            var result = await Validate(connection, format, asynchronous, throwOnError: true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(1, result.Warnings.Count);
        }

        [TestMethod]
        [DataRow("json", false)]
        [DataRow("json", true)]
        [DataRow("xml", false)]
        [DataRow("xml", true)]
        [DataRow("yaml", false)]
        [DataRow("yaml", true)]
        public async Task ValidateThrowsAMissingFileWhateverThrowOnErrorIs(string format,
            bool asynchronous)
        {
            // Setup
            using var connection = new CustomDbConnection();

            // Act/Assert
            await Assert.ThrowsExactlyAsync<FileNotFoundException>(() => Validate(connection, format, asynchronous, throwOnError: true)).ConfigureAwait(false);
            await Assert.ThrowsExactlyAsync<FileNotFoundException>(() => Validate(connection, format, asynchronous, throwOnError: false)).ConfigureAwait(false);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Writes a valid definition file, in the given format.
        /// </summary>
        /// <param name="format">The format of the file: json, xml or yaml.</param>
        private void WriteValidDefinition(string format)
        {
            var schema = new TableSchema(new TableInfo("FileTable", "FileSchema", "FileDatabase", "FileProvider"));
            schema.Columns.Add(new ColumnInfo { Ordinal = 1, Field = new DbField("Id", true, false, false, typeof(int), null, null, null, null) });
            schema.PrimaryKey = new PrimaryKeyInfo("PK_FileTable") { Columns = new[] { "Id" } };
            var definition = SchemaDefinition.Create(schema.Table, new[] { schema }, RelatedTableBehavior.TableOnly);
            File.WriteAllText(_filename, SchemaDefinitionSerializer.Serialize(definition, format));
        }

        /// <summary>
        /// Writes a definition file of the given tables, in the given format.
        /// </summary>
        /// <param name="format">The format of the file: json, xml or yaml.</param>
        /// <param name="behavior">The related tables that the definition includes.</param>
        /// <param name="root">The identity of the table that the definition was extracted for.</param>
        /// <param name="tables">The tables of the definition.</param>
        private void WriteDefinition(string format,
            RelatedTableBehavior behavior,
            TableInfo root,
            params TableSchema[] tables)
        {
            var definition = SchemaDefinition.Create(root, tables, behavior);
            File.WriteAllText(_filename, SchemaDefinitionSerializer.Serialize(definition, format));
        }

        /// <summary>
        /// Validates the definition file with the extension method of the format, in the sync or async way.
        /// The default tracking key of the extension method is used if no tracking key is given.
        /// </summary>
        /// <param name="connection">The connection that the extension method is invoked on.</param>
        /// <param name="format">The format of the file: json, xml or yaml.</param>
        /// <param name="asynchronous">Whether the asynchronous extension method is used.</param>
        /// <param name="filename">The name of the file (the file of the test if it is <c>null</c>).</param>
        /// <param name="traceKey">The tracking key (the default one of the extension method if it is <c>null</c>).</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="cancellationToken">The cancellation token of the asynchronous extension method.</param>
        /// <param name="throwOnError">Whether the first problem is thrown instead of being reported in the result.</param>
        /// <returns>A task that represents the operation. The task result is the outcome of the validation.</returns>
        private async Task<ValidateDefinitionResult> Validate(IDbConnection connection,
            string format,
            bool asynchronous,
            string filename = null,
            string traceKey = null,
            ITrace trace = null,
            CancellationToken cancellationToken = default,
            bool throwOnError = false)
        {
            filename ??= _filename;
            switch (format)
            {
                case "json":
                    return await ValidateJson(connection, asynchronous, filename, throwOnError, traceKey, trace, cancellationToken).ConfigureAwait(false);
                case "xml":
                    return await ValidateXml(connection, asynchronous, filename, throwOnError, traceKey, trace, cancellationToken).ConfigureAwait(false);
                default:
                    return await ValidateYaml(connection, asynchronous, filename, throwOnError, traceKey, trace, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Validates the definition file with the JSON extension method.
        /// </summary>
        /// <param name="connection">The connection that the extension method is invoked on.</param>
        /// <param name="asynchronous">Whether the asynchronous extension method is used.</param>
        /// <param name="filename">The name of the file.</param>
        /// <param name="throwOnError">Whether the first problem is thrown instead of being reported in the result.</param>
        /// <param name="traceKey">The tracking key (the default one of the extension method if it is <c>null</c>).</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="cancellationToken">The cancellation token of the asynchronous extension method.</param>
        /// <returns>A task that represents the operation. The task result is the outcome of the validation.</returns>
        private static Task<ValidateDefinitionResult> ValidateJson(IDbConnection connection,
            bool asynchronous,
            string filename,
            bool throwOnError,
            string traceKey,
            ITrace trace,
            CancellationToken cancellationToken)
        {
            if (traceKey == null)
            {
                return asynchronous
                    ? connection.ValidateDefinitionFromJsonAsync(filename, throwOnError, trace: trace, cancellationToken: cancellationToken)
                    : Task.FromResult(connection.ValidateDefinitionFromJson(filename, throwOnError, trace: trace));
            }
            return asynchronous
                ? connection.ValidateDefinitionFromJsonAsync(filename, throwOnError, traceKey, trace, cancellationToken)
                : Task.FromResult(connection.ValidateDefinitionFromJson(filename, throwOnError, traceKey, trace));
        }

        /// <summary>
        /// Validates the definition file with the XML extension method.
        /// </summary>
        /// <param name="connection">The connection that the extension method is invoked on.</param>
        /// <param name="asynchronous">Whether the asynchronous extension method is used.</param>
        /// <param name="filename">The name of the file.</param>
        /// <param name="throwOnError">Whether the first problem is thrown instead of being reported in the result.</param>
        /// <param name="traceKey">The tracking key (the default one of the extension method if it is <c>null</c>).</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="cancellationToken">The cancellation token of the asynchronous extension method.</param>
        /// <returns>A task that represents the operation. The task result is the outcome of the validation.</returns>
        private static Task<ValidateDefinitionResult> ValidateXml(IDbConnection connection,
            bool asynchronous,
            string filename,
            bool throwOnError,
            string traceKey,
            ITrace trace,
            CancellationToken cancellationToken)
        {
            if (traceKey == null)
            {
                return asynchronous
                    ? connection.ValidateDefinitionFromXmlAsync(filename, throwOnError, trace: trace, cancellationToken: cancellationToken)
                    : Task.FromResult(connection.ValidateDefinitionFromXml(filename, throwOnError, trace: trace));
            }
            return asynchronous
                ? connection.ValidateDefinitionFromXmlAsync(filename, throwOnError, traceKey, trace, cancellationToken)
                : Task.FromResult(connection.ValidateDefinitionFromXml(filename, throwOnError, traceKey, trace));
        }

        /// <summary>
        /// Validates the definition file with the YAML extension method.
        /// </summary>
        /// <param name="connection">The connection that the extension method is invoked on.</param>
        /// <param name="asynchronous">Whether the asynchronous extension method is used.</param>
        /// <param name="filename">The name of the file.</param>
        /// <param name="throwOnError">Whether the first problem is thrown instead of being reported in the result.</param>
        /// <param name="traceKey">The tracking key (the default one of the extension method if it is <c>null</c>).</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="cancellationToken">The cancellation token of the asynchronous extension method.</param>
        /// <returns>A task that represents the operation. The task result is the outcome of the validation.</returns>
        private static Task<ValidateDefinitionResult> ValidateYaml(IDbConnection connection,
            bool asynchronous,
            string filename,
            bool throwOnError,
            string traceKey,
            ITrace trace,
            CancellationToken cancellationToken)
        {
            if (traceKey == null)
            {
                return asynchronous
                    ? connection.ValidateDefinitionFromYamlAsync(filename, throwOnError, trace: trace, cancellationToken: cancellationToken)
                    : Task.FromResult(connection.ValidateDefinitionFromYaml(filename, throwOnError, trace: trace));
            }
            return asynchronous
                ? connection.ValidateDefinitionFromYamlAsync(filename, throwOnError, traceKey, trace, cancellationToken)
                : Task.FromResult(connection.ValidateDefinitionFromYaml(filename, throwOnError, traceKey, trace));
        }

        #endregion
    }
}
