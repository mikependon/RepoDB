using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Enumerations;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Interfaces
{
    [TestClass]
    public class IDbValidatorForDbConnection
    {
        private static Mock<IDbValidator> validator = new Mock<IDbValidator>();

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            DbSettingMapper.Add(typeof(DbValidatorDbConnection), new CustomDbSetting(), true);
            DbOperationMapper.Add(typeof(DbValidatorDbConnection), new CustomDbOperation(), true);
            DbHelperMapper.Add(typeof(DbValidatorDbConnection), new CustomDbHelper(), true);
            StatementBuilderMapper.Add(typeof(DbValidatorDbConnection), new CustomStatementBuilder(), true);
        }

        #region SubClasses

        private class DbValidatorDbConnection : CustomDbConnection { }

        private class DbValidatorEntity
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        #endregion

        #region Sync

        #region Average

        [TestMethod]
        public void TestDbConnectionDbValidatorForAverageViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.Average<DbValidatorEntity>(field: e => e.Id,
                where: (object)null);

            // Assert
            validator.Verify(t => t.ValidateAverage(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForAverageViaTableName()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.Average(ClassMappedNameCache.Get<DbValidatorEntity>(),
                field: new Field("Id"),
                where: (object)null);

            // Assert
            validator.Verify(t => t.ValidateAverage(), Times.Exactly(1));
        }

        #endregion

        #region AverageAll

        [TestMethod]
        public void TestDbConnectionDbValidatorForAverageAllViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.AverageAll<DbValidatorEntity>(field: e => e.Id);

            // Assert
            validator.Verify(t => t.ValidateAverageAll(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForAverageViaTableNameAll()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.AverageAll(ClassMappedNameCache.Get<DbValidatorEntity>(),
                field: new Field("Id"));

            // Assert
            validator.Verify(t => t.ValidateAverageAll(), Times.Exactly(1));
        }

        #endregion

        #region BatchQuery

        [TestMethod]
        public void TestDbConnectionDbValidatorForBatchQueryViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.BatchQuery<DbValidatorEntity>(page: 0,
                rowsPerBatch: 10,
                where: (object)null,
                orderBy: OrderField.Parse(new { Id = Order.Ascending }));

            // Assert
            validator.Verify(t => t.ValidateBatchQuery(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForBatchQueryViaTableName()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.BatchQuery(ClassMappedNameCache.Get<DbValidatorEntity>(),
                page: 0,
                rowsPerBatch: 10,
                where: (object)null,
                orderBy: OrderField.Parse(new { Id = Order.Ascending }));

            // Assert
            validator.Verify(t => t.ValidateBatchQuery(), Times.Exactly(1));
        }

        #endregion

        #region BulkInsert

        [TestMethod]
        public void TestDbConnectionDbValidatorForBulkInsertViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var entities = new[] { new DbValidatorEntity() };
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.BulkInsert<DbValidatorEntity>(entities);

            // Assert
            validator.Verify(t => t.ValidateBulkInsert(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForBulkInsertViaDataEntityAsDataReader()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var entities = new[] { new DbValidatorEntity() };
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            using (var reader = new DataEntityDataReader<DbValidatorEntity>(entities))
            {
                connection.BulkInsert<DbValidatorEntity>(reader);
            }

            // Assert
            validator.Verify(t => t.ValidateBulkInsert(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForBulkInsertViaTableName()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var entities = new[] { new DbValidatorEntity() };
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.BulkInsert(ClassMappedNameCache.Get<DbValidatorEntity>(),
                entities);

            // Assert
            validator.Verify(t => t.ValidateBulkInsert(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForBulkInsertViaTableNameAsDataReader()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var entities = new[] { new DbValidatorEntity() };
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            using (var reader = new DataEntityDataReader<DbValidatorEntity>(entities))
            {
                connection.BulkInsert(ClassMappedNameCache.Get<DbValidatorEntity>(),
                    reader);
            }

            // Assert
            validator.Verify(t => t.ValidateBulkInsert(), Times.Exactly(1));
        }

        #endregion

        #region Count

        [TestMethod]
        public void TestDbConnectionDbValidatorForCountViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.Count<DbValidatorEntity>(where: e => e.Id == 1);

            // Assert
            validator.Verify(t => t.ValidateCount(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForCountViaTableName()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.Count(ClassMappedNameCache.Get<DbValidatorEntity>(),
                where: new { Id = 1 });

            // Assert
            validator.Verify(t => t.ValidateCount(), Times.Exactly(1));
        }

        #endregion

        #region CountAll

        [TestMethod]
        public void TestDbConnectionDbValidatorForCountAllViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.CountAll<DbValidatorEntity>();

            // Assert
            validator.Verify(t => t.ValidateCountAll(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForCountViaTableNameAll()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.CountAll(ClassMappedNameCache.Get<DbValidatorEntity>());

            // Assert
            validator.Verify(t => t.ValidateCountAll(), Times.Exactly(1));
        }

        #endregion

        #region Delete

        [TestMethod]
        public void TestDbConnectionDbValidatorForDeleteViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.Delete<DbValidatorEntity>(where: e => e.Id == 1);

            // Assert
            validator.Verify(t => t.ValidateDelete(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForDeleteViaTableName()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.Delete(ClassMappedNameCache.Get<DbValidatorEntity>(),
                whereOrPrimaryKey: new { Id = 1 });

            // Assert
            validator.Verify(t => t.ValidateDelete(), Times.Exactly(1));
        }

        #endregion

        #region DeleteAll

        [TestMethod]
        public void TestDbConnectionDbValidatorForDeleteAllViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.DeleteAll<DbValidatorEntity>();

            // Assert
            validator.Verify(t => t.ValidateDeleteAll(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForDeleteViaTableNameAll()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.DeleteAll(ClassMappedNameCache.Get<DbValidatorEntity>());

            // Assert
            validator.Verify(t => t.ValidateDeleteAll(), Times.Exactly(1));
        }

        #endregion

        #region Exists

        [TestMethod]
        public void TestDbConnectionDbValidatorForExistsViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.Exists<DbValidatorEntity>(where: e => e.Id == 1);

            // Assert
            validator.Verify(t => t.ValidateExists(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForExistsViaTableName()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.Exists(ClassMappedNameCache.Get<DbValidatorEntity>(),
                whereOrPrimaryKey: new { Id = 1 });

            // Assert
            validator.Verify(t => t.ValidateExists(), Times.Exactly(1));
        }

        #endregion

        #region Insert

        [TestMethod]
        public void TestDbConnectionDbValidatorForInsertViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.Insert<DbValidatorEntity>(new DbValidatorEntity());

            // Assert
            validator.Verify(t => t.ValidateInsert(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForInsertViaTableName()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.Insert(ClassMappedNameCache.Get<DbValidatorEntity>(),
                new DbValidatorEntity());

            // Assert
            validator.Verify(t => t.ValidateInsert(), Times.Exactly(1));
        }

        #endregion

        #region InsertAll

        [TestMethod]
        public void TestDbConnectionDbValidatorForInsertAllViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.InsertAll<DbValidatorEntity>(new[] { new DbValidatorEntity() });

            // Assert
            validator.Verify(t => t.ValidateInsertAll(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForInsertViaTableNameAll()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.InsertAll(ClassMappedNameCache.Get<DbValidatorEntity>(),
                new[] { new DbValidatorEntity() });

            // Assert
            validator.Verify(t => t.ValidateInsertAll(), Times.Exactly(1));
        }

        #endregion

        #region Max

        [TestMethod]
        public void TestDbConnectionDbValidatorForMaxViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.Max<DbValidatorEntity>(field: e => e.Id,
                where: (object)null);

            // Assert
            validator.Verify(t => t.ValidateMax(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForMaxViaTableName()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.Max(ClassMappedNameCache.Get<DbValidatorEntity>(),
                field: new Field("Id"),
                where: (object)null);

            // Assert
            validator.Verify(t => t.ValidateMax(), Times.Exactly(1));
        }

        #endregion

        #region MaxAll

        [TestMethod]
        public void TestDbConnectionDbValidatorForMaxAllViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.MaxAll<DbValidatorEntity>(field: e => e.Id);

            // Assert
            validator.Verify(t => t.ValidateMaxAll(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForMaxViaTableNameAll()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.MaxAll(ClassMappedNameCache.Get<DbValidatorEntity>(),
                field: new Field("Id"));

            // Assert
            validator.Verify(t => t.ValidateMaxAll(), Times.Exactly(1));
        }

        #endregion

        #region Merge

        [TestMethod]
        public void TestDbConnectionDbValidatorForMergeViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.Merge<DbValidatorEntity>(new DbValidatorEntity());

            // Assert
            validator.Verify(t => t.ValidateMerge(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForMergeViaTableName()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.Merge(ClassMappedNameCache.Get<DbValidatorEntity>(),
                new DbValidatorEntity());

            // Assert
            validator.Verify(t => t.ValidateMerge(), Times.Exactly(1));
        }

        #endregion

        #region MergeAll

        [TestMethod]
        public void TestDbConnectionDbValidatorForMergeAllViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.MergeAll<DbValidatorEntity>(new[] { new DbValidatorEntity() });

            // Assert
            validator.Verify(t => t.ValidateMergeAll(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForMergeViaTableNameAll()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.MergeAll(ClassMappedNameCache.Get<DbValidatorEntity>(),
                new[] { new DbValidatorEntity() });

            // Assert
            validator.Verify(t => t.ValidateMergeAll(), Times.Exactly(1));
        }

        #endregion

        #region Min

        [TestMethod]
        public void TestDbConnectionDbValidatorForMinViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.Min<DbValidatorEntity>(field: e => e.Id,
                where: (object)null);

            // Assert
            validator.Verify(t => t.ValidateMin(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForMinViaTableName()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.Min(ClassMappedNameCache.Get<DbValidatorEntity>(),
                field: new Field("Id"),
                where: (object)null);

            // Assert
            validator.Verify(t => t.ValidateMin(), Times.Exactly(1));
        }

        #endregion

        #region MinAll

        [TestMethod]
        public void TestDbConnectionDbValidatorForMinAllViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.MinAll<DbValidatorEntity>(field: e => e.Id);

            // Assert
            validator.Verify(t => t.ValidateMinAll(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForMinViaTableNameAll()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.MinAll(ClassMappedNameCache.Get<DbValidatorEntity>(),
                field: new Field("Id"));

            // Assert
            validator.Verify(t => t.ValidateMinAll(), Times.Exactly(1));
        }

        #endregion

        #region Query

        [TestMethod]
        public void TestDbConnectionDbValidatorForQueryViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.Query<DbValidatorEntity>(where: e => e.Id == 1);

            // Assert
            validator.Verify(t => t.ValidateQuery(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForQueryViaTableName()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.Query(ClassMappedNameCache.Get<DbValidatorEntity>(),
                whereOrPrimaryKey: new { Id = 1 });

            // Assert
            validator.Verify(t => t.ValidateQuery(), Times.Exactly(1));
        }

        #endregion

        #region QueryAll

        [TestMethod]
        public void TestDbConnectionDbValidatorForQueryAllViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.QueryAll<DbValidatorEntity>();

            // Assert
            validator.Verify(t => t.ValidateQueryAll(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForQueryViaTableNameAll()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.QueryAll(ClassMappedNameCache.Get<DbValidatorEntity>());

            // Assert
            validator.Verify(t => t.ValidateQueryAll(), Times.Exactly(1));
        }

        #endregion

        #region QueryMultiple

        [TestMethod]
        public void TestDbConnectionDbValidatorForQueryMultipleT2()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.QueryMultiple<DbValidatorEntity, DbValidatorEntity>(e => e.Id == 1,
                e => e.Id == 2);

            // Assert
            validator.Verify(t => t.ValidateQueryMultiple(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForQueryMultipleT3()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.QueryMultiple<DbValidatorEntity, DbValidatorEntity, DbValidatorEntity>(e => e.Id == 1,
                e => e.Id == 2,
                e => e.Id == 2);

            // Assert
            validator.Verify(t => t.ValidateQueryMultiple(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForQueryMultipleT4()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.QueryMultiple<DbValidatorEntity, DbValidatorEntity, DbValidatorEntity,
                DbValidatorEntity>(e => e.Id == 1,
                e => e.Id == 2,
                e => e.Id == 2,
                e => e.Id == 2);

            // Assert
            validator.Verify(t => t.ValidateQueryMultiple(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForQueryMultipleT5()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.QueryMultiple<DbValidatorEntity, DbValidatorEntity, DbValidatorEntity,
                DbValidatorEntity, DbValidatorEntity>(e => e.Id == 1,
                e => e.Id == 2,
                e => e.Id == 2,
                e => e.Id == 2,
                e => e.Id == 2);

            // Assert
            validator.Verify(t => t.ValidateQueryMultiple(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForQueryMultipleT6()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.QueryMultiple<DbValidatorEntity, DbValidatorEntity, DbValidatorEntity,
                DbValidatorEntity, DbValidatorEntity, DbValidatorEntity>(e => e.Id == 1,
                e => e.Id == 2,
                e => e.Id == 2,
                e => e.Id == 2,
                e => e.Id == 2,
                e => e.Id == 2);

            // Assert
            validator.Verify(t => t.ValidateQueryMultiple(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForQueryMultipleT7()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.QueryMultiple<DbValidatorEntity, DbValidatorEntity, DbValidatorEntity,
                DbValidatorEntity, DbValidatorEntity, DbValidatorEntity, DbValidatorEntity>(e => e.Id == 1,
                e => e.Id == 2,
                e => e.Id == 2,
                e => e.Id == 2,
                e => e.Id == 2,
                e => e.Id == 2,
                e => e.Id == 2);

            // Assert
            validator.Verify(t => t.ValidateQueryMultiple(), Times.Exactly(1));
        }

        #endregion

        #region Sum

        [TestMethod]
        public void TestDbConnectionDbValidatorForSumViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.Sum<DbValidatorEntity>(field: e => e.Id,
                where: (object)null);

            // Assert
            validator.Verify(t => t.ValidateSum(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForSumViaTableName()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.Sum(ClassMappedNameCache.Get<DbValidatorEntity>(),
                field: new Field("Id"),
                where: (object)null);

            // Assert
            validator.Verify(t => t.ValidateSum(), Times.Exactly(1));
        }

        #endregion

        #region SumAll

        [TestMethod]
        public void TestDbConnectionDbValidatorForSumAllViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.SumAll<DbValidatorEntity>(field: e => e.Id);

            // Assert
            validator.Verify(t => t.ValidateSumAll(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForSumViaTableNameAll()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.SumAll(ClassMappedNameCache.Get<DbValidatorEntity>(),
                field: new Field("Id"));

            // Assert
            validator.Verify(t => t.ValidateSumAll(), Times.Exactly(1));
        }

        #endregion

        #region Truncate

        [TestMethod]
        public void TestDbConnectionDbValidatorForTruncateViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.Truncate<DbValidatorEntity>();

            // Assert
            validator.Verify(t => t.ValidateTruncate(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForTruncateViaTableName()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.Truncate(ClassMappedNameCache.Get<DbValidatorEntity>());

            // Assert
            validator.Verify(t => t.ValidateTruncate(), Times.Exactly(1));
        }

        #endregion

        #region Update

        [TestMethod]
        public void TestDbConnectionDbValidatorForUpdateViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.Update<DbValidatorEntity>(new DbValidatorEntity(),
                where: e => e.Id == 1);

            // Assert
            validator.Verify(t => t.ValidateUpdate(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForUpdateViaTableName()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.Update(ClassMappedNameCache.Get<DbValidatorEntity>(),
                new DbValidatorEntity());

            // Assert
            validator.Verify(t => t.ValidateUpdate(), Times.Exactly(1));
        }

        #endregion

        #region UpdateAll

        [TestMethod]
        public void TestDbConnectionDbValidatorForUpdateAllViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.UpdateAll<DbValidatorEntity>(new[] { new DbValidatorEntity() });

            // Assert
            validator.Verify(t => t.ValidateUpdateAll(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForUpdateViaTableNameAll()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.UpdateAll(ClassMappedNameCache.Get<DbValidatorEntity>(),
                new[] { new DbValidatorEntity() });

            // Assert
            validator.Verify(t => t.ValidateUpdateAll(), Times.Exactly(1));
        }

        #endregion

        #endregion

        #region Async

        #region AverageAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForAverageAsyncViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.AverageAsync<DbValidatorEntity>(field: e => e.Id,
                where: (object)null).Wait();

            // Assert
            validator.Verify(t => t.ValidateAverageAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForAverageAsyncViaTableName()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.AverageAsync(ClassMappedNameCache.Get<DbValidatorEntity>(),
                field: new Field("Id"),
                where: (object)null).Wait();

            // Assert
            validator.Verify(t => t.ValidateAverageAsync(), Times.Exactly(1));
        }

        #endregion

        #region AverageAllAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForAverageAllAsyncViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.AverageAllAsync<DbValidatorEntity>(field: e => e.Id).Wait();

            // Assert
            validator.Verify(t => t.ValidateAverageAllAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForAverageAsyncViaTableNameAll()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.AverageAllAsync(ClassMappedNameCache.Get<DbValidatorEntity>(),
                field: new Field("Id")).Wait();

            // Assert
            validator.Verify(t => t.ValidateAverageAllAsync(), Times.Exactly(1));
        }

        #endregion

        #region BatchQueryAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForBatchQueryAsyncViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.BatchQueryAsync<DbValidatorEntity>(page: 0,
                rowsPerBatch: 10,
                where: (object)null,
                orderBy: OrderField.Parse(new { Id = Order.Ascending })).Wait();

            // Assert
            validator.Verify(t => t.ValidateBatchQueryAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForBatchQueryAsyncViaTableName()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.BatchQueryAsync(ClassMappedNameCache.Get<DbValidatorEntity>(),
                page: 0,
                rowsPerBatch: 10,
                where: (object)null,
                orderBy: OrderField.Parse(new { Id = Order.Ascending })).Wait();

            // Assert
            validator.Verify(t => t.ValidateBatchQueryAsync(), Times.Exactly(1));
        }

        #endregion

        #region BulkInsertAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForBulkInsertAsyncViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var entities = new[] { new DbValidatorEntity() };
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.BulkInsertAsync<DbValidatorEntity>(entities).Wait();

            // Assert
            validator.Verify(t => t.ValidateBulkInsertAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForBulkInsertAsyncViaDataEntityAsDataReader()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var entities = new[] { new DbValidatorEntity() };
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            using (var reader = new DataEntityDataReader<DbValidatorEntity>(entities))
            {
                connection.BulkInsertAsync<DbValidatorEntity>(reader).Wait();
            }

            // Assert
            validator.Verify(t => t.ValidateBulkInsertAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForBulkInsertAsyncViaTableName()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var entities = new[] { new DbValidatorEntity() };
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.BulkInsertAsync(ClassMappedNameCache.Get<DbValidatorEntity>(),
                entities).Wait();

            // Assert
            validator.Verify(t => t.ValidateBulkInsertAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForBulkInsertAsyncViaTableNameAsDataReader()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var entities = new[] { new DbValidatorEntity() };
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            using (var reader = new DataEntityDataReader<DbValidatorEntity>(entities))
            {
                connection.BulkInsertAsync(ClassMappedNameCache.Get<DbValidatorEntity>(),
                    reader).Wait();
            }

            // Assert
            validator.Verify(t => t.ValidateBulkInsertAsync(), Times.Exactly(1));
        }

        #endregion

        #region CountAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForCountAsyncViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.CountAsync<DbValidatorEntity>(where: e => e.Id == 1).Wait();

            // Assert
            validator.Verify(t => t.ValidateCountAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForCountAsyncViaTableName()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.CountAsync(ClassMappedNameCache.Get<DbValidatorEntity>(),
                where: new { Id = 1 }).Wait();

            // Assert
            validator.Verify(t => t.ValidateCountAsync(), Times.Exactly(1));
        }

        #endregion

        #region CountAllAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForCountAllAsyncViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.CountAllAsync<DbValidatorEntity>().Wait();

            // Assert
            validator.Verify(t => t.ValidateCountAllAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForCountAsyncViaTableNameAll()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.CountAllAsync(ClassMappedNameCache.Get<DbValidatorEntity>()).Wait();

            // Assert
            validator.Verify(t => t.ValidateCountAllAsync(), Times.Exactly(1));
        }

        #endregion

        #region DeleteAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForDeleteAsyncViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.DeleteAsync<DbValidatorEntity>(where: e => e.Id == 1).Wait();

            // Assert
            validator.Verify(t => t.ValidateDeleteAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForDeleteAsyncViaTableName()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.DeleteAsync(ClassMappedNameCache.Get<DbValidatorEntity>(),
                whereOrPrimaryKey: new { Id = 1 }).Wait();

            // Assert
            validator.Verify(t => t.ValidateDeleteAsync(), Times.Exactly(1));
        }

        #endregion

        #region DeleteAllAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForDeleteAllAsyncViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.DeleteAllAsync<DbValidatorEntity>().Wait();

            // Assert
            validator.Verify(t => t.ValidateDeleteAllAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForDeleteAsyncViaTableNameAll()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.DeleteAllAsync(ClassMappedNameCache.Get<DbValidatorEntity>()).Wait();

            // Assert
            validator.Verify(t => t.ValidateDeleteAllAsync(), Times.Exactly(1));
        }

        #endregion

        #region ExistsAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForExistsAsyncViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.ExistsAsync<DbValidatorEntity>(where: e => e.Id == 1).Wait();

            // Assert
            validator.Verify(t => t.ValidateExistsAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForExistsAsyncViaTableName()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.ExistsAsync(ClassMappedNameCache.Get<DbValidatorEntity>(),
                whereOrPrimaryKey: new { Id = 1 }).Wait();

            // Assert
            validator.Verify(t => t.ValidateExistsAsync(), Times.Exactly(1));
        }

        #endregion

        #region InsertAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForInsertAsyncViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.InsertAsync<DbValidatorEntity>(new DbValidatorEntity()).Wait();

            // Assert
            validator.Verify(t => t.ValidateInsertAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForInsertAsyncViaTableName()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.InsertAsync(ClassMappedNameCache.Get<DbValidatorEntity>(),
                new DbValidatorEntity()).Wait();

            // Assert
            validator.Verify(t => t.ValidateInsertAsync(), Times.Exactly(1));
        }

        #endregion

        #region InsertAllAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForInsertAllAsyncViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.InsertAllAsync<DbValidatorEntity>(new[] { new DbValidatorEntity() }).Wait();

            // Assert
            validator.Verify(t => t.ValidateInsertAllAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForInsertAsyncViaTableNameAll()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.InsertAllAsync(ClassMappedNameCache.Get<DbValidatorEntity>(),
                new[] { new DbValidatorEntity() }).Wait();

            // Assert
            validator.Verify(t => t.ValidateInsertAllAsync(), Times.Exactly(1));
        }

        #endregion

        #region MaxAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForMaxAsyncViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.MaxAsync<DbValidatorEntity>(field: e => e.Id,
                where: (object)null).Wait();

            // Assert
            validator.Verify(t => t.ValidateMaxAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForMaxAsyncViaTableName()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.MaxAsync(ClassMappedNameCache.Get<DbValidatorEntity>(),
                field: new Field("Id"),
                where: (object)null).Wait();

            // Assert
            validator.Verify(t => t.ValidateMaxAsync(), Times.Exactly(1));
        }

        #endregion

        #region MaxAllAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForMaxAllAsyncViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.MaxAllAsync<DbValidatorEntity>(field: e => e.Id).Wait();

            // Assert
            validator.Verify(t => t.ValidateMaxAllAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForMaxAsyncViaTableNameAll()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.MaxAllAsync(ClassMappedNameCache.Get<DbValidatorEntity>(),
                field: new Field("Id")).Wait();

            // Assert
            validator.Verify(t => t.ValidateMaxAllAsync(), Times.Exactly(1));
        }

        #endregion

        #region MergeAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForMergeAsyncViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.MergeAsync<DbValidatorEntity>(new DbValidatorEntity()).Wait();

            // Assert
            validator.Verify(t => t.ValidateMergeAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForMergeAsyncViaTableName()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.MergeAsync(ClassMappedNameCache.Get<DbValidatorEntity>(),
                new DbValidatorEntity()).Wait();

            // Assert
            validator.Verify(t => t.ValidateMergeAsync(), Times.Exactly(1));
        }

        #endregion

        #region MergeAllAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForMergeAllAsyncViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.MergeAllAsync<DbValidatorEntity>(new[] { new DbValidatorEntity() }).Wait();

            // Assert
            validator.Verify(t => t.ValidateMergeAllAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForMergeAsyncViaTableNameAll()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.MergeAllAsync(ClassMappedNameCache.Get<DbValidatorEntity>(),
                new[] { new DbValidatorEntity() }).Wait();

            // Assert
            validator.Verify(t => t.ValidateMergeAllAsync(), Times.Exactly(1));
        }

        #endregion

        #region MinAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForMinAsyncViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.MinAsync<DbValidatorEntity>(field: e => e.Id,
                where: (object)null).Wait();

            // Assert
            validator.Verify(t => t.ValidateMinAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForMinAsyncViaTableName()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.MinAsync(ClassMappedNameCache.Get<DbValidatorEntity>(),
                field: new Field("Id"),
                where: (object)null).Wait();

            // Assert
            validator.Verify(t => t.ValidateMinAsync(), Times.Exactly(1));
        }

        #endregion

        #region MinAllAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForMinAllAsyncViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.MinAllAsync<DbValidatorEntity>(field: e => e.Id).Wait();

            // Assert
            validator.Verify(t => t.ValidateMinAllAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForMinAsyncViaTableNameAll()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.MinAllAsync(ClassMappedNameCache.Get<DbValidatorEntity>(),
                field: new Field("Id")).Wait();

            // Assert
            validator.Verify(t => t.ValidateMinAllAsync(), Times.Exactly(1));
        }

        #endregion

        #region QueryAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForQueryAsyncViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.QueryAsync<DbValidatorEntity>(where: e => e.Id == 1).Wait();

            // Assert
            validator.Verify(t => t.ValidateQueryAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForQueryAsyncViaTableName()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.QueryAsync(ClassMappedNameCache.Get<DbValidatorEntity>(),
                whereOrPrimaryKey: new { Id = 1 }).Wait();

            // Assert
            validator.Verify(t => t.ValidateQueryAsync(), Times.Exactly(1));
        }

        #endregion

        #region QueryAllAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForQueryAllAsyncViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.QueryAllAsync<DbValidatorEntity>().Wait();

            // Assert
            validator.Verify(t => t.ValidateQueryAllAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForQueryAsyncViaTableNameAll()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.QueryAllAsync(ClassMappedNameCache.Get<DbValidatorEntity>()).Wait();

            // Assert
            validator.Verify(t => t.ValidateQueryAllAsync(), Times.Exactly(1));
        }

        #endregion

        #region QueryMultipleAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForQueryMultipleAsyncT2()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.QueryMultipleAsync<DbValidatorEntity, DbValidatorEntity>(e => e.Id == 1,
                e => e.Id == 2).Wait();

            // Assert
            validator.Verify(t => t.ValidateQueryMultipleAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForQueryMultipleAsyncT3()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.QueryMultipleAsync<DbValidatorEntity, DbValidatorEntity, DbValidatorEntity>(e => e.Id == 1,
                e => e.Id == 2,
                e => e.Id == 2).Wait();

            // Assert
            validator.Verify(t => t.ValidateQueryMultipleAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForQueryMultipleAsyncT4()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.QueryMultipleAsync<DbValidatorEntity, DbValidatorEntity, DbValidatorEntity,
                DbValidatorEntity>(e => e.Id == 1,
                e => e.Id == 2,
                e => e.Id == 2,
                e => e.Id == 2).Wait();

            // Assert
            validator.Verify(t => t.ValidateQueryMultipleAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForQueryMultipleAsyncT5()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.QueryMultipleAsync<DbValidatorEntity, DbValidatorEntity, DbValidatorEntity,
                DbValidatorEntity, DbValidatorEntity>(e => e.Id == 1,
                e => e.Id == 2,
                e => e.Id == 2,
                e => e.Id == 2,
                e => e.Id == 2).Wait();

            // Assert
            validator.Verify(t => t.ValidateQueryMultipleAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForQueryMultipleAsyncT6()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.QueryMultipleAsync<DbValidatorEntity, DbValidatorEntity, DbValidatorEntity,
                DbValidatorEntity, DbValidatorEntity, DbValidatorEntity>(e => e.Id == 1,
                e => e.Id == 2,
                e => e.Id == 2,
                e => e.Id == 2,
                e => e.Id == 2,
                e => e.Id == 2).Wait();

            // Assert
            validator.Verify(t => t.ValidateQueryMultipleAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForQueryMultipleAsyncT7()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.QueryMultipleAsync<DbValidatorEntity, DbValidatorEntity, DbValidatorEntity,
                DbValidatorEntity, DbValidatorEntity, DbValidatorEntity, DbValidatorEntity>(e => e.Id == 1,
                e => e.Id == 2,
                e => e.Id == 2,
                e => e.Id == 2,
                e => e.Id == 2,
                e => e.Id == 2,
                e => e.Id == 2).Wait();

            // Assert
            validator.Verify(t => t.ValidateQueryMultipleAsync(), Times.Exactly(1));
        }

        #endregion

        #region SumAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForSumAsyncViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.SumAsync<DbValidatorEntity>(field: e => e.Id,
                where: (object)null).Wait();

            // Assert
            validator.Verify(t => t.ValidateSumAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForSumAsyncViaTableName()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.SumAsync(ClassMappedNameCache.Get<DbValidatorEntity>(),
                field: new Field("Id"),
                where: (object)null).Wait();

            // Assert
            validator.Verify(t => t.ValidateSumAsync(), Times.Exactly(1));
        }

        #endregion

        #region SumAllAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForSumAllAsyncViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.SumAllAsync<DbValidatorEntity>(field: e => e.Id).Wait();

            // Assert
            validator.Verify(t => t.ValidateSumAllAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForSumAsyncViaTableNameAll()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.SumAllAsync(ClassMappedNameCache.Get<DbValidatorEntity>(),
                field: new Field("Id")).Wait();

            // Assert
            validator.Verify(t => t.ValidateSumAllAsync(), Times.Exactly(1));
        }

        #endregion

        #region TruncateAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForTruncateAsyncViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.TruncateAsync<DbValidatorEntity>().Wait();

            // Assert
            validator.Verify(t => t.ValidateTruncateAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForTruncateAsyncViaTableName()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.TruncateAsync(ClassMappedNameCache.Get<DbValidatorEntity>()).Wait();

            // Assert
            validator.Verify(t => t.ValidateTruncateAsync(), Times.Exactly(1));
        }

        #endregion

        #region UpdateAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForUpdateAsyncViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.UpdateAsync<DbValidatorEntity>(new DbValidatorEntity(),
                where: e => e.Id == 1).Wait();

            // Assert
            validator.Verify(t => t.ValidateUpdateAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForUpdateAsyncViaTableName()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.UpdateAsync(ClassMappedNameCache.Get<DbValidatorEntity>(),
                new DbValidatorEntity()).Wait();

            // Assert
            validator.Verify(t => t.ValidateUpdateAsync(), Times.Exactly(1));
        }

        #endregion

        #region UpdateAllAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForUpdateAllAsyncViaDataEntity()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.UpdateAllAsync<DbValidatorEntity>(new[] { new DbValidatorEntity() }).Wait();

            // Assert
            validator.Verify(t => t.ValidateUpdateAllAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForUpdateAsyncViaTableNameAll()
        {
            // Prepare
            var connection = new DbValidatorDbConnection();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            connection.UpdateAllAsync(ClassMappedNameCache.Get<DbValidatorEntity>(),
                new[] { new DbValidatorEntity() }).Wait();

            // Assert
            validator.Verify(t => t.ValidateUpdateAllAsync(), Times.Exactly(1));
        }

        #endregion

        #endregion
    }
}
