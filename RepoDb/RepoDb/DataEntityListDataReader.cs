using RepoDb.Enumerations;
using RepoDb.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace RepoDb
{
    /// <summary>
    /// A data reader object used to manipulate the enumerable list of <i>DataEntity</i> objects.
    /// </summary>
    /// <typeparam name="TEntity">The type of the <i>DataEntity</i></typeparam>
    public class DataEntityListDataReader<TEntity> : IDataReader where TEntity : DataEntity
    {
        /// <summary>
        /// Creates a new instance of <i>RepoDb.DataEntityListDataReader</i> object.
        /// </summary>
        /// <param name="entities">The list of the <i>DataEntity</i> object to be used for manipulation.</param>
        /// <param name="command">The type of command to be used by this data reader.</param>
        public DataEntityListDataReader(IEnumerable<TEntity> entities, Command command)
        {
            if (entities == null)
            {
                throw new NullReferenceException("The entities could not be null.");
            }
            Properties = DataEntityExtension.GetPropertiesFor<TEntity>(command).ToList();
            Enumerator = entities.GetEnumerator();
            Entities = entities;
            Position = -1;
            FieldCount = Properties.Count;
        }

        /// <summary>
        /// Gets the current <i>DataEntity</i> list enumerator used by this <i>DataReader</i>.
        /// </summary>
        public IEnumerator<TEntity> Enumerator { get; private set; }

        /// <summary>
        /// Gets the list of <i>DataEntity</i> objects.
        /// </summary>
        public IEnumerable<TEntity> Entities { get; private set; }

        /// <summary>
        /// Gets the properties of <i>DataEntity</i> object.
        /// </summary>
        public IList<PropertyInfo> Properties { get; private set; }

        /// <summary>
        /// Gets the current position of the enumerator.
        /// </summary>
        public int Position { get; private set; }

        /// <summary>
        /// Gets the current value from the index.
        /// </summary>
        /// <param name="i">The index of the column.</param>
        /// <returns>The value from the column index.</returns>
        public object this[int i] { get { return GetValue(i); } }

        /// <summary>
        /// Gets the current value from the name.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns>The value from the column name.</returns>
        public object this[string name] { get { return this[GetOrdinal(name)]; } }

        /// <summary>
        /// Gets the depth value.
        /// </summary>
        public int Depth { get; private set; }

        /// <summary>
        /// Gets the value that indicates whether the current reader is closed.
        /// </summary>
        public bool IsClosed { get; private set; }

        /// <summary>
        /// Gets the value that indicates whether the current reader is already disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Gets the number of rows affected by the iteration.
        /// </summary>
        public int RecordsAffected { get; private set; }

        /// <summary>
        /// Gets the number of properties the <i>DataEntity</i> object has.
        /// </summary>
        public int FieldCount { get; }

        /// <summary>
        /// Closes the current data reader.
        /// </summary>
        public void Close()
        {
            IsClosed = true;
            IsDisposed = true;
        }

        /// <summary>
        /// Disposes the current data reader.
        /// </summary>
        public void Dispose()
        {
            Entities = null;
            Properties = null;
            Enumerator = null;
        }

        /// <summary>
        /// Resets the pointer of the position to the beginning.
        /// </summary>
        public void Reset()
        {
            ThrowExceptionIfNotAvailable();
            Position = -1;
            Enumerator = Entities.GetEnumerator();
        }

        /// <summary>
        /// Gets the boolean value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public bool GetBoolean(int i)
        {
            ThrowExceptionIfNotAvailable();
            return Convert.ToBoolean(GetValue(i));
        }

        /// <summary>
        /// Gets the byte value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public byte GetByte(int i)
        {
            ThrowExceptionIfNotAvailable();
            return Convert.ToByte(GetValue(i));
        }

        /// <summary>
        /// GetBytes
        /// </summary>
        /// <param name="i">Int</param>
        /// <param name="fieldOffset">Int64</param>
        /// <param name="buffer">byte[]</param>
        /// <param name="bufferoffset">Int</param>
        /// <param name="length">Int</param>
        /// <returns></returns>
        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            ThrowExceptionIfNotAvailable();
            throw new NotSupportedException("This is not supported by this data reader.");
        }

        /// <summary>
        /// Gets the char value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public char GetChar(int i)
        {
            ThrowExceptionIfNotAvailable();
            return Convert.ToChar(GetValue(i));
        }

        /// <summary>
        /// GetChars
        /// </summary>
        /// <param name="i">Int</param>
        /// <param name="fieldoffset">Int64</param>
        /// <param name="buffer">char[]</param>
        /// <param name="bufferoffset">Int</param>
        /// <param name="length">Int</param>
        /// <returns></returns>
        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            ThrowExceptionIfNotAvailable();
            throw new NotSupportedException("This is not supported by this data reader.");
        }

        /// <summary>
        /// GetData
        /// </summary>
        /// <param name="i">Int</param>
        /// <returns>Int</returns>
        public IDataReader GetData(int i)
        {
            ThrowExceptionIfNotAvailable();
            throw new NotSupportedException("This is not supported by this data reader.");
        }

        /// <summary>
        /// Gets the name of the property data type from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The property type name from the property index.</returns>
        public string GetDataTypeName(int i)
        {
            ThrowExceptionIfNotAvailable();
            return Properties[i].PropertyType.Name;
        }

        /// <summary>
        /// Gets the date time value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public DateTime GetDateTime(int i)
        {
            ThrowExceptionIfNotAvailable();
            return Convert.ToDateTime(GetValue(i));
        }

        /// <summary>
        /// Gets the decimal value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public decimal GetDecimal(int i)
        {
            ThrowExceptionIfNotAvailable();
            return Convert.ToDecimal(GetValue(i));
        }

        /// <summary>
        /// Gets the double value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public double GetDouble(int i)
        {
            ThrowExceptionIfNotAvailable();
            return Convert.ToDouble(GetValue(i));
        }

        /// <summary>
        /// Gets the type of the property from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The property type from the property index.</returns>
        public Type GetFieldType(int i)
        {
            ThrowExceptionIfNotAvailable();
            return Properties[i].PropertyType;
        }

        /// <summary>
        /// Gets the float value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public float GetFloat(int i)
        {
            ThrowExceptionIfNotAvailable();
            return float.Parse(GetValue(i)?.ToString());
        }

        /// <summary>
        /// Gets the Guid value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public Guid GetGuid(int i)
        {
            ThrowExceptionIfNotAvailable();
            return Guid.Parse(GetValue(i)?.ToString());
        }

        /// <summary>
        /// Gets the short value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public short GetInt16(int i)
        {
            ThrowExceptionIfNotAvailable();
            return Convert.ToInt16(GetValue(i));
        }

        /// <summary>
        /// Gets the int value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public int GetInt32(int i)
        {
            ThrowExceptionIfNotAvailable();
            return Convert.ToInt32(GetValue(i));
        }

        /// <summary>
        /// Gets the long value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public long GetInt64(int i)
        {
            ThrowExceptionIfNotAvailable();
            return Convert.ToInt64(GetValue(i));
        }

        /// <summary>
        /// Gets the name of the property from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The name from the property index.</returns>
        public string GetName(int i)
        {
            ThrowExceptionIfNotAvailable();
            return Properties[i].GetMappedName();
        }

        /// <summary>
        /// Gets the index of the property based on the property name.
        /// </summary>
        /// <param name="name">The index of the property.</param>
        /// <returns>The index of the property from property name.</returns>
        public int GetOrdinal(string name)
        {
            ThrowExceptionIfNotAvailable();
            return Properties.IndexOf(Properties.FirstOrDefault(p => p.GetMappedName() == name));
        }

        /// <summary>
        /// Gets the table schema.
        /// </summary>
        /// <returns>An instance of the <i>System.Data.DataTable</i> with the table schema.</returns>
        public DataTable GetSchemaTable()
        {
            ThrowExceptionIfNotAvailable();
            throw new NotSupportedException("This is not supported by this data reader.");
        }

        /// <summary>
        /// Gets the string value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public string GetString(int i)
        {
            ThrowExceptionIfNotAvailable();
            return Convert.ToString(GetValue(i));
        }

        /// <summary>
        /// Gets the current value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public object GetValue(int i)
        {
            ThrowExceptionIfNotAvailable();
            return Properties[i].GetValue(Enumerator.Current);
        }

        /// <summary>
        /// Populates the values of the array of the current values of the current row.
        /// </summary>
        /// <param name="values">The array variable on which to populate the data.</param>
        /// <returns></returns>
        public int GetValues(object[] values)
        {
            ThrowExceptionIfNotAvailable();
            if (values == null)
            {
                throw new NullReferenceException("The values array must not be null.");
            }
            if (values.Length != FieldCount)
            {
                throw new InvalidOperationException($"The length of the array must be equals to the number of fields of the data entity (it should be {FieldCount}).");
            }
            for (var i = 0; i < Properties.Count; i++)
            {
                values[i] = Properties[i].GetValue(Enumerator.Current);
            }
            return FieldCount;
        }

        /// <summary>
        /// Gets a value that checks whether the value of the property from the desired index is equals to <i>System.DbNull.Value</i>.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public bool IsDBNull(int i)
        {
            ThrowExceptionIfNotAvailable();
            return GetValue(i) == DBNull.Value;
        }

        /// <summary>
        /// Forwards the data reader to the next result.
        /// </summary>
        /// <returns>Returns true if the forward operation is successful.</returns>
        public bool NextResult()
        {
            ThrowExceptionIfNotAvailable();
            throw new NotSupportedException("This is not supported by this data reader.");
        }

        /// <summary>
        /// Forward the pointer into the next record.
        /// </summary>
        /// <returns>A value that indicates whether the movement is successful.</returns>
        public bool Read()
        {
            ThrowExceptionIfNotAvailable();
            Position++;
            RecordsAffected++;
            return Enumerator.MoveNext();
        }

        /// <summary>
        /// Throws an exception if the current data reader is not available.
        /// </summary>
        private void ThrowExceptionIfNotAvailable()
        {
            if (IsDisposed)
            {
                throw new InvalidOperationException("The reader is already disposed.");
            }
            if (IsClosed)
            {
                throw new InvalidOperationException("The reader is already closed.");
            }
        }
    }
}
