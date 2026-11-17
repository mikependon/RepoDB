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
    /// Contains the copy table schemas to columns test behavior.
    /// </summary>
    [TestClass]
    public class CopyTableSchemasToColumnsTest
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
            schema.Indexes.Add(new IndexInfo("IX_Person_Name") { Columns = new List<string> { "Name" } });
            schema.Indexes.Add(new IndexInfo("IX_Person_Age_Name") { Columns = new List<string> { "Age", "Name" } });
            schema.Indexes.Add(new IndexInfo("IX_Person_Id") { Columns = new List<string> { "Id" }, IncludedColumns = new List<string> { "Age" } });
            schema.UniqueConstraints.Add(new UniqueConstraintInfo("UQ_Person_Name") { Columns = new List<string> { "Name" } });
            schema.UniqueConstraints.Add(new UniqueConstraintInfo("UQ_Person_Age") { Columns = new List<string> { "Age" } });
            schema.CheckConstraints.Add(new CheckConstraintInfo("CK_Person_Age") { Expression = "([Age] >= 0)" });
            schema.CheckConstraints.Add(new CheckConstraintInfo("CK_Person_Name") { Expression = "(len([Name]) > 0)" });
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
                "Name",
                "Code"
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
        /// Performs script.
        /// </summary>
        /// <param name="schemas">The schemas.</param>
        private static IEnumerable<string> Script(IEnumerable<TableSchema> schemas)
        {
            var list = schemas.ToList();
            return list.Select(s => $"CREATE TABLE {s.Table.Name} ({string.Join(",",
                    s.Columns.Select(c => c.Field.Name))}" + $"{(s.PrimaryKey == null ? string.Empty : " PK(" + string.Join(",",
                    s.PrimaryKey.Columns) + ")")}" + $"{string.Concat(s.UniqueConstraints.Select(u => " UQ " + u.Name))}{string.Concat(s.CheckConstraints.Select(c => " CK " + c.Name))});").Concat(list.SelectMany(s => s.Indexes.Select(i => $"CREATE INDEX {i.Name} ON {s.Table.Name};"))).Concat(list.SelectMany(s => s.ForeignKeys.Select(f => $"ADD {f.Name} TO {s.Table.Name};")));
        }

        /// <summary>
        /// Performs map composer.
        /// </summary>
        private static void MapComposer()
        {
            var composer = new Mock<ISchemaComposer>();
            composer.Setup(c => c.ComposeName(It.IsAny<TableInfo>())).Returns<TableInfo>(t => t.Name);
            composer.Setup(c => c.ComposeSchemas(It.IsAny<IEnumerable<TableSchema>>())).Returns<IEnumerable<TableSchema>>(Script);
            composer.Setup(c => c.ComposeSchema(It.IsAny<TableSchema>())).Returns<TableSchema>(s => Script(new[] { s }));
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
        /// Performs tables.
        /// </summary>
        /// <param name="tables">The tables.</param>
        private static IList<TableMapping> Tables(params (string Name, string[] Columns)[] tables) => tables.Select(table => new TableMapping(table.Name,
                null,
                table.Columns.Select(column => new ColumnMapping(column)).ToList())).ToList();

        /// <summary>
        /// Performs get destination.
        /// </summary>
        /// <param name="existing">The existing.</param>
        private static CustomDbConnection GetDestination(params string[] existing) => new CustomDbConnection
        {
            ScalarResult = statement => existing.Contains(statement) ? 1 : 0
        };
        #endregion

        #region Filter

        /// <summary>
        /// Verifies copy table schema to with columns creates only the given columns.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToWithColumnsCreatesOnlyTheGivenColumns()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            var destination = GetDestination();
            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person",
                destination,
                columns: new[] { "Id", "Name" });
            // Assert
            Assert.AreEqual("CREATE TABLE Person (Id,Name PK(Id) UQ UQ_Person_Name CK CK_Person_Name);",
                destination.ExecutedCommands.First());
        }

        /// <summary>
        /// Verifies copy table schema to without columns creates all the columns.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToWithoutColumnsCreatesAllTheColumns()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            var destination = GetDestination();
            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person",
                destination,
                columns: null);
            // Assert
            Assert.AreEqual("CREATE TABLE Person (Id,Name,Age,CountryId PK(Id) UQ UQ_Person_Name UQ UQ_Person_Age CK CK_Person_Age CK CK_Person_Name);",
                destination.ExecutedCommands.First());
        }

        /// <summary>
        /// Verifies copy table schema to with columns creates only the indexes that use the given columns.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToWithColumnsCreatesOnlyTheIndexesThatUseTheGivenColumns()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            var destination = GetDestination();
            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person",
                destination,
                columns: new[] { "Id", "Name" });
            // Assert
            CollectionAssert.AreEqual(new[] { "CREATE INDEX IX_Person_Name ON Person;" },
                destination.ExecutedCommands.Where(c => c.StartsWith("CREATE INDEX")).ToList());
        }

        /// <summary>
        /// Verifies copy table schema to with columns creates the index that includes the given columns.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToWithColumnsCreatesTheIndexThatIncludesTheGivenColumns()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            var destination = GetDestination();
            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person",
                destination,
                columns: new[] { "Id", "Age" });
            // Assert
            CollectionAssert.AreEqual(new[] { "CREATE INDEX IX_Person_Id ON Person;" },
                destination.ExecutedCommands.Where(c => c.StartsWith("CREATE INDEX")).ToList());
        }

        /// <summary>
        /// Verifies copy table schema to with columns does not create the primary key if its column is not given.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToWithColumnsDoesNotCreateThePrimaryKeyIfItsColumnIsNotGiven()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            var destination = GetDestination();
            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person",
                destination,
                columns: new[] { "Name", "Age" });
            // Assert
            Assert.AreEqual("CREATE TABLE Person (Name,Age UQ UQ_Person_Name UQ UQ_Person_Age CK CK_Person_Age CK CK_Person_Name);",
                destination.ExecutedCommands.First());
        }

        /// <summary>
        /// Verifies copy table schema to with columns does not create the check constraint that mentions acolumn that is not given.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToWithColumnsDoesNotCreateTheCheckConstraintThatMentionsAColumnThatIsNotGiven()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            var destination = GetDestination();
            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person",
                destination,
                columns: new[] { "Id", "Age" });
            // Assert
            Assert.AreEqual("CREATE TABLE Person (Id,Age PK(Id) UQ UQ_Person_Age CK CK_Person_Age);",
                destination.ExecutedCommands.First());
        }

        /// <summary>
        /// Verifies copy table schema to with columns does not create the foreign key that uses acolumn that is not given.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToWithColumnsDoesNotCreateTheForeignKeyThatUsesAColumnThatIsNotGiven()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            var destination = GetDestination();
            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person",
                destination,
                columns: new[] { "Id", "Name" });
            // Assert
            Assert.IsFalse(destination.ExecutedCommands.Any(c => c.StartsWith("ADD FK_Person_Country")));
        }

        /// <summary>
        /// Verifies copy table schema to with columns creates the foreign key that uses only the given columns.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToWithColumnsCreatesTheForeignKeyThatUsesOnlyTheGivenColumns()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Country",
                "COLUMN_EXISTS Country.Id",
                "COLUMN_EXISTS Country.Name",
                "COLUMN_EXISTS Country.Code");
            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person",
                destination,
                columns: new[] { "Id", "CountryId" });
            // Assert
            CollectionAssert.Contains(destination.ExecutedCommands.ToList(),
                "ADD FK_Person_Country TO Person;");
        }

        /// <summary>
        /// Verifies copy table schema to with columns matches the names of the columns without case and without duplicates.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToWithColumnsMatchesTheNamesOfTheColumnsWithoutCaseAndWithoutDuplicates()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            var destination = GetDestination();
            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person",
                destination,
                columns: new[] { "ID", " name ", "Id", "NAME" });
            // Assert
            Assert.AreEqual("CREATE TABLE Person (Id,Name PK(Id) UQ UQ_Person_Name CK CK_Person_Name);",
                destination.ExecutedCommands.First());
        }

        /// <summary>
        /// Verifies copy table schema to with columns does not change the schema of the source.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToWithColumnsDoesNotChangeTheSchemaOfTheSource()
        {
            // Setup
            var person = GetPerson();
            MapReader(person,
                GetCountry());
            MapComposer();
            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person",
                GetDestination(),
                columns: new[] { "Id" });
            // Assert
            Assert.AreEqual(4,
                person.Columns.Count);
            Assert.AreEqual(3,
                person.Indexes.Count);
            Assert.AreEqual(1,
                person.ForeignKeys.Count);
            Assert.IsNotNull(person.PrimaryKey);
        }

        /// <summary>
        /// Verifies copy table schema to with columns reports the columns and the objects that are created.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToWithColumnsReportsTheColumnsAndTheObjectsThatAreCreated()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            var destination = GetDestination();
            // Act
            var result = new CustomDbConnection().CopyTableSchemaTo("Person",
                destination,
                columns: new[] { "Id", "Name" }).Items.Single();
            // Assert
            Assert.AreEqual(2,
                result.ColumnCount);
            Assert.AreEqual(1,
                result.IndexCount);
            Assert.AreEqual(0,
                result.ForeignKeyCount);
            Assert.AreEqual(1,
                result.UniqueConstraintCount);
            Assert.AreEqual(1,
                result.CheckConstraintCount);
        }

        #endregion

        #region Errors

        /// <summary>
        /// Verifies throw exception on copy table schema to if acolumn is not acolumn of the source table.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToIfAColumnIsNotAColumnOfTheSourceTable()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            var destination = GetDestination();
            // Act/Assert
            var exception = Assert.Throws<ArgumentException>(() => new CustomDbConnection().CopyTableSchemaTo("Person",
                    destination,
                    columns: new[] { "Id", "Missing" }));
            StringAssert.Contains(exception.Message,
                "'Missing'",
                StringComparison.Ordinal);
            StringAssert.Contains(exception.Message,
                "dbo.Person",
                StringComparison.Ordinal);
            Assert.AreEqual("columns",
                exception.ParamName);
            Assert.AreEqual(0,
                destination.ExecutedCommands.Count);
            Assert.AreEqual(0,
                destination.ExecutedScalars.Count);
        }

        /// <summary>
        /// Verifies throw exception on copy table schema to if the columns are empty.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToIfTheColumnsAreEmpty()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            var destination = GetDestination();
            // Act/Assert
            Assert.Throws<ArgumentException>(() => new CustomDbConnection().CopyTableSchemaTo("Person",
                    destination,
                    columns: new string[0]));
            Assert.AreEqual(0,
                destination.ExecutedCommands.Count);
        }

        /// <summary>
        /// Verifies throw exception on copy table schema to if acolumn is null or white space.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToIfAColumnIsNullOrWhiteSpace()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            var destination = GetDestination();
            // Act/Assert
            Assert.Throws<ArgumentException>(() => new CustomDbConnection().CopyTableSchemaTo("Person",
                    destination,
                    columns: new[] { "Id", null }));
            Assert.Throws<ArgumentException>(() => new CustomDbConnection().CopyTableSchemaTo("Person",
                    destination,
                    columns: new[] { "Id", " " }));
            Assert.AreEqual(0,
                destination.ExecutedCommands.Count);
        }

        /// <summary>
        /// Verifies throw exception on copy table schema to of several tables if acolumn is not acolumn of one of the tables.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToOfSeveralTablesIfAColumnIsNotAColumnOfOneOfTheTables()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            var destination = GetDestination();
            // Act/Assert
            var exception = Assert.Throws<ArgumentException>(() => new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" },
                    destination,
                    Tables(("Person", new[] { "Id" }),
                        ("Country", new[] { "Id", "Age" }))));
            StringAssert.Contains(exception.Message,
                "'Age'",
                StringComparison.Ordinal);
            StringAssert.Contains(exception.Message,
                "dbo.Country",
                StringComparison.Ordinal);
            Assert.AreEqual(0,
                destination.ExecutedCommands.Count);
        }

        /// <summary>
        /// Verifies throw exception on copy table schema to async if acolumn is not acolumn of the source table.
        /// </summary>
        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemaToAsyncIfAColumnIsNotAColumnOfTheSourceTable()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            var destination = GetDestination();
            // Act/Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => new CustomDbConnection().CopyTableSchemaToAsync("Person",
                    destination,
                    columns: new[] { "Id", "Missing" }));
            StringAssert.Contains(exception.Message,
                "'Missing'",
                StringComparison.Ordinal);
            Assert.AreEqual(0,
                destination.ExecutedCommands.Count);
        }

        /// <summary>
        /// Verifies throw exception on copy table schema to async of several tables if acolumn is not acolumn of one of the tables.
        /// </summary>
        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemaToAsyncOfSeveralTablesIfAColumnIsNotAColumnOfOneOfTheTables()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            var destination = GetDestination();
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentException>(() => new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Person", "Country" },
                    destination,
                    Tables(("Person", new[] { "Id" }),
                        ("Country", new[] { "Id", "Age" }))));
            Assert.AreEqual(0,
                destination.ExecutedCommands.Count);
        }

        #endregion

        #region Related Tables

        /// <summary>
        /// Verifies copy table schema to with columns keeps all the columns of the related tables.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToWithColumnsKeepsAllTheColumnsOfTheRelatedTables()
        {
            // Setup
            MapReader(GetCountry(),
                GetPerson());
            MapComposer();
            var destination = GetDestination();
            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person",
                destination,
                columns: new[] { "Id", "CountryId" },
                relationshipBehavior: RelatedTableBehavior.Parents);
            // Assert
            Assert.IsTrue(destination.ExecutedCommands.Any(c => c.StartsWith("CREATE TABLE Country (Id,Name,Code")));
            Assert.IsTrue(destination.ExecutedCommands.Any(c => c.StartsWith("CREATE TABLE Person (Id,CountryId")));
        }

        /// <summary>
        /// Verifies copy table schema to with columns does not create the foreign key of arelated table that references acolumn that is not given.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToWithColumnsDoesNotCreateTheForeignKeyOfARelatedTableThatReferencesAColumnThatIsNotGiven()
        {
            // Setup
            MapReader(GetPerson(),
                GetPet());
            MapComposer();
            var destination = GetDestination();
            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person",
                destination,
                columns: new[] { "Id", "Age" },
                relationshipBehavior: RelatedTableBehavior.Children);
            // Assert
            Assert.IsTrue(destination.ExecutedCommands.Any(c => c.StartsWith("CREATE TABLE Pet (Id,PersonName")));
            Assert.IsFalse(destination.ExecutedCommands.Any(c => c.StartsWith("ADD FK_Pet_Person")));
        }

        /// <summary>
        /// Verifies copy table schema to with columns creates the foreign key of arelated table that references acolumn that is given.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToWithColumnsCreatesTheForeignKeyOfARelatedTableThatReferencesAColumnThatIsGiven()
        {
            // Setup
            MapReader(GetPerson(),
                GetPet());
            MapComposer();
            var destination = GetDestination();
            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person",
                destination,
                columns: new[] { "Id", "Name" },
                relationshipBehavior: RelatedTableBehavior.Children);
            // Assert
            Assert.IsTrue(destination.ExecutedCommands.Any(c => c.StartsWith("ADD FK_Pet_Person")));
        }

        #endregion

        #region Align And Several Tables

        /// <summary>
        /// Verifies copy table schema to with columns and align adds only the given columns that the table that exists is missing.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToWithColumnsAndAlignAddsOnlyTheGivenColumnsThatTheTableThatExistsIsMissing()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            var destination = GetDestination("TABLE_EXISTS Person",
                "COLUMN_EXISTS Person.Id");
            // Act
            new CustomDbConnection().CopyTableSchemaTo("Person",
                destination,
                columns: new[] { "Id", "Name" },
                tableExistenceBehavior: TableExistsBehavior.Align);
            // Assert
            Assert.IsTrue(destination.ExecutedCommands.Contains("ADD COLUMN Name TO Person;"));
            Assert.IsFalse(destination.ExecutedCommands.Any(c => c.StartsWith("ADD COLUMN Age") || c.StartsWith("ADD COLUMN CountryId")));
        }

        /// <summary>
        /// Verifies copy table schema to of several tables with table mappings creates only the given columns of each table.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToOfSeveralTablesWithTableMappingsCreatesOnlyTheGivenColumnsOfEachTable()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            var destination = GetDestination();
            // Act
            new CustomDbConnection().CopyTableSchemaTo(new[] { "Person", "Country" },
                destination,
                Tables(("Person", new[] { "Id", "Name" }),
                    ("Country", new[] { "Id", "Name" })));
            // Assert
            Assert.IsTrue(destination.ExecutedCommands.Any(c => c.StartsWith("CREATE TABLE Person (Id,Name ")));
            Assert.IsTrue(destination.ExecutedCommands.Any(c => c.StartsWith("CREATE TABLE Country (Id,Name ")));
        }

        /// <summary>
        /// Verifies copy table schema to async with columns creates only the given columns.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncWithColumnsCreatesOnlyTheGivenColumns()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            var destination = GetDestination();
            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync("Person",
                destination,
                columns: new[] { "Id", "Name" });
            // Assert
            Assert.AreEqual("CREATE TABLE Person (Id,Name PK(Id) UQ UQ_Person_Name CK CK_Person_Name);",
                destination.ExecutedCommands.First());
            CollectionAssert.AreEqual(new[] { "CREATE INDEX IX_Person_Name ON Person;" },
                destination.ExecutedCommands.Where(c => c.StartsWith("CREATE INDEX")).ToList());
        }

        /// <summary>
        /// Verifies copy table schema to async of several tables with table mappings creates only the given columns of each table.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncOfSeveralTablesWithTableMappingsCreatesOnlyTheGivenColumnsOfEachTable()
        {
            // Setup
            MapReader(GetPerson(),
                GetCountry());
            MapComposer();
            var destination = GetDestination();
            // Act
            await new CustomDbConnection().CopyTableSchemaToAsync(new[] { "Person", "Country" },
                destination,
                Tables(("Person", new[] { "Id", "Name" }),
                    ("Country", new[] { "Id", "Name" })));
            // Assert
            Assert.IsTrue(destination.ExecutedCommands.Any(c => c.StartsWith("CREATE TABLE Person (Id,Name ")));
            Assert.IsTrue(destination.ExecutedCommands.Any(c => c.StartsWith("CREATE TABLE Country (Id,Name ")));
        }
        #endregion
    }
}
