#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Enumerations;
using RepoDb.PostgreSql.IntegrationTests.Models;
using RepoDb.PostgreSql.IntegrationTests.Setup;
using System.Threading.Tasks;

namespace RepoDb.PostgreSql.IntegrationTests.Operations
{
    [TestClass]
    [DoNotParallelize]
    public class DbNullResultTest
    {
        #region Private Variables

        private ConversionType _previousConversionType;
        private string _tableName;

        #endregion

        #region Private Methods

        private void AssertInsertedRow(NpgsqlConnection connection)
        {
            var count = connection.ExecuteScalar<long>($"SELECT COUNT(*) FROM public.\"{_tableName}\" WHERE tote_id = 1 AND name = 'x'");
            Assert.AreEqual(1L,
                count);
        }

        #endregion

        #region Public Methods

        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            _previousConversionType = GlobalConfiguration.Options.ConversionType;
            GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            _tableName = Database.CreateKeylessTable();
        }

        [TestCleanup]
        public void Cleanup()
        {
            try
            {
                Database.DropKeylessTable(_tableName);
            }
            finally
            {
                GlobalConfiguration.Options.ConversionType = _previousConversionType;
            }
        }

        [TestMethod]
        public void TestInsertWithoutIdentityReturnsNull()
        {
            // Setup
            using var connection = new NpgsqlConnection(Database.ConnectionString);
            var row = new KeylessRow { ToteId = 1, Name = "x" };

            // Act
            var result = connection.Insert(_tableName,
                row);

            // Assert
            Assert.IsNull(result);
            AssertInsertedRow(connection);
        }

        [TestMethod]
        public async Task TestInsertAsyncWithoutIdentityReturnsNull()
        {
            // Setup
            using var connection = new NpgsqlConnection(Database.ConnectionString);
            var row = new KeylessRow { ToteId = 1, Name = "x" };

            // Act
            var result = await connection.InsertAsync(_tableName,
                row);

            // Assert
            Assert.IsNull(result);
            AssertInsertedRow(connection);
        }

        [TestMethod]
        public void TestMaxOverEmptySetReturnsNull()
        {
            // Setup
            using var connection = new NpgsqlConnection(Database.ConnectionString);
            var field = new Field("tote_id");
            var where = new QueryField("tote_id",
                Operation.GreaterThan,
                0);

            // Act
            var result = connection.Max(_tableName,
                field,
                where);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task TestMaxAsyncOverEmptySetReturnsNull()
        {
            // Setup
            using var connection = new NpgsqlConnection(Database.ConnectionString);
            var field = new Field("tote_id");
            var where = new QueryField("tote_id",
                Operation.GreaterThan,
                0);

            // Act
            var result = await connection.MaxAsync(_tableName,
                field,
                where);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestSumOverEmptySetReturnsNull()
        {
            // Setup
            using var connection = new NpgsqlConnection(Database.ConnectionString);
            var field = new Field("tote_id");
            var where = new QueryField("tote_id",
                Operation.GreaterThan,
                0);

            // Act
            var result = connection.Sum(_tableName,
                field,
                where);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task TestSumAsyncOverEmptySetReturnsNull()
        {
            // Setup
            using var connection = new NpgsqlConnection(Database.ConnectionString);
            var field = new Field("tote_id");
            var where = new QueryField("tote_id",
                Operation.GreaterThan,
                0);

            // Act
            var result = await connection.SumAsync(_tableName,
                field,
                where);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestMinOverEmptySetReturnsNull()
        {
            // Setup
            using var connection = new NpgsqlConnection(Database.ConnectionString);
            var field = new Field("tote_id");
            var where = new QueryField("tote_id",
                Operation.GreaterThan,
                0);

            // Act
            var result = connection.Min(_tableName,
                field,
                where);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task TestMinAsyncOverEmptySetReturnsNull()
        {
            // Setup
            using var connection = new NpgsqlConnection(Database.ConnectionString);
            var field = new Field("tote_id");
            var where = new QueryField("tote_id",
                Operation.GreaterThan,
                0);

            // Act
            var result = await connection.MinAsync(_tableName,
                field,
                where);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestAverageOverEmptySetReturnsNull()
        {
            // Setup
            using var connection = new NpgsqlConnection(Database.ConnectionString);
            var field = new Field("tote_id");
            var where = new QueryField("tote_id",
                Operation.GreaterThan,
                0);

            // Act
            var result = connection.Average<object>(_tableName,
                field,
                where);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task TestAverageAsyncOverEmptySetReturnsNull()
        {
            // Setup
            using var connection = new NpgsqlConnection(Database.ConnectionString);
            var field = new Field("tote_id");
            var where = new QueryField("tote_id",
                Operation.GreaterThan,
                0);

            // Act
            var result = await connection.AverageAsync<object>(_tableName,
                field,
                where);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestCountOverEmptySetReturnsZero()
        {
            // Setup
            using var connection = new NpgsqlConnection(Database.ConnectionString);
            var where = new QueryField("tote_id",
                Operation.GreaterThan,
                0);

            // Act
            var result = connection.Count(_tableName,
                where);

            // Assert
            Assert.AreEqual(0L,
                result);
        }

        [TestMethod]
        public async Task TestCountAsyncOverEmptySetReturnsZero()
        {
            // Setup
            using var connection = new NpgsqlConnection(Database.ConnectionString);
            var where = new QueryField("tote_id",
                Operation.GreaterThan,
                0);

            // Act
            var result = await connection.CountAsync(_tableName,
                where);

            // Assert
            Assert.AreEqual(0L,
                result);
        }

        [TestMethod]
        public void TestExistsOverEmptySetReturnsFalse()
        {
            // Setup
            using var connection = new NpgsqlConnection(Database.ConnectionString);
            var where = new QueryField("tote_id",
                Operation.GreaterThan,
                0);

            // Act
            var result = connection.Exists(_tableName,
                where);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task TestExistsAsyncOverEmptySetReturnsFalse()
        {
            // Setup
            using var connection = new NpgsqlConnection(Database.ConnectionString);
            var where = new QueryField("tote_id",
                Operation.GreaterThan,
                0);

            // Act
            var result = await connection.ExistsAsync(_tableName,
                where);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestInsertAllWithoutIdentityReturnsInsertedCount()
        {
            // Setup
            using var connection = new NpgsqlConnection(Database.ConnectionString);
            var rows = new[]
            {
                new KeylessRow { ToteId = 1, Name = "x" },
                new KeylessRow { ToteId = 2, Name = "y" }
            };

            // Act
            var result = connection.InsertAll(_tableName,
                rows);

            // Assert
            Assert.AreEqual(2,
                result);
            Assert.AreEqual(2L,
                connection.ExecuteScalar<long>($"SELECT COUNT(*) FROM public.\"{_tableName}\""));
        }

        [TestMethod]
        public async Task TestInsertAllAsyncWithoutIdentityReturnsInsertedCount()
        {
            // Setup
            using var connection = new NpgsqlConnection(Database.ConnectionString);
            var rows = new[]
            {
                new KeylessRow { ToteId = 1, Name = "x" },
                new KeylessRow { ToteId = 2, Name = "y" }
            };

            // Act
            var result = await connection.InsertAllAsync(_tableName,
                rows);

            // Assert
            Assert.AreEqual(2,
                result);
            Assert.AreEqual(2L,
                connection.ExecuteScalar<long>($"SELECT COUNT(*) FROM public.\"{_tableName}\""));
        }

        #endregion
    }
}
