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
    public class CopyTableSchemasToColumnsTest
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
            schema.Indexes.Add(new IndexInfo("IX_Person_Name") { Columns = new List<string> { "Name" } });
            schema.Indexes.Add(new IndexInfo("IX_Person_Age_Name") { Columns = new List<string> { "Age", "Name" } });
            schema.Indexes.Add(new IndexInfo("IX_Person_Id") { Columns = new List<string> { "Id" }, IncludedColumns = new List<string> { "Age" } });
            schema.UniqueConstraints.Add(new UniqueConstraintInfo("UQ_Person_Name") { Columns = new List<string> { "Name" } });
            schema.UniqueConstraints.Add(new UniqueConstraintInfo("UQ_Person_Age") { Columns = new List<string> { "Age" } });
            schema.CheckConstraints.Add(new CheckConstraintInfo("CK_Person_Age") { Expression = "([Age] >= 0)" });
            schema.CheckConstraints.Add(new CheckConstraintInfo("CK_Person_Name") { Expression = "(len([Name]) > 0)" });
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
            foreach (var column in new[] { "Id", "Name", "Code" })
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

        private static IEnumerable<string> Script(IEnumerable<TableSchema> schemas)
        {
            var list = schemas.ToList();
            return list.Select(s => $"CREATE TABLE {s.Table.Name} ({string.Join(",", s.Columns.Select(c => c.Field.Name))}" +
                    $"{(s.PrimaryKey == null ? string.Empty : " PK(" + string.Join(",", s.PrimaryKey.Columns) + ")")}" +
                    $"{string.Concat(s.UniqueConstraints.Select(u => " UQ " + u.Name))}{string.Concat(s.CheckConstraints.Select(c => " CK " + c.Name))});")
                .Concat(list.SelectMany(s => s.Indexes.Select(i => $"CREATE INDEX {i.Name} ON {s.Table.Name};")))
                .Concat(list.SelectMany(s => s.ForeignKeys.Select(f => $"ADD {f.Name} TO {s.Table.Name};")));
        }

        private static void MapComposer()
        {
            var composer = new Mock<ISchemaComposer>();
            composer.Setup(c => c.ComposeName(It.IsAny<TableInfo>())).Returns<TableInfo>(t => t.Name);
            composer.Setup(c => c.ComposeSchemas(It.IsAny<IEnumerable<TableSchema>>())).Returns<IEnumerable<TableSchema>>(Script);
            composer.Setup(c => c.ComposeSchema(It.IsAny<TableSchema>())).Returns<TableSchema>(s => Script(new[] { s }));
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

        #endregion

        #region Filter

        [TestMethod]
        public void TestCopyTableSchemaToWithColumnsCreatesOnlyTheGivenColumns()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person", destination, columns: new[] { "Id", "Name" });

            // Assert
            Assert.AreEqual("CREATE TABLE Person (Id,Name PK(Id) UQ UQ_Person_Name CK CK_Person_Name);", destination.ExecutedCommands.First());
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithoutColumnsCreatesAllTheColumns()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person", destination, columns: null);

            // Assert
            Assert.AreEqual("CREATE TABLE Person (Id,Name,Age,CountryId PK(Id) UQ UQ_Person_Name UQ UQ_Person_Age CK CK_Person_Age CK CK_Person_Name);", destination.ExecutedCommands.First());
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithColumnsCreatesOnlyTheIndexesThatUseTheGivenColumns()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person", destination, columns: new[] { "Id", "Name" });

            // Assert
            CollectionAssert.AreEqual(new[] { "CREATE INDEX IX_Person_Name ON Person;" }, destination.ExecutedCommands.Where(c => c.StartsWith("CREATE INDEX")).ToList());
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithColumnsCreatesTheIndexThatIncludesTheGivenColumns()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person", destination, columns: new[] { "Id", "Age" });

            // Assert
            CollectionAssert.AreEqual(new[] { "CREATE INDEX IX_Person_Id ON Person;" }, destination.ExecutedCommands.Where(c => c.StartsWith("CREATE INDEX")).ToList());
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithColumnsDoesNotCreateThePrimaryKeyIfItsColumnIsNotGiven()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person", destination, columns: new[] { "Name", "Age" });

            // Assert
            Assert.AreEqual("CREATE TABLE Person (Name,Age UQ UQ_Person_Name UQ UQ_Person_Age CK CK_Person_Age CK CK_Person_Name);", destination.ExecutedCommands.First());
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithColumnsDoesNotCreateTheCheckConstraintThatMentionsAColumnThatIsNotGiven()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person", destination, columns: new[] { "Id", "Age" });

            // Assert
            Assert.AreEqual("CREATE TABLE Person (Id,Age PK(Id) UQ UQ_Person_Age CK CK_Person_Age);", destination.ExecutedCommands.First());
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithColumnsDoesNotCreateTheForeignKeyThatUsesAColumnThatIsNotGiven()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person", destination, columns: new[] { "Id", "Name" });

            // Assert
            Assert.IsFalse(destination.ExecutedCommands.Any(c => c.StartsWith("ADD FK_Person_Country")));
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithColumnsCreatesTheForeignKeyThatUsesOnlyTheGivenColumns()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Country", "COLUMN_EXISTS Country.Id", "COLUMN_EXISTS Country.Name", "COLUMN_EXISTS Country.Code");

            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person", destination, columns: new[] { "Id", "CountryId" });

            // Assert
            CollectionAssert.Contains(destination.ExecutedCommands.ToList(), "ADD FK_Person_Country TO Person;");
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithColumnsMatchesTheNamesOfTheColumnsWithoutCaseAndWithoutDuplicates()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person", destination, columns: new[] { "ID", " name ", "Id", "NAME" });

            // Assert
            Assert.AreEqual("CREATE TABLE Person (Id,Name PK(Id) UQ UQ_Person_Name CK CK_Person_Name);", destination.ExecutedCommands.First());
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithColumnsDoesNotChangeTheSchemaOfTheSource()
        {
            // Setup
            var person = GetPerson();
            MapReader(person, GetCountry());
            MapComposer();

            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person", GetDestination(), columns: new[] { "Id" });

            // Assert
            Assert.AreEqual(4, person.Columns.Count);
            Assert.AreEqual(3, person.Indexes.Count);
            Assert.AreEqual(1, person.ForeignKeys.Count);
            Assert.IsNotNull(person.PrimaryKey);
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithColumnsReportsTheColumnsAndTheObjectsThatAreCreated()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act
            var result = new CustomDbConnection().CopyTableSchemaTo("Person", destination, columns: new[] { "Id", "Name" });

            // Assert
            Assert.AreEqual(2, result.ColumnCount);
            Assert.AreEqual(1, result.IndexCount);
            Assert.AreEqual(0, result.ForeignKeyCount);
            Assert.AreEqual(1, result.UniqueConstraintCount);
            Assert.AreEqual(1, result.CheckConstraintCount);
        }

        #endregion

        #region Errors

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToIfAColumnIsNotAColumnOfTheSourceTable()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act/Assert
            var exception = Assert.Throws<ArgumentException>(() => new CustomDbConnection().CopyTableSchemaTo("Person", destination, columns: new[] { "Id", "Missing" }));
            StringAssert.Contains(exception.Message, "'Missing'", StringComparison.Ordinal);
            StringAssert.Contains(exception.Message, "dbo.Person", StringComparison.Ordinal);
            Assert.AreEqual("columns", exception.ParamName);
            Assert.AreEqual(0, destination.ExecutedCommands.Count);
            Assert.AreEqual(0, destination.ExecutedScalars.Count);
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToIfTheColumnsAreEmpty()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act/Assert
            Assert.Throws<ArgumentException>(() => new CustomDbConnection().CopyTableSchemaTo("Person", destination, columns: new string[0]));
            Assert.AreEqual(0, destination.ExecutedCommands.Count);
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToIfAColumnIsNullOrWhiteSpace()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act/Assert
            Assert.Throws<ArgumentException>(() => new CustomDbConnection().CopyTableSchemaTo("Person", destination, columns: new[] { "Id", null }));
            Assert.Throws<ArgumentException>(() => new CustomDbConnection().CopyTableSchemaTo("Person", destination, columns: new[] { "Id", " " }));
            Assert.AreEqual(0, destination.ExecutedCommands.Count);
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToOfSeveralTablesIfAColumnIsNotAColumnOfOneOfTheTables()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act/Assert
            var exception = Assert.Throws<ArgumentException>(() => new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" }, destination, columns: new[] { "Id", "Age" }));
            StringAssert.Contains(exception.Message, "'Age'", StringComparison.Ordinal);
            StringAssert.Contains(exception.Message, "dbo.Country", StringComparison.Ordinal);
            Assert.AreEqual(0, destination.ExecutedCommands.Count);
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemaToAsyncIfAColumnIsNotAColumnOfTheSourceTable()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act/Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => new CustomDbConnection().CopyTableSchemaToAsync("Person", destination, columns: new[] { "Id", "Missing" }));
            StringAssert.Contains(exception.Message, "'Missing'", StringComparison.Ordinal);
            Assert.AreEqual(0, destination.ExecutedCommands.Count);
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemaToAsyncOfSeveralTablesIfAColumnIsNotAColumnOfOneOfTheTables()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act/Assert
            await Assert.ThrowsAsync<ArgumentException>(() => new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Person", "Country" }, destination, columns: new[] { "Id", "Age" }));
            Assert.AreEqual(0, destination.ExecutedCommands.Count);
        }

        #endregion

        #region Related Tables

        [TestMethod]
        public void TestCopyTableSchemaToWithColumnsKeepsAllTheColumnsOfTheRelatedTables()
        {
            // Setup
            MapReader(GetCountry(), GetPerson());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person", destination, columns: new[] { "Id", "CountryId" }, relationshipBehavior: CopyTableSchemaRelationshipBehavior.Parents);

            // Assert
            Assert.IsTrue(destination.ExecutedCommands.Any(c => c.StartsWith("CREATE TABLE Country (Id,Name,Code")));
            Assert.IsTrue(destination.ExecutedCommands.Any(c => c.StartsWith("CREATE TABLE Person (Id,CountryId")));
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithColumnsDoesNotCreateTheForeignKeyOfARelatedTableThatReferencesAColumnThatIsNotGiven()
        {
            // Setup
            MapReader(GetPerson(), GetPet());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person", destination, columns: new[] { "Id", "Age" }, relationshipBehavior: CopyTableSchemaRelationshipBehavior.Children);

            // Assert
            Assert.IsTrue(destination.ExecutedCommands.Any(c => c.StartsWith("CREATE TABLE Pet (Id,PersonName")));
            Assert.IsFalse(destination.ExecutedCommands.Any(c => c.StartsWith("ADD FK_Pet_Person")));
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithColumnsCreatesTheForeignKeyOfARelatedTableThatReferencesAColumnThatIsGiven()
        {
            // Setup
            MapReader(GetPerson(), GetPet());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person", destination, columns: new[] { "Id", "Name" }, relationshipBehavior: CopyTableSchemaRelationshipBehavior.Children);

            // Assert
            Assert.IsTrue(destination.ExecutedCommands.Any(c => c.StartsWith("ADD FK_Pet_Person")));
        }

        #endregion

        #region Align And Several Tables

        [TestMethod]
        public void TestCopyTableSchemaToWithColumnsAndAlignAddsOnlyTheGivenColumnsThatTheTableThatExistsIsMissing()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Person", "COLUMN_EXISTS Person.Id");

            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person", destination, columns: new[] { "Id", "Name" }, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align);

            // Assert
            Assert.IsTrue(destination.ExecutedCommands.Contains("ADD COLUMN Name TO Person;"));
            Assert.IsFalse(destination.ExecutedCommands.Any(c => c.StartsWith("ADD COLUMN Age") || c.StartsWith("ADD COLUMN CountryId")));
        }

        [TestMethod]
        public void TestCopyTableSchemaToOfSeveralTablesWithColumnsCreatesOnlyTheGivenColumnsOfEachTable()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" }, destination, columns: new[] { "Id", "Name" });

            // Assert
            Assert.IsTrue(destination.ExecutedCommands.Any(c => c.StartsWith("CREATE TABLE Person (Id,Name ")));
            Assert.IsTrue(destination.ExecutedCommands.Any(c => c.StartsWith("CREATE TABLE Country (Id,Name ")));
        }

        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncWithColumnsCreatesOnlyTheGivenColumns()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync("Person", destination, columns: new[] { "Id", "Name" });

            // Assert
            Assert.AreEqual("CREATE TABLE Person (Id,Name PK(Id) UQ UQ_Person_Name CK CK_Person_Name);", destination.ExecutedCommands.First());
            CollectionAssert.AreEqual(new[] { "CREATE INDEX IX_Person_Name ON Person;" }, destination.ExecutedCommands.Where(c => c.StartsWith("CREATE INDEX")).ToList());
        }

        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncOfSeveralTablesWithColumnsCreatesOnlyTheGivenColumnsOfEachTable()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Person", "Country" }, destination, columns: new[] { "Id", "Name" });

            // Assert
            Assert.IsTrue(destination.ExecutedCommands.Any(c => c.StartsWith("CREATE TABLE Person (Id,Name ")));
            Assert.IsTrue(destination.ExecutedCommands.Any(c => c.StartsWith("CREATE TABLE Country (Id,Name ")));
        }

        #endregion
    }
}
