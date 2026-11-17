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
using MySqlConnector;
using RepoDb.Schema.MySqlConnector.IntegrationTests.Setup;

namespace RepoDb.Schema.MySqlConnector.IntegrationTests
{
    /// <summary>
    /// Creates schemas in a MySQL database: the schemas that a test creates are dropped when the test ends.
    /// </summary>
    [TestClass]
    public class MySqlConnectorCreateSchemaTest
    {
        #region Private Variables

        /// <summary>
        /// The statements that clean up what the test created (they are executed in the reverse order).
        /// </summary>
        private readonly List<string> _cleanup = new List<string>();
        #endregion

        #region Initialize

        /// <summary>
        /// Performs initialize.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
        }

        /// <summary>
        /// Performs cleanup.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            using (var connection = new MySqlConnection(Database.ConnectionStringForTarget))
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

        #region Private Methods

        /// <summary>
        /// Performs quote.
        /// </summary>
        /// <param name="name">The name.</param>
        private static string Quote(string name) => "`" + name.Replace("`",
            "``") + "`";

        /// <summary>
        /// Performs new name.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        private string NewName(string prefix = "repodb_cs_") => prefix + Guid.NewGuid().ToString("N").Substring(0,
            8);

        /// <summary>
        /// Performs track.
        /// </summary>
        /// <param name="name">The name.</param>
        private string Track(string name)
        {
            _cleanup.Add("DROP SCHEMA " + Quote(name));
            return name;
        }

        /// <summary>
        /// Performs schemas.
        /// </summary>
        private static List<string> Schemas()
        {
            using (var connection = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                return new MySqlConnectorSchemaReader().GetSchemas(connection,
                    transaction: null).Select(schema => schema.Name).ToList();
            }
        }

        #endregion

        #region CreateSchema

        /// <summary>
        /// Verifies create schema.
        /// </summary>
        [TestMethod]
        public void TestCreateSchema()
        {
            using (var connection = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var name=Track(NewName());
                // Act
                var actual = connection.CreateSchema(name);
                // Assert
                Assert.IsTrue(actual);
                CollectionAssert.Contains(Schemas(),
                    name);
            }
        }

        /// <summary>
        /// Verifies create schema does nothing if the schema exists.
        /// </summary>
        [TestMethod]
        public void TestCreateSchemaDoesNothingIfTheSchemaExists()
        {
            using (var connection = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var name=Track(NewName());
                connection.CreateSchema(name);
                // Act
                var actual = connection.CreateSchema(name);
                // Assert
                Assert.IsFalse(actual);
                Assert.AreEqual(1,
                    Schemas().Count(schema => schema == name));
            }
        }

        /// <summary>
        /// Verifies create schema with aname that has spaces.
        /// </summary>
        [TestMethod]
        public void TestCreateSchemaWithANameThatHasSpaces()
        {
            using (var connection = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var name=Track(NewName("repodb cs "));
                // Act
                var actual = connection.CreateSchema(name);
                // Assert
                Assert.IsTrue(actual);
                CollectionAssert.Contains(Schemas(),
                    name);
            }
        }

        /// <summary>
        /// Verifies create schema creates aschema that atable can be created in.
        /// </summary>
        [TestMethod]
        public void TestCreateSchemaCreatesASchemaThatATableCanBeCreatedIn()
        {
            using (var connection = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var name=Track(NewName());
                var table = Quote(name) + "." + Quote("Item");
                _cleanup.Add("DROP TABLE " + table);
                // Act
                connection.CreateSchema(name);
                connection.ExecuteNonQuery("CREATE TABLE " + table + " (" + Quote("Id") + " INTEGER NOT NULL)");
                // Assert
                Assert.AreEqual(0L,
                    Convert.ToInt64(connection.ExecuteScalar("SELECT COUNT(*) FROM " + table)));
            }
        }

        /// <summary>
        /// Verifies create schema with acommand timeout.
        /// </summary>
        [TestMethod]
        public void TestCreateSchemaWithACommandTimeout()
        {
            using (var connection = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var name=Track(NewName());
                // Act
                var actual = connection.CreateSchema(name,
                    commandTimeout: 30);
                // Assert
                Assert.IsTrue(actual);
                CollectionAssert.Contains(Schemas(),
                    name);
            }
        }

        /// <summary>
        /// Verifies throw exception on create schema if the name is empty.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCreateSchemaIfTheNameIsEmpty()
        {
            using (var connection = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                Assert.Throws<ArgumentNullException>(() => connection.CreateSchema(" "));
            }
        }

        #endregion

        #region CreateSchemaAsync

        /// <summary>
        /// Verifies create schema async.
        /// </summary>
        [TestMethod]
        public async Task TestCreateSchemaAsync()
        {
            using (var connection = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var name=Track(NewName());
                // Act
                var first = await connection.CreateSchemaAsync(name);
                var second = await connection.CreateSchemaAsync(name);
                // Assert
                Assert.IsTrue(first);
                Assert.IsFalse(second);
                CollectionAssert.Contains(Schemas(),
                    name);
            }
        }
        #endregion
    }
}
