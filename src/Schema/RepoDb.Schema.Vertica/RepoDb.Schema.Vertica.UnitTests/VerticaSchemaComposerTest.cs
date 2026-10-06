#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Vertica.UnitTests
{
    [TestClass]
    public class VerticaSchemaComposerTest
    {
        #region Helpers

        private static ColumnInfo GetColumn(string name,
            string databaseType,
            Type type,
            int ordinal,
            bool isNullable = true,
            int? size = null,
            byte? precision = null,
            byte? scale = null) =>
            new ColumnInfo
            {
                Ordinal = ordinal,
                Field = new DbField(name, false, false, isNullable, type, size, precision, scale, databaseType)
            };

        private static string ComposeTypeName(string databaseType,
            Type type,
            int? size = null,
            byte? precision = null,
            byte? scale = null) =>
            new VerticaSchemaComposer().ComposeTypeName(GetColumn("Column", databaseType, type, 1, true, size, precision, scale));

        private static TableSchema GetPersonSchema()
        {
            var schema = new TableSchema("Person", "dbo");
            schema.Columns.Add(new ColumnInfo
            {
                Ordinal = 1,
                IdentitySeed = 10,
                IdentityIncrement = 5,
                Field = new DbField("Id", true, true, false, typeof(long), 8, 19, 0, "bigint")
            });
            schema.Columns.Add(new ColumnInfo
            {
                Ordinal = 2,
                Collation = "BINARY_CI",
                Field = new DbField("Name", false, false, false, typeof(string), 128, 0, 0, "varchar")
            });
            schema.Columns.Add(new ColumnInfo
            {
                Ordinal = 3,
                DefaultExpression = "0",
                Field = new DbField("Age", false, false, true, typeof(int), 4, 10, 0, "int", true)
            });
            schema.PrimaryKey = new PrimaryKeyInfo("PK_Person") { Columns = { "Id" } };
            schema.UniqueConstraints.Add(new UniqueConstraintInfo("UQ_Person_Name") { Columns = { "Name" } });
            schema.CheckConstraints.Add(new CheckConstraintInfo("CK_Person_Age") { Expression = "(\"Age\" >= 0)" });
            schema.Indexes.Add(new IndexInfo("IX_Person_Name") { Columns = { "Name" }, IncludedColumns = { "Age" } });
            schema.ForeignKeys.Add(new ForeignKeyInfo("FK_Person_Country")
            {
                Columns = { "Age" },
                ReferencedTable = new TableInfo("Country", "dbo"),
                ReferencedColumns = { "Id" }
            });
            return schema;
        }

        #endregion

        #region ComposeCreateTable

        [TestMethod]
        public void TestVerticaSchemaComposerComposeCreateTable()
        {
            // Setup
            var composer = new VerticaSchemaComposer();
            var schema = GetPersonSchema();

            // Act
            var actual = composer.ComposeCreateTable(schema);
            var expected = string.Join(Environment.NewLine,
                "CREATE TABLE \"dbo\".\"Person\" (",
                "    \"Id\" IDENTITY(10, 5) NOT NULL,",
                "    \"Name\" VARCHAR(128) NOT NULL,",
                "    \"Age\" INT DEFAULT 0,",
                "    CONSTRAINT \"PK_Person\" PRIMARY KEY (\"Id\"),",
                "    CONSTRAINT \"UQ_Person_Name\" UNIQUE (\"Name\"),",
                "    CONSTRAINT \"CK_Person_Age\" CHECK ((\"Age\" >= 0))",
                ")");

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeCreateTableWithoutSchemaName()
        {
            // Setup
            var composer = new VerticaSchemaComposer();
            var schema = new TableSchema("Person", null);
            schema.Columns.Add(GetColumn("Name", "varchar", typeof(string), 1, false, 50));

            // Act
            var actual = composer.ComposeCreateTable(schema);
            var expected = string.Join(Environment.NewLine,
                "CREATE TABLE \"Person\" (",
                "    \"Name\" VARCHAR(50) NOT NULL",
                ")");

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeCreateTableOrdersTheColumnsByOrdinal()
        {
            // Setup
            var composer = new VerticaSchemaComposer();
            var schema = new TableSchema("Person", null);
            schema.Columns.Add(GetColumn("Second", "int", typeof(int), 2));
            schema.Columns.Add(GetColumn("First", "int", typeof(int), 1));

            // Act
            var actual = composer.ComposeCreateTable(schema);

            // Assert
            Assert.IsTrue(actual.IndexOf("\"First\"", StringComparison.Ordinal) < actual.IndexOf("\"Second\"", StringComparison.Ordinal));
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeCreateTableWithComputedColumn()
        {
            // Setup
            var composer = new VerticaSchemaComposer();
            var schema = new TableSchema("Person", null);
            schema.Columns.Add(new ColumnInfo
            {
                Ordinal = 1,
                ComputedExpression = "upper(\"Name\")",
                Field = new DbField("NameUpper", false, false, true, typeof(string), 128, 0, 0, "varchar")
            });

            // Act
            var actual = composer.ComposeCreateTable(schema);

            // Assert
            StringAssert.Contains(actual, "\"NameUpper\" VARCHAR(128) DEFAULT (upper(\"Name\"))", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeCreateTableIgnoresTheDefaultOfAnIdentityColumn()
        {
            // Setup
            var composer = new VerticaSchemaComposer();
            var schema = new TableSchema("Person", null);
            schema.Columns.Add(new ColumnInfo
            {
                Ordinal = 1,
                DefaultExpression = "0",
                Field = new DbField("Id", true, true, false, typeof(int), 4, 10, 0, "int")
            });

            // Act
            var actual = composer.ComposeCreateTable(schema);

            // Assert
            StringAssert.Contains(actual, "\"Id\" IDENTITY(1, 1) NOT NULL", StringComparison.Ordinal);
            Assert.IsFalse(actual.Contains("DEFAULT 0", StringComparison.Ordinal));
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeCreateTableIgnoresTheCollationOfTheColumn()
        {
            // Setup
            var composer = new VerticaSchemaComposer();
            var schema = new TableSchema("Person", null);
            schema.Columns.Add(new ColumnInfo
            {
                Ordinal = 1,
                Collation = "BINARY_CI",
                Field = new DbField("Age", false, false, true, typeof(int), 4, 10, 0, "int")
            });

            // Act
            var actual = composer.ComposeCreateTable(schema);

            // Assert
            Assert.IsFalse(actual.Contains("COLLATE", StringComparison.Ordinal));
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeCreateTableWithUnnamedPrimaryKey()
        {
            // Setup
            var composer = new VerticaSchemaComposer();
            var schema = new TableSchema("Person", null);
            schema.Columns.Add(GetColumn("Id", "int", typeof(int), 1, false));
            schema.PrimaryKey = new PrimaryKeyInfo(null) { Columns = { "Id" } };

            // Act
            var actual = composer.ComposeCreateTable(schema);

            // Assert
            StringAssert.Contains(actual, "    PRIMARY KEY (\"Id\")", StringComparison.Ordinal);
            Assert.IsFalse(actual.Contains("CONSTRAINT", StringComparison.Ordinal));
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeCreateTableWithCompositePrimaryKey()
        {
            // Setup
            var composer = new VerticaSchemaComposer();
            var schema = new TableSchema("OrderLine", null);
            schema.Columns.Add(GetColumn("OrderId", "int", typeof(int), 1, false));
            schema.Columns.Add(GetColumn("LineNumber", "int", typeof(int), 2, false));
            schema.PrimaryKey = new PrimaryKeyInfo("PK_OrderLine") { Columns = { "OrderId", "LineNumber" } };

            // Act
            var actual = composer.ComposeCreateTable(schema);

            // Assert
            StringAssert.Contains(actual, "PRIMARY KEY (\"OrderId\", \"LineNumber\")", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeCreateTableWithoutPrimaryKey()
        {
            // Setup
            var composer = new VerticaSchemaComposer();
            var schema = new TableSchema("NoKey", null);
            schema.Columns.Add(GetColumn("Value", "varchar", typeof(string), 1));

            // Act
            var actual = composer.ComposeCreateTable(schema);

            // Assert
            Assert.IsFalse(actual.Contains("PRIMARY KEY", StringComparison.Ordinal));
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeCreateTableQuotesTheIdentifiers()
        {
            // Setup
            var composer = new VerticaSchemaComposer();
            var schema = new TableSchema("Odd\"Table", null);
            schema.Columns.Add(GetColumn("Odd\"Column", "int", typeof(int), 1));

            // Act
            var actual = composer.ComposeCreateTable(schema);

            // Assert
            StringAssert.Contains(actual, "\"Odd\"\"Table\"", StringComparison.Ordinal);
            StringAssert.Contains(actual, "\"Odd\"\"Column\"", StringComparison.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaSchemaComposerComposeCreateTableIfTheSchemaIsNull()
        {
            // Setup
            var composer = new VerticaSchemaComposer();

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => composer.ComposeCreateTable(null));
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeCreateTableWithNamedPrimaryKey()
        {
            // Setup
            var composer = new VerticaSchemaComposer();
            var schema = new TableSchema("Product", null);
            schema.Columns.Add(GetColumn("Id", "int", typeof(int), 1, false));
            schema.PrimaryKey = new PrimaryKeyInfo("PK_Product") { IsClustered = false, Columns = { "Id" } };

            // Act
            var actual = composer.ComposeCreateTable(schema);

            // Assert
            StringAssert.Contains(actual, "CONSTRAINT \"PK_Product\" PRIMARY KEY (\"Id\")", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeCreateTableWithClusteredPrimaryKeyByDefault()
        {
            // Setup
            var composer = new VerticaSchemaComposer();
            var schema = new TableSchema("Product", null);
            schema.Columns.Add(GetColumn("Id", "int", typeof(int), 1, false));
            schema.PrimaryKey = new PrimaryKeyInfo("PK_Product") { Columns = { "Id" } };

            // Act
            var actual = composer.ComposeCreateTable(schema);

            // Assert
            StringAssert.Contains(actual, "CONSTRAINT \"PK_Product\" PRIMARY KEY (\"Id\")", StringComparison.Ordinal);
        }

        #endregion

        #region ComposeCreateIndex

        [TestMethod]
        public void ThrowExceptionOnVerticaSchemaComposerComposeCreateIndexIfTheIndexIsNull()
        {
            // Setup
            var composer = new VerticaSchemaComposer();

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => composer.ComposeCreateIndex("Person", null));
        }

        #endregion

        [TestMethod]
        public void ThrowExceptionOnVerticaSchemaComposerComposeCreateIndexAsVerticaHasNoIndex()
        {
            // Setup
            var composer = new VerticaSchemaComposer();
            var index = new IndexInfo("IX_Person_Name") { Columns = { "Name" } };

            // Act/Assert
            Assert.Throws<NotSupportedException>(() => composer.ComposeCreateIndex("dbo.Person", index));
        }

        #region ComposeAddForeignKey

        [TestMethod]
        public void TestVerticaSchemaComposerComposeAddForeignKey()
        {
            // Setup
            var composer = new VerticaSchemaComposer();
            var foreignKey = new ForeignKeyInfo("FK_Person_Country")
            {
                Columns = { "CountryId" },
                ReferencedTable = new TableInfo("Country", "dbo"),
                ReferencedColumns = { "Id" }
            };

            // Act
            var actual = composer.ComposeAddForeignKey("dbo.Person", foreignKey);
            var expected = "ALTER TABLE \"dbo\".\"Person\" ADD CONSTRAINT \"FK_Person_Country\" FOREIGN KEY (\"CountryId\") REFERENCES \"dbo\".\"Country\" (\"Id\")";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeAddForeignKeyIgnoresTheRules()
        {
            // Setup
            var composer = new VerticaSchemaComposer();
            var foreignKey = new ForeignKeyInfo("FK_Person_Country")
            {
                Columns = { "CountryId" },
                ReferencedTable = new TableInfo("Country", "dbo"),
                ReferencedColumns = { "Id" },
                DeleteRule = CopySchemaForeignKeyRule.SetNull,
                UpdateRule = CopySchemaForeignKeyRule.Cascade
            };

            // Act
            var actual = composer.ComposeAddForeignKey("dbo.Person", foreignKey);

            // Assert
            StringAssert.EndsWith(actual, "REFERENCES \"dbo\".\"Country\" (\"Id\")", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeAddForeignKeyIgnoresTheSetDefaultRule()
        {
            // Setup
            var composer = new VerticaSchemaComposer();
            var foreignKey = new ForeignKeyInfo("FK")
            {
                Columns = { "CountryId" },
                ReferencedTable = new TableInfo("Country", null),
                ReferencedColumns = { "Id" },
                DeleteRule = CopySchemaForeignKeyRule.SetDefault
            };

            // Act
            var actual = composer.ComposeAddForeignKey("Person", foreignKey);

            // Assert
            Assert.IsFalse(actual.Contains("ON DELETE", StringComparison.Ordinal));
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeAddForeignKeyIgnoresTheRestrictRule()
        {
            // Setup
            var composer = new VerticaSchemaComposer();
            var foreignKey = new ForeignKeyInfo("FK")
            {
                Columns = { "CountryId" },
                ReferencedTable = new TableInfo("Country", null),
                ReferencedColumns = { "Id" },
                DeleteRule = CopySchemaForeignKeyRule.Restrict,
                UpdateRule = CopySchemaForeignKeyRule.Restrict
            };

            // Act
            var actual = composer.ComposeAddForeignKey("Person", foreignKey);

            // Assert
            Assert.IsFalse(actual.Contains("ON DELETE", StringComparison.Ordinal));
            Assert.IsFalse(actual.Contains("ON UPDATE", StringComparison.Ordinal));
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeAddForeignKeyWithCompositeColumns()
        {
            // Setup
            var composer = new VerticaSchemaComposer();
            var foreignKey = new ForeignKeyInfo("FK_Line_Order")
            {
                Columns = { "OrderId", "LineNumber" },
                ReferencedTable = new TableInfo("OrderLine", "dbo"),
                ReferencedColumns = { "OrderId", "LineNumber" }
            };

            // Act
            var actual = composer.ComposeAddForeignKey("dbo.Shipment", foreignKey);

            // Assert
            StringAssert.Contains(actual, "FOREIGN KEY (\"OrderId\", \"LineNumber\") REFERENCES \"dbo\".\"OrderLine\" (\"OrderId\", \"LineNumber\")", StringComparison.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaSchemaComposerComposeAddForeignKeyIfTheForeignKeyIsNull()
        {
            // Setup
            var composer = new VerticaSchemaComposer();

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => composer.ComposeAddForeignKey("Person", null));
        }

        #endregion

        #region ComposeAddColumn

        [TestMethod]
        public void TestVerticaSchemaComposerComposeAddColumn()
        {
            // Setup
            var composer = new VerticaSchemaComposer();
            var column = GetColumn("Nickname", "varchar", typeof(string), 1, true, 50);

            // Act
            var actual = composer.ComposeAddColumn("dbo.Person", column);

            // Assert
            Assert.AreEqual("ALTER TABLE \"dbo\".\"Person\" ADD COLUMN \"Nickname\" VARCHAR(50)", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeAddColumnWithDefault()
        {
            // Setup
            var composer = new VerticaSchemaComposer();
            var column = GetColumn("Age", "int", typeof(int), 1, false);
            column.DefaultExpression = "0";

            // Act
            var actual = composer.ComposeAddColumn("Person", column);

            // Assert
            Assert.AreEqual("ALTER TABLE \"Person\" ADD COLUMN \"Age\" INT DEFAULT 0 NOT NULL", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeAddColumnWithComputedColumn()
        {
            // Setup
            var composer = new VerticaSchemaComposer();
            var column = GetColumn("NameUpper", "varchar", typeof(string), 1, true, 128);
            column.ComputedExpression = "upper(\"Name\")";

            // Act
            var actual = composer.ComposeAddColumn("dbo.Person", column);
            var expected = "ALTER TABLE \"dbo\".\"Person\" ADD COLUMN \"NameUpper\" VARCHAR(128) DEFAULT (upper(\"Name\"))";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaSchemaComposerComposeAddColumnIfTheColumnIsNull()
        {
            // Setup
            var composer = new VerticaSchemaComposer();

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => composer.ComposeAddColumn("Person", null));
        }

        #endregion

        #region ComposeDropTable

        [TestMethod]
        public void TestVerticaSchemaComposerComposeDropTable()
        {
            // Setup
            var composer = new VerticaSchemaComposer();

            // Act
            var actual = composer.ComposeDropTable("dbo.Person");

            // Assert
            Assert.AreEqual("DROP TABLE IF EXISTS \"dbo\".\"Person\" CASCADE", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeDropTableWithQuotedName()
        {
            // Setup
            var composer = new VerticaSchemaComposer();

            // Act
            var actual = composer.ComposeDropTable("\"Sales\".\"Invoice\"");

            // Assert
            Assert.AreEqual("DROP TABLE IF EXISTS \"Sales\".\"Invoice\" CASCADE", actual, StringComparer.Ordinal);
        }

        #endregion

        #region ComposeSchema

        [TestMethod]
        public void TestVerticaSchemaComposerComposeSchema()
        {
            // Setup
            var composer = new VerticaSchemaComposer();
            var schema = GetPersonSchema();

            // Act
            var actual = composer.ComposeSchema(schema).ToList();

            // Assert
            Assert.AreEqual(2, actual.Count);
            StringAssert.StartsWith(actual[0], "CREATE TABLE \"dbo\".\"Person\"", StringComparison.Ordinal);
            StringAssert.StartsWith(actual[1], "ALTER TABLE \"dbo\".\"Person\" ADD CONSTRAINT \"FK_Person_Country\"", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeSchemaWithOnlyColumns()
        {
            // Setup
            var composer = new VerticaSchemaComposer();
            var schema = new TableSchema("NoKey", null);
            schema.Columns.Add(GetColumn("Value", "varchar", typeof(string), 1));

            // Act
            var actual = composer.ComposeSchema(schema).ToList();

            // Assert
            Assert.AreEqual(1, actual.Count);
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaSchemaComposerComposeSchemaIfTheSchemaIsNull()
        {
            // Setup
            var composer = new VerticaSchemaComposer();

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => composer.ComposeSchema(null));
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeSchemas()
        {
            // Setup
            var composer = new VerticaSchemaComposer();
            var first = GetPersonSchema();
            var second = new TableSchema("Country", "dbo");
            second.Columns.Add(GetColumn("Id", "int", typeof(int), 1, false));
            second.Indexes.Add(new IndexInfo("IX_Country_Id") { Columns = { "Id" } });

            // Act
            var actual = composer.ComposeSchemas(new[] { first, second }).ToList();

            // Assert
            Assert.AreEqual(3, actual.Count);
            StringAssert.StartsWith(actual[0], "CREATE TABLE \"dbo\".\"Person\"", StringComparison.Ordinal);
            StringAssert.StartsWith(actual[1], "CREATE TABLE \"dbo\".\"Country\"", StringComparison.Ordinal);
            StringAssert.StartsWith(actual[2], "ALTER TABLE \"dbo\".\"Person\" ADD CONSTRAINT \"FK_Person_Country\"", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeSchemasCreatesTheForeignKeysAfterAllTheTables()
        {
            // Setup
            var composer = new VerticaSchemaComposer();
            var a = new TableSchema("CycleA", "dbo");
            a.Columns.Add(GetColumn("Id", "int", typeof(int), 1, false));
            a.Columns.Add(GetColumn("BId", "int", typeof(int), 2));
            a.ForeignKeys.Add(new ForeignKeyInfo("FK_CycleA_CycleB") { Columns = { "BId" }, ReferencedTable = new TableInfo("CycleB", "dbo"), ReferencedColumns = { "Id" } });
            var b = new TableSchema("CycleB", "dbo");
            b.Columns.Add(GetColumn("Id", "int", typeof(int), 1, false));
            b.Columns.Add(GetColumn("AId", "int", typeof(int), 2));
            b.ForeignKeys.Add(new ForeignKeyInfo("FK_CycleB_CycleA") { Columns = { "AId" }, ReferencedTable = new TableInfo("CycleA", "dbo"), ReferencedColumns = { "Id" } });

            // Act
            var actual = composer.ComposeSchemas(new[] { a, b }).ToList();

            // Assert
            Assert.AreEqual(4, actual.Count);
            StringAssert.StartsWith(actual[0], "CREATE TABLE \"dbo\".\"CycleA\"", StringComparison.Ordinal);
            StringAssert.StartsWith(actual[1], "CREATE TABLE \"dbo\".\"CycleB\"", StringComparison.Ordinal);
            StringAssert.StartsWith(actual[2], "ALTER TABLE \"dbo\".\"CycleA\" ADD CONSTRAINT \"FK_CycleA_CycleB\"", StringComparison.Ordinal);
            StringAssert.StartsWith(actual[3], "ALTER TABLE \"dbo\".\"CycleB\" ADD CONSTRAINT \"FK_CycleB_CycleA\"", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeSchemasWithoutTables()
        {
            // Setup
            var composer = new VerticaSchemaComposer();

            // Act
            var actual = composer.ComposeSchemas(Enumerable.Empty<TableSchema>()).ToList();

            // Assert
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeSchemasOfOneTableIsTheSameAsComposeSchema()
        {
            // Setup
            var composer = new VerticaSchemaComposer();
            var schema = GetPersonSchema();

            // Act
            var actual = composer.ComposeSchemas(new[] { schema }).ToList();
            var expected = composer.ComposeSchema(schema).ToList();

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaSchemaComposerComposeSchemasIfTheSchemasAreNull()
        {
            // Setup
            var composer = new VerticaSchemaComposer();

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => composer.ComposeSchemas(null));
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeCreateTableWithNameThatNeedsQuoting()
        {
            // Setup
            var composer = new VerticaSchemaComposer();
            var schema = new TableSchema("Odd.Name", "dbo");
            schema.Columns.Add(GetColumn("Unit Price", "decimal", typeof(decimal), 1, false, null, 10, 2));

            // Act
            var actual = composer.ComposeCreateTable(schema);

            // Assert
            StringAssert.StartsWith(actual, "CREATE TABLE \"dbo\".\"Odd.Name\" (", StringComparison.Ordinal);
            StringAssert.Contains(actual, "\"Unit Price\" NUMERIC(10,2) NOT NULL", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeCreateTableWithDoubleQuoteInTheNames()
        {
            // Setup
            var composer = new VerticaSchemaComposer();
            var schema = new TableSchema("Weird\"Name", "dbo");
            schema.Columns.Add(GetColumn("Id", "int", typeof(int), 1, false));

            // Act
            var actual = composer.ComposeCreateTable(schema);

            // Assert
            StringAssert.StartsWith(actual, "CREATE TABLE \"dbo\".\"Weird\"\"Name\" (", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeAddForeignKeyWithNamesThatNeedQuoting()
        {
            // Setup
            var composer = new VerticaSchemaComposer();
            var foreignKey = new ForeignKeyInfo("FK_OddChild_OddName")
            {
                Columns = { "ParentId" },
                ReferencedTable = new TableInfo("Odd.Name", "dbo"),
                ReferencedColumns = { "Id" }
            };

            // Act
            var actual = composer.ComposeAddForeignKey("\"dbo\".\"Odd.Child\"", foreignKey);
            var expected = "ALTER TABLE \"dbo\".\"Odd.Child\" ADD CONSTRAINT \"FK_OddChild_OddName\" FOREIGN KEY (\"ParentId\") REFERENCES \"dbo\".\"Odd.Name\" (\"Id\")";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeSchemasWithNamesThatNeedQuoting()
        {
            // Setup
            var composer = new VerticaSchemaComposer();
            var parent = new TableSchema("Odd.Name", "dbo");
            parent.Columns.Add(GetColumn("Id", "int", typeof(int), 1, false));
            parent.Indexes.Add(new IndexInfo("IX_OddName_Id") { Columns = { "Id" } });
            var child = new TableSchema("Odd.Child", "dbo");
            child.Columns.Add(GetColumn("ParentId", "int", typeof(int), 1, false));
            child.ForeignKeys.Add(new ForeignKeyInfo("FK_OddChild_OddName")
            {
                Columns = { "ParentId" },
                ReferencedTable = new TableInfo("Odd.Name", "dbo"),
                ReferencedColumns = { "Id" }
            });

            // Act
            var actual = composer.ComposeSchemas(new[] { child, parent }).ToList();

            // Assert
            Assert.AreEqual(3, actual.Count);
            StringAssert.StartsWith(actual[0], "CREATE TABLE \"dbo\".\"Odd.Child\"", StringComparison.Ordinal);
            StringAssert.StartsWith(actual[1], "CREATE TABLE \"dbo\".\"Odd.Name\"", StringComparison.Ordinal);
            StringAssert.Contains(actual[2], "ALTER TABLE \"dbo\".\"Odd.Child\" ADD CONSTRAINT \"FK_OddChild_OddName\"", StringComparison.Ordinal);
            StringAssert.Contains(actual[2], "REFERENCES \"dbo\".\"Odd.Name\" (\"Id\")", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeDropTableWithDoubleQuoteInTheName()
        {
            // Setup
            var composer = new VerticaSchemaComposer();

            // Act
            var actual = composer.ComposeDropTable("\"dbo\".\"Weird\"\"Name\"");

            // Assert
            Assert.AreEqual("DROP TABLE IF EXISTS \"dbo\".\"Weird\"\"Name\" CASCADE", actual, StringComparer.Ordinal);
        }

        #endregion

        #region ComposeTypeName

        [TestMethod]
        public void TestVerticaSchemaComposerComposeTypeNameForSizedUnicodeString()
        {
            Assert.AreEqual("VARCHAR(128)", ComposeTypeName("varchar", typeof(string), 128), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeTypeNameForUnicodeStringWithMaxSize()
        {
            Assert.AreEqual("LONG VARCHAR", ComposeTypeName("varchar", typeof(string), -1), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeTypeNameForUnicodeStringWithoutSize()
        {
            Assert.AreEqual("LONG VARCHAR", ComposeTypeName("varchar", typeof(string)), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeTypeNameForVarChar()
        {
            Assert.AreEqual("VARCHAR(50)", ComposeTypeName("varchar", typeof(string), 50), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeTypeNameForFixedLengthCharacters()
        {
            Assert.AreEqual("CHAR(10)", ComposeTypeName("char", typeof(string), 10), StringComparer.Ordinal);
            Assert.AreEqual("CHAR(4)", ComposeTypeName("char", typeof(string), 4), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeTypeNameForFixedLengthCharactersWithoutSize()
        {
            Assert.AreEqual("CHAR(1)", ComposeTypeName("char", typeof(string)), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeTypeNameForBinary()
        {
            Assert.AreEqual("BINARY(16)", ComposeTypeName("binary", typeof(byte[]), 16), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeTypeNameForVarBinary()
        {
            Assert.AreEqual("VARBINARY(256)", ComposeTypeName("varbinary", typeof(byte[]), 256), StringComparer.Ordinal);
            Assert.AreEqual("LONG VARBINARY", ComposeTypeName("varbinary", typeof(byte[]), -1), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeTypeNameForDecimal()
        {
            Assert.AreEqual("NUMERIC(18,2)", ComposeTypeName("decimal", typeof(decimal), 9, 18, 2), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeTypeNameForNumericWithoutPrecision()
        {
            Assert.AreEqual("NUMERIC", ComposeTypeName("decimal", typeof(decimal)), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeTypeNameForTemporalTypesWithScale()
        {
            Assert.AreEqual("TIMESTAMP(3)", ComposeTypeName("datetime", typeof(DateTime), scale: 3), StringComparer.Ordinal);
            Assert.AreEqual("TIMESTAMP(6)", ComposeTypeName("timestamp", typeof(DateTime), scale: 9), StringComparer.Ordinal);
            Assert.AreEqual("TIME(5)", ComposeTypeName("time", typeof(TimeSpan), scale: 5), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeTypeNameForTemporalTypesWithoutScale()
        {
            Assert.AreEqual("TIMESTAMP", ComposeTypeName("datetime", typeof(DateTime)), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeTypeNameForSimpleTypes()
        {
            Assert.AreEqual("INT", ComposeTypeName("int", typeof(int)), StringComparer.Ordinal);
            Assert.AreEqual("INT", ComposeTypeName("bigint", typeof(long)), StringComparer.Ordinal);
            Assert.AreEqual("BOOLEAN", ComposeTypeName("bit", typeof(bool)), StringComparer.Ordinal);
            Assert.AreEqual("UUID", ComposeTypeName("uuid", typeof(Guid)), StringComparer.Ordinal);
            Assert.AreEqual("FLOAT", ComposeTypeName("double", typeof(double)), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeTypeNameIsCaseInsensitive()
        {
            Assert.AreEqual("INT", ComposeTypeName("INT", typeof(int)), StringComparer.Ordinal);
            Assert.AreEqual("VARCHAR(20)", ComposeTypeName("VarChar", typeof(string), 20), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeTypeNameForTheTypesOfOtherDatabaseEngines()
        {
            Assert.AreEqual("BOOLEAN", ComposeTypeName("boolean", typeof(bool)), StringComparer.Ordinal);
            Assert.AreEqual("UUID", ComposeTypeName("uuid", typeof(Guid)), StringComparer.Ordinal);
            Assert.AreEqual("LONG VARCHAR", ComposeTypeName("varchar", typeof(string)), StringComparer.Ordinal);
            Assert.AreEqual("LONG VARBINARY", ComposeTypeName("varbinary", typeof(byte[])), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeTypeNameFallsBackToTheClientTypeIfThereIsNoDatabaseType()
        {
            Assert.AreEqual("INT", ComposeTypeName(null, typeof(long)), StringComparer.Ordinal);
            Assert.AreEqual("TIMESTAMP", ComposeTypeName(null, typeof(DateTime)), StringComparer.Ordinal);
            Assert.AreEqual("VARCHAR(20)", ComposeTypeName(null, typeof(string), 20), StringComparer.Ordinal);
            Assert.AreEqual("LONG VARCHAR", ComposeTypeName(null, typeof(object)), StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaSchemaComposerComposeTypeNameIfTheColumnIsNull()
        {
            // Setup
            var composer = new VerticaSchemaComposer();

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => composer.ComposeTypeName(null));
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaSchemaComposerComposeTypeNameIfTheColumnHasNoField()
        {
            // Setup
            var composer = new VerticaSchemaComposer();

            // Act/Assert
            Assert.Throws<ArgumentException>(() => composer.ComposeTypeName(new ColumnInfo()));
        }

        #endregion

        #region Interface

        [TestMethod]
        public void TestVerticaSchemaComposerIsASchemaComposer()
        {
            // Act
            var composer = new VerticaSchemaComposer();

            // Assert
            Assert.IsInstanceOfType<ISchemaComposer>(composer);
        }

        #endregion

        #region ComposeName / Exists

        [TestMethod]
        public void TestVerticaSchemaComposerComposeName()
        {
            // Act/Assert
            var composer = new VerticaSchemaComposer();
            Assert.AreEqual("dbo.Person", composer.ComposeName(new TableInfo("Person", "dbo")));
            Assert.AreEqual("dbo.\"Odd.Name\"", composer.ComposeName(new TableInfo("Odd.Name", "dbo")));
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeNameWithoutSchema()
        {
            // Act/Assert
            Assert.AreEqual("Person", new VerticaSchemaComposer().ComposeName(new TableInfo("Person", null)));
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaSchemaComposerComposeNameIfTheTableIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new VerticaSchemaComposer().ComposeName(null));
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeTableExists()
        {
            // Act/Assert
            Assert.AreEqual("SELECT CASE WHEN EXISTS (SELECT 1 FROM v_catalog.tables WHERE table_schema = 'dbo' AND table_name = 'Person') THEN 1 ELSE 0 END", new VerticaSchemaComposer().ComposeTableExists("dbo.Person"));
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeColumnExists()
        {
            // Act/Assert
            Assert.AreEqual("SELECT CASE WHEN EXISTS (SELECT 1 FROM v_catalog.columns WHERE table_schema = 'dbo' AND table_name = 'Person' AND column_name = 'Name') THEN 1 ELSE 0 END", new VerticaSchemaComposer().ComposeColumnExists("dbo.Person", "Name"));
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeIndexExists()
        {
            // Act/Assert
            Assert.AreEqual("SELECT 0", new VerticaSchemaComposer().ComposeIndexExists("dbo.Person", "IX_Person_Name"));
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposeExistsEscapesTheSingleQuotes()
        {
            // Act
            var composer = new VerticaSchemaComposer();

            // Assert
            StringAssert.Contains(composer.ComposeColumnExists("dbo.Person", "O'Brien"), "O''Brien", StringComparison.Ordinal);
            StringAssert.Contains(composer.ComposeTableExists("dbo.\"O'Brien\""), "O''Brien", StringComparison.Ordinal);
        }

        #endregion
    }
}
