#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FirebirdSql.Data.FirebirdClient;
using RepoDb.Firebird.IntegrationTests.Models;
using RepoDb.Firebird.IntegrationTests.Setup;

namespace RepoDb.Firebird.IntegrationTests
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
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                var helper = connection.GetDbHelper();

                // Act
                var fields = helper.GetFields(connection, "CompleteTable", null);

                // Assert
                using (var reader = connection.ExecuteReader(@"SELECT TRIM(RDB$FIELD_NAME) AS ColumnName
                    FROM RDB$RELATION_FIELDS
                    WHERE TRIM(RDB$RELATION_NAME) = @TableName
                    ORDER BY RDB$FIELD_POSITION", new { TableName = "CompleteTable" }))
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
            using (var connection = new FbConnection(Database.ConnectionString))
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
            using (var connection = new FbConnection(Database.ConnectionString))
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
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                var helper = connection.GetDbHelper();

                // Act
                var fields = await helper.GetFieldsAsync(connection, "CompleteTable", null);

                // Assert
                using (var reader = connection.ExecuteReader(@"SELECT TRIM(RDB$FIELD_NAME) AS ColumnName
                    FROM RDB$RELATION_FIELDS
                    WHERE TRIM(RDB$RELATION_NAME) = @TableName
                    ORDER BY RDB$FIELD_POSITION", new { TableName = "CompleteTable" }))
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
            using (var connection = new FbConnection(Database.ConnectionString))
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
            using (var connection = new FbConnection(Database.ConnectionString))
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
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                var helper = connection.GetDbHelper();
                var table = Helper.CreateCompleteTables(1).First();

                // Act
                var insertResult = connection.Insert<CompleteTable, long>(table);

                // Assert
                Assert.IsTrue(insertResult > 0);
                Assert.IsTrue(table.Id > 0);

                // Act & Assert
                // Firebird has no session-wide "last identity" concept (see FirebirdDbHelper.GetScopeIdentity),
                // so the helper deliberately throws instead of returning a (meaningless) value.
                Assert.Throws<NotSupportedException>(() =>
                    helper.GetScopeIdentity<long>(connection, null));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestDbHelperGetScopeIdentityAsync()
        {
            using (var connection = new FbConnection(Database.ConnectionString))
            {
                // Setup
                var helper = connection.GetDbHelper();
                var table = Helper.CreateCompleteTables(1).First();

                // Act
                var insertResult = connection.Insert<CompleteTable, long>(table);

                // Assert
                Assert.IsTrue(insertResult > 0);
                Assert.IsTrue(table.Id > 0);

                // Act & Assert
                // Firebird has no session-wide "last identity" concept (see FirebirdDbHelper.GetScopeIdentityAsync),
                // so the helper deliberately throws instead of returning a (meaningless) value.
                await Assert.ThrowsAsync<NotSupportedException>(() =>
                    helper.GetScopeIdentityAsync<long>(connection, null));
            }
        }

        #endregion

        #endregion
    }
}
