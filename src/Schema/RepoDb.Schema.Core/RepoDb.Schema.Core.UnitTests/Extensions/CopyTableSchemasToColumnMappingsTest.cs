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
    [TestClass]
    public class CopyTableSchemasToColumnMappingsTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            SchemaReaderMapper.Clear();
            SchemaComposerMapper.Clear();
        }

        #region Helpers

        private static TableSchema GetPerson()
        {
            var schema = new TableSchema("Person", "dbo");
            foreach (var column in new[] { "Id", "Name", "Age", "CountryId" })
            {
                schema.Columns.Add(new ColumnInfo { Field = new DbField(column, false, false, true, typeof(string), null, null, null, "text", false, "Test") });
            }
            schema.PrimaryKey = new PrimaryKeyInfo("PK_Person") { Columns = new List<string> { "Id" } };
            schema.Indexes.Add(new IndexInfo("IX_Person_Name") { Columns = new List<string> { "Name" }, IncludedColumns = new List<string> { "Age" }, DescendingColumns = new List<string> { "Name" } });
            schema.UniqueConstraints.Add(new UniqueConstraintInfo("UQ_Person_Name") { Columns = new List<string> { "Name" } });
            schema.CheckConstraints.Add(new CheckConstraintInfo("CK_Person_Age") { Expression = "([Age] >= 0)" });
            schema.ForeignKeys.Add(new ForeignKeyInfo("FK_Person_Country")
            {
                Columns = new List<string> { "CountryId" },
                ReferencedTable = new TableInfo("Country", "dbo"),
                ReferencedColumns = new List<string> { "Id" }
            });
            return schema;
        }

        private static TableSchema GetCountry()
        {
            var schema = new TableSchema("Country", "dbo");
            foreach (var column in new[] { "Id", "Name" })
            {
                schema.Columns.Add(new ColumnInfo { Field = new DbField(column, false, false, true, typeof(string), null, null, null, "text", false, "Test") });
            }
            schema.PrimaryKey = new PrimaryKeyInfo("PK_Country") { Columns = new List<string> { "Id" } };
            return schema;
        }

        private static TableSchema GetPet()
        {
            var schema = new TableSchema("Pet", "dbo");
            foreach (var column in new[] { "Id", "PersonName" })
            {
                schema.Columns.Add(new ColumnInfo { Field = new DbField(column, false, false, true, typeof(string), null, null, null, "text", false, "Test") });
            }
            schema.ForeignKeys.Add(new ForeignKeyInfo("FK_Pet_Person")
            {
                Columns = new List<string> { "PersonName" },
                ReferencedTable = new TableInfo("Person", "dbo"),
                ReferencedColumns = new List<string> { "Name" }
            });
            return schema;
        }

        private static void MapReader(params TableSchema[] schemas)
        {
            var reader = new Mock<ISchemaReader>();
            reader.Setup(r => r.GetTableSchema(It.IsAny<string>())).Returns<string>(name => schemas.Single(s => s.Table.Name == name));
            reader.Setup(r => r.GetTableSchemaAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns<string, CancellationToken>((name, _) => Task.FromResult(schemas.Single(s => s.Table.Name == name)));
            reader.Setup(r => r.GetRelatedTables(It.IsAny<IEnumerable<string>>(), It.IsAny<CopyTableSchemaRelationshipBehavior>())).Returns<IEnumerable<string>, CopyTableSchemaRelationshipBehavior>((names, _) => schemas.Select(s => s.Table.Name).ToList());
            reader.Setup(r => r.GetRelatedTablesAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CopyTableSchemaRelationshipBehavior>(), It.IsAny<CancellationToken>())).Returns<IEnumerable<string>, CopyTableSchemaRelationshipBehavior, CancellationToken>((names, _, __) => Task.FromResult<IEnumerable<string>>(schemas.Select(s => s.Table.Name).ToList()));
            reader.Setup(r => r.GetDependencyOrder(It.IsAny<IEnumerable<string>>())).Returns<IEnumerable<string>>(names => schemas.Where(s => names.Contains(s.Table.Name)).Select(s => new RelationshipInfo { Schema = s }).ToList());
            reader.Setup(r => r.GetDependencyOrderAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>())).Returns<IEnumerable<string>, CancellationToken>((names, _) => Task.FromResult<IEnumerable<RelationshipInfo>>(schemas.Where(s => names.Contains(s.Table.Name)).Select(s => new RelationshipInfo { Schema = s }).ToList()));
            SchemaReaderMapper.Add<CustomDbConnection>(reader.Object, true);
        }

        private static string Describe(TableSchema schema) =>
            $"{schema.Table.Name}({string.Join(",", schema.Columns.Select(c => c.Field.Name))})" +
            $" PK({(schema.PrimaryKey == null ? string.Empty : string.Join(",", schema.PrimaryKey.Columns))})" +
            string.Concat(schema.Indexes.Select(i => $" IX[{string.Join(",", i.Columns)}|{string.Join(",", i.IncludedColumns)}|{string.Join(",", i.DescendingColumns)}]")) +
            string.Concat(schema.UniqueConstraints.Select(u => $" UQ[{string.Join(",", u.Columns)}]")) +
            string.Concat(schema.CheckConstraints.Select(c => $" CK[{c.Expression}]")) +
            string.Concat(schema.ForeignKeys.Select(f => $" FK[{string.Join(",", f.Columns)}->{f.ReferencedTable.Name}({string.Join(",", f.ReferencedColumns)})]"));

        private static IEnumerable<string> Script(IEnumerable<TableSchema> schemas) =>
            schemas.Select(s => $"CREATE {Describe(s)};");

        private static void MapComposer()
        {
            var composer = new Mock<ISchemaComposer>();
            composer.Setup(c => c.ComposeName(It.IsAny<TableInfo>())).Returns<TableInfo>(t => t.Name);
            composer.Setup(c => c.ComposeSchemas(It.IsAny<IEnumerable<TableSchema>>())).Returns<IEnumerable<TableSchema>>(s => Script(s).ToList());
            composer.Setup(c => c.ComposeSchema(It.IsAny<TableSchema>())).Returns<TableSchema>(s => Script(new[] { s }).ToList());
            composer.Setup(c => c.ComposeDropTable(It.IsAny<string>())).Returns<string>(n => $"DROP TABLE {n};");
            composer.Setup(c => c.ComposeAddColumn(It.IsAny<string>(), It.IsAny<ColumnInfo>())).Returns<string, ColumnInfo>((n, c) => $"ADD COLUMN {c.Field.Name} TO {n};");
            composer.Setup(c => c.ComposeCreateIndex(It.IsAny<string>(), It.IsAny<IndexInfo>())).Returns<string, IndexInfo>((n, i) => $"CREATE INDEX {i.Name} ON {n};");
            composer.Setup(c => c.ComposeAddForeignKey(It.IsAny<string>(), It.IsAny<ForeignKeyInfo>())).Returns<string, ForeignKeyInfo>((n, f) => $"ADD {f.Name} TO {n};");
            composer.Setup(c => c.ComposeTableExists(It.IsAny<string>())).Returns<string>(n => $"TABLE_EXISTS {n}");
            composer.Setup(c => c.ComposeColumnExists(It.IsAny<string>(), It.IsAny<string>())).Returns<string, string>((n, c) => $"COLUMN_EXISTS {n}.{c}");
            composer.Setup(c => c.ComposeIndexExists(It.IsAny<string>(), It.IsAny<string>())).Returns<string, string>((n, i) => $"INDEX_EXISTS {n}.{i}");
            SchemaComposerMapper.Add<CustomDbConnection>(composer.Object, true);
        }

        private static CustomDbConnection GetDestination(params string[] existing) =>
            new CustomDbConnection { ScalarResult = statement => existing.Contains(statement) ? 1 : 0 };

        private static string Created(CustomDbConnection destination, string table) =>
            destination.ExecutedCommands.First(c => c.StartsWith($"CREATE {table}("));

        #endregion

        #region Mappings

        private static CopyTableSchemaColumnMapping Map(string source, string target = null) =>
            new CopyTableSchemaColumnMapping(source, target);

        private static IList<CopyTableSchemaColumnMapping> All(params CopyTableSchemaColumnMapping[] mappings) =>
            mappings;

        private static IList<CopyTableSchemaColumnMapping> AllColumns() =>
            All(Map("Id", "PersonId"), Map("Name", "FullName"), Map("Age", "Years"), Map("CountryId", "Country_Id"));

        [TestMethod]
        public void TestCopyTableSchemaColumnMappingConstructorSetsTheSourceAndTheTarget()
        {
            // Act
            var actual = new CopyTableSchemaColumnMapping("Name", "FullName");

            // Assert
            Assert.AreEqual("Name", actual.Source);
            Assert.AreEqual("FullName", actual.Target);
        }

        [TestMethod]
        public void TestCopyTableSchemaColumnMappingConstructorKeepsTheTargetNullByDefault()
        {
            // Act
            var actual = new CopyTableSchemaColumnMapping("Name");

            // Assert
            Assert.AreEqual("Name", actual.Source);
            Assert.IsNull(actual.Target);
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithMappingsCreatesOnlyTheMappedColumnsByTheirTargetNames()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person", destination, All(Map("Id"), Map("Name", "FullName")));

            // Assert
            Assert.AreEqual("CREATE Person(Id,FullName) PK(Id) UQ[FullName];", Created(destination, "Person"));
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithMappingsMakesTheObjectsFollowTheTargetNames()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Country");

            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person", destination, AllColumns());

            // Assert
            Assert.AreEqual("CREATE Person(PersonId,FullName,Years,Country_Id) PK(PersonId) IX[FullName|Years|FullName] UQ[FullName] CK[([Years] >= 0)] FK[Country_Id->Country(Id)];", Created(destination, "Person"));
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithMappingsKeepsTheSourceNameIfTheTargetIsNullOrWhiteSpace()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person", destination, All(Map("Id", null), Map("Name", " "), Map("Age", "Years")));

            // Assert
            Assert.AreEqual("CREATE Person(Id,Name,Years) PK(Id) IX[Name|Years|Name] UQ[Name] CK[([Years] >= 0)];", Created(destination, "Person"));
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithMappingsMatchesTheSourceNamesWithoutCase()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person", destination, All(Map("id"), Map("NAME", "FullName")));

            // Assert
            Assert.AreEqual("CREATE Person(Id,FullName) PK(Id) UQ[FullName];", Created(destination, "Person"));
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithMappingsKeepsTheOrderOfTheColumnsOfTheSourceTable()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person", destination, All(Map("Name"), Map("Id")));

            // Assert
            StringAssert.StartsWith(Created(destination, "Person"), "CREATE Person(Id,Name)");
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithMappingsCanSwapTheNamesOfTwoColumns()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Country");

            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person", destination, All(Map("Id"), Map("Name", "Age"), Map("Age", "Name"), Map("CountryId")));

            // Assert
            Assert.AreEqual("CREATE Person(Id,Age,Name,CountryId) PK(Id) IX[Age|Name|Age] UQ[Age] CK[([Name] >= 0)] FK[CountryId->Country(Id)];", Created(destination, "Person"));
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithMappingsCanUseTheNameOfAColumnThatIsNotMapped()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person", destination, All(Map("Id"), Map("Name", "Age")));

            // Assert
            Assert.AreEqual("CREATE Person(Id,Age) PK(Id) UQ[Age];", Created(destination, "Person"));
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithMappingsMakesTheForeignKeyOfARelatedTableFollowTheTargetNames()
        {
            // Setup
            MapReader(GetPerson(), GetPet());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person", destination, All(Map("Id"), Map("Name", "FullName")), relationshipBehavior: CopyTableSchemaRelationshipBehavior.Children);

            // Assert
            Assert.AreEqual("CREATE Pet(Id,PersonName) PK() FK[PersonName->Person(FullName)];", Created(destination, "Pet"));
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithMappingsDoesNotCreateTheForeignKeyOfARelatedTableThatReferencesAColumnThatIsNotMapped()
        {
            // Setup
            MapReader(GetPerson(), GetPet());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person", destination, All(Map("Id"), Map("Age")), relationshipBehavior: CopyTableSchemaRelationshipBehavior.Children);

            // Assert
            Assert.AreEqual("CREATE Pet(Id,PersonName) PK();", Created(destination, "Pet"));
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithMappingsDoesNotChangeTheRelatedTables()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person", destination, All(Map("Id", "PersonId"), Map("Name", "FullName"), Map("CountryId")), relationshipBehavior: CopyTableSchemaRelationshipBehavior.Parents);

            // Assert
            Assert.AreEqual("CREATE Country(Id,Name) PK(Id);", Created(destination, "Country"));
            StringAssert.Contains(Created(destination, "Person"), "FK[CountryId->Country(Id)]");
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithMappingsDoesNotChangeTheSchemaOfTheSource()
        {
            // Setup
            var person = GetPerson();
            MapReader(person, GetCountry());
            MapComposer();

            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person", GetDestination(), All(Map("Name", "FullName")));

            // Assert
            CollectionAssert.AreEqual(new[] { "Id", "Name", "Age", "CountryId" }, person.Columns.Select(c => c.Field.Name).ToList());
            CollectionAssert.AreEqual(new[] { "Name" }, person.Indexes.Single().Columns.ToList());
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithMappingsKeepsTheOtherPropertiesOfTheColumns()
        {
            // Setup
            var person = GetPerson();
            person.Columns[1] = new ColumnInfo
            {
                Field = new DbField("Name", false, false, false, typeof(string), 50, null, null, "varchar", true, "Test"),
                Ordinal = 2,
                DefaultExpression = "'N/A'",
                Collation = "Latin1",
                Comment = "The name"
            };
            MapReader(person, GetCountry());
            var composed = new List<TableSchema>();
            var composer = new Mock<ISchemaComposer>();
            composer.Setup(c => c.ComposeName(It.IsAny<TableInfo>())).Returns<TableInfo>(t => t.Name);
            composer.Setup(c => c.ComposeSchemas(It.IsAny<IEnumerable<TableSchema>>())).Returns<IEnumerable<TableSchema>>(s => { composed.AddRange(s); return new List<string> { "CREATE;" }; });
            composer.Setup(c => c.ComposeSchema(It.IsAny<TableSchema>())).Returns<TableSchema>(s => { composed.Add(s); return new List<string> { "CREATE;" }; });
            SchemaComposerMapper.Add<CustomDbConnection>(composer.Object, true);

            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person", GetDestination(), All(Map("Name", "FullName")));

            // Assert
            var column = composed.First().Columns.Single();
            Assert.AreEqual("FullName", column.Field.Name);
            Assert.AreEqual(2, column.Ordinal);
            Assert.AreEqual("'N/A'", column.DefaultExpression);
            Assert.AreEqual("Latin1", column.Collation);
            Assert.AreEqual("The name", column.Comment);
            Assert.AreEqual(50, column.Field.Size);
            Assert.AreEqual("varchar", column.Field.DatabaseType);
            Assert.IsFalse(column.Field.IsNullable);
            Assert.IsTrue(column.Field.HasDefaultValue);
        }

        [TestMethod]
        public void TestCopyTableSchemaToOfSeveralTablesWithMappingsAppliesTheMappingsToEachTable()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" }, destination, Tables(Table("Person", Map("Id"), Map("Name", "FullName")), Table("Country", Map("Id"), Map("Name", "FullName"))));

            // Assert
            Assert.AreEqual("CREATE Person(Id,FullName) PK(Id) UQ[FullName];", Created(destination, "Person"));
            Assert.AreEqual("CREATE Country(Id,FullName) PK(Id);", Created(destination, "Country"));
        }

        [TestMethod]
        public void TestCopyTableSchemaToOfAnEntityWithMappingsCreatesOnlyTheMappedColumnsByTheirTargetNames()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopyTableSchemaTo<Person>(destination, All(Map("Id"), Map("Name", "FullName")));

            // Assert
            Assert.AreEqual("CREATE Person(Id,FullName) PK(Id) UQ[FullName];", Created(destination, "Person"));
        }

        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncWithMappingsCreatesOnlyTheMappedColumnsByTheirTargetNames()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync("Person", destination, All(Map("Id"), Map("Name", "FullName")));

            // Assert
            Assert.AreEqual("CREATE Person(Id,FullName) PK(Id) UQ[FullName];", Created(destination, "Person"));
        }

        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncOfAnEntityWithMappingsCreatesOnlyTheMappedColumnsByTheirTargetNames()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync<Person>(destination, All(Map("Id"), Map("Name", "FullName")));

            // Assert
            Assert.AreEqual("CREATE Person(Id,FullName) PK(Id) UQ[FullName];", Created(destination, "Person"));
        }

        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncOfSeveralTablesWithMappingsAppliesTheMappingsToEachTable()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Person", "Country" }, destination, Tables(Table("Person", Map("Id"), Map("Name", "FullName")), Table("Country", Map("Id"), Map("Name", "FullName"))));

            // Assert
            Assert.AreEqual("CREATE Person(Id,FullName) PK(Id) UQ[FullName];", Created(destination, "Person"));
            Assert.AreEqual("CREATE Country(Id,FullName) PK(Id);", Created(destination, "Country"));
        }

        #endregion

        #region Errors

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToIfTheMappingsAreNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() =>
                new CustomDbConnection().CopyTableSchemaTo("Person", GetDestination(), (IList<CopyTableSchemaColumnMapping>)null));
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToIfTheMappingsAreEmpty()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() =>
                new CustomDbConnection().CopyTableSchemaTo("Person", GetDestination(), new List<CopyTableSchemaColumnMapping>()));
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToIfTheMappingsHaveANullItem()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() =>
                new CustomDbConnection().CopyTableSchemaTo("Person", GetDestination(), All(Map("Id"), null)));
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToIfASourceColumnIsNullOrWhiteSpace()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() =>
                new CustomDbConnection().CopyTableSchemaTo("Person", GetDestination(), All(Map(" ", "Id"))));
            Assert.Throws<ArgumentException>(() =>
                new CustomDbConnection().CopyTableSchemaTo("Person", GetDestination(), All(Map(null, "Id"))));
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToIfASourceColumnIsMappedMoreThanOnce()
        {
            // Act/Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                new CustomDbConnection().CopyTableSchemaTo("Person", GetDestination(), All(Map("Name", "A"), Map("name", "B"))));
            StringAssert.Contains(ex.Message, "more than once");
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToIfASourceColumnIsNotAColumnOfTheSourceTable()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();

            // Act/Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                new CustomDbConnection().CopyTableSchemaTo("Person", GetDestination(), All(Map("Missing", "Other"))));
            StringAssert.Contains(ex.Message, "Missing");
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToIfTheTargetNamesMakeTheTableHaveTheSameColumnTwice()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();

            // Act/Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                new CustomDbConnection().CopyTableSchemaTo("Person", GetDestination(), All(Map("Name", "Age"), Map("Age"))));
            StringAssert.Contains(ex.Message, "Age");
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToOfSeveralTablesIfASourceColumnIsNotAColumnOfOneOfTheTables()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();

            // Act/Assert
            Assert.Throws<ArgumentException>(() =>
                new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" }, GetDestination(), Tables(Table("Person", Map("Id")), Table("Country", Map("Age")))));
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemaToAsyncIfASourceColumnIsNotAColumnOfTheSourceTable()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();

            // Act/Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                new CustomDbConnection().CopyTableSchemaToAsync("Person", GetDestination(), All(Map("Missing", "Other"))));
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemaToAsyncIfTheMappingsAreEmpty()
        {
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                new CustomDbConnection().CopyTableSchemaToAsync("Person", GetDestination(), new List<CopyTableSchemaColumnMapping>()));
        }

        #endregion

        #region Table Mappings

        private static CopyTableSchemaTable Table(string name, params CopyTableSchemaColumnMapping[] columns) =>
            new CopyTableSchemaTable(name, columns);

        private static IList<CopyTableSchemaTable> Tables(params CopyTableSchemaTable[] tables) =>
            tables;

        [TestMethod]
        public void TestCopyTableSchemaTableConstructorSetsTheTableNameAndTheColumns()
        {
            // Setup
            var columns = new[] { Map("Id"), Map("Name", "FullName") };

            // Act
            var actual = new CopyTableSchemaTable("Person", columns);

            // Assert
            Assert.AreEqual("Person", actual.TableName);
            Assert.AreSame(columns, actual.Columns);
        }

        [TestMethod]
        public void TestCopyTableSchemaToOfSeveralTablesWithTableMappingsUsesTheColumnsOfEachTable()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" }, destination, Tables(
                Table("Person", Map("Id"), Map("Name", "FullName")),
                Table("Country", Map("Id", "CountryKey"), Map("Name", "Title"))));

            // Assert
            Assert.AreEqual("CREATE Person(Id,FullName) PK(Id) UQ[FullName];", Created(destination, "Person"));
            Assert.AreEqual("CREATE Country(CountryKey,Title) PK(CountryKey);", Created(destination, "Country"));
        }

        [TestMethod]
        public void TestCopyTableSchemaToOfSeveralTablesWithTableMappingsCreatesTheTablesThatAreNotMappedWithAllTheirColumns()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" }, destination, Tables(Table("Person", Map("Id"), Map("Name", "FullName"))));

            // Assert
            Assert.AreEqual("CREATE Person(Id,FullName) PK(Id) UQ[FullName];", Created(destination, "Person"));
            Assert.AreEqual("CREATE Country(Id,Name) PK(Id);", Created(destination, "Country"));
        }

        [TestMethod]
        public void TestCopyTableSchemaToOfSeveralTablesWithTableMappingsFindsTheTablesByTheirNameWithoutCaseAndQuotes()
        {
            foreach (var name in new[] { "Person", "person", "dbo.Person", "[dbo].[Person]", "\"DBO\".\"PERSON\"" })
            {
                // Setup
                MapReader(GetPerson(), GetCountry());
                MapComposer();
                var destination = GetDestination();

                // Act
                new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" }, destination, Tables(Table(name, Map("Id"), Map("Name", "FullName"))));

                // Assert
                Assert.AreEqual("CREATE Person(Id,FullName) PK(Id) UQ[FullName];", Created(destination, "Person"));
            }
        }

        [TestMethod]
        public void TestCopyTableSchemaToOfSeveralTablesWithTableMappingsCanMapTheRelatedTables()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" }, destination, Tables(Table("Country", Map("Id"), Map("Name", "Title"))), relationshipBehavior: CopyTableSchemaRelationshipBehavior.Parents);

            // Assert
            Assert.AreEqual("CREATE Country(Id,Title) PK(Id);", Created(destination, "Country"));
            StringAssert.StartsWith(Created(destination, "Person"), "CREATE Person(Id,Name,Age,CountryId)");
        }

        [TestMethod]
        public void TestCopyTableSchemaToOfSeveralTablesWithTableMappingsMakesTheForeignKeyFollowTheTargetNamesOfTheReferencedTable()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" }, destination, Tables(
                Table("Person", Map("Id"), Map("CountryId")),
                Table("Country", Map("Id", "CountryKey"), Map("Name"))));

            // Assert
            Assert.AreEqual("CREATE Person(Id,CountryId) PK(Id) FK[CountryId->Country(CountryKey)];", Created(destination, "Person"));
        }

        [TestMethod]
        public void TestCopyTableSchemaToOfSeveralTablesWithTableMappingsDoesNotCreateTheForeignKeyThatReferencesAColumnThatIsNotMapped()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" }, destination, Tables(
                Table("Person", Map("Id"), Map("CountryId")),
                Table("Country", Map("Name"))));

            // Assert
            Assert.AreEqual("CREATE Person(Id,CountryId) PK(Id);", Created(destination, "Person"));
        }

        [TestMethod]
        public void TestCopyTableSchemaToOfSeveralTablesWithTableMappingsDoesNotChangeTheSchemasOfTheSource()
        {
            // Setup
            var person = GetPerson();
            var country = GetCountry();
            MapReader(person, country);
            MapComposer();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" }, GetDestination(), Tables(
                Table("Person", Map("Name", "FullName")),
                Table("Country", Map("Id", "CountryKey"))));

            // Assert
            CollectionAssert.AreEqual(new[] { "Id", "Name", "Age", "CountryId" }, person.Columns.Select(c => c.Field.Name).ToList());
            CollectionAssert.AreEqual(new[] { "Id", "Name" }, country.Columns.Select(c => c.Field.Name).ToList());
            CollectionAssert.AreEqual(new[] { "Id" }, country.PrimaryKey.Columns.ToList());
        }

        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncOfSeveralTablesWithTableMappingsUsesTheColumnsOfEachTable()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Person", "Country" }, destination, Tables(
                Table("Person", Map("Id"), Map("Name", "FullName")),
                Table("Country", Map("Id", "CountryKey"), Map("Name", "Title"))));

            // Assert
            Assert.AreEqual("CREATE Person(Id,FullName) PK(Id) UQ[FullName];", Created(destination, "Person"));
            Assert.AreEqual("CREATE Country(CountryKey,Title) PK(CountryKey);", Created(destination, "Country"));
        }

        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncOfSeveralTablesWithTableMappingsCanMapTheRelatedTables()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Person" }, destination, Tables(Table("Country", Map("Id"), Map("Name", "Title"))), relationshipBehavior: CopyTableSchemaRelationshipBehavior.Parents);

            // Assert
            Assert.AreEqual("CREATE Country(Id,Title) PK(Id);", Created(destination, "Country"));
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToOfSeveralTablesIfTheTableMappingsAreNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() =>
                new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" }, GetDestination(), (IList<CopyTableSchemaTable>)null));
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToOfSeveralTablesIfTheTableMappingsAreEmpty()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() =>
                new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" }, GetDestination(), new List<CopyTableSchemaTable>()));
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToOfSeveralTablesIfTheTableMappingsHaveANullItem()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() =>
                new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" }, GetDestination(), Tables(Table("Person", Map("Id")), null)));
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToOfSeveralTablesIfTheNameOfATableIsNullOrWhiteSpace()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() =>
                new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" }, GetDestination(), Tables(Table(" ", Map("Id")))));
            Assert.Throws<ArgumentException>(() =>
                new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" }, GetDestination(), Tables(Table(null, Map("Id")))));
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToOfSeveralTablesIfATableHasNoColumns()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() =>
                new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" }, GetDestination(), Tables(new CopyTableSchemaTable("Person", null))));
            Assert.Throws<ArgumentException>(() =>
                new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" }, GetDestination(), Tables(Table("Person"))));
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToOfSeveralTablesIfATableIsGivenMoreThanOnce()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();

            // Act/Assert
            Assert.Throws<ArgumentException>(() =>
                new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" }, GetDestination(), Tables(Table("Person", Map("Id")), Table("person", Map("Name")))));
            Assert.Throws<ArgumentException>(() =>
                new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" }, GetDestination(), Tables(Table("Person", Map("Id")), Table("dbo.Person", Map("Name")))));
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToOfSeveralTablesIfATableIsNotOneOfTheTablesThatAreCopied()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();

            // Act/Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" }, GetDestination(), Tables(Table("Country", Map("Id")))));
            StringAssert.Contains(ex.Message, "Country");
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToOfSeveralTablesIfASourceColumnIsNotAColumnOfItsTable()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();

            // Act/Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" }, GetDestination(), Tables(Table("Person", Map("Id")), Table("Country", Map("Age")))));
            StringAssert.Contains(ex.Message, "Age");
            StringAssert.Contains(ex.Message, "Country");
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToOfSeveralTablesIfTheMappingsOfATableAreNotValid()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();

            // Act/Assert
            Assert.Throws<ArgumentException>(() =>
                new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" }, GetDestination(), Tables(Table("Person", Map("Name", "A"), Map("name", "B")))));
            Assert.Throws<ArgumentException>(() =>
                new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" }, GetDestination(), Tables(Table("Person", Map("Name", "Age"), Map("Age")))));
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemaToAsyncOfSeveralTablesIfATableIsNotOneOfTheTablesThatAreCopied()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();

            // Act/Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Person" }, GetDestination(), Tables(Table("Country", Map("Id")))));
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemaToAsyncOfSeveralTablesIfTheTableMappingsAreEmpty()
        {
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Person" }, GetDestination(), new List<CopyTableSchemaTable>()));
        }

        #endregion
    }

    public class Person
    {
    }
}
