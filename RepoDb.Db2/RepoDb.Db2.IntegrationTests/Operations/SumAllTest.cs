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
    public class SumAllTest
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

        // NOTE: see AverageTest.cs/SumTest.cs for why "ColumnSmallInt" (not "ColumnInt") is used as the
        // aggregate target - summing the full-Int32-range "ColumnInt" risks a checked-arithmetic overflow.

        #region DataEntity

        #region Sync

        [TestMethod]
        public void TestDb2ConnectionSumAll()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var result = connection.SumAll<CompleteTable>(e => e.ColumnSmallInt);

                // Assert
                Assert.AreEqual(tables.Sum(e => e.ColumnSmallInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestDb2ConnectionSumAllWithAutomaticConversion()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
                try
                {
                    // Act
                    var result = connection.SumAll<CompleteTable>(e => e.ColumnSmallInt);

                    // Assert
                    Assert.AreEqual(tables.Sum(e => e.ColumnSmallInt), Convert.ToInt32(result));
                }
                finally
                {
                    GlobalConfiguration.Options.ConversionType = ConversionType.Default;
                }
            }
        }

        [TestMethod]
        public void TestDb2ConnectionSumAllWithHintsThrows()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act/Assert: AreTableHintsSupported == false for Db2 - any non-null/non-whitespace
                // "hints" argument must throw, rather than silently being ignored.
                Assert.Throws<NotSupportedException>(() =>
                    connection.SumAll<CompleteTable>(e => e.ColumnSmallInt, hints: "NOLOCK"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestDb2ConnectionSumAllAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SumAllAsync<CompleteTable>(e => e.ColumnSmallInt);

                // Assert
                Assert.AreEqual(tables.Sum(e => e.ColumnSmallInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestDb2ConnectionSumAllAsyncWithAutomaticConversion()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
                try
                {
                    // Act
                    var result = await connection.SumAllAsync<CompleteTable>(e => e.ColumnSmallInt);

                    // Assert
                    Assert.AreEqual(tables.Sum(e => e.ColumnSmallInt), Convert.ToInt32(result));
                }
                finally
                {
                    GlobalConfiguration.Options.ConversionType = ConversionType.Default;
                }
            }
        }

        [TestMethod]
        public async Task TestDb2ConnectionSumAllAsyncWithHintsThrows()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act/Assert: AreTableHintsSupported == false for Db2 - any non-null/non-whitespace
                // "hints" argument must throw, rather than silently being ignored.
                await Assert.ThrowsAsync<NotSupportedException>(() =>
                    connection.SumAllAsync<CompleteTable>(e => e.ColumnSmallInt, hints: "NOLOCK"));
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestDb2ConnectionSumAllViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var result = connection.SumAll(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnSmallInt", typeof(short)));

                // Assert
                Assert.AreEqual(tables.Sum(e => e.ColumnSmallInt), Convert.ToInt32(result));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestDb2ConnectionSumAllAsyncViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SumAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnSmallInt", typeof(short)));

                // Assert
                Assert.AreEqual(tables.Sum(e => e.ColumnSmallInt), Convert.ToInt32(result));
            }
        }

        #endregion

        #endregion
    }
}
