using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
        #region SubClasses

        private class DataEntityForDbHelper
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class CustomDbConnectionForDbHelper : CustomDbConnection { }

        #endregion

        #region Helpers

        private static IEnumerable<DbField> GetDbFields()
        {
            return new[]
            {
                new DbField("Id", true, true, false, typeof(int), null, null, null),
                new DbField("Name", false, false, false, typeof(string), null, null, null)
            };
        }

        #endregion

        #region Operation<TEntity>

        [TestMethod]
        public void TestDbHelperForBatchQuery()
        {
            // Prepare
            var dbHelper = new Mock<IDbHelper>();
            var connection = new CustomDbConnectionForDbHelper();

            // Setup
            dbHelper.Setup(builder =>
                builder.GetFields(
                    It.Is<IDbConnection>(s => s == connection),
                    It.Is<string>(s => s == ClassMappedNameCache.Get<DataEntityForDbHelper>()))
                ).Returns(GetDbFields());

            // Act
            CommandTextCache.Flush();
            DbFieldCache.Flush();
            StatementBuilderMapper.Add(typeof(CustomDbConnectionForDbHelper), new SqlStatementBuilder(), true);
            DbHelperMapper.Add(typeof(CustomDbConnectionForDbHelper), dbHelper.Object, true);

            // Act
            connection.BatchQuery<DataEntityForDbHelper>(0,
                10,
                OrderField.Ascending<DataEntityForDbHelper>(e => e.Id).AsEnumerable(),
                e => e.Id == 1);

            // Assert
            dbHelper.Verify(builder =>
                builder.GetFields(
                    It.Is<IDbConnection>(s => s == connection),
                    It.Is<string>(s => s == ClassMappedNameCache.Get<DataEntityForDbHelper>())), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbHelperForInsert()
        {
            // Prepare
            var dbHelper = new Mock<IDbHelper>();
            var connection = new CustomDbConnectionForDbHelper();

            // Act
            CommandTextCache.Flush();
            DbFieldCache.Flush();
            StatementBuilderMapper.Add(typeof(CustomDbConnectionForDbHelper), new SqlStatementBuilder(), true);
            DbHelperMapper.Add(typeof(CustomDbConnectionForDbHelper), dbHelper.Object, true);

            // Act
            connection.Insert<DataEntityForDbHelper>(new DataEntityForDbHelper { Id = 1, Name = "Name" });

            // Assert
            dbHelper.Verify(builder =>
                builder.GetFields(
                    It.Is<IDbConnection>(s => s == connection),
                    It.Is<string>(s => s == ClassMappedNameCache.Get<DataEntityForDbHelper>())), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbHelperForInsertAll()
        {
            // Prepare
            var dbHelper = new Mock<IDbHelper>();
            var connection = new CustomDbConnectionForDbHelper();

            // Act
            CommandTextCache.Flush();
            DbFieldCache.Flush();
            StatementBuilderMapper.Add(typeof(CustomDbConnectionForDbHelper), new SqlStatementBuilder(), true);
            DbHelperMapper.Add(typeof(CustomDbConnectionForDbHelper), dbHelper.Object, true);

            // Act
            connection.InsertAll<DataEntityForDbHelper>(new[] { new DataEntityForDbHelper { Id = 1, Name = "Name" } });

            // Assert
            dbHelper.Verify(builder =>
                builder.GetFields(
                    It.Is<IDbConnection>(s => s == connection),
                    It.Is<string>(s => s == ClassMappedNameCache.Get<DataEntityForDbHelper>())), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbHelperForMerge()
        {
            // Prepare
            var dbHelper = new Mock<IDbHelper>();
            var connection = new CustomDbConnectionForDbHelper();

            // Act
            CommandTextCache.Flush();
            DbFieldCache.Flush();
            StatementBuilderMapper.Add(typeof(CustomDbConnectionForDbHelper), new SqlStatementBuilder(), true);
            DbHelperMapper.Add(typeof(CustomDbConnectionForDbHelper), dbHelper.Object, true);

            // Act
            connection.Merge<DataEntityForDbHelper>(new DataEntityForDbHelper { Id = 1, Name = "Name" });

            // Assert
            dbHelper.Verify(builder =>
                builder.GetFields(
                    It.Is<IDbConnection>(s => s == connection),
                    It.Is<string>(s => s == ClassMappedNameCache.Get<DataEntityForDbHelper>())), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbHelperForMergeAll()
        {
            // Prepare
            var dbHelper = new Mock<IDbHelper>();
            var connection = new CustomDbConnectionForDbHelper();

            // Act
            CommandTextCache.Flush();
            DbFieldCache.Flush();
            StatementBuilderMapper.Add(typeof(CustomDbConnectionForDbHelper), new SqlStatementBuilder(), true);
            DbHelperMapper.Add(typeof(CustomDbConnectionForDbHelper), dbHelper.Object, true);

            // Act
            connection.MergeAll<DataEntityForDbHelper>(new[] { new DataEntityForDbHelper { Id = 1, Name = "Name" } });

            // Assert
            dbHelper.Verify(builder =>
                builder.GetFields(
                    It.Is<IDbConnection>(s => s == connection),
                    It.Is<string>(s => s == ClassMappedNameCache.Get<DataEntityForDbHelper>())), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbHelperForQuery()
        {
            // Prepare
            var dbHelper = new Mock<IDbHelper>();
            var connection = new CustomDbConnectionForDbHelper();

            // Act
            CommandTextCache.Flush();
            DbFieldCache.Flush();
            StatementBuilderMapper.Add(typeof(CustomDbConnectionForDbHelper), new SqlStatementBuilder(), true);
            DbHelperMapper.Add(typeof(CustomDbConnectionForDbHelper), dbHelper.Object, true);

            // Act
            connection.Query<DataEntityForDbHelper>((object)null);

            // Assert
            dbHelper.Verify(builder =>
                builder.GetFields(
                    It.Is<IDbConnection>(s => s == connection),
                    It.Is<string>(s => s == ClassMappedNameCache.Get<DataEntityForDbHelper>())), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbHelperForQueryAll()
        {
            // Prepare
            var dbHelper = new Mock<IDbHelper>();
            var connection = new CustomDbConnectionForDbHelper();

            // Act
            CommandTextCache.Flush();
            DbFieldCache.Flush();
            StatementBuilderMapper.Add(typeof(CustomDbConnectionForDbHelper), new SqlStatementBuilder(), true);
            DbHelperMapper.Add(typeof(CustomDbConnectionForDbHelper), dbHelper.Object, true);

            // Act
            connection.QueryAll<DataEntityForDbHelper>();

            // Assert
            dbHelper.Verify(builder =>
                builder.GetFields(
                    It.Is<IDbConnection>(s => s == connection),
                    It.Is<string>(s => s == ClassMappedNameCache.Get<DataEntityForDbHelper>())), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbHelperForUpdate()
        {
            // Prepare
            var dbHelper = new Mock<IDbHelper>();
            var connection = new CustomDbConnectionForDbHelper();

            // Act
            CommandTextCache.Flush();
            DbFieldCache.Flush();
            StatementBuilderMapper.Add(typeof(CustomDbConnectionForDbHelper), new SqlStatementBuilder(), true);
            DbHelperMapper.Add(typeof(CustomDbConnectionForDbHelper), dbHelper.Object, true);

            // Act
            connection.Update<DataEntityForDbHelper>(new DataEntityForDbHelper { Id = 1, Name = "Name" });

            // Assert
            dbHelper.Verify(builder =>
                builder.GetFields(
                    It.Is<IDbConnection>(s => s == connection),
                    It.Is<string>(s => s == ClassMappedNameCache.Get<DataEntityForDbHelper>())), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbHelperForUpdateAll()
        {
            // Prepare
            var dbHelper = new Mock<IDbHelper>();
            var connection = new CustomDbConnectionForDbHelper();

            // Act
            CommandTextCache.Flush();
            DbFieldCache.Flush();
            StatementBuilderMapper.Add(typeof(CustomDbConnectionForDbHelper), new SqlStatementBuilder(), true);
            DbHelperMapper.Add(typeof(CustomDbConnectionForDbHelper), dbHelper.Object, true);

            // Act
            connection.UpdateAll<DataEntityForDbHelper>(new[] { new DataEntityForDbHelper { Id = 1, Name = "Name" } });

            // Assert
            dbHelper.Verify(builder =>
                builder.GetFields(
                    It.Is<IDbConnection>(s => s == connection),
                    It.Is<string>(s => s == ClassMappedNameCache.Get<DataEntityForDbHelper>())), Times.Exactly(1));
        }

        #endregion

        #region Operation(TableName)

        [TestMethod]
        public void TestDbHelperForBatchQueryViaTableName()
        {
            // Prepare
            var dbHelper = new Mock<IDbHelper>();
            var connection = new CustomDbConnectionForDbHelper();

            // Setup
            dbHelper.Setup(builder =>
                builder.GetFields(
                    It.Is<IDbConnection>(s => s == connection),
                    It.Is<string>(s => s == ClassMappedNameCache.Get<DataEntityForDbHelper>()))
                ).Returns(GetDbFields());

            // Act
            CommandTextCache.Flush();
            DbFieldCache.Flush();
            StatementBuilderMapper.Add(typeof(CustomDbConnectionForDbHelper), new SqlStatementBuilder(), true);
            DbHelperMapper.Add(typeof(CustomDbConnectionForDbHelper), dbHelper.Object, true);

            // Act
            connection.BatchQuery(ClassMappedNameCache.Get<DataEntityForDbHelper>(),
                0,
                10,
                OrderField.Ascending<DataEntityForDbHelper>(e => e.Id).AsEnumerable(),
                new { Id = 1 });

            // Assert
            dbHelper.Verify(builder =>
                builder.GetFields(
                    It.Is<IDbConnection>(s => s == connection),
                    It.Is<string>(s => s == ClassMappedNameCache.Get<DataEntityForDbHelper>())), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbHelperForInsertViaTableName()
        {
            // Prepare
            var dbHelper = new Mock<IDbHelper>();
            var connection = new CustomDbConnectionForDbHelper();

            // Setup
            dbHelper.Setup(builder =>
                builder.GetFields(
                    It.Is<IDbConnection>(s => s == connection),
                    It.Is<string>(s => s == ClassMappedNameCache.Get<DataEntityForDbHelper>()))
                ).Returns(GetDbFields());

            // Act
            CommandTextCache.Flush();
            DbFieldCache.Flush();
            StatementBuilderMapper.Add(typeof(CustomDbConnectionForDbHelper), new SqlStatementBuilder(), true);
            DbHelperMapper.Add(typeof(CustomDbConnectionForDbHelper), dbHelper.Object, true);

            // Act
            connection.Insert(ClassMappedNameCache.Get<DataEntityForDbHelper>(),
                new { Id = 1, Name = "Name" });

            // Assert
            dbHelper.Verify(builder =>
                builder.GetFields(
                    It.Is<IDbConnection>(s => s == connection),
                    It.Is<string>(s => s == ClassMappedNameCache.Get<DataEntityForDbHelper>())), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbHelperForInsertAllViaTableName()
        {
            // Prepare
            var dbHelper = new Mock<IDbHelper>();
            var connection = new CustomDbConnectionForDbHelper();

            // Setup
            dbHelper.Setup(builder =>
                builder.GetFields(
                    It.Is<IDbConnection>(s => s == connection),
                    It.Is<string>(s => s == ClassMappedNameCache.Get<DataEntityForDbHelper>()))
                ).Returns(GetDbFields());

            // Act
            CommandTextCache.Flush();
            DbFieldCache.Flush();
            StatementBuilderMapper.Add(typeof(CustomDbConnectionForDbHelper), new SqlStatementBuilder(), true);
            DbHelperMapper.Add(typeof(CustomDbConnectionForDbHelper), dbHelper.Object, true);

            // Act
            connection.InsertAll(ClassMappedNameCache.Get<DataEntityForDbHelper>(),
                new[] { new { Id = 1, Name = "Name" } });

            // Assert
            dbHelper.Verify(builder =>
                builder.GetFields(
                    It.Is<IDbConnection>(s => s == connection),
                    It.Is<string>(s => s == ClassMappedNameCache.Get<DataEntityForDbHelper>())), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbHelperForMergeViaTableName()
        {
            // Prepare
            var dbHelper = new Mock<IDbHelper>();
            var connection = new CustomDbConnectionForDbHelper();

            // Setup
            dbHelper.Setup(builder =>
                builder.GetFields(
                    It.Is<IDbConnection>(s => s == connection),
                    It.Is<string>(s => s == ClassMappedNameCache.Get<DataEntityForDbHelper>()))
                ).Returns(GetDbFields());

            // Act
            CommandTextCache.Flush();
            DbFieldCache.Flush();
            StatementBuilderMapper.Add(typeof(CustomDbConnectionForDbHelper), new SqlStatementBuilder(), true);
            DbHelperMapper.Add(typeof(CustomDbConnectionForDbHelper), dbHelper.Object, true);

            // Act
            connection.Merge(ClassMappedNameCache.Get<DataEntityForDbHelper>(),
                new { Id = 1, Name = "Name" });

            // Assert
            dbHelper.Verify(builder =>
                builder.GetFields(
                    It.Is<IDbConnection>(s => s == connection),
                    It.Is<string>(s => s == ClassMappedNameCache.Get<DataEntityForDbHelper>())), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbHelperForMergeAllViaTableName()
        {
            // Prepare
            var dbHelper = new Mock<IDbHelper>();
            var connection = new CustomDbConnectionForDbHelper();

            // Setup
            dbHelper.Setup(builder =>
                builder.GetFields(
                    It.Is<IDbConnection>(s => s == connection),
                    It.Is<string>(s => s == ClassMappedNameCache.Get<DataEntityForDbHelper>()))
                ).Returns(GetDbFields());

            // Act
            CommandTextCache.Flush();
            DbFieldCache.Flush();
            StatementBuilderMapper.Add(typeof(CustomDbConnectionForDbHelper), new SqlStatementBuilder(), true);
            DbHelperMapper.Add(typeof(CustomDbConnectionForDbHelper), dbHelper.Object, true);

            // Act
            connection.MergeAll(ClassMappedNameCache.Get<DataEntityForDbHelper>(),
                new[] { new DataEntityForDbHelper { Id = 1, Name = "Name" } });

            // Assert
            dbHelper.Verify(builder =>
                builder.GetFields(
                    It.Is<IDbConnection>(s => s == connection),
                    It.Is<string>(s => s == ClassMappedNameCache.Get<DataEntityForDbHelper>())), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbHelperForQueryViaTableName()
        {
            // Prepare
            var dbHelper = new Mock<IDbHelper>();
            var connection = new CustomDbConnectionForDbHelper();

            // Setup
            dbHelper.Setup(builder =>
                builder.GetFields(
                    It.Is<IDbConnection>(s => s == connection),
                    It.Is<string>(s => s == ClassMappedNameCache.Get<DataEntityForDbHelper>()))
                ).Returns(GetDbFields());

            // Act
            CommandTextCache.Flush();
            DbFieldCache.Flush();
            StatementBuilderMapper.Add(typeof(CustomDbConnectionForDbHelper), new SqlStatementBuilder(), true);
            DbHelperMapper.Add(typeof(CustomDbConnectionForDbHelper), dbHelper.Object, true);

            // Act
            connection.Query(ClassMappedNameCache.Get<DataEntityForDbHelper>(),
                (object)null);

            // Assert
            dbHelper.Verify(builder =>
                builder.GetFields(
                    It.Is<IDbConnection>(s => s == connection),
                    It.Is<string>(s => s == ClassMappedNameCache.Get<DataEntityForDbHelper>())), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbHelperForQueryAllViaTableName()
        {
            // Prepare
            var dbHelper = new Mock<IDbHelper>();
            var connection = new CustomDbConnectionForDbHelper();

            // Setup
            dbHelper.Setup(builder =>
                builder.GetFields(
                    It.Is<IDbConnection>(s => s == connection),
                    It.Is<string>(s => s == ClassMappedNameCache.Get<DataEntityForDbHelper>()))
                ).Returns(GetDbFields());

            // Act
            CommandTextCache.Flush();
            DbFieldCache.Flush();
            StatementBuilderMapper.Add(typeof(CustomDbConnectionForDbHelper), new SqlStatementBuilder(), true);
            DbHelperMapper.Add(typeof(CustomDbConnectionForDbHelper), dbHelper.Object, true);

            // Act
            connection.QueryAll(ClassMappedNameCache.Get<DataEntityForDbHelper>());

            // Assert
            dbHelper.Verify(builder =>
                builder.GetFields(
                    It.Is<IDbConnection>(s => s == connection),
                    It.Is<string>(s => s == ClassMappedNameCache.Get<DataEntityForDbHelper>())), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbHelperForUpdateViaTableName()
        {
            // Prepare
            var dbHelper = new Mock<IDbHelper>();
            var connection = new CustomDbConnectionForDbHelper();

            // Setup
            dbHelper.Setup(builder =>
                builder.GetFields(
                    It.Is<IDbConnection>(s => s == connection),
                    It.Is<string>(s => s == ClassMappedNameCache.Get<DataEntityForDbHelper>()))
                ).Returns(GetDbFields());

            // Act
            CommandTextCache.Flush();
            DbFieldCache.Flush();
            StatementBuilderMapper.Add(typeof(CustomDbConnectionForDbHelper), new SqlStatementBuilder(), true);
            DbHelperMapper.Add(typeof(CustomDbConnectionForDbHelper), dbHelper.Object, true);

            // Act
            connection.Update(ClassMappedNameCache.Get<DataEntityForDbHelper>(),
                new DataEntityForDbHelper { Id = 1, Name = "Name" });

            // Assert
            dbHelper.Verify(builder =>
                builder.GetFields(
                    It.Is<IDbConnection>(s => s == connection),
                    It.Is<string>(s => s == ClassMappedNameCache.Get<DataEntityForDbHelper>())), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbHelperForUpdateAllViaTableName()
        {
            // Prepare
            var dbHelper = new Mock<IDbHelper>();
            var connection = new CustomDbConnectionForDbHelper();

            // Setup
            dbHelper.Setup(builder =>
                builder.GetFields(
                    It.Is<IDbConnection>(s => s == connection),
                    It.Is<string>(s => s == ClassMappedNameCache.Get<DataEntityForDbHelper>()))
                ).Returns(GetDbFields());

            // Act
            CommandTextCache.Flush();
            DbFieldCache.Flush();
            StatementBuilderMapper.Add(typeof(CustomDbConnectionForDbHelper), new SqlStatementBuilder(), true);
            DbHelperMapper.Add(typeof(CustomDbConnectionForDbHelper), dbHelper.Object, true);

            // Act
            connection.UpdateAll(ClassMappedNameCache.Get<DataEntityForDbHelper>(),
                new[] { new DataEntityForDbHelper { Id = 1, Name = "Name" } });

            // Assert
            dbHelper.Verify(builder =>
                builder.GetFields(
                    It.Is<IDbConnection>(s => s == connection),
                    It.Is<string>(s => s == ClassMappedNameCache.Get<DataEntityForDbHelper>())), Times.Exactly(1));
        }

        #endregion
    }
}
