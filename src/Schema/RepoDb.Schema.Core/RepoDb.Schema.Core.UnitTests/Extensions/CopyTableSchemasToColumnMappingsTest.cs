#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Schema;
using RepoDb.Schema.Core.UnitTests.CustomObjects;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests.Extensions
{
    /// <summary>
    /// Contains the copy table schemas to column mappings test behavior.
    /// </summary>
    [TestClass]
    public class CopyTableSchemasToColumnMappingsTest
    {
        /// <summary>
        /// Performs initialize.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            Cleanup();
        }

        /// <summary>
        /// Performs cleanup.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            SchemaReaderMapper.Clear();
            SchemaComposerMapper.Clear();
        }

        #region Private Methods

        /// <summary>
        /// Performs get person.
        /// </summary>
        private static TableSchema GetPerson()
        {
            var schema = new TableSchema("Person",
                "dbo");
            foreach (var column in new[]
            {
                "Id",
                "Name",
                "Age",
                "CountryId"
            }

            )
            {
                schema.Columns.Add(new ColumnInfo { Field = new DbField(column,
                        false,
                        false,
                        true,
                        typeof(string),
                        null,
                        null,
                        null,
                        "text",
                        false,
                        "Test") });
            }

            schema.PrimaryKey = new PrimaryKeyInfo("PK_Person")
            {
                Columns = new List<string>
                {
                    "Id"
                }
            };
            schema.Indexes.Add(new IndexInfo("IX_Person_Name") { Columns = new List<string> { "Name" }, IncludedColumns = new List<string> { "Age" }, DescendingColumns = new List<string> { "Name" } });
            schema.UniqueConstraints.Add(new UniqueConstraintInfo("UQ_Person_Name") { Columns = new List<string> { "Name" } });
            schema.CheckConstraints.Add(new CheckConstraintInfo("CK_Person_Age") { Expression = "([Age] >= 0)" });
            schema.ForeignKeys.Add(new ForeignKeyInfo("FK_Person_Country") { Columns = new List<string> { "CountryId" }, ReferencedTable = new TableInfo("Country",
                    "dbo"), ReferencedColumns = new List<string> { "Id" } });
            return schema;
        }

        /// <summary>
        /// Performs get country.
        /// </summary>
        private static TableSchema GetCountry()
        {
            var schema = new TableSchema("Country",
                "dbo");
            foreach (var column in new[]
            {
                "Id",
                "Name"
            }

            )
            {
                schema.Columns.Add(new ColumnInfo { Field = new DbField(column,
                        false,
                        false,
                        true,
                        typeof(string),
                        null,
                        null,
                        null,
                        "text",
                        false,
                        "Test") });
            }

            schema.PrimaryKey = new PrimaryKeyInfo("PK_Country")
            {
                Columns = new List<string>
                {
                    "Id"
                }
            };
            return schema;
        }

        /// <summary>
        /// Performs get pet.
        /// </summary>
        private static TableSchema GetPet()
        {
            var schema = new TableSchema("Pet",
                "dbo");
            foreach (var column in new[]
            {
                "Id",
                "PersonName"
            }

            )
            {
                schema.Columns.Add(new ColumnInfo { Field = new DbField(column,
                        false,
                        false,
                        true,
                        typeof(string),
                        null,
                        null,
                        null,
                        "text",
                        false,
                        "Test") });
            }

            schema.ForeignKeys.Add(new ForeignKeyInfo("FK_Pet_Person") { Columns = new List<string> { "PersonName" }, ReferencedTable = new TableInfo("Person",
                    "dbo"), ReferencedColumns = new List<string> { "Name" } });
            return schema;
        }

        /// <summary>
        /// Performs map reader.
        /// </summary>
        /// <param name="schemas">The schemas.</param>
        private static void MapReader(params TableSchema[] schemas)
        {
            var reader = new Mock<ISchemaReader>();
            reader.Setup(r => r.GetTableSchema(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<System.Data.IDbTransaction>())).Returns<System.Data.IDbConnection, string, System.Data.IDbTransaction>((operationConnection,
                    name,
                    operationTransaction) => schemas.Single(s => s.Table.Name == name));
            reader.Setup(r => r.GetTableSchemaAsync(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<System.Data.IDbTransaction>(),
                    It.IsAny<CancellationToken>())).Returns<System.Data.IDbConnection, string, System.Data.IDbTransaction, CancellationToken>((operationConnection,
                    name,
                    operationTransaction,
                    _) => Task.FromResult(schemas.Single(s => s.Table.Name == name)));
            reader.Setup(r => r.GetRelatedTables(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<RelatedTableBehavior>(),
                    It.IsAny<System.Data.IDbTransaction>())).Returns<System.Data.IDbConnection, IEnumerable<string>, RelatedTableBehavior, System.Data.IDbTransaction>((operationConnection,
                    names,
                    _,
                    operationTransaction) => schemas.Select(s => s.Table.Name).ToList());
            reader.Setup(r => r.GetRelatedTablesAsync(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<RelatedTableBehavior>(),
                    It.IsAny<System.Data.IDbTransaction>(),
                    It.IsAny<CancellationToken>())).Returns<System.Data.IDbConnection, IEnumerable<string>, RelatedTableBehavior, System.Data.IDbTransaction, CancellationToken>((operationConnection,
                    names,
                    _,
                    operationTransaction,
                    __) => Task.FromResult<IEnumerable<string>>(schemas.Select(s => s.Table.Name).ToList()));
            reader.Setup(r => r.GetDependencyOrder(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<System.Data.IDbTransaction>())).Returns<System.Data.IDbConnection, IEnumerable<string>, System.Data.IDbTransaction>((operationConnection,
                    names,
                    operationTransaction) => schemas.Where(s => names.Contains(s.Table.Name)).Select(s => new RelationshipInfo { Schema = s }).ToList());
            reader.Setup(r => r.GetDependencyOrderAsync(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<System.Data.IDbTransaction>(),
                    It.IsAny<CancellationToken>())).Returns<System.Data.IDbConnection, IEnumerable<string>, System.Data.IDbTransaction, CancellationToken>((operationConnection,
                    names,
                    operationTransaction,
                    _) => Task.FromResult<IEnumerable<RelationshipInfo>>(schemas.Where(s => names.Contains(s.Table.Name)).Select(s => new RelationshipInfo { Schema = s }).ToList()));
            SchemaReaderMapper.Add<CustomDbConnection>(reader.Object,
                true);
        }

        /// <summary>
        /// Performs describe.
        /// </summary>
        /// <param name="schema">The schema.</param>
        private static string Describe(TableSchema schema) => $"{schema.Table.Name}({string.Join(",",
            schema.Columns.Select(c => c.Field.Name))})" + $" PK({(schema.PrimaryKey == null ? string.Empty : string.Join(",",
            schema.PrimaryKey.Columns))})" + string.Concat(schema.Indexes.Select(i => $" IX[{string.Join(",",
                    i.Columns)}|{string.Join(",",
                    i.IncludedColumns)}|{string.Join(",",
                    i.DescendingColumns)}]")) + string.Concat(schema.UniqueConstraints.Select(u => $" UQ[{string.Join(",",
                    u.Columns)}]")) + string.Concat(schema.CheckConstraints.Select(c => $" CK[{c.Expression}]")) + string.Concat(schema.ForeignKeys.Select(f => $" FK[{string.Join(",",
                    f.Columns)}->{f.ReferencedTable.Name}({string.Join(",",
                    f.ReferencedColumns)})]"));

        /// <summary>
        /// Performs script.
        /// </summary>
        /// <param name="schemas">The schemas.</param>
        private static IEnumerable<string> Script(IEnumerable<TableSchema> schemas) => schemas.Select(s => $"CREATE {Describe(s)};");

        /// <summary>
        /// Performs map composer.
        /// </summary>
        private static void MapComposer()
        {
            var composer = new Mock<ISchemaComposer>();
            composer.Setup(c => c.ComposeName(It.IsAny<TableInfo>())).Returns<TableInfo>(t => t.Name);
            composer.Setup(c => c.ComposeSchemas(It.IsAny<IEnumerable<TableSchema>>())).Returns<IEnumerable<TableSchema>>(s => Script(s).ToList());
            composer.Setup(c => c.ComposeSchema(It.IsAny<TableSchema>())).Returns<TableSchema>(s => Script(new[] { s }).ToList());
            composer.Setup(c => c.ComposeDropTable(It.IsAny<string>())).Returns<string>(n => $"DROP TABLE {n};");
            composer.Setup(c => c.ComposeAddColumn(It.IsAny<string>(),
                    It.IsAny<ColumnInfo>())).Returns<string, ColumnInfo>((n,
                    c) => $"ADD COLUMN {c.Field.Name} TO {n};");
            composer.Setup(c => c.ComposeCreateIndex(It.IsAny<string>(),
                    It.IsAny<IndexInfo>())).Returns<string, IndexInfo>((n,
                    i) => $"CREATE INDEX {i.Name} ON {n};");
            composer.Setup(c => c.ComposeAddForeignKey(It.IsAny<string>(),
                    It.IsAny<ForeignKeyInfo>())).Returns<string, ForeignKeyInfo>((n,
                    f) => $"ADD {f.Name} TO {n};");
            composer.Setup(c => c.ComposeTableExists(It.IsAny<string>())).Returns<string>(n => $"TABLE_EXISTS {n}");
            composer.Setup(c => c.ComposeColumnExists(It.IsAny<string>(),
                    It.IsAny<string>())).Returns<string, string>((n,
                    c) => $"COLUMN_EXISTS {n}.{c}");
            composer.Setup(c => c.ComposeIndexExists(It.IsAny<string>(),
                    It.IsAny<string>())).Returns<string, string>((n,
                    i) => $"INDEX_EXISTS {n}.{i}");
            SchemaComposerMapper.Add<CustomDbConnection>(composer.Object,
                true);
        }

        /// <summary>
        /// Performs get destination.
        /// </summary>
        /// <param name="existing">The existing.</param>
        private static CustomDbConnection GetDestination(params string[] existing) => new CustomDbConnection
        {
            ScalarResult = statement => existing.Contains(statement) ? 1 : 0
        };

        /// <summary>
        /// Performs created.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="table">The table.</param>
        private static string Created(CustomDbConnection destination,
            string table) => destination.ExecutedCommands.First(c => c.StartsWith($"CREATE {table}("));
        #endregion

        #region Mappings

        /// <summary>
        /// Performs map.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        private static ColumnMapping Map(string source,
            string target = null) => new ColumnMapping(source,
            target);

        /// <summary>
        /// Performs all.
        /// </summary>
        /// <param name="mappings">The mappings.</param>
        private static IList<ColumnMapping> All(params ColumnMapping[] mappings) => mappings;

        /// <summary>
        /// Performs all columns.
        /// </summary>
        private static IList<ColumnMapping> AllColumns() => All(Map("Id",
                "PersonId"),
            Map("Name",
                "FullName"),
            Map("Age",
                "Years"),
            Map("CountryId",
                "Country_Id"));

        /// <summary>
        /// Verifies copy table schema column mapping constructor sets the source and the target.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaColumnMappingConstructorSetsTheSourceAndTheTarget()
        {
            // Act
            var actual = new ColumnMapping("Name",
                "FullName");
            // Assert
            Assert.AreEqual("Name",
                actual.Source);
            Assert.AreEqual("FullName",
                actual.Target);
        }

        /// <summary>
        /// Verifies copy table schema column mapping constructor keeps the target null by default.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaColumnMappingConstructorKeepsTheTargetNullByDefault()
        {
            // Act
            var actual = new ColumnMapping("Name");
            // Assert
            Assert.AreEqual("Name",
                actual.Source);
            Assert.IsNull(actual.Target);
        }

        /// <summary>
        /// Verifies copy table schema to with mappings creates only the mapped columns by their target names.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToWithMappingsCreatesOnlyTheMappedColumnsByTheirTargetNames()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            var destination = GetDestination();
            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person",
                destination,
                All(Map("Id"),
                    Map("Name",
                        "FullName")));
            // Assert
            Assert.AreEqual("CREATE Person(Id,FullName) PK(Id) UQ[FullName];",
                Created(destination,
                    "Person"));
        }

        /// <summary>
        /// Verifies copy table schema to with mappings makes the objects follow the target names.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToWithMappingsMakesTheObjectsFollowTheTargetNames()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Country");
            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person",
                destination,
                AllColumns());
            // Assert
            Assert.AreEqual("CREATE Person(PersonId,FullName,Years,Country_Id) PK(PersonId) IX[FullName|Years|FullName] UQ[FullName] CK[([Years] >= 0)] FK[Country_Id->Country(Id)];",
                Created(destination,
                    "Person"));
        }

        /// <summary>
        /// Verifies copy table schema to with mappings keeps the source name if the target is null or white space.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToWithMappingsKeepsTheSourceNameIfTheTargetIsNullOrWhiteSpace()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            var destination = GetDestination();
            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person",
                destination,
                All(Map("Id",
                        null),
                    Map("Name",
                        " "),
                    Map("Age",
                        "Years")));
            // Assert
            Assert.AreEqual("CREATE Person(Id,Name,Years) PK(Id) IX[Name|Years|Name] UQ[Name] CK[([Years] >= 0)];",
                Created(destination,
                    "Person"));
        }

        /// <summary>
        /// Verifies copy table schema to with mappings matches the source names without case.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToWithMappingsMatchesTheSourceNamesWithoutCase()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            var destination = GetDestination();
            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person",
                destination,
                All(Map("id"),
                    Map("NAME",
                        "FullName")));
            // Assert
            Assert.AreEqual("CREATE Person(Id,FullName) PK(Id) UQ[FullName];",
                Created(destination,
                    "Person"));
        }

        /// <summary>
        /// Verifies copy table schema to with mappings keeps the order of the columns of the source table.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToWithMappingsKeepsTheOrderOfTheColumnsOfTheSourceTable()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            var destination = GetDestination();
            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person",
                destination,
                All(Map("Name"),
                    Map("Id")));
            // Assert
            StringAssert.StartsWith(Created(destination,
                    "Person"),
                "CREATE Person(Id,Name)");
        }

        /// <summary>
        /// Verifies copy table schema to with mappings can swap the names of two columns.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToWithMappingsCanSwapTheNamesOfTwoColumns()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Country");
            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person",
                destination,
                All(Map("Id"),
                    Map("Name",
                        "Age"),
                    Map("Age",
                        "Name"),
                    Map("CountryId")));
            // Assert
            Assert.AreEqual("CREATE Person(Id,Age,Name,CountryId) PK(Id) IX[Age|Name|Age] UQ[Age] CK[([Name] >= 0)] FK[CountryId->Country(Id)];",
                Created(destination,
                    "Person"));
        }

        /// <summary>
        /// Verifies copy table schema to with mappings can use the name of acolumn that is not mapped.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToWithMappingsCanUseTheNameOfAColumnThatIsNotMapped()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            var destination = GetDestination();
            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person",
                destination,
                All(Map("Id"),
                    Map("Name",
                        "Age")));
            // Assert
            Assert.AreEqual("CREATE Person(Id,Age) PK(Id) UQ[Age];",
                Created(destination,
                    "Person"));
        }

        /// <summary>
        /// Verifies copy table schema to with mappings makes the foreign key of arelated table follow the target names.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToWithMappingsMakesTheForeignKeyOfARelatedTableFollowTheTargetNames()
        {
            // Setup
            MapReader(GetPerson(),
                GetPet());
            MapComposer();
            var destination = GetDestination();
            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person",
                destination,
                All(Map("Id"),
                    Map("Name",
                        "FullName")),
                relationshipBehavior: RelatedTableBehavior.Children);
            // Assert
            Assert.AreEqual("CREATE Pet(Id,PersonName) PK() FK[PersonName->Person(FullName)];",
                Created(destination,
                    "Pet"));
        }

        /// <summary>
        /// Verifies copy table schema to with mappings does not create the foreign key of arelated table that references acolumn that is not mapped.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToWithMappingsDoesNotCreateTheForeignKeyOfARelatedTableThatReferencesAColumnThatIsNotMapped()
        {
            // Setup
            MapReader(GetPerson(),
                GetPet());
            MapComposer();
            var destination = GetDestination();
            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person",
                destination,
                All(Map("Id"),
                    Map("Age")),
                relationshipBehavior: RelatedTableBehavior.Children);
            // Assert
            Assert.AreEqual("CREATE Pet(Id,PersonName) PK();",
                Created(destination,
                    "Pet"));
        }

        /// <summary>
        /// Verifies copy table schema to with mappings does not change the related tables.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToWithMappingsDoesNotChangeTheRelatedTables()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            var destination = GetDestination();
            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person",
                destination,
                All(Map("Id",
                        "PersonId"),
                    Map("Name",
                        "FullName"),
                    Map("CountryId")),
                relationshipBehavior: RelatedTableBehavior.Parents);
            // Assert
            Assert.AreEqual("CREATE Country(Id,Name) PK(Id);",
                Created(destination,
                    "Country"));
            StringAssert.Contains(Created(destination,
                    "Person"),
                "FK[CountryId->Country(Id)]");
        }

        /// <summary>
        /// Verifies copy table schema to with mappings does not change the schema of the source.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToWithMappingsDoesNotChangeTheSchemaOfTheSource()
        {
            // Setup
            var person = GetPerson();
            MapReader(person,
                GetCountry());
            MapComposer();
            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person",
                GetDestination(),
                All(Map("Name",
                        "FullName")));
            // Assert
            CollectionAssert.AreEqual(new[] { "Id", "Name", "Age", "CountryId" },
                person.Columns.Select(c => c.Field.Name).ToList());
            CollectionAssert.AreEqual(new[] { "Name" },
                person.Indexes.Single().Columns.ToList());
        }

        /// <summary>
        /// Verifies copy table schema to with mappings keeps the other properties of the columns.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToWithMappingsKeepsTheOtherPropertiesOfTheColumns()
        {
            // Setup
            var person = GetPerson();
            person.Columns[1] = new ColumnInfo
            {
                Field = new DbField("Name",
                    false,
                    false,
                    false,
                    typeof(string),
                    50,
                    null,
                    null,
                    "varchar",
                    true,
                    "Test"),
                Ordinal = 2,
                DefaultExpression = "'N/A'",
                DefaultValue = ColumnDefaultValue.Literal,
                Collation = "Latin1",
                Comment = "The name"
            };
            person.Table.Database = "Northwind";
            person.Table.Provider = "SqlServer";
            MapReader(person,
                GetCountry());
            var composed = new List<TableSchema>();
            var composer = new Mock<ISchemaComposer>();
            composer.Setup(c => c.ComposeName(It.IsAny<TableInfo>())).Returns<TableInfo>(t => t.Name);
            composer.Setup(c => c.ComposeSchemas(It.IsAny<IEnumerable<TableSchema>>())).Returns<IEnumerable<TableSchema>>(s =>
            {
                composed.AddRange(s);
                return new List<string>
                {
                    "CREATE;"
                };
            });
            composer.Setup(c => c.ComposeSchema(It.IsAny<TableSchema>())).Returns<TableSchema>(s =>
            {
                composed.Add(s);
                return new List<string>
                {
                    "CREATE;"
                };
            });
            SchemaComposerMapper.Add<CustomDbConnection>(composer.Object,
                true);
            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person",
                GetDestination(),
                All(Map("Name",
                        "FullName")));
            // Assert
            var column = composed.First().Columns.Single();
            Assert.AreEqual("FullName",
                column.Field.Name);
            Assert.AreEqual(2,
                column.Ordinal);
            Assert.AreEqual("'N/A'",
                column.DefaultExpression);
            Assert.AreEqual(ColumnDefaultValue.Literal,
                column.DefaultValue);
            Assert.AreEqual("Latin1",
                column.Collation);
            Assert.AreEqual("The name",
                column.Comment);
            Assert.AreEqual(50,
                column.Field.Size);
            Assert.AreEqual("varchar",
                column.Field.DatabaseType);
            Assert.IsFalse(column.Field.IsNullable);
            Assert.IsTrue(column.Field.HasDefaultValue);
            Assert.AreEqual("Test",
                column.Field.Provider);
            Assert.AreEqual("Northwind",
                composed.First().Table.Database);
            Assert.AreEqual("SqlServer",
                composed.First().Table.Provider);
        }

        /// <summary>
        /// Verifies copy table schema to of several tables with mappings applies the mappings to each table.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToOfSeveralTablesWithMappingsAppliesTheMappingsToEachTable()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            var destination = GetDestination();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" },
                destination,
                Tables(Table("Person",
                        Map("Id"),
                        Map("Name",
                            "FullName")),
                    Table("Country",
                        Map("Id"),
                        Map("Name",
                            "FullName"))));
            // Assert
            Assert.AreEqual("CREATE Person(Id,FullName) PK(Id) UQ[FullName];",
                Created(destination,
                    "Person"));
            Assert.AreEqual("CREATE Country(Id,FullName) PK(Id);",
                Created(destination,
                    "Country"));
        }

        /// <summary>
        /// Verifies copy table schema to of an entity with mappings creates only the mapped columns by their target names.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToOfAnEntityWithMappingsCreatesOnlyTheMappedColumnsByTheirTargetNames()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            var destination = GetDestination();
            // Act
            new CustomDbConnection().CopyTableSchemaTo<Person>(destination,
                All(Map("Id"),
                    Map("Name",
                        "FullName")));
            // Assert
            Assert.AreEqual("CREATE Person(Id,FullName) PK(Id) UQ[FullName];",
                Created(destination,
                    "Person"));
        }

        /// <summary>
        /// Verifies copy table schema to async with mappings creates only the mapped columns by their target names.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncWithMappingsCreatesOnlyTheMappedColumnsByTheirTargetNames()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            var destination = GetDestination();
            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync("Person",
                destination,
                All(Map("Id"),
                    Map("Name",
                        "FullName")));
            // Assert
            Assert.AreEqual("CREATE Person(Id,FullName) PK(Id) UQ[FullName];",
                Created(destination,
                    "Person"));
        }

        /// <summary>
        /// Verifies copy table schema to async of an entity with mappings creates only the mapped columns by their target names.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncOfAnEntityWithMappingsCreatesOnlyTheMappedColumnsByTheirTargetNames()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            var destination = GetDestination();
            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync<Person>(destination,
                All(Map("Id"),
                    Map("Name",
                        "FullName")));
            // Assert
            Assert.AreEqual("CREATE Person(Id,FullName) PK(Id) UQ[FullName];",
                Created(destination,
                    "Person"));
        }

        /// <summary>
        /// Verifies copy table schema to async of several tables with mappings applies the mappings to each table.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncOfSeveralTablesWithMappingsAppliesTheMappingsToEachTable()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            var destination = GetDestination();
            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Person", "Country" },
                destination,
                Tables(Table("Person",
                        Map("Id"),
                        Map("Name",
                            "FullName")),
                    Table("Country",
                        Map("Id"),
                        Map("Name",
                            "FullName"))));
            // Assert
            Assert.AreEqual("CREATE Person(Id,FullName) PK(Id) UQ[FullName];",
                Created(destination,
                    "Person"));
            Assert.AreEqual("CREATE Country(Id,FullName) PK(Id);",
                Created(destination,
                    "Country"));
        }

        #endregion

        #region Errors

        /// <summary>
        /// Verifies throw exception on copy table schema to if the mappings are null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToIfTheMappingsAreNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new CustomDbConnection().CopyTableSchemaTo("Person",
                    GetDestination(),
                    (IList<ColumnMapping>)null));
        }

        /// <summary>
        /// Verifies throw exception on copy table schema to if the mappings are empty.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToIfTheMappingsAreEmpty()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() => new CustomDbConnection().CopyTableSchemaTo("Person",
                    GetDestination(),
                    new List<ColumnMapping>()));
        }

        /// <summary>
        /// Verifies throw exception on copy table schema to if the mappings have anull item.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToIfTheMappingsHaveANullItem()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() => new CustomDbConnection().CopyTableSchemaTo("Person",
                    GetDestination(),
                    All(Map("Id"),
                        null)));
        }

        /// <summary>
        /// Verifies throw exception on copy table schema to if asource column is null or white space.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToIfASourceColumnIsNullOrWhiteSpace()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() => new CustomDbConnection().CopyTableSchemaTo("Person",
                    GetDestination(),
                    All(Map(" ",
                            "Id"))));
            Assert.Throws<ArgumentException>(() => new CustomDbConnection().CopyTableSchemaTo("Person",
                    GetDestination(),
                    All(Map(null,
                            "Id"))));
        }

        /// <summary>
        /// Verifies throw exception on copy table schema to if asource column is mapped more than once.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToIfASourceColumnIsMappedMoreThanOnce()
        {
            // Act/Assert
            var ex = Assert.Throws<ArgumentException>(() => new CustomDbConnection().CopyTableSchemaTo("Person",
                    GetDestination(),
                    All(Map("Name",
                            "A"),
                        Map("name",
                            "B"))));
            StringAssert.Contains(ex.Message,
                "more than once");
        }

        /// <summary>
        /// Verifies throw exception on copy table schema to if asource column is not acolumn of the source table.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToIfASourceColumnIsNotAColumnOfTheSourceTable()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            // Act/Assert
            var ex = Assert.Throws<ArgumentException>(() => new CustomDbConnection().CopyTableSchemaTo("Person",
                    GetDestination(),
                    All(Map("Missing",
                            "Other"))));
            StringAssert.Contains(ex.Message,
                "Missing");
        }

        /// <summary>
        /// Verifies throw exception on copy table schema to if the target names make the table have the same column twice.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToIfTheTargetNamesMakeTheTableHaveTheSameColumnTwice()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            // Act/Assert
            var ex = Assert.Throws<ArgumentException>(() => new CustomDbConnection().CopyTableSchemaTo("Person",
                    GetDestination(),
                    All(Map("Name",
                            "Age"),
                        Map("Age"))));
            StringAssert.Contains(ex.Message,
                "Age");
        }

        /// <summary>
        /// Verifies throw exception on copy table schema to of several tables if asource column is not acolumn of one of the tables.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToOfSeveralTablesIfASourceColumnIsNotAColumnOfOneOfTheTables()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            // Act/Assert
            Assert.Throws<ArgumentException>(() => new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" },
                    GetDestination(),
                    Tables(Table("Person",
                            Map("Id")),
                        Table("Country",
                            Map("Age")))));
        }

        /// <summary>
        /// Verifies throw exception on copy table schema to async if asource column is not acolumn of the source table.
        /// </summary>
        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemaToAsyncIfASourceColumnIsNotAColumnOfTheSourceTable()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentException>(() => new CustomDbConnection().CopyTableSchemaToAsync("Person",
                    GetDestination(),
                    All(Map("Missing",
                            "Other"))));
        }

        /// <summary>
        /// Verifies throw exception on copy table schema to async if the mappings are empty.
        /// </summary>
        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemaToAsyncIfTheMappingsAreEmpty()
        {
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentException>(() => new CustomDbConnection().CopyTableSchemaToAsync("Person",
                    GetDestination(),
                    new List<ColumnMapping>()));
        }

        #endregion

        #region Table Mappings

        /// <summary>
        /// Performs table.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="columns">The columns.</param>
        private static TableMapping Table(string name,
            params ColumnMapping[] columns) => new TableMapping(name,
            null,
            columns);

        /// <summary>
        /// Performs tables.
        /// </summary>
        /// <param name="tables">The tables.</param>
        private static IList<TableMapping> Tables(params TableMapping[] tables) => tables;

        /// <summary>
        /// Verifies copy table schema table constructor sets the table name and the columns.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaTableConstructorSetsTheTableNameAndTheColumns()
        {
            // Setup
            var columns = new[]
            {
                Map("Id"),
                Map("Name",
                    "FullName")
            };
            // Act
            var actual = new TableMapping("Person",
                null,
                columns);
            // Assert
            Assert.AreEqual("Person",
                actual.Source);
            Assert.AreSame(columns,
                actual.Columns);
        }

        /// <summary>
        /// Verifies copy table schema to of several tables with table mappings uses the columns of each table.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToOfSeveralTablesWithTableMappingsUsesTheColumnsOfEachTable()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            var destination = GetDestination();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" },
                destination,
                Tables(Table("Person",
                        Map("Id"),
                        Map("Name",
                            "FullName")),
                    Table("Country",
                        Map("Id",
                            "CountryKey"),
                        Map("Name",
                            "Title"))));
            // Assert
            Assert.AreEqual("CREATE Person(Id,FullName) PK(Id) UQ[FullName];",
                Created(destination,
                    "Person"));
            Assert.AreEqual("CREATE Country(CountryKey,Title) PK(CountryKey);",
                Created(destination,
                    "Country"));
        }

        /// <summary>
        /// Verifies copy table schema to of several tables with table mappings creates the tables that are not mapped with all their columns.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToOfSeveralTablesWithTableMappingsCreatesTheTablesThatAreNotMappedWithAllTheirColumns()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            var destination = GetDestination();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" },
                destination,
                Tables(Table("Person",
                        Map("Id"),
                        Map("Name",
                            "FullName"))));
            // Assert
            Assert.AreEqual("CREATE Person(Id,FullName) PK(Id) UQ[FullName];",
                Created(destination,
                    "Person"));
            Assert.AreEqual("CREATE Country(Id,Name) PK(Id);",
                Created(destination,
                    "Country"));
        }

        /// <summary>
        /// Verifies copy table schema to of several tables with table mappings finds the tables by their name without case and quotes.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToOfSeveralTablesWithTableMappingsFindsTheTablesByTheirNameWithoutCaseAndQuotes()
        {
            foreach (var name in new[]
            {
                "Person",
                "person",
                "dbo.Person",
                "[dbo].[Person]",
                "\"DBO\".\"PERSON\""
            }

            )
            {
                // Setup
                MapReader(GetPerson(),
                    GetCountry());
                MapComposer();
                var destination = GetDestination();
                // Act
                new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" },
                    destination,
                    Tables(Table(name,
                            Map("Id"),
                            Map("Name",
                                "FullName"))));
                // Assert
                Assert.AreEqual("CREATE Person(Id,FullName) PK(Id) UQ[FullName];",
                    Created(destination,
                        "Person"));
            }
        }

        /// <summary>
        /// Verifies copy table schema to of several tables with table mappings can map the related tables.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToOfSeveralTablesWithTableMappingsCanMapTheRelatedTables()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            var destination = GetDestination();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" },
                destination,
                Tables(Table("Country",
                        Map("Id"),
                        Map("Name",
                            "Title"))),
                relationshipBehavior: RelatedTableBehavior.Parents);
            // Assert
            Assert.AreEqual("CREATE Country(Id,Title) PK(Id);",
                Created(destination,
                    "Country"));
            StringAssert.StartsWith(Created(destination,
                    "Person"),
                "CREATE Person(Id,Name,Age,CountryId)");
        }

        /// <summary>
        /// Verifies copy table schema to of several tables with table mappings makes the foreign key follow the target names of the referenced table.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToOfSeveralTablesWithTableMappingsMakesTheForeignKeyFollowTheTargetNamesOfTheReferencedTable()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            var destination = GetDestination();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" },
                destination,
                Tables(Table("Person",
                        Map("Id"),
                        Map("CountryId")),
                    Table("Country",
                        Map("Id",
                            "CountryKey"),
                        Map("Name"))));
            // Assert
            Assert.AreEqual("CREATE Person(Id,CountryId) PK(Id) FK[CountryId->Country(CountryKey)];",
                Created(destination,
                    "Person"));
        }

        /// <summary>
        /// Verifies copy table schema to of several tables with table mappings does not create the foreign key that references acolumn that is not mapped.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToOfSeveralTablesWithTableMappingsDoesNotCreateTheForeignKeyThatReferencesAColumnThatIsNotMapped()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            var destination = GetDestination();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" },
                destination,
                Tables(Table("Person",
                        Map("Id"),
                        Map("CountryId")),
                    Table("Country",
                        Map("Name"))));
            // Assert
            Assert.AreEqual("CREATE Person(Id,CountryId) PK(Id);",
                Created(destination,
                    "Person"));
        }

        /// <summary>
        /// Verifies copy table schema to of several tables with table mappings does not change the schemas of the source.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToOfSeveralTablesWithTableMappingsDoesNotChangeTheSchemasOfTheSource()
        {
            // Setup
            var person = GetPerson();
            var country = GetCountry();
            MapReader(person,
                country);
            MapComposer();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" },
                GetDestination(),
                Tables(Table("Person",
                        Map("Name",
                            "FullName")),
                    Table("Country",
                        Map("Id",
                            "CountryKey"))));
            // Assert
            CollectionAssert.AreEqual(new[] { "Id", "Name", "Age", "CountryId" },
                person.Columns.Select(c => c.Field.Name).ToList());
            CollectionAssert.AreEqual(new[] { "Id", "Name" },
                country.Columns.Select(c => c.Field.Name).ToList());
            CollectionAssert.AreEqual(new[] { "Id" },
                country.PrimaryKey.Columns.ToList());
        }

        /// <summary>
        /// Verifies copy table schema to async of several tables with table mappings uses the columns of each table.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncOfSeveralTablesWithTableMappingsUsesTheColumnsOfEachTable()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            var destination = GetDestination();
            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Person", "Country" },
                destination,
                Tables(Table("Person",
                        Map("Id"),
                        Map("Name",
                            "FullName")),
                    Table("Country",
                        Map("Id",
                            "CountryKey"),
                        Map("Name",
                            "Title"))));
            // Assert
            Assert.AreEqual("CREATE Person(Id,FullName) PK(Id) UQ[FullName];",
                Created(destination,
                    "Person"));
            Assert.AreEqual("CREATE Country(CountryKey,Title) PK(CountryKey);",
                Created(destination,
                    "Country"));
        }

        /// <summary>
        /// Verifies copy table schema to async of several tables with table mappings can map the related tables.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncOfSeveralTablesWithTableMappingsCanMapTheRelatedTables()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            var destination = GetDestination();
            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Person" },
                destination,
                Tables(Table("Country",
                        Map("Id"),
                        Map("Name",
                            "Title"))),
                relationshipBehavior: RelatedTableBehavior.Parents);
            // Assert
            Assert.AreEqual("CREATE Country(Id,Title) PK(Id);",
                Created(destination,
                    "Country"));
        }

        /// <summary>
        /// Verifies throw exception on copy table schema to of several tables if the table mappings are null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToOfSeveralTablesIfTheTableMappingsAreNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" },
                    GetDestination(),
                    (IList<TableMapping>)null));
        }

        /// <summary>
        /// Verifies throw exception on copy table schema to of several tables if the table mappings are empty.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToOfSeveralTablesIfTheTableMappingsAreEmpty()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() => new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" },
                    GetDestination(),
                    new List<TableMapping>()));
        }

        /// <summary>
        /// Verifies throw exception on copy table schema to of several tables if the table mappings have anull item.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToOfSeveralTablesIfTheTableMappingsHaveANullItem()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() => new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" },
                    GetDestination(),
                    Tables(Table("Person",
                            Map("Id")),
                        null)));
        }

        /// <summary>
        /// Verifies throw exception on copy table schema to of several tables if the name of atable is null or white space.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToOfSeveralTablesIfTheNameOfATableIsNullOrWhiteSpace()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() => new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" },
                    GetDestination(),
                    Tables(Table(" ",
                            Map("Id")))));
            Assert.Throws<ArgumentException>(() => new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" },
                    GetDestination(),
                    Tables(Table(null,
                            Map("Id")))));
        }

        /// <summary>
        /// Verifies throw exception on copy table schema to of several tables if atable has no columns.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToOfSeveralTablesIfATableHasNoColumns()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() => new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" },
                    GetDestination(),
                    Tables(new TableMapping("Person",
                            null,
                            null))));
            Assert.Throws<ArgumentException>(() => new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" },
                    GetDestination(),
                    Tables(Table("Person"))));
        }

        /// <summary>
        /// Verifies throw exception on copy table schema to of several tables if atable is given more than once.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToOfSeveralTablesIfATableIsGivenMoreThanOnce()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            // Act/Assert
            Assert.Throws<ArgumentException>(() => new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" },
                    GetDestination(),
                    Tables(Table("Person",
                            Map("Id")),
                        Table("person",
                            Map("Name")))));
            Assert.Throws<ArgumentException>(() => new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" },
                    GetDestination(),
                    Tables(Table("Person",
                            Map("Id")),
                        Table("dbo.Person",
                            Map("Name")))));
        }

        /// <summary>
        /// Verifies throw exception on copy table schema to of several tables if atable is not one of the tables that are copied.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToOfSeveralTablesIfATableIsNotOneOfTheTablesThatAreCopied()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            // Act/Assert
            var ex = Assert.Throws<ArgumentException>(() => new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" },
                    GetDestination(),
                    Tables(Table("Country",
                            Map("Id")))));
            StringAssert.Contains(ex.Message,
                "Country");
        }

        /// <summary>
        /// Verifies throw exception on copy table schema to of several tables if asource column is not acolumn of its table.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToOfSeveralTablesIfASourceColumnIsNotAColumnOfItsTable()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            // Act/Assert
            var ex = Assert.Throws<ArgumentException>(() => new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" },
                    GetDestination(),
                    Tables(Table("Person",
                            Map("Id")),
                        Table("Country",
                            Map("Age")))));
            StringAssert.Contains(ex.Message,
                "Age");
            StringAssert.Contains(ex.Message,
                "Country");
        }

        /// <summary>
        /// Verifies throw exception on copy table schema to of several tables if the mappings of atable are not valid.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToOfSeveralTablesIfTheMappingsOfATableAreNotValid()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            // Act/Assert
            Assert.Throws<ArgumentException>(() => new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" },
                    GetDestination(),
                    Tables(Table("Person",
                            Map("Name",
                                "A"),
                            Map("name",
                                "B")))));
            Assert.Throws<ArgumentException>(() => new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" },
                    GetDestination(),
                    Tables(Table("Person",
                            Map("Name",
                                "Age"),
                            Map("Age")))));
        }

        /// <summary>
        /// Verifies throw exception on copy table schema to async of several tables if atable is not one of the tables that are copied.
        /// </summary>
        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemaToAsyncOfSeveralTablesIfATableIsNotOneOfTheTablesThatAreCopied()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentException>(() => new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Person" },
                    GetDestination(),
                    Tables(Table("Country",
                            Map("Id")))));
        }

        /// <summary>
        /// Verifies throw exception on copy table schema to async of several tables if the table mappings are empty.
        /// </summary>
        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemaToAsyncOfSeveralTablesIfTheTableMappingsAreEmpty()
        {
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentException>(() => new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Person" },
                    GetDestination(),
                    new List<TableMapping>()));
        }
        #endregion
    }

    /// <summary>
    /// Contains the person behavior.
    /// </summary>
    public class Person
    {
    }
}
