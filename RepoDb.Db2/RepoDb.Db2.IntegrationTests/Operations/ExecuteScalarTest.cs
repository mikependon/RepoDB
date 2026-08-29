#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Data.Db2;
using RepoDb.Enumerations;
using RepoDb.Db2.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Db2.IntegrationTests.Operations
{
    [TestClass]
    public class ExecuteScalarTest
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

        #region Sync

        [TestMethod]
        public void TestDb2ConnectionExecuteScalar()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.ExecuteScalar("SELECT COUNT(*) FROM \"CompleteTable\"");

            // Assert
            Assert.AreEqual(tables.Count, Convert.ToInt32(result));
        }

        [TestMethod]
        public void TestDb2ConnectionExecuteScalarWithAutomaticConversion()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            try
            {
                // Act
                var result = connection.ExecuteScalar("SELECT COUNT(*) FROM \"CompleteTable\"");

                // Assert
                Assert.AreEqual(tables.Count, Convert.ToInt32(result));
            }
            finally
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestDb2ConnectionExecuteScalarWithReturnType()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM \"CompleteTable\"");

            // Assert
            Assert.AreEqual(tables.Count, result);
        }

        [TestMethod]
        public void TestDb2ConnectionExecuteScalarWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act: bind variables are prefixed with ":" (not "@") for Db2.
            var result = connection.ExecuteScalar<string>("SELECT \"ColumnVarchar\" FROM \"CompleteTable\" WHERE \"Id\" = :Id",
                new { tables.Last().Id });

            // Assert
            Assert.AreEqual(tables.Last().ColumnVarchar, result);
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestDb2ConnectionExecuteScalarAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.ExecuteScalarAsync("SELECT COUNT(*) FROM \"CompleteTable\"");

            // Assert
            Assert.AreEqual(tables.Count, Convert.ToInt32(result));
        }

        [TestMethod]
        public async Task TestDb2ConnectionExecuteScalarAsyncWithAutomaticConversion()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            try
            {
                // Act
                var result = await connection.ExecuteScalarAsync("SELECT COUNT(*) FROM \"CompleteTable\"");

                // Assert
                Assert.AreEqual(tables.Count, Convert.ToInt32(result));
            }
            finally
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public async Task TestDb2ConnectionExecuteScalarAsyncWithReturnType()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM \"CompleteTable\"");

            // Assert
            Assert.AreEqual(tables.Count, result);
        }

        [TestMethod]
        public async Task TestDb2ConnectionExecuteScalarAsyncWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.ExecuteScalarAsync<string>("SELECT \"ColumnVarchar\" FROM \"CompleteTable\" WHERE \"Id\" = :Id",
                new { tables.Last().Id });

            // Assert
            Assert.AreEqual(tables.Last().ColumnVarchar, result);
        }

        #endregion
    }
}
