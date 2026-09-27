#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.AuroraDb.PostgreSql.IntegrationTests.Setup;
using RepoDb.Connector.AuroraDb.Npgsql;
using RepoDb.Resolvers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.AuroraDb.PostgreSql.IntegrationTests
{
    /// <summary>
    /// Tests for the PostGIS spatial (GEOMETRY, GEOGRAPHY) columns. The values are exchanged in their Well-Known Text (WKT)
    /// form through the PostGIS functions, as reading or writing them natively requires an Npgsql spatial plugin (i.e. NetTopologySuite).
    /// </summary>
    [TestClass]
    public class SpatialTests
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

        #region SubClasses

        [Map("SpatialTable")]
        private class SpatialTableAsText
        {
            public long Id { get; set; }
            public string ColumnGeometry { get; set; }
            public string ColumnGeography { get; set; }
        }

        [Map("SpatialTable")]
        private class SpatialTableAsObject
        {
            public long Id { get; set; }
            public object ColumnGeometry { get; set; }
        }

        #endregion

        #region Helpers

        private const string InsertCommandText = @"INSERT INTO ""SpatialTable"" (""ColumnGeometry"", ""ColumnGeography"")
            VALUES (ST_GeomFromText(@Geometry, 4326), ST_GeogFromText(@Geography))
            RETURNING ""Id"";";

        private const string QueryCommandText = @"SELECT ""Id"",
                ST_AsText(""ColumnGeometry"") AS ""ColumnGeometry"",
                ST_AsText(""ColumnGeography"") AS ""ColumnGeography""
            FROM ""SpatialTable""
            WHERE ""Id"" = @Id;";

        #endregion

        #region Positive

        [TestMethod]
        public void TestSpatialInsertAndQueryViaWellKnownText()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var id = connection.ExecuteScalar<long>(InsertCommandText,
                    new { Geometry = "POINT(1 2)", Geography = "POINT(121.0 14.5)" });
                var result = connection.ExecuteQuery<SpatialTableAsText>(QueryCommandText, new { Id = id }).First();

                // Assert
                Assert.AreEqual(id, result.Id);
                Assert.AreEqual("POINT(1 2)", result.ColumnGeometry);
                Assert.AreEqual("POINT(121 14.5)", result.ColumnGeography);
            }
        }

        [TestMethod]
        public async Task TestSpatialInsertAndQueryViaWellKnownTextAsync()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var id = await connection.ExecuteScalarAsync<long>(InsertCommandText,
                    new { Geometry = "LINESTRING(0 0,1 1,2 1)", Geography = "POINT(0 0)" }).ConfigureAwait(false);
                var result = (await connection.ExecuteQueryAsync<SpatialTableAsText>(QueryCommandText, new { Id = id }).ConfigureAwait(false)).First();

                // Assert
                Assert.AreEqual("LINESTRING(0 0,1 1,2 1)", result.ColumnGeometry);
                Assert.AreEqual("POINT(0 0)", result.ColumnGeography);
            }
        }

        [TestMethod]
        public void TestSpatialInsertAndQueryWithNullValues()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var id = connection.ExecuteScalar<long>(InsertCommandText,
                    new { Geometry = (string)null, Geography = (string)null });
                var result = connection.ExecuteQuery<SpatialTableAsText>(QueryCommandText, new { Id = id }).First();

                // Assert
                Assert.IsNull(result.ColumnGeometry);
                Assert.IsNull(result.ColumnGeography);
            }
        }

        [TestMethod]
        public void TestSpatialQueryWithSpatialFunctionInTheFilter()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Setup
                connection.ExecuteScalar<long>(InsertCommandText, new { Geometry = "POINT(1 1)", Geography = "POINT(0 0)" });
                connection.ExecuteScalar<long>(InsertCommandText, new { Geometry = "POINT(50 50)", Geography = "POINT(0 0)" });

                // Act
                var count = connection.ExecuteScalar<long>(@"SELECT COUNT(*) FROM ""SpatialTable""
                    WHERE ST_DWithin(""ColumnGeometry"", ST_GeomFromText(@Origin, 4326), @Distance);",
                    new { Origin = "POINT(0 0)", Distance = 5.0 });

                // Assert
                Assert.AreEqual(1, count);
            }
        }

        [TestMethod]
        public void TestSpatialCountAndDeleteViaRepoDbOperations()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Setup
                var id = connection.ExecuteScalar<long>(InsertCommandText, new { Geometry = "POINT(1 2)", Geography = "POINT(0 0)" });

                // Act
                var countBefore = connection.CountAll("SpatialTable");
                var deleted = connection.Delete("SpatialTable", id);
                var countAfter = connection.CountAll("SpatialTable");

                // Assert
                Assert.AreEqual(1, countBefore);
                Assert.AreEqual(1, deleted);
                Assert.AreEqual(0, countAfter);
            }
        }

        [TestMethod]
        public void TestSpatialInsertViaRepoDbWithWellKnownText()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act (PostGIS implicitly casts a TEXT parameter into a GEOMETRY)
                var id = connection.Insert<long>("SpatialTable", new
                {
                    ColumnGeometry = "SRID=4326;POLYGON((0 0,0 1,1 1,1 0,0 0))"
                });
                var result = connection.ExecuteQuery<SpatialTableAsText>(QueryCommandText, new { Id = id }).First();

                // Assert
                Assert.AreEqual("POLYGON((0 0,0 1,1 1,1 0,0 0))", result.ColumnGeometry);
                Assert.IsNull(result.ColumnGeography);
                Assert.AreEqual(4326, connection.ExecuteScalar<int>(@"SELECT ST_SRID(""ColumnGeometry"") FROM ""SpatialTable"" WHERE ""Id"" = @Id;", new { Id = id }));
            }
        }

        [TestMethod]
        public void TestSpatialDbHelperGetFields()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var helper = connection.GetDbHelper();
                var fields = helper.GetFields(connection, "SpatialTable", null).ToList();

                // Assert
                Assert.AreEqual(3, fields.Count);
                var geometry = fields.FirstOrDefault(f => f.Name == "ColumnGeometry");
                var geography = fields.FirstOrDefault(f => f.Name == "ColumnGeography");
                Assert.IsNotNull(geometry);
                Assert.IsNotNull(geography);
                Assert.IsTrue(geometry.IsNullable);
                Assert.AreEqual(typeof(object), geometry.Type);
                Assert.AreEqual(typeof(object), geography.Type);
            }
        }

        [TestMethod]
        public void TestSpatialAuroraDbTypeResolution()
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToAuroraDbTypeResolver();

            // Act/Assert
            Assert.AreEqual(AuroraDbType.Geometry, resolver.Resolve("geometry"));
            Assert.AreEqual(AuroraDbType.Geography, resolver.Resolve("geography"));
        }

        #endregion

        #region Negative

        [TestMethod]
        public void ThrowExceptionOnSpatialInsertIfTheWellKnownTextIsInvalid()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act/Assert
                var exception = Assert.Throws<AuroraDbException>(() =>
                    connection.ExecuteScalar<long>(InsertCommandText, new { Geometry = "NOT A GEOMETRY", Geography = "POINT(0 0)" }));
                Assert.AreEqual("XX000", exception.SqlState);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnSpatialInsertViaRepoDbIfTheWellKnownTextIsInvalid()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act/Assert
                var exception = Assert.Throws<AuroraDbException>(() =>
                    connection.Insert("SpatialTable", new { ColumnGeometry = "POINT(1" }));
                Assert.AreEqual("XX000", exception.SqlState);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnSpatialInsertViaRepoDbIfTheGeographyIsAPlainText()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act/Assert (unlike GEOMETRY, there is no implicit cast from TEXT into a GEOGRAPHY)
                var exception = Assert.Throws<AuroraDbException>(() =>
                    connection.Insert("SpatialTable", new { ColumnGeography = "POINT(10 20)" }));
                Assert.AreEqual("42804", exception.SqlState);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnSpatialQueryIfTheColumnIsReadNativelyWithoutASpatialPlugin()
        {
            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Setup
                connection.ExecuteScalar<long>(InsertCommandText, new { Geometry = "POINT(1 2)", Geography = "POINT(0 0)" });

                // Act/Assert (reading a GEOMETRY value requires the Npgsql.NetTopologySuite plugin)
                Assert.Throws<InvalidCastException>(() =>
                    connection.QueryAll<SpatialTableAsObject>().ToList());
            }
        }

        #endregion
    }
}
