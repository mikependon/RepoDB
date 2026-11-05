#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Schema.Core.UnitTests.Helpers
{
    [TestClass]
    public class SchemaExpressionDialectTest
    {
        #region GetFamily

        [TestMethod]
        public void TestGetFamilyOfSqlServer()
        {
            // Act
            var family = SchemaExpressionDialect.GetFamily("MSSQL");

            // Assert
            Assert.AreEqual("SqlServer", family);
        }

        [TestMethod]
        public void TestGetFamilyOfSqliteMicrosoft()
        {
            // Act
            var family = SchemaExpressionDialect.GetFamily("MSSQLITE");

            // Assert
            Assert.AreEqual("Sqlite", family);
        }

        [TestMethod]
        public void TestGetFamilyOfSqliteAhtola()
        {
            // Act
            var family = SchemaExpressionDialect.GetFamily("AHTOLA");

            // Assert
            Assert.AreEqual("Sqlite", family);
        }

        [TestMethod]
        public void TestGetFamilyOfSqliteTurso()
        {
            // Act
            var family = SchemaExpressionDialect.GetFamily("TURSO");

            // Assert
            Assert.AreEqual("Sqlite", family);
        }

        [TestMethod]
        public void TestGetFamilyOfMySql()
        {
            // Act
            var family = SchemaExpressionDialect.GetFamily("MYSQL");

            // Assert
            Assert.AreEqual("MySql", family);
        }

        [TestMethod]
        public void TestGetFamilyOfMySqlConnector()
        {
            // Act
            var family = SchemaExpressionDialect.GetFamily("MYSQLC");

            // Assert
            Assert.AreEqual("MySql", family);
        }

        [TestMethod]
        public void TestGetFamilyOfMariaDb()
        {
            // Act
            var family = SchemaExpressionDialect.GetFamily("MARIADB");

            // Assert
            Assert.AreEqual("MySql", family);
        }

        [TestMethod]
        public void TestGetFamilyOfAuroraMySql()
        {
            // Act
            var family = SchemaExpressionDialect.GetFamily("AURORAMYSQL");

            // Assert
            Assert.AreEqual("MySql", family);
        }

        [TestMethod]
        public void TestGetFamilyOfPostgreSql()
        {
            // Act
            var family = SchemaExpressionDialect.GetFamily("PGSQL");

            // Assert
            Assert.AreEqual("PostgreSql", family);
        }

        [TestMethod]
        public void TestGetFamilyOfClickHouse()
        {
            // Act
            var family = SchemaExpressionDialect.GetFamily("CLICKHOUSE");

            // Assert
            Assert.AreEqual("ClickHouse", family);
        }

        [TestMethod]
        public void TestGetFamilyOfDb2()
        {
            // Act
            var family = SchemaExpressionDialect.GetFamily("DB2");

            // Assert
            Assert.AreEqual("Db2", family);
        }

        [TestMethod]
        public void TestGetFamilyOfDuckDb()
        {
            // Act
            var family = SchemaExpressionDialect.GetFamily("DUCKDB");

            // Assert
            Assert.AreEqual("DuckDb", family);
        }

        [TestMethod]
        public void TestGetFamilyOfFirebird()
        {
            // Act
            var family = SchemaExpressionDialect.GetFamily("FIREBIRD");

            // Assert
            Assert.AreEqual("Firebird", family);
        }

        [TestMethod]
        public void TestGetFamilyOfSapHana()
        {
            // Act
            var family = SchemaExpressionDialect.GetFamily("HANA");

            // Assert
            Assert.AreEqual("SapHana", family);
        }

        [TestMethod]
        public void TestGetFamilyOfOracle()
        {
            // Act
            var family = SchemaExpressionDialect.GetFamily("ORACLE");

            // Assert
            Assert.AreEqual("Oracle", family);
        }

        [TestMethod]
        public void TestGetFamilyOfVertica()
        {
            // Act
            var family = SchemaExpressionDialect.GetFamily("VERTICA");

            // Assert
            Assert.AreEqual("Vertica", family);
        }

        [TestMethod]
        public void TestGetFamilyIgnoresTheCaseOfTheProvider()
        {
            // Act
            var family = SchemaExpressionDialect.GetFamily("mssql");

            // Assert
            Assert.AreEqual("SqlServer", family);
        }

        [TestMethod]
        public void TestGetFamilyOfAnUnknownProviderIsNull()
        {
            // Act
            var family = SchemaExpressionDialect.GetFamily("FileProvider");

            // Assert
            Assert.IsNull(family);
        }

        [TestMethod]
        public void TestGetFamilyOfANullProviderIsNull()
        {
            // Act
            var family = SchemaExpressionDialect.GetFamily(null);

            // Assert
            Assert.IsNull(family);
        }

        #endregion

        #region GetQuotes

        [TestMethod]
        public void TestGetQuotesOfSqlServerAreTheBracketsAndTheDoubleQuotes()
        {
            // Act
            var quotes = SchemaExpressionDialect.GetQuotes("SqlServer");

            // Assert
            CollectionAssert.AreEqual(new[] { ('[', ']'), ('"', '"') }, quotes.ToArray());
        }

        [TestMethod]
        public void TestGetQuotesOfSqliteAreTheBracketsTheDoubleQuotesAndTheBackticks()
        {
            // Act
            var quotes = SchemaExpressionDialect.GetQuotes("Sqlite");

            // Assert
            CollectionAssert.AreEqual(new[] { ('[', ']'), ('"', '"'), ('`', '`') }, quotes.ToArray());
        }

        [TestMethod]
        public void TestGetQuotesOfMySqlAreTheBackticks()
        {
            // Act
            var quotes = SchemaExpressionDialect.GetQuotes("MySql");

            // Assert
            CollectionAssert.AreEqual(new[] { ('`', '`') }, quotes.ToArray());
        }

        [TestMethod]
        public void TestGetQuotesOfClickHouseAreTheBackticksAndTheDoubleQuotes()
        {
            // Act
            var quotes = SchemaExpressionDialect.GetQuotes("ClickHouse");

            // Assert
            CollectionAssert.AreEqual(new[] { ('`', '`'), ('"', '"') }, quotes.ToArray());
        }

        [TestMethod]
        public void TestGetQuotesOfAnyOtherFamilyAreTheDoubleQuotes()
        {
            // Act
            var quotes = SchemaExpressionDialect.GetQuotes("PostgreSql");

            // Assert
            CollectionAssert.AreEqual(new[] { ('"', '"') }, quotes.ToArray());
        }

        #endregion
    }
}
