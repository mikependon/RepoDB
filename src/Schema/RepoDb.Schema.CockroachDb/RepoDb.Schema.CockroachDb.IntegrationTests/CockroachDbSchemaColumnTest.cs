#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.CockroachDb;
using RepoDb.Schema.Models;
using RepoDb.Schema.CockroachDb.IntegrationTests.Setup;

namespace RepoDb.Schema.CockroachDb.IntegrationTests
{
    [TestClass]
    public class CockroachDbSchemaColumnTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup() =>
            Database.Cleanup();

        #region Helpers

        private static CockroachDbSchemaReader CreateReader() =>
            new CockroachDbSchemaReader(new CockroachDbConnection(Database.ConnectionStringForSource));

        private static ColumnInfo Column(string name) =>
            CreateReader().GetColumns("all_types").Single(c => c.Field.Name == name);

        #endregion

        #region Reader

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
        public void TestCockroachDbSchemaReaderGetColumnsOfTheTypes(string name, string databaseType, Type type)
        {
            // Act
            var column = Column(name);

            // Assert
            Assert.AreEqual(databaseType, column.Field.DatabaseType);
            Assert.AreEqual(type, column.Field.Type);
        }

        [TestMethod]
        public void TestCockroachDbSchemaReaderGetColumnsOfStringColumns()
        {
            // Assert
            Assert.AreEqual(5, Column("c_char").Field.Size);
            Assert.AreEqual(20, Column("c_varchar").Field.Size);
            Assert.AreEqual(0, Column("c_varchar_free").Field.Size);
            Assert.AreEqual(0, Column("c_text").Field.Size);
        }

        [TestMethod]
        public void TestCockroachDbSchemaReaderGetColumnsOfNumericColumns()
        {
            // Act
            var numeric = Column("c_numeric");
            var free = Column("c_numeric_free");

            // Assert
            Assert.AreEqual(10, (int)numeric.Field.Precision);
            Assert.AreEqual(3, (int)numeric.Field.Scale);
            Assert.AreEqual(0, (int)free.Field.Precision);
            Assert.AreEqual(0, (int)free.Field.Scale);
        }

        [TestMethod]
        public void TestCockroachDbSchemaReaderGetColumnsOfTemporalColumns()
        {
            // Assert
            Assert.AreEqual(2, (int)Column("c_time").Field.Scale);
            Assert.AreEqual(4, (int)Column("c_timestamp").Field.Scale);
            Assert.AreEqual(6, (int)Column("c_timestamp_free").Field.Scale);
            Assert.AreEqual(3, (int)Column("c_timestamptz").Field.Scale);
            Assert.AreEqual(0, (int)Column("c_date").Field.Scale);
        }

        [TestMethod]
        public void TestCockroachDbSchemaReaderGetColumnsOfNullability()
        {
            // Assert
            Assert.IsFalse(Column("c_smallint").Field.IsNullable);
            Assert.IsFalse(Column("c_integer").Field.IsNullable);
            Assert.IsTrue(Column("c_bigint").Field.IsNullable);
        }

        [TestMethod]
        public void TestCockroachDbSchemaReaderGetColumnsOfNonIdentityColumn()
        {
            // Act
            var column = Column("c_integer");

            // Assert
            Assert.IsFalse(column.Field.IsIdentity);
            Assert.IsFalse(column.Field.IsPrimary);
            Assert.IsNull(column.IdentitySeed);
            Assert.IsNull(column.IdentityIncrement);
            Assert.IsNull(column.DefaultExpression);
            Assert.IsNull(column.ComputedExpression);
        }

        [TestMethod]
        public void TestCockroachDbSchemaReaderGetColumnsAreInTheOrdinalOrder()
        {
            // Act
            var columns = CreateReader().GetColumns("all_types").ToList();

            // Assert
            CollectionAssert.AreEqual(columns.Select(c => c.Ordinal).OrderBy(x => x).ToList(), columns.Select(c => c.Ordinal).ToList());
            Assert.AreEqual("c_smallint", columns[0].Field.Name);
            Assert.AreEqual("c_jsonb", columns.Last().Field.Name);
        }

        [TestMethod]
        public void TestCockroachDbSchemaReaderGetColumnsOfColumnWithComment()
        {
            // Setup
            using (var connection = new CockroachDbConnection(Database.ConnectionStringForSource))
            {
                connection.ExecuteNonQuery("COMMENT ON COLUMN product.name IS 'The name of the product';");
            }

            // Act
            var column = CreateReader().GetColumns("product").Single(c => c.Field.Name == "name");

            // Assert
            Assert.AreEqual("The name of the product", column.Comment);
        }

        [TestMethod]
        public void TestCockroachDbSchemaReaderGetColumnsOfColumnWithDefault()
        {
            // Act
            var column = CreateReader().GetColumns("product").Single(c => c.Field.Name == "active");

            // Assert
            Assert.AreEqual("true", column.DefaultExpression);
            Assert.IsTrue(column.Field.HasDefaultValue);
        }

        [TestMethod]
        public void TestCockroachDbSchemaReaderGetColumnsOfCompositePrimaryKeyTable()
        {
            // Act
            var columns = CreateReader().GetColumns("region_code").ToList();

            // Assert
            Assert.IsTrue(columns.All(c => c.Field.IsPrimary));
            Assert.AreEqual(2, CreateReader().GetPrimaryKey("region_code").Columns.Count);
        }

        [TestMethod]
        public void TestCockroachDbSchemaReaderGetColumnsOfMissingTable()
        {
            // Assert
            Assert.AreEqual(0, CreateReader().GetColumns("Missing").Count());
            Assert.AreEqual(0, CreateReader().GetColumns("public.Missing").Count());
        }

        [TestMethod]
        public void TestCockroachDbSchemaReaderTableExistsWithTheTableOfAnotherSchema()
        {
            // Act
            var reader = CreateReader();

            // Assert
            Assert.IsTrue(reader.TableExists("sales.order_line"));
            Assert.IsFalse(reader.TableExists("public.order_line"));
            Assert.IsTrue(reader.TableExists("order_line"));
        }

        [TestMethod]
        public void TestCockroachDbSchemaReaderTableExistsWithQuotedName()
        {
            // Act
            var reader = CreateReader();

            // Assert
            Assert.IsTrue(reader.TableExists("\"Person\""));
            Assert.IsTrue(reader.TableExists("\"public\".\"Person\""));
            Assert.IsTrue(reader.TableExists("\"odd.name\""));
            Assert.IsFalse(reader.TableExists("\"person\""));
        }

        [TestMethod]
        public void TestCockroachDbSchemaReaderTableExistsDoesNotMatchTheViews()
        {
            // Setup
            using (var connection = new CockroachDbConnection(Database.ConnectionStringForSource))
            {
                connection.ExecuteNonQuery("CREATE OR REPLACE VIEW country_view AS SELECT * FROM country;");
            }

            // Assert
            Assert.IsFalse(CreateReader().TableExists("country_view"));
            Assert.IsFalse(CreateReader().GetTables().Contains("public.country_view"));
        }

        [TestMethod]
        public void TestCockroachDbSchemaReaderGetSchemaNameOfMissingTableIsTheDefaultSchema()
        {
            // Assert
            Assert.AreEqual("public", CreateReader().GetSchemaName("Missing"));
            Assert.AreEqual("sales", CreateReader().GetSchemaName("order_line"));
            Assert.AreEqual("sales", CreateReader().GetSchemaName("sales.anything"));
        }

        [TestMethod]
        public void TestCockroachDbSchemaReaderGetTablesOfMissingSchema()
        {
            // Assert
            Assert.AreEqual(0, CreateReader().GetTables("missing").Count());
        }

        [TestMethod]
        public void TestCockroachDbSchemaReaderGetUniqueConstraintsOfTableWithoutUniqueConstraints()
        {
            // Assert
            Assert.AreEqual(0, CreateReader().GetUniqueConstraints("Person").Count());
            Assert.AreEqual(0, CreateReader().GetCheckConstraints("country").Count());
        }

        [TestMethod]
        public void TestCockroachDbSchemaReaderGetTableSchemaOfTableWithoutKey()
        {
            // Act
            var schema = CreateReader().GetTableSchema("no_key");

            // Assert
            Assert.IsNull(schema.PrimaryKey);
            Assert.AreEqual(1, schema.Columns.Count);
            Assert.AreEqual(0, schema.Indexes.Count);
            Assert.AreEqual(0, schema.ForeignKeys.Count);
        }

        [TestMethod]
        public void TestCockroachDbSchemaReaderWithTransaction()
        {
            // Setup
            using (var connection = new CockroachDbConnection(Database.ConnectionStringForSource).EnsureOpen())
            using (var transaction = Helper.BeginDdlTransaction(connection))
            {
                connection.ExecuteNonQuery("CREATE TABLE in_transaction (id integer);", transaction: transaction);

                // Act/Assert
                Assert.IsTrue(new CockroachDbSchemaReader(connection, transaction).TableExists("in_transaction"));
                Assert.IsFalse(CreateReader().TableExists("in_transaction"));
                transaction.Rollback();
            }
        }

        [TestMethod]
        public async Task TestCockroachDbSchemaReaderAsyncMembersAreTheSameAsTheSyncOnes()
        {
            // Act
            var reader = CreateReader();
            var table = "product";

            // Assert
            Assert.AreEqual(reader.GetColumns(table).Count(), (await reader.GetColumnsAsync(table)).Count());
            Assert.AreEqual(reader.GetPrimaryKey(table).Name, (await reader.GetPrimaryKeyAsync(table)).Name);
            Assert.AreEqual(reader.GetIndexes(table).Count(), (await reader.GetIndexesAsync(table)).Count());
            Assert.AreEqual(reader.GetUniqueConstraints("country").Count(), (await reader.GetUniqueConstraintsAsync("country")).Count());
            Assert.AreEqual(reader.GetCheckConstraints("Person").Count(), (await reader.GetCheckConstraintsAsync("Person")).Count());
            Assert.AreEqual(reader.GetTables().Count(), (await reader.GetTablesAsync()).Count());
            Assert.IsFalse(await reader.TableExistsAsync("Missing"));
            Helper.AssertSchemaEquality(reader.GetTableSchema("Person"), await reader.GetTableSchemaAsync("Person"));
        }

        #endregion

        #region Composer

        [TestMethod]
        public void TestCockroachDbSchemaComposerComposedSchemaOfTableWithAllTheTypes()
        {
            // Act
            Helper.CopyToTarget("all_types");

            // Assert
            Helper.AssertTargetMatchesSource("all_types");
        }

        [TestMethod]
        public void TestCockroachDbSchemaComposerComposedSchemaOfTableWithIdentityAndKey()
        {
            // Act
            Helper.CopyToTarget("country", "Person");

            // Assert
            var id = Helper.GetTargetSchema("Person").Columns[0];
            Assert.IsTrue(id.Field.IsIdentity);
            Assert.AreEqual(10, id.IdentitySeed);
            Assert.AreEqual(5, id.IdentityIncrement);
        }

        [TestMethod]
        public void TestCockroachDbSchemaComposerComposedSerialColumnGeneratesTheValues()
        {
            // Setup
            Helper.CopyToTarget("ticket");
            using (var connection = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Act
                connection.ExecuteNonQuery("INSERT INTO ticket (note) VALUES ('a'), ('b');");

                // Assert
                Assert.AreEqual(2L, connection.ExecuteScalar<long>("SELECT COUNT(DISTINCT id) FROM ticket;"));
            }
        }

        [TestMethod]
        public void TestCockroachDbSchemaComposerComposedIdentityKeepsTheSeedAndIncrement()
        {
            // Setup
            Helper.CopyToTarget("country", "Person");
            using (var connection = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Act
                connection.ExecuteNonQuery("INSERT INTO \"Person\" (\"Name\") VALUES ('a'), ('b');");

                // Assert
                Assert.AreEqual(15L, connection.ExecuteScalar<long>("SELECT MAX(\"Id\") FROM \"Person\";"));
            }
        }

        [TestMethod]
        public void TestCockroachDbSchemaComposerComposedSchemaKeepsTheComputedColumn()
        {
            // Setup
            Helper.CopyToTarget("country", "Person");
            using (var connection = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Act
                connection.ExecuteNonQuery("INSERT INTO \"Person\" (\"Name\", \"Age\") VALUES ('a', 21);");

                // Assert
                Assert.AreEqual(42, connection.ExecuteScalar<int>("SELECT \"Double\" FROM \"Person\";"));
            }
        }

        [TestMethod]
        public void TestCockroachDbSchemaComposerComposedSchemaKeepsTheCheckConstraint()
        {
            // Setup
            Helper.CopyToTarget("country", "Person");
            using (var connection = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                Assert.Throws<CockroachDbException>(() => connection.ExecuteNonQuery("INSERT INTO \"Person\" (\"Name\", \"Age\") VALUES ('a', -1);"));
            }
        }

        [TestMethod]
        public void TestCockroachDbSchemaComposerComposedSchemaKeepsTheDefaultValues()
        {
            // Setup
            Helper.CopyToTarget("product");
            using (var connection = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Act
                connection.ExecuteNonQuery("INSERT INTO product (id, code) VALUES (1, 'A');");

                // Assert
                Assert.IsTrue(connection.ExecuteScalar<bool>("SELECT active FROM product;"));
            }
        }

        [TestMethod]
        public void TestCockroachDbSchemaComposerComposedSchemaOfTableWithoutKey()
        {
            // Act
            Helper.CopyToTarget("no_key");

            // Assert
            Helper.AssertTargetMatchesSource("no_key");
        }

        [TestMethod]
        public void TestCockroachDbSchemaComposerComposedSchemaOfTableInAnotherSchema()
        {
            // Act
            Helper.CopyToTarget("country", "Person", "sales.order_line");

            // Assert
            Assert.AreEqual("sales", Helper.GetTargetSchema("order_line").Table.Schema);
            Helper.AssertTargetMatchesSource("sales.order_line");
        }

        [TestMethod]
        public void TestCockroachDbSchemaComposerComposedDropTable()
        {
            // Setup
            Helper.CopyToTarget("country");

            // Act
            Helper.ExecuteOnTarget(new[] { new CockroachDbSchemaComposer().ComposeDropTable("country") });

            // Assert
            Assert.IsFalse(Helper.TargetTableExists("country"));
        }

        [TestMethod]
        public void TestCockroachDbSchemaComposerComposedDropTableOfMissingTable()
        {
            // Act
            Helper.ExecuteOnTarget(new[] { new CockroachDbSchemaComposer().ComposeDropTable("Missing") });

            // Assert
            Assert.IsFalse(Helper.TargetTableExists("Missing"));
        }

        [TestMethod]
        public void TestCockroachDbSchemaComposerComposedAddColumn()
        {
            // Setup
            Helper.CopyToTarget("country");
            var column = new ColumnInfo { Ordinal = 3, Field = new DbField("nick", false, false, true, typeof(string), 10, 0, 0, "character varying") };

            // Act
            Helper.ExecuteOnTarget(new[] { new CockroachDbSchemaComposer().ComposeAddColumn("country", column) });

            // Assert
            var added = Helper.GetTargetSchema("country").Columns.Last();
            Assert.AreEqual("nick", added.Field.Name);
            Assert.AreEqual(10, added.Field.Size);
        }

        #endregion
    }
}
