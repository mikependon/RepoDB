using System;
using System.Data;

namespace RepoDb.MySql.BulkOperations
{
    /// <summary>
    /// Wraps an inner <see cref="IDataReader"/> and exposes only a chosen subset of its columns - renumbered
    /// to sequential ordinals <c>0..N-1</c> - to whatever reads from this reader.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Needed because <see cref="MySql.MySqlBulkCopy"/> does not skip reading a source column just
    /// because it has no entry in <see cref="MySql.MySqlBulkCopy.ColumnMappings"/>. Internally, it
    /// still walks every ordinal up to the source reader's own <see cref="IDataReader.FieldCount"/>, and for
    /// each one either writes the real destination column (if mapped) or an ignored MySQL user variable (if
    /// not) - but either way it calls <c>GetValues</c> and serializes <em>every</em> field's value into the
    /// <c>LOAD DATA LOCAL INFILE</c> row, since that format is strictly positional/tab-delimited and can't
    /// simply omit a field. An "extra" property on a <c>TEntity</c> (e.g. a computed/joined field, or a
    /// navigation collection - see the <c>...WithExtraFields</c> integration test entities) that has no
    /// matching destination column is therefore still handed to MySql's value serializer even though
    /// its destination mapping was deliberately left out - and if that value's CLR type isn't one of the
    /// handful MySql's serializer recognizes (a plain scalar, string, <see cref="Guid"/>, etc.), it
    /// throws <see cref="NotSupportedException"/> <em>mid-transfer</em>, while <c>LOAD DATA LOCAL INFILE</c>
    /// packets are already in flight - which leaves the connection's protocol state desynced and the
    /// connection itself <c>Broken</c> for every command issued afterward (including the pseudo table
    /// clean-up that immediately follows in a <c>finally</c> block, which is what actually surfaces to the
    /// caller as "Cannot Open when State is Broken").
    /// </para>
    /// <para>
    /// The fix is to never let <see cref="MySql.MySqlBulkCopy"/> see those extra columns at all: this
    /// reader is built from the already-resolved (explicit or default) column mapping, so its
    /// <see cref="FieldCount"/> equals exactly the number of columns actually being written - there is no
    /// "extra" ordinal left for MySql to fall back to ignoring (and serializing anyway).
    /// </para>
    /// </remarks>
    internal sealed class ColumnFilteredDataReader : IDataReader
    {
        private readonly IDataReader _inner;
        private readonly int[] _sourceOrdinals;

        /// <summary>
        /// Creates a new instance of <see cref="ColumnFilteredDataReader"/>.
        /// </summary>
        /// <param name="inner">The reader to wrap.</param>
        /// <param name="sourceOrdinals">
        /// The ordinals of <paramref name="inner"/> to expose, in destination-column order - exposed ordinal
        /// <c>i</c> here reads <paramref name="inner"/>'s ordinal <c>sourceOrdinals[i]</c>.
        /// </param>
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

        /// <summary>
        /// Deliberately reads each value one at a time via <see cref="GetValue"/> (i.e. <c>_inner.GetValue(ordinal)</c>)
        /// rather than delegating to <c>_inner.GetValues(object[])</c> in bulk. <c>RepoDb.Core</c>'s
        /// <c>DataEntityDataReader&lt;TEntity&gt;.GetValues(object[])</c> extracts values via
        /// <c>ClassExpression.GetPropertiesAndValues(Enumerator.Current)</c> - a *generic* method resolved
        /// against the reader's own declared <c>TEntity</c> type parameter, not the entity's actual runtime
        /// type. For a call like <c>BulkDeleteAsync&lt;object&gt;(tableName, entities)</c> (used for
        /// anonymous-object entities, since anonymous types can't be named as a type argument),
        /// <c>TEntity</c> really is <see cref="object"/>, so that generic call resolves against
        /// <c>typeof(object)</c> - which has no properties - and returns an empty list, while the surrounding
        /// loop still iterates the reader's correctly runtime-resolved <c>Properties.Count</c>. The resulting
        /// out-of-bounds read throws mid-<c>LOAD DATA LOCAL INFILE</c> transfer, which leaves the MySql
        /// connection <c>Broken</c> for every command afterward - the same failure mode described on the type
        /// itself, just from a different trigger. <see cref="GetValue"/>/<c>DataEntityDataReader.GetValue(i)</c>
        /// sidesteps this entirely, since it reads via <c>Properties[i].PropertyInfo.GetValue(...)</c> - built
        /// from the entity's actual runtime type, not the generic type parameter.
        /// </summary>
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
