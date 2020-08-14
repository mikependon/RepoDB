using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Contexts.Execution;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RepoDb.UnitTests.DbHelpers
{
    [TestClass]
    public class DbHelperTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add(typeof(DbHelperDbConnection), new CustomDbSetting(), true);
            StatementBuilderMapper.Add(typeof(DbHelperDbConnection), new CustomStatementBuilder(), true);
        }

        #region SubClasses

        private class DbHelperDataEntity
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class DbHelperDbConnection : CustomDbConnection { }

        #endregion

        #region Helpers

        private IEnumerable<DbField> GetDbFields()
        {
            return new[]
            {
                new DbField("Id", true, true, false, typeof(int), null, null, null, null),
                new DbField("Name", false, false, false, typeof(string), null, null, null, null)
            };
        }

        private Mock<IDbHelper> GetMockedDbHelper()
        {
            var dbHelper = new Mock<IDbHelper>();
            dbHelper
                .Setup(e => e.GetFields(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<IDbTransaction>()))
                .Returns(GetDbFields());
            return dbHelper;
        }

        #endregion

        #region Operation<TEntity>

        [TestMethod]
        public void TestDbHelperForBatchQuery()
        {
            // Prepare
            var dbHelper = GetMockedDbHelper();
            var connection = new DbHelperDbConnection();

            // Act
            CommandTextCache.Flush();
            DbFieldCache.Flush();
            DbHelperMapper.Add(typeof(DbHelperDbConnection), dbHelper.Object, true);

            // Act
            connection.BatchQuery<DbHelperDataEntity>(0,
                10,
                OrderField.Ascending<DbHelperDataEntity>(e => e.Id).AsEnumerable(),
                e => e.Id == 1);

            // Assert
            dbHelper.Verify(builder =>
                builder.GetFields(
                    It.Is<IDbConnection>(s => s == connection),
                    It.Is<string>(s => s == ClassMappedNameCache.Get<DbHelperDataEntity>()),
                    It.IsAny<IDbTransaction>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbHelperForInsert()
        {
            // Prepare
            var dbHelper = GetMockedDbHelper();
            var connection = new DbHelperDbConnection();

            // Act
            CommandTextCache.Flush();
            DbFieldCache.Flush();
            DbHelperMapper.Add(typeof(DbHelperDbConnection), dbHelper.Object, true);

            // Act
            connection.Insert<DbHelperDataEntity>(new DbHelperDataEntity { Id = 1, Name = "Name" });

            // Assert
            dbHelper.Verify(builder =>
                builder.GetFields(
                    It.Is<IDbConnection>(s => s == connection),
                    It.Is<string>(s => s == ClassMappedNameCache.Get<DbHelperDataEntity>()),
                    It.IsAny<IDbTransaction>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbHelperForInsertAll()
        {
            // Prepare
            var dbHelper = GetMockedDbHelper();
            var connection = new DbHelperDbConnection();

            // Act
            CommandTextCache.Flush();
            DbFieldCache.Flush();
            DbHelperMapper.Add(typeof(DbHelperDbConnection), dbHelper.Object, true);

            // Act
            connection.InsertAll<DbHelperDataEntity>(new[] { new DbHelperDataEntity { Id = 1, Name = "Name" } });

            // Assert
            dbHelper.Verify(builder =>
                builder.GetFields(
                    It.Is<IDbConnection>(s => s == connection),
                    It.Is<string>(s => s == ClassMappedNameCache.Get<DbHelperDataEntity>()),
                    It.IsAny<IDbTransaction>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbHelperForMerge()
        {
            // Prepare
            var dbHelper = GetMockedDbHelper();
            var connection = new DbHelperDbConnection();

            // Act
            CommandTextCache.Flush();
            DbFieldCache.Flush();
            DbHelperMapper.Add(typeof(DbHelperDbConnection), dbHelper.Object, true);

            // Act
            connection.Merge<DbHelperDataEntity>(new DbHelperDataEntity { Id = 1, Name = "Name" });

            // Assert
            dbHelper.Verify(builder =>
                builder.GetFields(
                    It.Is<IDbConnection>(s => s == connection),
                    It.Is<string>(s => s == ClassMappedNameCache.Get<DbHelperDataEntity>()),
                    It.IsAny<IDbTransaction>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbHelperForMergeAll()
        {
            // Prepare
            var dbHelper = GetMockedDbHelper();
            var connection = new DbHelperDbConnection();

            // Act
            CommandTextCache.Flush();
            DbFieldCache.Flush();
            DbHelperMapper.Add(typeof(DbHelperDbConnection), dbHelper.Object, true);

            // Act
            connection.MergeAll<DbHelperDataEntity>(new[] { new DbHelperDataEntity { Id = 1, Name = "Name" } });

            // Assert
            dbHelper.Verify(builder =>
                builder.GetFields(
                    It.Is<IDbConnection>(s => s == connection),
                    It.Is<string>(s => s == ClassMappedNameCache.Get<DbHelperDataEntity>()),
                    It.IsAny<IDbTransaction>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbHelperForQuery()
        {
            // Prepare
            var dbHelper = GetMockedDbHelper();
            var connection = new DbHelperDbConnection();

            // Act
            CommandTextCache.Flush();
            DbFieldCache.Flush();
            DbHelperMapper.Add(typeof(DbHelperDbConnection), dbHelper.Object, true);

            // Act
            connection.Query<DbHelperDataEntity>((object)null);

            // Assert
            dbHelper.Verify(builder =>
                builder.GetFields(
                    It.Is<IDbConnection>(s => s == connection),
                    It.Is<string>(s => s == ClassMappedNameCache.Get<DbHelperDataEntity>()),
                    It.IsAny<IDbTransaction>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbHelperForQueryAll()
        {
            // Prepare
            var dbHelper = GetMockedDbHelper();
            var connection = new DbHelperDbConnection();

            // Act
            CommandTextCache.Flush();
            DbFieldCache.Flush();
            DbHelperMapper.Add(typeof(DbHelperDbConnection), dbHelper.Object, true);

            // Act
            connection.QueryAll<DbHelperDataEntity>();

            // Assert
            dbHelper.Verify(builder =>
                builder.GetFields(
                    It.Is<IDbConnection>(s => s == connection),
                    It.Is<string>(s => s == ClassMappedNameCache.Get<DbHelperDataEntity>()),
                    It.IsAny<IDbTransaction>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbHelperForUpdate()
        {
            // Prepare
            var dbHelper = GetMockedDbHelper();
            var connection = new DbHelperDbConnection();

            // Act
            CommandTextCache.Flush();
            DbFieldCache.Flush();
            DbHelperMapper.Add(typeof(DbHelperDbConnection), dbHelper.Object, true);

            // Act
            connection.Update<DbHelperDataEntity>(new DbHelperDataEntity { Id = 1, Name = "Name" });

            // Assert
            dbHelper.Verify(builder =>
                builder.GetFields(
                    It.Is<IDbConnection>(s => s == connection),
                    It.Is<string>(s => s == ClassMappedNameCache.Get<DbHelperDataEntity>()),
                    It.IsAny<IDbTransaction>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbHelperForUpdateAll()
        {
            // Prepare
            var dbHelper = GetMockedDbHelper();
            var connection = new DbHelperDbConnection();

            // Act
            CommandTextCache.Flush();
            DbFieldCache.Flush();
            DbHelperMapper.Add(typeof(DbHelperDbConnection), dbHelper.Object, true);

            // Act
            connection.UpdateAll<DbHelperDataEntity>(new[] { new DbHelperDataEntity { Id = 1, Name = "Name" } });

            // Assert
            dbHelper.Verify(builder =>
                builder.GetFields(
                    It.Is<IDbConnection>(s => s == connection),
                    It.Is<string>(s => s == ClassMappedNameCache.Get<DbHelperDataEntity>()),
                    It.IsAny<IDbTransaction>()), Times.Exactly(1));
        }

        #endregion

        #region Operation(TableName)

        [TestMethod]
        public void TestDbHelperForBatchQueryViaTableName()
        {
            // Prepare
            var dbHelper = GetMockedDbHelper();
            var connection = new DbHelperDbConnection();

            // Act
            CommandTextCache.Flush();
            DbFieldCache.Flush();
            DbHelperMapper.Add(typeof(DbHelperDbConnection), dbHelper.Object, true);

            // Act
            connection.BatchQuery(ClassMappedNameCache.Get<DbHelperDataEntity>(),
                0,
                10,
                OrderField.Ascending<DbHelperDataEntity>(e => e.Id).AsEnumerable(),
                new { Id = 1 });

            // Assert
            dbHelper.Verify(builder =>
                builder.GetFields(
                    It.Is<IDbConnection>(s => s == connection),
                    It.Is<string>(s => s == ClassMappedNameCache.Get<DbHelperDataEntity>()),
                    It.IsAny<IDbTransaction>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbHelperForInsertViaTableName()
        {
            // Prepare
            var dbHelper = GetMockedDbHelper();
            var connection = new DbHelperDbConnection();

            // Act
            CommandTextCache.Flush();
            DbFieldCache.Flush();
            DbHelperMapper.Add(typeof(DbHelperDbConnection), dbHelper.Object, true);

            // Act
            connection.Insert(ClassMappedNameCache.Get<DbHelperDataEntity>(),
                new { Id = 1, Name = "Name" });

            // Assert
            dbHelper.Verify(builder =>
                builder.GetFields(
                    It.Is<IDbConnection>(s => s == connection),
                    It.Is<string>(s => s == ClassMappedNameCache.Get<DbHelperDataEntity>()),
                    It.IsAny<IDbTransaction>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbHelperForInsertAllViaTableName()
        {
            // Prepare
            var dbHelper = GetMockedDbHelper();
            var connection = new DbHelperDbConnection();

            // Act
            CommandTextCache.Flush();
            DbFieldCache.Flush();
            DbHelperMapper.Add(typeof(DbHelperDbConnection), dbHelper.Object, true);

            // Act
            connection.InsertAll(ClassMappedNameCache.Get<DbHelperDataEntity>(),
                new[] { new { Id = 1, Name = "Name" } });

            // Assert
            dbHelper.Verify(builder =>
                builder.GetFields(
                    It.Is<IDbConnection>(s => s == connection),
                    It.Is<string>(s => s == ClassMappedNameCache.Get<DbHelperDataEntity>()),
                    It.IsAny<IDbTransaction>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbHelperForMergeViaTableName()
        {
            // Prepare
            var dbHelper = GetMockedDbHelper();
            var connection = new DbHelperDbConnection();

            // Act
            CommandTextCache.Flush();
            DbFieldCache.Flush();
            DbHelperMapper.Add(typeof(DbHelperDbConnection), dbHelper.Object, true);

            // Act
            connection.Merge(ClassMappedNameCache.Get<DbHelperDataEntity>(),
                new { Id = 1, Name = "Name" });

            // Assert
            dbHelper.Verify(builder =>
                builder.GetFields(
                    It.Is<IDbConnection>(s => s == connection),
                    It.Is<string>(s => s == ClassMappedNameCache.Get<DbHelperDataEntity>()),
                    It.IsAny<IDbTransaction>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbHelperForMergeAllViaTableName()
        {
            // Prepare
            var dbHelper = GetMockedDbHelper();
            var connection = new DbHelperDbConnection();

            // Act
            CommandTextCache.Flush();
            DbFieldCache.Flush();
            DbHelperMapper.Add(typeof(DbHelperDbConnection), dbHelper.Object, true);

            // Act
            connection.MergeAll(ClassMappedNameCache.Get<DbHelperDataEntity>(),
                new[] { new DbHelperDataEntity { Id = 1, Name = "Name" } });

            // Assert
            dbHelper.Verify(builder =>
                builder.GetFields(
                    It.Is<IDbConnection>(s => s == connection),
                    It.Is<string>(s => s == ClassMappedNameCache.Get<DbHelperDataEntity>()),
                    It.IsAny<IDbTransaction>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbHelperForQueryViaTableName()
        {
            // Prepare
            var dbHelper = GetMockedDbHelper();
            var connection = new DbHelperDbConnection();

            // Act
            CommandTextCache.Flush();
            DbFieldCache.Flush();
            DbHelperMapper.Add(typeof(DbHelperDbConnection), dbHelper.Object, true);

            // Act
            connection.Query(ClassMappedNameCache.Get<DbHelperDataEntity>(),
                (object)null);

            // Assert
            dbHelper.Verify(builder =>
                builder.GetFields(
                    It.Is<IDbConnection>(s => s == connection),
                    It.Is<string>(s => s == ClassMappedNameCache.Get<DbHelperDataEntity>()),
                    It.IsAny<IDbTransaction>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbHelperForQueryAllViaTableName()
        {
            // Prepare
            var dbHelper = GetMockedDbHelper();
            var connection = new DbHelperDbConnection();

            // Act
            CommandTextCache.Flush();
            DbFieldCache.Flush();
            DbHelperMapper.Add(typeof(DbHelperDbConnection), dbHelper.Object, true);

            // Act
            connection.QueryAll(ClassMappedNameCache.Get<DbHelperDataEntity>());

            // Assert
            dbHelper.Verify(builder =>
                builder.GetFields(
                    It.Is<IDbConnection>(s => s == connection),
                    It.Is<string>(s => s == ClassMappedNameCache.Get<DbHelperDataEntity>()),
                    It.IsAny<IDbTransaction>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbHelperForUpdateViaTableName()
        {
            // Prepare
            var dbHelper = GetMockedDbHelper();
            var connection = new DbHelperDbConnection();

            // Act
            CommandTextCache.Flush();
            DbFieldCache.Flush();
            DbHelperMapper.Add(typeof(DbHelperDbConnection), dbHelper.Object, true);

            // Act
            connection.Update(ClassMappedNameCache.Get<DbHelperDataEntity>(),
                new { Id = 1, Name = "Name" });

            // Assert
            dbHelper.Verify(builder =>
                builder.GetFields(
                    It.Is<IDbConnection>(s => s == connection),
                    It.Is<string>(s => s == ClassMappedNameCache.Get<DbHelperDataEntity>()),
                    It.IsAny<IDbTransaction>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbHelperForUpdateAllViaTableName()
        {
            // Prepare
            var dbHelper = GetMockedDbHelper();
            var connection = new DbHelperDbConnection();

            // Act
            CommandTextCache.Flush();
            DbFieldCache.Flush();
            DbHelperMapper.Add(typeof(DbHelperDbConnection), dbHelper.Object, true);

            // Act
            connection.UpdateAll(ClassMappedNameCache.Get<DbHelperDataEntity>(),
                new[] { new DbHelperDataEntity { Id = 1, Name = "Name" } });

            // Assert
            dbHelper.Verify(builder =>
                builder.GetFields(
                    It.Is<IDbConnection>(s => s == connection),
                    It.Is<string>(s => s == ClassMappedNameCache.Get<DbHelperDataEntity>()),
                    It.IsAny<IDbTransaction>()), Times.Exactly(1));
        }

        #endregion
    }
}
