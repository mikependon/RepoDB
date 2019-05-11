using System;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Setup;

namespace RepoDb.IntegrationTests
{
    [TestClass]
    public class ObjectNameCasingTest
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

        #region CorrectClassNameButWithImproperCasingForClassAndFields

        private class COMPLETETABLE
        {
            [Primary]
            public Guid SESSIONID { get; set; }
            public long? COLUMNBIGINT { get; set; }
            public bool? COLUMNBIT { get; set; }
            public decimal? COLUMNINT { get; set; }
            public DateTime? COLUMNDATETIME { get; set; }
            public DateTime? COLUMNDATETIME2 { get; set; }
            public string COLUMNNVARCHAR { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionCrudWithCorrectClassNameButWithImproperCasingForClassAndFields()
        {
            // Setup
            var entity = new COMPLETETABLE
            {
                SESSIONID = Guid.NewGuid(),
                COLUMNBIGINT = long.MaxValue,
                COLUMNBIT = true,
                COLUMNDATETIME2 = DateTime.Parse("1970-01-01 1:25:00.44569"),
                COLUMNDATETIME = DateTime.Parse("1970-01-01 10:30:30"),
                COLUMNINT = int.MaxValue,
                COLUMNNVARCHAR = Helper.GetAssemblyDescription()
            };

            using (var repository = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query<COMPLETETABLE>(e => e.SESSIONID == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.COLUMNBIGINT, data.COLUMNBIGINT);
                Assert.AreEqual(entity.COLUMNBIT, data.COLUMNBIT);
                Assert.AreEqual(entity.COLUMNDATETIME2, data.COLUMNDATETIME2);
                Assert.AreEqual(entity.COLUMNDATETIME, data.COLUMNDATETIME);
                Assert.AreEqual(entity.COLUMNINT, data.COLUMNINT);
                Assert.AreEqual(entity.COLUMNNVARCHAR, data.COLUMNNVARCHAR);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCrudViaInsertAllWithCorrectClassNameButWithImproperCasingForClassAndFields()
        {
            // Setup
            var entity = new COMPLETETABLE
            {
                SESSIONID = Guid.NewGuid(),
                COLUMNBIGINT = long.MaxValue,
                COLUMNBIT = true,
                COLUMNDATETIME2 = DateTime.Parse("1970-01-01 1:25:00.44569"),
                COLUMNDATETIME = DateTime.Parse("1970-01-01 10:30:30"),
                COLUMNINT = int.MaxValue,
                COLUMNNVARCHAR = Helper.GetAssemblyDescription()
            };

            using (var repository = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.InsertAll(new[] { entity });

                // Act Query
                var data = repository.Query<COMPLETETABLE>(e => e.SESSIONID == entity.SESSIONID).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.COLUMNBIGINT, data.COLUMNBIGINT);
                Assert.AreEqual(entity.COLUMNBIT, data.COLUMNBIT);
                Assert.AreEqual(entity.COLUMNDATETIME2, data.COLUMNDATETIME2);
                Assert.AreEqual(entity.COLUMNDATETIME, data.COLUMNDATETIME);
                Assert.AreEqual(entity.COLUMNINT, data.COLUMNINT);
                Assert.AreEqual(entity.COLUMNNVARCHAR, data.COLUMNNVARCHAR);
            }
        }

        #endregion

        #region MappedTableAndWithImproperCasingForClassAndFields

        [Map("COMPLETETABLE")]
        private class MappedTableAndWithImproperCasingForClassAndFieldsClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            [Map("COLUMNBIGINT")]
            public long? ColumnBigIntMapped { get; set; }
            [Map("COLUMNBIT")]
            public bool? ColumnBitMapped { get; set; }
            [Map("COLUMNINT")]
            public decimal? ColumnIntMapped { get; set; }
            [Map("COLUMNDATETIME")]
            public DateTime? ColumnDateTimeMapped { get; set; }
            [Map("COLUMNDATETIME2")]
            public DateTime? ColumnDateTime2Mapped { get; set; }
            [Map("COLUMNNVARCHAR")]
            public string ColumnNVarCharMapped { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionCrudWithMappedTableAndWithImproperCasingForClassAndFields()
        {
            // Setup
            var entity = new MappedTableAndWithImproperCasingForClassAndFieldsClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBigIntMapped = long.MaxValue,
                ColumnBitMapped = true,
                ColumnDateTime2Mapped = DateTime.Parse("1970-01-01 1:25:00.44569"),
                ColumnDateTimeMapped = DateTime.Parse("1970-01-01 10:30:30"),
                ColumnIntMapped = int.MaxValue,
                ColumnNVarCharMapped = Helper.GetAssemblyDescription()
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<MappedTableAndWithImproperCasingForClassAndFieldsClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.ColumnBigIntMapped, data.ColumnBigIntMapped);
                Assert.AreEqual(entity.ColumnBitMapped, data.ColumnBitMapped);
                Assert.AreEqual(entity.ColumnDateTime2Mapped, data.ColumnDateTime2Mapped);
                Assert.AreEqual(entity.ColumnDateTimeMapped, data.ColumnDateTimeMapped);
                Assert.AreEqual(entity.ColumnIntMapped, data.ColumnIntMapped);
                Assert.AreEqual(entity.ColumnNVarCharMapped, data.ColumnNVarCharMapped);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCrudViaInsertAllWithMappedTableAndWithImproperCasingForClassAndFields()
        {
            // Setup
            var entity = new MappedTableAndWithImproperCasingForClassAndFieldsClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBigIntMapped = long.MaxValue,
                ColumnBitMapped = true,
                ColumnDateTime2Mapped = DateTime.Parse("1970-01-01 1:25:00.44569"),
                ColumnDateTimeMapped = DateTime.Parse("1970-01-01 10:30:30"),
                ColumnIntMapped = int.MaxValue,
                ColumnNVarCharMapped = Helper.GetAssemblyDescription()
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = connection.InsertAll(new[] { entity });

                // Act Query
                var data = connection.Query<MappedTableAndWithImproperCasingForClassAndFieldsClass>(e => e.SessionId == entity.SessionId).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.ColumnBigIntMapped, data.ColumnBigIntMapped);
                Assert.AreEqual(entity.ColumnBitMapped, data.ColumnBitMapped);
                Assert.AreEqual(entity.ColumnDateTime2Mapped, data.ColumnDateTime2Mapped);
                Assert.AreEqual(entity.ColumnDateTimeMapped, data.ColumnDateTimeMapped);
                Assert.AreEqual(entity.ColumnIntMapped, data.ColumnIntMapped);
                Assert.AreEqual(entity.ColumnNVarCharMapped, data.ColumnNVarCharMapped);
            }
        }

        #endregion

        #region TestSqlConnectionCrudWithImproperCasingForClassAndFieldsViaTableName

        [TestMethod]
        public void TestSqlConnectionCrudWithImproperCasingForClassAndFieldsViaTableName()
        {
            // Setup
            var entity = new
            {
                SESSIONID = Guid.NewGuid(),
                COLUMNBIGINT = long.MaxValue,
                COLUMNBIT = true,
                COLUMNDATETIME2 = DateTime.Parse("1970-01-01 1:25:00.44569"),
                COLUMNDATETIME = DateTime.Parse("1970-01-01 10:30:30"),
                COLUMNINT = int.MaxValue,
                COLUMNNVARCHAR = Helper.GetAssemblyDescription()
            };

            using (var repository = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert("COMPLETETABLE", entity);

                // Act Query
                var data = repository.Query("COMPLETETABLE", new { SessionId = (Guid)id }).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.COLUMNBIGINT, data.ColumnBigInt);
                Assert.AreEqual(entity.COLUMNBIT, data.ColumnBit);
                Assert.AreEqual(entity.COLUMNDATETIME2, data.ColumnDateTime2);
                Assert.AreEqual(entity.COLUMNDATETIME, data.ColumnDateTime);
                Assert.AreEqual(entity.COLUMNINT, data.ColumnInt);
                Assert.AreEqual(entity.COLUMNNVARCHAR, data.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCrudViaInsertAllWithImproperCasingForClassAndFieldsViaTableName()
        {
            // Setup
            var entity = new
            {
                SESSIONID = Guid.NewGuid(),
                COLUMNBIGINT = long.MaxValue,
                COLUMNBIT = true,
                COLUMNDATETIME2 = DateTime.Parse("1970-01-01 1:25:00.44569"),
                COLUMNDATETIME = DateTime.Parse("1970-01-01 10:30:30"),
                COLUMNINT = int.MaxValue,
                COLUMNNVARCHAR = Helper.GetAssemblyDescription()
            };

            using (var repository = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.InsertAll("COMPLETETABLE",
                    new[] { entity },
                    fields: entity.GetType().GetProperties().AsFields());

                // Act Query
                var data = repository.Query("COMPLETETABLE", new { SessionId = entity.SESSIONID }).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.COLUMNBIGINT, data.ColumnBigInt);
                Assert.AreEqual(entity.COLUMNBIT, data.ColumnBit);
                Assert.AreEqual(entity.COLUMNDATETIME2, data.ColumnDateTime2);
                Assert.AreEqual(entity.COLUMNDATETIME, data.ColumnDateTime);
                Assert.AreEqual(entity.COLUMNINT, data.ColumnInt);
                Assert.AreEqual(entity.COLUMNNVARCHAR, data.ColumnNVarChar);
            }
        }

        #endregion

    }
}
