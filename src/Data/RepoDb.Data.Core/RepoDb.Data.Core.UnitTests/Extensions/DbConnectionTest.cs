#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Data.Core.UnitTests.CustomObjects;
using RepoDb.Data.Core.UnitTests.Fake.BulkOperations;
using RepoDb.Data.Enumerations;
using RepoDb.Data.Models;
using Endpoint = RepoDb.Schema.Models.Endpoint;
using ColumnMapping = RepoDb.Data.Models.ColumnMapping;
using RepoDb.DbSettings;
using RepoDb.Enumerations;
using RepoDb.Interfaces;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;
using DataDbConnection = RepoDb.Data.DbConnection;

namespace RepoDb.Data.Core.UnitTests.Extensions
{
    /// <summary>
    /// Contains the db connection test behavior.
    /// </summary>
    [TestClass]
    public class DbConnectionTest
    {
        #region Initialize

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

        #endregion

        #region Private Methods

        /// <summary>
        /// Performs get connection.
        /// </summary>
        private static IDbConnection GetConnection() => new Mock<IDbConnection>().Object;

        /// <summary>
        /// Performs get schema.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="schema">The schema.</param>
        private static TableSchema GetSchema(string tableName,
            string schema = "dbo") => new TableSchema(tableName,
            schema)
        {
            Columns =
            {
                new ColumnInfo()
            }
        };

        /// <summary>
        /// Performs create relationship.
        /// </summary>
        /// <param name="schema">The schema.</param>
        private static RelationshipInfo CreateRelationship(TableSchema schema)
        {
            var relationship = new RelationshipInfo();
            typeof(RelationshipInfo).GetProperty(nameof(RelationshipInfo.Schema)).SetValue(relationship,
                schema);
            return relationship;
        }

        /// <summary>
        /// Performs map reader.
        /// </summary>
        /// <param name="related">The related.</param>
        /// <param name="ordered">The ordered.</param>
        private static Mock<ISchemaReader> MapReader(string[] related,
            params string[] ordered)
        {
            var relationships = ordered.Select(name => CreateRelationship(GetSchema(name))).ToList();
            var reader = new Mock<ISchemaReader>();
            reader.Setup(r => r.GetRelatedTables(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<RelatedTableBehavior>(),
                    It.IsAny<System.Data.IDbTransaction>())).Returns(related);
            reader.Setup(r => r.GetRelatedTablesAsync(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<RelatedTableBehavior>(),
                    It.IsAny<System.Data.IDbTransaction>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(related);
            reader.Setup(r => r.GetDependencyOrder(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<System.Data.IDbTransaction>())).Returns(relationships);
            reader.Setup(r => r.GetDependencyOrderAsync(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<System.Data.IDbTransaction>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(relationships);
            reader.Setup(r => r.GetTableSchema(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<System.Data.IDbTransaction>())).Returns<System.Data.IDbConnection, string, System.Data.IDbTransaction>((operationConnection,
                    name,
                    operationTransaction) => GetSchema(name));
            reader.Setup(r => r.GetTableSchemaAsync(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<System.Data.IDbTransaction>(),
                    It.IsAny<CancellationToken>())).Returns<System.Data.IDbConnection, string, System.Data.IDbTransaction, CancellationToken>((operationConnection,
                    name,
                    operationTransaction,
                    _) => Task.FromResult(GetSchema(name)));
            SchemaReaderMapper.Add<FakeDbConnection>(reader.Object,
                true);
            return reader;
        }

        /// <summary>
        /// Performs create result.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="sourceSchema">The source schema.</param>
        /// <param name="targetSchema">The target schema.</param>
        private static CopyTableSchemaResultItem CreateResult(string table,
            string sourceSchema,
            string targetSchema) => new CopyTableSchemaResultItem
        {
            Source = new Endpoint
            {
                Table = new TableSchema(table,
                    sourceSchema)
            },
            Target = new Endpoint
            {
                Table = new TableSchema(table,
                    targetSchema)
            }
        };

        /// <summary>
        /// Performs get schema of person.
        /// </summary>
        private static TableSchema GetSchemaOfPerson()
        {
            var schema = new TableSchema("Person",
                "dbo");
            foreach (var name in new[]
            {
                "Id",
                "Name",
                "Birthday"
            }

            )
            {
                var column = new ColumnInfo();
                typeof(ColumnInfo).GetProperty(nameof(ColumnInfo.Field)).SetValue(column,
                    new DbField(name,
                        false,
                        false,
                        true,
                        typeof(string),
                        null,
                        null,
                        null,
                        null));
                schema.Columns.Add(column);
            }

            return schema;
        }

        /// <summary>
        /// Performs map columns of person.
        /// </summary>
        private static List<string[]> MapColumnsOfPerson()
        {
            var composed = new List<string[]>();
            var reader = MapReader(new[] { "Person" },
                "Person");
            reader.Setup(r => r.GetTableSchema(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<System.Data.IDbTransaction>())).Returns<System.Data.IDbConnection, string, System.Data.IDbTransaction>((operationConnection,
                    _,
                    operationTransaction) => GetSchemaOfPerson());
            reader.Setup(r => r.GetDependencyOrder(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<System.Data.IDbTransaction>())).Returns(() => new[] { CreateRelationship(GetSchemaOfPerson()) }.ToList());
            reader.Setup(r => r.GetDependencyOrderAsync(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<System.Data.IDbTransaction>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(() => new[] { CreateRelationship(GetSchemaOfPerson()) }.ToList());
            reader.Setup(r => r.GetTableSchemaAsync(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<System.Data.IDbTransaction>(),
                    It.IsAny<CancellationToken>())).Returns<System.Data.IDbConnection, string, System.Data.IDbTransaction, CancellationToken>((operationConnection,
                    _,
                    operationTransaction,
                    __) => Task.FromResult(GetSchemaOfPerson()));
            var composer = new Mock<ISchemaComposer>();
            composer.Setup(c => c.ComposeSchemas(It.IsAny<IEnumerable<TableSchema>>())).Returns<IEnumerable<TableSchema>>(schemas =>
            {
                composed.AddRange(schemas.Select(x => x.Columns.Select(c => c.Field.Name).ToArray()));
                return schemas.Select(x => $"CREATE TABLE {x.Table.Name};").ToList();
            });
            composer.Setup(c => c.ComposeSchema(It.IsAny<TableSchema>())).Returns<TableSchema>(x =>
            {
                composed.Add(x.Columns.Select(c => c.Field.Name).ToArray());
                return new[]
                {
                    $"CREATE TABLE {x.Table.Name};"};
            });
            SchemaComposerMapper.Add<FakeDbConnection>(composer.Object,
                true);
            return composed;
        }

        /// <summary>
        /// Performs map composer.
        /// </summary>
        private static Mock<ISchemaComposer> MapComposer()
        {
            var composer = new Mock<ISchemaComposer>();
            composer.Setup(c => c.ComposeSchemas(It.IsAny<IEnumerable<TableSchema>>())).Returns<IEnumerable<TableSchema>>(schemas => schemas.Select(s => $"CREATE TABLE {s.Table.Name};").ToList());
            composer.Setup(c => c.ComposeSchema(It.IsAny<TableSchema>())).Returns<TableSchema>(s => new[] { $"CREATE TABLE {s.Table.Name};" });
            SchemaComposerMapper.Add<FakeDbConnection>(composer.Object,
                true);
            return composer;
        }

        #endregion

        #region IsSchemaRequested

        /// <summary>
        /// Verifies is schema requested is false with the default behaviors and no target schema.
        /// </summary>
        [TestMethod]
        public void TestIsSchemaRequestedIsFalseWithTheDefaultBehaviorsAndNoTargetSchema()
        {
            // Act/Assert
            Assert.IsFalse(DataDbConnection.IsSchemaRequested(RelationshipBehavior.TableOnly,
                    TableExistsBehavior.Skip,
                    null));
            Assert.IsFalse(DataDbConnection.IsSchemaRequested(RelationshipBehavior.TableOnly,
                    TableExistsBehavior.Skip,
                    string.Empty));
            Assert.IsFalse(DataDbConnection.IsSchemaRequested(RelationshipBehavior.TableOnly,
                    TableExistsBehavior.Skip,
                    "  "));
        }

        /// <summary>
        /// Verifies is schema requested is true with arelationship behavior other than table only.
        /// </summary>
        [TestMethod]
        public void TestIsSchemaRequestedIsTrueWithARelationshipBehaviorOtherThanTableOnly()
        {
            foreach (var behavior in new[]
            {
                RelationshipBehavior.Parents,
                RelationshipBehavior.Children,
                RelationshipBehavior.ParentsAndChildren
            }

            )
            {
                // Act/Assert
                Assert.IsTrue(DataDbConnection.IsSchemaRequested(behavior,
                        TableExistsBehavior.Skip,
                        null));
            }
        }

        /// <summary>
        /// Verifies is schema requested is true with atable existence behavior other than skip.
        /// </summary>
        [TestMethod]
        public void TestIsSchemaRequestedIsTrueWithATableExistenceBehaviorOtherThanSkip()
        {
            foreach (var behavior in new[]
            {
                TableExistsBehavior.Align,
                TableExistsBehavior.Throw,
                TableExistsBehavior.Drop
            }

            )
            {
                // Act/Assert
                Assert.IsTrue(DataDbConnection.IsSchemaRequested(RelationshipBehavior.TableOnly,
                        behavior,
                        null));
            }
        }

        /// <summary>
        /// Verifies is schema requested is true with atarget schema.
        /// </summary>
        [TestMethod]
        public void TestIsSchemaRequestedIsTrueWithATargetSchema()
        {
            // Act/Assert
            Assert.IsTrue(DataDbConnection.IsSchemaRequested(RelationshipBehavior.TableOnly,
                    TableExistsBehavior.Skip,
                    "Sales"));
        }

        #endregion

        #region ToSchemaBehavior

        /// <summary>
        /// Verifies to schema behavior maps every relationship behavior.
        /// </summary>
        [TestMethod]
        public void TestToSchemaBehaviorMapsEveryRelationshipBehavior()
        {
            // Act/Assert
            Assert.AreEqual(RelatedTableBehavior.TableOnly,
                DataDbConnection.ToSchemaBehavior(RelationshipBehavior.TableOnly));
            Assert.AreEqual(RelatedTableBehavior.Parents,
                DataDbConnection.ToSchemaBehavior(RelationshipBehavior.Parents));
            Assert.AreEqual(RelatedTableBehavior.Children,
                DataDbConnection.ToSchemaBehavior(RelationshipBehavior.Children));
            Assert.AreEqual(RelatedTableBehavior.ParentsAndChildren,
                DataDbConnection.ToSchemaBehavior(RelationshipBehavior.ParentsAndChildren));
        }

        /// <summary>
        /// Verifies throw exception on to schema behavior if the relationship behavior is not defined.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnToSchemaBehaviorIfTheRelationshipBehaviorIsNotDefined()
        {
            // Act/Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => DataDbConnection.ToSchemaBehavior((RelationshipBehavior)99));
        }

        #endregion

        #region GetName

        /// <summary>
        /// Verifies get name quotes the schema and the table.
        /// </summary>
        [TestMethod]
        public void TestGetNameQuotesTheSchemaAndTheTable()
        {
            // Act
            var actual = DataDbConnection.GetName(new FakeDbConnection(),
                "Sales",
                "Person");
            // Assert
            Assert.AreEqual("[Sales].[Person]",
                actual);
        }

        /// <summary>
        /// Verifies get name without aschema quotes the table only.
        /// </summary>
        [TestMethod]
        public void TestGetNameWithoutASchemaQuotesTheTableOnly()
        {
            // Act/Assert
            Assert.AreEqual("[Person]",
                DataDbConnection.GetName(new FakeDbConnection(),
                    null,
                    "Person"));
            Assert.AreEqual("[Person]",
                DataDbConnection.GetName(new FakeDbConnection(),
                    string.Empty,
                    "Person"));
            Assert.AreEqual("[Person]",
                DataDbConnection.GetName(new FakeDbConnection(),
                    " ",
                    "Person"));
        }

        /// <summary>
        /// Verifies get name of acopy table schema result item uses the target schema and the table name.
        /// </summary>
        [TestMethod]
        public void TestGetNameOfACopyTableSchemaResultItemUsesTheTargetSchemaAndTheTableName()
        {
            // Setup
            var result = CreateResult("Person",
                "dbo",
                "Sales");
            // Act
            var actual = DataDbConnection.GetName(new FakeDbConnection(),
                result);
            // Assert
            Assert.AreEqual("[Sales].[Person]",
                actual);
        }

        #endregion

        #region GetRenamedTable

        /// <summary>
        /// Verifies get renamed table returns the new simple name only when it is not the name of the source table.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="expected">The expected.</param>
        [DataTestMethod]
        [DataRow("Person", "Human", "Human")]
        [DataRow("dbo.Person", "Sales.Human", "Human")]
        [DataRow("Person", "[Human]", "Human")]
        [DataRow("Person", "person", null)]
        [DataRow("dbo.Person", "[Sales].[PERSON]", null)]
        [DataRow("Person", null, null)]
        [DataRow("Person", " ", null)]
        public void TestGetRenamedTableReturnsTheNewSimpleNameOnlyWhenItIsNotTheNameOfTheSourceTable(string source,
            string target,
            string expected)
        {
            // Act
            var actual = DataDbConnection.GetRenamedTable(source,
                target);
            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        #endregion

        #region GetTargetSchema

        /// <summary>
        /// Verifies get target schema returns the schema of the target table.
        /// </summary>
        [TestMethod]
        public void TestGetTargetSchemaReturnsTheSchemaOfTheTargetTable()
        {
            // Act/Assert
            Assert.AreEqual("Sales",
                DataDbConnection.GetTargetSchema(new FakeDbConnection(),
                    "Sales.Person"));
            Assert.AreEqual("Sales",
                DataDbConnection.GetTargetSchema(new FakeDbConnection(),
                    "[Sales].[Person]"));
        }

        /// <summary>
        /// Verifies get target schema returns null if the target table has no schema.
        /// </summary>
        [TestMethod]
        public void TestGetTargetSchemaReturnsNullIfTheTargetTableHasNoSchema()
        {
            // Act/Assert
            Assert.IsNull(DataDbConnection.GetTargetSchema(new FakeDbConnection(),
                    "Person"));
            Assert.IsNull(DataDbConnection.GetTargetSchema(new FakeDbConnection(),
                    "[Person]"));
        }

        /// <summary>
        /// Verifies get target schema returns null if there is no destination connection or no target table.
        /// </summary>
        [TestMethod]
        public void TestGetTargetSchemaReturnsNullIfThereIsNoDestinationConnectionOrNoTargetTable()
        {
            // Act/Assert
            Assert.IsNull(DataDbConnection.GetTargetSchema(null,
                    "Sales.Person"));
            Assert.IsNull(DataDbConnection.GetTargetSchema(new FakeDbConnection(),
                    null));
            Assert.IsNull(DataDbConnection.GetTargetSchema(new FakeDbConnection(),
                    " "));
        }

        #endregion

        #region GetTableName

        /// <summary>
        /// Verifies get table name returns the name without the schema and the quotes.
        /// </summary>
        [TestMethod]
        public void TestGetTableNameReturnsTheNameWithoutTheSchemaAndTheQuotes()
        {
            // Act/Assert
            Assert.AreEqual("Person",
                DataDbConnection.GetTableName(new FakeDbConnection(),
                    "Person"));
            Assert.AreEqual("Person",
                DataDbConnection.GetTableName(new FakeDbConnection(),
                    "[Person]"));
            Assert.AreEqual("Person",
                DataDbConnection.GetTableName(new FakeDbConnection(),
                    "Sales.Person"));
            Assert.AreEqual("Person",
                DataDbConnection.GetTableName(new FakeDbConnection(),
                    "[Sales].[Person]"));
        }

        #endregion

        #region ToTables

        /// <summary>
        /// Verifies to tables keeps the source name and the filter of the requested table.
        /// </summary>
        [TestMethod]
        public void TestToTablesKeepsTheSourceNameAndTheFilterOfTheRequestedTable()
        {
            // Setup
            var where = new QueryGroup(new QueryField("Id",
                    1));
            var relationships = new[]
            {
                CreateRelationship(GetSchema("Person"))
            };
            var results = new[]
            {
                CreateResult("Person",
                    "dbo",
                    "Sales")
            };
            // Act
            var actual = DataDbConnection.ToTables(relationships,
                new TableInfo("Person",
                    "dbo"),
                ("Person", "Customer", where),
                results,
                new FakeDbConnection(),
                new FakeDbConnection());
            // Assert
            Assert.AreEqual(1,
                actual.Count);
            Assert.AreEqual("Person",
                actual[0].Source);
            Assert.AreEqual("[Sales].[Person]",
                actual[0].Target);
            Assert.AreSame(where,
                actual[0].Where);
        }

        /// <summary>
        /// Verifies to tables copies the related tables as awhole and in the given order.
        /// </summary>
        [TestMethod]
        public void TestToTablesCopiesTheRelatedTablesAsAWholeAndInTheGivenOrder()
        {
            // Setup
            var where = new QueryGroup(new QueryField("Id",
                    1));
            var relationships = new[]
            {
                CreateRelationship(GetSchema("Country")),
                CreateRelationship(GetSchema("Person"))
            };
            var results = new[]
            {
                CreateResult("Country",
                    "dbo",
                    "Sales"),
                CreateResult("Person",
                    "dbo",
                    "Sales")
            };
            // Act
            var actual = DataDbConnection.ToTables(relationships,
                new TableInfo("Person",
                    "dbo"),
                ("Person", "Person", where),
                results,
                new FakeDbConnection(),
                new FakeDbConnection());
            // Assert
            Assert.AreEqual(2,
                actual.Count);
            Assert.AreEqual("[dbo].[Country]",
                actual[0].Source);
            Assert.AreEqual("[Sales].[Country]",
                actual[0].Target);
            Assert.IsNull(actual[0].Where);
            Assert.AreEqual("Person",
                actual[1].Source);
            Assert.AreEqual("[Sales].[Person]",
                actual[1].Target);
            Assert.AreSame(where,
                actual[1].Where);
        }

        /// <summary>
        /// Verifies to tables matches the result of atable by its schema and its name.
        /// </summary>
        [TestMethod]
        public void TestToTablesMatchesTheResultOfATableByItsSchemaAndItsName()
        {
            // Setup
            var relationships = new[]
            {
                CreateRelationship(GetSchema("Person",
                        "Sales")),
                CreateRelationship(GetSchema("Person",
                        "Hr"))
            };
            var results = new[]
            {
                CreateResult("Person",
                    "Sales",
                    "Target1"),
                CreateResult("Person",
                    "Hr",
                    "Target2")
            };
            // Act
            var actual = DataDbConnection.ToTables(relationships,
                new TableInfo("Person",
                    "Sales"),
                ("Person", "Person", null),
                results,
                new FakeDbConnection(),
                new FakeDbConnection());
            // Assert
            Assert.AreEqual("[Target1].[Person]",
                actual[0].Target);
            Assert.AreEqual("[Target2].[Person]",
                actual[1].Target);
            Assert.AreEqual("[Hr].[Person]",
                actual[1].Source);
        }

        #endregion

        #region GetTables

        /// <summary>
        /// Verifies get tables without the schema requested returns the requested table only.
        /// </summary>
        [TestMethod]
        public void TestGetTablesWithoutTheSchemaRequestedReturnsTheRequestedTableOnly()
        {
            // Setup
            var where = new QueryGroup(new QueryField("Id",
                    1));
            // Act
            var actual = DataDbConnection.GetTables(GetConnection(),
                GetConnection(),
                "Person",
                "Customer",
                null,
                where,
                RelationshipBehavior.TableOnly,
                TableExistsBehavior.Skip,
                null,
                null,
                null);
            // Assert
            Assert.AreEqual(1,
                actual.Count);
            Assert.AreEqual("Person",
                actual[0].Source);
            Assert.AreEqual("Customer",
                actual[0].Target);
            Assert.AreSame(where,
                actual[0].Where);
        }

        /// <summary>
        /// Verifies get tables async without the schema requested returns the requested table only.
        /// </summary>
        [TestMethod]
        public async Task TestGetTablesAsyncWithoutTheSchemaRequestedReturnsTheRequestedTableOnly()
        {
            // Setup
            var where = new QueryGroup(new QueryField("Id",
                    1));
            // Act
            var actual = await DataDbConnection.GetTablesAsync(GetConnection(),
                GetConnection(),
                "Person",
                "Customer",
                null,
                where,
                RelationshipBehavior.TableOnly,
                TableExistsBehavior.Skip,
                null,
                null,
                null,
                CancellationToken.None);
            // Assert
            Assert.AreEqual(1,
                actual.Count);
            Assert.AreEqual("Person",
                actual[0].Source);
            Assert.AreEqual("Customer",
                actual[0].Target);
            Assert.AreSame(where,
                actual[0].Where);
        }

        /// <summary>
        /// Verifies get tables with atarget schema copies the schema of the table and returns its name in the target schema.
        /// </summary>
        [TestMethod]
        public void TestGetTablesWithATargetSchemaCopiesTheSchemaOfTheTableAndReturnsItsNameInTheTargetSchema()
        {
            // Setup
            var reader = MapReader(new[] { "Person" },
                "Person");
            var composer = MapComposer();
            var destination = new FakeDbConnection();
            var where = new QueryGroup(new QueryField("Id",
                    1));
            // Act
            var actual = DataDbConnection.GetTables(new FakeDbConnection(),
                destination,
                "Person",
                "Person",
                "Sales",
                where,
                RelationshipBehavior.TableOnly,
                TableExistsBehavior.Skip,
                null,
                null,
                null);
            // Assert
            Assert.AreEqual(1,
                actual.Count);
            Assert.AreEqual("Person",
                actual[0].Source);
            Assert.AreEqual("[Sales].[Person]",
                actual[0].Target);
            Assert.AreSame(where,
                actual[0].Where);
            reader.Verify(r => r.GetRelatedTables(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<RelatedTableBehavior>(),
                    It.IsAny<System.Data.IDbTransaction>()),
                Times.Never);
            composer.Verify(c => c.ComposeSchemas(It.IsAny<IEnumerable<TableSchema>>()),
                Times.Once);
            Assert.IsTrue(destination.Executions.Any(e => e.Kind == "NonQuery" && e.CommandText == "CREATE TABLE Person;"));
        }

        /// <summary>
        /// Verifies get tables async with atarget schema copies the schema of the table and returns its name in the target schema.
        /// </summary>
        [TestMethod]
        public async Task TestGetTablesAsyncWithATargetSchemaCopiesTheSchemaOfTheTableAndReturnsItsNameInTheTargetSchema()
        {
            // Setup
            MapReader(new[] { "Person" },
                "Person");
            MapComposer();
            var destination = new FakeDbConnection();
            // Act
            var actual = await DataDbConnection.GetTablesAsync(new FakeDbConnection(),
                destination,
                "Person",
                "Person",
                "Sales",
                null,
                RelationshipBehavior.TableOnly,
                TableExistsBehavior.Skip,
                null,
                null,
                null,
                CancellationToken.None);
            // Assert
            Assert.AreEqual(1,
                actual.Count);
            Assert.AreEqual("Person",
                actual[0].Source);
            Assert.AreEqual("[Sales].[Person]",
                actual[0].Target);
            Assert.IsNull(actual[0].Where);
        }

        /// <summary>
        /// Verifies get tables with arelationship behavior returns the related tables in the dependency order.
        /// </summary>
        [TestMethod]
        public void TestGetTablesWithARelationshipBehaviorReturnsTheRelatedTablesInTheDependencyOrder()
        {
            // Setup
            var reader = MapReader(new[] { "Person", "Country" },
                "Country",
                "Person");
            MapComposer();
            var where = new QueryGroup(new QueryField("Id",
                    1));
            // Act
            var actual = DataDbConnection.GetTables(new FakeDbConnection(),
                new FakeDbConnection(),
                "Person",
                "Person",
                null,
                where,
                RelationshipBehavior.Parents,
                TableExistsBehavior.Skip,
                null,
                null,
                null);
            // Assert
            Assert.AreEqual(2,
                actual.Count);
            Assert.AreEqual("[dbo].[Country]",
                actual[0].Source);
            Assert.AreEqual("[dbo].[Country]",
                actual[0].Target);
            Assert.IsNull(actual[0].Where);
            Assert.AreEqual("Person",
                actual[1].Source);
            Assert.AreEqual("[dbo].[Person]",
                actual[1].Target);
            Assert.AreSame(where,
                actual[1].Where);
            reader.Verify(r => r.GetRelatedTables(It.IsAny<System.Data.IDbConnection>(),
                    It.Is<IEnumerable<string>>(n => n.SequenceEqual(new[] { "Person" })),
                    RelatedTableBehavior.Parents,
                    It.IsAny<System.Data.IDbTransaction>()),
                Times.AtLeastOnce);
        }

        /// <summary>
        /// Verifies get tables async with arelationship behavior returns the related tables in the dependency order.
        /// </summary>
        [TestMethod]
        public async Task TestGetTablesAsyncWithARelationshipBehaviorReturnsTheRelatedTablesInTheDependencyOrder()
        {
            // Setup
            var reader = MapReader(new[] { "Person", "Country" },
                "Country",
                "Person");
            MapComposer();
            var where = new QueryGroup(new QueryField("Id",
                    1));
            // Act
            var actual = await DataDbConnection.GetTablesAsync(new FakeDbConnection(),
                new FakeDbConnection(),
                "Person",
                "Person",
                null,
                where,
                RelationshipBehavior.Children,
                TableExistsBehavior.Skip,
                null,
                null,
                null,
                CancellationToken.None);
            // Assert
            Assert.AreEqual(2,
                actual.Count);
            Assert.AreEqual("[dbo].[Country]",
                actual[0].Source);
            Assert.IsNull(actual[0].Where);
            Assert.AreEqual("Person",
                actual[1].Source);
            Assert.AreSame(where,
                actual[1].Where);
            reader.Verify(r => r.GetRelatedTablesAsync(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<IEnumerable<string>>(),
                    RelatedTableBehavior.Children,
                    It.IsAny<System.Data.IDbTransaction>(),
                    It.IsAny<CancellationToken>()),
                Times.AtLeastOnce);
        }

        /// <summary>
        /// Verifies get tables passes the command timeout and the trace to the schema copy.
        /// </summary>
        [TestMethod]
        public void TestGetTablesPassesTheCommandTimeoutAndTheTraceToTheSchemaCopy()
        {
            // Setup
            MapReader(new[] { "Person" },
                "Person");
            MapComposer();
            var destination = new FakeDbConnection();
            // Act
            DataDbConnection.GetTables(new FakeDbConnection(),
                destination,
                "Person",
                "Person",
                "Sales",
                null,
                RelationshipBehavior.TableOnly,
                TableExistsBehavior.Skip,
                77,
                null,
                null);
            // Assert
            Assert.IsTrue(destination.Executions.Any(e => e.CommandTimeout == 77));
        }

        /// <summary>
        /// Verifies get tables with mappings creates the target table with only the mapped columns.
        /// </summary>
        [TestMethod]
        public void TestGetTablesWithMappingsCreatesTheTargetTableWithOnlyTheMappedColumns()
        {
            // Setup
            var composed = MapColumnsOfPerson();
            // Act
            DataDbConnection.GetTables(new FakeDbConnection(),
                new FakeDbConnection(),
                "Person",
                "Person",
                "Sales",
                null,
                RelationshipBehavior.TableOnly,
                TableExistsBehavior.Skip,
                null,
                null,
                null,
                new[] { new ColumnMapping("Name",
                    "Name"), new ColumnMapping("name",
                    "Alias") });
            // Assert
            CollectionAssert.AreEqual(new[] { "Name" },
                composed.First());
        }

        /// <summary>
        /// Verifies get tables async with mappings creates the target table with only the mapped columns.
        /// </summary>
        [TestMethod]
        public async Task TestGetTablesAsyncWithMappingsCreatesTheTargetTableWithOnlyTheMappedColumns()
        {
            // Setup
            var composed = MapColumnsOfPerson();
            // Act
            await DataDbConnection.GetTablesAsync(new FakeDbConnection(),
                new FakeDbConnection(),
                "Person",
                "Person",
                "Sales",
                null,
                RelationshipBehavior.TableOnly,
                TableExistsBehavior.Skip,
                null,
                null,
                null,
                CancellationToken.None,
                new[] { new ColumnMapping("Id",
                    "Id"), new ColumnMapping("Birthday",
                    "Birthday") });
            // Assert
            CollectionAssert.AreEquivalent(new[] { "Id", "Birthday" },
                composed.First());
        }

        /// <summary>
        /// Verifies get tables without mappings creates the target table with all the columns.
        /// </summary>
        [TestMethod]
        public void TestGetTablesWithoutMappingsCreatesTheTargetTableWithAllTheColumns()
        {
            // Setup
            var composed = MapColumnsOfPerson();
            // Act
            DataDbConnection.GetTables(new FakeDbConnection(),
                new FakeDbConnection(),
                "Person",
                "Person",
                "Sales",
                null,
                RelationshipBehavior.TableOnly,
                TableExistsBehavior.Skip,
                null,
                null,
                null);
            // Assert
            CollectionAssert.AreEquivalent(new[] { "Id", "Name", "Birthday" },
                composed.First());
        }

        /// <summary>
        /// Verifies throw exception on get tables if amapped source column does not exist on the source table.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnGetTablesIfAMappedSourceColumnDoesNotExistOnTheSourceTable()
        {
            // Setup
            MapColumnsOfPerson();
            // Act/Assert
            Assert.Throws<ArgumentException>(() => DataDbConnection.GetTables(new FakeDbConnection(),
                    new FakeDbConnection(),
                    "Person",
                    "Person",
                    "Sales",
                    null,
                    RelationshipBehavior.TableOnly,
                    TableExistsBehavior.Skip,
                    null,
                    null,
                    null,
                    new[] { new ColumnMapping("Missing",
                        "Missing") }));
        }

        /// <summary>
        /// Verifies get tables with mappings creates the target table with the destination column names.
        /// </summary>
        [TestMethod]
        public void TestGetTablesWithMappingsCreatesTheTargetTableWithTheDestinationColumnNames()
        {
            // Setup
            var composed = MapColumnsOfPerson();
            // Act
            DataDbConnection.GetTables(new FakeDbConnection(),
                new FakeDbConnection(),
                "Person",
                "Person",
                "Sales",
                null,
                RelationshipBehavior.TableOnly,
                TableExistsBehavior.Skip,
                null,
                null,
                null,
                new[] { new ColumnMapping("Id",
                    "PersonId"), new ColumnMapping("Name",
                    "FullName") });
            // Assert
            CollectionAssert.AreEqual(new[] { "PersonId", "FullName" },
                composed.First());
        }

        /// <summary>
        /// Verifies get tables async with mappings creates the target table with the destination column names.
        /// </summary>
        [TestMethod]
        public async Task TestGetTablesAsyncWithMappingsCreatesTheTargetTableWithTheDestinationColumnNames()
        {
            // Setup
            var composed = MapColumnsOfPerson();
            // Act
            await DataDbConnection.GetTablesAsync(new FakeDbConnection(),
                new FakeDbConnection(),
                "Person",
                "Person",
                "Sales",
                null,
                RelationshipBehavior.TableOnly,
                TableExistsBehavior.Skip,
                null,
                null,
                null,
                CancellationToken.None,
                new[] { new ColumnMapping("Id",
                    "PersonId"), new ColumnMapping("Name",
                    "FullName") });
            // Assert
            CollectionAssert.AreEqual(new[] { "PersonId", "FullName" },
                composed.First());
        }

        /// <summary>
        /// Verifies get tables with asource column mapped more than once creates the column with the first destination column.
        /// </summary>
        [TestMethod]
        public void TestGetTablesWithASourceColumnMappedMoreThanOnceCreatesTheColumnWithTheFirstDestinationColumn()
        {
            // Setup
            var composed = MapColumnsOfPerson();
            // Act
            DataDbConnection.GetTables(new FakeDbConnection(),
                new FakeDbConnection(),
                "Person",
                "Person",
                "Sales",
                null,
                RelationshipBehavior.TableOnly,
                TableExistsBehavior.Skip,
                null,
                null,
                null,
                new[] { new ColumnMapping("Name",
                    "Name"), new ColumnMapping("Name",
                    "Alias") });
            // Assert
            CollectionAssert.AreEqual(new[] { "Name" },
                composed.First());
        }

        #endregion

        #region CopyRows

        /// <summary>
        /// Performs get connections.
        /// </summary>
        /// <param name="rows">The rows.</param>
        private static (FakeDbConnection Source, FakeBulkKeyedDbConnection Destination) GetConnections(int rows = 3)
        {
            var source = new FakeDbConnection
            {
                Data = FakeDbConnection.CreatePersons(rows)
            };
            return (source, FakeBulkDestination.Create());
        }

        /// <summary>
        /// Performs get fallback connections.
        /// </summary>
        /// <param name="rows">The rows.</param>
        private static (FakeDbConnection Source, FakeDbConnection Destination) GetFallbackConnections(int rows = 3)
        {
            var source = new FakeDbConnection
            {
                Data = FakeDbConnection.CreatePersons(rows)
            };
            return (source, new FakeDbConnection());
        }

        /// <summary>
        /// Verifies copy rows with abulk insert reads the source table and bulk inserts the rows as abatch.
        /// </summary>
        [TestMethod]
        public void TestCopyRowsWithABulkInsertReadsTheSourceTableAndBulkInsertsTheRowsAsABatch()
        {
            // Setup
            var source = new FakeDbConnection();
            var destination = new FakeBulkDbConnection();
            var options = new CopyProgress();
            var reported = new List<(int Batch, int Rows, int Total)>();
            FakeBulkOperations.Reset();
            // Act
            DataDbConnection.CopyRows(source,
                destination,
                "Person",
                "Customer",
                null,
                500,
                false,
                null,
                ColumnMappingBehavior.Automatic,
                IdentityBehavior.KeepIdentity,
                null,
                null,
                options,
                p => reported.Add((p.BatchCount, p.CurrentBatch.RowCount, p.RowCount)),
                33,
                "MyTraceKey",
                null,
                null);
            // Assert
            var call = FakeBulkOperations.Calls.Single();
            Assert.AreEqual("BulkInsert",
                call.Method);
            Assert.AreEqual("Customer",
                call.TableName);
            Assert.AreEqual(500,
                call.BatchSize);
            Assert.AreEqual(33,
                call.Timeout);
            Assert.AreEqual(3,
                call.RowCount);
            Assert.AreEqual(1,
                source.Executions.Count);
            Assert.AreEqual("Reader",
                source.Executions[0].Kind);
            Assert.AreEqual(33,
                source.Executions[0].CommandTimeout);
            CollectionAssert.AreEqual(new[] { (1, 3, 3) },
                reported);
            Assert.AreEqual(1,
                options.BatchCount);
            Assert.AreEqual(3,
                options.RowCount);
            Assert.AreNotEqual(default(DateTime),
                options.EndTime);
        }

        /// <summary>
        /// Verifies copy rows async with abulk insert reads the source table and bulk inserts the rows as abatch.
        /// </summary>
        [TestMethod]
        public async Task TestCopyRowsAsyncWithABulkInsertReadsTheSourceTableAndBulkInsertsTheRowsAsABatch()
        {
            // Setup
            var source = new FakeDbConnection();
            var destination = new FakeBulkDbConnection();
            var options = new CopyProgress();
            var reported = new List<(int Batch, int Rows, int Total)>();
            using (var tokenSource = new CancellationTokenSource())
            {
                FakeBulkOperations.Reset();
                // Act
                await DataDbConnection.CopyRowsAsync(source,
                    destination,
                    "Person",
                    "Customer",
                    null,
                    500,
                    false,
                    null,
                    ColumnMappingBehavior.Automatic,
                    IdentityBehavior.KeepIdentity,
                    null,
                    null,
                    options,
                    p => reported.Add((p.BatchCount, p.CurrentBatch.RowCount, p.RowCount)),
                    33,
                    null,
                    null,
                    null,
                    tokenSource.Token);
                // Assert
                var call = FakeBulkOperations.Calls.Single();
                Assert.AreEqual("BulkInsertAsync",
                    call.Method);
                Assert.AreEqual("Customer",
                    call.TableName);
                Assert.AreEqual(500,
                    call.BatchSize);
                Assert.AreEqual(33,
                    call.Timeout);
                Assert.AreEqual(3,
                    call.RowCount);
                Assert.AreEqual(tokenSource.Token,
                    call.CancellationToken);
                Assert.AreEqual("Reader",
                    source.Executions.Single().Kind);
                CollectionAssert.AreEqual(new[] { (1, 3, 3) },
                    reported);
            }
        }

        /// <summary>
        /// Verifies copy rows without abulk insert inserts the rows in batches.
        /// </summary>
        [TestMethod]
        public void TestCopyRowsWithoutABulkInsertInsertsTheRowsInBatches()
        {
            // Setup
            var (source, destination) = GetFallbackConnections(5);
            var options = new CopyProgress();
            var reported = new List<(int Batch, int Rows, int Total)>();
            // Act
            DataDbConnection.CopyRows(source,
                destination,
                "Person",
                "Customer",
                null,
                2,
                false,
                null,
                ColumnMappingBehavior.Automatic,
                IdentityBehavior.KeepIdentity,
                null,
                null,
                options,
                p => reported.Add((p.BatchCount, p.CurrentBatch.RowCount, p.RowCount)),
                null,
                null,
                null,
                null);
            // Assert
            Assert.AreEqual(5,
                destination.Executions.Count(e => e.Kind == "NonQuery" && e.CommandText.StartsWith("INSERT",
                        StringComparison.Ordinal)));
            CollectionAssert.AreEqual(new[] { (1, 2, 2), (2, 2, 4), (3, 1, 5) },
                reported);
            Assert.AreEqual(3,
                options.BatchCount);
            Assert.AreEqual(5,
                options.RowCount);
        }

        /// <summary>
        /// Verifies copy rows async without abulk insert inserts the rows in batches.
        /// </summary>
        [TestMethod]
        public async Task TestCopyRowsAsyncWithoutABulkInsertInsertsTheRowsInBatches()
        {
            // Setup
            var (source, destination) = GetFallbackConnections(5);
            var options = new CopyProgress();
            var reported = new List<(int Batch, int Rows, int Total)>();
            // Act
            await DataDbConnection.CopyRowsAsync(source,
                destination,
                "Person",
                "Customer",
                null,
                2,
                false,
                null,
                ColumnMappingBehavior.Automatic,
                IdentityBehavior.KeepIdentity,
                null,
                null,
                options,
                p => reported.Add((p.BatchCount, p.CurrentBatch.RowCount, p.RowCount)),
                null,
                null,
                null,
                null,
                CancellationToken.None);
            // Assert
            Assert.AreEqual(5,
                destination.Executions.Count(e => e.Kind == "NonQuery" && e.CommandText.StartsWith("INSERT",
                        StringComparison.Ordinal)));
            CollectionAssert.AreEqual(new[] { (1, 2, 2), (2, 2, 4), (3, 1, 5) },
                reported);
            Assert.AreEqual(5,
                options.RowCount);
        }

        /// <summary>
        /// Verifies copy rows without abulk insert and an exact number of batches does not insert an empty batch.
        /// </summary>
        [TestMethod]
        public void TestCopyRowsWithoutABulkInsertAndAnExactNumberOfBatchesDoesNotInsertAnEmptyBatch()
        {
            // Setup
            var (source, destination) = GetFallbackConnections(4);
            var options = new CopyProgress();
            // Act
            DataDbConnection.CopyRows(source,
                destination,
                "Person",
                "Customer",
                null,
                2,
                false,
                null,
                ColumnMappingBehavior.Automatic,
                IdentityBehavior.KeepIdentity,
                null,
                null,
                options,
                null,
                null,
                null,
                null,
                null);
            // Assert
            Assert.AreEqual(2,
                options.BatchCount);
            Assert.AreEqual(4,
                options.RowCount);
        }

        /// <summary>
        /// Verifies copy rows with bypass existing rows bulk merges the rows with the primary key as the qualifiers.
        /// </summary>
        [TestMethod]
        public void TestCopyRowsWithBypassExistingRowsBulkMergesTheRowsWithThePrimaryKeyAsTheQualifiers()
        {
            // Setup
            var (source, destination) = GetConnections();
            var options = new CopyProgress();
            FakeBulkOperations.Reset();
            // Act
            DataDbConnection.CopyRows(source,
                destination,
                "Person",
                "Customer",
                null,
                500,
                true,
                null,
                ColumnMappingBehavior.Automatic,
                IdentityBehavior.KeepIdentity,
                null,
                null,
                options,
                null,
                33,
                null,
                null,
                null);
            // Assert
            var call = FakeBulkOperations.Calls.Single();
            Assert.AreEqual("BulkMerge",
                call.Method);
            Assert.AreEqual("Customer",
                call.TableName);
            Assert.AreEqual(500,
                call.BatchSize);
            Assert.AreEqual(3,
                call.RowCount);
            CollectionAssert.AreEqual(new[] { "Id" },
                call.Qualifiers);
            Assert.AreEqual(3,
                options.RowCount);
            Assert.IsNull(options.Items.Single().SkippedRowCount);
        }

        /// <summary>
        /// Verifies copy rows async with bypass existing rows bulk merges the rows with the primary key as the qualifiers.
        /// </summary>
        [TestMethod]
        public async Task TestCopyRowsAsyncWithBypassExistingRowsBulkMergesTheRowsWithThePrimaryKeyAsTheQualifiers()
        {
            // Setup
            var (source, destination) = GetConnections();
            var options = new CopyProgress();
            FakeBulkOperations.Reset();
            // Act
            await DataDbConnection.CopyRowsAsync(source,
                destination,
                "Person",
                "Customer",
                null,
                500,
                true,
                null,
                ColumnMappingBehavior.Automatic,
                IdentityBehavior.KeepIdentity,
                null,
                null,
                options,
                null,
                33,
                null,
                null,
                null,
                CancellationToken.None);
            // Assert
            var call = FakeBulkOperations.Calls.Single();
            Assert.AreEqual("BulkMergeAsync",
                call.Method);
            CollectionAssert.AreEqual(new[] { "Id" },
                call.Qualifiers);
            Assert.AreEqual(3,
                options.RowCount);
        }

        /// <summary>
        /// Verifies copy rows with bypass existing rows bulk merges the rows with the identity as the qualifier if there is no primary key.
        /// </summary>
        [TestMethod]
        public void TestCopyRowsWithBypassExistingRowsBulkMergesTheRowsWithTheIdentityAsTheQualifierIfThereIsNoPrimaryKey()
        {
            // Setup
            var source = new FakeDbConnection();
            var destination = FakeBulkDestination.CreateIdentityOnly();
            FakeBulkOperations.Reset();
            // Act
            DataDbConnection.CopyRows(source,
                destination,
                "Person",
                "Customer",
                null,
                500,
                true,
                null,
                ColumnMappingBehavior.Automatic,
                IdentityBehavior.KeepIdentity,
                null,
                null,
                new CopyProgress(),
                null,
                null,
                null,
                null,
                null);
            // Assert
            CollectionAssert.AreEqual(new[] { "Id" },
                FakeBulkOperations.Calls.Single().Qualifiers);
        }

        /// <summary>
        /// Verifies copy rows without bypass existing rows bulk inserts the rows.
        /// </summary>
        [TestMethod]
        public void TestCopyRowsWithoutBypassExistingRowsBulkInsertsTheRows()
        {
            // Setup
            var (source, destination) = GetConnections();
            FakeBulkOperations.Reset();
            // Act
            DataDbConnection.CopyRows(source,
                destination,
                "Person",
                "Customer",
                null,
                500,
                false,
                null,
                ColumnMappingBehavior.Automatic,
                IdentityBehavior.KeepIdentity,
                null,
                null,
                new CopyProgress(),
                null,
                null,
                null,
                null,
                null);
            // Assert
            var call = FakeBulkOperations.Calls.Single();
            Assert.AreEqual("BulkInsert",
                call.Method);
            Assert.IsNull(call.Qualifiers);
        }

        /// <summary>
        /// Verifies throw exception on copy rows with bypass existing rows if the target table has no primary key and no identity.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyRowsWithBypassExistingRowsIfTheTargetTableHasNoPrimaryKeyAndNoIdentity()
        {
            // Setup
            var source = new FakeDbConnection();
            var destination = FakeBulkDestination.CreateKeyless();
            FakeBulkOperations.Reset();
            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => DataDbConnection.CopyRows(source,
                    destination,
                    "Person",
                    "Customer",
                    null,
                    500,
                    true,
                    null,
                    ColumnMappingBehavior.Automatic,
                    IdentityBehavior.KeepIdentity,
                    null,
                    null,
                    new CopyProgress(),
                    null,
                    null,
                    null,
                    null,
                    null));
            Assert.AreEqual(0,
                FakeBulkOperations.Calls.Length);
        }

        /// <summary>
        /// Verifies throw exception on copy rows async with bypass existing rows if the target table has no primary key and no identity.
        /// </summary>
        [TestMethod]
        public async Task ThrowExceptionOnCopyRowsAsyncWithBypassExistingRowsIfTheTargetTableHasNoPrimaryKeyAndNoIdentity()
        {
            // Setup
            var source = new FakeDbConnection();
            var destination = FakeBulkDestination.CreateKeyless();
            FakeBulkOperations.Reset();
            // Act/Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => DataDbConnection.CopyRowsAsync(source,
                    destination,
                    "Person",
                    "Customer",
                    null,
                    500,
                    true,
                    null,
                    ColumnMappingBehavior.Automatic,
                    IdentityBehavior.KeepIdentity,
                    null,
                    null,
                    new CopyProgress(),
                    null,
                    null,
                    null,
                    null,
                    null,
                    CancellationToken.None));
            Assert.AreEqual(0,
                FakeBulkOperations.Calls.Length);
        }

        /// <summary>
        /// Verifies throw exception on copy rows with bypass existing rows if the qualifiers are generated by the database.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyRowsWithBypassExistingRowsIfTheQualifiersAreGeneratedByTheDatabase()
        {
            // Setup
            var (source, destination) = GetConnections();
            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => DataDbConnection.CopyRows(source,
                    destination,
                    "Person",
                    "Customer",
                    null,
                    500,
                    true,
                    null,
                    ColumnMappingBehavior.Automatic,
                    IdentityBehavior.Ignore,
                    null,
                    null,
                    new CopyProgress(),
                    null,
                    null,
                    null,
                    null,
                    null));
        }

        /// <summary>
        /// Verifies copy rows with bypass existing rows merges the rows one by one if the provider has no bulk merge.
        /// </summary>
        [TestMethod]
        public void TestCopyRowsWithBypassExistingRowsMergesTheRowsOneByOneIfTheProviderHasNoBulkMerge()
        {
            // Setup
            var (source, destination) = GetFallbackConnections();
            // Act
            DataDbConnection.CopyRows(source,
                destination,
                "Person",
                "Customer",
                null,
                500,
                true,
                null,
                ColumnMappingBehavior.Automatic,
                IdentityBehavior.KeepIdentity,
                null,
                null,
                new CopyProgress(),
                null,
                null,
                null,
                null,
                null);
            // Assert
            var updates = destination.Executions.Where(e => e.CommandText.StartsWith("UPDATE",
                    StringComparison.Ordinal)).ToList();
            Assert.AreEqual(3,
                updates.Count);
            StringAssert.Contains(updates[0].CommandText,
                "WHERE [Id] = ");
            Assert.AreEqual(0,
                destination.Executions.Count(e => e.CommandText.StartsWith("INSERT INTO",
                        StringComparison.Ordinal)));
        }

        /// <summary>
        /// Verifies copy rows async with bypass existing rows merges the rows one by one if the provider has no bulk merge.
        /// </summary>
        [TestMethod]
        public async Task TestCopyRowsAsyncWithBypassExistingRowsMergesTheRowsOneByOneIfTheProviderHasNoBulkMerge()
        {
            // Setup
            var (source, destination) = GetFallbackConnections();
            // Act
            await DataDbConnection.CopyRowsAsync(source,
                destination,
                "Person",
                "Customer",
                null,
                500,
                true,
                new[] { "Name" },
                ColumnMappingBehavior.Automatic,
                IdentityBehavior.KeepIdentity,
                null,
                null,
                new CopyProgress(),
                null,
                null,
                null,
                null,
                null,
                CancellationToken.None);
            // Assert
            var updates = destination.Executions.Where(e => e.CommandText.StartsWith("UPDATE",
                    StringComparison.Ordinal)).ToList();
            Assert.AreEqual(3,
                updates.Count);
            StringAssert.Contains(updates[0].CommandText,
                "WHERE [Name] = ");
        }

        /// <summary>
        /// Verifies copy rows with qualifiers bulk merges the rows with the qualifiers.
        /// </summary>
        [TestMethod]
        public void TestCopyRowsWithQualifiersBulkMergesTheRowsWithTheQualifiers()
        {
            // Setup
            var (source, destination) = GetConnections();
            FakeBulkOperations.Reset();
            // Act
            DataDbConnection.CopyRows(source,
                destination,
                "Person",
                "Customer",
                null,
                500,
                true,
                new[] { "name", "Birthday" },
                ColumnMappingBehavior.Automatic,
                IdentityBehavior.KeepIdentity,
                null,
                null,
                new CopyProgress(),
                null,
                null,
                null,
                null,
                null);
            // Assert
            var call = FakeBulkOperations.Calls.Single();
            Assert.AreEqual("BulkMerge",
                call.Method);
            CollectionAssert.AreEqual(new[] { "Name", "Birthday" },
                call.Qualifiers);
        }

        /// <summary>
        /// Verifies copy rows async with qualifiers bulk merges the rows of atable without primary key and identity.
        /// </summary>
        [TestMethod]
        public async Task TestCopyRowsAsyncWithQualifiersBulkMergesTheRowsOfATableWithoutPrimaryKeyAndIdentity()
        {
            // Setup
            var source = new FakeDbConnection();
            var destination = FakeBulkDestination.CreateKeyless();
            FakeBulkOperations.Reset();
            // Act
            await DataDbConnection.CopyRowsAsync(source,
                destination,
                "Person",
                "Customer",
                null,
                500,
                true,
                new[] { "Id" },
                ColumnMappingBehavior.Automatic,
                IdentityBehavior.KeepIdentity,
                null,
                null,
                new CopyProgress(),
                null,
                null,
                null,
                null,
                null,
                CancellationToken.None);
            // Assert
            CollectionAssert.AreEqual(new[] { "Id" },
                FakeBulkOperations.Calls.Single().Qualifiers);
        }

        /// <summary>
        /// Verifies copy rows ignores the qualifiers if the existing rows are not bypassed.
        /// </summary>
        [TestMethod]
        public void TestCopyRowsIgnoresTheQualifiersIfTheExistingRowsAreNotBypassed()
        {
            // Setup
            var (source, destination) = GetConnections();
            FakeBulkOperations.Reset();
            // Act
            DataDbConnection.CopyRows(source,
                destination,
                "Person",
                "Customer",
                null,
                500,
                false,
                new[] { "Name" },
                ColumnMappingBehavior.Automatic,
                IdentityBehavior.KeepIdentity,
                null,
                null,
                new CopyProgress(),
                null,
                null,
                null,
                null,
                null);
            // Assert
            var call = FakeBulkOperations.Calls.Single();
            Assert.AreEqual("BulkInsert",
                call.Method);
            Assert.IsNull(call.Qualifiers);
        }

        /// <summary>
        /// Verifies throw exception on copy rows with bypass existing rows if aqualifier is not acolumn of the target table.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyRowsWithBypassExistingRowsIfAQualifierIsNotAColumnOfTheTargetTable()
        {
            // Setup
            var (source, destination) = GetConnections();
            // Act/Assert
            Assert.Throws<ArgumentException>(() => DataDbConnection.CopyRows(source,
                    destination,
                    "Person",
                    "Customer",
                    null,
                    500,
                    true,
                    new[] { "Missing" },
                    ColumnMappingBehavior.Automatic,
                    IdentityBehavior.KeepIdentity,
                    null,
                    null,
                    new CopyProgress(),
                    null,
                    null,
                    null,
                    null,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on copy rows with bypass existing rows if aqualifier is empty.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyRowsWithBypassExistingRowsIfAQualifierIsEmpty()
        {
            // Setup
            var (source, destination) = GetConnections();
            // Act/Assert
            Assert.Throws<ArgumentException>(() => DataDbConnection.CopyRows(source,
                    destination,
                    "Person",
                    "Customer",
                    null,
                    500,
                    true,
                    new[] { "Id", " " },
                    ColumnMappingBehavior.Automatic,
                    IdentityBehavior.KeepIdentity,
                    null,
                    null,
                    new CopyProgress(),
                    null,
                    null,
                    null,
                    null,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on copy rows with bypass existing rows if aqualifier is the identity that is generated by the database.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyRowsWithBypassExistingRowsIfAQualifierIsTheIdentityThatIsGeneratedByTheDatabase()
        {
            // Setup
            var (source, destination) = GetConnections();
            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => DataDbConnection.CopyRows(source,
                    destination,
                    "Person",
                    "Customer",
                    null,
                    500,
                    true,
                    new[] { "Id" },
                    ColumnMappingBehavior.Automatic,
                    IdentityBehavior.Ignore,
                    null,
                    null,
                    new CopyProgress(),
                    null,
                    null,
                    null,
                    null,
                    null));
        }

        /// <summary>
        /// Verifies copy rows gives the column mappings behavior to the bulk insert.
        /// </summary>
        [TestMethod]
        public void TestCopyRowsGivesTheColumnMappingsBehaviorToTheBulkInsert()
        {
            // Setup
            var destination = new FakeBulkSettingDbConnection();
            DbSettingMapper.Add<FakeBulkSettingDbConnection>(new FakeBulkDbSetting(),
                true);
            FakeBulkOperations.Reset();
            // Act
            DataDbConnection.CopyRows(new FakeDbConnection(),
                destination,
                "Person",
                "Customer",
                null,
                500,
                false,
                null,
                ColumnMappingBehavior.Strict,
                IdentityBehavior.KeepIdentity,
                null,
                null,
                new CopyProgress(),
                null,
                null,
                null,
                null,
                null);
            // Assert
            Assert.AreEqual("Strict",
                FakeBulkOperations.Calls.Single().ColumnMappingsBehavior);
        }

        /// <summary>
        /// Verifies copy rows async gives the column mappings behavior to the bulk insert.
        /// </summary>
        [TestMethod]
        public async Task TestCopyRowsAsyncGivesTheColumnMappingsBehaviorToTheBulkInsert()
        {
            // Setup
            var destination = new FakeBulkSettingDbConnection();
            DbSettingMapper.Add<FakeBulkSettingDbConnection>(new FakeBulkDbSetting(),
                true);
            FakeBulkOperations.Reset();
            // Act
            await DataDbConnection.CopyRowsAsync(new FakeDbConnection(),
                destination,
                "Person",
                "Customer",
                null,
                500,
                false,
                null,
                ColumnMappingBehavior.StrictBypass,
                IdentityBehavior.KeepIdentity,
                null,
                null,
                new CopyProgress(),
                null,
                null,
                null,
                null,
                null,
                CancellationToken.None);
            // Assert
            Assert.AreEqual("StrictBypass",
                FakeBulkOperations.Calls.Single().ColumnMappingsBehavior);
        }

        /// <summary>
        /// Verifies copy rows without abulk insert ignores the column mappings behavior.
        /// </summary>
        [TestMethod]
        public void TestCopyRowsWithoutABulkInsertIgnoresTheColumnMappingsBehavior()
        {
            // Setup
            var (source, destination) = GetFallbackConnections(3);
            var options = new CopyProgress();
            // Act
            DataDbConnection.CopyRows(source,
                destination,
                "Person",
                "Customer",
                null,
                2,
                false,
                null,
                ColumnMappingBehavior.Strict,
                IdentityBehavior.KeepIdentity,
                null,
                null,
                options,
                null,
                null,
                null,
                null,
                null);
            // Assert
            Assert.AreEqual(3,
                options.RowCount);
            Assert.AreEqual(3,
                destination.Executions.Count(e => e.Kind == "NonQuery" && e.CommandText.StartsWith("INSERT",
                        StringComparison.Ordinal)));
        }

        /// <summary>
        /// Verifies copy rows async without abulk insert ignores the column mappings behavior.
        /// </summary>
        [TestMethod]
        public async Task TestCopyRowsAsyncWithoutABulkInsertIgnoresTheColumnMappingsBehavior()
        {
            // Setup
            var (source, destination) = GetFallbackConnections(3);
            var options = new CopyProgress();
            // Act
            await DataDbConnection.CopyRowsAsync(source,
                destination,
                "Person",
                "Customer",
                null,
                2,
                false,
                null,
                ColumnMappingBehavior.StrictBypass,
                IdentityBehavior.KeepIdentity,
                null,
                null,
                options,
                null,
                null,
                null,
                null,
                null,
                CancellationToken.None);
            // Assert
            Assert.AreEqual(3,
                options.RowCount);
        }

        /// <summary>
        /// Verifies copy rows without rows inserts nothing.
        /// </summary>
        [TestMethod]
        public void TestCopyRowsWithoutRowsInsertsNothing()
        {
            // Setup
            var (source, destination) = GetFallbackConnections(0);
            var options = new CopyProgress();
            var called = false;
            // Act
            DataDbConnection.CopyRows(source,
                destination,
                "Person",
                "Customer",
                null,
                2,
                false,
                null,
                ColumnMappingBehavior.Automatic,
                IdentityBehavior.KeepIdentity,
                null,
                null,
                options,
                p => called = true,
                null,
                null,
                null,
                null);
            // Assert
            Assert.IsFalse(called);
            Assert.AreEqual(0,
                options.BatchCount);
            Assert.AreEqual(0,
                options.RowCount);
            Assert.AreEqual(0,
                destination.Executions.Count);
        }

        /// <summary>
        /// Verifies copy rows async without rows inserts nothing.
        /// </summary>
        [TestMethod]
        public async Task TestCopyRowsAsyncWithoutRowsInsertsNothing()
        {
            // Setup
            var (source, destination) = GetFallbackConnections(0);
            var options = new CopyProgress();
            // Act
            await DataDbConnection.CopyRowsAsync(source,
                destination,
                "Person",
                "Customer",
                null,
                2,
                false,
                null,
                ColumnMappingBehavior.Automatic,
                IdentityBehavior.KeepIdentity,
                null,
                null,
                options,
                null,
                null,
                null,
                null,
                null,
                CancellationToken.None);
            // Assert
            Assert.AreEqual(0,
                options.BatchCount);
            Assert.AreEqual(0,
                destination.Executions.Count);
        }

        /// <summary>
        /// Verifies copy rows without abulk insert inserts the columns of the rows.
        /// </summary>
        [TestMethod]
        public void TestCopyRowsWithoutABulkInsertInsertsTheColumnsOfTheRows()
        {
            // Setup
            var (source, destination) = GetFallbackConnections(3);
            // Act
            DataDbConnection.CopyRows(source,
                destination,
                "Person",
                "Customer",
                null,
                3,
                false,
                null,
                ColumnMappingBehavior.Automatic,
                IdentityBehavior.KeepIdentity,
                null,
                null,
                new CopyProgress(),
                null,
                null,
                null,
                null,
                null);
            // Assert
            var insert = destination.Executions.First(e => e.Kind == "NonQuery" && e.CommandText.StartsWith("INSERT",
                    StringComparison.Ordinal));
            StringAssert.Contains(insert.CommandText,
                "[Customer]");
            StringAssert.Contains(insert.CommandText,
                "[Name]");
            StringAssert.Contains(insert.CommandText,
                "[Birthday]");
            Assert.IsTrue(insert.Parameters.Values.Contains("Name 1"));
        }

        /// <summary>
        /// Verifies copy rows without abulk insert inserts the null values of the rows as null.
        /// </summary>
        [TestMethod]
        public void TestCopyRowsWithoutABulkInsertInsertsTheNullValuesOfTheRowsAsNull()
        {
            // Setup
            var (source, destination) = GetFallbackConnections(3);
            // Act
            DataDbConnection.CopyRows(source,
                destination,
                "Person",
                "Customer",
                null,
                3,
                false,
                null,
                ColumnMappingBehavior.Automatic,
                IdentityBehavior.KeepIdentity,
                null,
                null,
                new CopyProgress(),
                null,
                null,
                null,
                null,
                null);
            // Assert
            var withoutBirthday = destination.Executions.Where(e => e.Kind == "NonQuery" && e.CommandText.StartsWith("INSERT",
                    StringComparison.Ordinal)).Single(e => e.Parameters.Values.Contains("Name 2"));
            Assert.IsFalse(withoutBirthday.Parameters.Values.Any(v => v is DateTime));
        }

        /// <summary>
        /// Verifies copy rows selects the columns of the source table.
        /// </summary>
        [TestMethod]
        public void TestCopyRowsSelectsTheColumnsOfTheSourceTable()
        {
            // Setup
            var (source, destination) = GetConnections();
            // Act
            DataDbConnection.CopyRows(source,
                destination,
                "Person",
                "Person",
                null,
                1000,
                false,
                null,
                ColumnMappingBehavior.Automatic,
                IdentityBehavior.KeepIdentity,
                null,
                null,
                new CopyProgress(),
                null,
                null,
                null,
                null,
                null);
            // Assert
            var select = source.Executions.Single();
            StringAssert.Contains(select.CommandText,
                "SELECT");
            StringAssert.Contains(select.CommandText,
                "[Person]");
            StringAssert.Contains(select.CommandText,
                "[Id]");
            Assert.AreEqual(0,
                select.Parameters.Count);
        }

        /// <summary>
        /// Verifies copy rows with afilter selects the rows that match the filter.
        /// </summary>
        [TestMethod]
        public void TestCopyRowsWithAFilterSelectsTheRowsThatMatchTheFilter()
        {
            // Setup
            var (source, destination) = GetConnections();
            var where = new QueryGroup(new QueryField("Id",
                    Operation.GreaterThan,
                    1));
            // Act
            DataDbConnection.CopyRows(source,
                destination,
                "Person",
                "Person",
                where,
                1000,
                false,
                null,
                ColumnMappingBehavior.Automatic,
                IdentityBehavior.KeepIdentity,
                null,
                null,
                new CopyProgress(),
                null,
                null,
                null,
                null,
                null);
            // Assert
            var select = source.Executions.Single();
            StringAssert.Contains(select.CommandText,
                "WHERE");
            Assert.AreEqual(1,
                select.Parameters.Count);
            Assert.AreEqual(1,
                select.Parameters.Values.Single());
        }

        /// <summary>
        /// Verifies copy rows with abetween filter gives the left and the right values.
        /// </summary>
        [TestMethod]
        public void TestCopyRowsWithABetweenFilterGivesTheLeftAndTheRightValues()
        {
            // Setup
            var (source, destination) = GetConnections();
            var where = new QueryGroup(new QueryField("Id",
                    Operation.Between,
                    new[] { 1, 3 }));
            // Act
            DataDbConnection.CopyRows(source,
                destination,
                "Person",
                "Person",
                where,
                1000,
                false,
                null,
                ColumnMappingBehavior.Automatic,
                IdentityBehavior.KeepIdentity,
                null,
                null,
                new CopyProgress(),
                null,
                null,
                null,
                null,
                null);
            // Assert
            var parameters = source.Executions.Single().Parameters;
            Assert.AreEqual(2,
                parameters.Count);
            Assert.AreEqual(1,
                parameters.Single(p => p.Key.EndsWith("_Left",
                        StringComparison.Ordinal)).Value);
            Assert.AreEqual(3,
                parameters.Single(p => p.Key.EndsWith("_Right",
                        StringComparison.Ordinal)).Value);
        }

        /// <summary>
        /// Verifies copy rows with abetween filter that has asingle value gives anull right value.
        /// </summary>
        [TestMethod]
        public void TestCopyRowsWithABetweenFilterThatHasASingleValueGivesANullRightValue()
        {
            // Setup
            var (source, destination) = GetConnections();
            var where = new QueryGroup(new QueryField("Id",
                    Operation.Between,
                    new[] { 1 }));
            // Act
            DataDbConnection.CopyRows(source,
                destination,
                "Person",
                "Person",
                where,
                1000,
                false,
                null,
                ColumnMappingBehavior.Automatic,
                IdentityBehavior.KeepIdentity,
                null,
                null,
                new CopyProgress(),
                null,
                null,
                null,
                null,
                null);
            // Assert
            var parameters = source.Executions.Single().Parameters;
            Assert.AreEqual(1,
                parameters.Single(p => p.Key.EndsWith("_Left",
                        StringComparison.Ordinal)).Value);
            Assert.IsTrue(parameters.Single(p => p.Key.EndsWith("_Right",
                        StringComparison.Ordinal)).Value == null || parameters.Single(p => p.Key.EndsWith("_Right",
                        StringComparison.Ordinal)).Value is DBNull);
        }

        /// <summary>
        /// Verifies copy rows with an in filter gives aparameter per value.
        /// </summary>
        [TestMethod]
        public void TestCopyRowsWithAnInFilterGivesAParameterPerValue()
        {
            // Setup
            var (source, destination) = GetConnections();
            var where = new QueryGroup(new QueryField("Id",
                    Operation.In,
                    new[] { 1, 2, 3 }));
            // Act
            DataDbConnection.CopyRows(source,
                destination,
                "Person",
                "Person",
                where,
                1000,
                false,
                null,
                ColumnMappingBehavior.Automatic,
                IdentityBehavior.KeepIdentity,
                null,
                null,
                new CopyProgress(),
                null,
                null,
                null,
                null,
                null);
            // Assert
            var parameters = source.Executions.Single().Parameters;
            Assert.AreEqual(3,
                parameters.Count);
            CollectionAssert.AreEquivalent(new object[] { 1, 2, 3 },
                parameters.Values.ToArray());
            Assert.IsTrue(parameters.Keys.All(k => k.Contains("_In_")));
        }

        /// <summary>
        /// Verifies copy rows with afilter of asingle value gives the value by its parameter name.
        /// </summary>
        [TestMethod]
        public void TestCopyRowsWithAFilterOfASingleValueGivesTheValueByItsParameterName()
        {
            // Setup
            var (source, destination) = GetConnections();
            var where = new QueryGroup(new QueryField("Name",
                    "Name 2"));
            // Act
            DataDbConnection.CopyRows(source,
                destination,
                "Person",
                "Person",
                where,
                1000,
                false,
                null,
                ColumnMappingBehavior.Automatic,
                IdentityBehavior.KeepIdentity,
                null,
                null,
                new CopyProgress(),
                null,
                null,
                null,
                null,
                null);
            // Assert
            var parameters = source.Executions.Single().Parameters;
            Assert.AreEqual(1,
                parameters.Count);
            Assert.AreEqual("Name 2",
                parameters.Values.Single());
        }

        /// <summary>
        /// Verifies copy rows async with afilter selects the rows that match the filter.
        /// </summary>
        [TestMethod]
        public async Task TestCopyRowsAsyncWithAFilterSelectsTheRowsThatMatchTheFilter()
        {
            // Setup
            var (source, destination) = GetConnections();
            var where = new QueryGroup(new QueryField("Id",
                    Operation.In,
                    new[] { 1, 2 }));
            // Act
            await DataDbConnection.CopyRowsAsync(source,
                destination,
                "Person",
                "Person",
                where,
                1000,
                false,
                null,
                ColumnMappingBehavior.Automatic,
                IdentityBehavior.KeepIdentity,
                null,
                null,
                new CopyProgress(),
                null,
                null,
                null,
                null,
                null,
                CancellationToken.None);
            // Assert
            var parameters = source.Executions.First().Parameters;
            Assert.AreEqual(2,
                parameters.Count);
        }

        /// <summary>
        /// Verifies copy rows without aprogress callback still updates the progress.
        /// </summary>
        [TestMethod]
        public void TestCopyRowsWithoutAProgressCallbackStillUpdatesTheProgress()
        {
            // Setup
            var (source, destination) = GetConnections();
            var options = new CopyProgress();
            // Act
            DataDbConnection.CopyRows(source,
                destination,
                "Person",
                "Person",
                null,
                1000,
                false,
                null,
                ColumnMappingBehavior.Automatic,
                IdentityBehavior.KeepIdentity,
                null,
                null,
                options,
                null,
                null,
                null,
                null,
                null);
            // Assert
            Assert.AreEqual(1,
                options.BatchCount);
            Assert.AreEqual(3,
                options.RowCount);
        }

        /// <summary>
        /// Verifies throw exception on copy rows if there is no statement builder for the source connection.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyRowsIfThereIsNoStatementBuilderForTheSourceConnection()
        {
            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => DataDbConnection.CopyRows(new Mock<IDbConnection>().Object,
                    new FakeDbConnection(),
                    "Person",
                    "Person",
                    null,
                    1000,
                    false,
                    null,
                    ColumnMappingBehavior.Automatic,
                    IdentityBehavior.KeepIdentity,
                    null,
                    null,
                    new CopyProgress(),
                    null,
                    null,
                    null,
                    null,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on copy rows async if there is no statement builder for the source connection.
        /// </summary>
        [TestMethod]
        public async Task ThrowExceptionOnCopyRowsAsyncIfThereIsNoStatementBuilderForTheSourceConnection()
        {
            // Act/Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => DataDbConnection.CopyRowsAsync(new Mock<IDbConnection>().Object,
                    new FakeDbConnection(),
                    "Person",
                    "Person",
                    null,
                    1000,
                    false,
                    null,
                    ColumnMappingBehavior.Automatic,
                    IdentityBehavior.KeepIdentity,
                    null,
                    null,
                    new CopyProgress(),
                    null,
                    null,
                    null,
                    null,
                    null,
                    CancellationToken.None));
        }

        /// <summary>
        /// Verifies throw exception on copy rows async if the operation is cancelled before the rows are read.
        /// </summary>
        [TestMethod]
        public async Task ThrowExceptionOnCopyRowsAsyncIfTheOperationIsCancelledBeforeTheRowsAreRead()
        {
            // Setup
            var (source, destination) = GetConnections();
            using (var tokenSource = new CancellationTokenSource())
            {
                tokenSource.Cancel();
                // Act/Assert
                await Assert.ThrowsAsync<OperationCanceledException>(() => DataDbConnection.CopyRowsAsync(source,
                        destination,
                        "Person",
                        "Person",
                        null,
                        1000,
                        false,
                        null,
                        ColumnMappingBehavior.Automatic,
                        IdentityBehavior.KeepIdentity,
                        null,
                        null,
                        new CopyProgress(),
                        null,
                        null,
                        null,
                        null,
                        null,
                        tokenSource.Token));
            }
        }

        /// <summary>
        /// Verifies copy rows passes the trace key of the source command.
        /// </summary>
        [TestMethod]
        public void TestCopyRowsPassesTheTraceKeyOfTheSourceCommand()
        {
            // Setup
            var (source, destination) = GetConnections();
            var trace = new Mock<ITrace>();
            // Act
            DataDbConnection.CopyRows(source,
                destination,
                "Person",
                "Person",
                null,
                1000,
                false,
                null,
                ColumnMappingBehavior.Automatic,
                IdentityBehavior.KeepIdentity,
                null,
                null,
                new CopyProgress(),
                null,
                null,
                "MyTraceKey",
                trace.Object,
                null);
            // Assert
            trace.Verify(t => t.BeforeExecution(It.Is<CancellableTraceLog>(l => l.Key == "MyTraceKey")),
                Times.AtLeastOnce);
        }

        #endregion

        #region Validate

        /// <summary>
        /// Verifies throw exception on validate if the column mappings behavior is not defined.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnValidateIfTheColumnMappingsBehaviorIsNotDefined()
        {
            // Act/Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => DataDbConnection.Validate(GetConnection(),
                    "Person",
                    "Person",
                    GetConnection(),
                    null,
                    1,
                    RelationshipBehavior.TableOnly,
                    TableExistsBehavior.Skip,
                    (ColumnMappingBehavior)99,
                    IdentityBehavior.KeepIdentity,
                    ConstraintCheckBehavior.Enforce,
                    null));
        }

        /// <summary>
        /// Verifies validate accepts every column mappings behavior.
        /// </summary>
        [TestMethod]
        public void TestValidateAcceptsEveryColumnMappingsBehavior()
        {
            foreach (var behavior in new[]
            {
                ColumnMappingBehavior.Automatic,
                ColumnMappingBehavior.Strict,
                ColumnMappingBehavior.StrictBypass
            }

            )
            {
                // Act/Assert
                DataDbConnection.Validate(GetConnection(),
                    "Person",
                    "Person",
                    GetConnection(),
                    null,
                    1,
                    RelationshipBehavior.TableOnly,
                    TableExistsBehavior.Skip,
                    behavior,
                    IdentityBehavior.KeepIdentity,
                    ConstraintCheckBehavior.Enforce,
                    null);
            }
        }

        /// <summary>
        /// Verifies throw exception on validate if the connection is null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnValidateIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => DataDbConnection.Validate(null,
                    "Person",
                    "Person",
                    GetConnection(),
                    null,
                    1,
                    RelationshipBehavior.TableOnly,
                    TableExistsBehavior.Skip,
                    ColumnMappingBehavior.Automatic,
                    IdentityBehavior.KeepIdentity,
                    ConstraintCheckBehavior.Enforce,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on validate if the source table is null or white space.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnValidateIfTheSourceTableIsNullOrWhiteSpace()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => DataDbConnection.Validate(GetConnection(),
                    null,
                    "Person",
                    GetConnection(),
                    null,
                    1,
                    RelationshipBehavior.TableOnly,
                    TableExistsBehavior.Skip,
                    ColumnMappingBehavior.Automatic,
                    IdentityBehavior.KeepIdentity,
                    ConstraintCheckBehavior.Enforce,
                    null));
            Assert.Throws<ArgumentNullException>(() => DataDbConnection.Validate(GetConnection(),
                    " ",
                    "Person",
                    GetConnection(),
                    null,
                    1,
                    RelationshipBehavior.TableOnly,
                    TableExistsBehavior.Skip,
                    ColumnMappingBehavior.Automatic,
                    IdentityBehavior.KeepIdentity,
                    ConstraintCheckBehavior.Enforce,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on validate if the target table is null or white space.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnValidateIfTheTargetTableIsNullOrWhiteSpace()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => DataDbConnection.Validate(GetConnection(),
                    "Person",
                    null,
                    GetConnection(),
                    null,
                    1,
                    RelationshipBehavior.TableOnly,
                    TableExistsBehavior.Skip,
                    ColumnMappingBehavior.Automatic,
                    IdentityBehavior.KeepIdentity,
                    ConstraintCheckBehavior.Enforce,
                    null));
            Assert.Throws<ArgumentNullException>(() => DataDbConnection.Validate(GetConnection(),
                    "Person",
                    " ",
                    GetConnection(),
                    null,
                    1,
                    RelationshipBehavior.TableOnly,
                    TableExistsBehavior.Skip,
                    ColumnMappingBehavior.Automatic,
                    IdentityBehavior.KeepIdentity,
                    ConstraintCheckBehavior.Enforce,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on validate if the destination connection is null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnValidateIfTheDestinationConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => DataDbConnection.Validate(GetConnection(),
                    "Person",
                    "Person",
                    null,
                    null,
                    1,
                    RelationshipBehavior.TableOnly,
                    TableExistsBehavior.Skip,
                    ColumnMappingBehavior.Automatic,
                    IdentityBehavior.KeepIdentity,
                    ConstraintCheckBehavior.Enforce,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on validate if the batch size is not greater than zero.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnValidateIfTheBatchSizeIsNotGreaterThanZero()
        {
            // Act/Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => DataDbConnection.Validate(GetConnection(),
                    "Person",
                    "Person",
                    GetConnection(),
                    null,
                    0,
                    RelationshipBehavior.TableOnly,
                    TableExistsBehavior.Skip,
                    ColumnMappingBehavior.Automatic,
                    IdentityBehavior.KeepIdentity,
                    ConstraintCheckBehavior.Enforce,
                    null));
            Assert.Throws<ArgumentOutOfRangeException>(() => DataDbConnection.Validate(GetConnection(),
                    "Person",
                    "Person",
                    GetConnection(),
                    null,
                    -1,
                    RelationshipBehavior.TableOnly,
                    TableExistsBehavior.Skip,
                    ColumnMappingBehavior.Automatic,
                    IdentityBehavior.KeepIdentity,
                    ConstraintCheckBehavior.Enforce,
                    null));
        }

        /// <summary>
        /// Verifies throw exception on validate if the tables have different names and the relationship behavior is not table only.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnValidateIfTheTablesHaveDifferentNamesAndTheRelationshipBehaviorIsNotTableOnly()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() => DataDbConnection.Validate(new FakeDbConnection(),
                    "Person",
                    "Customer",
                    new FakeDbConnection(),
                    null,
                    1,
                    RelationshipBehavior.Parents,
                    TableExistsBehavior.Skip,
                    ColumnMappingBehavior.Automatic,
                    IdentityBehavior.KeepIdentity,
                    ConstraintCheckBehavior.Enforce,
                    null));
        }

        /// <summary>
        /// Verifies validate accepts tables with different names if only the table is copied even if the schema is requested.
        /// </summary>
        [TestMethod]
        public void TestValidateAcceptsTablesWithDifferentNamesIfOnlyTheTableIsCopiedEvenIfTheSchemaIsRequested()
        {
            // Act/Assert
            DataDbConnection.Validate(new FakeDbConnection(),
                "Person",
                "Customer",
                new FakeDbConnection(),
                null,
                1,
                RelationshipBehavior.TableOnly,
                TableExistsBehavior.Drop,
                ColumnMappingBehavior.Automatic,
                IdentityBehavior.KeepIdentity,
                ConstraintCheckBehavior.Enforce,
                null);
            DataDbConnection.Validate(new FakeDbConnection(),
                "Person",
                "Customer",
                new FakeDbConnection(),
                "Sales",
                1,
                RelationshipBehavior.TableOnly,
                TableExistsBehavior.Skip,
                ColumnMappingBehavior.Automatic,
                IdentityBehavior.KeepIdentity,
                ConstraintCheckBehavior.Enforce,
                null);
        }

        /// <summary>
        /// Verifies validate accepts tables with different names if the schema is not requested.
        /// </summary>
        [TestMethod]
        public void TestValidateAcceptsTablesWithDifferentNamesIfTheSchemaIsNotRequested()
        {
            // Act/Assert
            DataDbConnection.Validate(new FakeDbConnection(),
                "Person",
                "Customer",
                new FakeDbConnection(),
                null,
                1,
                RelationshipBehavior.TableOnly,
                TableExistsBehavior.Skip,
                ColumnMappingBehavior.Automatic,
                IdentityBehavior.KeepIdentity,
                ConstraintCheckBehavior.Enforce,
                null);
        }

        /// <summary>
        /// Verifies validate accepts tables with the same name in different schemas if the schema is requested.
        /// </summary>
        [TestMethod]
        public void TestValidateAcceptsTablesWithTheSameNameInDifferentSchemasIfTheSchemaIsRequested()
        {
            // Act/Assert
            DataDbConnection.Validate(new FakeDbConnection(),
                "[dbo].[Person]",
                "[Sales].[Person]",
                new FakeDbConnection(),
                "Sales",
                1,
                RelationshipBehavior.Parents,
                TableExistsBehavior.Drop,
                ColumnMappingBehavior.Automatic,
                IdentityBehavior.KeepIdentity,
                ConstraintCheckBehavior.Enforce,
                null);
        }
        #endregion
    }
}
