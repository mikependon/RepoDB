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

namespace RepoDb.Schema.AuroraDbPostgreSql.UnitTests
{
    [TestClass]
    public class AuroraDbPostgreSqlSchemaComposerTest
    {
        #region Helpers

        private static ColumnInfo Column(string name,
            string databaseType,
            Type type = null,
            int? size = null,
            byte? precision = null,
            byte? scale = null,
            bool isNullable = true,
            bool isIdentity = false) =>
            new ColumnInfo
            {
                Ordinal = 1,
                Field = new DbField(name, false, isIdentity, isNullable, type ?? typeof(object), size, precision, scale, databaseType)
            };

        private static string TypeName(ColumnInfo column) =>
            new AuroraDbPostgreSqlSchemaComposer().ComposeTypeName(column);

        private static TableSchema GetSchema() =>
            new TableSchema("Person", "public")
            {
                Columns =
                {
                    new ColumnInfo { Ordinal = 1, Field = new DbField("Id", true, true, false, typeof(long), 0, 0, 0, "bigint"), IdentitySeed = 10, IdentityIncrement = 5 },
                    new ColumnInfo { Ordinal = 2, Field = new DbField("Name", false, false, false, typeof(string), 50, 0, 0, "character varying"), DefaultExpression = "'none'::character varying" },
                    new ColumnInfo { Ordinal = 3, Field = new DbField("Age", false, false, true, typeof(int), 0, 0, 0, "integer") },
                    new ColumnInfo { Ordinal = 4, Field = new DbField("Double", false, false, true, typeof(int), 0, 0, 0, "integer"), ComputedExpression = "(\"Age\" * 2)" }
                },
                PrimaryKey = new PrimaryKeyInfo("pk_person") { Columns = { "Id" } },
                UniqueConstraints = { new UniqueConstraintInfo("uq_person_name") { Columns = { "Name" } } },
                CheckConstraints = { new CheckConstraintInfo("ck_person_age") { Expression = "\"Age\" >= 0" } },
                Indexes = { new IndexInfo("ix_person_name") { Columns = { "Name", "Age" }, DescendingColumns = { "Name" }, IncludedColumns = { "Double" }, IsUnique = true, Filter = "\"Age\" > 0" } },
                ForeignKeys = { new ForeignKeyInfo("fk_person_country") { Columns = { "Id" }, ReferencedTable = new TableInfo("country", "public"), ReferencedColumns = { "id" }, DeleteRule = CopySchemaForeignKeyRule.Cascade, UpdateRule = CopySchemaForeignKeyRule.SetNull } }
            };

        #endregion

        #region ComposeTypeName

        [TestMethod]
        [DataRow("character varying", 50, 0, 0, "varchar(50)")]
        [DataRow("nvarchar", 128, 0, 0, "varchar(128)")]
        [DataRow("nvarchar", -1, 0, 0, "varchar")]
        [DataRow("text", 0, 0, 0, "text")]
        [DataRow("character", 8, 0, 0, "char(8)")]
        [DataRow("nchar", 0, 0, 0, "char(1)")]
        [DataRow("numeric", 0, 12, 2, "numeric(12,2)")]
        [DataRow("decimal", 0, 18, 0, "numeric(18,0)")]
        [DataRow("numeric", 0, 0, 0, "numeric")]
        [DataRow("integer", 0, 0, 0, "integer")]
        [DataRow("int", 0, 10, 0, "integer")]
        [DataRow("bit", 0, 0, 0, "boolean")]
        [DataRow("varbinary", -1, 0, 0, "bytea")]
        [DataRow("uniqueidentifier", 0, 0, 0, "uuid")]
        [DataRow("jsonb", 0, 0, 0, "jsonb")]
        public void TestAuroraDbPostgreSqlSchemaComposerComposeTypeName(string databaseType, int size, int precision, int scale, string expected)
        {
            // Act
            var column = Column("c", databaseType, size: size, precision: (byte)precision, scale: (byte)scale);

            // Assert
            Assert.AreEqual(expected, TypeName(column));
        }

        [TestMethod]
        [DataRow("timestamp without time zone", 3, "timestamp(3)")]
        [DataRow("datetime2", 7, "timestamp(6)")]
        [DataRow("datetimeoffset", 2, "timestamptz(2)")]
        [DataRow("time without time zone", 0, "time(0)")]
        public void TestAuroraDbPostgreSqlSchemaComposerComposeTypeNameWithFractionalSeconds(string databaseType, int scale, string expected)
        {
            // Assert
            Assert.AreEqual(expected, TypeName(Column("c", databaseType, scale: (byte)scale)));
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposeTypeNameWithoutTheDatabaseTypeUsesTheClientType()
        {
            // Assert
            Assert.AreEqual("bigint", TypeName(Column("c", null, typeof(long))));
            Assert.AreEqual("boolean", TypeName(Column("c", " ", typeof(bool?))));
            Assert.AreEqual("text", TypeName(Column("c", null, typeof(object))));
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbPostgreSqlSchemaComposerComposeTypeNameIfTheColumnHasNoField()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new AuroraDbPostgreSqlSchemaComposer().ComposeTypeName(null));
            Assert.Throws<ArgumentException>(() => new AuroraDbPostgreSqlSchemaComposer().ComposeTypeName(new ColumnInfo()));
        }

        #endregion

        #region ComposeCreateTable

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposeCreateTable()
        {
            // Act
            var actual = new AuroraDbPostgreSqlSchemaComposer().ComposeCreateTable(GetSchema());

            // Assert
            var expected = string.Join(Environment.NewLine,
                "CREATE TABLE \"public\".\"Person\" (",
                "    \"Id\" bigint GENERATED BY DEFAULT AS IDENTITY (START WITH 10 INCREMENT BY 5) NOT NULL,",
                "    \"Name\" varchar(50) NOT NULL DEFAULT 'none'::character varying,",
                "    \"Age\" integer NULL,",
                "    \"Double\" integer GENERATED ALWAYS AS ((\"Age\" * 2)) STORED,",
                "    CONSTRAINT \"pk_person\" PRIMARY KEY (\"Id\"),",
                "    CONSTRAINT \"uq_person_name\" UNIQUE (\"Name\"),",
                "    CONSTRAINT \"ck_person_age\" CHECK (\"Age\" >= 0)",
                ");");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposeCreateTableWithoutKeysAndConstraints()
        {
            // Setup
            var schema = new TableSchema("no_key", null) { Columns = { Column("value", "integer") } };

            // Act
            var actual = new AuroraDbPostgreSqlSchemaComposer().ComposeCreateTable(schema);

            // Assert
            Assert.AreEqual($"CREATE TABLE \"no_key\" ({Environment.NewLine}    \"value\" integer NULL{Environment.NewLine});", actual);
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposeCreateTableIgnoresTheDefaultOfAnIdentityColumn()
        {
            // Setup
            var column = Column("id", "integer", isNullable: false, isIdentity: true);
            column.DefaultExpression = "nextval('x')";

            // Act
            var actual = new AuroraDbPostgreSqlSchemaComposer().ComposeCreateTable(new TableSchema("t", null) { Columns = { column } });

            // Assert
            StringAssert.Contains(actual, "\"id\" integer GENERATED BY DEFAULT AS IDENTITY (START WITH 1 INCREMENT BY 1) NOT NULL", StringComparison.Ordinal);
            Assert.IsFalse(actual.Contains("DEFAULT nextval"));
        }

        #endregion

        #region ComposeCreateIndex / ComposeAddForeignKey / others

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposeCreateIndex()
        {
            // Act
            var actual = new AuroraDbPostgreSqlSchemaComposer().ComposeCreateIndex("public.\"Person\"", GetSchema().Indexes[0]);

            // Assert
            Assert.AreEqual("CREATE UNIQUE INDEX \"ix_person_name\" ON \"public\".\"Person\" (\"Name\" DESC, \"Age\") INCLUDE (\"Double\") WHERE \"Age\" > 0;", actual);
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposeCreateIndexSimple()
        {
            // Act
            var actual = new AuroraDbPostgreSqlSchemaComposer().ComposeCreateIndex("t", new IndexInfo("ix") { Columns = { "a" } });

            // Assert
            Assert.AreEqual("CREATE INDEX \"ix\" ON \"t\" (\"a\");", actual);
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposeAddForeignKey()
        {
            // Act
            var actual = new AuroraDbPostgreSqlSchemaComposer().ComposeAddForeignKey("public.\"Person\"", GetSchema().ForeignKeys[0]);

            // Assert
            Assert.AreEqual("ALTER TABLE \"public\".\"Person\" ADD CONSTRAINT \"fk_person_country\" FOREIGN KEY (\"Id\") REFERENCES \"public\".\"country\" (\"id\") ON DELETE CASCADE ON UPDATE SET NULL;", actual);
        }

        [TestMethod]
        [DataRow(CopySchemaForeignKeyRule.NoAction, "")]
        [DataRow(CopySchemaForeignKeyRule.Restrict, " ON DELETE RESTRICT ON UPDATE RESTRICT")]
        [DataRow(CopySchemaForeignKeyRule.SetDefault, " ON DELETE SET DEFAULT ON UPDATE SET DEFAULT")]
        public void TestAuroraDbPostgreSqlSchemaComposerComposeAddForeignKeyRules(CopySchemaForeignKeyRule rule, string expected)
        {
            // Setup
            var foreignKey = new ForeignKeyInfo(null) { Columns = { "a" }, ReferencedTable = new TableInfo("b", null), ReferencedColumns = { "id" }, DeleteRule = rule, UpdateRule = rule };

            // Act
            var actual = new AuroraDbPostgreSqlSchemaComposer().ComposeAddForeignKey("t", foreignKey);

            // Assert
            Assert.AreEqual($"ALTER TABLE \"t\" ADD FOREIGN KEY (\"a\") REFERENCES \"b\" (\"id\"){expected};", actual);
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposeAddColumnAndDropTable()
        {
            // Act
            var composer = new AuroraDbPostgreSqlSchemaComposer();

            // Assert
            Assert.AreEqual("ALTER TABLE \"public\".\"Person\" ADD COLUMN \"Nick\" varchar(10) NULL;", composer.ComposeAddColumn("public.Person", Column("Nick", "varchar", size: 10)));
            Assert.AreEqual("DROP TABLE IF EXISTS \"public\".\"Person\";", composer.ComposeDropTable("public.Person"));
        }

        #endregion

        #region ComposeSchema / ComposeSchemas

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposeSchema()
        {
            // Act
            var actual = new AuroraDbPostgreSqlSchemaComposer().ComposeSchema(GetSchema()).ToList();

            // Assert
            Assert.AreEqual(3, actual.Count);
            StringAssert.StartsWith(actual[0], "CREATE TABLE ", StringComparison.Ordinal);
            StringAssert.StartsWith(actual[1], "CREATE UNIQUE INDEX ", StringComparison.Ordinal);
            StringAssert.StartsWith(actual[2], "ALTER TABLE ", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposeSchemasCreatesTheTablesThenTheIndexesThenTheForeignKeys()
        {
            // Setup
            var first = GetSchema();
            var second = GetSchema();
            second.Table.Name = "Other";

            // Act
            var actual = new AuroraDbPostgreSqlSchemaComposer().ComposeSchemas(new[] { first, second }).ToList();

            // Assert
            Assert.AreEqual(6, actual.Count);
            Assert.IsTrue(actual.Take(2).All(s => s.StartsWith("CREATE TABLE ", StringComparison.Ordinal)));
            Assert.IsTrue(actual.Skip(2).Take(2).All(s => s.StartsWith("CREATE UNIQUE INDEX ", StringComparison.Ordinal)));
            Assert.IsTrue(actual.Skip(4).All(s => s.StartsWith("ALTER TABLE ", StringComparison.Ordinal)));
            StringAssert.Contains(actual[0], "\"Person\"", StringComparison.Ordinal);
            StringAssert.Contains(actual[1], "\"Other\"", StringComparison.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbPostgreSqlSchemaComposerIfTheArgumentIsNull()
        {
            // Act/Assert
            var composer = new AuroraDbPostgreSqlSchemaComposer();
            Assert.Throws<ArgumentNullException>(() => composer.ComposeSchema(null));
            Assert.Throws<ArgumentNullException>(() => composer.ComposeSchemas(null));
            Assert.Throws<ArgumentNullException>(() => composer.ComposeCreateTable(null));
            Assert.Throws<ArgumentNullException>(() => composer.ComposeCreateIndex("t", null));
            Assert.Throws<ArgumentNullException>(() => composer.ComposeAddForeignKey("t", null));
            Assert.Throws<ArgumentNullException>(() => composer.ComposeAddColumn("t", null));
        }

        #endregion

        #region More scenarios

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerIsASchemaComposer()
        {
            // Assert
            Assert.IsInstanceOfType<ISchemaComposer>(new AuroraDbPostgreSqlSchemaComposer());
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbPostgreSqlSchemaComposerComposeCreateTableIfTheTableHasNoColumns()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() => new AuroraDbPostgreSqlSchemaComposer().ComposeCreateTable(new TableSchema("Missing", null)));
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposeCreateTableWithoutSchemaName()
        {
            // Act
            var actual = new AuroraDbPostgreSqlSchemaComposer().ComposeCreateTable(new TableSchema("Person", null) { Columns = { Column("Id", "integer") } });

            // Assert
            StringAssert.StartsWith(actual, "CREATE TABLE \"Person\" (", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposeCreateTableOrdersTheColumnsByOrdinal()
        {
            // Setup
            var second = Column("Second", "integer");
            second.Ordinal = 2;
            var first = Column("First", "integer");
            first.Ordinal = 1;

            // Act
            var actual = new AuroraDbPostgreSqlSchemaComposer().ComposeCreateTable(new TableSchema("t", null) { Columns = { second, first } });

            // Assert
            Assert.IsTrue(actual.IndexOf("\"First\"", StringComparison.Ordinal) < actual.IndexOf("\"Second\"", StringComparison.Ordinal));
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposeCreateTableWithUnnamedPrimaryKey()
        {
            // Act
            var actual = new AuroraDbPostgreSqlSchemaComposer().ComposeCreateTable(new TableSchema("t", null)
            {
                Columns = { Column("Id", "integer") },
                PrimaryKey = new PrimaryKeyInfo(null) { Columns = { "Id" } }
            });

            // Assert
            StringAssert.Contains(actual, "    PRIMARY KEY (\"Id\")", StringComparison.Ordinal);
            Assert.IsFalse(actual.Contains("CONSTRAINT"));
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposeCreateTableWithCompositePrimaryKey()
        {
            // Act
            var actual = new AuroraDbPostgreSqlSchemaComposer().ComposeCreateTable(new TableSchema("t", null)
            {
                Columns = { Column("A", "integer"), Column("B", "integer") },
                PrimaryKey = new PrimaryKeyInfo("pk_t") { Columns = { "A", "B" } }
            });

            // Assert
            StringAssert.Contains(actual, "CONSTRAINT \"pk_t\" PRIMARY KEY (\"A\", \"B\")", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposeCreateTableIgnoresThePrimaryKeyWithoutColumns()
        {
            // Act
            var actual = new AuroraDbPostgreSqlSchemaComposer().ComposeCreateTable(new TableSchema("t", null)
            {
                Columns = { Column("A", "integer") },
                PrimaryKey = new PrimaryKeyInfo("pk_t")
            });

            // Assert
            Assert.IsFalse(actual.Contains("PRIMARY KEY"));
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposeCreateTableQuotesTheIdentifiers()
        {
            // Act
            var actual = new AuroraDbPostgreSqlSchemaComposer().ComposeCreateTable(new TableSchema("Order \"Details\"", "My Schema")
            {
                Columns = { Column("Unit Price", "integer") }
            });

            // Assert
            StringAssert.StartsWith(actual, "CREATE TABLE \"My Schema\".\"Order \"\"Details\"\"\" (", StringComparison.Ordinal);
            StringAssert.Contains(actual, "\"Unit Price\" integer NULL", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposeCreateTableDoesNotComposeTheCollation()
        {
            // Setup
            var column = Column("Name", "character varying", size: 10);
            column.Collation = "SQL_Latin1_General_CP1_CI_AS";

            // Act
            var actual = new AuroraDbPostgreSqlSchemaComposer().ComposeCreateTable(new TableSchema("t", null) { Columns = { column } });

            // Assert
            Assert.IsFalse(actual.Contains("COLLATE"));
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposeCreateTableWithTheTypesOfAnotherDatabaseEngine()
        {
            // Act
            var actual = new AuroraDbPostgreSqlSchemaComposer().ComposeCreateTable(new TableSchema("t", null)
            {
                Columns =
                {
                    Column("A", "nvarchar", size: 30),
                    Column("B", "bit"),
                    Column("C", "uniqueidentifier"),
                    Column("D", "datetime2", scale: 7),
                    Column("E", "decimal", precision: 9, scale: 2)
                }
            });

            // Assert
            StringAssert.Contains(actual, "\"A\" varchar(30) NULL", StringComparison.Ordinal);
            StringAssert.Contains(actual, "\"B\" boolean NULL", StringComparison.Ordinal);
            StringAssert.Contains(actual, "\"C\" uuid NULL", StringComparison.Ordinal);
            StringAssert.Contains(actual, "\"D\" timestamp(6) NULL", StringComparison.Ordinal);
            StringAssert.Contains(actual, "\"E\" numeric(9,2) NULL", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposeCreateIndexWithAllDescendingKeys()
        {
            // Act
            var actual = new AuroraDbPostgreSqlSchemaComposer().ComposeCreateIndex("t", new IndexInfo("ix") { Columns = { "a", "b" }, DescendingColumns = { "a", "b" } });

            // Assert
            Assert.AreEqual("CREATE INDEX \"ix\" ON \"t\" (\"a\" DESC, \"b\" DESC);", actual);
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposeCreateIndexIgnoresTheBlankFilterAndTheClusteredFlag()
        {
            // Act
            var actual = new AuroraDbPostgreSqlSchemaComposer().ComposeCreateIndex("t", new IndexInfo("ix") { Columns = { "a" }, Filter = " ", IsClustered = true });

            // Assert
            Assert.AreEqual("CREATE INDEX \"ix\" ON \"t\" (\"a\");", actual);
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposeCreateIndexWithTableNameThatNeedsQuoting()
        {
            // Act
            var actual = new AuroraDbPostgreSqlSchemaComposer().ComposeCreateIndex("public.\"Order Details\"", new IndexInfo("ix") { Columns = { "a" } });

            // Assert
            Assert.AreEqual("CREATE INDEX \"ix\" ON \"public\".\"Order Details\" (\"a\");", actual);
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposeAddForeignKeyWithCompositeColumns()
        {
            // Act
            var actual = new AuroraDbPostgreSqlSchemaComposer().ComposeAddForeignKey("t", new ForeignKeyInfo("fk")
            {
                Columns = { "a", "b" },
                ReferencedTable = new TableInfo("r", "public"),
                ReferencedColumns = { "x", "y" }
            });

            // Assert
            Assert.AreEqual("ALTER TABLE \"t\" ADD CONSTRAINT \"fk\" FOREIGN KEY (\"a\", \"b\") REFERENCES \"public\".\"r\" (\"x\", \"y\");", actual);
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposeAddForeignKeyWithNamesThatNeedQuoting()
        {
            // Act
            var actual = new AuroraDbPostgreSqlSchemaComposer().ComposeAddForeignKey("public.\"Odd Child\"", new ForeignKeyInfo(null)
            {
                Columns = { "a" },
                ReferencedTable = new TableInfo("Odd.Name", "public"),
                ReferencedColumns = { "id" }
            });

            // Assert
            Assert.AreEqual("ALTER TABLE \"public\".\"Odd Child\" ADD FOREIGN KEY (\"a\") REFERENCES \"public\".\"Odd.Name\" (\"id\");", actual);
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposeAddColumnWithDefault()
        {
            // Setup
            var column = Column("Active", "boolean", isNullable: false);
            column.DefaultExpression = "true";

            // Act
            var actual = new AuroraDbPostgreSqlSchemaComposer().ComposeAddColumn("t", column);

            // Assert
            Assert.AreEqual("ALTER TABLE \"t\" ADD COLUMN \"Active\" boolean NOT NULL DEFAULT true;", actual);
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposeDropTableWithQuotedName()
        {
            // Assert
            Assert.AreEqual("DROP TABLE IF EXISTS \"public\".\"Order \"\"Details\"\"\";", new AuroraDbPostgreSqlSchemaComposer().ComposeDropTable("public.\"Order \"\"Details\"\"\""));
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposeSchemaWithOnlyColumns()
        {
            // Act
            var actual = new AuroraDbPostgreSqlSchemaComposer().ComposeSchema(new TableSchema("t", null) { Columns = { Column("A", "integer") } }).ToList();

            // Assert
            Assert.AreEqual(1, actual.Count);
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposeSchemasWithoutTables()
        {
            // Assert
            Assert.AreEqual(0, new AuroraDbPostgreSqlSchemaComposer().ComposeSchemas(Enumerable.Empty<TableSchema>()).Count());
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposeSchemasOfOneTableIsTheSameAsComposeSchema()
        {
            // Act
            var composer = new AuroraDbPostgreSqlSchemaComposer();
            var schema = GetSchema();

            // Assert
            CollectionAssert.AreEqual(composer.ComposeSchema(schema).ToList(), composer.ComposeSchemas(new[] { schema }).ToList());
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposeSchemasWithNamesThatNeedQuoting()
        {
            // Setup
            var schema = new TableSchema("Odd Child", "public") { Columns = { Column("a", "integer") } };
            schema.ForeignKeys.Add(new ForeignKeyInfo(null) { Columns = { "a" }, ReferencedTable = new TableInfo("Odd.Name", "public"), ReferencedColumns = { "id" } });
            var parent = new TableSchema("Odd.Name", "public") { Columns = { Column("id", "integer") } };

            // Act
            var actual = new AuroraDbPostgreSqlSchemaComposer().ComposeSchemas(new[] { schema, parent }).ToList();

            // Assert
            Assert.AreEqual(3, actual.Count);
            StringAssert.StartsWith(actual[0], "CREATE TABLE \"public\".\"Odd Child\"", StringComparison.Ordinal);
            StringAssert.StartsWith(actual[1], "CREATE TABLE \"public\".\"Odd.Name\"", StringComparison.Ordinal);
            StringAssert.Contains(actual[2], "REFERENCES \"public\".\"Odd.Name\"", StringComparison.Ordinal);
        }

        [TestMethod]
        [DataRow("INTEGER", "integer")]
        [DataRow("Character Varying", "varchar")]
        [DataRow("BOOLEAN", "boolean")]
        public void TestAuroraDbPostgreSqlSchemaComposerComposeTypeNameIsCaseInsensitive(string databaseType, string expected)
        {
            // Assert
            Assert.AreEqual(expected, TypeName(Column("c", databaseType)));
        }

        [TestMethod]
        [DataRow("bigint")]
        [DataRow("smallint")]
        [DataRow("real")]
        [DataRow("double precision")]
        [DataRow("boolean")]
        [DataRow("text")]
        [DataRow("bytea")]
        [DataRow("uuid")]
        [DataRow("date")]
        [DataRow("jsonb")]
        public void TestAuroraDbPostgreSqlSchemaComposerComposeTypeNameForSimpleTypes(string type)
        {
            // Assert
            Assert.AreEqual(type, TypeName(Column("c", type, size: 8, precision: 5, scale: 2)));
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposeTypeNameForVarCharWithoutSizeIsUnlimited()
        {
            // Assert
            Assert.AreEqual("varchar", TypeName(Column("c", "character varying", size: 0)));
            Assert.AreEqual("varchar", TypeName(Column("c", "nvarchar", size: -1)));
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposeTypeNameForTemporalTypesWithoutScale()
        {
            // Assert
            Assert.AreEqual("timestamp", TypeName(Column("c", "timestamp without time zone")));
            Assert.AreEqual("timestamptz", TypeName(Column("c", "timestamp with time zone")));
            Assert.AreEqual("time", TypeName(Column("c", "time without time zone")));
            Assert.AreEqual("timetz", TypeName(Column("c", "time with time zone")));
        }

        #endregion

        #region ComposeName / Exists

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposeName()
        {
            // Act/Assert
            var composer = new AuroraDbPostgreSqlSchemaComposer();
            Assert.AreEqual("public.person", composer.ComposeName(new TableInfo("person", "public")));
            Assert.AreEqual("public.\"Person\"", composer.ComposeName(new TableInfo("Person", "public")));
            Assert.AreEqual("public.\"Odd.Name\"", composer.ComposeName(new TableInfo("Odd.Name", "public")));
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposeNameWithoutSchema()
        {
            // Act/Assert
            Assert.AreEqual("person", new AuroraDbPostgreSqlSchemaComposer().ComposeName(new TableInfo("person", null)));
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbPostgreSqlSchemaComposerComposeNameIfTheTableIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new AuroraDbPostgreSqlSchemaComposer().ComposeName(null));
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposeTableExists()
        {
            // Act/Assert
            Assert.AreEqual("SELECT CASE WHEN EXISTS (SELECT 1 FROM pg_class WHERE oid = to_regclass('\"public\".\"Person\"') AND relkind IN ('r', 'p')) THEN 1 ELSE 0 END;", new AuroraDbPostgreSqlSchemaComposer().ComposeTableExists("public.Person"));
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposeColumnExists()
        {
            // Act/Assert
            Assert.AreEqual("SELECT CASE WHEN EXISTS (SELECT 1 FROM pg_attribute WHERE attrelid = to_regclass('\"public\".\"Person\"') AND attname = 'Name' AND attnum > 0 AND NOT attisdropped) THEN 1 ELSE 0 END;", new AuroraDbPostgreSqlSchemaComposer().ComposeColumnExists("public.Person", "Name"));
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposeIndexExists()
        {
            // Act/Assert
            Assert.AreEqual("SELECT CASE WHEN EXISTS (SELECT 1 FROM pg_index i INNER JOIN pg_class ic ON ic.oid = i.indexrelid WHERE i.indrelid = to_regclass('\"public\".\"Person\"') AND ic.relname = 'IX_Person_Name') THEN 1 ELSE 0 END;", new AuroraDbPostgreSqlSchemaComposer().ComposeIndexExists("public.Person", "IX_Person_Name"));
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposeExistsEscapesTheSingleQuotes()
        {
            // Act
            var composer = new AuroraDbPostgreSqlSchemaComposer();

            // Assert
            StringAssert.Contains(composer.ComposeColumnExists("public.Person", "O'Brien"), "O''Brien", StringComparison.Ordinal);
            StringAssert.Contains(composer.ComposeIndexExists("public.Person", "IX_O'Brien"), "IX_O''Brien", StringComparison.Ordinal);
            StringAssert.Contains(composer.ComposeTableExists("public.\"O'Brien\""), "O''Brien", StringComparison.Ordinal);
        }

        #endregion
    }
}
