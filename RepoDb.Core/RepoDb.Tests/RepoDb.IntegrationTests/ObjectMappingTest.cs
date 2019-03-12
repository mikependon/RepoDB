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

        [Map("COMPLETETABLE")]
        private class MappedObject
        {
            [Map("SESSIONID"), Primary]
            public Guid SessionIdMapped { get; set; }
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

        #region Query

        [TestMethod]
        public void TestSqlConnectionObjectMappingInsertAndQueryViaDynamic()
        {
            // Setup
            var entity = new MappedObject
            {
                SessionIdMapped = Guid.NewGuid(),
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
                var data = connection.Query<MappedObject>(new { SessionId = id }).FirstOrDefault();

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
        public void TestSqlConnectionObjectMappingInsertAndQueryViaExpression()
        {
            // Setup
            var entity = new MappedObject
            {
                SessionIdMapped = Guid.NewGuid(),
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
                var data = connection.Query<MappedObject>(e => e.SessionIdMapped == (Guid)id).FirstOrDefault();

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
        public void TestSqlConnectionObjectMappingInsertAndQueryViaQueryField()
        {
            // Setup
            var entity = new MappedObject
            {
                SessionIdMapped = Guid.NewGuid(),
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
                var data = connection.Query<MappedObject>(new QueryField("SessionId", id)).FirstOrDefault();

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
        public void TestSqlConnectionObjectMappingInsertAndQueryViaQueryFields()
        {
            // Setup
            var entity = new MappedObject
            {
                SessionIdMapped = Guid.NewGuid(),
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
                var data = connection.Query<MappedObject>(new QueryField("SessionId", id).AsEnumerable()).FirstOrDefault();

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
        public void TestSqlConnectionObjectMappingInsertAndQueryViaQueryGroup()
        {
            // Setup
            var entity = new MappedObject
            {
                SessionIdMapped = Guid.NewGuid(),
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
                var data = connection.Query<MappedObject>(new QueryGroup(new QueryField("SessionId", id).AsEnumerable())).FirstOrDefault();

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

        #region Update

        [TestMethod]
        public void TestSqlConnectionObjectMappingInsertAndUpdateViaDynamic()
        {
            // Setup
            var entity = new MappedObject
            {
                SessionIdMapped = Guid.NewGuid(),
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

                // Setup
                entity.ColumnNVarCharMapped = $"{entity.ColumnNVarCharMapped} (Updated)";

                // Act Update
                var affectedRows = connection.Update<MappedObject>(entity, new { SessionId = id });

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act Query
                var data = connection.Query<MappedObject>(new { SessionId = id }).FirstOrDefault();

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
        public void TestSqlConnectionObjectMappingInsertAndUpdateViaExpression()
        {
            // Setup
            var entity = new MappedObject
            {
                SessionIdMapped = Guid.NewGuid(),
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

                // Setup
                entity.ColumnNVarCharMapped = $"{entity.ColumnNVarCharMapped} (Updated)";

                // Act Update
                var affectedRows = connection.Update<MappedObject>(entity, c => c.SessionIdMapped == (Guid)id);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act Query
                var data = connection.Query<MappedObject>(new { SessionId = id }).FirstOrDefault();

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
        public void TestSqlConnectionObjectMappingInsertAndUpdateViaQueryField()
        {
            // Setup
            var entity = new MappedObject
            {
                SessionIdMapped = Guid.NewGuid(),
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

                // Setup
                var field = new QueryField("SessionId", id);
                entity.ColumnNVarCharMapped = $"{entity.ColumnNVarCharMapped} (Updated)";

                // Act Update
                var affectedRows = connection.Update<MappedObject>(entity, field);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act Query
                field.Reset();
                var data = connection.Query<MappedObject>(field).FirstOrDefault();

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
        public void TestSqlConnectionObjectMappingInsertAndUpdateViaQueryFields()
        {
            // Setup
            var entity = new MappedObject
            {
                SessionIdMapped = Guid.NewGuid(),
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

                // Setup
                var fields = new QueryField("SessionId", id).AsEnumerable();
                entity.ColumnNVarCharMapped = $"{entity.ColumnNVarCharMapped} (Updated)";

                // Act Update
                var affectedRows = connection.Update<MappedObject>(entity, fields);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act Query
                fields.ResetAll();
                var data = connection.Query<MappedObject>(fields).FirstOrDefault();

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
        public void TestSqlConnectionObjectMappingInsertAndUpdateViaQueryGroup()
        {
            // Setup
            var entity = new MappedObject
            {
                SessionIdMapped = Guid.NewGuid(),
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

                // Setup
                var queryGroup = new QueryGroup(new QueryField("SessionId", id).AsEnumerable());
                entity.ColumnNVarCharMapped = $"{entity.ColumnNVarCharMapped} (Updated)";

                // Act Update
                var affectedRows = connection.Update<MappedObject>(entity, queryGroup);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act Query
                queryGroup.Reset();
                var data = connection.Query<MappedObject>(queryGroup).FirstOrDefault();

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

        #region InlineUpdate

        [TestMethod]
        public void TestSqlConnectionObjectMappingInsertAndInlineUpdateViaDynamic()
        {
            // Setup
            var entity = new MappedObject
            {
                SessionIdMapped = Guid.NewGuid(),
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

                // Act Update
                var affectedRows = connection.InlineUpdate<MappedObject>(new
                {
                    ColumnNVarChar = $"{entity.ColumnNVarCharMapped} (Updated)"
                }, new { SessionId = id });

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act Query
                var data = connection.Query<MappedObject>(new { SessionId = id }).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.ColumnBigIntMapped, data.ColumnBigIntMapped);
                Assert.AreEqual(entity.ColumnBitMapped, data.ColumnBitMapped);
                Assert.AreEqual(entity.ColumnDateTime2Mapped, data.ColumnDateTime2Mapped);
                Assert.AreEqual(entity.ColumnDateTimeMapped, data.ColumnDateTimeMapped);
                Assert.AreEqual(entity.ColumnIntMapped, data.ColumnIntMapped);
                Assert.AreEqual($"{entity.ColumnNVarCharMapped} (Updated)", data.ColumnNVarCharMapped);
            }
        }

        [TestMethod]
        public void TestSqlConnectionObjectMappingInsertAndInlineUpdateViaExpression()
        {
            // Setup
            var entity = new MappedObject
            {
                SessionIdMapped = Guid.NewGuid(),
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

                // Act Update
                var affectedRows = connection.InlineUpdate<MappedObject>(new
                {
                    ColumnNVarChar = $"{entity.ColumnNVarCharMapped} (Updated)"
                }, c => c.SessionIdMapped == (Guid)id);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act Query
                var data = connection.Query<MappedObject>(new { SessionId = id }).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.ColumnBigIntMapped, data.ColumnBigIntMapped);
                Assert.AreEqual(entity.ColumnBitMapped, data.ColumnBitMapped);
                Assert.AreEqual(entity.ColumnDateTime2Mapped, data.ColumnDateTime2Mapped);
                Assert.AreEqual(entity.ColumnDateTimeMapped, data.ColumnDateTimeMapped);
                Assert.AreEqual(entity.ColumnIntMapped, data.ColumnIntMapped);
                Assert.AreEqual($"{entity.ColumnNVarCharMapped} (Updated)", data.ColumnNVarCharMapped);
            }
        }

        [TestMethod]
        public void TestSqlConnectionObjectMappingInsertAndInlineUpdateViaQueryField()
        {
            // Setup
            var entity = new MappedObject
            {
                SessionIdMapped = Guid.NewGuid(),
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

                // Setup
                var field = new QueryField("SessionId", id);

                // Act Update
                var affectedRows = connection.InlineUpdate<MappedObject>(new
                {
                    ColumnNVarChar = $"{entity.ColumnNVarCharMapped} (Updated)"
                }, field);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act Query
                field.Reset();
                var data = connection.Query<MappedObject>(field).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.ColumnBigIntMapped, data.ColumnBigIntMapped);
                Assert.AreEqual(entity.ColumnBitMapped, data.ColumnBitMapped);
                Assert.AreEqual(entity.ColumnDateTime2Mapped, data.ColumnDateTime2Mapped);
                Assert.AreEqual(entity.ColumnDateTimeMapped, data.ColumnDateTimeMapped);
                Assert.AreEqual(entity.ColumnIntMapped, data.ColumnIntMapped);
                Assert.AreEqual($"{entity.ColumnNVarCharMapped} (Updated)", data.ColumnNVarCharMapped);
            }
        }

        [TestMethod]
        public void TestSqlConnectionObjectMappingInsertAndInlineUpdateViaQueryFields()
        {
            // Setup
            var entity = new MappedObject
            {
                SessionIdMapped = Guid.NewGuid(),
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

                // Setup
                var fields = new QueryField("SessionId", id).AsEnumerable();

                // Act Update
                var affectedRows = connection.InlineUpdate<MappedObject>(new
                {
                    ColumnNVarChar = $"{entity.ColumnNVarCharMapped} (Updated)"
                }, fields);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act Query
                fields.ResetAll();
                var data = connection.Query<MappedObject>(fields).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.ColumnBigIntMapped, data.ColumnBigIntMapped);
                Assert.AreEqual(entity.ColumnBitMapped, data.ColumnBitMapped);
                Assert.AreEqual(entity.ColumnDateTime2Mapped, data.ColumnDateTime2Mapped);
                Assert.AreEqual(entity.ColumnDateTimeMapped, data.ColumnDateTimeMapped);
                Assert.AreEqual(entity.ColumnIntMapped, data.ColumnIntMapped);
                Assert.AreEqual($"{entity.ColumnNVarCharMapped} (Updated)", data.ColumnNVarCharMapped);
            }
        }

        [TestMethod]
        public void TestSqlConnectionObjectMappingInsertAndInlineUpdateViaQueryGroup()
        {
            // Setup
            var entity = new MappedObject
            {
                SessionIdMapped = Guid.NewGuid(),
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

                // Setup
                var queryGroup = new QueryGroup(new QueryField("SessionId", id).AsEnumerable());

                // Act Update
                var affectedRows = connection.InlineUpdate<MappedObject>(new
                {
                    ColumnNVarChar = $"{entity.ColumnNVarCharMapped} (Updated)"
                }, queryGroup);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act Query
                queryGroup.Reset();
                var data = connection.Query<MappedObject>(queryGroup).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.ColumnBigIntMapped, data.ColumnBigIntMapped);
                Assert.AreEqual(entity.ColumnBitMapped, data.ColumnBitMapped);
                Assert.AreEqual(entity.ColumnDateTime2Mapped, data.ColumnDateTime2Mapped);
                Assert.AreEqual(entity.ColumnDateTimeMapped, data.ColumnDateTimeMapped);
                Assert.AreEqual(entity.ColumnIntMapped, data.ColumnIntMapped);
                Assert.AreEqual($"{entity.ColumnNVarCharMapped} (Updated)", data.ColumnNVarCharMapped);
            }
        }

        #endregion

        #region Merge

        [TestMethod]
        public void TestSqlConnectionObjectMappingInsertAndMerge()
        {
            // Setup
            var entity = new MappedObject
            {
                SessionIdMapped = Guid.NewGuid(),
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

                // Setup
                entity.ColumnNVarCharMapped = $"{entity.ColumnNVarCharMapped} (Merged)";

                // Act Update
                var affectedRows = connection.Merge<MappedObject>(entity, Field.From("SessionId"));

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act Query
                var data = connection.Query<MappedObject>(new { SessionId = id }).FirstOrDefault();

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

        #region InlineMerge

        [TestMethod]
        public void TestSqlConnectionObjectMappingInsertAndInlineMerge()
        {
            // Setup
            var entity = new MappedObject
            {
                SessionIdMapped = Guid.NewGuid(),
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

                // Act Update
                var affectedRows = connection.InlineMerge<MappedObject>(new
                {
                    SessionId = (Guid)id,
                    ColumnNVarChar = $"{entity.ColumnNVarCharMapped} (Merged)"
                }, Field.From("SessionId"));

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act Query
                var data = connection.Query<MappedObject>(new { SessionId = id }).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.ColumnBigIntMapped, data.ColumnBigIntMapped);
                Assert.AreEqual(entity.ColumnBitMapped, data.ColumnBitMapped);
                Assert.AreEqual(entity.ColumnDateTime2Mapped, data.ColumnDateTime2Mapped);
                Assert.AreEqual(entity.ColumnDateTimeMapped, data.ColumnDateTimeMapped);
                Assert.AreEqual(entity.ColumnIntMapped, data.ColumnIntMapped);
                Assert.AreEqual($"{entity.ColumnNVarCharMapped} (Merged)", data.ColumnNVarCharMapped);
            }
        }

        #endregion

    }
}
