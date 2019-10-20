using RepoDb.Interfaces;
using System.Data.SqlClient;

namespace RepoDb.DbValidators
{
    /// <summary>
    /// A class that is used to validate the <see cref="SqlConnection"/> extended methods.
    /// </summary>
    internal sealed class SqlServerDbValidator : IDbValidator
    {
        #region BatchQuery

        /// <summary>
        /// Validates the BatchQuery extended method of the <see cref="SqlConnection"/> object.
        /// </summary>
        public void ValidateBatchQuery() { /* This is supported. */}

        /// <summary>
        /// Validates the BatchQueryAsync extended method of the <see cref="SqlConnection"/> object.
        /// </summary>
        public void ValidateBatchQueryAsync() { /* This is supported. */ }

        #endregion

        #region BulkInsert

        /// <summary>
        /// Validates the BulkInsert extended method of the <see cref="SqlConnection"/> object.
        /// </summary>
        public void ValidateBulkInsert() { /* This is supported. */ }

        /// <summary>
        /// Validates the BulkInsertAsync extended method of the <see cref="SqlConnection"/> object.
        /// </summary>
        public void ValidateBulkInsertAsync() { /* This is supported. */ }

        #endregion

        #region Count

        /// <summary>
        /// Validates the Count extended method of the <see cref="SqlConnection"/> object.
        /// </summary>
        public void ValidateCount() { /* This is supported. */ }

        /// <summary>
        /// Validates the CountAsync extended method of the <see cref="SqlConnection"/> object.
        /// </summary>
        public void ValidateCountAsync() { /* This is supported. */ }

        #endregion

        #region CountAll

        /// <summary>
        /// Validates the CountAll extended method of the <see cref="SqlConnection"/> object.
        /// </summary>
        public void ValidateCountAll() { /* This is supported. */ }

        /// <summary>
        /// Validates the CountAllAsync extended method of the <see cref="SqlConnection"/> object.
        /// </summary>
        public void ValidateCountAllAsync() { /* This is supported. */ }

        #endregion

        #region Delete

        /// <summary>
        /// Validates the Delete extended method of the <see cref="SqlConnection"/> object.
        /// </summary>
        public void ValidateDelete() { /* This is supported. */ }

        /// <summary>
        /// Validates the DeleteAsync extended method of the <see cref="SqlConnection"/> object.
        /// </summary>
        public void ValidateDeleteAsync() { /* This is supported. */ }

        #endregion

        #region DeleteAll

        /// <summary>
        /// Validates the DeleteAll extended method of the <see cref="SqlConnection"/> object.
        /// </summary>
        public void ValidateDeleteAll() { /* This is supported. */ }

        /// <summary>
        /// Validates the DeleteAllAsync extended method of the <see cref="SqlConnection"/> object.
        /// </summary>
        public void ValidateDeleteAllAsync() { /* This is supported. */ }

        #endregion

        #region Insert

        /// <summary>
        /// Validates the Insert extended method of the <see cref="SqlConnection"/> object.
        /// </summary>
        public void ValidateInsert() { /* This is supported. */ }

        /// <summary>
        /// Validates the InsertAsync extended method of the <see cref="SqlConnection"/> object.
        /// </summary>
        public void ValidateInsertAsync() { /* This is supported. */ }

        #endregion

        #region InsertAll

        /// <summary>
        /// Validates the InsertAll extended method of the <see cref="SqlConnection"/> object.
        /// </summary>
        public void ValidateInsertAll() { /* This is supported. */ }

        /// <summary>
        /// Validates the InsertAllAsync extended method of the <see cref="SqlConnection"/> object.
        /// </summary>
        public void ValidateInsertAllAsync() { /* This is supported. */ }

        #endregion

        #region Merge

        /// <summary>
        /// Validates the Merge extended method of the <see cref="SqlConnection"/> object.
        /// </summary>
        public void ValidateMerge() { /* This is supported. */ }

        /// <summary>
        /// Validates the MergeAsync extended method of the <see cref="SqlConnection"/> object.
        /// </summary>
        public void ValidateMergeAsync() { /* This is supported. */ }

        #endregion

        #region MergeAll

        /// <summary>
        /// Validates the MergeAll extended method of the <see cref="SqlConnection"/> object.
        /// </summary>
        public void ValidateMergeAll() { /* This is supported. */ }

        /// <summary>
        /// Validates the MergeAllAsync extended method of the <see cref="SqlConnection"/> object.
        /// </summary>
        public void ValidateMergeAllAsync() { /* This is supported. */ }

        #endregion

        #region Query

        /// <summary>
        /// Validates the Query extended method of the <see cref="SqlConnection"/> object.
        /// </summary>
        public void ValidateQuery() { /* This is supported. */ }

        /// <summary>
        /// Validates the QueryAsync extended method of the <see cref="SqlConnection"/> object.
        /// </summary>
        public void ValidateQueryAsync() { /* This is supported. */ }

        #endregion

        #region QueryAll

        /// <summary>
        /// Validates the QueryAll extended method of the <see cref="SqlConnection"/> object.
        /// </summary>
        public void ValidateQueryAll() { /* This is supported. */ }

        /// <summary>
        /// Validates the QueryAllAsync extended method of the <see cref="SqlConnection"/> object.
        /// </summary>
        public void ValidateQueryAllAsync() { /* This is supported. */ }

        #endregion

        #region QueryMultiple

        /// <summary>
        /// Validates the QueryMultiple extended method of the <see cref="SqlConnection"/> object.
        /// </summary>
        public void ValidateQueryMultiple() { /* This is supported. */ }

        /// <summary>
        /// Validates the QueryMultipleAsync extended method of the <see cref="SqlConnection"/> object.
        /// </summary>
        public void ValidateQueryMultipleAsync() { /* This is supported. */ }

        #endregion

        #region Truncate

        /// <summary>
        /// Validates the Truncate extended method of the <see cref="SqlConnection"/> object.
        /// </summary>
        public void ValidateTruncate() { /* This is supported. */ }

        /// <summary>
        /// Validates the TruncateAsync extended method of the <see cref="SqlConnection"/> object.
        /// </summary>
        public void ValidateTruncateAsync() { /* This is supported. */ }

        #endregion

        #region Update

        /// <summary>
        /// Validates the Update extended method of the <see cref="SqlConnection"/> object.
        /// </summary>
        public void ValidateUpdate() { /* This is supported. */ }

        /// <summary>
        /// Validates the UpdateAsync extended method of the <see cref="SqlConnection"/> object.
        /// </summary>
        public void ValidateUpdateAsync() { /* This is supported. */ }

        #endregion

        #region UpdateAll

        /// <summary>
        /// Validates the UpdateAll extended method of the <see cref="SqlConnection"/> object.
        /// </summary>
        public void ValidateUpdateAll() { /* This is supported. */ }

        /// <summary>
        /// Validates the UpdateAllAsync extended method of the <see cref="SqlConnection"/> object.
        /// </summary>
        public void ValidateUpdateAllAsync() { /* This is supported. */ }

        #endregion
    }
}
