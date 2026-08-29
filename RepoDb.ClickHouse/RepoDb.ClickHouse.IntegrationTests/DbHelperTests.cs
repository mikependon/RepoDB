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
using ClickHouse.Driver.ADO;
using RepoDb.ClickHouse.IntegrationTests.Setup;

namespace RepoDb.ClickHouse.IntegrationTests
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
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                var helper = connection.GetDbHelper();

                // Act
                var fields = helper.GetFields(connection, "CompleteTable", null);

                // Assert
                // Values are inlined (rather than passed as RepoDb-style '@name' parameters) because
                // ClickHouse.Driver expects DbParameter.ParameterName without the '@' prefix, and RepoDb's
                // automatic parameter creation for raw ad-hoc SQL always includes it - safe here since
                // both values are trusted test-internal constants, not user input.
                using (var reader = connection.ExecuteReader($@"SELECT name AS ColumnName
                    FROM system.columns
                    WHERE
                        table = 'CompleteTable'
                        AND database = '{connection.Database}'
                    ORDER BY position;"))
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
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
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
        public void TestDbHelperGetFieldsHasNoIdentity()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                var helper = connection.GetDbHelper();

                // Act - ClickHouse has no identity/auto-increment mechanism; no field is ever reported as one
                var fields = helper.GetFields(connection, "CompleteTable", null);

                // Assert
                Assert.IsFalse(fields.Any(f => f.IsIdentity == true));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestDbHelperGetFieldsAsync()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                var helper = connection.GetDbHelper();

                // Act
                var fields = await helper.GetFieldsAsync(connection, "CompleteTable", null);

                // Assert
                using (var reader = connection.ExecuteReader($@"SELECT name AS ColumnName
                    FROM system.columns
                    WHERE
                        table = 'CompleteTable'
                        AND database = '{connection.Database}'
                    ORDER BY position;"))
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
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
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
        public async Task TestDbHelperGetFieldsAsyncHasNoIdentity()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                var helper = connection.GetDbHelper();

                // Act
                var fields = await helper.GetFieldsAsync(connection, "CompleteTable", null);

                // Assert
                Assert.IsFalse(fields.Any(f => f.IsIdentity == true));
            }
        }

        #endregion

        #endregion

        #region GetScopeIdentity

        #region Sync

        [TestMethod]
        public void ThrowExceptionOnDbHelperGetScopeIdentity()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                var helper = connection.GetDbHelper();

                // Act - ClickHouse has no session-wide scope identity, sequence, or auto-increment mechanism
                Assert.Throws<NotSupportedException>(() =>
                    helper.GetScopeIdentity<long>(connection, null));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task ThrowExceptionOnDbHelperGetScopeIdentityAsync()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Setup
                var helper = connection.GetDbHelper();

                // Act
                await Assert.ThrowsAsync<NotSupportedException>(() =>
                    helper.GetScopeIdentityAsync<long>(connection, null));
            }
        }

        #endregion

        #endregion
    }
}
