using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Enumerations;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Interfaces
{
    [TestClass]
    public class IDbValidatorForDbRepository
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

        private class DbValidatorCustomDbOperation : CustomDbOperation { }

        #endregion

        #region Sync

        #region Average

        [TestMethod]
        public void TestDbConnectionDbValidatorForAverageViaDataEntity()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.Average<DbValidatorEntity>(field: e => e.Id,
                where: (object)null);

            // Assert
            validator.Verify(t => t.ValidateAverage(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForAverageViaTableName()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.Average(ClassMappedNameCache.Get<DbValidatorEntity>(),
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
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.AverageAll<DbValidatorEntity>(field: e => e.Id);

            // Assert
            validator.Verify(t => t.ValidateAverageAll(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForAverageViaTableNameAll()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.AverageAll(ClassMappedNameCache.Get<DbValidatorEntity>(),
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
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.BatchQuery<DbValidatorEntity>(page: 0,
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
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.BatchQuery(ClassMappedNameCache.Get<DbValidatorEntity>(),
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
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var entities = new[] { new DbValidatorEntity() };
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.BulkInsert<DbValidatorEntity>(entities);

            // Assert
            validator.Verify(t => t.ValidateBulkInsert(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForBulkInsertViaDataEntityAsDataReader()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var entities = new[] { new DbValidatorEntity() };
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            using (var reader = new DataEntityDataReader<DbValidatorEntity>(entities))
            {
                repository.BulkInsert<DbValidatorEntity>(reader);
            }

            // Assert
            validator.Verify(t => t.ValidateBulkInsert(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForBulkInsertViaTableName()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var entities = new[] { new DbValidatorEntity() };
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.BulkInsert(ClassMappedNameCache.Get<DbValidatorEntity>(),
                entities);

            // Assert
            validator.Verify(t => t.ValidateBulkInsert(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForBulkInsertViaTableNameAsDataReader()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var entities = new[] { new DbValidatorEntity() };
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            using (var reader = new DataEntityDataReader<DbValidatorEntity>(entities))
            {
                repository.BulkInsert(ClassMappedNameCache.Get<DbValidatorEntity>(),
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
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.Count<DbValidatorEntity>(where: e => e.Id == 1);

            // Assert
            validator.Verify(t => t.ValidateCount(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForCountViaTableName()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.Count(ClassMappedNameCache.Get<DbValidatorEntity>(),
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
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.CountAll<DbValidatorEntity>();

            // Assert
            validator.Verify(t => t.ValidateCountAll(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForCountViaTableNameAll()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.CountAll(ClassMappedNameCache.Get<DbValidatorEntity>());

            // Assert
            validator.Verify(t => t.ValidateCountAll(), Times.Exactly(1));
        }

        #endregion

        #region Delete

        [TestMethod]
        public void TestDbConnectionDbValidatorForDeleteViaDataEntity()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.Delete<DbValidatorEntity>(where: e => e.Id == 1);

            // Assert
            validator.Verify(t => t.ValidateDelete(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForDeleteViaTableName()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.Delete(ClassMappedNameCache.Get<DbValidatorEntity>(),
                where: new { Id = 1 });

            // Assert
            validator.Verify(t => t.ValidateDelete(), Times.Exactly(1));
        }

        #endregion

        #region DeleteAll

        [TestMethod]
        public void TestDbConnectionDbValidatorForDeleteAllViaDataEntity()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.DeleteAll<DbValidatorEntity>();

            // Assert
            validator.Verify(t => t.ValidateDeleteAll(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForDeleteViaTableNameAll()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.DeleteAll(ClassMappedNameCache.Get<DbValidatorEntity>());

            // Assert
            validator.Verify(t => t.ValidateDeleteAll(), Times.Exactly(1));
        }

        #endregion

        #region Insert

        [TestMethod]
        public void TestDbConnectionDbValidatorForInsertViaDataEntity()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.Insert<DbValidatorEntity>(new DbValidatorEntity());

            // Assert
            validator.Verify(t => t.ValidateInsert(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForInsertViaTableName()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.Insert(ClassMappedNameCache.Get<DbValidatorEntity>(),
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
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.InsertAll<DbValidatorEntity>(new[] { new DbValidatorEntity() });

            // Assert
            validator.Verify(t => t.ValidateInsertAll(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForInsertViaTableNameAll()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.InsertAll(ClassMappedNameCache.Get<DbValidatorEntity>(),
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
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.Max<DbValidatorEntity>(field: e => e.Id,
                where: (object)null);

            // Assert
            validator.Verify(t => t.ValidateMax(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForMaxViaTableName()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.Max(ClassMappedNameCache.Get<DbValidatorEntity>(),
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
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.MaxAll<DbValidatorEntity>(field: e => e.Id);

            // Assert
            validator.Verify(t => t.ValidateMaxAll(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForMaxViaTableNameAll()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.MaxAll(ClassMappedNameCache.Get<DbValidatorEntity>(),
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
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.Merge<DbValidatorEntity>(new DbValidatorEntity());

            // Assert
            validator.Verify(t => t.ValidateMerge(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForMergeViaTableName()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.Merge(ClassMappedNameCache.Get<DbValidatorEntity>(),
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
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.MergeAll<DbValidatorEntity>(new[] { new DbValidatorEntity() });

            // Assert
            validator.Verify(t => t.ValidateMergeAll(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForMergeViaTableNameAll()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.MergeAll(ClassMappedNameCache.Get<DbValidatorEntity>(),
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
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.Min<DbValidatorEntity>(field: e => e.Id,
                where: (object)null);

            // Assert
            validator.Verify(t => t.ValidateMin(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForMinViaTableName()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.Min(ClassMappedNameCache.Get<DbValidatorEntity>(),
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
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.MinAll<DbValidatorEntity>(field: e => e.Id);

            // Assert
            validator.Verify(t => t.ValidateMinAll(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForMinViaTableNameAll()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.MinAll(ClassMappedNameCache.Get<DbValidatorEntity>(),
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
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.Query<DbValidatorEntity>(where: e => e.Id == 1);

            // Assert
            validator.Verify(t => t.ValidateQuery(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForQueryViaTableName()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.Query(ClassMappedNameCache.Get<DbValidatorEntity>(),
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
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.QueryAll<DbValidatorEntity>();

            // Assert
            validator.Verify(t => t.ValidateQueryAll(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForQueryViaTableNameAll()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.QueryAll(ClassMappedNameCache.Get<DbValidatorEntity>());

            // Assert
            validator.Verify(t => t.ValidateQueryAll(), Times.Exactly(1));
        }

        #endregion

        #region QueryMultiple

        [TestMethod]
        public void TestDbConnectionDbValidatorForQueryMultipleT2()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.QueryMultiple<DbValidatorEntity, DbValidatorEntity>(e => e.Id == 1,
                e => e.Id == 2);

            // Assert
            validator.Verify(t => t.ValidateQueryMultiple(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForQueryMultipleT3()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.QueryMultiple<DbValidatorEntity, DbValidatorEntity, DbValidatorEntity>(e => e.Id == 1,
                e => e.Id == 2,
                e => e.Id == 2);

            // Assert
            validator.Verify(t => t.ValidateQueryMultiple(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForQueryMultipleT4()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.QueryMultiple<DbValidatorEntity, DbValidatorEntity, DbValidatorEntity,
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
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.QueryMultiple<DbValidatorEntity, DbValidatorEntity, DbValidatorEntity,
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
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.QueryMultiple<DbValidatorEntity, DbValidatorEntity, DbValidatorEntity,
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
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.QueryMultiple<DbValidatorEntity, DbValidatorEntity, DbValidatorEntity,
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
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.Sum<DbValidatorEntity>(field: e => e.Id,
                where: (object)null);

            // Assert
            validator.Verify(t => t.ValidateSum(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForSumViaTableName()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.Sum(ClassMappedNameCache.Get<DbValidatorEntity>(),
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
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.SumAll<DbValidatorEntity>(field: e => e.Id);

            // Assert
            validator.Verify(t => t.ValidateSumAll(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForSumViaTableNameAll()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.SumAll(ClassMappedNameCache.Get<DbValidatorEntity>(),
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
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.Truncate<DbValidatorEntity>();

            // Assert
            validator.Verify(t => t.ValidateTruncate(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForTruncateViaTableName()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.Truncate(ClassMappedNameCache.Get<DbValidatorEntity>());

            // Assert
            validator.Verify(t => t.ValidateTruncate(), Times.Exactly(1));
        }

        #endregion

        #region Update

        [TestMethod]
        public void TestDbConnectionDbValidatorForUpdateViaDataEntity()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.Update<DbValidatorEntity>(new DbValidatorEntity(),
                where: e => e.Id == 1);

            // Assert
            validator.Verify(t => t.ValidateUpdate(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForUpdateViaTableName()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.Update(ClassMappedNameCache.Get<DbValidatorEntity>(),
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
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.UpdateAll<DbValidatorEntity>(new[] { new DbValidatorEntity() });

            // Assert
            validator.Verify(t => t.ValidateUpdateAll(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForUpdateViaTableNameAll()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.UpdateAll(ClassMappedNameCache.Get<DbValidatorEntity>(),
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
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.AverageAsync<DbValidatorEntity>(field: e => e.Id,
                where: (object)null).Wait();

            // Assert
            validator.Verify(t => t.ValidateAverageAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForAverageAsyncViaTableName()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.AverageAsync(ClassMappedNameCache.Get<DbValidatorEntity>(),
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
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.AverageAllAsync<DbValidatorEntity>(field: e => e.Id).Wait();

            // Assert
            validator.Verify(t => t.ValidateAverageAllAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForAverageAsyncViaTableNameAll()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.AverageAllAsync(ClassMappedNameCache.Get<DbValidatorEntity>(),
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
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.BatchQueryAsync<DbValidatorEntity>(page: 0,
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
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.BatchQueryAsync(ClassMappedNameCache.Get<DbValidatorEntity>(),
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
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var entities = new[] { new DbValidatorEntity() };
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.BulkInsertAsync<DbValidatorEntity>(entities).Wait();

            // Assert
            validator.Verify(t => t.ValidateBulkInsertAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForBulkInsertAsyncViaDataEntityAsDataReader()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var entities = new[] { new DbValidatorEntity() };
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            using (var reader = new DataEntityDataReader<DbValidatorEntity>(entities))
            {
                repository.BulkInsertAsync<DbValidatorEntity>(reader).Wait();
            }

            // Assert
            validator.Verify(t => t.ValidateBulkInsertAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForBulkInsertAsyncViaTableName()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var entities = new[] { new DbValidatorEntity() };
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.BulkInsertAsync(ClassMappedNameCache.Get<DbValidatorEntity>(),
                entities).Wait();

            // Assert
            validator.Verify(t => t.ValidateBulkInsertAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForBulkInsertAsyncViaTableNameAsDataReader()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var entities = new[] { new DbValidatorEntity() };
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            using (var reader = new DataEntityDataReader<DbValidatorEntity>(entities))
            {
                repository.BulkInsertAsync(ClassMappedNameCache.Get<DbValidatorEntity>(),
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
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.CountAsync<DbValidatorEntity>(where: e => e.Id == 1).Wait();

            // Assert
            validator.Verify(t => t.ValidateCountAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForCountAsyncViaTableName()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.CountAsync(ClassMappedNameCache.Get<DbValidatorEntity>(),
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
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.CountAllAsync<DbValidatorEntity>().Wait();

            // Assert
            validator.Verify(t => t.ValidateCountAllAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForCountAsyncViaTableNameAll()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.CountAllAsync(ClassMappedNameCache.Get<DbValidatorEntity>()).Wait();

            // Assert
            validator.Verify(t => t.ValidateCountAllAsync(), Times.Exactly(1));
        }

        #endregion

        #region DeleteAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForDeleteAsyncViaDataEntity()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.DeleteAsync<DbValidatorEntity>(where: e => e.Id == 1).Wait();

            // Assert
            validator.Verify(t => t.ValidateDeleteAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForDeleteAsyncViaTableName()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.DeleteAsync(ClassMappedNameCache.Get<DbValidatorEntity>(),
                where: new { Id = 1 }).Wait();

            // Assert
            validator.Verify(t => t.ValidateDeleteAsync(), Times.Exactly(1));
        }

        #endregion

        #region DeleteAllAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForDeleteAllAsyncViaDataEntity()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.DeleteAllAsync<DbValidatorEntity>().Wait();

            // Assert
            validator.Verify(t => t.ValidateDeleteAllAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForDeleteAsyncViaTableNameAll()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.DeleteAllAsync(ClassMappedNameCache.Get<DbValidatorEntity>()).Wait();

            // Assert
            validator.Verify(t => t.ValidateDeleteAllAsync(), Times.Exactly(1));
        }

        #endregion

        #region InsertAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForInsertAsyncViaDataEntity()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.InsertAsync<DbValidatorEntity>(new DbValidatorEntity()).Wait();

            // Assert
            validator.Verify(t => t.ValidateInsertAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForInsertAsyncViaTableName()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.InsertAsync(ClassMappedNameCache.Get<DbValidatorEntity>(),
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
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.InsertAllAsync<DbValidatorEntity>(new[] { new DbValidatorEntity() }).Wait();

            // Assert
            validator.Verify(t => t.ValidateInsertAllAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForInsertAsyncViaTableNameAll()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.InsertAllAsync(ClassMappedNameCache.Get<DbValidatorEntity>(),
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
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.MaxAsync<DbValidatorEntity>(field: e => e.Id,
                where: (object)null).Wait();

            // Assert
            validator.Verify(t => t.ValidateMaxAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForMaxAsyncViaTableName()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.MaxAsync(ClassMappedNameCache.Get<DbValidatorEntity>(),
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
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.MaxAllAsync<DbValidatorEntity>(field: e => e.Id).Wait();

            // Assert
            validator.Verify(t => t.ValidateMaxAllAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForMaxAsyncViaTableNameAll()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.MaxAllAsync(ClassMappedNameCache.Get<DbValidatorEntity>(),
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
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.MergeAsync<DbValidatorEntity>(new DbValidatorEntity()).Wait();

            // Assert
            validator.Verify(t => t.ValidateMergeAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForMergeAsyncViaTableName()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.MergeAsync(ClassMappedNameCache.Get<DbValidatorEntity>(),
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
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.MergeAllAsync<DbValidatorEntity>(new[] { new DbValidatorEntity() }).Wait();

            // Assert
            validator.Verify(t => t.ValidateMergeAllAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForMergeAsyncViaTableNameAll()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.MergeAllAsync(ClassMappedNameCache.Get<DbValidatorEntity>(),
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
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.MinAsync<DbValidatorEntity>(field: e => e.Id,
                where: (object)null).Wait();

            // Assert
            validator.Verify(t => t.ValidateMinAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForMinAsyncViaTableName()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.MinAsync(ClassMappedNameCache.Get<DbValidatorEntity>(),
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
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.MinAllAsync<DbValidatorEntity>(field: e => e.Id).Wait();

            // Assert
            validator.Verify(t => t.ValidateMinAllAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForMinAsyncViaTableNameAll()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.MinAllAsync(ClassMappedNameCache.Get<DbValidatorEntity>(),
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
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.QueryAsync<DbValidatorEntity>(where: e => e.Id == 1).Wait();

            // Assert
            validator.Verify(t => t.ValidateQueryAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForQueryAsyncViaTableName()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.QueryAsync(ClassMappedNameCache.Get<DbValidatorEntity>(),
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
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.QueryAllAsync<DbValidatorEntity>().Wait();

            // Assert
            validator.Verify(t => t.ValidateQueryAllAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForQueryAsyncViaTableNameAll()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.QueryAllAsync(ClassMappedNameCache.Get<DbValidatorEntity>()).Wait();

            // Assert
            validator.Verify(t => t.ValidateQueryAllAsync(), Times.Exactly(1));
        }

        #endregion

        #region QueryMultipleAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForQueryMultipleAsyncT2()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.QueryMultipleAsync<DbValidatorEntity, DbValidatorEntity>(e => e.Id == 1,
                e => e.Id == 2).Wait();

            // Assert
            validator.Verify(t => t.ValidateQueryMultipleAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForQueryMultipleAsyncT3()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.QueryMultipleAsync<DbValidatorEntity, DbValidatorEntity, DbValidatorEntity>(e => e.Id == 1,
                e => e.Id == 2,
                e => e.Id == 2).Wait();

            // Assert
            validator.Verify(t => t.ValidateQueryMultipleAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForQueryMultipleAsyncT4()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.QueryMultipleAsync<DbValidatorEntity, DbValidatorEntity, DbValidatorEntity,
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
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.QueryMultipleAsync<DbValidatorEntity, DbValidatorEntity, DbValidatorEntity,
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
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.QueryMultipleAsync<DbValidatorEntity, DbValidatorEntity, DbValidatorEntity,
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
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.QueryMultipleAsync<DbValidatorEntity, DbValidatorEntity, DbValidatorEntity,
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
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.SumAsync<DbValidatorEntity>(field: e => e.Id,
                where: (object)null).Wait();

            // Assert
            validator.Verify(t => t.ValidateSumAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForSumAsyncViaTableName()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.SumAsync(ClassMappedNameCache.Get<DbValidatorEntity>(),
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
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.SumAllAsync<DbValidatorEntity>(field: e => e.Id).Wait();

            // Assert
            validator.Verify(t => t.ValidateSumAllAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForSumAsyncViaTableNameAll()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.SumAllAsync(ClassMappedNameCache.Get<DbValidatorEntity>(),
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
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.TruncateAsync<DbValidatorEntity>().Wait();

            // Assert
            validator.Verify(t => t.ValidateTruncateAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForTruncateAsyncViaTableName()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.TruncateAsync(ClassMappedNameCache.Get<DbValidatorEntity>()).Wait();

            // Assert
            validator.Verify(t => t.ValidateTruncateAsync(), Times.Exactly(1));
        }

        #endregion

        #region UpdateAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForUpdateAsyncViaDataEntity()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.UpdateAsync<DbValidatorEntity>(new DbValidatorEntity(),
                where: e => e.Id == 1).Wait();

            // Assert
            validator.Verify(t => t.ValidateUpdateAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForUpdateAsyncViaTableName()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.UpdateAsync(ClassMappedNameCache.Get<DbValidatorEntity>(),
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
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.UpdateAllAsync<DbValidatorEntity>(new[] { new DbValidatorEntity() }).Wait();

            // Assert
            validator.Verify(t => t.ValidateUpdateAllAsync(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForUpdateAsyncViaTableNameAll()
        {
            // Prepare
            var repository = new DbRepository<DbValidatorDbConnection>("ConnectionString");
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.UpdateAllAsync(ClassMappedNameCache.Get<DbValidatorEntity>(),
                new[] { new DbValidatorEntity() }).Wait();

            // Assert
            validator.Verify(t => t.ValidateUpdateAllAsync(), Times.Exactly(1));
        }

        #endregion

        #endregion
    }
}
