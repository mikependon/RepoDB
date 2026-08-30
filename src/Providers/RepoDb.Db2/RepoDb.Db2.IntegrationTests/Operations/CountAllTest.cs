#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Data.Db2;
using RepoDb.Enumerations;
using RepoDb.Db2.IntegrationTests.Models;
using RepoDb.Db2.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Db2.IntegrationTests.Operations
{
    [TestClass]
    public class CountAllTest
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

        #region DataEntity

        #region Sync

        [TestMethod]
        public void TestDb2ConnectionCountAll()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var result = connection.CountAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public void TestDb2ConnectionCountAllWithAutomaticConversion()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
                try
                {
                    // Act
                    var result = connection.CountAll<CompleteTable>();

                    // Assert
                    Assert.AreEqual(tables.Count(), result);
                }
                finally
                {
                    GlobalConfiguration.Options.ConversionType = ConversionType.Default;
                }
            }
        }

        [TestMethod]
        public void TestDb2ConnectionCountAllWithHintsThrows()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act/Assert: AreTableHintsSupported == false for Db2 - any non-null/non-whitespace
                // "hints" argument must throw, rather than silently being ignored.
                Assert.Throws<NotSupportedException>(() =>
                    connection.CountAll<CompleteTable>(hints: "NOLOCK"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestDb2ConnectionCountAllAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var result = await connection.CountAllAsync<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public async Task TestDb2ConnectionCountAllAsyncWithAutomaticConversion()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
                try
                {
                    // Act
                    var result = await connection.CountAllAsync<CompleteTable>();

                    // Assert
                    Assert.AreEqual(tables.Count(), result);
                }
                finally
                {
                    GlobalConfiguration.Options.ConversionType = ConversionType.Default;
                }
            }
        }

        [TestMethod]
        public async Task TestDb2ConnectionCountAllAsyncWithHintsThrows()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act/Assert: AreTableHintsSupported == false for Db2 - any non-null/non-whitespace
                // "hints" argument must throw, rather than silently being ignored.
                await Assert.ThrowsAsync<NotSupportedException>(() =>
                    connection.CountAllAsync<CompleteTable>(hints: "NOLOCK"));
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestDb2ConnectionCountAllViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var result = connection.CountAll(ClassMappedNameCache.Get<CompleteTable>());

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestDb2ConnectionCountAllAsyncViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var result = await connection.CountAllAsync(ClassMappedNameCache.Get<CompleteTable>());

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        #endregion

        #endregion
    }
}
