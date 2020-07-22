using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Collections;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb
{
    /// <summary>
    /// A data reader object used to manipulate the enumerable list of data entity objects.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity</typeparam>
    public class DataEntityDataReader<TEntity> : DbDataReader
        where TEntity : class
    {
        #region Fields

        private bool m_isClosed = false;
        private bool m_isDisposed = false;
        private int m_position = -1;
        private int m_recordsAffected = -1;

        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="DataEntityDataReader{TEntity}"/> object.
        /// </summary>
        /// <param name="entities">The list of the data entity object to be used for manipulation.</param>
        public DataEntityDataReader(IEnumerable<TEntity> entities)
            : this(entities,
                null,
                null)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="DataEntityDataReader{TEntity}"/> object.
        /// </summary>
        /// <param name="entities">The list of the data entity object to be used for manipulation.</param>
        /// <param name="connection">The actual <see cref="IDbConnection"/> object used.</param>
        public DataEntityDataReader(IEnumerable<TEntity> entities,
            IDbConnection connection)
            : this(entities,
                  connection,
                  null)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="DataEntityDataReader{TEntity}"/> object.
        /// </summary>
        /// <param name="entities">The list of the data entity object to be used for manipulation.</param>
        /// <param name="connection">The actual <see cref="IDbConnection"/> object used.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        public DataEntityDataReader(IEnumerable<TEntity> entities,
            IDbConnection connection,
            IDbTransaction transaction)
        {
            if (entities == null)
            {
                throw new NullReferenceException("The entities could not be null.");
            }

            // Fields
            m_isClosed = false;
            m_isDisposed = false;
            m_position = -1;
            m_recordsAffected = -1;

            // DbSetting
            DbSetting = connection?.GetDbSetting();

            // Properties
            if (connection != null)
            {
                var fields = DbFieldCache.Get(connection, ClassMappedNameCache.Get<TEntity>(), transaction);
                if (fields?.Any() == true)
                {
                    Properties = PropertyCache.Get<TEntity>()
                        .Where(p => fields.FirstOrDefault(f => string.Equals(f.Name.AsQuoted(DbSetting), p.GetMappedName().AsQuoted(DbSetting), StringComparison.OrdinalIgnoreCase)) != null)
                        .AsList();
                }
            }
            if (Properties?.Count == 0)
            {
                Properties = PropertyCache.Get<TEntity>().AsList();
            }
            Enumerator = entities.GetEnumerator();
            Entities = entities;
            FieldCount = Properties.Count;
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection of data entity objects.
        /// </summary>
        /// <returns>The enumerator object of the current collection.</returns>
        public override IEnumerator GetEnumerator()
        {
            return Entities?.GetEnumerator();
        }

        /// <summary>
        /// Returns the database setting that is currently in used.
        /// </summary>
        private IDbSetting DbSetting { get; }

        /// <summary>
        /// Returns an enumerator that iterates through a collection of data entity objects.
        /// </summary>
        public IEnumerator<TEntity> Enumerator { get; private set; }

        /// <summary>
        /// Gets the list of data entity objects.
        /// </summary>
        public IEnumerable<TEntity> Entities { get; private set; }

        /// <summary>
        /// Gets the properties of data entity object.
        /// </summary>
        public IList<ClassProperty> Properties { get; private set; }

        /// <summary>
        /// Gets the current position of the enumerator.
        /// </summary>
        public int Position { get { return m_position; } }

        /// <summary>
        /// Gets the current value from the index.
        /// </summary>
        /// <param name="i">The index of the column.</param>
        /// <returns>The value from the column index.</returns>
        public override object this[int i] { get { return GetValue(i); } }

        /// <summary>
        /// Gets the current value from the name.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns>The value from the column name.</returns>
        public override object this[string name] { get { return this[GetOrdinal(name)]; } }

        /// <summary>
        /// Gets the depth value.
        /// </summary>
        public override int Depth { get; }

        /// <summary>
        /// Gets the value that indicates whether the current reader is closed.
        /// </summary>
        public override bool IsClosed { get { return m_isClosed; } }

        /// <summary>
        /// Gets the value that indicates whether the current reader is already disposed.
        /// </summary>
        public bool IsDisposed { get { return m_isDisposed; } }

        /// <summary>
        /// Gets the number of rows affected by the iteration.
        /// </summary>
        public override int RecordsAffected { get { return m_recordsAffected; } }

        /// <summary>
        /// Gets the number of properties the data entity object has.
        /// </summary>
        public override int FieldCount { get; }

        /// <summary>
        /// Gets a value that signify whether the current data reader has data entities.
        /// </summary>
        public override bool HasRows => Entities?.Any() ?? false;

        /// <summary>
        /// Closes the current data reader.
        /// </summary>
        public override void Close()
        {
            m_isClosed = true;
        }

        /// <summary>
        /// Disposes the current data reader.
        /// </summary>
        public new void Dispose()
        {
            base.Dispose();
            Entities = null;
            Properties = null;
            Enumerator = null;
            Close();
            m_isDisposed = true;
        }

        /// <summary>
        /// Resets the pointer of the position to the beginning.
        /// </summary>
        public void Reset()
        {
            ThrowExceptionIfNotAvailable();
            Enumerator = Entities.GetEnumerator();
            m_position = -1;
            m_recordsAffected = -1;
        }

        /// <summary>
        /// Gets the boolean value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public override bool GetBoolean(int i)
        {
            ThrowExceptionIfNotAvailable();
            return Convert.ToBoolean(GetValue(i));
        }

        /// <summary>
        /// Gets the byte value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public override byte GetByte(int i)
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
        public override long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            ThrowExceptionIfNotAvailable();
            throw new NotSupportedException("This is not supported by this data reader.");
        }

        /// <summary>
        /// Gets the char value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public override char GetChar(int i)
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
        public override long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            ThrowExceptionIfNotAvailable();
            throw new NotSupportedException("This is not supported by this data reader.");
        }

        /// <summary>
        /// GetData
        /// </summary>
        /// <param name="i">Int</param>
        /// <returns>Int</returns>
        public new IDataReader GetData(int i)
        {
            ThrowExceptionIfNotAvailable();
            throw new NotSupportedException("This is not supported by this data reader.");
        }

        /// <summary>
        /// Gets the name of the property data type from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The property type name from the property index.</returns>
        public override string GetDataTypeName(int i)
        {
            ThrowExceptionIfNotAvailable();
            return Properties[i].PropertyInfo.PropertyType.Name;
        }

        /// <summary>
        /// Gets the date time value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public override DateTime GetDateTime(int i)
        {
            ThrowExceptionIfNotAvailable();
            return Convert.ToDateTime(GetValue(i));
        }

        /// <summary>
        /// Gets the decimal value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public override decimal GetDecimal(int i)
        {
            ThrowExceptionIfNotAvailable();
            return Convert.ToDecimal(GetValue(i));
        }

        /// <summary>
        /// Gets the double value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public override double GetDouble(int i)
        {
            ThrowExceptionIfNotAvailable();
            return Convert.ToDouble(GetValue(i));
        }

        /// <summary>
        /// Gets the type of the property from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The property type from the property index.</returns>
        public override Type GetFieldType(int i)
        {
            ThrowExceptionIfNotAvailable();
            return Properties[i].PropertyInfo.PropertyType;
        }

        /// <summary>
        /// Gets the float value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public override float GetFloat(int i)
        {
            ThrowExceptionIfNotAvailable();
            return float.Parse(GetValue(i)?.ToString());
        }

        /// <summary>
        /// Gets the Guid value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public override Guid GetGuid(int i)
        {
            ThrowExceptionIfNotAvailable();
            return Guid.Parse(GetValue(i)?.ToString());
        }

        /// <summary>
        /// Gets the short value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public override short GetInt16(int i)
        {
            ThrowExceptionIfNotAvailable();
            return Convert.ToInt16(GetValue(i));
        }

        /// <summary>
        /// Gets the int value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public override int GetInt32(int i)
        {
            ThrowExceptionIfNotAvailable();
            return Convert.ToInt32(GetValue(i));
        }

        /// <summary>
        /// Gets the long value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public override long GetInt64(int i)
        {
            ThrowExceptionIfNotAvailable();
            return Convert.ToInt64(GetValue(i));
        }

        /// <summary>
        /// Gets the name of the property from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The name from the property index.</returns>
        public override string GetName(int i)
        {
            ThrowExceptionIfNotAvailable();
            return Properties[i].GetMappedName();
        }

        /// <summary>
        /// Gets the index of the property based on the property name.
        /// </summary>
        /// <param name="name">The index of the property.</param>
        /// <returns>The index of the property from property name.</returns>
        public override int GetOrdinal(string name)
        {
            ThrowExceptionIfNotAvailable();
            return Properties.IndexOf(Properties.FirstOrDefault(p => p.GetMappedName() == name));
        }

        /// <summary>
        /// Gets the table schema.
        /// </summary>
        /// <returns>An instance of the <see cref="DataTable"/> with the table schema.</returns>
        public override DataTable GetSchemaTable()
        {
            ThrowExceptionIfNotAvailable();
            throw new NotSupportedException("This is not supported by this data reader.");
        }

        /// <summary>
        /// Gets the string value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public override string GetString(int i)
        {
            ThrowExceptionIfNotAvailable();
            return Convert.ToString(GetValue(i));
        }

        /// <summary>
        /// Gets the current value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public override object GetValue(int i)
        {
            ThrowExceptionIfNotAvailable();
            return Properties[i].PropertyInfo.GetValue(Enumerator.Current);
        }

        /// <summary>
        /// Populates the values of the array of the current values of the current row.
        /// </summary>
        /// <param name="values">The array variable on which to populate the data.</param>
        /// <returns></returns>
        public override int GetValues(object[] values)
        {
            ThrowExceptionIfNotAvailable();
            if (values == null)
            {
                throw new NullReferenceException("The values array must not be null.");
            }
            if (values.Length != FieldCount)
            {
                throw new ArgumentOutOfRangeException($"The length of the array must be equals to the number of fields of the data entity (it should be {FieldCount}).");
            }
            var extracted = ClassExpression.GetPropertiesAndValues(Enumerator.Current).ToArray();
            for (var i = 0; i < Properties.Count; i++)
            {
                values[i] = extracted[i].Value;
            }
            return FieldCount;
        }

        /// <summary>
        /// Gets a value that checks whether the value of the property from the desired index is equals to <see cref="DBNull.Value"/>.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public override bool IsDBNull(int i)
        {
            ThrowExceptionIfNotAvailable();
            return GetValue(i) == DBNull.Value;
        }

        /// <summary>
        /// Forwards the data reader to the next result.
        /// </summary>
        /// <returns>Returns true if the forward operation is successful.</returns>
        public override bool NextResult()
        {
            ThrowExceptionIfNotAvailable();
            throw new NotSupportedException("This is not supported by this data reader.");
        }

        /// <summary>
        /// Forward the pointer into the next record.
        /// </summary>
        /// <returns>A value that indicates whether the movement is successful.</returns>
        public override bool Read()
        {
            ThrowExceptionIfNotAvailable();
            m_position++;
            m_recordsAffected++;
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
