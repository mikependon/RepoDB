using System;
using System.Data;

namespace RepoDb.Oracle.BulkOperations
{
    /// <summary>
    /// A minimal read-through <see cref="IDataReader"/> decorator that counts how many rows were
    /// actually enumerated via <see cref="Read"/>. <see cref="Oracle.ManagedDataAccess.Client.OracleBulkCopy"/>
    /// does not expose a rows-written count of its own the way the in-memory <c>TEntity</c>/<see cref="DataTable"/>
    /// overloads already know their row count up front (<c>entities.Count()</c>/<c>rows.Length</c>) -
    /// wrapping the source reader lets the counter reflect exactly what
    /// <see cref="Oracle.ManagedDataAccess.Client.OracleBulkCopy.WriteToServer(IDataReader)"/> pulled
    /// through, with no need to buffer or pre-enumerate the (potentially large, streaming) source. Used by
    /// the <c>DbDataReader</c> overloads in <c>Base/WriteToServer.cs</c>.
    /// </summary>
    /// <remarks>
    /// Deliberately does not own - and therefore does not dispose - the wrapped <paramref name="inner"/>
    /// reader on <see cref="Dispose"/>. A caller-supplied <see cref="System.Data.Common.DbDataReader"/>
    /// (e.g. the live result of a streaming <c>SELECT</c> against a source connection) is the caller's to
    /// manage; this decorator only exists to count, not to take over its lifetime.
    /// </remarks>
    internal sealed class CountingDataReader : IDataReader
    {
        private readonly IDataReader _inner;

        public CountingDataReader(IDataReader inner) => _inner = inner;

        /// <summary>
        /// The number of rows successfully pulled through <see cref="Read"/> so far.
        /// </summary>
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

        /// <summary>
        /// Intentionally a no-op - see the remarks on <see cref="CountingDataReader"/>.
        /// </summary>
        public void Dispose()
        {
        }
    }
}
