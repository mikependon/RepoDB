using System;
using System.Collections;
using System.Data.Common;

namespace RepoDb.UnitTests.CustomObjects
{
    public class CustomDbDataReader : DbDataReader
    {
        public override object this[int ordinal] => default;

        public override object this[string name] => default;

        public override int Depth => default;

        public override int FieldCount => default;

        public override bool HasRows => default;

        public override bool IsClosed => default;

        public override int RecordsAffected => default;

        public override bool GetBoolean(int ordinal)
        {
            return default;
        }

        public override byte GetByte(int ordinal)
        {
            return default;
        }

        public override long GetBytes(int ordinal,
            long dataOffset,
            byte[] buffer,
            int bufferOffset,
            int length)
        {
            return default;
        }

        public override char GetChar(int ordinal)
        {
            return char.MinValue;
        }

        public override long GetChars(int ordinal,
            long dataOffset,
            char[] buffer,
            int bufferOffset,
            int length)
        {
            return default;
        }

        public override string GetDataTypeName(int ordinal)
        {
            return default;
        }

        public override DateTime GetDateTime(int ordinal)
        {
            return default;
        }

        public override decimal GetDecimal(int ordinal)
        {
            return default;
        }

        public override double GetDouble(int ordinal)
        {
            return default;
        }

        public override IEnumerator GetEnumerator()
        {
            return default;
        }

        public override Type GetFieldType(int ordinal)
        {
            return default;
        }

        public override float GetFloat(int ordinal)
        {
            return default;
        }

        public override Guid GetGuid(int ordinal)
        {
            return default;
        }

        public override short GetInt16(int ordinal)
        {
            return default;
        }

        public override int GetInt32(int ordinal)
        {
            return default;
        }

        public override long GetInt64(int ordinal)
        {
            return default;
        }

        public override string GetName(int ordinal)
        {
            return default;
        }

        public override int GetOrdinal(string name)
        {
            return default;
        }

        public override string GetString(int ordinal)
        {
            return default;
        }

        public override object GetValue(int ordinal)
        {
            return default;
        }

        public override int GetValues(object[] values)
        {
            return default;
        }

        public override bool IsDBNull(int ordinal)
        {
            return default;
        }

        public override bool NextResult()
        {
            return default;
        }

        public override bool Read()
        {
            return default;
        }
    }
}
