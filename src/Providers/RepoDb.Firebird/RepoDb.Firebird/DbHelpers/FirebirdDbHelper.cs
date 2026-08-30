#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using FirebirdSql.Data.FirebirdClient;
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
    /// A helper class for database specially for the direct access. This class is only meant for Firebird.
    /// </summary>
    /// <remarks>
    /// Targets Firebird 3.0 and later - identity-column introspection relies on the
    /// RDB$RELATION_FIELDS.RDB$IDENTITY_TYPE/RDB$GENERATOR_NAME columns, which do not exist on Firebird 2.5
    /// and earlier. Tables whose auto-increment behavior is implemented the pre-3.0 way (a BEFORE INSERT
    /// trigger plus a bare RDB$GENERATOR/SEQUENCE) will not be detected as identity columns here.
    /// </remarks>
    public sealed class FirebirdDbHelper : IDbHelper
    {
        /// <summary>
        /// Creates a new instance of <see cref="FirebirdDbHelper"/> class.
        /// </summary>
        public FirebirdDbHelper()
            : this(new FirebirdDbTypeNameToClientTypeResolver())
        { }

        /// <summary>
        /// Creates a new instance of <see cref="FirebirdDbHelper"/> class.
        /// </summary>
        /// <param name="dbTypeResolver">The type resolver to be used.</param>
        public FirebirdDbHelper(IResolver<string, Type> dbTypeResolver)
        {
            DbTypeResolver = dbTypeResolver;
        }

        #region Properties

        /// <summary>
        /// Gets the type resolver used by this <see cref="FirebirdDbHelper"/> instance.
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
            return @"SELECT TRIM(rf.RDB$FIELD_NAME) AS ColumnName
                , CASE WHEN pk.RDB$FIELD_NAME IS NOT NULL THEN 1 ELSE 0 END AS IsPrimary
                , CASE WHEN rf.RDB$IDENTITY_TYPE IS NOT NULL THEN 1 ELSE 0 END AS IsIdentity
                , CASE WHEN COALESCE(rf.RDB$NULL_FLAG, f.RDB$NULL_FLAG, 0) = 1 THEN 0 ELSE 1 END AS IsNullable
                , f.RDB$FIELD_TYPE AS FieldType
                , f.RDB$FIELD_SUB_TYPE AS FieldSubType
                , f.RDB$CHARACTER_SET_ID AS CharacterSetId
                , CASE WHEN f.RDB$FIELD_TYPE = 261 THEN NULL
                    ELSE COALESCE(f.RDB$CHARACTER_LENGTH, f.RDB$FIELD_LENGTH) END AS ColumnSize
                , f.RDB$FIELD_PRECISION AS NumericPrecision
                , CASE WHEN f.RDB$FIELD_SCALE IS NULL THEN NULL ELSE (0 - f.RDB$FIELD_SCALE) END AS NumericScale
                , CASE WHEN rf.RDB$IDENTITY_TYPE IS NOT NULL
                    OR rf.RDB$DEFAULT_SOURCE IS NOT NULL
                    OR f.RDB$DEFAULT_SOURCE IS NOT NULL THEN 1 ELSE 0 END AS HasDefaultValue
            FROM RDB$RELATION_FIELDS rf
            INNER JOIN RDB$FIELDS f
                ON f.RDB$FIELD_NAME = rf.RDB$FIELD_SOURCE
            LEFT JOIN (
                SELECT TRIM(s.RDB$FIELD_NAME) AS RDB$FIELD_NAME
                FROM RDB$RELATION_CONSTRAINTS rc
                INNER JOIN RDB$INDEX_SEGMENTS s
                    ON s.RDB$INDEX_NAME = rc.RDB$INDEX_NAME
                WHERE rc.RDB$CONSTRAINT_TYPE = 'PRIMARY KEY'
                    AND TRIM(rc.RDB$RELATION_NAME) = @TableName
            ) pk
                ON pk.RDB$FIELD_NAME = TRIM(rf.RDB$FIELD_NAME)
            WHERE TRIM(rf.RDB$RELATION_NAME) = @TableName
            ORDER BY rf.RDB$FIELD_POSITION;";
        }

        /// <summary>
        /// Maps a Firebird RDB$FIELD_TYPE/RDB$FIELD_SUB_TYPE/RDB$CHARACTER_SET_ID triple into the canonical
        /// type-name strings consumed by <see cref="FirebirdDbTypeNameToClientTypeResolver"/>. RDB$FIELD_TYPE
        /// codes are not exposed as named constants by the ADO.NET provider, so the raw integers (per the
        /// Firebird engine's internal blr type codes) are matched directly.
        /// </summary>
        /// <param name="fieldType"></param>
        /// <param name="subType"></param>
        /// <param name="characterSetId"></param>
        /// <returns></returns>
        private string ResolveColumnTypeName(short fieldType,
            short? subType,
            short? characterSetId)
        {
            // CHARACTER_SET_ID 1 == OCTETS, i.e. a CHAR/VARCHAR declared as a binary string.
            var isOctets = characterSetId == 1;

            return fieldType switch
            {
                7 => "smallint",
                8 => "integer",
                10 => "float",
                12 => "date",
                13 => "time",
                14 => isOctets ? "binary" : "char",
                16 => subType switch
                {
                    1 => "numeric",
                    2 => "decimal",
                    _ => "bigint",
                },
                23 => "boolean",
                24 => "dec16",
                25 => "dec34",
                26 => subType switch
                {
                    1 => "numeric",
                    2 => "decimal",
                    _ => "int128",
                },
                27 => "double precision",
                28 => "time_tz",
                29 => "timestamp_tz",
                35 => "timestamp",
                37 => isOctets ? "varbinary" : "varchar",
                261 => subType == 1 ? "blob_text" : "blob_binary",
                _ => "none",
            };
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private DbField ReaderToDbField(DbDataReader reader)
        {
            var fieldType = reader.GetInt16(4);
            var subType = reader.IsDBNull(5) ? (short?)null : reader.GetInt16(5);
            var characterSetId = reader.IsDBNull(6) ? (short?)null : reader.GetInt16(6);
            var columnType = ResolveColumnTypeName(fieldType, subType, characterSetId);

            return new DbField(reader.GetString(0),
                reader.GetBoolean(1),
                reader.GetBoolean(2),
                reader.GetBoolean(3),
                DbTypeResolver.Resolve(columnType),
                reader.IsDBNull(7) ? (int?)null : reader.GetInt32(7),
                reader.IsDBNull(8) ? (byte?)null : byte.Parse(reader.GetInt16(8).ToString()),
                reader.IsDBNull(9) ? (byte?)null : byte.Parse(reader.GetInt16(9).ToString()),
                columnType,
                reader.GetBoolean(10),
                "FIREBIRD");
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
            var fieldType = await reader.GetFieldValueAsync<short>(4, cancellationToken);
            var subType = await reader.IsDBNullAsync(5, cancellationToken) ? (short?)null :
                await reader.GetFieldValueAsync<short>(5, cancellationToken);
            var characterSetId = await reader.IsDBNullAsync(6, cancellationToken) ? (short?)null :
                await reader.GetFieldValueAsync<short>(6, cancellationToken);
            var columnType = ResolveColumnTypeName(fieldType, subType, characterSetId);

            return new DbField(await reader.GetFieldValueAsync<string>(0, cancellationToken),
                await reader.GetFieldValueAsync<bool>(1, cancellationToken),
                await reader.GetFieldValueAsync<bool>(2, cancellationToken),
                await reader.GetFieldValueAsync<bool>(3, cancellationToken),
                DbTypeResolver.Resolve(columnType),
                await reader.IsDBNullAsync(7, cancellationToken) ? (int?)null : await reader.GetFieldValueAsync<int>(7, cancellationToken),
                await reader.IsDBNullAsync(8, cancellationToken) ? null : byte.Parse((await reader.GetFieldValueAsync<short>(8, cancellationToken)).ToString()),
                await reader.IsDBNullAsync(9, cancellationToken) ? null : byte.Parse((await reader.GetFieldValueAsync<short>(9, cancellationToken)).ToString()),
                columnType,
                await reader.GetFieldValueAsync<bool>(10, cancellationToken),
                "FIREBIRD");
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
            cancellationToken.ThrowIfCancellationRequested();

            // Variables
            var commandText = GetCommandText();
            var setting = connection.GetDbSetting();
            var param = new
            {
                TableName = DataEntityExtension.GetTableName(tableName, setting).AsUnquoted(setting)
            };

            // Iterate and extract
            using var reader = (DbDataReader)await connection.ExecuteReaderAsync(commandText, param, transaction: transaction,
                cancellationToken: cancellationToken);

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
        /// Firebird has no session-wide "last identity" construct equivalent to SQL Server's SCOPE_IDENTITY()
        /// or MySQL's LAST_INSERT_ID() - identity/generator values are scoped per-generator, not per-session.
        /// Use the identity value returned directly by the Insert/Merge operations (via the RETURNING clause)
        /// instead of this method.
        /// </summary>
        /// <typeparam name="T">The type of newly generated identity.</typeparam>
        /// <param name="connection">The instance of the connection object.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>The newly generated identity from the database.</returns>
        public T GetScopeIdentity<T>(IDbConnection connection,
            IDbTransaction transaction = null) =>
            throw new NotSupportedException("Firebird has no session-wide scope identity. The generated key " +
                "is already returned by the Insert/Merge operations via the RETURNING clause; query the " +
                "underlying generator explicitly (e.g. via GEN_ID(generator_name, 0), found in " +
                "RDB$RELATION_FIELDS.RDB$GENERATOR_NAME) if you need it out-of-band.");

        /// <summary>
        /// Firebird has no session-wide "last identity" construct equivalent to SQL Server's SCOPE_IDENTITY()
        /// or MySQL's LAST_INSERT_ID() - identity/generator values are scoped per-generator, not per-session.
        /// Use the identity value returned directly by the Insert/Merge operations (via the RETURNING clause)
        /// instead of this method.
        /// </summary>
        /// <typeparam name="T">The type of newly generated identity.</typeparam>
        /// <param name="connection">The instance of the connection object.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The newly generated identity from the database.</returns>
        public Task<T> GetScopeIdentityAsync<T>(IDbConnection connection,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException("Firebird has no session-wide scope identity. The generated key " +
                "is already returned by the Insert/Merge operations via the RETURNING clause; query the " +
                "underlying generator explicitly (e.g. via GEN_ID(generator_name, 0), found in " +
                "RDB$RELATION_FIELDS.RDB$GENERATOR_NAME) if you need it out-of-band.");

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
                HandleDbParameterPostCreation((FbParameter)(object)instance);
            }
        }

        #region Handlers

        /// <summary>
        ///
        /// </summary>
        /// <param name="parameter"></param>
        private void HandleDbParameterPostCreation(FbParameter parameter)
        {
            // Do nothing for now
        }

        #endregion

        #endregion

        #endregion
    }
}
