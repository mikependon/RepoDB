#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.AuroraDb.Npgsql;
using RepoDb.Schema.AuroraDbPostgreSql.IntegrationTests.Setup;

namespace RepoDb.Schema.AuroraDbPostgreSql.IntegrationTests
{
    /// <summary>
    /// Creates schemas in a Aurora PostgreSQL database: the schemas that a test creates are dropped when the test ends.
    /// </summary>
    [TestClass]
    public class AuroraDbPostgreSqlCreateSchemaTest
    {
        #region Private Variables

        /// <summary>
        /// The statements that clean up what the test created (they are executed in the reverse order).
        /// </summary>
        private readonly List<string> _cleanup = new List<string>();

        #endregion

        #region Initialize

        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
        }

        [TestCleanup]
        public void Cleanup()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                foreach (var statement in Enumerable.Reverse(_cleanup))
                {
                    try
                    {
                        connection.ExecuteNonQuery(statement);
                    }
                    catch (Exception)
                    {
                        // The schema (or the table) may not have been created
                    }
                }
            }
            _cleanup.Clear();
            Database.Cleanup();
        }

        #endregion

        #region Helpers

        private static string Quote(string name) =>
            "\"" + name.Replace("\"", "\"\"") + "\"";

        private string NewName(string prefix = "RepoDb_Cs_") =>
            prefix + Guid.NewGuid().ToString("N").Substring(0, 8);

        private string Track(string name)
        {
            _cleanup.Add("DROP SCHEMA " + Quote(name));
            return name;
        }

        private static List<string> Schemas()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                return new AuroraDbPostgreSqlSchemaReader(connection).GetSchemas().Select(schema => schema.Name).ToList();
            }
        }

        #endregion

        #region CreateSchema

        [TestMethod]
        public void TestCreateSchema()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var name = Track(NewName());

                // Act
                var actual = connection.CreateSchema(name);

                // Assert
                Assert.IsTrue(actual);
                CollectionAssert.Contains(Schemas(), name);
            }
        }

        [TestMethod]
        public void TestCreateSchemaDoesNothingIfTheSchemaExists()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var name = Track(NewName());
                connection.CreateSchema(name);

                // Act
                var actual = connection.CreateSchema(name);

                // Assert
                Assert.IsFalse(actual);
                Assert.AreEqual(1, Schemas().Count(schema => schema == name));
            }
        }

        [TestMethod]
        public void TestCreateSchemaWithANameThatHasSpaces()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var name = Track(NewName("RepoDb Cs "));

                // Act
                var actual = connection.CreateSchema(name);

                // Assert
                Assert.IsTrue(actual);
                CollectionAssert.Contains(Schemas(), name);
            }
        }

        [TestMethod]
        public void TestCreateSchemaKeepsTheCaseOfTheName()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var name = Track(NewName("RepoDb_Cs_"));

                // Act
                connection.CreateSchema(name);

                // Assert
                CollectionAssert.Contains(Schemas(), name);
            }
        }


        [TestMethod]
        public void TestCreateSchemaWithANameThatHasTheQuoteOfTheProvider()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var name = Track(NewName("RepoDb\"Cs_"));

                // Act
                var actual = connection.CreateSchema(name);

                // Assert
                Assert.IsTrue(actual);
                CollectionAssert.Contains(Schemas(), name);
            }
        }

        [TestMethod]
        public void TestCreateSchemaCreatesASchemaThatATableCanBeCreatedIn()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var name = Track(NewName());
                var table = Quote(name) + "." + Quote("Item");
                _cleanup.Add("DROP TABLE " + table);

                // Act
                connection.CreateSchema(name);
                connection.ExecuteNonQuery("CREATE TABLE " + table + " (" + Quote("Id") + " INTEGER NOT NULL)");

                // Assert
                Assert.AreEqual(0L, Convert.ToInt64(connection.ExecuteScalar("SELECT COUNT(*) FROM " + table)));
            }
        }

        [TestMethod]
        public void TestCreateSchemaWithACommandTimeout()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var name = Track(NewName());

                // Act
                var actual = connection.CreateSchema(name, commandTimeout: 30);

                // Assert
                Assert.IsTrue(actual);
                CollectionAssert.Contains(Schemas(), name);
            }
        }

        [TestMethod]
        public void TestCreateSchemaInATransactionIsRolledBackWithIt()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var name = Track(NewName());
                connection.Open();

                // Act
                using (var transaction = connection.BeginTransaction())
                {
                    var actual = connection.CreateSchema(name, transaction: transaction);
                    Assert.IsTrue(actual);
                    transaction.Rollback();
                }

                // Assert
                CollectionAssert.DoesNotContain(Schemas(), name);
            }
        }

        [TestMethod]
        public void TestCreateSchemaInATransactionIsKeptWhenItIsCommitted()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var name = Track(NewName());
                connection.Open();

                // Act
                using (var transaction = connection.BeginTransaction())
                {
                    connection.CreateSchema(name, transaction: transaction);
                    transaction.Commit();
                }

                // Assert
                CollectionAssert.Contains(Schemas(), name);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCreateSchemaIfTheNameIsEmpty()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                Assert.Throws<ArgumentNullException>(() => connection.CreateSchema(" "));
            }
        }

        #endregion

        #region CreateSchemaAsync

        [TestMethod]
        public async Task TestCreateSchemaAsync()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var name = Track(NewName());

                // Act
                var first = await connection.CreateSchemaAsync(name);
                var second = await connection.CreateSchemaAsync(name);

                // Assert
                Assert.IsTrue(first);
                Assert.IsFalse(second);
                CollectionAssert.Contains(Schemas(), name);
            }
        }

        #endregion
    }
}
