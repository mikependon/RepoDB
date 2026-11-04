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
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.PostgreSql.IntegrationTests.PropertyHandlers
{
    /// <summary>
    /// Verifies property handler null results under default conversion settings.
    /// </summary>
    [TestClass]
    [DoNotParallelize]
    public class DbNullPropertyHandlerTest
    {
        #region Private Variables

        private NpgsqlConnection _connection;
        private ConversionType _previousConversionType;
        private NpgsqlTransaction _transaction;

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts an isolated transaction with default conversion settings.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            _previousConversionType = GlobalConfiguration.Options.ConversionType;
            GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            _connection = new NpgsqlConnection(Database.ConnectionString);
            _connection.Open();
            _transaction = _connection.BeginTransaction();
        }

        /// <summary>
        /// Rolls back the test rows and restores conversion settings.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            try
            {
                _transaction?.Dispose();
            }
            finally
            {
                _connection?.Dispose();
                GlobalConfiguration.Options.ConversionType = _previousConversionType;
            }
        }

        /// <summary>
        /// Verifies a null ColumnTsVector round trip.
        /// </summary>
        [TestMethod]
        public void TestColumnTsVectorWithNull()
        {
            // Setup
            var entity = new PostgreSqlTsVectorEntity { ColumnTsVector = null };


            // Act
            var id = _connection.Insert<PostgreSqlTsVectorEntity, long>(entity,
                transaction: _transaction);
            var rows = _connection.Query<PostgreSqlTsVectorEntity>(row => row.Id == id,
                transaction: _transaction);
            var result = rows.Single();


            // Assert
            Assert.AreEqual(id,
                result.Id);
            Assert.IsNull(result.ColumnTsVector);
        }

        /// <summary>
        /// Verifies a null ColumnTsVector round trip asynchronously.
        /// </summary>
        [TestMethod]
        public async Task TestColumnTsVectorAsyncWithNull()
        {
            // Setup
            var entity = new PostgreSqlTsVectorEntity { ColumnTsVector = null };


            // Act
            var id = await _connection.InsertAsync<PostgreSqlTsVectorEntity, long>(entity,
                transaction: _transaction);
            var rows = await _connection.QueryAsync<PostgreSqlTsVectorEntity>(row => row.Id == id,
                transaction: _transaction);
            var result = rows.Single();


            // Assert
            Assert.AreEqual(id,
                result.Id);
            Assert.IsNull(result.ColumnTsVector);
        }

        /// <summary>
        /// Verifies a null ColumnTsQuery round trip.
        /// </summary>
        [TestMethod]
        public void TestColumnTsQueryWithNull()
        {
            // Setup
            var entity = new PostgreSqlTsQueryEntity { ColumnTsQuery = null };


            // Act
            var id = _connection.Insert<PostgreSqlTsQueryEntity, long>(entity,
                transaction: _transaction);
            var rows = _connection.Query<PostgreSqlTsQueryEntity>(row => row.Id == id,
                transaction: _transaction);
            var result = rows.Single();


            // Assert
            Assert.AreEqual(id,
                result.Id);
            Assert.IsNull(result.ColumnTsQuery);
        }

        /// <summary>
        /// Verifies a null ColumnTsQuery round trip asynchronously.
        /// </summary>
        [TestMethod]
        public async Task TestColumnTsQueryAsyncWithNull()
        {
            // Setup
            var entity = new PostgreSqlTsQueryEntity { ColumnTsQuery = null };


            // Act
            var id = await _connection.InsertAsync<PostgreSqlTsQueryEntity, long>(entity,
                transaction: _transaction);
            var rows = await _connection.QueryAsync<PostgreSqlTsQueryEntity>(row => row.Id == id,
                transaction: _transaction);
            var result = rows.Single();


            // Assert
            Assert.AreEqual(id,
                result.Id);
            Assert.IsNull(result.ColumnTsQuery);
        }

        #endregion
    }
}
