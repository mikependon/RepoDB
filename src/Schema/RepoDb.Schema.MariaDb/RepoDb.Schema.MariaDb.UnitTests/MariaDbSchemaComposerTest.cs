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

namespace RepoDb.Schema.MariaDb.UnitTests
{
    [TestClass]
    public class MariaDbSchemaComposerTest
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
            new MariaDbSchemaComposer().ComposeTypeName(GetColumn("Column", databaseType, type, 1, true, size, precision, scale));

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
                Collation = "utf8mb4_general_ci",
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
            schema.CheckConstraints.Add(new CheckConstraintInfo("CK_Person_Age") { Expression = "(`Age` >= 0)" });
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
        public void TestMariaDbSchemaComposerComposeCreateTable()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();
            var schema = GetPersonSchema();

            // Act
            var actual = composer.ComposeCreateTable(schema);
            var expected = string.Join(Environment.NewLine,
                "CREATE TABLE `dbo`.`Person` (",
                "    `Id` bigint NOT NULL AUTO_INCREMENT,",
                "    `Name` varchar(128) COLLATE utf8mb4_general_ci NOT NULL,",
                "    `Age` int NULL DEFAULT 0,",
                "    PRIMARY KEY (`Id`),",
                "    UNIQUE KEY `UQ_Person_Name` (`Name`),",
                "    CONSTRAINT `CK_Person_Age` CHECK ((`Age` >= 0))",
                ");");

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeCreateTableWithoutSchemaName()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();
            var schema = new TableSchema("Person", null);
            schema.Columns.Add(GetColumn("Name", "varchar", typeof(string), 1, false, 50));

            // Act
            var actual = composer.ComposeCreateTable(schema);
            var expected = string.Join(Environment.NewLine,
                "CREATE TABLE `Person` (",
                "    `Name` varchar(50) NOT NULL",
                ");");

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeCreateTableOrdersTheColumnsByOrdinal()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();
            var schema = new TableSchema("Person", null);
            schema.Columns.Add(GetColumn("Second", "int", typeof(int), 2));
            schema.Columns.Add(GetColumn("First", "int", typeof(int), 1));

            // Act
            var actual = composer.ComposeCreateTable(schema);

            // Assert
            Assert.IsTrue(actual.IndexOf("`First`", StringComparison.Ordinal) < actual.IndexOf("`Second`", StringComparison.Ordinal));
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeCreateTableWithComputedColumn()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();
            var schema = new TableSchema("Person", null);
            schema.Columns.Add(new ColumnInfo
            {
                Ordinal = 1,
                ComputedExpression = "upper(`Name`)",
                Field = new DbField("NameUpper", false, false, true, typeof(string), 128, 0, 0, "varchar")
            });

            // Act
            var actual = composer.ComposeCreateTable(schema);

            // Assert
            StringAssert.Contains(actual, "`NameUpper` varchar(128) GENERATED ALWAYS AS (upper(`Name`)) STORED", StringComparison.Ordinal);
            Assert.IsFalse(actual.Contains("DEFAULT", StringComparison.Ordinal));
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeCreateTableIgnoresTheDefaultOfAnIdentityColumn()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();
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
            StringAssert.Contains(actual, "`Id` int NOT NULL AUTO_INCREMENT", StringComparison.Ordinal);
            Assert.IsFalse(actual.Contains("DEFAULT", StringComparison.Ordinal));
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeCreateTableDoesNotCollateTheNonCharacterColumns()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();
            var schema = new TableSchema("Person", null);
            schema.Columns.Add(new ColumnInfo
            {
                Ordinal = 1,
                Collation = "utf8mb4_general_ci",
                Field = new DbField("Age", false, false, true, typeof(int), 4, 10, 0, "int")
            });

            // Act
            var actual = composer.ComposeCreateTable(schema);

            // Assert
            Assert.IsFalse(actual.Contains("COLLATE", StringComparison.Ordinal));
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeCreateTableWithUnnamedPrimaryKey()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();
            var schema = new TableSchema("Person", null);
            schema.Columns.Add(GetColumn("Id", "int", typeof(int), 1, false));
            schema.PrimaryKey = new PrimaryKeyInfo(null) { Columns = { "Id" } };

            // Act
            var actual = composer.ComposeCreateTable(schema);

            // Assert
            StringAssert.Contains(actual, "    PRIMARY KEY (`Id`)", StringComparison.Ordinal);
            Assert.IsFalse(actual.Contains("CONSTRAINT", StringComparison.Ordinal));
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeCreateTableWithCompositePrimaryKey()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();
            var schema = new TableSchema("OrderLine", null);
            schema.Columns.Add(GetColumn("OrderId", "int", typeof(int), 1, false));
            schema.Columns.Add(GetColumn("LineNumber", "int", typeof(int), 2, false));
            schema.PrimaryKey = new PrimaryKeyInfo("PK_OrderLine") { Columns = { "OrderId", "LineNumber" } };

            // Act
            var actual = composer.ComposeCreateTable(schema);

            // Assert
            StringAssert.Contains(actual, "PRIMARY KEY (`OrderId`, `LineNumber`)", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeCreateTableWithoutPrimaryKey()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();
            var schema = new TableSchema("NoKey", null);
            schema.Columns.Add(GetColumn("Value", "varchar", typeof(string), 1));

            // Act
            var actual = composer.ComposeCreateTable(schema);

            // Assert
            Assert.IsFalse(actual.Contains("PRIMARY KEY", StringComparison.Ordinal));
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeCreateTableQuotesTheIdentifiers()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();
            var schema = new TableSchema("Odd`Table", null);
            schema.Columns.Add(GetColumn("Odd`Column", "int", typeof(int), 1));

            // Act
            var actual = composer.ComposeCreateTable(schema);

            // Assert
            StringAssert.Contains(actual, "`Odd``Table`", StringComparison.Ordinal);
            StringAssert.Contains(actual, "`Odd``Column`", StringComparison.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbSchemaComposerComposeCreateTableIfTheSchemaIsNull()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => composer.ComposeCreateTable(null));
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeCreateTableWithNonClusteredPrimaryKey()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();
            var schema = new TableSchema("Product", null);
            schema.Columns.Add(GetColumn("Id", "int", typeof(int), 1, false));
            schema.PrimaryKey = new PrimaryKeyInfo("PK_Product") { IsClustered = false, Columns = { "Id" } };

            // Act
            var actual = composer.ComposeCreateTable(schema);

            // Assert
            StringAssert.Contains(actual, "PRIMARY KEY (`Id`)", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeCreateTableWithClusteredPrimaryKeyByDefault()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();
            var schema = new TableSchema("Product", null);
            schema.Columns.Add(GetColumn("Id", "int", typeof(int), 1, false));
            schema.PrimaryKey = new PrimaryKeyInfo("PK_Product") { Columns = { "Id" } };

            // Act
            var actual = composer.ComposeCreateTable(schema);

            // Assert
            StringAssert.Contains(actual, "PRIMARY KEY (`Id`)", StringComparison.Ordinal);
            Assert.IsFalse(actual.Contains("CONSTRAINT", StringComparison.Ordinal));
        }

        #endregion

        #region ComposeCreateIndex

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeCreateIndex()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();
            var index = new IndexInfo("IX_Person_Name") { Columns = { "Name" } };

            // Act
            var actual = composer.ComposeCreateIndex("dbo.Person", index);

            // Assert
            Assert.AreEqual("CREATE INDEX `IX_Person_Name` ON `dbo`.`Person` (`Name`);", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeCreateIndexWithUniqueAndIncludedColumns()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();
            var index = new IndexInfo("IX_Person_Name")
            {
                IsUnique = true,
                Columns = { "Name", "Age" },
                IncludedColumns = { "Salary", "CountryId" }
            };

            // Act
            var actual = composer.ComposeCreateIndex("`dbo`.`Person`", index);
            var expected = "CREATE UNIQUE INDEX `IX_Person_Name` ON `dbo`.`Person` (`Name`, `Age`);";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbSchemaComposerComposeCreateIndexIfTheIndexIsNull()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => composer.ComposeCreateIndex("Person", null));
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeCreateIndexWithClusteredIndex()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();
            var index = new IndexInfo("CIX_Product_Code") { IsUnique = true, IsClustered = true, Columns = { "Code" } };

            // Act
            var actual = composer.ComposeCreateIndex("dbo.Product", index);

            // Assert
            Assert.AreEqual("CREATE UNIQUE INDEX `CIX_Product_Code` ON `dbo`.`Product` (`Code`);", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeCreateIndexWithNonUniqueClusteredIndex()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();
            var index = new IndexInfo("CIX") { IsClustered = true, Columns = { "Code" } };

            // Act
            var actual = composer.ComposeCreateIndex("Product", index);

            // Assert
            Assert.AreEqual("CREATE INDEX `CIX` ON `Product` (`Code`);", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeCreateIndexWithDescendingKey()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();
            var index = new IndexInfo("IX_Product_Category_Price")
            {
                Columns = { "Category", "Price" },
                DescendingColumns = { "Price" },
                IncludedColumns = { "Name" }
            };

            // Act
            var actual = composer.ComposeCreateIndex("dbo.Product", index);
            var expected = "CREATE INDEX `IX_Product_Category_Price` ON `dbo`.`Product` (`Category`, `Price` DESC);";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeCreateIndexWithAllDescendingKeys()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();
            var index = new IndexInfo("IX") { Columns = { "A", "B" }, DescendingColumns = { "A", "B" } };

            // Act
            var actual = composer.ComposeCreateIndex("T", index);

            // Assert
            Assert.AreEqual("CREATE INDEX `IX` ON `T` (`A` DESC, `B` DESC);", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeCreateIndexWithFilter()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();
            var index = new IndexInfo("IX_Product_Active_Name")
            {
                Columns = { "Name" },
                Filter = "(`IsActive`=(1))"
            };

            // Act
            var actual = composer.ComposeCreateIndex("dbo.Product", index);

            // Assert
            Assert.AreEqual("CREATE INDEX `IX_Product_Active_Name` ON `dbo`.`Product` (`Name`);", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeCreateIndexWithAllTheOptions()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();
            var index = new IndexInfo("IX")
            {
                IsUnique = true,
                IsClustered = true,
                Columns = { "A", "B" },
                DescendingColumns = { "B" },
                IncludedColumns = { "C" },
                Filter = "(`A`>(0))"
            };

            // Act
            var actual = composer.ComposeCreateIndex("T", index);

            // Assert
            Assert.AreEqual("CREATE UNIQUE INDEX `IX` ON `T` (`A`, `B` DESC);", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeCreateIndexIgnoresTheBlankFilter()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();
            var index = new IndexInfo("IX") { Columns = { "A" }, Filter = "  " };

            // Act
            var actual = composer.ComposeCreateIndex("T", index);

            // Assert
            Assert.AreEqual("CREATE INDEX `IX` ON `T` (`A`);", actual, StringComparer.Ordinal);
        }

        #endregion

        #region ComposeAddForeignKey

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeAddForeignKey()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();
            var foreignKey = new ForeignKeyInfo("FK_Person_Country")
            {
                Columns = { "CountryId" },
                ReferencedTable = new TableInfo("Country", "dbo"),
                ReferencedColumns = { "Id" }
            };

            // Act
            var actual = composer.ComposeAddForeignKey("dbo.Person", foreignKey);
            var expected = "ALTER TABLE `dbo`.`Person` ADD CONSTRAINT `FK_Person_Country` FOREIGN KEY (`CountryId`) REFERENCES `dbo`.`Country` (`Id`);";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeAddForeignKeyWithRules()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();
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
            StringAssert.EndsWith(actual, "ON DELETE SET NULL ON UPDATE CASCADE;", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeAddForeignKeyIgnoresTheSetDefaultRule()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();
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
        public void TestMariaDbSchemaComposerComposeAddForeignKeyWithRestrictRule()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();
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
            StringAssert.Contains(actual, "ON DELETE RESTRICT", StringComparison.Ordinal);
            StringAssert.Contains(actual, "ON UPDATE RESTRICT", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeAddForeignKeyWithCompositeColumns()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();
            var foreignKey = new ForeignKeyInfo("FK_Line_Order")
            {
                Columns = { "OrderId", "LineNumber" },
                ReferencedTable = new TableInfo("OrderLine", "dbo"),
                ReferencedColumns = { "OrderId", "LineNumber" }
            };

            // Act
            var actual = composer.ComposeAddForeignKey("dbo.Shipment", foreignKey);

            // Assert
            StringAssert.Contains(actual, "FOREIGN KEY (`OrderId`, `LineNumber`) REFERENCES `dbo`.`OrderLine` (`OrderId`, `LineNumber`)", StringComparison.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbSchemaComposerComposeAddForeignKeyIfTheForeignKeyIsNull()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => composer.ComposeAddForeignKey("Person", null));
        }

        #endregion

        #region ComposeAddColumn

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeAddColumn()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();
            var column = GetColumn("Nickname", "varchar", typeof(string), 1, true, 50);

            // Act
            var actual = composer.ComposeAddColumn("dbo.Person", column);

            // Assert
            Assert.AreEqual("ALTER TABLE `dbo`.`Person` ADD COLUMN `Nickname` varchar(50) NULL;", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeAddColumnWithDefault()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();
            var column = GetColumn("Age", "int", typeof(int), 1, false);
            column.DefaultExpression = "0";

            // Act
            var actual = composer.ComposeAddColumn("Person", column);

            // Assert
            Assert.AreEqual("ALTER TABLE `Person` ADD COLUMN `Age` int NOT NULL DEFAULT 0;", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbSchemaComposerComposeAddColumnIfTheColumnIsNull()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => composer.ComposeAddColumn("Person", null));
        }

        #endregion

        #region ComposeDropTable

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeDropTable()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();

            // Act
            var actual = composer.ComposeDropTable("dbo.Person");

            // Assert
            Assert.AreEqual("DROP TABLE IF EXISTS `dbo`.`Person`;", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeDropTableWithQuotedName()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();

            // Act
            var actual = composer.ComposeDropTable("`Sales`.`Invoice`");

            // Assert
            Assert.AreEqual("DROP TABLE IF EXISTS `Sales`.`Invoice`;", actual, StringComparer.Ordinal);
        }

        #endregion

        #region ComposeSchema

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeSchema()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();
            var schema = GetPersonSchema();

            // Act
            var actual = composer.ComposeSchema(schema).ToList();

            // Assert
            Assert.AreEqual(3, actual.Count);
            StringAssert.StartsWith(actual[0], "CREATE TABLE `dbo`.`Person`", StringComparison.Ordinal);
            StringAssert.StartsWith(actual[1], "CREATE INDEX `IX_Person_Name` ON `dbo`.`Person`", StringComparison.Ordinal);
            StringAssert.StartsWith(actual[2], "ALTER TABLE `dbo`.`Person` ADD CONSTRAINT `FK_Person_Country`", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeSchemaWithOnlyColumns()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();
            var schema = new TableSchema("NoKey", null);
            schema.Columns.Add(GetColumn("Value", "varchar", typeof(string), 1));

            // Act
            var actual = composer.ComposeSchema(schema).ToList();

            // Assert
            Assert.AreEqual(1, actual.Count);
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbSchemaComposerComposeSchemaIfTheSchemaIsNull()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => composer.ComposeSchema(null));
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeSchemas()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();
            var first = GetPersonSchema();
            var second = new TableSchema("Country", "dbo");
            second.Columns.Add(GetColumn("Id", "int", typeof(int), 1, false));
            second.Indexes.Add(new IndexInfo("IX_Country_Id") { Columns = { "Id" } });

            // Act
            var actual = composer.ComposeSchemas(new[] { first, second }).ToList();

            // Assert
            Assert.AreEqual(5, actual.Count);
            StringAssert.StartsWith(actual[0], "CREATE TABLE `dbo`.`Person`", StringComparison.Ordinal);
            StringAssert.StartsWith(actual[1], "CREATE TABLE `dbo`.`Country`", StringComparison.Ordinal);
            StringAssert.StartsWith(actual[2], "CREATE INDEX `IX_Person_Name` ON `dbo`.`Person`", StringComparison.Ordinal);
            StringAssert.StartsWith(actual[3], "CREATE INDEX `IX_Country_Id` ON `dbo`.`Country`", StringComparison.Ordinal);
            StringAssert.StartsWith(actual[4], "ALTER TABLE `dbo`.`Person` ADD CONSTRAINT `FK_Person_Country`", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeSchemasCreatesTheForeignKeysAfterAllTheTables()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();
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
            StringAssert.StartsWith(actual[0], "CREATE TABLE `dbo`.`CycleA`", StringComparison.Ordinal);
            StringAssert.StartsWith(actual[1], "CREATE TABLE `dbo`.`CycleB`", StringComparison.Ordinal);
            StringAssert.StartsWith(actual[2], "ALTER TABLE `dbo`.`CycleA` ADD CONSTRAINT `FK_CycleA_CycleB`", StringComparison.Ordinal);
            StringAssert.StartsWith(actual[3], "ALTER TABLE `dbo`.`CycleB` ADD CONSTRAINT `FK_CycleB_CycleA`", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeSchemasWithoutTables()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();

            // Act
            var actual = composer.ComposeSchemas(Enumerable.Empty<TableSchema>()).ToList();

            // Assert
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeSchemasOfOneTableIsTheSameAsComposeSchema()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();
            var schema = GetPersonSchema();

            // Act
            var actual = composer.ComposeSchemas(new[] { schema }).ToList();
            var expected = composer.ComposeSchema(schema).ToList();

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbSchemaComposerComposeSchemasIfTheSchemasAreNull()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => composer.ComposeSchemas(null));
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeCreateTableWithNameThatNeedsQuoting()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();
            var schema = new TableSchema("Odd.Name", "dbo");
            schema.Columns.Add(GetColumn("Unit Price", "decimal", typeof(decimal), 1, false, null, 10, 2));

            // Act
            var actual = composer.ComposeCreateTable(schema);

            // Assert
            StringAssert.StartsWith(actual, "CREATE TABLE `dbo`.`Odd.Name` (", StringComparison.Ordinal);
            StringAssert.Contains(actual, "`Unit Price` decimal(10,2) NOT NULL", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeCreateTableWithClosingBracketInTheNames()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();
            var schema = new TableSchema("Weird`Name", "dbo");
            schema.Columns.Add(GetColumn("Id", "int", typeof(int), 1, false));

            // Act
            var actual = composer.ComposeCreateTable(schema);

            // Assert
            StringAssert.StartsWith(actual, "CREATE TABLE `dbo`.`Weird``Name` (", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeAddForeignKeyWithNamesThatNeedQuoting()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();
            var foreignKey = new ForeignKeyInfo("FK_OddChild_OddName")
            {
                Columns = { "ParentId" },
                ReferencedTable = new TableInfo("Odd.Name", "dbo"),
                ReferencedColumns = { "Id" }
            };

            // Act
            var actual = composer.ComposeAddForeignKey("`dbo`.`Odd.Child`", foreignKey);
            var expected = "ALTER TABLE `dbo`.`Odd.Child` ADD CONSTRAINT `FK_OddChild_OddName` FOREIGN KEY (`ParentId`) REFERENCES `dbo`.`Odd.Name` (`Id`);";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeSchemasWithNamesThatNeedQuoting()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();
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
            StringAssert.StartsWith(actual[0], "CREATE TABLE `dbo`.`Odd.Child`", StringComparison.Ordinal);
            StringAssert.StartsWith(actual[1], "CREATE TABLE `dbo`.`Odd.Name`", StringComparison.Ordinal);
            Assert.AreEqual("CREATE INDEX `IX_OddName_Id` ON `dbo`.`Odd.Name` (`Id`);", actual[2], StringComparer.Ordinal);
            StringAssert.Contains(actual[3], "ALTER TABLE `dbo`.`Odd.Child` ADD CONSTRAINT `FK_OddChild_OddName`", StringComparison.Ordinal);
            StringAssert.Contains(actual[3], "REFERENCES `dbo`.`Odd.Name` (`Id`)", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeDropTableWithClosingBracketInTheName()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();

            // Act
            var actual = composer.ComposeDropTable("`dbo`.`Weird``Name`");

            // Assert
            Assert.AreEqual("DROP TABLE IF EXISTS `dbo`.`Weird``Name`;", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeCreateIndexWithTableNameThatNeedsQuoting()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();
            var index = new IndexInfo("IX") { Columns = { "Id" } };

            // Act
            var actual = composer.ComposeCreateIndex("dbo.`Odd.Name`", index);

            // Assert
            Assert.AreEqual("CREATE INDEX `IX` ON `dbo`.`Odd.Name` (`Id`);", actual, StringComparer.Ordinal);
        }

        #endregion

        #region ComposeTypeName

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeTypeNameForSizedUnicodeString()
        {
            Assert.AreEqual("varchar(128)", ComposeTypeName("varchar", typeof(string), 128), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeTypeNameForUnicodeStringWithMaxSize()
        {
            Assert.AreEqual("longtext", ComposeTypeName("varchar", typeof(string), -1), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeTypeNameForUnicodeStringWithoutSize()
        {
            Assert.AreEqual("longtext", ComposeTypeName("varchar", typeof(string)), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeTypeNameForVarChar()
        {
            Assert.AreEqual("varchar(50)", ComposeTypeName("varchar", typeof(string), 50), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeTypeNameForFixedLengthCharacters()
        {
            Assert.AreEqual("char(10)", ComposeTypeName("char", typeof(string), 10), StringComparer.Ordinal);
            Assert.AreEqual("char(4)", ComposeTypeName("char", typeof(string), 4), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeTypeNameForFixedLengthCharactersWithoutSize()
        {
            Assert.AreEqual("char(1)", ComposeTypeName("char", typeof(string)), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeTypeNameForBinary()
        {
            Assert.AreEqual("binary(16)", ComposeTypeName("binary", typeof(byte[]), 16), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeTypeNameForVarBinary()
        {
            Assert.AreEqual("varbinary(256)", ComposeTypeName("varbinary", typeof(byte[]), 256), StringComparer.Ordinal);
            Assert.AreEqual("longblob", ComposeTypeName("varbinary", typeof(byte[]), -1), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeTypeNameForDecimal()
        {
            Assert.AreEqual("decimal(18,2)", ComposeTypeName("decimal", typeof(decimal), 9, 18, 2), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeTypeNameForNumericWithoutPrecision()
        {
            Assert.AreEqual("decimal(18,0)", ComposeTypeName("decimal", typeof(decimal)), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeTypeNameForTemporalTypesWithScale()
        {
            Assert.AreEqual("datetime(3)", ComposeTypeName("datetime", typeof(DateTime), scale: 3), StringComparer.Ordinal);
            Assert.AreEqual("timestamp(6)", ComposeTypeName("timestamp", typeof(DateTime), scale: 9), StringComparer.Ordinal);
            Assert.AreEqual("time(5)", ComposeTypeName("time", typeof(TimeSpan), scale: 5), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeTypeNameForTemporalTypesWithoutScale()
        {
            Assert.AreEqual("datetime", ComposeTypeName("datetime", typeof(DateTime)), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeTypeNameForSimpleTypes()
        {
            Assert.AreEqual("int", ComposeTypeName("int", typeof(int)), StringComparer.Ordinal);
            Assert.AreEqual("bigint", ComposeTypeName("bigint", typeof(long)), StringComparer.Ordinal);
            Assert.AreEqual("bit(1)", ComposeTypeName("bit", typeof(bool)), StringComparer.Ordinal);
            Assert.AreEqual("char(36)", ComposeTypeName("uuid", typeof(Guid)), StringComparer.Ordinal);
            Assert.AreEqual("double", ComposeTypeName("double", typeof(double)), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeTypeNameIsCaseInsensitive()
        {
            Assert.AreEqual("int", ComposeTypeName("INT", typeof(int)), StringComparer.Ordinal);
            Assert.AreEqual("varchar(20)", ComposeTypeName("VarChar", typeof(string), 20), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeTypeNameForTheTypesOfOtherDatabaseEngines()
        {
            Assert.AreEqual("tinyint(1)", ComposeTypeName("boolean", typeof(bool)), StringComparer.Ordinal);
            Assert.AreEqual("char(36)", ComposeTypeName("uuid", typeof(Guid)), StringComparer.Ordinal);
            Assert.AreEqual("longtext", ComposeTypeName("varchar", typeof(string)), StringComparer.Ordinal);
            Assert.AreEqual("longblob", ComposeTypeName("varbinary", typeof(byte[])), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeTypeNameFallsBackToTheClientTypeIfThereIsNoDatabaseType()
        {
            Assert.AreEqual("bigint", ComposeTypeName(null, typeof(long)), StringComparer.Ordinal);
            Assert.AreEqual("datetime", ComposeTypeName(null, typeof(DateTime)), StringComparer.Ordinal);
            Assert.AreEqual("varchar(20)", ComposeTypeName(null, typeof(string), 20), StringComparer.Ordinal);
            Assert.AreEqual("longtext", ComposeTypeName(null, typeof(object)), StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbSchemaComposerComposeTypeNameIfTheColumnIsNull()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => composer.ComposeTypeName(null));
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbSchemaComposerComposeTypeNameIfTheColumnHasNoField()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();

            // Act/Assert
            Assert.Throws<ArgumentException>(() => composer.ComposeTypeName(new ColumnInfo()));
        }

        #endregion

        #region Interface

        [TestMethod]
        public void TestMariaDbSchemaComposerIsASchemaComposer()
        {
            // Act
            var composer = new MariaDbSchemaComposer();

            // Assert
            Assert.IsInstanceOfType<ISchemaComposer>(composer);
        }

        #endregion

        #region ComposeName / Exists

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeName()
        {
            // Act/Assert
            var composer = new MariaDbSchemaComposer();
            Assert.AreEqual("dbo.Person", composer.ComposeName(new TableInfo("Person", "dbo")));
            Assert.AreEqual("dbo.`Odd.Name`", composer.ComposeName(new TableInfo("Odd.Name", "dbo")));
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeNameWithoutSchema()
        {
            // Act/Assert
            Assert.AreEqual("Person", new MariaDbSchemaComposer().ComposeName(new TableInfo("Person", null)));
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbSchemaComposerComposeNameIfTheTableIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new MariaDbSchemaComposer().ComposeName(null));
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeTableExists()
        {
            // Act/Assert
            Assert.AreEqual("SELECT CASE WHEN EXISTS (SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Person' AND TABLE_TYPE = 'BASE TABLE') THEN 1 ELSE 0 END;", new MariaDbSchemaComposer().ComposeTableExists("dbo.Person"));
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeColumnExists()
        {
            // Act/Assert
            Assert.AreEqual("SELECT CASE WHEN EXISTS (SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Person' AND COLUMN_NAME = 'Name') THEN 1 ELSE 0 END;", new MariaDbSchemaComposer().ComposeColumnExists("dbo.Person", "Name"));
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeIndexExists()
        {
            // Act/Assert
            Assert.AreEqual("SELECT CASE WHEN EXISTS (SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Person' AND INDEX_NAME = 'IX_Person_Name') THEN 1 ELSE 0 END;", new MariaDbSchemaComposer().ComposeIndexExists("dbo.Person", "IX_Person_Name"));
        }

        [TestMethod]
        public void TestMariaDbSchemaComposerComposeExistsEscapesTheSingleQuotes()
        {
            // Act
            var composer = new MariaDbSchemaComposer();

            // Assert
            StringAssert.Contains(composer.ComposeColumnExists("dbo.Person", "O'Brien"), "O''Brien", StringComparison.Ordinal);
            StringAssert.Contains(composer.ComposeIndexExists("dbo.Person", "IX_O'Brien"), "IX_O''Brien", StringComparison.Ordinal);
            StringAssert.Contains(composer.ComposeTableExists("dbo.\"O'Brien\""), "O''Brien", StringComparison.Ordinal);
        }

        #endregion
    }
}
