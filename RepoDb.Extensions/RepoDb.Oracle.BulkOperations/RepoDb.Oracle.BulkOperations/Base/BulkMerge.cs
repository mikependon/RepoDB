using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Enumerations.Oracle;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Oracle.BulkOperations;
using RepoDb.Oracle.BulkOperations.Base;
using RepoDb.Oracle.BulkOperations.Extensions;

namespace RepoDb
{
    /// <summary>
    ///
    /// </summary>
    public static partial class OracleConnectionExtension
    {
        #region Sync

        #region BulkMergeBase<TEntity>

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="qualifiers"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private static int BulkMergeBase<TEntity>(this OracleConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            OracleBulkImportIdentityBehavior identityBehavior = default,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            OracleTransaction transaction = null)
            where TEntity : class
        {
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var identityField = dbFields.GetIdentity();
            var returnIdentity = identityBehavior == OracleBulkImportIdentityBehavior.ReturnIdentity && identityField != null;

            if (returnIdentity)
            {
                return connection.BulkMergeBaseForReturnIdentity(tableName,
                    entities,
                    qualifiers,
                    mappings,
                    bulkCopyTimeout,
                    identityBehavior,
                    pseudoTableType,
                    transaction);
            }
            else
            {
                return connection.BulkMergeBaseNoReturnIdentity(tableName,
                    entities,
                    qualifiers,
                    mappings,
                    bulkCopyTimeout,
                    pseudoTableType,
                    transaction);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="qualifiers"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException">
        /// Not implemented yet. This pass only covers the <see cref="OracleBulkImportIdentityBehavior.ReturnIdentity"/> == <c>false</c>
        /// path - see <see cref="BulkMergeBaseNoReturnIdentity{TEntity}"/>.
        /// </exception>
        private static int BulkMergeBaseForReturnIdentity<TEntity>(this OracleConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            OracleBulkImportIdentityBehavior identityBehavior = default,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            OracleTransaction transaction = null)
            where TEntity : class
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Upserts <paramref name="entities"/> into <paramref name="tableName"/> via a staging (pseudo)
        /// table: the pseudo table is (re)used and cleared, the entities are bulk-written into it, and a
        /// single <c>MERGE</c> statement upserts every staged row into the real table.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="qualifiers">The field(s) to match an existing row on. Defaults to the primary/identity key when not provided.</param>
        /// <param name="mappings">
        /// The explicit source-to-destination column mapping. When provided, only the destination columns named here
        /// (plus, always, the qualifier column(s)) are staged and merged - if a qualifier column is intentionally
        /// left out of <paramref name="mappings"/>, that column will never be populated on the staging table and
        /// every row will be treated as new (insert-only) rather than matched for update.
        /// </param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction">
        /// The transaction under which the staging-table DDL and the final <c>MERGE</c> statement run. Note that
        /// the bulk-write step in between (<see cref="OracleBulkCopy"/>) is transaction-agnostic - ODP.NET does not
        /// support enlisting a bulk-copy operation into a transaction, so that specific step always commits
        /// immediately regardless of this parameter.
        /// </param>
        /// <returns>The number of rows affected by the <c>MERGE</c>.</returns>
        private static int BulkMergeBaseNoReturnIdentity<TEntity>(this OracleConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            OracleTransaction transaction = null)
            where TEntity : class
        {
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers);
            var mergeFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields);
            var pseudoTableName = OracleText.GetPseudoTableNameForMerge(tableName, pseudoTableType);

            OracleExecution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, transaction);
            OracleExecution.TruncatePseudoTable(connection, pseudoTableName, transaction);
            WriteToServer.WriteToServerInternal(connection, pseudoTableName, entities, mappings, bulkCopyTimeout);
            return OracleExecution.MergeFromPseudoTable(connection, tableName, pseudoTableName, mergeFields, qualifierFields, transaction);
        }

        #endregion

        #region BulkMergeBase<DataTable>

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="qualifiers"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static int BulkMergeBase(this OracleConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            OracleBulkImportIdentityBehavior identityBehavior = default,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            OracleTransaction transaction = null)
        {
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var identityField = dbFields.GetIdentity();
            var returnIdentity = identityBehavior == OracleBulkImportIdentityBehavior.ReturnIdentity && identityField != null;

            if (returnIdentity)
            {
                return connection.BulkMergeBaseForReturnIdentity(tableName,
                    table,
                    qualifiers,
                    rowState,
                    mappings,
                    bulkCopyTimeout,
                    identityBehavior,
                    pseudoTableType,
                    transaction);
            }
            else
            {
                return connection.BulkMergeBaseNoReturnIdentity(tableName,
                    table,
                    qualifiers,
                    rowState,
                    mappings,
                    bulkCopyTimeout,
                    pseudoTableType,
                    transaction);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="qualifiers"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException">
        /// Not implemented yet. This pass only covers the <see cref="OracleBulkImportIdentityBehavior.ReturnIdentity"/> == <c>false</c>
        /// path - see <see cref="BulkMergeBaseNoReturnIdentity(OracleConnection, string, DataTable, IEnumerable{Field}, DataRowState?, IEnumerable{OracleBulkInsertMapItem}, int?, OracleBulkImportPseudoTableType, OracleTransaction)"/>.
        /// </exception>
        private static int BulkMergeBaseForReturnIdentity(this OracleConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            OracleBulkImportIdentityBehavior identityBehavior = default,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            OracleTransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Upserts the rows of <paramref name="table"/> into <paramref name="tableName"/> via a staging
        /// (pseudo) table, following the same steps as the <c>TEntity</c> overload - see
        /// <see cref="BulkMergeBaseNoReturnIdentity{TEntity}"/> for the detailed remarks (identical
        /// caveats around <paramref name="mappings"/> and <paramref name="transaction"/> apply here).
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="qualifiers"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <returns>The number of rows affected by the <c>MERGE</c>.</returns>
        private static int BulkMergeBaseNoReturnIdentity(this OracleConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            OracleTransaction transaction = null)
        {
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers);
            var mergeFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields);
            var pseudoTableName = OracleText.GetPseudoTableNameForMerge(tableName, pseudoTableType);

            OracleExecution.CreatePseudoTable(connection, tableName, pseudoTableName, pseudoTableType, transaction);
            OracleExecution.TruncatePseudoTable(connection, pseudoTableName, transaction);
            WriteToServer.WriteToServerInternal(connection, pseudoTableName, table, rowState, mappings, bulkCopyTimeout);
            return OracleExecution.MergeFromPseudoTable(connection, tableName, pseudoTableName, mergeFields, qualifierFields, transaction);
        }

        #endregion

        #endregion

        #region Async

        #region BulkMergeBaseAsync<TEntity>

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="qualifiers"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkMergeBaseAsync<TEntity>(this OracleConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            OracleBulkImportIdentityBehavior identityBehavior = default,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var identityField = dbFields.GetIdentity();
            var returnIdentity = identityBehavior == OracleBulkImportIdentityBehavior.ReturnIdentity && identityField != null;

            if (returnIdentity)
            {
                return await connection.BulkMergeBaseForReturnIdentityAsync(tableName,
                    entities,
                    qualifiers,
                    mappings,
                    bulkCopyTimeout,
                    identityBehavior,
                    pseudoTableType,
                    transaction,
                    cancellationToken);
            }
            else
            {
                return await connection.BulkMergeBaseNoReturnIdentityAsync(tableName,
                    entities,
                    qualifiers,
                    mappings,
                    bulkCopyTimeout,
                    pseudoTableType,
                    transaction,
                    cancellationToken);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="qualifiers"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException">
        /// Not implemented yet. This pass only covers the <see cref="OracleBulkImportIdentityBehavior.ReturnIdentity"/> == <c>false</c>
        /// path - see <see cref="BulkMergeBaseNoReturnIdentityAsync{TEntity}"/>.
        /// </exception>
        private static async Task<int> BulkMergeBaseForReturnIdentityAsync<TEntity>(this OracleConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers = null,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            OracleBulkImportIdentityBehavior identityBehavior = default,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Asynchronous counterpart of <see cref="BulkMergeBaseNoReturnIdentity{TEntity}"/> - see its remarks
        /// for the detailed behavior and caveats (identical here).
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="entities"></param>
        /// <param name="qualifiers"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkMergeBaseNoReturnIdentityAsync<TEntity>(this OracleConnection connection,
            string tableName,
            IEnumerable<TEntity> entities,
            IEnumerable<Field> qualifiers,
            IEnumerable<OracleBulkInsertMapItem> mappings,
            int? bulkCopyTimeout,
            OracleBulkImportPseudoTableType pseudoTableType,
            OracleTransaction transaction,
            CancellationToken cancellationToken)
            where TEntity : class
        {
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers);
            var mergeFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields);
            var pseudoTableName = OracleText.GetPseudoTableNameForMerge(tableName, pseudoTableType);

            await OracleExecution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, transaction, cancellationToken);
            await OracleExecution.TruncatePseudoTableAsync(connection, pseudoTableName, transaction, cancellationToken);
            await WriteToServer.WriteToServerAsyncInternal(connection, pseudoTableName, entities, mappings, bulkCopyTimeout, cancellationToken);
            return await OracleExecution.MergeFromPseudoTableAsync(connection, tableName, pseudoTableName, mergeFields, qualifierFields, transaction, cancellationToken);
        }

        #endregion

        #region BulkMergeBaseAsync<DataTable>

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="qualifiers"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private static async Task<int> BulkMergeBaseAsync(this OracleConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            OracleBulkImportIdentityBehavior identityBehavior = default,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var identityField = dbFields.GetIdentity();
            var returnIdentity = identityBehavior == OracleBulkImportIdentityBehavior.ReturnIdentity && identityField != null;

            if (returnIdentity)
            {
                return await connection.BulkMergeBaseForReturnIdentityAsync(tableName,
                    table,
                    qualifiers,
                    rowState,
                    mappings,
                    bulkCopyTimeout,
                    identityBehavior,
                    pseudoTableType,
                    transaction,
                    cancellationToken);
            }
            else
            {
                return await connection.BulkMergeBaseNoReturnIdentityAsync(tableName,
                    table,
                    qualifiers,
                    rowState,
                    mappings,
                    bulkCopyTimeout,
                    pseudoTableType,
                    transaction,
                    cancellationToken);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="qualifiers"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="identityBehavior"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException">
        /// Not implemented yet. This pass only covers the <see cref="OracleBulkImportIdentityBehavior.ReturnIdentity"/> == <c>false</c>
        /// path - see <see cref="BulkMergeBaseNoReturnIdentityAsync(OracleConnection, string, DataTable, IEnumerable{Field}, DataRowState?, IEnumerable{OracleBulkInsertMapItem}, int?, OracleBulkImportPseudoTableType, OracleTransaction, CancellationToken)"/>.
        /// </exception>
        private static async Task<int> BulkMergeBaseForReturnIdentityAsync(this OracleConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            OracleBulkImportIdentityBehavior identityBehavior = default,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Asynchronous counterpart of the <c>DataTable</c> <see cref="BulkMergeBaseNoReturnIdentity(OracleConnection, string, DataTable, IEnumerable{Field}, DataRowState?, IEnumerable{OracleBulkInsertMapItem}, int?, OracleBulkImportPseudoTableType, OracleTransaction)"/> -
        /// see its remarks for the detailed behavior and caveats (identical here).
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="qualifiers"></param>
        /// <param name="rowState"></param>
        /// <param name="mappings"></param>
        /// <param name="bulkCopyTimeout"></param>
        /// <param name="pseudoTableType"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<int> BulkMergeBaseNoReturnIdentityAsync(this OracleConnection connection,
            string tableName,
            DataTable table,
            IEnumerable<Field> qualifiers = null,
            DataRowState? rowState = null,
            IEnumerable<OracleBulkInsertMapItem> mappings = null,
            int? bulkCopyTimeout = null,
            OracleBulkImportPseudoTableType pseudoTableType = default,
            OracleTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            var qualifierFields = GetQualifierFields(tableName, dbFields, qualifiers);
            var mergeFields = GetMergeFields(tableName, dbFields, mappings, qualifierFields);
            var pseudoTableName = OracleText.GetPseudoTableNameForMerge(tableName, pseudoTableType);

            await OracleExecution.CreatePseudoTableAsync(connection, tableName, pseudoTableName, pseudoTableType, transaction, cancellationToken);
            await OracleExecution.TruncatePseudoTableAsync(connection, pseudoTableName, transaction, cancellationToken);
            await WriteToServer.WriteToServerAsyncInternal(connection, pseudoTableName, table, rowState, mappings, bulkCopyTimeout, cancellationToken);
            return await OracleExecution.MergeFromPseudoTableAsync(connection, tableName, pseudoTableName, mergeFields, qualifierFields, transaction, cancellationToken);
        }

        #endregion

        #endregion

        #region Helpers

        /// <summary>
        /// Resolves the field(s) used to match an existing row during the <c>MERGE</c> (the <c>ON</c> clause).
        /// Falls back to the table's primary key, then its identity key, when <paramref name="qualifiers"/>
        /// is not provided.
        /// </summary>
        /// <exception cref="PrimaryFieldNotFoundException">
        /// No <paramref name="qualifiers"/> were given, and the table has neither a primary nor an identity key.
        /// </exception>
        private static IEnumerable<Field> GetQualifierFields(string tableName,
            DbFieldCollection dbFields,
            IEnumerable<Field> qualifiers)
        {
            if (qualifiers?.Any() == true)
            {
                return qualifiers;
            }

            var primaryOrIdentity = dbFields?.GetPrimary() ?? dbFields?.GetIdentity();

            if (primaryOrIdentity == null)
            {
                throw new PrimaryFieldNotFoundException(
                    $"No primary or identity key found for table '{tableName}'. Provide explicit 'qualifiers' instead.");
            }

            return new[] { primaryOrIdentity.AsField() };
        }

        /// <summary>
        /// Resolves the full set of fields to stage and merge (both inserted and, where not a qualifier,
        /// updated). When <paramref name="mappings"/> is provided, only its destination columns - plus,
        /// always, <paramref name="qualifierFields"/> (needed for the <c>ON</c> clause regardless of
        /// whether they were explicitly mapped) - are kept.
        /// </summary>
        /// <exception cref="MissingFieldsException">The resulting field list is empty.</exception>
        private static IEnumerable<Field> GetMergeFields(string tableName,
            DbFieldCollection dbFields,
            IEnumerable<OracleBulkInsertMapItem> mappings,
            IEnumerable<Field> qualifierFields)
        {
            var fields = dbFields?.GetAsFields();

            if (mappings?.Any() == true)
            {
                fields = fields?.Where(field =>
                    mappings.Any(mapping => string.Equals(mapping.DestinationColumn, field.Name, StringComparison.OrdinalIgnoreCase)) ||
                    qualifierFields.Any(qualifier => string.Equals(qualifier.Name, field.Name, StringComparison.OrdinalIgnoreCase)));
            }

            if (fields?.Any() != true)
            {
                throw new MissingFieldsException($"There are no field(s) found for table '{tableName}' for this operation.");
            }

            return fields;
        }

        #endregion
    }
}
