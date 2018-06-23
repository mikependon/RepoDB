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
        public DataEntityListDataReader(IEnumerable<TEntity> entities)
        {
            Properties = DataEntityExtension.GetPropertiesFor<TEntity>(Command.None).ToList();
            Enumerator = entities.GetEnumerator();
            Entities = entities;
            Position = -1;
            FieldCount = Properties.Count();
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
        /// <param name="i">The name of the column.</param>
        /// <returns>The value from the column name.</returns>
        public object this[string name] { get { return GetValue(GetOrdinal(name)); } }

        /// <summary>
        /// Gets the depth value.
        /// </summary>
        public int Depth { get; private set; }

        /// <summary>
        /// Gets the value that indicates whether the current reader is closed.
        /// </summary>
        public bool IsClosed { get; private set; }

        /// <summary>
        /// Gets the number of rows affected.
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
        /// Gets the boolean value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public bool GetBoolean(int i)
        {
            return Convert.ToBoolean(GetValue(i));
        }

        /// <summary>
        /// Gets the byte value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public byte GetByte(int i)
        {
            return Convert.ToByte(GetValue(i));
        }
        
        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the char value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public char GetChar(int i)
        {
            return Convert.ToChar(GetValue(i));
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the name of the property data type from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The property type name from the property index.</returns>
        public string GetDataTypeName(int i)
        {
            return Properties[i].PropertyType.Name;
        }

        /// <summary>
        /// Gets the date time value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public DateTime GetDateTime(int i)
        {
            return Convert.ToDateTime(GetValue(i));
        }

        /// <summary>
        /// Gets the decimal value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public decimal GetDecimal(int i)
        {
            return Convert.ToDecimal(GetValue(i));
        }

        /// <summary>
        /// Gets the double value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public double GetDouble(int i)
        {
            return Convert.ToDouble(GetValue(i));
        }

        /// <summary>
        /// Gets the type of the property from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The property type from the property index.</returns>
        public Type GetFieldType(int i)
        {
            return Properties[i].PropertyType;
        }

        /// <summary>
        /// Gets the float value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public float GetFloat(int i)
        {
            return float.Parse(GetValue(i)?.ToString());
        }

        /// <summary>
        /// Gets the Guid value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public Guid GetGuid(int i)
        {
            return Guid.Parse(GetValue(i)?.ToString());
        }

        /// <summary>
        /// Gets the short value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public short GetInt16(int i)
        {
            return Convert.ToInt16(GetValue(i));
        }

        /// <summary>
        /// Gets the int value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public int GetInt32(int i)
        {
            return Convert.ToInt32(GetValue(i));
        }

        /// <summary>
        /// Gets the long value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public long GetInt64(int i)
        {
            return Convert.ToInt64(GetValue(i));
        }

        /// <summary>
        /// Gets the name of the property from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The name from the property index.</returns>
        public string GetName(int i)
        {
            return Properties[i].GetMappedName();
        }

        /// <summary>
        /// Gets the index of the property based on the property name.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The index of the property from property name.</returns>
        public int GetOrdinal(string name)
        {
            return Properties.IndexOf(Properties.FirstOrDefault(p => p.GetMappedName() == name));
        }

        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the string value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public string GetString(int i)
        {
            return Convert.ToString(GetValue(i));
        }

        /// <summary>
        /// Gets the current value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public object GetValue(int i)
        {
            return Properties[i].GetValue(Enumerator.Current);
        }

        public int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a value that checks whether the value of the property from the desired index is equals to <i>System.DbNull.Value</i>.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public bool IsDBNull(int i)
        {
            return GetValue(i) == DBNull.Value;
        }

        public bool NextResult()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Forward the pointer into the next record.
        /// </summary>
        /// <returns>A value that indicates whether the movement is successful.</returns>
        public bool Read()
        {
            if (IsClosed)
            {
                throw new InvalidOperationException("The reader is close.");
            }
            Position++;
            RecordsAffected++;
            return Enumerator.MoveNext();
        }
    }
}
