using System;
using System.Collections;
using System.Data.Common;

namespace RepoDb.UnitTests.CustomObjects
{
    public class CustomDbDataReader : DbDataReader
    {
        public override object this[int ordinal] => throw new NotImplementedException();

        public override object this[string name] => throw new NotImplementedException();

        public override int Depth => throw new NotImplementedException();

        public override int FieldCount => throw new NotImplementedException();

        public override bool HasRows => throw new NotImplementedException();

        public override bool IsClosed => throw new NotImplementedException();

        public override int RecordsAffected => throw new NotImplementedException();

        public override bool GetBoolean(int ordinal)
        {
            return true;
        }

        public override byte GetByte(int ordinal)
        {
            return 0;
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            return 0;
        }

        public override char GetChar(int ordinal)
        {
            return char.MinValue;
        }

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            return 0;
        }

        public override string GetDataTypeName(int ordinal)
        {
            return null;
        }

        public override DateTime GetDateTime(int ordinal)
        {
            return DateTime.MinValue;
        }

        public override decimal GetDecimal(int ordinal)
        {
            return 0;
        }

        public override double GetDouble(int ordinal)
        {
            return 0;
        }

        public override IEnumerator GetEnumerator()
        {
            return null;
        }

        public override Type GetFieldType(int ordinal)
        {
            return typeof(object);
        }

        public override float GetFloat(int ordinal)
        {
            return 0;
        }

        public override Guid GetGuid(int ordinal)
        {
            return Guid.Empty;
        }

        public override short GetInt16(int ordinal)
        {
            return 0;
        }

        public override int GetInt32(int ordinal)
        {
            return 0;
        }

        public override long GetInt64(int ordinal)
        {
            return 0;
        }

        public override string GetName(int ordinal)
        {
            return null;
        }

        public override int GetOrdinal(string name)
        {
            return 0;
        }

        public override string GetString(int ordinal)
        {
            return null;
        }

        public override object GetValue(int ordinal)
        {
            return null;
        }

        public override int GetValues(object[] values)
        {
            return 0;
        }

        public override bool IsDBNull(int ordinal)
        {
            return true;
        }

        public override bool NextResult()
        {
            return true;
        }

        public override bool Read()
        {
            return true;
        }
    }
}
