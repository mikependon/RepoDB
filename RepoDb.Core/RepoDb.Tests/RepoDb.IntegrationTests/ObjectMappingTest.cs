using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Setup;

namespace RepoDb.IntegrationTests
{
    [TestClass]
    public class ObjectMappingTest
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

        [Map("[dbo].[COMPLETETABLE]")]
        private class MappedCompleteTable
        {
            [Map("SESSIONID"), Primary]
            public Guid SessionIdMapped { get; set; }
            [Map("COLUMNBIGINT")]
            public long? ColumnBigIntMapped { get; set; }
            [Map("COLUMNBIT")]
            public bool? ColumnBitMapped { get; set; }
            [Map("COLUMNINT")]
            public int? ColumnIntMapped { get; set; }
            [Map("COLUMNDATETIME")]
            public DateTime? ColumnDateTimeMapped { get; set; }
            [Map("COLUMNDATETIME2")]
            public DateTime? ColumnDateTime2Mapped { get; set; }
            [Map("[COLUMNNVARCHAR]")]
            public string ColumnNVarCharMapped { get; set; }
        }

        [Map("[sc].[IDENTITYTABLE]")]
        private class MappedIdentityTable
        {
            [Map("ID"), Primary]
            public long IdMapped { get; set; }
            [Map("ROWGUID")]
            public Guid RowGuidMapped { get; set; }
            [Map("COLUMNBIT")]
            public bool? ColumnBitMapped { get; set; }
            [Map("COLUMNDATETIME")]
            public DateTime? ColumnDateTimeMapped { get; set; }
            [Map("COLUMNDATETIME2")]
            public DateTime? ColumnDateTime2Mapped { get; set; }
            [Map("COLUMNFLOAT")]
            public double? ColumnFloatMapped { get; set; }
            [Map("COLUMNINT")]
            public int? ColumnIntMapped { get; set; }
            [Map("[COLUMNNVARCHAR]")]
            public string ColumnNVarCharMapped { get; set; }
        }

        [Map("[dbo].[COMPLETETABLE]")]
        private class MappedCompleteTableForKey
        {
            [Map("SESSIONID")]
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
        public void TestSqlConnectionObjectMappingInsertAndDeleteByKey()
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
        public void TestSqlConnectionObjectMappingInsertAndExistsByKey()
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
        public void TestSqlConnectionObjectMappingInsertAndQueryByKey()
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
        public void TestSqlConnectionObjectMappingInsertAndQueryForIdentityViaDynamic()
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
        public void TestSqlConnectionObjectMappingInsertAndQueryForIdentityViaExpression()
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
        public void TestSqlConnectionObjectMappingInsertAndQueryForIdentityViaQueryField()
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
        public void TestSqlConnectionObjectMappingInsertAndQueryForIdentityViaQueryFields()
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
        public void TestSqlConnectionObjectMappingInsertAndQueryForIdentityViaQueryGroup()
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
        public void TestSqlConnectionObjectMappingInsertAndUpdateAndQueryForIdentityViaDynamic()
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
        public void TestSqlConnectionObjectMappingInsertAndUpdateAndQueryForIdentityViaExpression()
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
        public void TestSqlConnectionObjectMappingInsertAndUpdateAndQueryForIdentityViaQueryField()
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
        public void TestSqlConnectionObjectMappingInsertAndUpdateAndQueryForIdentityViaQueryFields()
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
        public void TestSqlConnectionObjectMappingInsertAndUpdateAndQueryForIdentityViaQueryGroup()
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
        public void TestSqlConnectionObjectMappingInsertAndMergeAndQueryAndQueryForIdentity()
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
        public void TestSqlConnectionObjectMappingInsertAllAndQueryAllAndForIdentity()
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
        public void TestSqlConnectionObjectMappingInsertAllAndMergeAllAndQueryAllForIdentity()
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
        public void TestSqlConnectionObjectMappingInsertAndQueryForNonIdentityViaDynamic()
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
        public void TestSqlConnectionObjectMappingInsertAndQueryForNonIdentityViaExpression()
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
        public void TestSqlConnectionObjectMappingInsertAndQueryForNonIdentityViaQueryField()
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
        public void TestSqlConnectionObjectMappingInsertAndQueryForNonIdentityViaQueryFields()
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
        public void TestSqlConnectionObjectMappingInsertAndQueryForNonIdentityViaQueryGroup()
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
        public void TestSqlConnectionObjectMappingInsertAndUpdateAndQueryForNonIdentityViaDynamic()
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
        public void TestSqlConnectionObjectMappingInsertAndUpdateAndQueryForNonIdentityViaExpression()
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
        public void TestSqlConnectionObjectMappingInsertAndUpdateAndQueryForNonIdentityViaQueryField()
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
        public void TestSqlConnectionObjectMappingInsertAndUpdateAndQueryForNonIdentityViaQueryFields()
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
        public void TestSqlConnectionObjectMappingInsertAndUpdateAndQueryForNonIdentityViaQueryGroup()
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
        public void TestSqlConnectionObjectMappingInsertAndMergeAndQueryAndQueryForNonIdentity()
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
        public void TestSqlConnectionObjectMappingInsertAllAndQueryAllAndForNonIdentity()
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
        public void TestSqlConnectionObjectMappingInsertAllAndMergeAllAndQueryAllForNonIdentity()
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
