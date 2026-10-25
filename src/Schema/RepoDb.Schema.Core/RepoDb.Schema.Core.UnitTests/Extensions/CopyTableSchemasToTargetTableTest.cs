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
    public class CopyTableSchemasToTargetTableTest
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
            reader.Setup(r => r.GetRelatedTables(It.IsAny<IEnumerable<string>>(), It.IsAny<RelatedTableBehavior>())).Returns<IEnumerable<string>, RelatedTableBehavior>((names, _) => schemas.Select(s => s.Table.Name).ToList());
            reader.Setup(r => r.GetRelatedTablesAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<RelatedTableBehavior>(), It.IsAny<CancellationToken>())).Returns<IEnumerable<string>, RelatedTableBehavior, CancellationToken>((names, _, __) => Task.FromResult<IEnumerable<string>>(schemas.Select(s => s.Table.Name).ToList()));
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

        private static IList<ColumnMapping> Columns() =>
            new[] { new ColumnMapping("Id", "PersonId"), new ColumnMapping("Name", "FullName") };

        [TestMethod]
        public void TestTableMappingConstructorKeepsTheDestinationNull()
        {
            // Act
            var actual = new TableMapping("Person", null, Columns());

            // Assert
            Assert.IsNull(actual.Destination);
        }

        [TestMethod]
        public void TestTableMappingConstructorSetsTheDestination()
        {
            // Act
            var actual = new TableMapping("Person", "Human", Columns());

            // Assert
            Assert.AreEqual("Human", actual.Destination);
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithTargetTableCreatesTheTableByTheNewName()
        {
            // Setup
            MapReader(GetPerson());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person", destination, targetTable: "Human");
            // Assert
            Assert.IsTrue(destination.ExecutedCommands.Any(c => c.StartsWith("CREATE Human(Id,Name,Age,CountryId)")));
            Assert.IsFalse(destination.ExecutedCommands.Any(c => c.StartsWith("CREATE Person(")));
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithTargetTableAndColumnsCreatesTheTableByTheNewName()
        {
            // Setup
            MapReader(GetPerson());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person", destination, columns: new[] { "Id", "Name" }, targetTable: "Human");
            // Assert
            Assert.AreEqual("CREATE Human(Id,Name) PK(Id) UQ[Name];", Created(destination, "Human"));
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithTargetTableUnquotesTheNewName()
        {
            // Setup
            MapReader(GetPerson());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person", destination, targetTable: "[Human]");
            // Assert
            Assert.IsTrue(destination.ExecutedCommands.Any(c => c.StartsWith("CREATE Human(")));
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithTargetTableAndColumnMappingsCreatesTheTableByTheNewNameWithTheNewColumns()
        {
            // Setup
            MapReader(GetPerson());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person", destination, Columns(), targetTable: "Human");
            // Assert
            Assert.AreEqual("CREATE Human(PersonId,FullName) PK(PersonId) UQ[FullName];", Created(destination, "Human"));
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithTargetTableThatIsTheSourceNameKeepsTheName()
        {
            // Setup
            MapReader(GetPerson());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person", destination, targetTable: "Person");
            // Assert
            Assert.IsTrue(destination.ExecutedCommands.Any(c => c.StartsWith("CREATE Person(")));
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithTargetTableMakesTheForeignKeysOfTheChildTablesFollowTheNewName()
        {
            // Setup
            MapReader(GetPerson(), GetCountry(), GetPet());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person", destination, targetTable: "Human", relationshipBehavior: RelatedTableBehavior.Children);
            // Assert
            Assert.IsTrue(destination.ExecutedCommands.Any(c => c.StartsWith("CREATE Human(")));
            Assert.IsTrue(Created(destination, "Pet").Contains("FK[PersonName->Human(Name)]"));
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithTargetTableKeepsTheOwnForeignKeysToTheParents()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person", destination, targetTable: "Human", relationshipBehavior: RelatedTableBehavior.Parents);
            // Assert
            Assert.IsTrue(Created(destination, "Human").Contains("FK[CountryId->Country(Id)]"));
        }

        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncWithTargetTableCreatesTheTableByTheNewName()
        {
            // Setup
            MapReader(GetPerson());
            MapComposer();
            var destination = GetDestination();

            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync("Person", destination, targetTable: "Human");
            // Assert
            Assert.IsTrue(destination.ExecutedCommands.Any(c => c.StartsWith("CREATE Human(")));
        }

        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncWithTargetTableAndColumnMappingsCreatesTheTableByTheNewName()
        {
            // Setup
            MapReader(GetPerson());
            MapComposer();
            var destination = GetDestination();

            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync("Person", destination, Columns(), targetTable: "Human");
            // Assert
            Assert.AreEqual("CREATE Human(PersonId,FullName) PK(PersonId) UQ[FullName];", Created(destination, "Human"));
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithTableMappingsRenamesOnlyTheTablesThatHaveATargetName()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" }, destination,
                new[] { new TableMapping("Person", "Human", Columns()), new TableMapping("Country", null, new[] { new ColumnMapping("Id"), new ColumnMapping("Name") }) });
            // Assert
            Assert.IsTrue(destination.ExecutedCommands.Any(c => c.StartsWith("CREATE Human(PersonId,FullName)")));
            Assert.IsTrue(destination.ExecutedCommands.Any(c => c.StartsWith("CREATE Country(Id,Name)")));
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithTableMappingsMakesTheForeignKeysFollowTheRenamedParent()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" }, destination,
                new[]
                {
                    new TableMapping("Person", null, new[] { new ColumnMapping("Id"), new ColumnMapping("Name"), new ColumnMapping("CountryId") }),
                    new TableMapping("Country", "Nation", new[] { new ColumnMapping("Id"), new ColumnMapping("Name") })
                });
            // Assert
            Assert.IsTrue(destination.ExecutedCommands.Any(c => c.StartsWith("CREATE Nation(Id,Name)")));
            Assert.IsTrue(Created(destination, "Person").Contains("FK[CountryId->Nation(Id)]"));
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithTableMappingsMatchesTheMappingBySourceTableNameEvenWhenQuoted()
        {
            // Setup
            MapReader(GetPerson());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" }, destination, new[] { new TableMapping("[Person]", "[Human]", Columns()) });
            // Assert
            Assert.IsTrue(destination.ExecutedCommands.Any(c => c.StartsWith("CREATE Human(PersonId,FullName)")));
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithTableMappingsThrowsOnWhiteSpaceTargetName()
        {
            // Setup
            MapReader(GetPerson());
            MapComposer();

            // Act and Assert
            Assert.Throws<ArgumentException>(() =>
                new CustomDbConnection().CopyTableSchemaTo(new[] { "Person" }, GetDestination(), new[] { new TableMapping("Person", " ", Columns()) }));
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithTableMappingsThrowsWhenTwoTablesAreRenamedToTheSameName()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();

            // Act and Assert
            Assert.Throws<ArgumentException>(() =>
                new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" }, GetDestination(), new[] { new TableMapping("Person", "Same", Columns()), new TableMapping("Country", "same", new[] { new ColumnMapping("Id") }) }));
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithTableMappingsThrowsWhenATableIsRenamedToTheNameOfAnotherTable()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();

            // Act and Assert
            Assert.Throws<ArgumentException>(() =>
                new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" }, GetDestination(), new[] { new TableMapping("Person", "Country", Columns()), new TableMapping("Country", null, new[] { new ColumnMapping("Id") }) }));
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithTargetTableThrowsOnWhiteSpace()
        {
            // Setup
            MapReader(GetPerson());
            MapComposer();

            // Act and Assert
            Assert.Throws<ArgumentException>(() =>
                new CustomDbConnection().CopyTableSchemaTo("Person", GetDestination(), targetTable: " "));
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithMappingsThrowsListingTheTablesFoundWhenTheTableIsNotOneOfThem()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();

            // Act
            var actual = Assert.Throws<ArgumentException>(() =>
                new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" }, GetDestination(), new[] { new TableMapping("Human", null, Columns()) }));

            // Assert
            StringAssert.Contains(actual.Message, "The table 'Human' is not one of the tables that are copied.");
            StringAssert.Contains(actual.Message, "'dbo.Person'");
            StringAssert.Contains(actual.Message, "'dbo.Country'");
            Assert.AreEqual("mappings", actual.ParamName);
        }
    }
}
