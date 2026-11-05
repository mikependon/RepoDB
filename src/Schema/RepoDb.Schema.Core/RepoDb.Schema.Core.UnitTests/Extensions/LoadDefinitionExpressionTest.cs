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
    public class LoadDefinitionExpressionTest
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
            _filename = Path.Combine(Path.GetTempPath(), "repodb-load-expression-" + Guid.NewGuid().ToString("N"));
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
            return composer.Object;
        }

        private static IDbSetting CreateSetting()
        {
            var setting = new Mock<IDbSetting>();
            setting.SetupGet(item => item.IsSchemaSupported).Returns(true);
            setting.SetupGet(item => item.Provider).Returns("PGSQL");
            setting.SetupGet(item => item.OpeningQuote).Returns("\"");
            setting.SetupGet(item => item.ClosingQuote).Returns("\"");
            return setting.Object;
        }

        private void WriteDefinition()
        {
            var schema = new TableSchema(new TableInfo("Employees",
                "dbo",
                "Northwind",
                "MSSQL"));
            schema.Columns.Add(new ColumnInfo { Ordinal = 1, Field = new DbField("Id", true, false, false, typeof(int), null, null, null, null) });
            schema.PrimaryKey = new PrimaryKeyInfo("PK_Employees") { Columns = new[] { "Id" } };
            schema.CheckConstraints.Add(new CheckConstraintInfo("CK_Birthdate") { Expression = "([BirthDate]<getdate())" });
            schema.CheckConstraints.Add(new CheckConstraintInfo("CK_Price") { Expression = "([UnitPrice]>=(0))" });
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
                ScalarResult = text => text.StartsWith("SELECT COUNT", StringComparison.Ordinal) ? 1 : null
            };

        #endregion

        #region Public Methods

        [TestMethod]
        public void TestLoadDefinitionFromJsonTranslatesTheQuotedNamesOfTheCheckConstraints()
        {
            // Setup
            using var connection = CreateConnection();

            // Act
            connection.LoadDefinitionFromJson(_filename);

            // Assert
            Assert.AreEqual("(\"UnitPrice\">=(0))", _composed.Single().CheckConstraints.Single().Expression);
        }

        [TestMethod]
        public void TestLoadDefinitionFromJsonLeavesOutTheCheckConstraintsThatAreNotPortable()
        {
            // Setup
            using var connection = CreateConnection();

            // Act
            connection.LoadDefinitionFromJson(_filename);

            // Assert
            Assert.AreEqual("CK_Price", _composed.Single().CheckConstraints.Single().Name);
        }

        [TestMethod]
        public void TestLoadDefinitionFromJsonReportsTheCheckConstraintThatIsLeftOut()
        {
            // Setup
            using var connection = CreateConnection();

            // Act
            var result = connection.LoadDefinitionFromJson(_filename);

            // Assert
            StringAssert.Contains(result.CopyResult.Items.Single().Warnings.Single(), "'CK_Birthdate'");
        }

        [TestMethod]
        public void TestLoadDefinitionFromJsonDoesNotChangeTheLoadedDefinition()
        {
            // Setup
            using var connection = CreateConnection();

            // Act
            var result = connection.LoadDefinitionFromJson(_filename);

            // Assert
            Assert.AreEqual("([UnitPrice]>=(0))", result.Definition.Tables.Single().CheckConstraints.Last().Expression);
        }

        [TestMethod]
        public async Task TestLoadDefinitionFromJsonAsyncTranslatesTheQuotedNamesOfTheCheckConstraints()
        {
            // Setup
            using var connection = CreateConnection();

            // Act
            await connection.LoadDefinitionFromJsonAsync(_filename);

            // Assert
            Assert.AreEqual("(\"UnitPrice\">=(0))", _composed.Single().CheckConstraints.Single().Expression);
        }

        [TestMethod]
        public async Task TestLoadDefinitionFromJsonAsyncLeavesOutTheCheckConstraintsThatAreNotPortable()
        {
            // Setup
            using var connection = CreateConnection();

            // Act
            await connection.LoadDefinitionFromJsonAsync(_filename);

            // Assert
            Assert.AreEqual("CK_Price", _composed.Single().CheckConstraints.Single().Name);
        }

        #endregion
    }
}
