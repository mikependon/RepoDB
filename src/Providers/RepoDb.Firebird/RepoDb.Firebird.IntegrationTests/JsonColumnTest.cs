#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using FirebirdSql.Data.FirebirdClient;
using RepoDb.Firebird.IntegrationTests.Models;
using RepoDb.Firebird.IntegrationTests.Setup;
using System;
using System.Linq;

namespace RepoDb.Firebird.IntegrationTests
{
    [TestClass]
    public class JsonColumnTest
    {
        #region Private Methods

        private static FbConnection CreateConnection()
        {
            var connection = new FbConnection(Database.ConnectionString);
            connection.EnsureOpen();
            DropTable(connection);
            connection.ExecuteNonQuery(@"CREATE TABLE ""JsonSample"" (""Id"" INTEGER NOT NULL PRIMARY KEY, ""Name"" VARCHAR(100), ""Document"" BLOB SUB_TYPE TEXT, ""Tags"" BLOB SUB_TYPE TEXT)");
            return connection;
        }

        private static void DropTable(FbConnection connection)
        {
            try
            {
                connection.ExecuteNonQuery(@"DROP TABLE ""JsonSample""");
            }
            catch (Exception)
            {
                // The table does not exist yet
            }
        }

        private static JsonSampleEntity CreateCopenhagen() =>
            new JsonSampleEntity
            {
                Id = 1,
                Name = "First",
                Document = new JsonSampleDocument { Id = 5, Address = new JsonSampleAddress { City = "Copenhagen" } },
                Tags = new[] { "repo", "json" }
            };

        private static JsonSampleEntity CreateGentofte() =>
            new JsonSampleEntity
            {
                Id = 2,
                Name = "Second",
                Document = new JsonSampleDocument { Id = 6, Address = new JsonSampleAddress { City = "Gentofte" } },
                Tags = new[] { "other" }
            };

        private static void InsertSamples(FbConnection connection)
        {
            connection.Insert(CreateCopenhagen());
            connection.Insert(CreateGentofte());
        }

        #endregion

        #region Initialize

        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
        }

        #endregion

        #region Insert and Query

        [TestMethod]
        public void TestJsonColumnInsertAndQueryDocument()
        {
            using (var connection = CreateConnection())
            {
                // Setup
                InsertSamples(connection);

                // Act
                var row = connection.Query<JsonSampleEntity>(e => e.Id == 1).Single();

                // Assert
                Assert.AreEqual("Copenhagen", row.Document.Value.Address.City);
            }
        }

        [TestMethod]
        public void TestJsonColumnInsertAndQueryEmptyDocument()
        {
            using (var connection = CreateConnection())
            {
                // Setup
                connection.Insert(new JsonSampleEntity { Id = 3, Name = "Empty" });

                // Act
                var row = connection.Query<JsonSampleEntity>(e => e.Name == "Empty").Single();

                // Assert
                Assert.IsTrue(row.Document.IsEmpty);
            }
        }

        #endregion

        #region Filter

        [TestMethod]
        public void TestJsonColumnQueryByValueIsNotSupported()
        {
            using (var connection = CreateConnection())
            {
                // Setup
                InsertSamples(connection);

                // Act and Assert
                Assert.Throws<NotSupportedException>(() => connection.Query<JsonSampleEntity>(e => e.Document.Value.Id == 5).ToList());
            }
        }

        #endregion

        #region Update

        [TestMethod]
        public void TestJsonColumnUpdateDocument()
        {
            using (var connection = CreateConnection())
            {
                // Setup
                InsertSamples(connection);

                var row = connection.Query<JsonSampleEntity>(e => e.Id == 1).Single();
                row.Document = new JsonSampleDocument { Id = 7, Address = new JsonSampleAddress { City = "Aarhus" } };

                // Act
                connection.Update(row);

                // Assert
                Assert.AreEqual(7, connection.Query<JsonSampleEntity>(e => e.Id == 1).Single().Document.Value.Id);
            }
        }

        #endregion
    }
}
