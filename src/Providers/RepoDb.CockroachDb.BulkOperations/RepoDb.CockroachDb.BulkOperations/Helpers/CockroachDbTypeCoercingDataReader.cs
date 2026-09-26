#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using RepoDb.Connector.CockroachDb;
using RepoDb.Connector.CockroachDb.Bulk;
using RepoDb.Extensions;

namespace RepoDb.CockroachDb.BulkOperations.Extensions
{
    /// <summary>
    /// A forward-only <see cref="IDataReader"/> that converts every mapped value to the .NET type of its
    /// destination column before it reaches <see cref="CockroachDbBulkCopy"/>.
    /// </summary>
    /// <remarks>
    /// <see cref="CockroachDbBulkCopy"/> writes rows through Npgsql's binary <c>COPY</c> without explicit
    /// types, so the wire type is inferred from each CLR value. CockroachDB's <c>INT</c>/<c>INTEGER</c> is a
    /// 64-bit <c>INT8</c>, so an <see cref="int"/> property sent as a 4-byte <c>INT4</c> datum is rejected
    /// with <c>42601: read binary tuple: decode datum as INT8</c>. Coercing the values to the destination
    /// field type first makes Npgsql emit the width the server expects.
    /// </remarks>
    internal sealed class CockroachDbTypeCoercingDataReader : IDataReader
    {
        private readonly IDataReader _reader;
        private readonly DataRow[] _rows;
        private readonly DataColumnCollection _columns;
        private readonly Type[] _targetTypes;
        private int _rowIndex = -1;

        private CockroachDbTypeCoercingDataReader(IDataReader reader,
            DataRow[] rows,
            Type[] targetTypes)
        {
            _reader = reader;
            _rows = rows;
            _columns = rows?.Length > 0 ? rows[0].Table.Columns : null;
            _targetTypes = targetTypes;
        }

        #region Factory

        /// <summary>
        /// Wraps a data reader so its values match the destination table's column types.
        /// </summary>
        public static IDataReader Create(CockroachDbConnection connection,
            string tableName,
            CockroachDbBulkCopy bulkCopy,
            IDataReader reader,
            CockroachDbTransaction transaction = null) =>
            new CockroachDbTypeCoercingDataReader(reader, null,
                GetTargetTypes(connection, tableName, bulkCopy, reader.FieldCount, transaction));

        /// <summary>
        /// Exposes data rows as a data reader whose values match the destination table's column types.
        /// </summary>
        public static IDataReader Create(CockroachDbConnection connection,
            string tableName,
            CockroachDbBulkCopy bulkCopy,
            DataRow[] rows,
            CockroachDbTransaction transaction = null) =>
            new CockroachDbTypeCoercingDataReader(null, rows ?? Array.Empty<DataRow>(),
                GetTargetTypes(connection, tableName, bulkCopy, rows?.Length > 0 ? rows[0].Table.Columns.Count : 0, transaction));

        private static Type[] GetTargetTypes(CockroachDbConnection connection,
            string tableName,
            CockroachDbBulkCopy bulkCopy,
            int fieldCount,
            CockroachDbTransaction transaction)
        {
            var targetTypes = new Type[fieldCount];
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            if (dbFields == null)
            {
                return targetTypes;
            }

            var dbSetting = connection.GetDbSetting();
            foreach (CockroachDbBulkColumnMapping mapping in bulkCopy.ColumnMappings)
            {
                if (mapping.SourceOrdinal < 0 || mapping.SourceOrdinal >= fieldCount || string.IsNullOrEmpty(mapping.DestinationColumn))
                {
                    continue;
                }
                var dbField = dbFields.GetByUnquotedName(mapping.DestinationColumn.AsUnquoted(true, dbSetting));
                if (dbField?.Type != null)
                {
                    targetTypes[mapping.SourceOrdinal] = Nullable.GetUnderlyingType(dbField.Type) ?? dbField.Type;
                }
            }

            return targetTypes;
        }

        #endregion

        #region Coercion

        internal static object Coerce(object value,
            Type targetType)
        {
            if (value == null || value is DBNull || targetType == null)
            {
                return value;
            }

            var sourceType = value.GetType();
            if (sourceType == targetType)
            {
                return value;
            }

            if (targetType == typeof(string) && value is Guid guid)
            {
                return guid.ToString();
            }
            if (targetType == typeof(Guid) && value is string text && Guid.TryParse(text, out var parsed))
            {
                return parsed;
            }
            if (IsNumeric(targetType) && (IsNumeric(sourceType) || sourceType.IsEnum || sourceType == typeof(bool)))
            {
                return Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
            }

            return value;
        }

        private static bool IsNumeric(Type type)
        {
            var code = Type.GetTypeCode(type);
            return code is TypeCode.SByte or TypeCode.Byte or TypeCode.Int16 or TypeCode.UInt16
                or TypeCode.Int32 or TypeCode.UInt32 or TypeCode.Int64 or TypeCode.UInt64
                or TypeCode.Single or TypeCode.Double or TypeCode.Decimal;
        }

        #endregion

        #region IDataReader

        public int FieldCount => _reader?.FieldCount ?? _columns?.Count ?? 0;

        public object this[int i] => GetValue(i);

        public object this[string name] => GetValue(GetOrdinal(name));

        public int Depth => _reader?.Depth ?? 0;

        public bool IsClosed => _reader?.IsClosed ?? _rowIndex >= _rows.Length;

        public int RecordsAffected => _reader?.RecordsAffected ?? -1;

        public bool Read() => _reader?.Read() ?? ++_rowIndex < _rows.Length;

        public bool NextResult() => _reader?.NextResult() ?? false;

        public void Close() => _reader?.Close();

        public void Dispose()
        {
            // The wrapped reader is owned (and disposed) by the caller.
        }

        public DataTable GetSchemaTable() => _reader?.GetSchemaTable();

        public string GetName(int i) => _reader?.GetName(i) ?? _columns[i].ColumnName;

        public int GetOrdinal(string name) => _reader?.GetOrdinal(name) ?? _columns.IndexOf(name);

        public Type GetFieldType(int i) => _targetTypes[i] ?? _reader?.GetFieldType(i) ?? _columns[i].DataType;

        public string GetDataTypeName(int i) => GetFieldType(i).Name;

        public object GetValue(int i) => Coerce(GetRawValue(i), _targetTypes[i]);

        public int GetValues(object[] values)
        {
            var count = Math.Min(values.Length, FieldCount);
            for (var i = 0; i < count; i++)
            {
                values[i] = GetValue(i);
            }
            return count;
        }

        public bool IsDBNull(int i)
        {
            var value = GetRawValue(i);
            return value == null || value is DBNull;
        }

        public bool GetBoolean(int i) => (bool)GetValue(i);

        public byte GetByte(int i) => (byte)GetValue(i);

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length) =>
            _reader?.GetBytes(i, fieldOffset, buffer, bufferoffset, length) ?? throw new NotSupportedException();

        public char GetChar(int i) => (char)GetValue(i);

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length) =>
            _reader?.GetChars(i, fieldoffset, buffer, bufferoffset, length) ?? throw new NotSupportedException();

        public IDataReader GetData(int i) => throw new NotSupportedException();

        public DateTime GetDateTime(int i) => (DateTime)GetValue(i);

        public decimal GetDecimal(int i) => (decimal)GetValue(i);

        public double GetDouble(int i) => (double)GetValue(i);

        public float GetFloat(int i) => (float)GetValue(i);

        public Guid GetGuid(int i) => (Guid)GetValue(i);

        public short GetInt16(int i) => (short)GetValue(i);

        public int GetInt32(int i) => (int)GetValue(i);

        public long GetInt64(int i) => (long)GetValue(i);

        public string GetString(int i) => (string)GetValue(i);

        private object GetRawValue(int i) => _reader != null ? _reader.GetValue(i) : _rows[_rowIndex][i];

        #endregion
    }
}
