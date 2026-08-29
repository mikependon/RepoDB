using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Setup;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace RepoDb.IntegrationTests
{
    [TestClass]
    public class ObjectMappingViaDataAnnotationsTest
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

        [Table("[dbo].[COMPLETETABLE]")]
        private class MappedCompleteTable
        {
            [Column("SESSIONID"), Key]
            public Guid SessionIdMapped { get; set; }
            [Column("COLUMNBIGINT")]
            public long? ColumnBigIntMapped { get; set; }
            [Column("COLUMNBIT")]
            public bool? ColumnBitMapped { get; set; }
            [Column("COLUMNINT")]
            public int? ColumnIntMapped { get; set; }
            [Column("COLUMNDATETIME")]
            public DateTime? ColumnDateTimeMapped { get; set; }
            [Column("COLUMNDATETIME2")]
            public DateTime? ColumnDateTime2Mapped { get; set; }
            [Column("[COLUMNNVARCHAR]")]
            public string ColumnNVarCharMapped { get; set; }
        }

        [Table("[sc].[IDENTITYTABLE]")]
        private class MappedIdentityTable
        {
            [Column("ID"), Key]
            public long IdMapped { get; set; }
            [Column("ROWGUID")]
            public Guid RowGuidMapped { get; set; }
            [Column("COLUMNBIT")]
            public bool? ColumnBitMapped { get; set; }
            [Column("COLUMNDATETIME")]
            public DateTime? ColumnDateTimeMapped { get; set; }
            [Column("COLUMNDATETIME2")]
            public DateTime? ColumnDateTime2Mapped { get; set; }
            [Column("COLUMNFLOAT")]
            public double? ColumnFloatMapped { get; set; }
            [Column("COLUMNINT")]
            public int? ColumnIntMapped { get; set; }
            [Column("[COLUMNNVARCHAR]")]
            public string ColumnNVarCharMapped { get; set; }
        }

        [Table("[dbo].[COMPLETETABLE]")]
        private class MappedCompleteTableForKey
        {
            [Column("SESSIONID")]
            public Guid IdMapped { get; set; }
        }

        #endregion

        #region Methods

        private MappedCompleteTable GetMappedCompleteTable()
        {
            return new MappedCompleteTable
            {
                SessionIdMapped = Guid.NewGuid(),
                ColumnBigIntMapped = long.MaxValue,
                ColumnBitMapped = true,
                ColumnDateTime2Mapped = DateTime.Parse("1970-01-01 1:25:00.44569"),
                ColumnDateTimeMapped = DateTime.Parse("1970-01-01 10:30:30"),
                ColumnIntMapped = int.MaxValue,
                ColumnNVarCharMapped = Helper.GetAssemblyDescription()
            };
        }

        private IEnumerable<MappedCompleteTable> GetMappedCompleteTables(int count = 10)
        {
            var random = new Random();
            for (var i = 0; i < count; i++)
            {
                yield return new MappedCompleteTable
                {
                    SessionIdMapped = Guid.NewGuid(),
                    ColumnBigIntMapped = long.MaxValue,
                    ColumnBitMapped = true,
                    ColumnDateTime2Mapped = DateTime.Parse("1970-01-01 1:25:00.44569").AddMonths(random.Next(100)),
                    ColumnDateTimeMapped = DateTime.Parse("1970-01-01 10:30:30").AddMonths(random.Next(100)),
                    ColumnIntMapped = int.MaxValue,
                    ColumnNVarCharMapped = Helper.GetAssemblyDescription()
                };
            }
        }

        private MappedIdentityTable GetMappedIdentityTable()
        {
            return new MappedIdentityTable
            {
                RowGuidMapped = Guid.NewGuid(),
                ColumnFloatMapped = double.MaxValue,
                ColumnBitMapped = true,
                ColumnDateTime2Mapped = DateTime.Parse("1970-01-01 1:25:00.44569"),
                ColumnDateTimeMapped = DateTime.Parse("1970-01-01 10:30:30"),
                ColumnIntMapped = int.MaxValue,
                ColumnNVarCharMapped = Helper.GetAssemblyDescription()
            };
        }

        private IEnumerable<MappedIdentityTable> GetMappedIdentityTables(int count = 10)
        {
            var random = new Random();
            for (var i = 0; i < count; i++)
            {
                yield return new MappedIdentityTable
                {
                    RowGuidMapped = Guid.NewGuid(),
                    ColumnFloatMapped = double.MaxValue,
                    ColumnBitMapped = true,
                    ColumnDateTime2Mapped = DateTime.Parse("1970-01-01 1:25:00.44569"),
                    ColumnDateTimeMapped = DateTime.Parse("1970-01-01 10:30:30"),
                    ColumnIntMapped = int.MaxValue,
                    ColumnNVarCharMapped = Helper.GetAssemblyDescription()
                };
            }
        }

        #endregion

        #region KeyTest

        #region InsertAndDeleteByKey

        [TestMethod]
        public void TestSqlConnectionObjectMappingViaDataAnnotationsInsertAndDeleteByKey()
        {
            // Setup
            var entity = GetMappedCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var result = connection.Delete<MappedCompleteTableForKey>(id);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionObjectMappingViaDataAnnotationsInsertAsyncAndDeleteByKey()
        {
            // Setup
            var entity = GetMappedCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = await connection.InsertAsync(entity);

                // Act Query
                var result = await connection.DeleteAsync<MappedCompleteTableForKey>(id);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        #endregion

        #region InsertAndExistsByKey

        [TestMethod]
        public void TestSqlConnectionObjectMappingViaDataAnnotationsInsertAndExistsByKey()
        {
            // Setup
            var entity = GetMappedCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var result = connection.Exists<MappedCompleteTableForKey>(id);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionObjectMappingViaDataAnnotationsInsertAsyncAndExistsByKey()
        {
            // Setup
            var entity = GetMappedCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = await connection.InsertAsync(entity);

                // Act Query
                var result = await connection.ExistsAsync<MappedCompleteTableForKey>(id);

                // Assert
                Assert.IsTrue(result);
            }
        }

        #endregion

        #region InsertAndQueryByKey

        [TestMethod]
        public void TestSqlConnectionObjectMappingViaDataAnnotationsInsertAndQueryByKey()
        {
            // Setup
            var entity = GetMappedCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var result = connection.Query<MappedCompleteTable>(id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionObjectMappingViaDataAnnotationsInsertAsyncAndQueryByKey()
        {
            // Setup
            var entity = GetMappedCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = await connection.InsertAsync(entity);

                // Act Query
                var result = (await connection.QueryAsync<MappedCompleteTable>(id)).FirstOrDefault();

                // Assert
                Assert.IsNotNull(result);
            }
        }

        #endregion

        #endregion

        #region ForIdentity

        #region InsertAndQueryForIdentity

        [TestMethod]
        public void TestSqlConnectionObjectMappingViaDataAnnotationsInsertAndQueryForIdentityViaDynamic()
        {
            // Setup
            var entity = GetMappedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<MappedIdentityTable>(new { Id = id }).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.RowGuidMapped, data.RowGuidMapped);
                Assert.AreEqual(entity.ColumnBitMapped, data.ColumnBitMapped);
                Assert.AreEqual(entity.ColumnDateTime2Mapped, data.ColumnDateTime2Mapped);
                Assert.AreEqual(entity.ColumnDateTimeMapped, data.ColumnDateTimeMapped);
                Assert.AreEqual(entity.ColumnFloatMapped, data.ColumnFloatMapped);
                Assert.AreEqual(entity.ColumnIntMapped, data.ColumnIntMapped);
                Assert.AreEqual(entity.ColumnNVarCharMapped, data.ColumnNVarCharMapped);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionObjectMappingViaDataAnnotationsInsertAsyncAndQueryForIdentityViaDynamic()
        {
            // Setup
            var entity = GetMappedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = await connection.InsertAsync(entity);

                // Act Query
                var data = (await connection.QueryAsync<MappedIdentityTable>(new { Id = id })).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.RowGuidMapped, data.RowGuidMapped);
                Assert.AreEqual(entity.ColumnBitMapped, data.ColumnBitMapped);
                Assert.AreEqual(entity.ColumnDateTime2Mapped, data.ColumnDateTime2Mapped);
                Assert.AreEqual(entity.ColumnDateTimeMapped, data.ColumnDateTimeMapped);
                Assert.AreEqual(entity.ColumnFloatMapped, data.ColumnFloatMapped);
                Assert.AreEqual(entity.ColumnIntMapped, data.ColumnIntMapped);
                Assert.AreEqual(entity.ColumnNVarCharMapped, data.ColumnNVarCharMapped);
            }
        }

        [TestMethod]
        public void TestSqlConnectionObjectMappingViaDataAnnotationsInsertAndQueryForIdentityViaExpression()
        {
            // Setup
            var entity = GetMappedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<MappedIdentityTable>(e => e.IdMapped == (long)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.RowGuidMapped, data.RowGuidMapped);
                Assert.AreEqual(entity.ColumnBitMapped, data.ColumnBitMapped);
                Assert.AreEqual(entity.ColumnDateTime2Mapped, data.ColumnDateTime2Mapped);
                Assert.AreEqual(entity.ColumnDateTimeMapped, data.ColumnDateTimeMapped);
                Assert.AreEqual(entity.ColumnFloatMapped, data.ColumnFloatMapped);
                Assert.AreEqual(entity.ColumnIntMapped, data.ColumnIntMapped);
                Assert.AreEqual(entity.ColumnNVarCharMapped, data.ColumnNVarCharMapped);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionObjectMappingViaDataAnnotationsInsertAsyncAndQueryForIdentityViaExpression()
        {
            // Setup
            var entity = GetMappedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = await connection.InsertAsync(entity);

                // Act Query
                var data = (await connection.QueryAsync<MappedIdentityTable>(e => e.IdMapped == (long)id)).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.RowGuidMapped, data.RowGuidMapped);
                Assert.AreEqual(entity.ColumnBitMapped, data.ColumnBitMapped);
                Assert.AreEqual(entity.ColumnDateTime2Mapped, data.ColumnDateTime2Mapped);
                Assert.AreEqual(entity.ColumnDateTimeMapped, data.ColumnDateTimeMapped);
                Assert.AreEqual(entity.ColumnFloatMapped, data.ColumnFloatMapped);
                Assert.AreEqual(entity.ColumnIntMapped, data.ColumnIntMapped);
                Assert.AreEqual(entity.ColumnNVarCharMapped, data.ColumnNVarCharMapped);
            }
        }

        [TestMethod]
        public void TestSqlConnectionObjectMappingViaDataAnnotationsInsertAndQueryForIdentityViaQueryField()
        {
            // Setup
            var entity = GetMappedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<MappedIdentityTable>(new QueryField("Id", id)).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.RowGuidMapped, data.RowGuidMapped);
                Assert.AreEqual(entity.ColumnBitMapped, data.ColumnBitMapped);
                Assert.AreEqual(entity.ColumnDateTime2Mapped, data.ColumnDateTime2Mapped);
                Assert.AreEqual(entity.ColumnDateTimeMapped, data.ColumnDateTimeMapped);
                Assert.AreEqual(entity.ColumnFloatMapped, data.ColumnFloatMapped);
                Assert.AreEqual(entity.ColumnIntMapped, data.ColumnIntMapped);
                Assert.AreEqual(entity.ColumnNVarCharMapped, data.ColumnNVarCharMapped);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionObjectMappingViaDataAnnotationsInsertAsyncAndQueryForIdentityViaQueryField()
        {
            // Setup
            var entity = GetMappedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = await connection.InsertAsync(entity);

                // Act Query
                var data = (await connection.QueryAsync<MappedIdentityTable>(new QueryField("Id", id))).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.RowGuidMapped, data.RowGuidMapped);
                Assert.AreEqual(entity.ColumnBitMapped, data.ColumnBitMapped);
                Assert.AreEqual(entity.ColumnDateTime2Mapped, data.ColumnDateTime2Mapped);
                Assert.AreEqual(entity.ColumnDateTimeMapped, data.ColumnDateTimeMapped);
                Assert.AreEqual(entity.ColumnFloatMapped, data.ColumnFloatMapped);
                Assert.AreEqual(entity.ColumnIntMapped, data.ColumnIntMapped);
                Assert.AreEqual(entity.ColumnNVarCharMapped, data.ColumnNVarCharMapped);
            }
        }

        [TestMethod]
        public void TestSqlConnectionObjectMappingViaDataAnnotationsInsertAndQueryForIdentityViaQueryFields()
        {
            // Setup
            var entity = GetMappedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<MappedIdentityTable>(new QueryField("Id", id).AsEnumerable()).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.RowGuidMapped, data.RowGuidMapped);
                Assert.AreEqual(entity.ColumnBitMapped, data.ColumnBitMapped);
                Assert.AreEqual(entity.ColumnDateTime2Mapped, data.ColumnDateTime2Mapped);
                Assert.AreEqual(entity.ColumnDateTimeMapped, data.ColumnDateTimeMapped);
                Assert.AreEqual(entity.ColumnFloatMapped, data.ColumnFloatMapped);
                Assert.AreEqual(entity.ColumnIntMapped, data.ColumnIntMapped);
                Assert.AreEqual(entity.ColumnNVarCharMapped, data.ColumnNVarCharMapped);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionObjectMappingViaDataAnnotationsInsertAsyncAndQueryForIdentityViaQueryFields()
        {
            // Setup
            var entity = GetMappedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = await connection.InsertAsync(entity);

                // Act Query
                var data = (await connection.QueryAsync<MappedIdentityTable>(new QueryField("Id", id).AsEnumerable())).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.RowGuidMapped, data.RowGuidMapped);
                Assert.AreEqual(entity.ColumnBitMapped, data.ColumnBitMapped);
                Assert.AreEqual(entity.ColumnDateTime2Mapped, data.ColumnDateTime2Mapped);
                Assert.AreEqual(entity.ColumnDateTimeMapped, data.ColumnDateTimeMapped);
                Assert.AreEqual(entity.ColumnFloatMapped, data.ColumnFloatMapped);
                Assert.AreEqual(entity.ColumnIntMapped, data.ColumnIntMapped);
                Assert.AreEqual(entity.ColumnNVarCharMapped, data.ColumnNVarCharMapped);
            }
        }

        [TestMethod]
        public void TestSqlConnectionObjectMappingViaDataAnnotationsInsertAndQueryForIdentityViaQueryGroup()
        {
            // Setup
            var entity = GetMappedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<MappedIdentityTable>(new QueryGroup(new QueryField("Id", id))).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.RowGuidMapped, data.RowGuidMapped);
                Assert.AreEqual(entity.ColumnBitMapped, data.ColumnBitMapped);
                Assert.AreEqual(entity.ColumnDateTime2Mapped, data.ColumnDateTime2Mapped);
                Assert.AreEqual(entity.ColumnDateTimeMapped, data.ColumnDateTimeMapped);
                Assert.AreEqual(entity.ColumnFloatMapped, data.ColumnFloatMapped);
                Assert.AreEqual(entity.ColumnIntMapped, data.ColumnIntMapped);
                Assert.AreEqual(entity.ColumnNVarCharMapped, data.ColumnNVarCharMapped);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionObjectMappingViaDataAnnotationsInsertAsyncAndQueryForIdentityViaQueryGroup()
        {
            // Setup
            var entity = GetMappedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = await connection.InsertAsync(entity);

                // Act Query
                var data = (await connection.QueryAsync<MappedIdentityTable>(new QueryGroup(new QueryField("Id", id)))).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.RowGuidMapped, data.RowGuidMapped);
                Assert.AreEqual(entity.ColumnBitMapped, data.ColumnBitMapped);
                Assert.AreEqual(entity.ColumnDateTime2Mapped, data.ColumnDateTime2Mapped);
                Assert.AreEqual(entity.ColumnDateTimeMapped, data.ColumnDateTimeMapped);
                Assert.AreEqual(entity.ColumnFloatMapped, data.ColumnFloatMapped);
                Assert.AreEqual(entity.ColumnIntMapped, data.ColumnIntMapped);
                Assert.AreEqual(entity.ColumnNVarCharMapped, data.ColumnNVarCharMapped);
            }
        }

        #endregion

        #region InsertAndUpdateAndQueryrNonIdentity

        [TestMethod]
        public void TestSqlConnectionObjectMappingViaDataAnnotationsInsertAndUpdateAndQueryForIdentityViaDynamic()
        {
            // Setup
            var entity = GetMappedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Setup
                entity.ColumnNVarCharMapped = $"{entity.ColumnNVarCharMapped} (Updated)";

                // Act Update
                var affectedRows = connection.Update<MappedIdentityTable>(entity, new { Id = id });

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act Query
                var data = connection.Query<MappedIdentityTable>(new { Id = id }).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.RowGuidMapped, data.RowGuidMapped);
                Assert.AreEqual(entity.ColumnBitMapped, data.ColumnBitMapped);
                Assert.AreEqual(entity.ColumnDateTime2Mapped, data.ColumnDateTime2Mapped);
                Assert.AreEqual(entity.ColumnDateTimeMapped, data.ColumnDateTimeMapped);
                Assert.AreEqual(entity.ColumnFloatMapped, data.ColumnFloatMapped);
                Assert.AreEqual(entity.ColumnIntMapped, data.ColumnIntMapped);
                Assert.AreEqual(entity.ColumnNVarCharMapped, data.ColumnNVarCharMapped);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionObjectMappingViaDataAnnotationsInsertAsyncAndUpdateAndQueryForIdentityViaDynamic()
        {
            // Setup
            var entity = GetMappedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = await connection.InsertAsync(entity);

                // Setup
                entity.ColumnNVarCharMapped = $"{entity.ColumnNVarCharMapped} (Updated)";

                // Act Update
                var affectedRows = await connection.UpdateAsync<MappedIdentityTable>(entity, new { Id = id });

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act Query
                var data = (await connection.QueryAsync<MappedIdentityTable>(new { Id = id })).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.RowGuidMapped, data.RowGuidMapped);
                Assert.AreEqual(entity.ColumnBitMapped, data.ColumnBitMapped);
                Assert.AreEqual(entity.ColumnDateTime2Mapped, data.ColumnDateTime2Mapped);
                Assert.AreEqual(entity.ColumnDateTimeMapped, data.ColumnDateTimeMapped);
                Assert.AreEqual(entity.ColumnFloatMapped, data.ColumnFloatMapped);
                Assert.AreEqual(entity.ColumnIntMapped, data.ColumnIntMapped);
                Assert.AreEqual(entity.ColumnNVarCharMapped, data.ColumnNVarCharMapped);
            }
        }

        [TestMethod]
        public void TestSqlConnectionObjectMappingViaDataAnnotationsInsertAndUpdateAndQueryForIdentityViaExpression()
        {
            // Setup
            var entity = GetMappedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Setup
                entity.ColumnNVarCharMapped = $"{entity.ColumnNVarCharMapped} (Updated)";

                // Act Update
                var affectedRows = connection.Update<MappedIdentityTable>(entity, c => c.IdMapped == (long)id);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act Query
                var data = connection.Query<MappedIdentityTable>(new { Id = id }).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.RowGuidMapped, data.RowGuidMapped);
                Assert.AreEqual(entity.ColumnBitMapped, data.ColumnBitMapped);
                Assert.AreEqual(entity.ColumnDateTime2Mapped, data.ColumnDateTime2Mapped);
                Assert.AreEqual(entity.ColumnDateTimeMapped, data.ColumnDateTimeMapped);
                Assert.AreEqual(entity.ColumnFloatMapped, data.ColumnFloatMapped);
                Assert.AreEqual(entity.ColumnIntMapped, data.ColumnIntMapped);
                Assert.AreEqual(entity.ColumnNVarCharMapped, data.ColumnNVarCharMapped);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionObjectMappingViaDataAnnotationsInsertAsyncAndUpdateAndQueryForIdentityViaExpression()
        {
            // Setup
            var entity = GetMappedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = await connection.InsertAsync(entity);

                // Setup
                entity.ColumnNVarCharMapped = $"{entity.ColumnNVarCharMapped} (Updated)";

                // Act Update
                var affectedRows = await connection.UpdateAsync<MappedIdentityTable>(entity, c => c.IdMapped == (long)id);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act Query
                var data = (await connection.QueryAsync<MappedIdentityTable>(new { Id = id })).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.RowGuidMapped, data.RowGuidMapped);
                Assert.AreEqual(entity.ColumnBitMapped, data.ColumnBitMapped);
                Assert.AreEqual(entity.ColumnDateTime2Mapped, data.ColumnDateTime2Mapped);
                Assert.AreEqual(entity.ColumnDateTimeMapped, data.ColumnDateTimeMapped);
                Assert.AreEqual(entity.ColumnFloatMapped, data.ColumnFloatMapped);
                Assert.AreEqual(entity.ColumnIntMapped, data.ColumnIntMapped);
                Assert.AreEqual(entity.ColumnNVarCharMapped, data.ColumnNVarCharMapped);
            }
        }

        [TestMethod]
        public void TestSqlConnectionObjectMappingViaDataAnnotationsInsertAndUpdateAndQueryForIdentityViaQueryField()
        {
            // Setup
            var entity = GetMappedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Setup
                var field = new QueryField("Id", id);
                entity.ColumnNVarCharMapped = $"{entity.ColumnNVarCharMapped} (Updated)";

                // Act Update
                var affectedRows = connection.Update<MappedIdentityTable>(entity, field);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act Query
                field.Reset();
                var data = connection.Query<MappedIdentityTable>(field).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.RowGuidMapped, data.RowGuidMapped);
                Assert.AreEqual(entity.ColumnBitMapped, data.ColumnBitMapped);
                Assert.AreEqual(entity.ColumnDateTime2Mapped, data.ColumnDateTime2Mapped);
                Assert.AreEqual(entity.ColumnDateTimeMapped, data.ColumnDateTimeMapped);
                Assert.AreEqual(entity.ColumnFloatMapped, data.ColumnFloatMapped);
                Assert.AreEqual(entity.ColumnIntMapped, data.ColumnIntMapped);
                Assert.AreEqual(entity.ColumnNVarCharMapped, data.ColumnNVarCharMapped);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionObjectMappingViaDataAnnotationsInsertAsyncAndUpdateAndQueryForIdentityViaQueryField()
        {
            // Setup
            var entity = GetMappedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = await connection.InsertAsync(entity);

                // Setup
                var field = new QueryField("Id", id);
                entity.ColumnNVarCharMapped = $"{entity.ColumnNVarCharMapped} (Updated)";

                // Act Update
                var affectedRows = await connection.UpdateAsync<MappedIdentityTable>(entity, field);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act Query
                field.Reset();
                var data = (await connection.QueryAsync<MappedIdentityTable>(field)).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.RowGuidMapped, data.RowGuidMapped);
                Assert.AreEqual(entity.ColumnBitMapped, data.ColumnBitMapped);
                Assert.AreEqual(entity.ColumnDateTime2Mapped, data.ColumnDateTime2Mapped);
                Assert.AreEqual(entity.ColumnDateTimeMapped, data.ColumnDateTimeMapped);
                Assert.AreEqual(entity.ColumnFloatMapped, data.ColumnFloatMapped);
                Assert.AreEqual(entity.ColumnIntMapped, data.ColumnIntMapped);
                Assert.AreEqual(entity.ColumnNVarCharMapped, data.ColumnNVarCharMapped);
            }
        }

        [TestMethod]
        public void TestSqlConnectionObjectMappingViaDataAnnotationsInsertAndUpdateAndQueryForIdentityViaQueryFields()
        {
            // Setup
            var entity = GetMappedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Setup
                var fields = new QueryField("Id", id).AsEnumerable();
                entity.ColumnNVarCharMapped = $"{entity.ColumnNVarCharMapped} (Updated)";

                // Act Update
                var affectedRows = connection.Update<MappedIdentityTable>(entity, fields);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act Query
                fields.ResetAll();
                var data = connection.Query<MappedIdentityTable>(fields).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.RowGuidMapped, data.RowGuidMapped);
                Assert.AreEqual(entity.ColumnBitMapped, data.ColumnBitMapped);
                Assert.AreEqual(entity.ColumnDateTime2Mapped, data.ColumnDateTime2Mapped);
                Assert.AreEqual(entity.ColumnDateTimeMapped, data.ColumnDateTimeMapped);
                Assert.AreEqual(entity.ColumnFloatMapped, data.ColumnFloatMapped);
                Assert.AreEqual(entity.ColumnIntMapped, data.ColumnIntMapped);
                Assert.AreEqual(entity.ColumnNVarCharMapped, data.ColumnNVarCharMapped);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionObjectMappingViaDataAnnotationsInsertAsyncAndUpdateAndQueryForIdentityViaQueryFields()
        {
            // Setup
            var entity = GetMappedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = await connection.InsertAsync(entity);

                // Setup
                var fields = new QueryField("Id", id).AsEnumerable();
                entity.ColumnNVarCharMapped = $"{entity.ColumnNVarCharMapped} (Updated)";

                // Act Update
                var affectedRows = await connection.UpdateAsync<MappedIdentityTable>(entity, fields);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act Query
                fields.ResetAll();
                var data = (await connection.QueryAsync<MappedIdentityTable>(fields)).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.RowGuidMapped, data.RowGuidMapped);
                Assert.AreEqual(entity.ColumnBitMapped, data.ColumnBitMapped);
                Assert.AreEqual(entity.ColumnDateTime2Mapped, data.ColumnDateTime2Mapped);
                Assert.AreEqual(entity.ColumnDateTimeMapped, data.ColumnDateTimeMapped);
                Assert.AreEqual(entity.ColumnFloatMapped, data.ColumnFloatMapped);
                Assert.AreEqual(entity.ColumnIntMapped, data.ColumnIntMapped);
                Assert.AreEqual(entity.ColumnNVarCharMapped, data.ColumnNVarCharMapped);
            }
        }

        [TestMethod]
        public void TestSqlConnectionObjectMappingViaDataAnnotationsInsertAndUpdateAndQueryForIdentityViaQueryGroup()
        {
            // Setup
            var entity = GetMappedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Setup
                var queryGroup = new QueryGroup(new QueryField("Id", id));
                entity.ColumnNVarCharMapped = $"{entity.ColumnNVarCharMapped} (Updated)";

                // Act Update
                var affectedRows = connection.Update<MappedIdentityTable>(entity, queryGroup);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act Query
                queryGroup.Reset();
                var data = connection.Query<MappedIdentityTable>(queryGroup).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.RowGuidMapped, data.RowGuidMapped);
                Assert.AreEqual(entity.ColumnBitMapped, data.ColumnBitMapped);
                Assert.AreEqual(entity.ColumnDateTime2Mapped, data.ColumnDateTime2Mapped);
                Assert.AreEqual(entity.ColumnDateTimeMapped, data.ColumnDateTimeMapped);
                Assert.AreEqual(entity.ColumnFloatMapped, data.ColumnFloatMapped);
                Assert.AreEqual(entity.ColumnIntMapped, data.ColumnIntMapped);
                Assert.AreEqual(entity.ColumnNVarCharMapped, data.ColumnNVarCharMapped);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionObjectMappingViaDataAnnotationsInsertAsyncAndUpdateAndQueryForIdentityViaQueryGroup()
        {
            // Setup
            var entity = GetMappedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = await connection.InsertAsync(entity);

                // Setup
                var queryGroup = new QueryGroup(new QueryField("Id", id));
                entity.ColumnNVarCharMapped = $"{entity.ColumnNVarCharMapped} (Updated)";

                // Act Update
                var affectedRows = await connection.UpdateAsync<MappedIdentityTable>(entity, queryGroup);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act Query
                queryGroup.Reset();
                var data = (await connection.QueryAsync<MappedIdentityTable>(queryGroup)).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.RowGuidMapped, data.RowGuidMapped);
                Assert.AreEqual(entity.ColumnBitMapped, data.ColumnBitMapped);
                Assert.AreEqual(entity.ColumnDateTime2Mapped, data.ColumnDateTime2Mapped);
                Assert.AreEqual(entity.ColumnDateTimeMapped, data.ColumnDateTimeMapped);
                Assert.AreEqual(entity.ColumnFloatMapped, data.ColumnFloatMapped);
                Assert.AreEqual(entity.ColumnIntMapped, data.ColumnIntMapped);
                Assert.AreEqual(entity.ColumnNVarCharMapped, data.ColumnNVarCharMapped);
            }
        }

        #endregion

        #region InsertAndMergeAndQueryForIdentity

        [TestMethod]
        public void TestSqlConnectionObjectMappingViaDataAnnotationsInsertAndMergeAndQueryAndQueryForIdentity()
        {
            // Setup
            var entity = GetMappedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Setup
                entity.ColumnNVarCharMapped = $"{entity.ColumnNVarCharMapped} (Merged)";

                // Act Update
                var mergeResult = connection.Merge<MappedIdentityTable>(entity,
                    qualifiers: Field.From(new[] { "Id" }));

                // Assert
                Assert.AreEqual(entity.IdMapped, mergeResult);

                // Act Query
                var data = connection.Query<MappedIdentityTable>(new { Id = id }).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.RowGuidMapped, data.RowGuidMapped);
                Assert.AreEqual(entity.ColumnBitMapped, data.ColumnBitMapped);
                Assert.AreEqual(entity.ColumnDateTime2Mapped, data.ColumnDateTime2Mapped);
                Assert.AreEqual(entity.ColumnDateTimeMapped, data.ColumnDateTimeMapped);
                Assert.AreEqual(entity.ColumnFloatMapped, data.ColumnFloatMapped);
                Assert.AreEqual(entity.ColumnIntMapped, data.ColumnIntMapped);
                Assert.AreEqual(entity.ColumnNVarCharMapped, data.ColumnNVarCharMapped);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionObjectMappingViaDataAnnotationsInsertAsyncAndMergeAndQueryAndQueryForIdentity()
        {
            // Setup
            var entity = GetMappedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = await connection.InsertAsync(entity);

                // Setup
                entity.ColumnNVarCharMapped = $"{entity.ColumnNVarCharMapped} (Merged)";

                // Act Update
                var mergeResult = await connection.MergeAsync<MappedIdentityTable>(entity,
                    qualifiers: Field.From(new[] { "Id" }));

                // Assert
                Assert.AreEqual(entity.IdMapped, mergeResult);

                // Act Query
                var data = (await connection.QueryAsync<MappedIdentityTable>(new { Id = id })).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.RowGuidMapped, data.RowGuidMapped);
                Assert.AreEqual(entity.ColumnBitMapped, data.ColumnBitMapped);
                Assert.AreEqual(entity.ColumnDateTime2Mapped, data.ColumnDateTime2Mapped);
                Assert.AreEqual(entity.ColumnDateTimeMapped, data.ColumnDateTimeMapped);
                Assert.AreEqual(entity.ColumnFloatMapped, data.ColumnFloatMapped);
                Assert.AreEqual(entity.ColumnIntMapped, data.ColumnIntMapped);
                Assert.AreEqual(entity.ColumnNVarCharMapped, data.ColumnNVarCharMapped);
            }
        }

        #endregion

        #region InsertAllAndQueryAndQueryAllForIdentity

        [TestMethod]
        public void TestSqlConnectionObjectMappingViaDataAnnotationsInsertAllAndQueryAllAndForIdentity()
        {
            // Setup
            var entities = GetMappedIdentityTables().AsList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act InsertAll
                var rowsInserted = connection.InsertAll(entities);

                // Act QueryAll
                var data = connection.QueryAll<MappedIdentityTable>();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(rowsInserted, data.Count());
                entities.ForEach(entity =>
                {
                    var mappedObject = data.FirstOrDefault(d => d.IdMapped == entity.IdMapped);
                    Assert.IsNotNull(mappedObject);
                    Assert.AreEqual(entity.RowGuidMapped, mappedObject.RowGuidMapped);
                    Assert.AreEqual(entity.ColumnBitMapped, mappedObject.ColumnBitMapped);
                    Assert.AreEqual(entity.ColumnDateTime2Mapped, mappedObject.ColumnDateTime2Mapped);
                    Assert.AreEqual(entity.ColumnDateTimeMapped, mappedObject.ColumnDateTimeMapped);
                    Assert.AreEqual(entity.ColumnFloatMapped, mappedObject.ColumnFloatMapped);
                    Assert.AreEqual(entity.ColumnIntMapped, mappedObject.ColumnIntMapped);
                    Assert.AreEqual(entity.ColumnNVarCharMapped, mappedObject.ColumnNVarCharMapped);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionObjectMappingViaDataAnnotationsInsertAllAsyncAndQueryAllAndForIdentity()
        {
            // Setup
            var entities = GetMappedIdentityTables().AsList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act InsertAll
                var rowsInserted = await connection.InsertAllAsync(entities);

                // Act QueryAll
                var data = await connection.QueryAllAsync<MappedIdentityTable>();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(rowsInserted, data.Count());
                entities.ForEach(entity =>
                {
                    var mappedObject = data.FirstOrDefault(d => d.IdMapped == entity.IdMapped);
                    Assert.IsNotNull(mappedObject);
                    Assert.AreEqual(entity.RowGuidMapped, mappedObject.RowGuidMapped);
                    Assert.AreEqual(entity.ColumnBitMapped, mappedObject.ColumnBitMapped);
                    Assert.AreEqual(entity.ColumnDateTime2Mapped, mappedObject.ColumnDateTime2Mapped);
                    Assert.AreEqual(entity.ColumnDateTimeMapped, mappedObject.ColumnDateTimeMapped);
                    Assert.AreEqual(entity.ColumnFloatMapped, mappedObject.ColumnFloatMapped);
                    Assert.AreEqual(entity.ColumnIntMapped, mappedObject.ColumnIntMapped);
                    Assert.AreEqual(entity.ColumnNVarCharMapped, mappedObject.ColumnNVarCharMapped);
                });
            }
        }

        #endregion

        #region InsertAllAndMergeAllAndQueryAllForIdentity

        [TestMethod]
        public void TestSqlConnectionObjectMappingViaDataAnnotationsInsertAllAndMergeAllAndQueryAllForIdentity()
        {
            // Setup
            var entities = GetMappedIdentityTables().AsList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act InsertAll
                var rowsInserted = connection.InsertAll(entities);

                // Setup
                entities.ForEach(entity =>
                {
                    entity.RowGuidMapped = Guid.NewGuid();
                    entity.ColumnBitMapped = !entity.ColumnBitMapped;
                    entity.ColumnDateTime2Mapped = entity.ColumnDateTime2Mapped.Value.AddMonths(1);
                    entity.ColumnDateTimeMapped = entity.ColumnDateTimeMapped.Value.AddMonths(1);
                    entity.ColumnFloatMapped = 500;
                    entity.ColumnIntMapped = 100;
                    entity.ColumnNVarCharMapped = $"Merged - {entity.ColumnNVarCharMapped}";
                });

                // Act MergeAll
                var rowsMerged = connection.MergeAll(entities);

                // Act QueryAll
                var data = connection.QueryAll<MappedIdentityTable>();

                // Assert333333333333
                Assert.IsNotNull(data);
                Assert.AreEqual(rowsMerged, data.Count());
                entities.ForEach(entity =>
                {
                    var mappedObject = data.FirstOrDefault(d => d.IdMapped == entity.IdMapped);
                    Assert.IsNotNull(mappedObject);
                    Assert.AreEqual(entity.RowGuidMapped, mappedObject.RowGuidMapped);
                    Assert.AreEqual(entity.ColumnBitMapped, mappedObject.ColumnBitMapped);
                    Assert.AreEqual(entity.ColumnDateTime2Mapped, mappedObject.ColumnDateTime2Mapped);
                    Assert.AreEqual(entity.ColumnDateTimeMapped, mappedObject.ColumnDateTimeMapped);
                    Assert.AreEqual(entity.ColumnFloatMapped, mappedObject.ColumnFloatMapped);
                    Assert.AreEqual(entity.ColumnIntMapped, mappedObject.ColumnIntMapped);
                    Assert.AreEqual(entity.ColumnNVarCharMapped, mappedObject.ColumnNVarCharMapped);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionObjectMappingViaDataAnnotationsInsertAllAsyncAndMergeAllAndQueryAllForIdentity()
        {
            // Setup
            var entities = GetMappedIdentityTables().AsList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act InsertAll
                var rowsInserted = await connection.InsertAllAsync(entities);

                // Setup
                entities.ForEach(entity =>
                {
                    entity.RowGuidMapped = Guid.NewGuid();
                    entity.ColumnBitMapped = !entity.ColumnBitMapped;
                    entity.ColumnDateTime2Mapped = entity.ColumnDateTime2Mapped.Value.AddMonths(1);
                    entity.ColumnDateTimeMapped = entity.ColumnDateTimeMapped.Value.AddMonths(1);
                    entity.ColumnFloatMapped = 500;
                    entity.ColumnIntMapped = 100;
                    entity.ColumnNVarCharMapped = $"Merged - {entity.ColumnNVarCharMapped}";
                });

                // Act MergeAll
                var rowsMerged = await connection.MergeAllAsync(entities);

                // Act QueryAll
                var data = await connection.QueryAllAsync<MappedIdentityTable>();

                // Assert333333333333
                Assert.IsNotNull(data);
                Assert.AreEqual(rowsMerged, data.Count());
                entities.ForEach(entity =>
                {
                    var mappedObject = data.FirstOrDefault(d => d.IdMapped == entity.IdMapped);
                    Assert.IsNotNull(mappedObject);
                    Assert.AreEqual(entity.RowGuidMapped, mappedObject.RowGuidMapped);
                    Assert.AreEqual(entity.ColumnBitMapped, mappedObject.ColumnBitMapped);
                    Assert.AreEqual(entity.ColumnDateTime2Mapped, mappedObject.ColumnDateTime2Mapped);
                    Assert.AreEqual(entity.ColumnDateTimeMapped, mappedObject.ColumnDateTimeMapped);
                    Assert.AreEqual(entity.ColumnFloatMapped, mappedObject.ColumnFloatMapped);
                    Assert.AreEqual(entity.ColumnIntMapped, mappedObject.ColumnIntMapped);
                    Assert.AreEqual(entity.ColumnNVarCharMapped, mappedObject.ColumnNVarCharMapped);
                });
            }
        }

        #endregion

        #endregion

        #region ForNonIdentity

        #region InsertAndQueryForNonIdentity

        [TestMethod]
        public void TestSqlConnectionObjectMappingViaDataAnnotationsInsertAndQueryForNonIdentityViaDynamic()
        {
            // Setup
            var entity = GetMappedCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<MappedCompleteTable>(new { SessionId = id }).FirstOrDefault();

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
        public async Task TestSqlConnectionObjectMappingViaDataAnnotationsInsertAsyncAndQueryForNonIdentityViaDynamic()
        {
            // Setup
            var entity = GetMappedCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = await connection.InsertAsync(entity);

                // Act Query
                var data = (await connection.QueryAsync<MappedCompleteTable>(new { SessionId = id })).FirstOrDefault();

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
        public void TestSqlConnectionObjectMappingViaDataAnnotationsInsertAndQueryForNonIdentityViaExpression()
        {
            // Setup
            var entity = GetMappedCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<MappedCompleteTable>(e => e.SessionIdMapped == (Guid)id).FirstOrDefault();

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
        public async Task TestSqlConnectionObjectMappingViaDataAnnotationsInsertAsyncAndQueryForNonIdentityViaExpression()
        {
            // Setup
            var entity = GetMappedCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = await connection.InsertAsync(entity);

                // Act Query
                var data = (await connection.QueryAsync<MappedCompleteTable>(e => e.SessionIdMapped == (Guid)id)).FirstOrDefault();

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
        public void TestSqlConnectionObjectMappingViaDataAnnotationsInsertAndQueryForNonIdentityViaQueryField()
        {
            // Setup
            var entity = GetMappedCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<MappedCompleteTable>(new QueryField("SessionId", id)).FirstOrDefault();

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
        public async Task TestSqlConnectionObjectMappingViaDataAnnotationsInsertAsyncAndQueryForNonIdentityViaQueryField()
        {
            // Setup
            var entity = GetMappedCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = await connection.InsertAsync(entity);

                // Act Query
                var data = (await connection.QueryAsync<MappedCompleteTable>(new QueryField("SessionId", id))).FirstOrDefault();

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
        public void TestSqlConnectionObjectMappingViaDataAnnotationsInsertAndQueryForNonIdentityViaQueryFields()
        {
            // Setup
            var entity = GetMappedCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<MappedCompleteTable>(new QueryField("SessionId", id).AsEnumerable()).FirstOrDefault();

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
        public async Task TestSqlConnectionObjectMappingViaDataAnnotationsInsertAsyncAndQueryForNonIdentityViaQueryFields()
        {
            // Setup
            var entity = GetMappedCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = await connection.InsertAsync(entity);

                // Act Query
                var data = (await connection.QueryAsync<MappedCompleteTable>(new QueryField("SessionId", id).AsEnumerable())).FirstOrDefault();

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
        public void TestSqlConnectionObjectMappingViaDataAnnotationsInsertAndQueryForNonIdentityViaQueryGroup()
        {
            // Setup
            var entity = GetMappedCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<MappedCompleteTable>(new QueryGroup(new QueryField("SessionId", id))).FirstOrDefault();

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
        public async Task TestSqlConnectionObjectMappingViaDataAnnotationsInsertAsyncAndQueryForNonIdentityViaQueryGroup()
        {
            // Setup
            var entity = GetMappedCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = await connection.InsertAsync(entity);

                // Act Query
                var data = (await connection.QueryAsync<MappedCompleteTable>(new QueryGroup(new QueryField("SessionId", id)))).FirstOrDefault();

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

        #region InsertAndUpdateAndQueryrNonIdentity

        [TestMethod]
        public void TestSqlConnectionObjectMappingViaDataAnnotationsInsertAndUpdateAndQueryForNonIdentityViaDynamic()
        {
            // Setup
            var entity = GetMappedCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Setup
                entity.ColumnNVarCharMapped = $"{entity.ColumnNVarCharMapped} (Updated)";

                // Act Update
                var affectedRows = connection.Update<MappedCompleteTable>(entity, new { SessionId = id });

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act Query
                var data = connection.Query<MappedCompleteTable>(new { SessionId = id }).FirstOrDefault();

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
        public async Task TestSqlConnectionObjectMappingViaDataAnnotationsInsertAsyncAndUpdateAndQueryForNonIdentityViaDynamic()
        {
            // Setup
            var entity = GetMappedCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = await connection.InsertAsync(entity);

                // Setup
                entity.ColumnNVarCharMapped = $"{entity.ColumnNVarCharMapped} (Updated)";

                // Act Update
                var affectedRows = await connection.UpdateAsync<MappedCompleteTable>(entity, new { SessionId = id });

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act Query
                var data = (await connection.QueryAsync<MappedCompleteTable>(new { SessionId = id })).FirstOrDefault();

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
        public void TestSqlConnectionObjectMappingViaDataAnnotationsInsertAndUpdateAndQueryForNonIdentityViaExpression()
        {
            // Setup
            var entity = GetMappedCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Setup
                entity.ColumnNVarCharMapped = $"{entity.ColumnNVarCharMapped} (Updated)";

                // Act Update
                var affectedRows = connection.Update<MappedCompleteTable>(entity, c => c.SessionIdMapped == (Guid)id);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act Query
                var data = connection.Query<MappedCompleteTable>(new { SessionId = id }).FirstOrDefault();

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
        public async Task TestSqlConnectionObjectMappingViaDataAnnotationsInsertAsyncAndUpdateAndQueryForNonIdentityViaExpression()
        {
            // Setup
            var entity = GetMappedCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = await connection.InsertAsync(entity);

                // Setup
                entity.ColumnNVarCharMapped = $"{entity.ColumnNVarCharMapped} (Updated)";

                // Act Update
                var affectedRows = await connection.UpdateAsync<MappedCompleteTable>(entity, c => c.SessionIdMapped == (Guid)id);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act Query
                var data = (await connection.QueryAsync<MappedCompleteTable>(new { SessionId = id })).FirstOrDefault();

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
        public void TestSqlConnectionObjectMappingViaDataAnnotationsInsertAndUpdateAndQueryForNonIdentityViaQueryField()
        {
            // Setup
            var entity = GetMappedCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Setup
                var field = new QueryField("SessionId", id);
                entity.ColumnNVarCharMapped = $"{entity.ColumnNVarCharMapped} (Updated)";

                // Act Update
                var affectedRows = connection.Update<MappedCompleteTable>(entity, field);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act Query
                field.Reset();
                var data = connection.Query<MappedCompleteTable>(field).FirstOrDefault();

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
        public async Task TestSqlConnectionObjectMappingViaDataAnnotationsInsertAsyncAndUpdateAndQueryForNonIdentityViaQueryField()
        {
            // Setup
            var entity = GetMappedCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = await connection.InsertAsync(entity);

                // Setup
                var field = new QueryField("SessionId", id);
                entity.ColumnNVarCharMapped = $"{entity.ColumnNVarCharMapped} (Updated)";

                // Act Update
                var affectedRows = await connection.UpdateAsync<MappedCompleteTable>(entity, field);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act Query
                field.Reset();
                var data = (await connection.QueryAsync<MappedCompleteTable>(field)).FirstOrDefault();

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
        public void TestSqlConnectionObjectMappingViaDataAnnotationsInsertAndUpdateAndQueryForNonIdentityViaQueryFields()
        {
            // Setup
            var entity = GetMappedCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Setup
                var fields = new QueryField("SessionId", id).AsEnumerable();
                entity.ColumnNVarCharMapped = $"{entity.ColumnNVarCharMapped} (Updated)";

                // Act Update
                var affectedRows = connection.Update<MappedCompleteTable>(entity, fields);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act Query
                fields.ResetAll();
                var data = connection.Query<MappedCompleteTable>(fields).FirstOrDefault();

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
        public async Task TestSqlConnectionObjectMappingViaDataAnnotationsInsertAsyncAndUpdateAndQueryForNonIdentityViaQueryFields()
        {
            // Setup
            var entity = GetMappedCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = await connection.InsertAsync(entity);

                // Setup
                var fields = new QueryField("SessionId", id).AsEnumerable();
                entity.ColumnNVarCharMapped = $"{entity.ColumnNVarCharMapped} (Updated)";

                // Act Update
                var affectedRows = await connection.UpdateAsync<MappedCompleteTable>(entity, fields);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act Query
                fields.ResetAll();
                var data = (await connection.QueryAsync<MappedCompleteTable>(fields)).FirstOrDefault();

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
        public void TestSqlConnectionObjectMappingViaDataAnnotationsInsertAndUpdateAndQueryForNonIdentityViaQueryGroup()
        {
            // Setup
            var entity = GetMappedCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Setup
                var queryGroup = new QueryGroup(new QueryField("SessionId", id));
                entity.ColumnNVarCharMapped = $"{entity.ColumnNVarCharMapped} (Updated)";

                // Act Update
                var affectedRows = connection.Update<MappedCompleteTable>(entity, queryGroup);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act Query
                queryGroup.Reset();
                var data = connection.Query<MappedCompleteTable>(queryGroup).FirstOrDefault();

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
        public async Task TestSqlConnectionObjectMappingViaDataAnnotationsInsertAsyncAndUpdateAndQueryForNonIdentityViaQueryGroup()
        {
            // Setup
            var entity = GetMappedCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = await connection.InsertAsync(entity);

                // Setup
                var queryGroup = new QueryGroup(new QueryField("SessionId", id));
                entity.ColumnNVarCharMapped = $"{entity.ColumnNVarCharMapped} (Updated)";

                // Act Update
                var affectedRows = await connection.UpdateAsync<MappedCompleteTable>(entity, queryGroup);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act Query
                queryGroup.Reset();
                var data = (await connection.QueryAsync<MappedCompleteTable>(queryGroup)).FirstOrDefault();

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

        #region InsertAndMergeAndQueryForNonIdentity

        [TestMethod]
        public void TestSqlConnectionObjectMappingViaDataAnnotationsInsertAndMergeAndQueryAndQueryForNonIdentity()
        {
            // Setup
            var entity = GetMappedCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Setup
                entity.ColumnNVarCharMapped = $"{entity.ColumnNVarCharMapped} (Merged)";

                // Act Update
                var mergeResult = connection.Merge<MappedCompleteTable>(entity,
                    qualifiers: Field.From(new[] { "SessionId" }));

                // Assert
                Assert.AreEqual(entity.SessionIdMapped, mergeResult);

                // Act Query
                var data = connection.Query<MappedCompleteTable>(new { SessionId = id }).FirstOrDefault();

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
        public async Task TestSqlConnectionObjectMappingViaDataAnnotationsInsertAsyncAndMergeAndQueryAndQueryForNonIdentity()
        {
            // Setup
            var entity = GetMappedCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act Insert
                var id = await connection.InsertAsync(entity);

                // Setup
                entity.ColumnNVarCharMapped = $"{entity.ColumnNVarCharMapped} (Merged)";

                // Act Update
                var mergeResult = await connection.MergeAsync<MappedCompleteTable>(entity,
                    qualifiers: Field.From(new[] { "SessionId" }));

                // Assert
                Assert.AreEqual(entity.SessionIdMapped, mergeResult);

                // Act Query
                var data = (await connection.QueryAsync<MappedCompleteTable>(new { SessionId = id })).FirstOrDefault();

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

        #region InsertAllAndQueryAndQueryAllForNonIdentity

        [TestMethod]
        public void TestSqlConnectionObjectMappingViaDataAnnotationsInsertAllAndQueryAllAndForNonIdentity()
        {
            // Setup
            var entities = GetMappedCompleteTables().AsList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act InsertAll
                var rowsInserted = connection.InsertAll(entities);

                // Act QueryAll
                var data = connection.QueryAll<MappedCompleteTable>();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(rowsInserted, data.Count());
                entities.ForEach(entity =>
                {
                    var mappedObject = data.FirstOrDefault(d => d.SessionIdMapped == entity.SessionIdMapped);
                    Assert.IsNotNull(mappedObject);
                    Assert.AreEqual(entity.ColumnBigIntMapped, mappedObject.ColumnBigIntMapped);
                    Assert.AreEqual(entity.ColumnBitMapped, mappedObject.ColumnBitMapped);
                    Assert.AreEqual(entity.ColumnDateTime2Mapped, mappedObject.ColumnDateTime2Mapped);
                    Assert.AreEqual(entity.ColumnDateTimeMapped, mappedObject.ColumnDateTimeMapped);
                    Assert.AreEqual(entity.ColumnIntMapped, mappedObject.ColumnIntMapped);
                    Assert.AreEqual(entity.ColumnNVarCharMapped, mappedObject.ColumnNVarCharMapped);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionObjectMappingViaDataAnnotationsInsertAllAsyncAndQueryAllAndForNonIdentity()
        {
            // Setup
            var entities = GetMappedCompleteTables().AsList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act InsertAll
                var rowsInserted = await connection.InsertAllAsync(entities);

                // Act QueryAll
                var data = await connection.QueryAllAsync<MappedCompleteTable>();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(rowsInserted, data.Count());
                entities.ForEach(entity =>
                {
                    var mappedObject = data.FirstOrDefault(d => d.SessionIdMapped == entity.SessionIdMapped);
                    Assert.IsNotNull(mappedObject);
                    Assert.AreEqual(entity.ColumnBigIntMapped, mappedObject.ColumnBigIntMapped);
                    Assert.AreEqual(entity.ColumnBitMapped, mappedObject.ColumnBitMapped);
                    Assert.AreEqual(entity.ColumnDateTime2Mapped, mappedObject.ColumnDateTime2Mapped);
                    Assert.AreEqual(entity.ColumnDateTimeMapped, mappedObject.ColumnDateTimeMapped);
                    Assert.AreEqual(entity.ColumnIntMapped, mappedObject.ColumnIntMapped);
                    Assert.AreEqual(entity.ColumnNVarCharMapped, mappedObject.ColumnNVarCharMapped);
                });
            }
        }

        #endregion

        #region InsertAllAndMergeAllAndQueryAllForNonIdentity

        [TestMethod]
        public void TestSqlConnectionObjectMappingViaDataAnnotationsInsertAllAndMergeAllAndQueryAllForNonIdentity()
        {
            // Setup
            var entities = GetMappedCompleteTables().AsList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act InsertAll
                var rowsInserted = connection.InsertAll(entities);

                // Setup
                entities.ForEach(entity =>
                {
                    entity.ColumnBigIntMapped = 1000;
                    entity.ColumnBitMapped = !entity.ColumnBitMapped;
                    entity.ColumnDateTime2Mapped = entity.ColumnDateTime2Mapped.Value.AddMonths(1);
                    entity.ColumnDateTimeMapped = entity.ColumnDateTimeMapped.Value.AddMonths(1);
                    entity.ColumnIntMapped = 100;
                    entity.ColumnNVarCharMapped = $"Merged - {entity.ColumnNVarCharMapped}";
                });

                // Act MergeAll
                var rowsMerged = connection.MergeAll(entities);

                // Act QueryAll
                var data = connection.QueryAll<MappedCompleteTable>();

                // Assert333333333333
                Assert.IsNotNull(data);
                Assert.AreEqual(rowsMerged, data.Count());
                entities.ForEach(entity =>
                {
                    var mappedObject = data.FirstOrDefault(d => d.SessionIdMapped == entity.SessionIdMapped);
                    Assert.IsNotNull(mappedObject);
                    Assert.AreEqual(entity.ColumnBigIntMapped, mappedObject.ColumnBigIntMapped);
                    Assert.AreEqual(entity.ColumnBitMapped, mappedObject.ColumnBitMapped);
                    Assert.AreEqual(entity.ColumnDateTime2Mapped, mappedObject.ColumnDateTime2Mapped);
                    Assert.AreEqual(entity.ColumnDateTimeMapped, mappedObject.ColumnDateTimeMapped);
                    Assert.AreEqual(entity.ColumnIntMapped, mappedObject.ColumnIntMapped);
                    Assert.AreEqual(entity.ColumnNVarCharMapped, mappedObject.ColumnNVarCharMapped);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionObjectMappingViaDataAnnotationsInsertAllAsyncAndMergeAllAndQueryAllForNonIdentity()
        {
            // Setup
            var entities = GetMappedCompleteTables().AsList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act InsertAll
                var rowsInserted = await connection.InsertAllAsync(entities);

                // Setup
                entities.ForEach(entity =>
                {
                    entity.ColumnBigIntMapped = 1000;
                    entity.ColumnBitMapped = !entity.ColumnBitMapped;
                    entity.ColumnDateTime2Mapped = entity.ColumnDateTime2Mapped.Value.AddMonths(1);
                    entity.ColumnDateTimeMapped = entity.ColumnDateTimeMapped.Value.AddMonths(1);
                    entity.ColumnIntMapped = 100;
                    entity.ColumnNVarCharMapped = $"Merged - {entity.ColumnNVarCharMapped}";
                });

                // Act MergeAll
                var rowsMerged = await connection.MergeAllAsync(entities);

                // Act QueryAll
                var data = await connection.QueryAllAsync<MappedCompleteTable>();

                // Assert333333333333
                Assert.IsNotNull(data);
                Assert.AreEqual(rowsMerged, data.Count());
                entities.ForEach(entity =>
                {
                    var mappedObject = data.FirstOrDefault(d => d.SessionIdMapped == entity.SessionIdMapped);
                    Assert.IsNotNull(mappedObject);
                    Assert.AreEqual(entity.ColumnBigIntMapped, mappedObject.ColumnBigIntMapped);
                    Assert.AreEqual(entity.ColumnBitMapped, mappedObject.ColumnBitMapped);
                    Assert.AreEqual(entity.ColumnDateTime2Mapped, mappedObject.ColumnDateTime2Mapped);
                    Assert.AreEqual(entity.ColumnDateTimeMapped, mappedObject.ColumnDateTimeMapped);
                    Assert.AreEqual(entity.ColumnIntMapped, mappedObject.ColumnIntMapped);
                    Assert.AreEqual(entity.ColumnNVarCharMapped, mappedObject.ColumnNVarCharMapped);
                });
            }
        }

        #endregion

        #endregion
    }
}
