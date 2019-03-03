using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace RepoDb.IntegrationTests.Types.Dates
{
    [TestClass]
    public class DbRepositoryDatesTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Database.Init();
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.DeleteAll<DatesClass>();
            }
        }

        [TestMethod]
        public void TestDbRepositoryDatesCrud()
        {
            // Setup
            var dateTime = new DateTime(1970, 1, 1, 12, 50, 30, DateTimeKind.Utc);
            var dateTime2 = dateTime.AddMilliseconds(100);
            var entity = new DatesClass
            {
                SessionId = Guid.NewGuid(),
                ColumnDate = dateTime.Date,
                ColumnDateTime = dateTime,
                ColumnDateTime2 = dateTime2,
                ColumnSmallDateTime = dateTime,
                ColumnDateTimeOffset = new DateTimeOffset(dateTime.Date).ToOffset(TimeSpan.FromHours(2)),
                ColumnTime = dateTime.TimeOfDay
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query<DatesClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.ColumnDate, data.ColumnDate);
                Assert.AreEqual(entity.ColumnDateTime, data.ColumnDateTime);
                Assert.AreEqual(entity.ColumnDateTime2, data.ColumnDateTime2); Assert.AreEqual(dateTime.AddSeconds(30), data.ColumnSmallDateTime); // Always in a fraction of minutes, round (off/up)
                Assert.AreEqual(entity.ColumnDateTimeOffset, data.ColumnDateTimeOffset);
                Assert.AreEqual(entity.ColumnTime, data.ColumnTime);

                // Act Delete
                var deletedRows = repository.Delete<DatesClass>(e => e.SessionId == (Guid)id);

                // Act Query
                data = repository.Query<DatesClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositoryDatesNullCrud()
        {
            // Setup
            var entity = new DatesClass
            {
                SessionId = Guid.NewGuid(),
                ColumnDate = null,
                ColumnDateTime = null,
                ColumnDateTime2 = null,
                ColumnSmallDateTime = null,
                ColumnDateTimeOffset = null,
                ColumnTime = null
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query<DatesClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnDate);
                Assert.IsNull(data.ColumnDateTime);
                Assert.IsNull(data.ColumnDateTime2);
                Assert.IsNull(data.ColumnSmallDateTime);
                Assert.IsNull(data.ColumnDateTimeOffset);
                Assert.IsNull(data.ColumnTime);

                // Act Delete
                var deletedRows = repository.Delete<DatesClass>(e => e.SessionId == (Guid)id);

                // Act Query
                data = repository.Query<DatesClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositoryDatesMappedCrud()
        {
            // Setup
            var dateTime = new DateTime(1970, 1, 1, 12, 50, 30, DateTimeKind.Utc);
            var dateTime2 = dateTime.AddMilliseconds(100);
            var entity = new DatesMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnDateMapped = dateTime.Date,
                ColumnDateTimeMapped = dateTime,
                ColumnDateTime2Mapped = dateTime2,
                ColumnSmallDateTimeMapped = dateTime,
                ColumnDateTimeOffsetMapped = new DateTimeOffset(dateTime.Date).ToOffset(TimeSpan.FromHours(2)),
                ColumnTimeMapped = dateTime.TimeOfDay
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query<DatesMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.ColumnDateMapped, data.ColumnDateMapped);
                Assert.AreEqual(entity.ColumnDateTimeMapped, data.ColumnDateTimeMapped);
                Assert.AreEqual(entity.ColumnDateTime2Mapped, data.ColumnDateTime2Mapped); Assert.AreEqual(dateTime.AddSeconds(30), data.ColumnSmallDateTimeMapped); // Always in a fraction of minutes, round (off/up)
                Assert.AreEqual(entity.ColumnDateTimeOffsetMapped, data.ColumnDateTimeOffsetMapped);
                Assert.AreEqual(entity.ColumnTimeMapped, data.ColumnTimeMapped);

                // Act Delete
                var deletedRows = repository.Delete<DatesMapClass>(e => e.SessionId == (Guid)id);

                // Act Query
                data = repository.Query<DatesMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositoryDatesMappedNullCrud()
        {
            // Setup
            var entity = new DatesMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnDateMapped = null,
                ColumnDateTimeMapped = null,
                ColumnDateTime2Mapped = null,
                ColumnSmallDateTimeMapped = null,
                ColumnDateTimeOffsetMapped = null,
                ColumnTimeMapped = null
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query<DatesMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnDateMapped);
                Assert.IsNull(data.ColumnDateTimeMapped);
                Assert.IsNull(data.ColumnDateTime2Mapped);
                Assert.IsNull(data.ColumnSmallDateTimeMapped);
                Assert.IsNull(data.ColumnDateTimeOffsetMapped);
                Assert.IsNull(data.ColumnTimeMapped);

                // Act Delete
                var deletedRows = repository.Delete<DatesMapClass>(e => e.SessionId == (Guid)id);

                // Act Query
                data = repository.Query<DatesMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositoryDatesCrudAsync()
        {
            // Setup
            var dateTime = new DateTime(1970, 1, 1, 12, 50, 30, DateTimeKind.Utc);
            var dateTime2 = dateTime.AddMilliseconds(100);
            var entity = new DatesClass
            {
                SessionId = Guid.NewGuid(),
                ColumnDate = dateTime.Date,
                ColumnDateTime = dateTime,
                ColumnDateTime2 = dateTime2,
                ColumnSmallDateTime = dateTime,
                ColumnDateTimeOffset = new DateTimeOffset(dateTime.Date).ToOffset(TimeSpan.FromHours(2)),
                ColumnTime = dateTime.TimeOfDay
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result.Extract();

                // Act Query
                var queryResult = repository.QueryAsync<DatesClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.ColumnDate, data.ColumnDate);
                Assert.AreEqual(entity.ColumnDateTime, data.ColumnDateTime);
                Assert.AreEqual(entity.ColumnDateTime2, data.ColumnDateTime2); Assert.AreEqual(dateTime.AddSeconds(30), data.ColumnSmallDateTime); // Always in a fraction of minutes, round (off/up)
                Assert.AreEqual(entity.ColumnDateTimeOffset, data.ColumnDateTimeOffset);
                Assert.AreEqual(entity.ColumnTime, data.ColumnTime);

                // Act Delete
                var deleteAsyncResult = repository.DeleteAsync<DatesClass>(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result.Extract();

                // Act Query
                queryResult = repository.QueryAsync<DatesClass>(e => e.SessionId == (Guid)id);
                data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositoryDatesNullCrudAsync()
        {
            // Setup
            var entity = new DatesClass
            {
                SessionId = Guid.NewGuid(),
                ColumnDate = null,
                ColumnDateTime = null,
                ColumnDateTime2 = null,
                ColumnSmallDateTime = null,
                ColumnDateTimeOffset = null,
                ColumnTime = null
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result.Extract();

                // Act Query
                var queryResult = repository.QueryAsync<DatesClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnDate);
                Assert.IsNull(data.ColumnDateTime);
                Assert.IsNull(data.ColumnDateTime2);
                Assert.IsNull(data.ColumnSmallDateTime);
                Assert.IsNull(data.ColumnDateTimeOffset);
                Assert.IsNull(data.ColumnTime);

                // Act Delete
                var deleteAsyncResult = repository.DeleteAsync<DatesClass>(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result.Extract();

                // Act Query
                queryResult = repository.QueryAsync<DatesClass>(e => e.SessionId == (Guid)id);
                data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositoryDatesMappedCrudAsync()
        {
            // Setup
            var dateTime = new DateTime(1970, 1, 1, 12, 50, 30, DateTimeKind.Utc);
            var dateTime2 = dateTime.AddMilliseconds(100);
            var entity = new DatesMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnDateMapped = dateTime.Date,
                ColumnDateTimeMapped = dateTime,
                ColumnDateTime2Mapped = dateTime2,
                ColumnSmallDateTimeMapped = dateTime,
                ColumnDateTimeOffsetMapped = new DateTimeOffset(dateTime.Date).ToOffset(TimeSpan.FromHours(2)),
                ColumnTimeMapped = dateTime.TimeOfDay
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result.Extract();

                // Act Query
                var queryResult = repository.QueryAsync<DatesMapClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.ColumnDateMapped, data.ColumnDateMapped);
                Assert.AreEqual(entity.ColumnDateTimeMapped, data.ColumnDateTimeMapped);
                Assert.AreEqual(entity.ColumnDateTime2Mapped, data.ColumnDateTime2Mapped); Assert.AreEqual(dateTime.AddSeconds(30), data.ColumnSmallDateTimeMapped); // Always in a fraction of minutes, round (off/up)
                Assert.AreEqual(entity.ColumnDateTimeOffsetMapped, data.ColumnDateTimeOffsetMapped);
                Assert.AreEqual(entity.ColumnTimeMapped, data.ColumnTimeMapped);

                // Act Delete
                var deleteAsyncResult = repository.DeleteAsync<DatesMapClass>(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result.Extract();

                // Act Query
                queryResult = repository.QueryAsync<DatesMapClass>(e => e.SessionId == (Guid)id);
                data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositoryDatesMappedNullCrudAsync()
        {
            // Setup
            var entity = new DatesMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnDateMapped = null,
                ColumnDateTimeMapped = null,
                ColumnDateTime2Mapped = null,
                ColumnSmallDateTimeMapped = null,
                ColumnDateTimeOffsetMapped = null,
                ColumnTimeMapped = null
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result.Extract();

                // Act Query
                var queryResult = repository.QueryAsync<DatesMapClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnDateMapped);
                Assert.IsNull(data.ColumnDateTimeMapped);
                Assert.IsNull(data.ColumnDateTime2Mapped);
                Assert.IsNull(data.ColumnSmallDateTimeMapped);
                Assert.IsNull(data.ColumnDateTimeOffsetMapped);
                Assert.IsNull(data.ColumnTimeMapped);

                // Act Delete
                var deleteAsyncResult = repository.DeleteAsync<DatesMapClass>(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result.Extract();

                // Act Query
                queryResult = repository.QueryAsync<DatesMapClass>(e => e.SessionId == (Guid)id);
                data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }
    }
}
