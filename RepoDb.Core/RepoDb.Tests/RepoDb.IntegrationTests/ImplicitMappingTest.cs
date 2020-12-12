using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Setup;

namespace RepoDb.IntegrationTests
{
    [TestClass]
    public class ImplicitMappingTest
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Setup();
        }

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

        private static void Setup()
        {
            // MappedCompleteTable
            ClassMapper.Add<MappedCompleteTable>("[dbo].[COMPLETETABLE]", true);
            PrimaryMapper.Add<MappedCompleteTable>(e => e.SessionIdMapped, true);
            PropertyMapper.Add<MappedCompleteTable>(e => e.SessionIdMapped, "SESSIONID", true);
            PropertyMapper.Add<MappedCompleteTable>(e => e.ColumnBigIntMapped, "COLUMNBIGINT", true);
            PropertyMapper.Add<MappedCompleteTable>(e => e.ColumnBitMapped, "COLUMNBIT", true);
            PropertyMapper.Add<MappedCompleteTable>(e => e.ColumnIntMapped, "COLUMNINT", true);
            PropertyMapper.Add<MappedCompleteTable>(e => e.ColumnDateTimeMapped, "COLUMNDATETIME", true);
            PropertyMapper.Add<MappedCompleteTable>(e => e.ColumnDateTime2Mapped, "COLUMNDATETIME2", true);
            PropertyMapper.Add<MappedCompleteTable>(e => e.ColumnNVarCharMapped, "COLUMNNVARCHAR", true);

            // MappedCompleteTable
            ClassMapper.Add<MappedIdentityTable>("[sc].[IDENTITYTABLE]", true);
            PrimaryMapper.Add<MappedIdentityTable>(e => e.IdMapped, true);
            PropertyMapper.Add<MappedIdentityTable>(e => e.IdMapped, "ID", true);
            PropertyMapper.Add<MappedIdentityTable>(e => e.RowGuidMapped, "ROWGUID", true);
            PropertyMapper.Add<MappedIdentityTable>(e => e.ColumnBitMapped, "COLUMNBIT", true);
            PropertyMapper.Add<MappedIdentityTable>(e => e.ColumnIntMapped, "COLUMNINT", true);
            PropertyMapper.Add<MappedIdentityTable>(e => e.ColumnDateTimeMapped, "COLUMNDATETIME", true);
            PropertyMapper.Add<MappedIdentityTable>(e => e.ColumnDateTime2Mapped, "COLUMNDATETIME2", true);
            PropertyMapper.Add<MappedIdentityTable>(e => e.ColumnFloatMapped, "COLUMNFLOAT", true);
            PropertyMapper.Add<MappedIdentityTable>(e => e.ColumnNVarCharMapped, "COLUMNNVARCHAR", true);

            // MappedCompleteTableForKey
            ClassMapper.Add<MappedCompleteTableForKey>("[dbo].[COMPLETETABLE]", true);
            PropertyMapper.Add<MappedCompleteTableForKey>(e => e.IdMapped, "SESSIONID", true);
        }

        #region SubClasses

        private class MappedCompleteTable
        {
            public Guid SessionIdMapped { get; set; }
            public long? ColumnBigIntMapped { get; set; }
            public bool? ColumnBitMapped { get; set; }
            public int? ColumnIntMapped { get; set; }
            public DateTime? ColumnDateTimeMapped { get; set; }
            public DateTime? ColumnDateTime2Mapped { get; set; }
            public string ColumnNVarCharMapped { get; set; }
        }

        private class MappedIdentityTable
        {
            public long IdMapped { get; set; }
            public Guid RowGuidMapped { get; set; }
            public bool? ColumnBitMapped { get; set; }
            public DateTime? ColumnDateTimeMapped { get; set; }
            public DateTime? ColumnDateTime2Mapped { get; set; }
            public double? ColumnFloatMapped { get; set; }
            public int? ColumnIntMapped { get; set; }
            public string ColumnNVarCharMapped { get; set; }
        }

        private class MappedCompleteTableForKey
        {
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
        public void TestSqlConnectionImplicitMappingInsertAndDeleteByKey()
        {
            // Setup
            var entity = GetMappedCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var result = connection.Delete<MappedCompleteTableForKey>(id);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        #endregion

        #region InsertAndExistsByKey

        [TestMethod]
        public void TestSqlConnectionImplicitMappingInsertAndExistsByKey()
        {
            // Setup
            var entity = GetMappedCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var result = connection.Exists<MappedCompleteTableForKey>(id);

                // Assert
                Assert.IsTrue(result);
            }
        }

        #endregion

        #region InsertAndQueryByKey

        [TestMethod]
        public void TestSqlConnectionImplicitMappingInsertAndQueryByKey()
        {
            // Setup
            var entity = GetMappedCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var result = connection.Query<MappedCompleteTable>(id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(result);
            }
        }

        #endregion

        #endregion

        #region ForIdentity

        #region InsertAndQueryForIdentity

        [TestMethod]
        public void TestSqlConnectionImplicitMappingInsertAndQueryForIdentityViaDynamic()
        {
            // Setup
            var entity = GetMappedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
        public void TestSqlConnectionImplicitMappingInsertAndQueryForIdentityViaExpression()
        {
            // Setup
            var entity = GetMappedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
        public void TestSqlConnectionImplicitMappingInsertAndQueryForIdentityViaQueryField()
        {
            // Setup
            var entity = GetMappedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
        public void TestSqlConnectionImplicitMappingInsertAndQueryForIdentityViaQueryFields()
        {
            // Setup
            var entity = GetMappedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
        public void TestSqlConnectionImplicitMappingInsertAndQueryForIdentityViaQueryGroup()
        {
            // Setup
            var entity = GetMappedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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

        #endregion

        #region InsertAndUpdateAndQueryrNonIdentity

        [TestMethod]
        public void TestSqlConnectionImplicitMappingInsertAndUpdateAndQueryForIdentityViaDynamic()
        {
            // Setup
            var entity = GetMappedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
        public void TestSqlConnectionImplicitMappingInsertAndUpdateAndQueryForIdentityViaExpression()
        {
            // Setup
            var entity = GetMappedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
        public void TestSqlConnectionImplicitMappingInsertAndUpdateAndQueryForIdentityViaQueryField()
        {
            // Setup
            var entity = GetMappedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
        public void TestSqlConnectionImplicitMappingInsertAndUpdateAndQueryForIdentityViaQueryFields()
        {
            // Setup
            var entity = GetMappedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
        public void TestSqlConnectionImplicitMappingInsertAndUpdateAndQueryForIdentityViaQueryGroup()
        {
            // Setup
            var entity = GetMappedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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

        #endregion

        #region InsertAndMergeAndQueryForIdentity

        [TestMethod]
        public void TestSqlConnectionImplicitMappingInsertAndMergeAndQueryAndQueryForIdentity()
        {
            // Setup
            var entity = GetMappedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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

        #endregion

        #region InsertAllAndQueryAndQueryAllForIdentity

        [TestMethod]
        public void TestSqlConnectionImplicitMappingInsertAllAndQueryAllAndForIdentity()
        {
            // Setup
            var entities = GetMappedIdentityTables().AsList();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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

        #endregion

        #region InsertAllAndMergeAllAndQueryAllForIdentity

        [TestMethod]
        public void TestSqlConnectionImplicitMappingInsertAllAndMergeAllAndQueryAllForIdentity()
        {
            // Setup
            var entities = GetMappedIdentityTables().AsList();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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

        #endregion

        #endregion

        #region ForNonIdentity

        #region InsertAndQueryForNonIdentity

        [TestMethod]
        public void TestSqlConnectionImplicitMappingInsertAndQueryForNonIdentityViaDynamic()
        {
            // Setup
            var entity = GetMappedCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
        public void TestSqlConnectionImplicitMappingInsertAndQueryForNonIdentityViaExpression()
        {
            // Setup
            var entity = GetMappedCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
        public void TestSqlConnectionImplicitMappingInsertAndQueryForNonIdentityViaQueryField()
        {
            // Setup
            var entity = GetMappedCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
        public void TestSqlConnectionImplicitMappingInsertAndQueryForNonIdentityViaQueryFields()
        {
            // Setup
            var entity = GetMappedCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
        public void TestSqlConnectionImplicitMappingInsertAndQueryForNonIdentityViaQueryGroup()
        {
            // Setup
            var entity = GetMappedCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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

        #endregion

        #region InsertAndUpdateAndQueryrNonIdentity

        [TestMethod]
        public void TestSqlConnectionImplicitMappingInsertAndUpdateAndQueryForNonIdentityViaDynamic()
        {
            // Setup
            var entity = GetMappedCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
        public void TestSqlConnectionImplicitMappingInsertAndUpdateAndQueryForNonIdentityViaExpression()
        {
            // Setup
            var entity = GetMappedCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
        public void TestSqlConnectionImplicitMappingInsertAndUpdateAndQueryForNonIdentityViaQueryField()
        {
            // Setup
            var entity = GetMappedCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
        public void TestSqlConnectionImplicitMappingInsertAndUpdateAndQueryForNonIdentityViaQueryFields()
        {
            // Setup
            var entity = GetMappedCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
        public void TestSqlConnectionImplicitMappingInsertAndUpdateAndQueryForNonIdentityViaQueryGroup()
        {
            // Setup
            var entity = GetMappedCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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

        #endregion

        #region InsertAndMergeAndQueryForNonIdentity

        [TestMethod]
        public void TestSqlConnectionImplicitMappingInsertAndMergeAndQueryAndQueryForNonIdentity()
        {
            // Setup
            var entity = GetMappedCompleteTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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

        #endregion

        #region InsertAllAndQueryAndQueryAllForNonIdentity

        [TestMethod]
        public void TestSqlConnectionImplicitMappingInsertAllAndQueryAllAndForNonIdentity()
        {
            // Setup
            var entities = GetMappedCompleteTables().AsList();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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

        #endregion

        #region InsertAllAndMergeAllAndQueryAllForNonIdentity

        [TestMethod]
        public void TestSqlConnectionImplicitMappingInsertAllAndMergeAllAndQueryAllForNonIdentity()
        {
            // Setup
            var entities = GetMappedCompleteTables().AsList();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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

        #endregion

        #endregion
    }
}
