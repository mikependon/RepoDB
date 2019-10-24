using System.Data.Common;

namespace RepoDb.Interfaces
{
    /// <summary>
    /// An interface used to mark a class to be a validator for the database operations.
    /// </summary>
    public interface IDbValidator
    {
        #region Average

        /// <summary>
        /// Validates the Average extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateAverage();

        /// <summary>
        /// Validates the AverageAsync extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateAverageAsync();

        #endregion

        #region AverageAll

        /// <summary>
        /// Validates the AverageAll extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateAverageAll();

        /// <summary>
        /// Validates the AverageAllAsync extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateAverageAllAsync();

        #endregion

        #region BatchQuery

        /// <summary>
        /// Validates the BatchQuery extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateBatchQuery();

        /// <summary>
        /// Validates the BatchQueryAsync extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateBatchQueryAsync();

        #endregion

        #region BulkInsert

        /// <summary>
        /// Validates the BulkInsert extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateBulkInsert();

        /// <summary>
        /// Validates the BulkInsertAsync extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateBulkInsertAsync();

        #endregion

        #region Count

        /// <summary>
        /// Validates the Count extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateCount();

        /// <summary>
        /// Validates the CountAsync extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateCountAsync();

        #endregion

        #region CountAll

        /// <summary>
        /// Validates the CountAll extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateCountAll();

        /// <summary>
        /// Validates the CountAllAsync extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateCountAllAsync();

        #endregion

        #region Delete

        /// <summary>
        /// Validates the Delete extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateDelete();

        /// <summary>
        /// Validates the DeleteAsync extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateDeleteAsync();

        #endregion

        #region DeleteAll

        /// <summary>
        /// Validates the DeleteAll extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateDeleteAll();

        /// <summary>
        /// Validates the DeleteAllAsync extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateDeleteAllAsync();

        #endregion

        #region Insert

        /// <summary>
        /// Validates the Insert extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateInsert();

        /// <summary>
        /// Validates the InsertAsync extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateInsertAsync();

        #endregion

        #region Insert

        /// <summary>
        /// Validates the InsertAll extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateInsertAll();

        /// <summary>
        /// Validates the InsertAllAsync extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateInsertAllAsync();

        #endregion

        #region Max

        /// <summary>
        /// Validates the Max extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateMax();

        /// <summary>
        /// Validates the MaxAsync extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateMaxAsync();

        #endregion

        #region MaxAll

        /// <summary>
        /// Validates the MaxAll extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateMaxAll();

        /// <summary>
        /// Validates the MaxAllAsync extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateMaxAllAsync();

        #endregion

        #region Merge

        /// <summary>
        /// Validates the Merge extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateMerge();

        /// <summary>
        /// Validates the MergeAsync extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateMergeAsync();

        #endregion

        #region MergeAll

        /// <summary>
        /// Validates the MergeAll extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateMergeAll();

        /// <summary>
        /// Validates the MergeAllAsync extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateMergeAllAsync();

        #endregion

        #region Min

        /// <summary>
        /// Validates the Min extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateMin();

        /// <summary>
        /// Validates the MinAsync extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateMinAsync();

        #endregion

        #region MinAll

        /// <summary>
        /// Validates the MinAll extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateMinAll();

        /// <summary>
        /// Validates the MinAllAsync extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateMinAllAsync();

        #endregion

        #region Query

        /// <summary>
        /// Validates the Query extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateQuery();

        /// <summary>
        /// Validates the QueryAsync extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateQueryAsync();

        #endregion

        #region QueryAll

        /// <summary>
        /// Validates the QueryAll extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateQueryAll();

        /// <summary>
        /// Validates the QueryAllAsync extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateQueryAllAsync();

        #endregion

        #region QueryMultiple

        /// <summary>
        /// Validates the QueryMultiple extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateQueryMultiple();

        /// <summary>
        /// Validates the QueryMultipleAsync extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateQueryMultipleAsync();

        #endregion

        #region Sum

        /// <summary>
        /// Validates the Sum extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateSum();

        /// <summary>
        /// Validates the SumAsync extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateSumAsync();

        #endregion

        #region SumAll

        /// <summary>
        /// Validates the SumAll extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateSumAll();

        /// <summary>
        /// Validates the SumAllAsync extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateSumAllAsync();

        #endregion

        #region Truncate

        /// <summary>
        /// Validates the Truncate extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateTruncate();

        /// <summary>
        /// Validates the TruncateAsync extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateTruncateAsync();

        #endregion

        #region Update

        /// <summary>
        /// Validates the Update extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateUpdate();

        /// <summary>
        /// Validates the UpdateAsync extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateUpdateAsync();

        #endregion

        #region UpdateAll

        /// <summary>
        /// Validates the UpdateAll extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateUpdateAll();

        /// <summary>
        /// Validates the UpdateAllAsync extended method of the <see cref="DbConnection"/> object.
        /// </summary>
        void ValidateUpdateAllAsync();

        #endregion
    }
}
