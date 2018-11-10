using System.Data.SqlClient;
using System.Linq;
using RepoDb.IntegrationTests.Setup;
using Shouldly;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;

namespace RepoDb.IntegrationTests
{
    [TestClass]
    public class SpatialDataTypeMappingTest
    {
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            TypeMapper.AddMap(typeof(DateTime), DbType.DateTime2, true);
            SetupHelper.InitDatabase();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            SetupHelper.CleanDatabase();
        }

        [TestMethod]
        public void TestGeographyDataType()
        {
            // Setup
            var fixtureData = new Models.TypeMapSpatial
            {
                geography_column = "POLYGON ((0 0, 50 0, 50 50, 0 50, 0 0))"
            };

            // Act
            var sut = new DbRepository<SqlConnection>(Constants.TestDatabase);
            var returnedId = sut.Insert(fixtureData);

            // Assert
            var saveData = sut.Query<Models.TypeMapSpatial>(top: 1).FirstOrDefault();
            saveData.ShouldNotBeNull();
            saveData.geography_column.ToString().ShouldBe(fixtureData.geography_column);
        }

        [TestMethod]
        public void TestGeometryDataType()
        {
            // Setup
            var fixtureData = new Models.TypeMapSpatial
            {
                geometry_column = "LINESTRING (-122.36 47.656, -122.343 47.656)"
            };

            // Act
            var sut = new DbRepository<SqlConnection>(Constants.TestDatabase);
            var returnedId = sut.Insert(fixtureData);

            //TODO: support guid primary key

            // Assert
            var saveData = sut.Query<Models.TypeMapSpatial>(top: 1).FirstOrDefault();
            saveData.ShouldNotBeNull();
            saveData.geometry_column.ToString().ShouldBe(fixtureData.geometry_column);
        }
    }
}