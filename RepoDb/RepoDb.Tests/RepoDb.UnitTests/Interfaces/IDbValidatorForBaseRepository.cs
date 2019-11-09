using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Enumerations;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Interfaces
{
    [TestClass]
    public class IDbValidatorForBaseRepository
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

        private class DbValidatorEntityRepository : BaseRepository<DbValidatorEntity, DbValidatorDbConnection>
        {
            public DbValidatorEntityRepository() :
                base("ConnectionString")
            { }
        }

        #endregion

        #region Sync

        #region Average

        [TestMethod]
        public void TestDbConnectionDbValidatorForAverageViaDataEntity()
        {
            // Prepare
            var repository = new DbValidatorEntityRepository();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.Average(field: e => e.Id,
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
            var repository = new DbValidatorEntityRepository();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.AverageAll(field: e => e.Id);

            // Assert
            validator.Verify(t => t.ValidateAverageAll(), Times.Exactly(1));
        }

        #endregion

        #region BatchQuery

        [TestMethod]
        public void TestDbConnectionDbValidatorForBatchQueryViaDataEntity()
        {
            // Prepare
            var repository = new DbValidatorEntityRepository();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.BatchQuery(page: 0,
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
            var repository = new DbValidatorEntityRepository();
            var entities = new[] { new DbValidatorEntity() };
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.BulkInsert(entities);

            // Assert
            validator.Verify(t => t.ValidateBulkInsert(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionDbValidatorForBulkInsertViaTableName()
        {
            // Prepare
            var repository = new DbValidatorEntityRepository();
            var entities = new[] { new DbValidatorEntity() };
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.BulkInsert(ClassMappedNameCache.Get<DbValidatorEntity>(),
                entities);

            // Assert
            validator.Verify(t => t.ValidateBulkInsert(), Times.Exactly(1));
        }

        #endregion

        #region Count

        [TestMethod]
        public void TestDbConnectionDbValidatorForCountViaDataEntity()
        {
            // Prepare
            var repository = new DbValidatorEntityRepository();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.Count(where: e => e.Id == 1);

            // Assert
            validator.Verify(t => t.ValidateCount(), Times.Exactly(1));
        }

        #endregion

        #region CountAll

        [TestMethod]
        public void TestDbConnectionDbValidatorForCountAllViaDataEntity()
        {
            // Prepare
            var repository = new DbValidatorEntityRepository();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.CountAll();

            // Assert
            validator.Verify(t => t.ValidateCountAll(), Times.Exactly(1));
        }

        #endregion

        #region Delete

        [TestMethod]
        public void TestDbConnectionDbValidatorForDeleteViaDataEntity()
        {
            // Prepare
            var repository = new DbValidatorEntityRepository();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.Delete(where: e => e.Id == 1);

            // Assert
            validator.Verify(t => t.ValidateDelete(), Times.Exactly(1));
        }

        #endregion

        #region DeleteAll

        [TestMethod]
        public void TestDbConnectionDbValidatorForDeleteAllViaDataEntity()
        {
            // Prepare
            var repository = new DbValidatorEntityRepository();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.DeleteAll();

            // Assert
            validator.Verify(t => t.ValidateDeleteAll(), Times.Exactly(1));
        }

        #endregion

        #region Exists

        [TestMethod]
        public void TestDbConnectionDbValidatorForExistsViaDataEntity()
        {
            // Prepare
            var repository = new DbValidatorEntityRepository();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.Exists(where: e => e.Id == 1);

            // Assert
            validator.Verify(t => t.ValidateExists(), Times.Exactly(1));
        }

        #endregion

        #region Insert

        [TestMethod]
        public void TestDbConnectionDbValidatorForInsertViaDataEntity()
        {
            // Prepare
            var repository = new DbValidatorEntityRepository();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.Insert(new DbValidatorEntity());

            // Assert
            validator.Verify(t => t.ValidateInsert(), Times.Exactly(1));
        }

        #endregion

        #region InsertAll

        [TestMethod]
        public void TestDbConnectionDbValidatorForInsertAllViaDataEntity()
        {
            // Prepare
            var repository = new DbValidatorEntityRepository();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.InsertAll(new[] { new DbValidatorEntity() });

            // Assert
            validator.Verify(t => t.ValidateInsertAll(), Times.Exactly(1));
        }

        #endregion

        #region Max

        [TestMethod]
        public void TestDbConnectionDbValidatorForMaxViaDataEntity()
        {
            // Prepare
            var repository = new DbValidatorEntityRepository();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.Max(field: e => e.Id,
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
            var repository = new DbValidatorEntityRepository();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.MaxAll(field: e => e.Id);

            // Assert
            validator.Verify(t => t.ValidateMaxAll(), Times.Exactly(1));
        }

        #endregion

        #region Merge

        [TestMethod]
        public void TestDbConnectionDbValidatorForMergeViaDataEntity()
        {
            // Prepare
            var repository = new DbValidatorEntityRepository();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.Merge(new DbValidatorEntity());

            // Assert
            validator.Verify(t => t.ValidateMerge(), Times.Exactly(1));
        }

        #endregion

        #region MergeAll

        [TestMethod]
        public void TestDbConnectionDbValidatorForMergeAllViaDataEntity()
        {
            // Prepare
            var repository = new DbValidatorEntityRepository();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.MergeAll(new[] { new DbValidatorEntity() });

            // Assert
            validator.Verify(t => t.ValidateMergeAll(), Times.Exactly(1));
        }

        #endregion

        #region Min

        [TestMethod]
        public void TestDbConnectionDbValidatorForMinViaDataEntity()
        {
            // Prepare
            var repository = new DbValidatorEntityRepository();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.Min(field: e => e.Id,
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
            var repository = new DbValidatorEntityRepository();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.MinAll(field: e => e.Id);

            // Assert
            validator.Verify(t => t.ValidateMinAll(), Times.Exactly(1));
        }

        #endregion

        #region Query

        [TestMethod]
        public void TestDbConnectionDbValidatorForQueryViaDataEntity()
        {
            // Prepare
            var repository = new DbValidatorEntityRepository();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.Query(where: e => e.Id == 1);

            // Assert
            validator.Verify(t => t.ValidateQuery(), Times.Exactly(1));
        }

        #endregion

        #region QueryAll

        [TestMethod]
        public void TestDbConnectionDbValidatorForQueryAllViaDataEntity()
        {
            // Prepare
            var repository = new DbValidatorEntityRepository();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.QueryAll();

            // Assert
            validator.Verify(t => t.ValidateQueryAll(), Times.Exactly(1));
        }

        #endregion

        #region Sum

        [TestMethod]
        public void TestDbConnectionDbValidatorForSumViaDataEntity()
        {
            // Prepare
            var repository = new DbValidatorEntityRepository();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.Sum(field: e => e.Id,
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
            var repository = new DbValidatorEntityRepository();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.SumAll(field: e => e.Id);

            // Assert
            validator.Verify(t => t.ValidateSumAll(), Times.Exactly(1));
        }

        #endregion

        #region Truncate

        [TestMethod]
        public void TestDbConnectionDbValidatorForTruncateViaDataEntity()
        {
            // Prepare
            var repository = new DbValidatorEntityRepository();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.Truncate();

            // Assert
            validator.Verify(t => t.ValidateTruncate(), Times.Exactly(1));
        }

        #endregion

        #region Update

        [TestMethod]
        public void TestDbConnectionDbValidatorForUpdateViaDataEntity()
        {
            // Prepare
            var repository = new DbValidatorEntityRepository();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.Update(new DbValidatorEntity(),
                where: e => e.Id == 1);

            // Assert
            validator.Verify(t => t.ValidateUpdate(), Times.Exactly(1));
        }

        #endregion

        #region UpdateAll

        [TestMethod]
        public void TestDbConnectionDbValidatorForUpdateAllViaDataEntity()
        {
            // Prepare
            var repository = new DbValidatorEntityRepository();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.UpdateAll(new[] { new DbValidatorEntity() });

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
            var repository = new DbValidatorEntityRepository();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.AverageAsync(field: e => e.Id,
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
            var repository = new DbValidatorEntityRepository();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.AverageAllAsync(field: e => e.Id).Wait();

            // Assert
            validator.Verify(t => t.ValidateAverageAllAsync(), Times.Exactly(1));
        }

        #endregion

        #region BatchQueryAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForBatchQueryAsyncViaDataEntity()
        {
            // Prepare
            var repository = new DbValidatorEntityRepository();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.BatchQueryAsync(page: 0,
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
            var repository = new DbValidatorEntityRepository();
            var entities = new[] { new DbValidatorEntity() };
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.BulkInsertAsync(entities).Wait();

            // Assert
            validator.Verify(t => t.ValidateBulkInsertAsync(), Times.Exactly(1));
        }
        
        [TestMethod]
        public void TestDbConnectionDbValidatorForBulkInsertAsyncViaTableName()
        {
            // Prepare
            var repository = new DbValidatorEntityRepository();
            var entities = new[] { new DbValidatorEntity() };
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.BulkInsertAsync(ClassMappedNameCache.Get<DbValidatorEntity>(),
                entities).Wait();

            // Assert
            validator.Verify(t => t.ValidateBulkInsertAsync(), Times.Exactly(1));
        }
        
        #endregion

        #region CountAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForCountAsyncViaDataEntity()
        {
            // Prepare
            var repository = new DbValidatorEntityRepository();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.CountAsync(where: e => e.Id == 1).Wait();

            // Assert
            validator.Verify(t => t.ValidateCountAsync(), Times.Exactly(1));
        }

        #endregion

        #region CountAllAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForCountAllAsyncViaDataEntity()
        {
            // Prepare
            var repository = new DbValidatorEntityRepository();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.CountAllAsync().Wait();

            // Assert
            validator.Verify(t => t.ValidateCountAllAsync(), Times.Exactly(1));
        }

        #endregion

        #region DeleteAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForDeleteAsyncViaDataEntity()
        {
            // Prepare
            var repository = new DbValidatorEntityRepository();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.DeleteAsync(where: e => e.Id == 1).Wait();

            // Assert
            validator.Verify(t => t.ValidateDeleteAsync(), Times.Exactly(1));
        }

        #endregion

        #region DeleteAllAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForDeleteAllAsyncViaDataEntity()
        {
            // Prepare
            var repository = new DbValidatorEntityRepository();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.DeleteAllAsync().Wait();

            // Assert
            validator.Verify(t => t.ValidateDeleteAllAsync(), Times.Exactly(1));
        }

        #endregion

        #region ExistsAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForExistsAsyncViaDataEntity()
        {
            // Prepare
            var repository = new DbValidatorEntityRepository();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.ExistsAsync(where: e => e.Id == 1).Wait();

            // Assert
            validator.Verify(t => t.ValidateExistsAsync(), Times.Exactly(1));
        }

        #endregion

        #region InsertAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForInsertAsyncViaDataEntity()
        {
            // Prepare
            var repository = new DbValidatorEntityRepository();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.InsertAsync(new DbValidatorEntity()).Wait();

            // Assert
            validator.Verify(t => t.ValidateInsertAsync(), Times.Exactly(1));
        }

        #endregion

        #region InsertAllAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForInsertAllAsyncViaDataEntity()
        {
            // Prepare
            var repository = new DbValidatorEntityRepository();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.InsertAllAsync(new[] { new DbValidatorEntity() }).Wait();

            // Assert
            validator.Verify(t => t.ValidateInsertAllAsync(), Times.Exactly(1));
        }

        #endregion

        #region MaxAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForMaxAsyncViaDataEntity()
        {
            // Prepare
            var repository = new DbValidatorEntityRepository();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.MaxAsync(field: e => e.Id,
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
            var repository = new DbValidatorEntityRepository();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.MaxAllAsync(field: e => e.Id).Wait();

            // Assert
            validator.Verify(t => t.ValidateMaxAllAsync(), Times.Exactly(1));
        }

        #endregion

        #region MergeAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForMergeAsyncViaDataEntity()
        {
            // Prepare
            var repository = new DbValidatorEntityRepository();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.MergeAsync(new DbValidatorEntity()).Wait();

            // Assert
            validator.Verify(t => t.ValidateMergeAsync(), Times.Exactly(1));
        }

        #endregion

        #region MergeAllAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForMergeAllAsyncViaDataEntity()
        {
            // Prepare
            var repository = new DbValidatorEntityRepository();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.MergeAllAsync(new[] { new DbValidatorEntity() }).Wait();

            // Assert
            validator.Verify(t => t.ValidateMergeAllAsync(), Times.Exactly(1));
        }

        #endregion

        #region MinAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForMinAsyncViaDataEntity()
        {
            // Prepare
            var repository = new DbValidatorEntityRepository();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.MinAsync(field: e => e.Id,
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
            var repository = new DbValidatorEntityRepository();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.MinAllAsync(field: e => e.Id).Wait();

            // Assert
            validator.Verify(t => t.ValidateMinAllAsync(), Times.Exactly(1));
        }

        #endregion

        #region QueryAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForQueryAsyncViaDataEntity()
        {
            // Prepare
            var repository = new DbValidatorEntityRepository();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.QueryAsync(where: e => e.Id == 1).Wait();

            // Assert
            validator.Verify(t => t.ValidateQueryAsync(), Times.Exactly(1));
        }

        #endregion

        #region QueryAllAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForQueryAllAsyncViaDataEntity()
        {
            // Prepare
            var repository = new DbValidatorEntityRepository();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.QueryAllAsync().Wait();

            // Assert
            validator.Verify(t => t.ValidateQueryAllAsync(), Times.Exactly(1));
        }

        #endregion

        #region SumAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForSumAsyncViaDataEntity()
        {
            // Prepare
            var repository = new DbValidatorEntityRepository();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.SumAsync(field: e => e.Id,
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
            var repository = new DbValidatorEntityRepository();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.SumAllAsync(field: e => e.Id).Wait();

            // Assert
            validator.Verify(t => t.ValidateSumAllAsync(), Times.Exactly(1));
        }

        #endregion

        #region TruncateAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForTruncateAsyncViaDataEntity()
        {
            // Prepare
            var repository = new DbValidatorEntityRepository();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.TruncateAsync().Wait();

            // Assert
            validator.Verify(t => t.ValidateTruncateAsync(), Times.Exactly(1));
        }

        #endregion

        #region UpdateAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForUpdateAsyncViaDataEntity()
        {
            // Prepare
            var repository = new DbValidatorEntityRepository();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.UpdateAsync(new DbValidatorEntity(),
                where: e => e.Id == 1).Wait();

            // Assert
            validator.Verify(t => t.ValidateUpdateAsync(), Times.Exactly(1));
        }

        #endregion

        #region UpdateAllAsync

        [TestMethod]
        public void TestDbConnectionDbValidatorForUpdateAllAsyncViaDataEntity()
        {
            // Prepare
            var repository = new DbValidatorEntityRepository();
            var validator = new Mock<IDbValidator>();
            DbValidatorMapper.Add(typeof(DbValidatorDbConnection), validator.Object, true);

            // Act
            repository.UpdateAllAsync(new[] { new DbValidatorEntity() }).Wait();

            // Assert
            validator.Verify(t => t.ValidateUpdateAllAsync(), Times.Exactly(1));
        }

        #endregion

        #endregion
    }
}
