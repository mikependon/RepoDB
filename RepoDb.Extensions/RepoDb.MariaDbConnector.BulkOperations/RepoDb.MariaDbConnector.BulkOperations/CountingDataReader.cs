using System;
using System.Data;

namespace RepoDb.MariaDbConnector.BulkOperations
{
    internal sealed class CountingDataReader : IDataReader
    {
        private readonly IDataReader _inner;

        public CountingDataReader(IDataReader inner) => _inner = inner;

        public int Count { get; private set; }

        public bool Read()
        {
            var read = _inner.Read();
            if (read)
            {
                Count++;
            }
            return read;
        }

        public int FieldCount => _inner.FieldCount;
        public string GetName(int i) => _inner.GetName(i);
        public string GetDataTypeName(int i) => _inner.GetDataTypeName(i);
        public Type GetFieldType(int i) => _inner.GetFieldType(i);
        public object GetValue(int i) => _inner.GetValue(i);
        public int GetValues(object[] values) => _inner.GetValues(values);
        public int GetOrdinal(string name) => _inner.GetOrdinal(name);
        public bool GetBoolean(int i) => _inner.GetBoolean(i);
        public byte GetByte(int i) => _inner.GetByte(i);
        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length) =>
            _inner.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        public char GetChar(int i) => _inner.GetChar(i);
        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length) =>
            _inner.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        public Guid GetGuid(int i) => _inner.GetGuid(i);
        public short GetInt16(int i) => _inner.GetInt16(i);
        public int GetInt32(int i) => _inner.GetInt32(i);
        public long GetInt64(int i) => _inner.GetInt64(i);
        public float GetFloat(int i) => _inner.GetFloat(i);
        public double GetDouble(int i) => _inner.GetDouble(i);
        public string GetString(int i) => _inner.GetString(i);
        public decimal GetDecimal(int i) => _inner.GetDecimal(i);
        public DateTime GetDateTime(int i) => _inner.GetDateTime(i);
        public IDataReader GetData(int i) => _inner.GetData(i);
        public bool IsDBNull(int i) => _inner.IsDBNull(i);
        public object this[int i] => _inner[i];
        public object this[string name] => _inner[name];
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
