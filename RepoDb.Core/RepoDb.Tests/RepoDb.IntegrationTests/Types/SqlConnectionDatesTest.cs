using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace RepoDb.IntegrationTests.Types.Dates
{
    [TestClass]
    public class SqlConnectionDatesTest
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

        #region <TEntity>

        [TestMethod]
        public void TestSqlConnectionDatesCrud()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<DatesClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.ColumnDate, data.ColumnDate);
                Assert.AreEqual(entity.ColumnDateTime, data.ColumnDateTime);
                Assert.AreEqual(entity.ColumnDateTime2, data.ColumnDateTime2);
                Assert.AreEqual(dateTime.AddSeconds(30), data.ColumnSmallDateTime); // Always in a fraction of minutes, round (off/up)
                Assert.AreEqual(entity.ColumnDateTimeOffset, data.ColumnDateTimeOffset);
                Assert.AreEqual(entity.ColumnTime, data.ColumnTime);
            }
        }

        [TestMethod]
        public void TestSqlConnectionDatesNullCrud()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<DatesClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnDate);
                Assert.IsNull(data.ColumnDateTime);
                Assert.IsNull(data.ColumnDateTime2);
                Assert.IsNull(data.ColumnSmallDateTime);
                Assert.IsNull(data.ColumnDateTimeOffset);
                Assert.IsNull(data.ColumnTime);
            }
        }

        [TestMethod]
        public void TestSqlConnectionDatesMappedCrud()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<DatesMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.ColumnDateMapped, data.ColumnDateMapped);
                Assert.AreEqual(entity.ColumnDateTimeMapped, data.ColumnDateTimeMapped);
                Assert.AreEqual(entity.ColumnDateTime2Mapped, data.ColumnDateTime2Mapped);
                Assert.AreEqual(dateTime.AddSeconds(30), data.ColumnSmallDateTimeMapped); // Always in a fraction of minutes, round (off/up)
                Assert.AreEqual(entity.ColumnDateTimeOffsetMapped, data.ColumnDateTimeOffsetMapped);
                Assert.AreEqual(entity.ColumnTimeMapped, data.ColumnTimeMapped);
            }
        }

        [TestMethod]
        public void TestSqlConnectionDatesMappedNullCrud()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<DatesMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnDateMapped);
                Assert.IsNull(data.ColumnDateTimeMapped);
                Assert.IsNull(data.ColumnDateTime2Mapped);
                Assert.IsNull(data.ColumnSmallDateTimeMapped);
                Assert.IsNull(data.ColumnDateTimeOffsetMapped);
                Assert.IsNull(data.ColumnTimeMapped);
            }
        }

        [TestMethod]
        public void TestSqlConnectionDatesCrudAsync()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = connection.InsertAsync(entity);
                var id = insertResult.Result;

                // Act Query
                var queryResult = connection.QueryAsync<DatesClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.ColumnDate, data.ColumnDate);
                Assert.AreEqual(entity.ColumnDateTime, data.ColumnDateTime);
                Assert.AreEqual(entity.ColumnDateTime2, data.ColumnDateTime2); Assert.AreEqual(dateTime.AddSeconds(30), data.ColumnSmallDateTime); // Always in a fraction of minutes, round (off/up)
                Assert.AreEqual(entity.ColumnDateTimeOffset, data.ColumnDateTimeOffset);
                Assert.AreEqual(entity.ColumnTime, data.ColumnTime);
            }
        }

        [TestMethod]
        public void TestSqlConnectionDatesNullCrudAsync()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = connection.InsertAsync(entity);
                var id = insertResult.Result;

                // Act Query
                var queryResult = connection.QueryAsync<DatesClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnDate);
                Assert.IsNull(data.ColumnDateTime);
                Assert.IsNull(data.ColumnDateTime2);
                Assert.IsNull(data.ColumnSmallDateTime);
                Assert.IsNull(data.ColumnDateTimeOffset);
                Assert.IsNull(data.ColumnTime);
            }
        }

        [TestMethod]
        public void TestSqlConnectionDatesMappedCrudAsync()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = connection.InsertAsync(entity);
                var id = insertResult.Result;

                // Act Query
                var queryResult = connection.QueryAsync<DatesMapClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.ColumnDateMapped, data.ColumnDateMapped);
                Assert.AreEqual(entity.ColumnDateTimeMapped, data.ColumnDateTimeMapped);
                Assert.AreEqual(entity.ColumnDateTime2Mapped, data.ColumnDateTime2Mapped); Assert.AreEqual(dateTime.AddSeconds(30), data.ColumnSmallDateTimeMapped); // Always in a fraction of minutes, round (off/up)
                Assert.AreEqual(entity.ColumnDateTimeOffsetMapped, data.ColumnDateTimeOffsetMapped);
                Assert.AreEqual(entity.ColumnTimeMapped, data.ColumnTimeMapped);
            }
        }

        [TestMethod]
        public void TestSqlConnectionDatesMappedNullCrudAsync()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = connection.InsertAsync(entity);
                var id = insertResult.Result;

                // Act Query
                var queryResult = connection.QueryAsync<DatesMapClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnDateMapped);
                Assert.IsNull(data.ColumnDateTimeMapped);
                Assert.IsNull(data.ColumnDateTime2Mapped);
                Assert.IsNull(data.ColumnSmallDateTimeMapped);
                Assert.IsNull(data.ColumnDateTimeOffsetMapped);
                Assert.IsNull(data.ColumnTimeMapped);
            }
        }

        #endregion

        #region (TableName)

        [TestMethod]
        public void TestSqlConnectionDatesCrudViaTableName()
        {
            // Setup
            var dateTime = new DateTime(1970, 1, 1, 12, 50, 30, DateTimeKind.Utc);
            var dateTime2 = dateTime.AddMilliseconds(100);
            var entity = new
            {
                SessionId = Guid.NewGuid(),
                ColumnDate = dateTime.Date,
                ColumnDateTime = dateTime,
                ColumnDateTime2 = dateTime2,
                ColumnSmallDateTime = dateTime,
                ColumnDateTimeOffset = new DateTimeOffset(dateTime.Date).ToOffset(TimeSpan.FromHours(2)),
                ColumnTime = dateTime.TimeOfDay
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = connection.Insert(ClassMappedNameCache.Get<DatesClass>(), entity);

                // Act Query
                var data = connection.Query(ClassMappedNameCache.Get<DatesClass>(), new { SessionId = (Guid)id }).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.ColumnDate, data.ColumnDate);
                Assert.AreEqual(entity.ColumnDateTime, data.ColumnDateTime);
                Assert.AreEqual(entity.ColumnDateTime2, data.ColumnDateTime2);
                Assert.AreEqual(dateTime.AddSeconds(30), data.ColumnSmallDateTime); // Always in a fraction of minutes, round (off/up)
                Assert.AreEqual(entity.ColumnDateTimeOffset, data.ColumnDateTimeOffset);
                Assert.AreEqual(entity.ColumnTime, data.ColumnTime);
            }
        }

        [TestMethod]
        public void TestSqlConnectionDatesNullCrudViaTableName()
        {
            // Setup
            var entity = new
            {
                SessionId = Guid.NewGuid(),
                ColumnDate = (DateTime?)null,
                ColumnDateTime = (DateTime?)null,
                ColumnDateTime2 = (DateTime?)null,
                ColumnSmallDateTime = (DateTime?)null,
                ColumnDateTimeOffset = (DateTimeOffset?)null,
                ColumnTime = (TimeSpan?)null
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = connection.Insert(ClassMappedNameCache.Get<DatesClass>(), entity);

                // Act Query
                var data = connection.Query(ClassMappedNameCache.Get<DatesClass>(), new { SessionId = (Guid)id }).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnDate);
                Assert.IsNull(data.ColumnDateTime);
                Assert.IsNull(data.ColumnDateTime2);
                Assert.IsNull(data.ColumnSmallDateTime);
                Assert.IsNull(data.ColumnDateTimeOffset);
                Assert.IsNull(data.ColumnTime);
            }
        }

        [TestMethod]
        public void TestSqlConnectionDatesCrudViaAsyncViaTableName()
        {
            // Setup
            var dateTime = new DateTime(1970, 1, 1, 12, 50, 30, DateTimeKind.Utc);
            var dateTime2 = dateTime.AddMilliseconds(100);
            var entity = new
            {
                SessionId = Guid.NewGuid(),
                ColumnDate = dateTime.Date,
                ColumnDateTime = dateTime,
                ColumnDateTime2 = dateTime2,
                ColumnSmallDateTime = dateTime,
                ColumnDateTimeOffset = new DateTimeOffset(dateTime.Date).ToOffset(TimeSpan.FromHours(2)),
                ColumnTime = dateTime.TimeOfDay
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = connection.InsertAsync(ClassMappedNameCache.Get<DatesClass>(), entity);
                var id = insertResult.Result;

                // Act Query
                var queryResult = connection.QueryAsync(ClassMappedNameCache.Get<DatesClass>(), new { SessionId = (Guid)id });
                var data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.ColumnDate, data.ColumnDate);
                Assert.AreEqual(entity.ColumnDateTime, data.ColumnDateTime);
                Assert.AreEqual(entity.ColumnDateTime2, data.ColumnDateTime2);
                Assert.AreEqual(dateTime.AddSeconds(30), data.ColumnSmallDateTime); // Always in a fraction of minutes, round (off/up)
                Assert.AreEqual(entity.ColumnDateTimeOffset, data.ColumnDateTimeOffset);
                Assert.AreEqual(entity.ColumnTime, data.ColumnTime);
            }
        }

        [TestMethod]
        public void TestSqlConnectionDatesNullCrudViaAsyncViaTableName()
        {
            // Setup
            var entity = new
            {
                SessionId = Guid.NewGuid(),
                ColumnDate = (DateTime?)null,
                ColumnDateTime = (DateTime?)null,
                ColumnDateTime2 = (DateTime?)null,
                ColumnSmallDateTime = (DateTime?)null,
                ColumnDateTimeOffset = (DateTimeOffset?)null,
                ColumnTime = (TimeSpan?)null
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = connection.InsertAsync(ClassMappedNameCache.Get<DatesClass>(), entity);
                var id = insertResult.Result;

                // Act Query
                var queryResult = connection.QueryAsync(ClassMappedNameCache.Get<DatesClass>(), new { SessionId = (Guid)id });
                var data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnDate);
                Assert.IsNull(data.ColumnDateTime);
                Assert.IsNull(data.ColumnDateTime2);
                Assert.IsNull(data.ColumnSmallDateTime);
                Assert.IsNull(data.ColumnDateTimeOffset);
                Assert.IsNull(data.ColumnTime);
            }
        }

        #endregion
    }
}
