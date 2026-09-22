#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DuckDB.NET.Data;
using RepoDb.DuckDb.IntegrationTests.Models;
using RepoDb.DuckDb.IntegrationTests.Setup;

namespace RepoDb.DuckDb.IntegrationTests
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
            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Setup
                var helper = connection.GetDbHelper();

                // Act
                var fields = helper.GetFields(connection, "CompleteTable", null);

                // Assert
                using (var reader = connection.ExecuteReader(@"SELECT column_name AS ColumnName
                    FROM information_schema.columns
                    WHERE
                        table_name = $TableName
                        AND table_schema = $TableSchema
                    ORDER BY ordinal_position;", new { TableName = "CompleteTable", TableSchema = "main" }))
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
            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Setup
                var helper = connection.GetDbHelper();

                // Act
                var fields = helper.GetFields(connection, "CompleteTable", null);
                var primary = fields.FirstOrDefault(f => f.IsPrimary == true);

                // Assert
                Assert.IsNotNull(primary);
                Assert.AreEqual("Id", primary.Name, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestDbHelperGetFieldsIdentity()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Setup
                var helper = connection.GetDbHelper();

                // Act
                var fields = helper.GetFields(connection, "CompleteTable", null);
                var primary = fields.FirstOrDefault(f => f.IsIdentity == true);

                // Assert
                Assert.IsNotNull(primary);
                Assert.AreEqual("Id", primary.Name, StringComparer.Ordinal);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestDbHelperGetFieldsAsync()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Setup
                var helper = connection.GetDbHelper();

                // Act
                var fields = await helper.GetFieldsAsync(connection, "CompleteTable", null).ConfigureAwait(false);

                // Assert
                using (var reader = connection.ExecuteReader(@"SELECT column_name AS ColumnName
                    FROM information_schema.columns
                    WHERE
                        table_name = $TableName
                        AND table_schema = $TableSchema
                    ORDER BY ordinal_position;", new { TableName = "CompleteTable", TableSchema = "main" }))
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
            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Setup
                var helper = connection.GetDbHelper();

                // Act
                var fields = await helper.GetFieldsAsync(connection, "CompleteTable", null).ConfigureAwait(false);
                var primary = fields.FirstOrDefault(f => f.IsPrimary == true);

                // Assert
                Assert.IsNotNull(primary);
                Assert.AreEqual("Id", primary.Name, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public async Task TestDbHelperGetFieldsAsyncIdentity()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Setup
                var helper = connection.GetDbHelper();

                // Act
                var fields = await helper.GetFieldsAsync(connection, "CompleteTable", null).ConfigureAwait(false);
                var primary = fields.FirstOrDefault(f => f.IsIdentity == true);

                // Assert
                Assert.IsNotNull(primary);
                Assert.AreEqual("Id", primary.Name, StringComparer.Ordinal);
            }
        }

        #endregion

        #endregion

        #region GetScopeIdentity

        // DuckDB has no session-scoped "last identity" function (see DuckDbDbHelper.GetScopeIdentity's remarks);
        // identity values normally flow back via the statement builder's 'RETURNING' clause instead (see
        // TestDbHelperGetScopeIdentity's use of Insert<T> above, which already exercises that path), so these
        // two tests just confirm the documented NotSupportedException rather than a real scope-identity lookup.

        #region Sync

        [TestMethod]
        public void TestDbHelperGetScopeIdentity()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Setup
                var helper = connection.GetDbHelper();
                var table = Helper.CreateCompleteTables(1).First();

                // Act
                var insertResult = connection.Insert<CompleteTable, long>(table);

                // Assert
                Assert.IsTrue(insertResult > 0);
                Assert.IsTrue(table.Id > 0);

                // Act / Assert
                Assert.Throws<NotSupportedException>(() => helper.GetScopeIdentity<long>(connection, null));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestDbHelperGetScopeIdentityAsync()
        {
            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Setup
                var helper = connection.GetDbHelper();
                var table = Helper.CreateCompleteTables(1).First();

                // Act
                var insertResult = connection.Insert<CompleteTable, long>(table);

                // Assert
                Assert.IsTrue(insertResult > 0);
                Assert.IsTrue(table.Id > 0);

                // Act / Assert
                await Assert.ThrowsAsync<NotSupportedException>(async () =>
                    await helper.GetScopeIdentityAsync<long>(connection, null).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #endregion
    }
}
