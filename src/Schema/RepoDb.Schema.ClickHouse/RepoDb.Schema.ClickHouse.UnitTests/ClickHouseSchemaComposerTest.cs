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

namespace RepoDb.Schema.ClickHouse.UnitTests
{
    [TestClass]
    public class ClickHouseSchemaComposerTest
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
            new ClickHouseSchemaComposer().ComposeTypeName(GetColumn("Column", databaseType, type, 1, true, size, precision, scale));

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
                Field = new DbField("Name", false, false, false, typeof(string), 128, 0, 0, "varchar")
            });
            schema.Columns.Add(new ColumnInfo
            {
                Ordinal = 3,
                DefaultExpression = "0",
                Field = new DbField("Age", false, false, true, typeof(int), 4, 10, 0, "int", true)
            });
            schema.PrimaryKey = new PrimaryKeyInfo("PK_Person") { Columns = { "Id" } };
            schema.CheckConstraints.Add(new CheckConstraintInfo("CK_Person_Age") { Expression = "(`Age` >= 0)" });
            schema.Indexes.Add(new IndexInfo("IX_Person_Name") { Columns = { "Name" } });
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
        public void TestClickHouseSchemaComposerComposeCreateTable()
        {
            // Setup
            var composer = new ClickHouseSchemaComposer();

            // Act
            var actual = composer.ComposeCreateTable(GetPersonSchema());

            // Assert
            var expected = string.Join(Environment.NewLine,
                "CREATE TABLE `dbo`.`Person` (",
                "    `Id` Int64,",
                "    `Name` String,",
                "    `Age` Nullable(Int32) DEFAULT 0,",
                "    CONSTRAINT `CK_Person_Age` CHECK (`Age` >= 0)",
                ") ENGINE = MergeTree ORDER BY `Id`");
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseSchemaComposerComposeCreateTableWithoutSchemaName()
        {
            // Setup
            var schema = new TableSchema("Country", null);
            schema.Columns.Add(GetColumn("Id", "int", typeof(int), 1, false));

            // Act
            var actual = new ClickHouseSchemaComposer().ComposeCreateTable(schema);

            // Assert
            StringAssert.StartsWith(actual, "CREATE TABLE `Country` (", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseSchemaComposerComposeCreateTableWithComputedColumn()
        {
            // Setup
            var schema = new TableSchema("Person", null);
            schema.Columns.Add(GetColumn("Name", "varchar", typeof(string), 1, false));
            var column = GetColumn("NameUpper", "varchar", typeof(string), 2, false);
            column.ComputedExpression = "upper(Name)";
            schema.Columns.Add(column);

            // Act
            var actual = new ClickHouseSchemaComposer().ComposeCreateTable(schema);

            // Assert
            StringAssert.Contains(actual, "`NameUpper` String MATERIALIZED upper(Name)", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseSchemaComposerComposeCreateTableWithCompositePrimaryKey()
        {
            // Setup
            var schema = new TableSchema("OrderLine", null);
            schema.Columns.Add(GetColumn("OrderId", "int", typeof(int), 1, false));
            schema.Columns.Add(GetColumn("LineNumber", "int", typeof(int), 2, false));
            schema.PrimaryKey = new PrimaryKeyInfo("PK_OrderLine") { Columns = { "OrderId", "LineNumber" } };

            // Act
            var actual = new ClickHouseSchemaComposer().ComposeCreateTable(schema);

            // Assert
            StringAssert.EndsWith(actual, ") ENGINE = MergeTree ORDER BY (`OrderId`, `LineNumber`)", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseSchemaComposerComposeCreateTableWithoutPrimaryKey()
        {
            // Setup
            var schema = new TableSchema("NoKey", null);
            schema.Columns.Add(GetColumn("Value", "varchar", typeof(string), 1));

            // Act
            var actual = new ClickHouseSchemaComposer().ComposeCreateTable(schema);

            // Assert
            StringAssert.EndsWith(actual, ") ENGINE = MergeTree ORDER BY tuple()", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseSchemaComposerComposeCreateTableWithNameThatNeedsQuoting()
        {
            // Setup
            var schema = new TableSchema("Odd`Name", null);
            schema.Columns.Add(GetColumn("Unit Price", "int", typeof(int), 1, false));

            // Act
            var actual = new ClickHouseSchemaComposer().ComposeCreateTable(schema);

            // Assert
            StringAssert.StartsWith(actual, "CREATE TABLE `Odd``Name` (", StringComparison.Ordinal);
            StringAssert.Contains(actual, "`Unit Price` Int32", StringComparison.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseSchemaComposerComposeCreateTableIfTheSchemaIsNull()
        {
            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => new ClickHouseSchemaComposer().ComposeCreateTable(null));
        }

        #endregion

        #region ComposeCreateIndex

        [TestMethod]
        public void TestClickHouseSchemaComposerComposeCreateIndex()
        {
            // Setup
            var index = new IndexInfo("IX_Person_Name") { Columns = { "Name", "Age" } };

            // Act
            var actual = new ClickHouseSchemaComposer().ComposeCreateIndex("dbo.Person", index);

            // Assert
            Assert.AreEqual("ALTER TABLE `dbo`.`Person` ADD INDEX `IX_Person_Name` (`Name`, `Age`) TYPE minmax GRANULARITY 1", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseSchemaComposerComposeCreateIndexWithTableNameThatNeedsQuoting()
        {
            // Setup
            var index = new IndexInfo("IX_Odd") { Columns = { "Id" } };

            // Act
            var actual = new ClickHouseSchemaComposer().ComposeCreateIndex("`Odd.Name`", index);

            // Assert
            StringAssert.StartsWith(actual, "ALTER TABLE `Odd.Name` ADD INDEX", StringComparison.Ordinal);
        }

        #endregion

        #region ComposeAddForeignKey

        [TestMethod]
        public void TestClickHouseSchemaComposerComposeAddForeignKeyIsBlank()
        {
            // Setup
            var foreignKey = new ForeignKeyInfo("FK_Person_Country")
            {
                Columns = { "Age" },
                ReferencedTable = new TableInfo("Country", "dbo"),
                ReferencedColumns = { "Id" }
            };

            // Act
            var actual = new ClickHouseSchemaComposer().ComposeAddForeignKey("dbo.Person", foreignKey);

            // Assert
            Assert.AreEqual(string.Empty, actual);
        }

        #endregion

        #region ComposeAddColumn

        [TestMethod]
        public void TestClickHouseSchemaComposerComposeAddColumn()
        {
            // Act
            var actual = new ClickHouseSchemaComposer().ComposeAddColumn("dbo.Person", GetColumn("Salary", "decimal", typeof(decimal), 1, true, null, 18, 2));

            // Assert
            Assert.AreEqual("ALTER TABLE `dbo`.`Person` ADD COLUMN `Salary` Nullable(Decimal(18, 2))", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseSchemaComposerComposeAddColumnWithDefault()
        {
            // Setup
            var column = GetColumn("Age", "int", typeof(int), 1, false);
            column.DefaultExpression = "0";

            // Act
            var actual = new ClickHouseSchemaComposer().ComposeAddColumn("Person", column);

            // Assert
            Assert.AreEqual("ALTER TABLE `Person` ADD COLUMN `Age` Int32 DEFAULT 0", actual, StringComparer.Ordinal);
        }

        #endregion

        #region ComposeDropTable

        [TestMethod]
        public void TestClickHouseSchemaComposerComposeDropTable()
        {
            // Act
            var actual = new ClickHouseSchemaComposer().ComposeDropTable("dbo.Person");

            // Assert
            Assert.AreEqual("DROP TABLE IF EXISTS `dbo`.`Person` SYNC", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseSchemaComposerComposeDropTableWithQuotedName()
        {
            // Act
            var actual = new ClickHouseSchemaComposer().ComposeDropTable("`Odd.Name`");

            // Assert
            Assert.AreEqual("DROP TABLE IF EXISTS `Odd.Name` SYNC", actual, StringComparer.Ordinal);
        }

        #endregion

        #region ComposeSchema

        [TestMethod]
        public void TestClickHouseSchemaComposerComposeSchema()
        {
            // Act
            var actual = new ClickHouseSchemaComposer().ComposeSchema(GetPersonSchema()).ToArray();

            // Assert
            Assert.AreEqual(3, actual.Length);
            StringAssert.StartsWith(actual[0], "CREATE TABLE `dbo`.`Person`", StringComparison.Ordinal);
            StringAssert.StartsWith(actual[1], "ALTER TABLE `dbo`.`Person` ADD INDEX `IX_Person_Name`", StringComparison.Ordinal);
            Assert.AreEqual(string.Empty, actual[2]);
        }

        [TestMethod]
        public void TestClickHouseSchemaComposerComposeSchemas()
        {
            // Setup
            var country = new TableSchema("Country", "dbo");
            country.Columns.Add(GetColumn("Id", "int", typeof(int), 1, false));

            // Act
            var actual = new ClickHouseSchemaComposer().ComposeSchemas(new[] { GetPersonSchema(), country }).ToArray();

            // Assert
            Assert.AreEqual(4, actual.Length);
            StringAssert.StartsWith(actual[0], "CREATE TABLE `dbo`.`Person`", StringComparison.Ordinal);
            StringAssert.StartsWith(actual[1], "CREATE TABLE `dbo`.`Country`", StringComparison.Ordinal);
            StringAssert.StartsWith(actual[2], "ALTER TABLE `dbo`.`Person` ADD INDEX", StringComparison.Ordinal);
            Assert.AreEqual(string.Empty, actual[3]);
        }

        #endregion

        #region Exists

        [TestMethod]
        public void TestClickHouseSchemaComposerComposeTableExists()
        {
            // Setup
            var composer = new ClickHouseSchemaComposer();

            // Act & Assert
            Assert.AreEqual("SELECT toUInt8(count() > 0) FROM system.tables WHERE database = 'dbo' AND name = 'Person'", composer.ComposeTableExists("dbo.Person"), StringComparer.Ordinal);
            Assert.AreEqual("SELECT toUInt8(count() > 0) FROM system.tables WHERE database = currentDatabase() AND name = 'Person'", composer.ComposeTableExists("Person"), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseSchemaComposerComposeColumnExists()
        {
            // Act
            var actual = new ClickHouseSchemaComposer().ComposeColumnExists("dbo.Person", "Name");

            // Assert
            Assert.AreEqual("SELECT toUInt8(count() > 0) FROM system.columns WHERE database = 'dbo' AND table = 'Person' AND name = 'Name'", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseSchemaComposerComposeIndexExists()
        {
            // Act
            var actual = new ClickHouseSchemaComposer().ComposeIndexExists("Person", "IX_Person_Name");

            // Assert
            Assert.AreEqual("SELECT toUInt8(count() > 0) FROM system.data_skipping_indices WHERE database = currentDatabase() AND table = 'Person' AND name = 'IX_Person_Name'", actual, StringComparer.Ordinal);
        }

        #endregion

        #region ComposeTypeName

        [TestMethod]
        public void TestClickHouseSchemaComposerComposeTypeNameForString()
        {
            // Act & Assert
            Assert.AreEqual("Nullable(String)", ComposeTypeName("varchar", typeof(string), 128), StringComparer.Ordinal);
            Assert.AreEqual("Nullable(String)", ComposeTypeName("text", typeof(string)), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseSchemaComposerComposeTypeNameForDecimal()
        {
            // Act & Assert
            Assert.AreEqual("Nullable(Decimal(18, 2))", ComposeTypeName("decimal", typeof(decimal), null, 18, 2), StringComparer.Ordinal);
            Assert.AreEqual("Nullable(Decimal(38, 9))", ComposeTypeName("numeric", typeof(decimal)), StringComparer.Ordinal);
            Assert.AreEqual("Nullable(Decimal(76, 10))", ComposeTypeName("decimal", typeof(decimal), null, 100, 10), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseSchemaComposerComposeTypeNameForDateTime()
        {
            // Act & Assert
            Assert.AreEqual("Nullable(DateTime64(4))", ComposeTypeName("datetime", typeof(DateTime), null, null, 4), StringComparer.Ordinal);
            Assert.AreEqual("Nullable(DateTime64(3))", ComposeTypeName("datetime", typeof(DateTime)), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseSchemaComposerComposeTypeNameForSimpleTypes()
        {
            // Act & Assert
            Assert.AreEqual("Nullable(Int32)", ComposeTypeName("int", typeof(int)), StringComparer.Ordinal);
            Assert.AreEqual("Nullable(Int64)", ComposeTypeName("bigint", typeof(long)), StringComparer.Ordinal);
            Assert.AreEqual("Nullable(Int16)", ComposeTypeName("smallint", typeof(short)), StringComparer.Ordinal);
            Assert.AreEqual("Nullable(Bool)", ComposeTypeName("boolean", typeof(bool)), StringComparer.Ordinal);
            Assert.AreEqual("Nullable(Float64)", ComposeTypeName("double", typeof(double)), StringComparer.Ordinal);
            Assert.AreEqual("Nullable(Float32)", ComposeTypeName("real", typeof(float)), StringComparer.Ordinal);
            Assert.AreEqual("Nullable(UUID)", ComposeTypeName("uuid", typeof(Guid)), StringComparer.Ordinal);
            Assert.AreEqual("Nullable(Date)", ComposeTypeName("date", typeof(DateTime)), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseSchemaComposerComposeTypeNameIsCaseInsensitive()
        {
            // Act & Assert
            Assert.AreEqual("Nullable(Int32)", ComposeTypeName("INT", typeof(int)), StringComparer.Ordinal);
            Assert.AreEqual("Nullable(String)", ComposeTypeName("VARCHAR", typeof(string)), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseSchemaComposerComposeTypeNameWithoutNullable()
        {
            // Setup
            var column = GetColumn("Id", "bigint", typeof(long), 1, false);

            // Act
            var actual = new ClickHouseSchemaComposer().ComposeTypeName(column);

            // Assert
            Assert.AreEqual("Int64", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseSchemaComposerComposeTypeNameKeepsTheTypeOfClickHouse()
        {
            // Setup
            var column = new ColumnInfo
            {
                Ordinal = 1,
                Field = new DbField("Tags", false, false, true, typeof(string), 0, (byte)0, (byte)0, "LowCardinality(String)", false, "ClickHouse")
            };

            // Act
            var actual = new ClickHouseSchemaComposer().ComposeTypeName(column);

            // Assert
            Assert.AreEqual("LowCardinality(Nullable(String))", actual, StringComparer.Ordinal);
        }

        #endregion
    }
}
