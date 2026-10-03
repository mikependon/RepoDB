#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Data;

namespace RepoDb.Data
{
    /// <summary>
    /// A data reader that wraps a source reader and passes it to an <see cref="ICopyDataInterceptor"/>
    /// every time a row is read.
    /// </summary>
    internal sealed class CopyDataReader : IDataReader
    {
        #region Private Variables

        private readonly IDataReader _source;
        private readonly ICopyDataInterceptor _interceptor;
        private readonly CopyDataInterceptorOptions _options = new CopyDataInterceptorOptions();

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="CopyDataReader"/> class.
        /// </summary>
        /// <param name="source">The source data reader.</param>
        /// <param name="interceptor">The data interceptor.</param>
        public CopyDataReader(IDataReader source,
            ICopyDataInterceptor interceptor)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _interceptor = interceptor ?? throw new ArgumentNullException(nameof(interceptor));
        }

        #endregion

        #region Public Methods

        #region IDataReader

        public int Depth => _source.Depth;

        public bool IsClosed => _source.IsClosed;

        public int RecordsAffected => _source.RecordsAffected;

        public void Close() => _source.Close();

        public DataTable GetSchemaTable() => _source.GetSchemaTable();

        public bool NextResult() => _source.NextResult();

        public bool Read()
        {
            if (_source.Read() == false)
            {
                return false;
            }
            _interceptor.Intercept(_source, _options);
            return true;
        }

        public void Dispose() => _source.Dispose();

        #endregion

        #region IDataRecord

        public int FieldCount => _source.FieldCount;

        public object this[int i] => _source[i];

        public object this[string name] => _source[name];

        public bool GetBoolean(int i) => _source.GetBoolean(i);

        public byte GetByte(int i) => _source.GetByte(i);

        public char GetChar(int i) => _source.GetChar(i);

        public Guid GetGuid(int i) => _source.GetGuid(i);

        public short GetInt16(int i) => _source.GetInt16(i);

        public int GetInt32(int i) => _source.GetInt32(i);

        public long GetInt64(int i) => _source.GetInt64(i);

        public float GetFloat(int i) => _source.GetFloat(i);

        public double GetDouble(int i) => _source.GetDouble(i);

        public string GetString(int i) => _source.GetString(i);

        public decimal GetDecimal(int i) => _source.GetDecimal(i);

        public DateTime GetDateTime(int i) => _source.GetDateTime(i);

        public object GetValue(int i) => _source.GetValue(i);

        public bool IsDBNull(int i) => _source.IsDBNull(i);

        public string GetName(int i) => _source.GetName(i);

        public string GetDataTypeName(int i) => _source.GetDataTypeName(i);

        public Type GetFieldType(int i) => _source.GetFieldType(i);

        public int GetOrdinal(string name) => _source.GetOrdinal(name);

        public IDataReader GetData(int i) => _source.GetData(i);

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length) =>
            _source.GetBytes(i, fieldOffset, buffer, bufferoffset, length);

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length) =>
            _source.GetChars(i, fieldoffset, buffer, bufferoffset, length);

        public int GetValues(object[] values) => _source.GetValues(values);

        #endregion

        #endregion
    }
}
