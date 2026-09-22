#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySqlConnector;
using RepoDb.MySqlConnector.IntegrationTests.Models;
using RepoDb.MySqlConnector.IntegrationTests.Setup;
using RepoDb.MySqlConnector.PropertyHandlers;
using System;
using System.Linq;

namespace RepoDb.MySqlConnector.IntegrationTests.PropertyHandlers
{
    [TestClass]
    public class TestSByteToBooleanPropertyHandler
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

        [TestMethod]
        public void TestSByteToBooleanPropertyHandlerSet()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new SByteToBooleanPropertyHandler();

                // Act
                var resultOfTrue = handler.Set(true, null);
                var resultOfFalse = handler.Set(false, null);

                // Assert
                Assert.AreEqual((sbyte)1, resultOfTrue);
                Assert.AreEqual((sbyte)0, resultOfFalse);
            }
        }

        [TestMethod]
        public void TestSByteToBooleanPropertyHandlerGet()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new SByteToBooleanPropertyHandler();

                // Act
                var resultOfOne = handler.Get(1, null);
                var resultOfZero = handler.Get(0, null);

                // Assert
                Assert.IsTrue(resultOfOne);
                Assert.IsFalse(resultOfZero);
            }
        }

        [TestMethod]
        public void TestSByteToBooleanPropertyHandlerGetWithNonZeroValues()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new SByteToBooleanPropertyHandler();

                // Act
                var resultOfNegative = handler.Get(-1, null);
                var resultOfMax = handler.Get(sbyte.MaxValue, null);
                var resultOfMin = handler.Get(sbyte.MinValue, null);

                // Assert
                Assert.IsTrue(resultOfNegative);
                Assert.IsTrue(resultOfMax);
                Assert.IsTrue(resultOfMin);
            }
        }

        [TestMethod]
        public void TestSByteToBooleanPropertyHandlerInsertAndQueryWithTrue()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new MySqlConnectorBooleanEntity { ColumnTinyInt = true };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity), System.Globalization.CultureInfo.InvariantCulture);
                var result = connection.Query<MySqlConnectorBooleanEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.IsTrue(result.ColumnTinyInt);
            }
        }

        [TestMethod]
        public void TestSByteToBooleanPropertyHandlerInsertAndQueryWithFalse()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new MySqlConnectorBooleanEntity { ColumnTinyInt = false };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity), System.Globalization.CultureInfo.InvariantCulture);
                var result = connection.Query<MySqlConnectorBooleanEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.IsFalse(result.ColumnTinyInt);
            }
        }

        [TestMethod]
        public void TestSByteToBooleanPropertyHandlerInsertAllAndQueryAll()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Setup
                var entities = new[]
                {
                    new MySqlConnectorBooleanEntity { ColumnTinyInt = true },
                    new MySqlConnectorBooleanEntity { ColumnTinyInt = false },
                    new MySqlConnectorBooleanEntity { ColumnTinyInt = true }
                };

                // Act
                connection.InsertAll(entities);
                var result = connection.QueryAll<MySqlConnectorBooleanEntity>().OrderBy(e => e.Id).ToList();

                // Assert
                Assert.AreEqual(3, result.Count);
                Assert.IsTrue(result[0].ColumnTinyInt);
                Assert.IsFalse(result[1].ColumnTinyInt);
                Assert.IsTrue(result[2].ColumnTinyInt);
            }
        }
    }
}
