#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using RepoDb.Schema.Enumerations;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Connector.AuroraDb.Npgsql;
using RepoDb.Schema.Models;
using RepoDb.Schema.AuroraDbPostgreSql.IntegrationTests.Setup;

namespace RepoDb.Schema.AuroraDbPostgreSql.IntegrationTests
{
    /// <summary>
    /// Contains the aurora db postgre sql schema column test behavior.
    /// </summary>
    [TestClass]
    public class AuroraDbPostgreSqlSchemaColumnTest
    {
        /// <summary>
        /// Performs initialize.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();
        }

        /// <summary>
        /// Performs cleanup.
        /// </summary>
        [TestCleanup]
        public void Cleanup() => Database.Cleanup();
        #region Private Methods

        /// <summary>
        /// Performs create reader.
        /// </summary>
        private static AuroraDbPostgreSqlSchemaReader CreateReader() => new AuroraDbPostgreSqlSchemaReader();

        /// <summary>
        /// Performs column.
        /// </summary>
        /// <param name="name">The name.</param>
        private static ColumnInfo Column(string name)
        {
            using var operationConnection = new AuroraDbConnection(Database.ConnectionStringForSource);
            return CreateReader().GetColumns(operationConnection,
                "all_types").Single(c => c.Field.Name == name);
        }

        #endregion

        #region Reader

        /// <summary>
        /// Verifies aurora db postgre sql schema reader get columns of the types.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="databaseType">The database type.</param>
        /// <param name="type">The type.</param>
        [TestMethod]
        [DataRow("c_smallint", "smallint", typeof(short))]
        [DataRow("c_integer", "integer", typeof(int))]
        [DataRow("c_bigint", "bigint", typeof(long))]
        [DataRow("c_real", "real", typeof(float))]
        [DataRow("c_double", "double precision", typeof(double))]
        [DataRow("c_numeric", "numeric", typeof(decimal))]
        [DataRow("c_boolean", "boolean", typeof(bool))]
        [DataRow("c_char", "character", typeof(string))]
        [DataRow("c_varchar", "character varying", typeof(string))]
        [DataRow("c_text", "text", typeof(string))]
        [DataRow("c_bytea", "bytea", typeof(byte[]))]
        [DataRow("c_uuid", "uuid", typeof(Guid))]
        [DataRow("c_date", "date", typeof(DateOnly))]
        [DataRow("c_timestamp", "timestamp without time zone", typeof(DateTime))]
        public void TestAuroraDbPostgreSqlSchemaReaderGetColumnsOfTheTypes(string name,
            string databaseType,
            Type type)
        {
            // Act
            var column = Column(name);
            // Assert
            Assert.AreEqual(databaseType,
                column.Field.DatabaseType);
            Assert.AreEqual(type,
                column.Field.Type);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema reader get columns of string columns.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderGetColumnsOfStringColumns()
        {
            // Assert
            Assert.AreEqual(5,
                Column("c_char").Field.Size);
            Assert.AreEqual(20,
                Column("c_varchar").Field.Size);
            Assert.AreEqual(0,
                Column("c_varchar_free").Field.Size);
            Assert.AreEqual(0,
                Column("c_text").Field.Size);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema reader get columns of numeric columns.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderGetColumnsOfNumericColumns()
        {
            // Act
            var numeric = Column("c_numeric");
            var free = Column("c_numeric_free");
            // Assert
            Assert.AreEqual(10,
                (int)numeric.Field.Precision);
            Assert.AreEqual(3,
                (int)numeric.Field.Scale);
            Assert.AreEqual(0,
                (int)free.Field.Precision);
            Assert.AreEqual(0,
                (int)free.Field.Scale);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema reader get columns of temporal columns.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderGetColumnsOfTemporalColumns()
        {
            // Assert
            Assert.AreEqual(2,
                (int)Column("c_time").Field.Scale);
            Assert.AreEqual(4,
                (int)Column("c_timestamp").Field.Scale);
            Assert.AreEqual(6,
                (int)Column("c_timestamp_free").Field.Scale);
            Assert.AreEqual(3,
                (int)Column("c_timestamptz").Field.Scale);
            Assert.AreEqual(0,
                (int)Column("c_date").Field.Scale);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema reader get columns of nullability.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderGetColumnsOfNullability()
        {
            // Assert
            Assert.IsFalse(Column("c_smallint").Field.IsNullable);
            Assert.IsFalse(Column("c_integer").Field.IsNullable);
            Assert.IsTrue(Column("c_bigint").Field.IsNullable);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema reader get columns of non identity column.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderGetColumnsOfNonIdentityColumn()
        {
            // Act
            var column = Column("c_integer");
            // Assert
            Assert.IsFalse(column.Field.IsIdentity);
            Assert.IsFalse(column.Field.IsPrimary);
            Assert.IsNull(column.IdentitySeed);
            Assert.IsNull(column.IdentityIncrement);
            Assert.IsNull(column.DefaultExpression);
            Assert.AreEqual(ColumnDefaultValue.None,
                column.DefaultValue);
            Assert.IsNull(column.ComputedExpression);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema reader get columns are in the ordinal order.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderGetColumnsAreInTheOrdinalOrder()
        {
            using var operationConnection = new AuroraDbConnection(Database.ConnectionStringForSource); // Act
            var columns = CreateReader().GetColumns(operationConnection,
                "all_types").ToList();
            // Assert
            CollectionAssert.AreEqual(columns.Select(c => c.Ordinal).OrderBy(x => x).ToList(),
                columns.Select(c => c.Ordinal).ToList());
            Assert.AreEqual("c_smallint",
                columns[0].Field.Name);
            Assert.AreEqual("c_jsonb",
                columns.Last().Field.Name);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema reader get columns of column with comment.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderGetColumnsOfColumnWithComment()
        {
            using var operationConnection = new AuroraDbConnection(Database.ConnectionStringForSource); // Setup
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                connection.ExecuteNonQuery("COMMENT ON COLUMN product.name IS 'The name of the product';");
            }

            // Act
            var column = CreateReader().GetColumns(operationConnection,
                "product").Single(c => c.Field.Name == "name");
            // Assert
            Assert.AreEqual("The name of the product",
                column.Comment);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema reader get columns of column with default.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderGetColumnsOfColumnWithDefault()
        {
            using var operationConnection = new AuroraDbConnection(Database.ConnectionStringForSource); // Act
            var column = CreateReader().GetColumns(operationConnection,
                "product").Single(c => c.Field.Name == "active");
            // Assert
            Assert.AreEqual("true",
                column.DefaultExpression);
            Assert.AreEqual(ColumnDefaultValue.BooleanTrue,
                column.DefaultValue);
            Assert.IsTrue(column.Field.HasDefaultValue);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema reader get columns of composite primary key table.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderGetColumnsOfCompositePrimaryKeyTable()
        {
            using var operationConnection = new AuroraDbConnection(Database.ConnectionStringForSource); // Act
            var columns = CreateReader().GetColumns(operationConnection,
                "region_code").ToList();
            // Assert
            Assert.IsTrue(columns.All(c => c.Field.IsPrimary));
            Assert.AreEqual(2,
                CreateReader().GetPrimaryKey(operationConnection,
                    "region_code").Columns.Count);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema reader get columns of missing table.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderGetColumnsOfMissingTable()
        {
            using var operationConnection = new AuroraDbConnection(Database.ConnectionStringForSource); // Assert
            Assert.AreEqual(0,
                CreateReader().GetColumns(operationConnection,
                    "Missing").Count());
            Assert.AreEqual(0,
                CreateReader().GetColumns(operationConnection,
                    "public.Missing").Count());
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema reader table exists with the table of another schema.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderTableExistsWithTheTableOfAnotherSchema()
        {
            using var operationConnection = new AuroraDbConnection(Database.ConnectionStringForSource); // Act
            var reader = CreateReader();
            // Assert
            Assert.IsTrue(reader.TableExists(operationConnection,
                    "sales.order_line"));
            Assert.IsFalse(reader.TableExists(operationConnection,
                    "public.order_line"));
            Assert.IsTrue(reader.TableExists(operationConnection,
                    "order_line"));
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema reader table exists with quoted name.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderTableExistsWithQuotedName()
        {
            using var operationConnection = new AuroraDbConnection(Database.ConnectionStringForSource); // Act
            var reader = CreateReader();
            // Assert
            Assert.IsTrue(reader.TableExists(operationConnection,
                    "\"Person\""));
            Assert.IsTrue(reader.TableExists(operationConnection,
                    "\"public\".\"Person\""));
            Assert.IsTrue(reader.TableExists(operationConnection,
                    "\"odd.name\""));
            Assert.IsFalse(reader.TableExists(operationConnection,
                    "\"person\""));
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema reader table exists does not match the views.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderTableExistsDoesNotMatchTheViews()
        {
            using var operationConnection = new AuroraDbConnection(Database.ConnectionStringForSource); // Setup
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
            {
                connection.ExecuteNonQuery("CREATE OR REPLACE VIEW country_view AS SELECT * FROM country;");
            }

            // Assert
            Assert.IsFalse(CreateReader().TableExists(operationConnection,
                    "country_view"));
            Assert.IsFalse(CreateReader().GetTables(operationConnection).Contains("public.country_view"));
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema reader get schema name of missing table is the default schema.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderGetSchemaNameOfMissingTableIsTheDefaultSchema()
        {
            using var operationConnection = new AuroraDbConnection(Database.ConnectionStringForSource); // Assert
            Assert.AreEqual("public",
                CreateReader().GetSchemaName(operationConnection,
                    "Missing"));
            Assert.AreEqual("sales",
                CreateReader().GetSchemaName(operationConnection,
                    "order_line"));
            Assert.AreEqual("sales",
                CreateReader().GetSchemaName(operationConnection,
                    "sales.anything"));
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema reader get tables of missing schema.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderGetTablesOfMissingSchema()
        {
            using var operationConnection = new AuroraDbConnection(Database.ConnectionStringForSource); // Assert
            Assert.AreEqual(0,
                CreateReader().GetTables(operationConnection,
                    "missing").Count());
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema reader get unique constraints of table without unique constraints.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderGetUniqueConstraintsOfTableWithoutUniqueConstraints()
        {
            using var operationConnection = new AuroraDbConnection(Database.ConnectionStringForSource); // Assert
            Assert.AreEqual(0,
                CreateReader().GetUniqueConstraints(operationConnection,
                    "Person").Count());
            Assert.AreEqual(0,
                CreateReader().GetCheckConstraints(operationConnection,
                    "country").Count());
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema reader get table schema of table without key.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderGetTableSchemaOfTableWithoutKey()
        {
            using var operationConnection = new AuroraDbConnection(Database.ConnectionStringForSource); // Act
            var schema = CreateReader().GetTableSchema(operationConnection,
                "no_key");
            // Assert
            Assert.IsNull(schema.PrimaryKey);
            Assert.AreEqual(1,
                schema.Columns.Count);
            Assert.AreEqual(0,
                schema.Indexes.Count);
            Assert.AreEqual(0,
                schema.ForeignKeys.Count);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema reader with transaction.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaReaderWithTransaction()
        {
            using var operationConnection = new AuroraDbConnection(Database.ConnectionStringForSource); // Setup
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource).EnsureOpen())
            using (var transaction = connection.BeginTransaction())
            {
                connection.ExecuteNonQuery("CREATE TABLE in_transaction (id integer);",
                    transaction: transaction);
                // Act/Assert
                Assert.IsTrue(new AuroraDbPostgreSqlSchemaReader().TableExists(connection,
                        "in_transaction",
                        transaction: transaction));
                Assert.IsFalse(CreateReader().TableExists(operationConnection,
                        "in_transaction"));
                transaction.Rollback();
            }
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema reader async members are the same as the sync ones.
        /// </summary>
        [TestMethod]
        public async Task TestAuroraDbPostgreSqlSchemaReaderAsyncMembersAreTheSameAsTheSyncOnes()
        {
            using var operationConnection = new AuroraDbConnection(Database.ConnectionStringForSource); // Act
            var reader = CreateReader();
            var table = "product";
            // Assert
            Assert.AreEqual(reader.GetColumns(operationConnection,
                    table).Count(),
                (await reader.GetColumnsAsync(operationConnection,
                    table)).Count());
            Assert.AreEqual(reader.GetPrimaryKey(operationConnection,
                    table).Name,
                (await reader.GetPrimaryKeyAsync(operationConnection,
                    table)).Name);
            Assert.AreEqual(reader.GetIndexes(operationConnection,
                    table).Count(),
                (await reader.GetIndexesAsync(operationConnection,
                    table)).Count());
            Assert.AreEqual(reader.GetUniqueConstraints(operationConnection,
                    "country").Count(),
                (await reader.GetUniqueConstraintsAsync(operationConnection,
                    "country")).Count());
            Assert.AreEqual(reader.GetCheckConstraints(operationConnection,
                    "Person").Count(),
                (await reader.GetCheckConstraintsAsync(operationConnection,
                    "Person")).Count());
            Assert.AreEqual(reader.GetTables(operationConnection).Count(),
                (await reader.GetTablesAsync(operationConnection)).Count());
            Assert.IsFalse(await reader.TableExistsAsync(operationConnection,
                    "Missing"));
            Helper.AssertSchemaEquality(reader.GetTableSchema(operationConnection,
                    "Person"),
                await reader.GetTableSchemaAsync(operationConnection,
                    "Person"));
        }

        #endregion

        #region Composer

        /// <summary>
        /// Verifies aurora db postgre sql schema composer composed schema of table with all the types.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposedSchemaOfTableWithAllTheTypes()
        {
            // Act
            Helper.CopyToTarget("all_types");
            // Assert
            Helper.AssertTargetMatchesSource("all_types");
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema composer composed schema of table with identity and key.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposedSchemaOfTableWithIdentityAndKey()
        {
            // Act
            Helper.CopyToTarget("country",
                "Person");
            // Assert
            var id = Helper.GetTargetSchema("Person").Columns[0];
            Assert.IsTrue(id.Field.IsIdentity);
            Assert.AreEqual(10,
                id.IdentitySeed);
            Assert.AreEqual(5,
                id.IdentityIncrement);
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema composer composed serial column generates the values.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposedSerialColumnGeneratesTheValues()
        {
            // Setup
            Helper.CopyToTarget("ticket");
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Act
                connection.ExecuteNonQuery("INSERT INTO ticket (note) VALUES ('a'), ('b');");
                // Assert
                Assert.AreEqual(2L,
                    connection.ExecuteScalar<long>("SELECT MAX(id) FROM ticket;"));
            }
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema composer composed identity keeps the seed and increment.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposedIdentityKeepsTheSeedAndIncrement()
        {
            // Setup
            Helper.CopyToTarget("country",
                "Person");
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Act
                connection.ExecuteNonQuery("INSERT INTO \"Person\" (\"Name\") VALUES ('a'), ('b');");
                // Assert
                Assert.AreEqual(15L,
                    connection.ExecuteScalar<long>("SELECT MAX(\"Id\") FROM \"Person\";"));
            }
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema composer composed schema keeps the computed column.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposedSchemaKeepsTheComputedColumn()
        {
            // Setup
            Helper.CopyToTarget("country",
                "Person");
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Act
                connection.ExecuteNonQuery("INSERT INTO \"Person\" (\"Name\", \"Age\") VALUES ('a', 21);");
                // Assert
                Assert.AreEqual(42,
                    connection.ExecuteScalar<int>("SELECT \"Double\" FROM \"Person\";"));
            }
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema composer composed schema keeps the check constraint.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposedSchemaKeepsTheCheckConstraint()
        {
            // Setup
            Helper.CopyToTarget("country",
                "Person");
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                Assert.Throws<AuroraDbException>(() => connection.ExecuteNonQuery("INSERT INTO \"Person\" (\"Name\", \"Age\") VALUES ('a', -1);"));
            }
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema composer composed schema keeps the default values.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposedSchemaKeepsTheDefaultValues()
        {
            // Setup
            Helper.CopyToTarget("product");
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Act
                connection.ExecuteNonQuery("INSERT INTO product (id, code) VALUES (1, 'A');");
                // Assert
                Assert.IsTrue(connection.ExecuteScalar<bool>("SELECT active FROM product;"));
            }
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema composer composed schema of table without key.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposedSchemaOfTableWithoutKey()
        {
            // Act
            Helper.CopyToTarget("no_key");
            // Assert
            Helper.AssertTargetMatchesSource("no_key");
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema composer composed schema of table in another schema.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposedSchemaOfTableInAnotherSchema()
        {
            // Act
            Helper.CopyToTarget("country",
                "Person",
                "sales.order_line");
            // Assert
            Assert.AreEqual("sales",
                Helper.GetTargetSchema("order_line").Table.Schema);
            Helper.AssertTargetMatchesSource("sales.order_line");
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema composer composed drop table.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposedDropTable()
        {
            // Setup
            Helper.CopyToTarget("country");
            // Act
            Helper.ExecuteOnTarget(new[] { new AuroraDbPostgreSqlSchemaComposer().ComposeDropTable("country") });
            // Assert
            Assert.IsFalse(Helper.TargetTableExists("country"));
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema composer composed drop table of missing table.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposedDropTableOfMissingTable()
        {
            // Act
            Helper.ExecuteOnTarget(new[] { new AuroraDbPostgreSqlSchemaComposer().ComposeDropTable("Missing") });
            // Assert
            Assert.IsFalse(Helper.TargetTableExists("Missing"));
        }

        /// <summary>
        /// Verifies aurora db postgre sql schema composer composed add column.
        /// </summary>
        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaComposerComposedAddColumn()
        {
            // Setup
            Helper.CopyToTarget("country");
            var column = new ColumnInfo
            {
                Ordinal = 3,
                Field = new DbField("nick",
                    false,
                    false,
                    true,
                    typeof(string),
                    10,
                    0,
                    0,
                    "character varying")
            };
            // Act
            Helper.ExecuteOnTarget(new[] { new AuroraDbPostgreSqlSchemaComposer().ComposeAddColumn("country",
                    column) });
            // Assert
            var added = Helper.GetTargetSchema("country").Columns.Last();
            Assert.AreEqual("nick",
                added.Field.Name);
            Assert.AreEqual(10,
                added.Field.Size);
        }
        #endregion
    }
}
