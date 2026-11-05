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
    /// Loads a definition that is read from SQL Server into PostgreSQL: the schema is created, the quoted names of its check constraints are translated,
    /// and the check constraint that uses a function that is not portable is left out and reported.
    /// </summary>
    [TestClass]
    public class PostgreSqlLoadDefinitionExpressionTest
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
            _schemaName = "nwind" + Guid.NewGuid().ToString("N").Substring(0,
                8);
            _filename = Path.Combine(Path.GetTempPath(),
                "repodb-load-expression-" + Guid.NewGuid().ToString("N"));
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

        private static TableSchema CreateEmployees(string schemaName)
        {
            var schema = new TableSchema(new TableInfo("Employees",
                schemaName,
                "Northwind",
                "MSSQL"));
            schema.Columns.Add(new ColumnInfo { Ordinal = 1, Field = new DbField("Id", true, false, false, typeof(int), null, null, null, null) });
            schema.Columns.Add(new ColumnInfo { Ordinal = 2, Field = new DbField("UnitPrice", false, false, false, typeof(int), null, null, null, null) });
            schema.Columns.Add(new ColumnInfo { Ordinal = 3, Field = new DbField("BirthDate", false, false, true, typeof(DateTime), null, null, null, null) });
            schema.PrimaryKey = new PrimaryKeyInfo("PK_Employees") { Columns = new[] { "Id" } };
            schema.CheckConstraints.Add(new CheckConstraintInfo("CK_Birthdate") { Expression = "([BirthDate]<getdate())" });
            schema.CheckConstraints.Add(new CheckConstraintInfo("CK_UnitPrice") { Expression = "([UnitPrice]>=(0))" });
            return schema;
        }

        private void WriteDefinition()
        {
            var schema = CreateEmployees(_schemaName);
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

        private int Insert(int id,
            int unitPrice,
            string birthDate)
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                return connection.ExecuteNonQuery($"INSERT INTO \"{_schemaName}\".\"Employees\" (\"Id\", \"UnitPrice\", \"BirthDate\") VALUES ({id}, {unitPrice}, {birthDate})");
            }
        }

        private long CountSchemas()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                return connection.ExecuteScalar<long>($"SELECT COUNT(*) FROM information_schema.schemata WHERE schema_name = '{_schemaName}'");
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
        public void TestLoadDefinitionFromJsonCreatesTheTable()
        {
            // Act
            var result = Load();

            // Assert
            Assert.AreEqual(TableOutcome.Created, result.CopyResult.Items.Single().Outcome);
        }

        [TestMethod]
        public void TestLoadDefinitionFromJsonCreatesTheSchema()
        {
            // Act
            Load();

            // Assert
            Assert.AreEqual(1L, CountSchemas());
        }

        [TestMethod]
        public void TestLoadDefinitionFromJsonKeepsTheTranslatedCheckConstraint()
        {
            // Setup
            Load();

            // Act
            var exception = Assert.ThrowsExactly<PostgresException>(() => Insert(1,
                -1,
                "NULL"));

            // Assert
            Assert.AreEqual("23514", exception.SqlState);
        }

        [TestMethod]
        public void TestLoadDefinitionFromJsonAcceptsARowThatPassesTheTranslatedCheckConstraint()
        {
            // Setup
            Load();

            // Act
            var affected = Insert(1,
                5,
                "NULL");

            // Assert
            Assert.AreEqual(1, affected);
        }

        [TestMethod]
        public void TestLoadDefinitionFromJsonLeavesOutTheCheckConstraintThatIsNotPortable()
        {
            // Setup
            Load();

            // Act
            var affected = Insert(1,
                5,
                "now() + interval '10 years'");

            // Assert
            Assert.AreEqual(1, affected);
        }

        [TestMethod]
        public void TestLoadDefinitionFromJsonReportsTheCheckConstraintThatIsLeftOut()
        {
            // Act
            var result = Load();

            // Assert
            StringAssert.Contains(result.CopyResult.Items.Single().Warnings.Single(), "'CK_Birthdate'");
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
        public async Task TestLoadDefinitionFromJsonAsyncKeepsTheTranslatedCheckConstraint()
        {
            // Setup
            await LoadAsync();

            // Act
            var exception = Assert.ThrowsExactly<PostgresException>(() => Insert(1,
                -1,
                "NULL"));

            // Assert
            Assert.AreEqual("23514", exception.SqlState);
        }

        [TestMethod]
        public async Task TestLoadDefinitionFromJsonAsyncReportsTheCheckConstraintThatIsLeftOut()
        {
            // Act
            var result = await LoadAsync();

            // Assert
            StringAssert.Contains(result.CopyResult.Items.Single().Warnings.Single(), "'CK_Birthdate'");
        }

        #endregion
    }
}
