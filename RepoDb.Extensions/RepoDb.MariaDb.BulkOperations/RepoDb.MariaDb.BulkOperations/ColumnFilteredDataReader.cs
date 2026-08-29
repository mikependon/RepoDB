using System;
using System.Data;

namespace RepoDb.MariaDb.BulkOperations
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class ColumnFilteredDataReader : IDataReader
    {
        private readonly IDataReader _inner;
        private readonly int[] _sourceOrdinals;

        public ColumnFilteredDataReader(IDataReader inner,
            int[] sourceOrdinals)
        {
            _inner = inner;
            _sourceOrdinals = sourceOrdinals;
        }

        public bool Read() => _inner.Read();

        public int FieldCount => _sourceOrdinals.Length;
        public string GetName(int i) => _inner.GetName(_sourceOrdinals[i]);
        public string GetDataTypeName(int i) => _inner.GetDataTypeName(_sourceOrdinals[i]);
        public Type GetFieldType(int i) => _inner.GetFieldType(_sourceOrdinals[i]);
        public object GetValue(int i) => _inner.GetValue(_sourceOrdinals[i]);

        public int GetValues(object[] values)
        {
            var count = Math.Min(_sourceOrdinals.Length, values.Length);
            for (var i = 0; i < count; i++)
            {
                values[i] = GetValue(i);
            }
            return count;
        }
        public int GetOrdinal(string name)
        {
            var innerOrdinal = _inner.GetOrdinal(name);
            return Array.IndexOf(_sourceOrdinals, innerOrdinal);
        }
        public bool GetBoolean(int i) => _inner.GetBoolean(_sourceOrdinals[i]);
        public byte GetByte(int i) => _inner.GetByte(_sourceOrdinals[i]);
        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length) =>
            _inner.GetBytes(_sourceOrdinals[i], fieldOffset, buffer, bufferoffset, length);
        public char GetChar(int i) => _inner.GetChar(_sourceOrdinals[i]);
        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length) =>
            _inner.GetChars(_sourceOrdinals[i], fieldoffset, buffer, bufferoffset, length);
        public Guid GetGuid(int i) => _inner.GetGuid(_sourceOrdinals[i]);
        public short GetInt16(int i) => _inner.GetInt16(_sourceOrdinals[i]);
        public int GetInt32(int i) => _inner.GetInt32(_sourceOrdinals[i]);
        public long GetInt64(int i) => _inner.GetInt64(_sourceOrdinals[i]);
        public float GetFloat(int i) => _inner.GetFloat(_sourceOrdinals[i]);
        public double GetDouble(int i) => _inner.GetDouble(_sourceOrdinals[i]);
        public string GetString(int i) => _inner.GetString(_sourceOrdinals[i]);
        public decimal GetDecimal(int i) => _inner.GetDecimal(_sourceOrdinals[i]);
        public DateTime GetDateTime(int i) => _inner.GetDateTime(_sourceOrdinals[i]);
        public IDataReader GetData(int i) => _inner.GetData(_sourceOrdinals[i]);
        public bool IsDBNull(int i) => _inner.IsDBNull(_sourceOrdinals[i]);
        public object this[int i] => GetValue(i);
        public object this[string name] => GetValue(GetOrdinal(name));
        public int Depth => _inner.Depth;
        public bool IsClosed => _inner.IsClosed;
        public int RecordsAffected => _inner.RecordsAffected;
        public void Close() => _inner.Close();
        public DataTable GetSchemaTable() => _inner.GetSchemaTable();
        public bool NextResult() => _inner.NextResult();

        public void Dispose()
        {
        }
    }
}
