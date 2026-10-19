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
    public class CopySchemasToRenamedColumnsTest
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
            reader.Setup(r => r.GetRelatedTables(It.IsAny<IEnumerable<string>>(), It.IsAny<CopySchemaRelationshipBehavior>())).Returns<IEnumerable<string>, CopySchemaRelationshipBehavior>((names, _) => schemas.Select(s => s.Table.Name).ToList());
            reader.Setup(r => r.GetRelatedTablesAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CopySchemaRelationshipBehavior>(), It.IsAny<CancellationToken>())).Returns<IEnumerable<string>, CopySchemaRelationshipBehavior, CancellationToken>((names, _, __) => Task.FromResult<IEnumerable<string>>(schemas.Select(s => s.Table.Name).ToList()));
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

        #region Rename

        [TestMethod]
        public void TestCopySchemaToWithRenamedColumnsCreatesTheColumnsByTheirNewNames()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopySchemaTo("Person", destination, renamedColumns: new Dictionary<string, string> { ["Name"] = "FullName" });

            // Assert
            StringAssert.StartsWith(Created(destination, "Person"), "CREATE Person(Id,FullName,Age,CountryId)");
        }

        [TestMethod]
        public void TestCopySchemaToWithRenamedColumnsMakesTheObjectsFollowTheNewNames()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopySchemaTo("Person", destination, renamedColumns: new Dictionary<string, string> { ["Id"] = "PersonId", ["Name"] = "FullName", ["Age"] = "Years", ["CountryId"] = "Country_Id" });

            // Assert
            Assert.AreEqual("CREATE Person(PersonId,FullName,Years,Country_Id) PK(PersonId) IX[FullName|Years|FullName] UQ[FullName] CK[([Years] >= 0)] FK[Country_Id->Country(Id)];", Created(destination, "Person"));
        }

        [TestMethod]
        public void TestCopySchemaToWithRenamedColumnsMatchesTheNamesWithoutCase()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopySchemaTo("Person", destination, renamedColumns: new Dictionary<string, string> { ["name"] = "FullName", ["AGE"] = "Years" });

            // Assert
            StringAssert.StartsWith(Created(destination, "Person"), "CREATE Person(Id,FullName,Years,CountryId)");
        }

        [TestMethod]
        public void TestCopySchemaToWithRenamedColumnsCanSwapTheNamesOfTwoColumns()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopySchemaTo("Person", destination, renamedColumns: new Dictionary<string, string> { ["Name"] = "Age", ["Age"] = "Name" });

            // Assert
            Assert.AreEqual("CREATE Person(Id,Age,Name,CountryId) PK(Id) IX[Age|Name|Age] UQ[Age] CK[([Name] >= 0)] FK[CountryId->Country(Id)];", Created(destination, "Person"));
        }

        [TestMethod]
        public void TestCopySchemaToWithColumnsAndRenamedColumnsCreatesOnlyTheGivenColumnsByTheirNewNames()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopySchemaTo("Person", destination, columns: new[] { "Id", "Name" }, renamedColumns: new Dictionary<string, string> { ["Name"] = "FullName" });

            // Assert
            Assert.AreEqual("CREATE Person(Id,FullName) PK(Id) UQ[FullName];", Created(destination, "Person"));
        }

        [TestMethod]
        public void TestCopySchemaToWithoutRenamedColumnsKeepsTheNames()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopySchemaTo("Person", destination, renamedColumns: null);
            new CustomDbConnection().CopySchemaTo("Person", GetDestination(), renamedColumns: new Dictionary<string, string>());

            // Assert
            StringAssert.StartsWith(Created(destination, "Person"), "CREATE Person(Id,Name,Age,CountryId)");
        }

        [TestMethod]
        public void TestCopySchemaToWithRenamedColumnsMakesTheForeignKeyOfARelatedTableFollowTheNewNames()
        {
            // Setup
            MapReader(GetPerson(), GetPet());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopySchemaTo("Person", destination, relationshipBehavior: CopySchemaRelationshipBehavior.Children, renamedColumns: new Dictionary<string, string> { ["Name"] = "FullName" });

            // Assert
            Assert.AreEqual("CREATE Pet(Id,PersonName) PK() FK[PersonName->Person(FullName)];", Created(destination, "Pet"));
        }

        [TestMethod]
        public void TestCopySchemaToWithRenamedColumnsDoesNotRenameTheColumnsOfTheRelatedTables()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopySchemaTo("Person", destination, relationshipBehavior: CopySchemaRelationshipBehavior.Parents, renamedColumns: new Dictionary<string, string> { ["Name"] = "FullName", ["Id"] = "PersonId" });

            // Assert
            Assert.AreEqual("CREATE Country(Id,Name) PK(Id);", Created(destination, "Country"));
            StringAssert.Contains(Created(destination, "Person"), "FK[CountryId->Country(Id)]");
        }

        [TestMethod]
        public void TestCopySchemaToWithRenamedColumnsDoesNotChangeTheSchemaOfTheSource()
        {
            // Setup
            var person = GetPerson();
            MapReader(person, GetCountry());
            MapComposer();

            // Act
            new CustomDbConnection().CopySchemaTo("Person", GetDestination(), renamedColumns: new Dictionary<string, string> { ["Name"] = "FullName" });

            // Assert
            CollectionAssert.AreEqual(new[] { "Id", "Name", "Age", "CountryId" }, person.Columns.Select(c => c.Field.Name).ToList());
            CollectionAssert.AreEqual(new[] { "Name" }, person.Indexes.Single().Columns.ToList());
        }

        [TestMethod]
        public void TestCopySchemaToWithRenamedColumnsKeepsTheOtherPropertiesOfTheColumns()
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
            new CustomDbConnection().CopySchemaTo("Person", GetDestination(), renamedColumns: new Dictionary<string, string> { ["Name"] = "FullName" });

            // Assert
            var column = composed.First().Columns.Single(c => c.Field.Name == "FullName");
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
        public void TestCopySchemaToOfSeveralTablesWithRenamedColumnsRenamesTheColumnsOfEachTable()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act
            new CustomDbConnection().CopySchemaTo(new[] { "Person", "Country" }, destination, renamedColumns: new Dictionary<string, string> { ["Name"] = "FullName" });

            // Assert
            StringAssert.StartsWith(Created(destination, "Person"), "CREATE Person(Id,FullName,Age,CountryId)");
            Assert.AreEqual("CREATE Country(Id,FullName) PK(Id);", Created(destination, "Country"));
        }

        [TestMethod]
        public async Task TestCopySchemaToAsyncWithRenamedColumnsCreatesTheColumnsByTheirNewNames()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act
            await new CustomDbConnection().CopySchemaToAsync("Person", destination, renamedColumns: new Dictionary<string, string> { ["Name"] = "FullName" });

            // Assert
            StringAssert.StartsWith(Created(destination, "Person"), "CREATE Person(Id,FullName,Age,CountryId)");
        }

        [TestMethod]
        public async Task TestCopySchemaToAsyncWithColumnsAndRenamedColumnsCreatesOnlyTheGivenColumnsByTheirNewNames()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();
            var destination = GetDestination();

            // Act
            await new CustomDbConnection().CopySchemaToAsync("Person", destination, columns: new[] { "Id", "Name" }, renamedColumns: new Dictionary<string, string> { ["Name"] = "FullName" });

            // Assert
            Assert.AreEqual("CREATE Person(Id,FullName) PK(Id) UQ[FullName];", Created(destination, "Person"));
        }

        #endregion

        #region Errors

        [TestMethod]
        public void ThrowExceptionOnCopySchemaToIfARenamedColumnIsNotAColumnOfTheSourceTable()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();

            // Act/Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                new CustomDbConnection().CopySchemaTo("Person", GetDestination(), renamedColumns: new Dictionary<string, string> { ["Missing"] = "Other" }));
            StringAssert.Contains(ex.Message, "Missing");
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemaToIfTheNewNameOfAColumnIsNullOrWhiteSpace()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();

            // Act/Assert
            Assert.Throws<ArgumentException>(() =>
                new CustomDbConnection().CopySchemaTo("Person", GetDestination(), renamedColumns: new Dictionary<string, string> { ["Name"] = " " }));
            Assert.Throws<ArgumentException>(() =>
                new CustomDbConnection().CopySchemaTo("Person", GetDestination(), renamedColumns: new Dictionary<string, string> { ["Name"] = null }));
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemaToIfAColumnIsRenamedMoreThanOnce()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();

            // Act/Assert
            Assert.Throws<ArgumentException>(() =>
                new CustomDbConnection().CopySchemaTo("Person", GetDestination(), renamedColumns: new Dictionary<string, string> { ["Name"] = "A", ["name"] = "B" }));
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemaToIfTheNewNamesMakeTheTableHaveTheSameColumnTwice()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();

            // Act/Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                new CustomDbConnection().CopySchemaTo("Person", GetDestination(), renamedColumns: new Dictionary<string, string> { ["Name"] = "Age" }));
            StringAssert.Contains(ex.Message, "Age");
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopySchemaToAsyncIfARenamedColumnIsNotAColumnOfTheSourceTable()
        {
            // Setup
            MapReader(GetPerson(), GetCountry());
            MapComposer();

            // Act/Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                new CustomDbConnection().CopySchemaToAsync("Person", GetDestination(), renamedColumns: new Dictionary<string, string> { ["Missing"] = "Other" }));
        }

        #endregion
    }
}
