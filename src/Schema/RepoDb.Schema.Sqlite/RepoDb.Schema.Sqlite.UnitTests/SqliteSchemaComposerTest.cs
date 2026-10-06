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

namespace RepoDb.Schema.Sqlite.UnitTests
{
    [TestClass]
    public class SqliteSchemaComposerTest
    {
        #region Helpers

        private static ColumnInfo GetColumn(string name,
            string databaseType,
            Type type,
            int ordinal,
            bool isNullable = true,
            int? size = null,
            byte? precision = null,
            byte? scale = null,
            bool isIdentity = false) =>
            new ColumnInfo
            {
                Ordinal = ordinal,
                Field = new DbField(name, false, isIdentity, isNullable, type, size, precision, scale, databaseType)
            };

        private static string ComposeTypeName(string databaseType,
            Type type,
            int? size = null,
            byte? precision = null,
            byte? scale = null) =>
            new SqliteSchemaComposer().ComposeTypeName(GetColumn("Column", databaseType, type, 1, true, size, precision, scale));

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
                Collation = "NOCASE",
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
        public void TestSqliteSchemaComposerComposeCreateTable()
        {
            // Setup
            var composer = new SqliteSchemaComposer();
            var schema = GetPersonSchema();

            // Act
            var actual = composer.ComposeCreateTable(schema);
            var expected = string.Join(Environment.NewLine,
                "CREATE TABLE \"dbo\".\"Person\" (",
                "    \"Id\" integer NOT NULL CONSTRAINT \"PK_Person\" PRIMARY KEY AUTOINCREMENT,",
                "    \"Name\" varchar(128) NOT NULL COLLATE NOCASE,",
                "    \"Age\" integer DEFAULT 0,",
                "    CONSTRAINT \"UQ_Person_Name\" UNIQUE (\"Name\"),",
                "    CONSTRAINT \"CK_Person_Age\" CHECK ((\"Age\" >= 0)),",
                "    CONSTRAINT \"FK_Person_Country\" FOREIGN KEY (\"Age\") REFERENCES \"Country\" (\"Id\")",
                ");");

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeCreateTableWithoutSchemaName()
        {
            // Setup
            var composer = new SqliteSchemaComposer();
            var schema = new TableSchema("Person", null);
            schema.Columns.Add(GetColumn("Name", "varchar", typeof(string), 1, false, 50));

            // Act
            var actual = composer.ComposeCreateTable(schema);
            var expected = string.Join(Environment.NewLine,
                "CREATE TABLE \"Person\" (",
                "    \"Name\" varchar(50) NOT NULL",
                ");");

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeCreateTableOrdersTheColumnsByOrdinal()
        {
            // Setup
            var composer = new SqliteSchemaComposer();
            var schema = new TableSchema("Person", null);
            schema.Columns.Add(GetColumn("Second", "int", typeof(int), 2));
            schema.Columns.Add(GetColumn("First", "int", typeof(int), 1));

            // Act
            var actual = composer.ComposeCreateTable(schema);

            // Assert
            Assert.IsTrue(actual.IndexOf("\"First\"", StringComparison.Ordinal) < actual.IndexOf("\"Second\"", StringComparison.Ordinal));
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeCreateTableWithComputedColumn()
        {
            // Setup
            var composer = new SqliteSchemaComposer();
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
            StringAssert.Contains(actual, "\"NameUpper\" varchar(128) GENERATED ALWAYS AS (upper(\"Name\"))", StringComparison.Ordinal);
            Assert.IsFalse(actual.Contains("DEFAULT", StringComparison.Ordinal));
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeCreateTableIgnoresTheDefaultOfAnIdentityColumn()
        {
            // Setup
            var composer = new SqliteSchemaComposer();
            var schema = new TableSchema("Person", null);
            schema.Columns.Add(new ColumnInfo
            {
                Ordinal = 1,
                DefaultExpression = "0",
                Field = new DbField("Id", true, true, false, typeof(int), 4, 10, 0, "int")
            });
            schema.PrimaryKey = new PrimaryKeyInfo(null) { Columns = { "Id" } };

            // Act
            var actual = composer.ComposeCreateTable(schema);

            // Assert
            StringAssert.Contains(actual, "\"Id\" integer NOT NULL PRIMARY KEY AUTOINCREMENT", StringComparison.Ordinal);
            Assert.IsFalse(actual.Contains("DEFAULT", StringComparison.Ordinal));
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeCreateTableCollatesTheColumn()
        {
            // Setup
            var composer = new SqliteSchemaComposer();
            var schema = new TableSchema("Person", null);
            schema.Columns.Add(new ColumnInfo
            {
                Ordinal = 1,
                Collation = "NOCASE",
                Field = new DbField("Age", false, false, true, typeof(int), 4, 10, 0, "int")
            });

            // Act
            var actual = composer.ComposeCreateTable(schema);

            // Assert
            StringAssert.Contains(actual, "\"Age\" integer COLLATE NOCASE", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeCreateTableWithUnnamedPrimaryKey()
        {
            // Setup
            var composer = new SqliteSchemaComposer();
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
        public void TestSqliteSchemaComposerComposeCreateTableWithCompositePrimaryKey()
        {
            // Setup
            var composer = new SqliteSchemaComposer();
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
        public void TestSqliteSchemaComposerComposeCreateTableWithoutPrimaryKey()
        {
            // Setup
            var composer = new SqliteSchemaComposer();
            var schema = new TableSchema("NoKey", null);
            schema.Columns.Add(GetColumn("Value", "varchar", typeof(string), 1));

            // Act
            var actual = composer.ComposeCreateTable(schema);

            // Assert
            Assert.IsFalse(actual.Contains("PRIMARY KEY", StringComparison.Ordinal));
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeCreateTableQuotesTheIdentifiers()
        {
            // Setup
            var composer = new SqliteSchemaComposer();
            var schema = new TableSchema("Odd\"Table", null);
            schema.Columns.Add(GetColumn("Odd\"Column", "int", typeof(int), 1));

            // Act
            var actual = composer.ComposeCreateTable(schema);

            // Assert
            StringAssert.Contains(actual, "\"Odd\"\"Table\"", StringComparison.Ordinal);
            StringAssert.Contains(actual, "\"Odd\"\"Column\"", StringComparison.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnSqliteSchemaComposerComposeCreateTableIfTheSchemaIsNull()
        {
            // Setup
            var composer = new SqliteSchemaComposer();

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => composer.ComposeCreateTable(null));
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeCreateTableWithNamedPrimaryKey()
        {
            // Setup
            var composer = new SqliteSchemaComposer();
            var schema = new TableSchema("Product", null);
            schema.Columns.Add(GetColumn("Id", "int", typeof(int), 1, false));
            schema.PrimaryKey = new PrimaryKeyInfo("PK_Product") { IsClustered = false, Columns = { "Id" } };

            // Act
            var actual = composer.ComposeCreateTable(schema);

            // Assert
            StringAssert.Contains(actual, "CONSTRAINT \"PK_Product\" PRIMARY KEY (\"Id\")", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeCreateTableWithIdentityPrimaryKeyDefinedWithTheColumn()
        {
            // Setup
            var composer = new SqliteSchemaComposer();
            var schema = new TableSchema("Product", null);
            schema.Columns.Add(GetColumn("Id", "int", typeof(int), 1, false, isIdentity: true));
            schema.PrimaryKey = new PrimaryKeyInfo("PK_Product") { Columns = { "Id" } };

            // Act
            var actual = composer.ComposeCreateTable(schema);

            // Assert
            StringAssert.Contains(actual, "\"Id\" integer NOT NULL CONSTRAINT \"PK_Product\" PRIMARY KEY AUTOINCREMENT", StringComparison.Ordinal);
            Assert.IsFalse(actual.Contains("PRIMARY KEY (", StringComparison.Ordinal));
        }

        #endregion

        #region ComposeCreateIndex

        [TestMethod]
        public void TestSqliteSchemaComposerComposeCreateIndex()
        {
            // Setup
            var composer = new SqliteSchemaComposer();
            var index = new IndexInfo("IX_Person_Name") { Columns = { "Name" } };

            // Act
            var actual = composer.ComposeCreateIndex("dbo.Person", index);

            // Assert
            Assert.AreEqual("CREATE INDEX \"dbo\".\"IX_Person_Name\" ON \"Person\" (\"Name\");", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeCreateIndexWithUniqueAndIncludedColumns()
        {
            // Setup
            var composer = new SqliteSchemaComposer();
            var index = new IndexInfo("IX_Person_Name")
            {
                IsUnique = true,
                Columns = { "Name", "Age" },
                IncludedColumns = { "Salary", "CountryId" }
            };

            // Act
            var actual = composer.ComposeCreateIndex("\"dbo\".\"Person\"", index);
            var expected = "CREATE UNIQUE INDEX \"dbo\".\"IX_Person_Name\" ON \"Person\" (\"Name\", \"Age\");";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnSqliteSchemaComposerComposeCreateIndexIfTheIndexIsNull()
        {
            // Setup
            var composer = new SqliteSchemaComposer();

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => composer.ComposeCreateIndex("Person", null));
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeCreateIndexWithClusteredIndex()
        {
            // Setup
            var composer = new SqliteSchemaComposer();
            var index = new IndexInfo("CIX_Product_Code") { IsUnique = true, IsClustered = true, Columns = { "Code" } };

            // Act
            var actual = composer.ComposeCreateIndex("dbo.Product", index);

            // Assert
            Assert.AreEqual("CREATE UNIQUE INDEX \"dbo\".\"CIX_Product_Code\" ON \"Product\" (\"Code\");", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeCreateIndexWithNonUniqueClusteredIndex()
        {
            // Setup
            var composer = new SqliteSchemaComposer();
            var index = new IndexInfo("CIX") { IsClustered = true, Columns = { "Code" } };

            // Act
            var actual = composer.ComposeCreateIndex("Product", index);

            // Assert
            Assert.AreEqual("CREATE INDEX \"CIX\" ON \"Product\" (\"Code\");", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeCreateIndexWithDescendingKey()
        {
            // Setup
            var composer = new SqliteSchemaComposer();
            var index = new IndexInfo("IX_Product_Category_Price")
            {
                Columns = { "Category", "Price" },
                DescendingColumns = { "Price" },
                IncludedColumns = { "Name" }
            };

            // Act
            var actual = composer.ComposeCreateIndex("dbo.Product", index);
            var expected = "CREATE INDEX \"dbo\".\"IX_Product_Category_Price\" ON \"Product\" (\"Category\", \"Price\" DESC);";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeCreateIndexWithAllDescendingKeys()
        {
            // Setup
            var composer = new SqliteSchemaComposer();
            var index = new IndexInfo("IX") { Columns = { "A", "B" }, DescendingColumns = { "A", "B" } };

            // Act
            var actual = composer.ComposeCreateIndex("T", index);

            // Assert
            Assert.AreEqual("CREATE INDEX \"IX\" ON \"T\" (\"A\" DESC, \"B\" DESC);", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeCreateIndexWithFilter()
        {
            // Setup
            var composer = new SqliteSchemaComposer();
            var index = new IndexInfo("IX_Product_Active_Name")
            {
                Columns = { "Name" },
                Filter = "(\"IsActive\"=(1))"
            };

            // Act
            var actual = composer.ComposeCreateIndex("dbo.Product", index);

            // Assert
            Assert.AreEqual("CREATE INDEX \"dbo\".\"IX_Product_Active_Name\" ON \"Product\" (\"Name\") WHERE (\"IsActive\"=(1));", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeCreateIndexWithAllTheOptions()
        {
            // Setup
            var composer = new SqliteSchemaComposer();
            var index = new IndexInfo("IX")
            {
                IsUnique = true,
                IsClustered = true,
                Columns = { "A", "B" },
                DescendingColumns = { "B" },
                IncludedColumns = { "C" },
                Filter = "(\"A\">(0))"
            };

            // Act
            var actual = composer.ComposeCreateIndex("T", index);

            // Assert
            Assert.AreEqual("CREATE UNIQUE INDEX \"IX\" ON \"T\" (\"A\", \"B\" DESC) WHERE (\"A\">(0));", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeCreateIndexIgnoresTheBlankFilter()
        {
            // Setup
            var composer = new SqliteSchemaComposer();
            var index = new IndexInfo("IX") { Columns = { "A" }, Filter = "  " };

            // Act
            var actual = composer.ComposeCreateIndex("T", index);

            // Assert
            Assert.AreEqual("CREATE INDEX \"IX\" ON \"T\" (\"A\");", actual, StringComparer.Ordinal);
        }

        #endregion

        #region ComposeAddForeignKey

        [TestMethod]
        public void TestSqliteSchemaComposerComposeAddForeignKey()
        {
            // Setup
            var composer = new SqliteSchemaComposer();
            var foreignKey = new ForeignKeyInfo("FK_Person_Country")
            {
                Columns = { "CountryId" },
                ReferencedTable = new TableInfo("Country", "dbo"),
                ReferencedColumns = { "Id" }
            };

            // Act
            var actual = composer.ComposeAddForeignKey("dbo.Person", foreignKey);

            // Assert
            Assert.AreEqual(string.Empty, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeCreateTableWithForeignKeyRules()
        {
            // Setup
            var composer = new SqliteSchemaComposer();
            var schema = new TableSchema("Person", "dbo");
            schema.Columns.Add(GetColumn("CountryId", "int", typeof(int), 1));
            schema.ForeignKeys.Add(new ForeignKeyInfo("FK_Person_Country")
            {
                Columns = { "CountryId" },
                ReferencedTable = new TableInfo("Country", "dbo"),
                ReferencedColumns = { "Id" },
                DeleteRule = CopySchemaForeignKeyRule.SetNull,
                UpdateRule = CopySchemaForeignKeyRule.Cascade
            });

            // Act
            var actual = composer.ComposeCreateTable(schema);

            // Assert
            StringAssert.Contains(actual, "CONSTRAINT \"FK_Person_Country\" FOREIGN KEY (\"CountryId\") REFERENCES \"Country\" (\"Id\") ON DELETE SET NULL ON UPDATE CASCADE", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeCreateTableWithSetDefaultRule()
        {
            // Setup
            var composer = new SqliteSchemaComposer();
            var schema = new TableSchema("Person", null);
            schema.Columns.Add(GetColumn("CountryId", "int", typeof(int), 1));
            schema.ForeignKeys.Add(new ForeignKeyInfo("FK")
            {
                Columns = { "CountryId" },
                ReferencedTable = new TableInfo("Country", null),
                ReferencedColumns = { "Id" },
                DeleteRule = CopySchemaForeignKeyRule.SetDefault
            });

            // Act
            var actual = composer.ComposeCreateTable(schema);

            // Assert
            StringAssert.Contains(actual, "ON DELETE SET DEFAULT", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeCreateTableWithRestrictRule()
        {
            // Setup
            var composer = new SqliteSchemaComposer();
            var schema = new TableSchema("Person", null);
            schema.Columns.Add(GetColumn("CountryId", "int", typeof(int), 1));
            schema.ForeignKeys.Add(new ForeignKeyInfo("FK")
            {
                Columns = { "CountryId" },
                ReferencedTable = new TableInfo("Country", null),
                ReferencedColumns = { "Id" },
                DeleteRule = CopySchemaForeignKeyRule.Restrict,
                UpdateRule = CopySchemaForeignKeyRule.Restrict
            });

            // Act
            var actual = composer.ComposeCreateTable(schema);

            // Assert
            StringAssert.Contains(actual, "ON DELETE RESTRICT", StringComparison.Ordinal);
            StringAssert.Contains(actual, "ON UPDATE RESTRICT", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeCreateTableWithCompositeForeignKey()
        {
            // Setup
            var composer = new SqliteSchemaComposer();
            var schema = new TableSchema("Shipment", "dbo");
            schema.Columns.Add(GetColumn("OrderId", "int", typeof(int), 1));
            schema.Columns.Add(GetColumn("LineNumber", "int", typeof(int), 2));
            schema.ForeignKeys.Add(new ForeignKeyInfo("FK_Line_Order")
            {
                Columns = { "OrderId", "LineNumber" },
                ReferencedTable = new TableInfo("OrderLine", "dbo"),
                ReferencedColumns = { "OrderId", "LineNumber" }
            });

            // Act
            var actual = composer.ComposeCreateTable(schema);

            // Assert
            StringAssert.Contains(actual, "FOREIGN KEY (\"OrderId\", \"LineNumber\") REFERENCES \"OrderLine\" (\"OrderId\", \"LineNumber\")", StringComparison.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnSqliteSchemaComposerComposeAddForeignKeyIfTheForeignKeyIsNull()
        {
            // Setup
            var composer = new SqliteSchemaComposer();

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => composer.ComposeAddForeignKey("Person", null));
        }

        #endregion

        #region ComposeAddColumn

        [TestMethod]
        public void TestSqliteSchemaComposerComposeAddColumn()
        {
            // Setup
            var composer = new SqliteSchemaComposer();
            var column = GetColumn("Nickname", "varchar", typeof(string), 1, true, 50);

            // Act
            var actual = composer.ComposeAddColumn("dbo.Person", column);

            // Assert
            Assert.AreEqual("ALTER TABLE \"dbo\".\"Person\" ADD COLUMN \"Nickname\" varchar(50);", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeAddColumnWithDefault()
        {
            // Setup
            var composer = new SqliteSchemaComposer();
            var column = GetColumn("Age", "int", typeof(int), 1, false);
            column.DefaultExpression = "0";

            // Act
            var actual = composer.ComposeAddColumn("Person", column);

            // Assert
            Assert.AreEqual("ALTER TABLE \"Person\" ADD COLUMN \"Age\" integer NOT NULL DEFAULT 0;", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnSqliteSchemaComposerComposeAddColumnIfTheColumnIsNull()
        {
            // Setup
            var composer = new SqliteSchemaComposer();

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => composer.ComposeAddColumn("Person", null));
        }

        #endregion

        #region ComposeDropTable

        [TestMethod]
        public void TestSqliteSchemaComposerComposeDropTable()
        {
            // Setup
            var composer = new SqliteSchemaComposer();

            // Act
            var actual = composer.ComposeDropTable("dbo.Person");

            // Assert
            Assert.AreEqual("DROP TABLE IF EXISTS \"dbo\".\"Person\";", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeDropTableWithQuotedName()
        {
            // Setup
            var composer = new SqliteSchemaComposer();

            // Act
            var actual = composer.ComposeDropTable("\"Sales\".\"Invoice\"");

            // Assert
            Assert.AreEqual("DROP TABLE IF EXISTS \"Sales\".\"Invoice\";", actual, StringComparer.Ordinal);
        }

        #endregion

        #region ComposeSchema

        [TestMethod]
        public void TestSqliteSchemaComposerComposeSchema()
        {
            // Setup
            var composer = new SqliteSchemaComposer();
            var schema = GetPersonSchema();

            // Act
            var actual = composer.ComposeSchema(schema).ToList();

            // Assert
            Assert.AreEqual(2, actual.Count);
            StringAssert.StartsWith(actual[0], "CREATE TABLE \"dbo\".\"Person\"", StringComparison.Ordinal);
            StringAssert.StartsWith(actual[1], "CREATE INDEX \"dbo\".\"IX_Person_Name\" ON \"Person\"", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeSchemaWithOnlyColumns()
        {
            // Setup
            var composer = new SqliteSchemaComposer();
            var schema = new TableSchema("NoKey", null);
            schema.Columns.Add(GetColumn("Value", "varchar", typeof(string), 1));

            // Act
            var actual = composer.ComposeSchema(schema).ToList();

            // Assert
            Assert.AreEqual(1, actual.Count);
        }

        [TestMethod]
        public void ThrowExceptionOnSqliteSchemaComposerComposeSchemaIfTheSchemaIsNull()
        {
            // Setup
            var composer = new SqliteSchemaComposer();

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => composer.ComposeSchema(null));
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeSchemas()
        {
            // Setup
            var composer = new SqliteSchemaComposer();
            var first = GetPersonSchema();
            var second = new TableSchema("Country", "dbo");
            second.Columns.Add(GetColumn("Id", "int", typeof(int), 1, false));
            second.Indexes.Add(new IndexInfo("IX_Country_Id") { Columns = { "Id" } });

            // Act
            var actual = composer.ComposeSchemas(new[] { first, second }).ToList();

            // Assert
            Assert.AreEqual(5, actual.Count);
            StringAssert.StartsWith(actual[0], "CREATE TABLE \"dbo\".\"Person\"", StringComparison.Ordinal);
            StringAssert.StartsWith(actual[1], "CREATE TABLE \"dbo\".\"Country\"", StringComparison.Ordinal);
            StringAssert.StartsWith(actual[2], "CREATE INDEX \"dbo\".\"IX_Person_Name\" ON \"Person\"", StringComparison.Ordinal);
            StringAssert.StartsWith(actual[3], "CREATE INDEX \"dbo\".\"IX_Country_Id\" ON \"Country\"", StringComparison.Ordinal);
            Assert.AreEqual(string.Empty, actual[4]);
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeSchemasCreatesTheForeignKeysWithTheTables()
        {
            // Setup
            var composer = new SqliteSchemaComposer();
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
            StringAssert.Contains(actual[0], "CONSTRAINT \"FK_CycleA_CycleB\" FOREIGN KEY (\"BId\") REFERENCES \"CycleB\" (\"Id\")", StringComparison.Ordinal);
            StringAssert.Contains(actual[1], "CONSTRAINT \"FK_CycleB_CycleA\" FOREIGN KEY (\"AId\") REFERENCES \"CycleA\" (\"Id\")", StringComparison.Ordinal);
            Assert.AreEqual(string.Empty, actual[2]);
            Assert.AreEqual(string.Empty, actual[3]);
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeSchemasWithoutTables()
        {
            // Setup
            var composer = new SqliteSchemaComposer();

            // Act
            var actual = composer.ComposeSchemas(Enumerable.Empty<TableSchema>()).ToList();

            // Assert
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeSchemasOfOneTableIsTheSameAsComposeSchema()
        {
            // Setup
            var composer = new SqliteSchemaComposer();
            var schema = GetPersonSchema();

            // Act
            var actual = composer.ComposeSchemas(new[] { schema }).ToList();
            var expected = composer.ComposeSchema(schema).ToList();
            expected.Add(string.Empty);

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ThrowExceptionOnSqliteSchemaComposerComposeSchemasIfTheSchemasAreNull()
        {
            // Setup
            var composer = new SqliteSchemaComposer();

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => composer.ComposeSchemas(null));
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeCreateTableWithNameThatNeedsQuoting()
        {
            // Setup
            var composer = new SqliteSchemaComposer();
            var schema = new TableSchema("Odd.Name", "dbo");
            schema.Columns.Add(GetColumn("Unit Price", "decimal", typeof(decimal), 1, false, null, 10, 2));

            // Act
            var actual = composer.ComposeCreateTable(schema);

            // Assert
            StringAssert.StartsWith(actual, "CREATE TABLE \"dbo\".\"Odd.Name\" (", StringComparison.Ordinal);
            StringAssert.Contains(actual, "\"Unit Price\" decimal(10,2) NOT NULL", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeCreateTableWithClosingBracketInTheNames()
        {
            // Setup
            var composer = new SqliteSchemaComposer();
            var schema = new TableSchema("Weird\"Name", "dbo");
            schema.Columns.Add(GetColumn("Id", "int", typeof(int), 1, false));

            // Act
            var actual = composer.ComposeCreateTable(schema);

            // Assert
            StringAssert.StartsWith(actual, "CREATE TABLE \"dbo\".\"Weird\"\"Name\" (", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeCreateTableWithForeignKeyToNameThatNeedQuoting()
        {
            // Setup
            var composer = new SqliteSchemaComposer();
            var schema = new TableSchema("Odd.Child", "dbo");
            schema.Columns.Add(GetColumn("ParentId", "int", typeof(int), 1));
            schema.ForeignKeys.Add(new ForeignKeyInfo("FK_OddChild_OddName")
            {
                Columns = { "ParentId" },
                ReferencedTable = new TableInfo("Odd.Name", "dbo"),
                ReferencedColumns = { "Id" }
            });

            // Act
            var actual = composer.ComposeCreateTable(schema);

            // Assert
            StringAssert.StartsWith(actual, "CREATE TABLE \"dbo\".\"Odd.Child\" (", StringComparison.Ordinal);
            StringAssert.Contains(actual, "CONSTRAINT \"FK_OddChild_OddName\" FOREIGN KEY (\"ParentId\") REFERENCES \"Odd.Name\" (\"Id\")", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeSchemasWithNamesThatNeedQuoting()
        {
            // Setup
            var composer = new SqliteSchemaComposer();
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
            Assert.AreEqual(4, actual.Count);
            StringAssert.StartsWith(actual[0], "CREATE TABLE \"dbo\".\"Odd.Child\"", StringComparison.Ordinal);
            StringAssert.StartsWith(actual[1], "CREATE TABLE \"dbo\".\"Odd.Name\"", StringComparison.Ordinal);
            Assert.AreEqual("CREATE INDEX \"dbo\".\"IX_OddName_Id\" ON \"Odd.Name\" (\"Id\");", actual[2], StringComparer.Ordinal);
            StringAssert.Contains(actual[0], "CONSTRAINT \"FK_OddChild_OddName\" FOREIGN KEY (\"ParentId\")", StringComparison.Ordinal);
            StringAssert.Contains(actual[0], "REFERENCES \"Odd.Name\" (\"Id\")", StringComparison.Ordinal);
            Assert.AreEqual(string.Empty, actual[3]);
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeDropTableWithClosingBracketInTheName()
        {
            // Setup
            var composer = new SqliteSchemaComposer();

            // Act
            var actual = composer.ComposeDropTable("\"dbo\".\"Weird\"\"Name\"");

            // Assert
            Assert.AreEqual("DROP TABLE IF EXISTS \"dbo\".\"Weird\"\"Name\";", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeCreateIndexWithTableNameThatNeedsQuoting()
        {
            // Setup
            var composer = new SqliteSchemaComposer();
            var index = new IndexInfo("IX") { Columns = { "Id" } };

            // Act
            var actual = composer.ComposeCreateIndex("dbo.\"Odd.Name\"", index);

            // Assert
            Assert.AreEqual("CREATE INDEX \"dbo\".\"IX\" ON \"Odd.Name\" (\"Id\");", actual, StringComparer.Ordinal);
        }

        #endregion

        #region ComposeTypeName

        [TestMethod]
        public void TestSqliteSchemaComposerComposeTypeNameForSizedUnicodeString()
        {
            Assert.AreEqual("varchar(128)", ComposeTypeName("varchar", typeof(string), 128), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeTypeNameForUnicodeStringWithMaxSize()
        {
            Assert.AreEqual("varchar", ComposeTypeName("varchar", typeof(string), -1), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeTypeNameForUnicodeStringWithoutSize()
        {
            Assert.AreEqual("varchar", ComposeTypeName("varchar", typeof(string)), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeTypeNameForVarChar()
        {
            Assert.AreEqual("varchar(50)", ComposeTypeName("varchar", typeof(string), 50), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeTypeNameForFixedLengthCharacters()
        {
            Assert.AreEqual("char(10)", ComposeTypeName("char", typeof(string), 10), StringComparer.Ordinal);
            Assert.AreEqual("char(4)", ComposeTypeName("char", typeof(string), 4), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeTypeNameForFixedLengthCharactersWithoutSize()
        {
            Assert.AreEqual("char", ComposeTypeName("char", typeof(string)), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeTypeNameForBinary()
        {
            Assert.AreEqual("blob", ComposeTypeName("binary", typeof(byte[]), 16), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeTypeNameForVarBinary()
        {
            Assert.AreEqual("blob", ComposeTypeName("varbinary", typeof(byte[]), 256), StringComparer.Ordinal);
            Assert.AreEqual("blob", ComposeTypeName("varbinary", typeof(byte[]), -1), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeTypeNameForDecimal()
        {
            Assert.AreEqual("decimal(18,2)", ComposeTypeName("decimal", typeof(decimal), 9, 18, 2), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeTypeNameForNumericWithoutPrecision()
        {
            Assert.AreEqual("decimal", ComposeTypeName("decimal", typeof(decimal)), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeTypeNameForTemporalTypesWithScale()
        {
            Assert.AreEqual("datetime", ComposeTypeName("datetime", typeof(DateTime), scale: 3), StringComparer.Ordinal);
            Assert.AreEqual("datetime", ComposeTypeName("timestamp", typeof(DateTime), scale: 9), StringComparer.Ordinal);
            Assert.AreEqual("time", ComposeTypeName("time", typeof(TimeSpan), scale: 5), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeTypeNameForTemporalTypesWithoutScale()
        {
            Assert.AreEqual("datetime", ComposeTypeName("datetime", typeof(DateTime)), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeTypeNameForSimpleTypes()
        {
            Assert.AreEqual("integer", ComposeTypeName("int", typeof(int)), StringComparer.Ordinal);
            Assert.AreEqual("bigint", ComposeTypeName("bigint", typeof(long)), StringComparer.Ordinal);
            Assert.AreEqual("boolean", ComposeTypeName("bit", typeof(bool)), StringComparer.Ordinal);
            Assert.AreEqual("char(36)", ComposeTypeName("uuid", typeof(Guid)), StringComparer.Ordinal);
            Assert.AreEqual("double", ComposeTypeName("double", typeof(double)), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeTypeNameIsCaseInsensitive()
        {
            Assert.AreEqual("integer", ComposeTypeName("INT", typeof(int)), StringComparer.Ordinal);
            Assert.AreEqual("varchar(20)", ComposeTypeName("VarChar", typeof(string), 20), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeTypeNameForTheTypesOfOtherDatabaseEngines()
        {
            Assert.AreEqual("boolean", ComposeTypeName("boolean", typeof(bool)), StringComparer.Ordinal);
            Assert.AreEqual("char(36)", ComposeTypeName("uniqueidentifier", typeof(Guid)), StringComparer.Ordinal);
            Assert.AreEqual("varchar(50)", ComposeTypeName("nvarchar", typeof(string), 50), StringComparer.Ordinal);
            Assert.AreEqual("blob", ComposeTypeName("bytea", typeof(byte[])), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeTypeNameFallsBackToTheClientTypeIfThereIsNoDatabaseType()
        {
            Assert.AreEqual("bigint", ComposeTypeName(null, typeof(long)), StringComparer.Ordinal);
            Assert.AreEqual("datetime", ComposeTypeName(null, typeof(DateTime)), StringComparer.Ordinal);
            Assert.AreEqual("text", ComposeTypeName(null, typeof(string), 20), StringComparer.Ordinal);
            Assert.AreEqual("text", ComposeTypeName(null, typeof(object)), StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnSqliteSchemaComposerComposeTypeNameIfTheColumnIsNull()
        {
            // Setup
            var composer = new SqliteSchemaComposer();

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => composer.ComposeTypeName(null));
        }

        [TestMethod]
        public void ThrowExceptionOnSqliteSchemaComposerComposeTypeNameIfTheColumnHasNoField()
        {
            // Setup
            var composer = new SqliteSchemaComposer();

            // Act/Assert
            Assert.Throws<ArgumentException>(() => composer.ComposeTypeName(new ColumnInfo()));
        }

        #endregion

        #region Interface

        [TestMethod]
        public void TestSqliteSchemaComposerIsASchemaComposer()
        {
            // Act
            var composer = new SqliteSchemaComposer();

            // Assert
            Assert.IsInstanceOfType<ISchemaComposer>(composer);
        }

        #endregion

        #region ComposeName / Exists

        [TestMethod]
        public void TestSqliteSchemaComposerComposeName()
        {
            // Act/Assert
            var composer = new SqliteSchemaComposer();
            Assert.AreEqual("dbo.Person", composer.ComposeName(new TableInfo("Person", "dbo")));
            Assert.AreEqual("dbo.\"Odd.Name\"", composer.ComposeName(new TableInfo("Odd.Name", "dbo")));
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeNameWithoutSchema()
        {
            // Act/Assert
            Assert.AreEqual("Person", new SqliteSchemaComposer().ComposeName(new TableInfo("Person", null)));
        }

        [TestMethod]
        public void ThrowExceptionOnSqliteSchemaComposerComposeNameIfTheTableIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new SqliteSchemaComposer().ComposeName(null));
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeTableExists()
        {
            // Act/Assert
            Assert.AreEqual("SELECT CASE WHEN EXISTS (SELECT 1 FROM \"sales\".sqlite_master WHERE type = 'table' AND name = 'Person' COLLATE NOCASE) THEN 1 ELSE 0 END;", new SqliteSchemaComposer().ComposeTableExists("sales.Person"));
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeColumnExists()
        {
            // Act/Assert
            Assert.AreEqual("SELECT CASE WHEN EXISTS (SELECT 1 FROM pragma_table_xinfo('Person', 'sales') WHERE name = 'Name' COLLATE NOCASE) THEN 1 ELSE 0 END;", new SqliteSchemaComposer().ComposeColumnExists("sales.Person", "Name"));
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeIndexExists()
        {
            // Act/Assert
            Assert.AreEqual("SELECT CASE WHEN EXISTS (SELECT 1 FROM \"main\".sqlite_master WHERE type = 'index' AND tbl_name = 'Person' COLLATE NOCASE AND name = 'IX_Person_Name' COLLATE NOCASE) THEN 1 ELSE 0 END;", new SqliteSchemaComposer().ComposeIndexExists("Person", "IX_Person_Name"));
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposeExistsEscapesTheSingleQuotes()
        {
            // Act
            var composer = new SqliteSchemaComposer();

            // Assert
            StringAssert.Contains(composer.ComposeColumnExists("dbo.Person", "O'Brien"), "O''Brien", StringComparison.Ordinal);
            StringAssert.Contains(composer.ComposeIndexExists("dbo.Person", "IX_O'Brien"), "IX_O''Brien", StringComparison.Ordinal);
            StringAssert.Contains(composer.ComposeTableExists("dbo.\"O'Brien\""), "O''Brien", StringComparison.Ordinal);
        }

        #endregion
    }
}
