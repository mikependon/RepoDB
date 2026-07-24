using Oracle.ManagedDataAccess.Client;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Resolvers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb.DbHelpers
{
    /// <summary>
    /// A helper class for database specially for the direct access. This class is only meant for Oracle.
    /// </summary>
    public sealed class OracleDbHelper : IDbHelper
    {
        /// <summary>
        /// Creates a new instance of <see cref="OracleDbHelper"/> class.
        /// </summary>
        public OracleDbHelper()
            : this(new OracleDbTypeNameToClientTypeResolver())
        { }

        /// <summary>
        /// Creates a new instance of <see cref="OracleDbHelper"/> class.
        /// </summary>
        /// <param name="dbTypeResolver">The type resolver to be used.</param>
        public OracleDbHelper(IResolver<string, Type> dbTypeResolver)
        {
            DbTypeResolver = dbTypeResolver;
        }

        #region Properties

        /// <summary>
        /// Gets the type resolver used by this <see cref="OracleDbHelper"/> instance.
        /// </summary>
        public IResolver<string, Type> DbTypeResolver { get; }

        #endregion

        #region Helpers

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        private string GetCommandText()
        {
            // ALL_TAB_COLUMNS/ALL_CONSTRAINTS/ALL_TAB_IDENTITY_COLS store unquoted identifiers in
            // the case they were created in (uppercase by default). The schema falls back to the
            // session's current schema when none is specified on the mapped table name (Oracle has
            // no fixed default schema the way SQL Server has 'dbo').
            return @"
                SELECT C.COLUMN_NAME AS ColumnName
                    , CASE WHEN PK.COLUMN_NAME IS NOT NULL THEN 1 ELSE 0 END AS IsPrimary
                    , CASE WHEN IC.COLUMN_NAME IS NOT NULL THEN 1 ELSE 0 END AS IsIdentity
                    , CASE WHEN C.NULLABLE = 'Y' THEN 1 ELSE 0 END AS IsNullable
                    , C.DATA_TYPE AS DataType
                    , COALESCE(C.CHAR_LENGTH, C.DATA_LENGTH, 0) AS Size
                    , C.DATA_PRECISION AS Precision
                    , C.DATA_SCALE AS Scale
                    , CASE WHEN C.DATA_DEFAULT IS NOT NULL THEN 1 ELSE 0 END AS HasDefaultValue
                FROM ALL_TAB_COLUMNS C
                LEFT JOIN (
                    SELECT COLS.OWNER, COLS.TABLE_NAME, COLS.COLUMN_NAME
                    FROM ALL_CONSTRAINTS CONS
                    INNER JOIN ALL_CONS_COLUMNS COLS
                        ON COLS.OWNER = CONS.OWNER
                        AND COLS.CONSTRAINT_NAME = CONS.CONSTRAINT_NAME
                        AND COLS.TABLE_NAME = CONS.TABLE_NAME
                    WHERE CONS.CONSTRAINT_TYPE = 'P'
                ) PK
                    ON PK.OWNER = C.OWNER
                    AND PK.TABLE_NAME = C.TABLE_NAME
                    AND PK.COLUMN_NAME = C.COLUMN_NAME
                LEFT JOIN ALL_TAB_IDENTITY_COLS IC
                    ON IC.OWNER = C.OWNER
                    AND IC.TABLE_NAME = C.TABLE_NAME
                    AND IC.COLUMN_NAME = C.COLUMN_NAME
                WHERE C.OWNER = COALESCE(:Schema, SYS_CONTEXT('USERENV', 'CURRENT_SCHEMA'))
                    AND C.TABLE_NAME = :TableName
                ORDER BY C.COLUMN_ID";
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private DbField ReaderToDbField(DbDataReader reader)
        {
            return new DbField(reader.GetString(0),
                !reader.IsDBNull(1) && reader.GetInt32(1) == 1,
                !reader.IsDBNull(2) && reader.GetInt32(2) == 1,
                !reader.IsDBNull(3) && reader.GetInt32(3) == 1,
                reader.IsDBNull(4) ? DbTypeResolver.Resolve("varchar2") : DbTypeResolver.Resolve(reader.GetString(4)),
                reader.IsDBNull(5) ? 0 : Convert.ToInt32(reader.GetDecimal(5)),
                reader.IsDBNull(6) ? (byte?)0 : Convert.ToByte(reader.GetDecimal(6)),
                reader.IsDBNull(7) ? (byte?)0 : Convert.ToByte(reader.GetDecimal(7)),
                reader.IsDBNull(4) ? "varchar2" : reader.GetString(4),
                !reader.IsDBNull(8) && reader.GetInt32(8) == 1,
                "ORACLE");
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DbField> ReaderToDbFieldAsync(DbDataReader reader,
            CancellationToken cancellationToken = default)
        {
            return new DbField(await reader.GetFieldValueAsync<string>(0, cancellationToken),
                !await reader.IsDBNullAsync(1, cancellationToken) && await reader.GetFieldValueAsync<int>(1, cancellationToken) == 1,
                !await reader.IsDBNullAsync(2, cancellationToken) && await reader.GetFieldValueAsync<int>(2, cancellationToken) == 1,
                !await reader.IsDBNullAsync(3, cancellationToken) && await reader.GetFieldValueAsync<int>(3, cancellationToken) == 1,
                await reader.IsDBNullAsync(4, cancellationToken) ? DbTypeResolver.Resolve("varchar2") : DbTypeResolver.Resolve(await reader.GetFieldValueAsync<string>(4, cancellationToken)),
                await reader.IsDBNullAsync(5, cancellationToken) ? 0 : Convert.ToInt32(await reader.GetFieldValueAsync<decimal>(5, cancellationToken)),
                await reader.IsDBNullAsync(6, cancellationToken) ? (byte?)0 : Convert.ToByte(await reader.GetFieldValueAsync<decimal>(6, cancellationToken)),
                await reader.IsDBNullAsync(7, cancellationToken) ? (byte?)0 : Convert.ToByte(await reader.GetFieldValueAsync<decimal>(7, cancellationToken)),
                await reader.IsDBNullAsync(4, cancellationToken) ? "varchar2" : await reader.GetFieldValueAsync<string>(4, cancellationToken),
                !await reader.IsDBNullAsync(8, cancellationToken) && await reader.GetFieldValueAsync<int>(8, cancellationToken) == 1,
                "ORACLE");
        }

        #endregion

        #region Methods

        #region GetFields

        /// <summary>
        /// Gets the list of <see cref="DbField"/> of the table.
        /// </summary>
        /// <param name="connection">The instance of the connection object.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>A list of <see cref="DbField"/> of the target table.</returns>
        public IEnumerable<DbField> GetFields(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null)
        {
            // Variables
            var commandText = GetCommandText();
            var setting = connection.GetDbSetting();
            var param = new
            {
                Schema = DataEntityExtension.GetSchema(tableName, setting).AsUnquoted(setting),
                TableName = DataEntityExtension.GetTableName(tableName, setting).AsUnquoted(setting)
            };

            // Iterate and extract
            using var reader = (DbDataReader)connection.ExecuteReader(commandText, param, transaction: transaction);

            var dbFields = new List<DbField>();

            // Iterate the list of the fields
            while (reader.Read())
            {
                dbFields.Add(ReaderToDbField(reader));
            }

            // Return the list of fields
            return dbFields;
        }

        /// <summary>
        /// Gets the list of <see cref="DbField"/> of the table in an asynchronous way.
        /// </summary>
        /// <param name="connection">The instance of the connection object.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A list of <see cref="DbField"/> of the target table.</returns>
        public async Task<IEnumerable<DbField>> GetFieldsAsync(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            // Variables
            var commandText = GetCommandText();
            var setting = connection.GetDbSetting();
            var param = new
            {
                Schema = DataEntityExtension.GetSchema(tableName, setting).AsUnquoted(setting),
                TableName = DataEntityExtension.GetTableName(tableName, setting).AsUnquoted(setting)
            };

            // Iterate and extract
            using var reader = (DbDataReader)await connection.ExecuteReaderAsync(commandText, param,
                transaction: transaction, cancellationToken: cancellationToken);

            var dbFields = new List<DbField>();

            // Iterate the list of the fields
            while (await reader.ReadAsync(cancellationToken))
            {
                dbFields.Add(await ReaderToDbFieldAsync(reader, cancellationToken));
            }

            // Return the list of fields
            return dbFields;
        }

        #endregion

        #region GetScopeIdentity

        /// <summary>
        /// Gets the newly generated identity from the database. Oracle has no session-wide
        /// "last identity" construct equivalent to SQL Server's SCOPE_IDENTITY() — identity/sequence
        /// values are scoped per-sequence, not per-session. Use the identity value returned directly
        /// by the Insert/Merge operations (via the RETURNING mechanism) instead of this method.
        /// </summary>
        /// <typeparam name="T">The type of newly generated identity.</typeparam>
        /// <param name="connection">The instance of the connection object.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>The newly generated identity from the database.</returns>
        public T GetScopeIdentity<T>(IDbConnection connection,
            IDbTransaction transaction = null) =>
            throw new NotSupportedException("Oracle has no session-wide scope identity. The generated " +
                "key is already returned by the Insert/Merge operations; query the underlying sequence " +
                "explicitly (e.g. via ALL_TAB_IDENTITY_COLS.SEQUENCE_NAME) if you need it out-of-band.");

        /// <summary>
        /// Gets the newly generated identity from the database in an asynchronous way. Oracle has no
        /// session-wide "last identity" construct equivalent to SQL Server's SCOPE_IDENTITY() — identity/
        /// sequence values are scoped per-sequence, not per-session. Use the identity value returned
        /// directly by the Insert/Merge operations (via the RETURNING mechanism) instead of this method.
        /// </summary>
        /// <typeparam name="T">The type of newly generated identity.</typeparam>
        /// <param name="connection">The instance of the connection object.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The newly generated identity from the database.</returns>
        public Task<T> GetScopeIdentityAsync<T>(IDbConnection connection,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException("Oracle has no session-wide scope identity. The generated " +
                "key is already returned by the Insert/Merge operations; query the underlying sequence " +
                "explicitly (e.g. via ALL_TAB_IDENTITY_COLS.SEQUENCE_NAME) if you need it out-of-band.");

        #endregion

        #region DynamicHandler

        /// <summary>
        /// A backdoor access from the core library used to handle an instance of an object to whatever purpose within the extended library.
        /// </summary>
        /// <typeparam name="TEventInstance">The type of the event instance to handle.</typeparam>
        /// <param name="instance">The instance of the event object to handle.</param>
        /// <param name="key">The key of the event to handle.</param>
        public void DynamicHandler<TEventInstance>(TEventInstance instance,
            string key)
        {
            if (key == "RepoDb.Internal.Compiler.Events[AfterCreateDbParameter]")
            {
                HandleDbParameterPostCreation((OracleParameter)(object)instance);
            }
        }

        #region Handlers

        /// <summary>
        ///
        /// </summary>
        /// <param name="parameter"></param>
        private void HandleDbParameterPostCreation(OracleParameter parameter)
        {
            // Do nothing for now
        }

        #endregion

        #endregion

        #endregion
    }
}
