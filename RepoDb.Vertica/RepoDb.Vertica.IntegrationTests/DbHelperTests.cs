#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vertica.Data.VerticaClient;
using RepoDb.Vertica.IntegrationTests.Models;
using RepoDb.Vertica.IntegrationTests.Setup;

namespace RepoDb.Vertica.IntegrationTests
{
    [TestClass]
    public class DbHelperTests
    {
        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Database.Cleanup();
        }

        #region GetFields

        #region Sync

        [TestMethod]
        public void TestDbHelperGetFields()
        {
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Setup
                var helper = connection.GetDbHelper();

                // Act
                var fields = helper.GetFields(connection, "CompleteTable", null);

                // Assert
                using (var reader = connection.ExecuteReader(@"SELECT column_name AS ColumnName
                    FROM v_catalog.columns
                    WHERE table_name = @TableName
                    ORDER BY ordinal_position", new { TableName = "CompleteTable" }))
                {
                    var fieldCount = 0;

                    while (reader.Read())
                    {
                        var name = reader.GetString(0);
                        var field = fields.FirstOrDefault(f => string.Equals(f.Name, name, StringComparison.OrdinalIgnoreCase));

                        // Assert
                        Assert.IsNotNull(field);

                        fieldCount++;
                    }

                    // Assert
                    Assert.AreEqual(fieldCount, fields.Count());
                }
            }
        }

        [TestMethod]
        public void TestDbHelperGetFieldsPrimary()
        {
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Setup
                var helper = connection.GetDbHelper();

                // Act
                var fields = helper.GetFields(connection, "CompleteTable", null);
                var primary = fields.FirstOrDefault(f => f.IsPrimary == true);

                // Assert
                Assert.IsNotNull(primary);
                Assert.AreEqual("Id", primary.Name);
            }
        }

        [TestMethod]
        public void TestDbHelperGetFieldsIdentity()
        {
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Setup
                var helper = connection.GetDbHelper();

                // Act
                var fields = helper.GetFields(connection, "CompleteTable", null);
                var primary = fields.FirstOrDefault(f => f.IsIdentity == true);

                // Assert
                Assert.IsNotNull(primary);
                Assert.AreEqual("Id", primary.Name);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestDbHelperGetFieldsAsync()
        {
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Setup
                var helper = connection.GetDbHelper();

                // Act
                var fields = await helper.GetFieldsAsync(connection, "CompleteTable", null);

                // Assert
                using (var reader = connection.ExecuteReader(@"SELECT column_name AS ColumnName
                    FROM v_catalog.columns
                    WHERE table_name = @TableName
                    ORDER BY ordinal_position", new { TableName = "CompleteTable" }))
                {
                    var fieldCount = 0;

                    while (reader.Read())
                    {
                        var name = reader.GetString(0);
                        var field = fields.FirstOrDefault(f => string.Equals(f.Name, name, StringComparison.OrdinalIgnoreCase));

                        // Assert
                        Assert.IsNotNull(field);

                        fieldCount++;
                    }

                    // Assert
                    Assert.AreEqual(fieldCount, fields.Count());
                }
            }
        }

        [TestMethod]
        public async Task TestDbHelperGetFieldsAsyncPrimary()
        {
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Setup
                var helper = connection.GetDbHelper();

                // Act
                var fields = await helper.GetFieldsAsync(connection, "CompleteTable", null);
                var primary = fields.FirstOrDefault(f => f.IsPrimary == true);

                // Assert
                Assert.IsNotNull(primary);
                Assert.AreEqual("Id", primary.Name);
            }
        }

        [TestMethod]
        public async Task TestDbHelperGetFieldsAsyncIdentity()
        {
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Setup
                var helper = connection.GetDbHelper();

                // Act
                var fields = await helper.GetFieldsAsync(connection, "CompleteTable", null);
                var primary = fields.FirstOrDefault(f => f.IsIdentity == true);

                // Assert
                Assert.IsNotNull(primary);
                Assert.AreEqual("Id", primary.Name);
            }
        }

        #endregion

        #endregion

        #region GetScopeIdentity

        #region Sync

        [TestMethod]
        public void TestDbHelperGetScopeIdentity()
        {
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Setup
                var helper = connection.GetDbHelper();
                var table = Helper.CreateCompleteTables(1).First();

                // Act
                connection.Insert<CompleteTable>(table);

                // Assert - verify the row was actually written.
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());

                // Act & Assert
                Assert.AreEqual(table.Id, helper.GetScopeIdentity<long>(connection, null));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestDbHelperGetScopeIdentityAsync()
        {
            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Setup
                var helper = connection.GetDbHelper();
                var table = Helper.CreateCompleteTables(1).First();

                // Act
                await connection.InsertAsync<CompleteTable>(table);

                // Assert
                Assert.AreEqual(1, await connection.CountAllAsync<CompleteTable>());

                // Act & Assert - Vertica has no RETURNING clause, so RepoDb reads the generated IDENTITY
                // value back via LAST_INSERT_ID() (see VerticaDbHelper.GetScopeIdentityAsync) as a
                // separate query; connection.InsertAsync above already used it internally to populate
                // table.Id, and it remains valid (same session, no intervening insert) to query again
                // directly here.
                Assert.AreEqual(table.Id, await helper.GetScopeIdentityAsync<long>(connection, null));
            }
        }

        #endregion

        #endregion
    }
}
