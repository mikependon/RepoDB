#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Connector.EnterpriseDb;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NpgsqlTypes;
using RepoDb.PropertyHandlers.EnterpriseDb;
using RepoDb.EnterpriseDb.IntegrationTests.Models;
using RepoDb.EnterpriseDb.IntegrationTests.Setup;
using System;
using System.Linq;

namespace RepoDb.EnterpriseDb.IntegrationTests.PropertyHandlers
{
    [TestClass]
    public class TestEnterpriseDbTsVectorToStringPropertyHandler
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
        public void TestEnterpriseDbTsVectorToStringPropertyHandlerSet()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new EnterpriseDbTsVectorToStringPropertyHandler();

                // Act
                var result = handler.Set("'cat':3 'fat':2 'rat':5", null);

                // Assert
                Assert.IsInstanceOfType(result, typeof(NpgsqlTsVector));
                Assert.AreEqual("'cat':3 'fat':2 'rat':5", result.ToString(), StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestEnterpriseDbTsVectorToStringPropertyHandlerSetWithNull()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new EnterpriseDbTsVectorToStringPropertyHandler();

                // Act
                var result = handler.Set(null, null);

                // Assert
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void TestEnterpriseDbTsVectorToStringPropertyHandlerGetWithSupportedTypes()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new EnterpriseDbTsVectorToStringPropertyHandler();
                var value = NpgsqlTsVector.Parse("'cat':3 'fat':2 'rat':5");

                // Act
                var resultOfType = handler.Get(value, null);
                var resultOfString = handler.Get("'cat':3 'fat':2 'rat':5", null);

                // Assert
                Assert.AreEqual("'cat':3 'fat':2 'rat':5", resultOfType, StringComparer.Ordinal);
                Assert.AreEqual("'cat':3 'fat':2 'rat':5", resultOfString, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestEnterpriseDbTsVectorToStringPropertyHandlerGetWithNullValues()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new EnterpriseDbTsVectorToStringPropertyHandler();

                // Act
                var resultOfNull = handler.Get(null, null);
                var resultOfDbNull = handler.Get(DBNull.Value, null);

                // Assert
                Assert.IsNull(resultOfNull);
                Assert.IsNull(resultOfDbNull);
            }
        }

        [TestMethod]
        public void TestEnterpriseDbTsVectorToStringPropertyHandlerGetWithUnsupportedType()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                var handler = new EnterpriseDbTsVectorToStringPropertyHandler();

                // Act & Assert
                Assert.ThrowsExactly<ArgumentException>(() => handler.Get(123, null));
            }
        }

        [TestMethod]
        public void TestEnterpriseDbTsVectorToStringPropertyHandlerInsertAndQuery()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new EnterpriseDbTsVectorEntity
                {
                    ColumnTsVector = "'cat':3 'fat':2 'rat':5"
                };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity), System.Globalization.CultureInfo.InvariantCulture);
                var result = connection.Query<EnterpriseDbTsVectorEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.AreEqual("'cat':3 'fat':2 'rat':5", result.ColumnTsVector, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestEnterpriseDbTsVectorToStringPropertyHandlerInsertAndQueryWithNull()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                var entity = new EnterpriseDbTsVectorEntity
                {
                    ColumnTsVector = null
                };

                // Act
                var id = Convert.ToInt64(connection.Insert(entity), System.Globalization.CultureInfo.InvariantCulture);
                var result = connection.Query<EnterpriseDbTsVectorEntity>(e => e.Id == id).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.IsNull(result.ColumnTsVector);
            }
        }

        [TestMethod]
        public void TestEnterpriseDbTsVectorToStringPropertyHandlerInsertAllAndQueryAll()
        {
            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                var entities = new[]
                {
                    new EnterpriseDbTsVectorEntity { ColumnTsVector = "'cat':3 'fat':2 'rat':5" },
                    new EnterpriseDbTsVectorEntity { ColumnTsVector = null },
                    new EnterpriseDbTsVectorEntity { ColumnTsVector = "'cat' 'fat'" }
                };

                // Act
                connection.InsertAll(entities);
                var result = connection.QueryAll<EnterpriseDbTsVectorEntity>().OrderBy(e => e.Id).ToList();

                // Assert
                Assert.AreEqual(3, result.Count);
                Assert.AreEqual("'cat':3 'fat':2 'rat':5", result[0].ColumnTsVector, StringComparer.Ordinal);
                Assert.IsNull(result[1].ColumnTsVector);
                Assert.AreEqual("'cat' 'fat'", result[2].ColumnTsVector, StringComparer.Ordinal);
            }
        }
    }
}
