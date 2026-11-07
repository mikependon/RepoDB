#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;
using RepoDb.Schema.PostgreSql.IntegrationTests.Setup;

namespace RepoDb.Schema.PostgreSql.IntegrationTests
{
    /// <summary>
    /// Loads a definition that is read from SQL Server into PostgreSQL: the default values are written in the format of PostgreSQL,
    /// and a row that is inserted without a value for the columns gets them.
    /// </summary>
    [TestClass]
    public class PostgreSqlLoadDefinitionDefaultValueTest
    {
        #region Private Variables

        private string _filename;
        private string _schemaName;

        #endregion

        #region Setup

        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            _schemaName = "dflt" + Guid.NewGuid().ToString("N").Substring(0,
                8);
            _filename = Path.Combine(Path.GetTempPath(),
                "repodb-load-default-" + Guid.NewGuid().ToString("N"));
            WriteDefinition();
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(_filename))
            {
                File.Delete(_filename);
            }

            using (var connection = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                connection.ExecuteNonQuery($"DROP SCHEMA IF EXISTS \"{_schemaName}\" CASCADE");
            }
        }

        #endregion

        #region Private Methods

        private static ColumnInfo CreateColumn(int ordinal,
            string name,
            Type type,
            string nativeType,
            ColumnDefaultValue kind,
            string expression,
            int? size = null) =>
            new ColumnInfo
            {
                Ordinal = ordinal,
                NativeType = nativeType,
                DefaultValue = kind,
                DefaultExpression = expression,
                Field = new DbField(name, false, false, true, type, size, null, null, nativeType, expression != null, "MSSQL")
            };

        private void WriteDefinition()
        {
            var schema = new TableSchema(new TableInfo("Items",
                _schemaName,
                "Northwind",
                "MSSQL"));
            schema.Columns.Add(new ColumnInfo { Ordinal = 1, Field = new DbField("Id", true, false, false, typeof(int), null, null, null, "int", false, "MSSQL") });
            schema.Columns.Add(CreateColumn(2, "Flag", typeof(bool), "bit", ColumnDefaultValue.Literal, "((0))"));
            schema.Columns.Add(CreateColumn(3, "Price", typeof(decimal), "money", ColumnDefaultValue.Literal, "((0))"));
            schema.Columns.Add(CreateColumn(4, "Label", typeof(string), "nvarchar", ColumnDefaultValue.Literal, "(N'N/A')", 20));
            schema.Columns.Add(CreateColumn(5, "Born", typeof(DateTime), "datetime", ColumnDefaultValue.Literal, "('1900-01-01T00:00:00')"));
            schema.Columns.Add(CreateColumn(6, "Stamp", typeof(DateTime), "datetime", ColumnDefaultValue.CurrentTimestamp, "(getdate())"));
            schema.Columns.Add(CreateColumn(7, "Token", typeof(Guid), "uniqueidentifier", ColumnDefaultValue.NewGuid, "(newid())"));
            schema.PrimaryKey = new PrimaryKeyInfo("PK_Items") { Columns = new[] { "Id" } };
            var definition = SchemaDefinition.Create(schema.Table,
                new[] { schema },
                RelatedTableBehavior.TableOnly);
            File.WriteAllText(_filename,
                SchemaDefinitionSerializer.Serialize(definition,
                    "json"));
        }

        private LoadDefinitionResult Load()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                return connection.LoadDefinitionFromJson(_filename);
            }
        }

        private async Task<LoadDefinitionResult> LoadAsync()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                return await connection.LoadDefinitionFromJsonAsync(_filename);
            }
        }

        private object ReadDefault(string column)
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                connection.Open();
                connection.ExecuteNonQuery($"INSERT INTO \"{_schemaName}\".\"Items\" (\"Id\") VALUES (1)");
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT \"{column}\" FROM \"{_schemaName}\".\"Items\" WHERE \"Id\" = 1";
                    return command.ExecuteScalar();
                }
            }
        }

        #endregion

        #region Public Methods

        [TestMethod]
        public void TestLoadDefinitionFromJsonHasNoErrors()
        {
            // Act
            var result = Load();

            // Assert
            Assert.AreEqual(0, result.CopyResult.Errors.Count);
        }

        [TestMethod]
        public void TestLoadDefinitionFromJsonHasNoWarnings()
        {
            // Act
            var result = Load();

            // Assert
            Assert.AreEqual(0, result.CopyResult.Items.Single().Warnings.Count);
        }

        [TestMethod]
        public void TestLoadDefinitionFromJsonGivesABitColumnTheDefaultFalse()
        {
            // Setup
            Load();

            // Act
            var value = ReadDefault("Flag");

            // Assert
            Assert.AreEqual(false, value);
        }

        [TestMethod]
        public void TestLoadDefinitionFromJsonGivesAMoneyColumnTheDefaultZero()
        {
            // Setup
            Load();

            // Act
            var value = ReadDefault("Price");

            // Assert
            Assert.AreEqual(0m, value);
        }

        [TestMethod]
        public void TestLoadDefinitionFromJsonGivesATextColumnItsDefaultWithoutThePrefix()
        {
            // Setup
            Load();

            // Act
            var value = ReadDefault("Label");

            // Assert
            Assert.AreEqual("N/A", value);
        }

        [TestMethod]
        public void TestLoadDefinitionFromJsonGivesADateTimeColumnItsDefaultDate()
        {
            // Setup
            Load();

            // Act
            var value = ReadDefault("Born");

            // Assert
            Assert.AreEqual(new DateTime(1900, 1, 1), value);
        }

        [TestMethod]
        public void TestLoadDefinitionFromJsonGivesAColumnTheCurrentTimeForGetdate()
        {
            // Setup
            Load();

            // Act
            var value = (DateTime)ReadDefault("Stamp");

            // Assert
            Assert.IsTrue(Math.Abs((value - DateTime.UtcNow).TotalHours) < 24);
        }

        [TestMethod]
        public void TestLoadDefinitionFromJsonGivesAColumnANewGuidForNewid()
        {
            // Setup
            Load();

            // Act
            var value = (Guid)ReadDefault("Token");

            // Assert
            Assert.AreNotEqual(Guid.Empty, value);
        }

        [TestMethod]
        public async Task TestLoadDefinitionFromJsonAsyncHasNoErrors()
        {
            // Act
            var result = await LoadAsync();

            // Assert
            Assert.AreEqual(0, result.CopyResult.Errors.Count);
        }

        [TestMethod]
        public async Task TestLoadDefinitionFromJsonAsyncGivesABitColumnTheDefaultFalse()
        {
            // Setup
            await LoadAsync();

            // Act
            var value = ReadDefault("Flag");

            // Assert
            Assert.AreEqual(false, value);
        }

        #endregion
    }
}
