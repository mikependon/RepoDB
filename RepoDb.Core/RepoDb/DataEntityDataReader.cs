using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Collections;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using System.Threading.Tasks;
using System.Threading;

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

        private int fieldCount = 0;
        private bool isClosed = false;
        private bool isDisposed = false;
        private int position = -1;
        private int recordsAffected = -1;

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
            isClosed = false;
            isDisposed = false;
            position = -1;
            recordsAffected = -1;

            // DbSetting
            DbSetting = connection?.GetDbSetting();

            // Properties
            Connection = connection;
            Transaction = transaction;
            Enumerator = entities.GetEnumerator();
            Entities = entities;
        }

        /// <summary>
        /// Initializes the current instance of <see cref="DataEntityDataReader{TEntity}"/> object.
        /// </summary>
        public void Initialize()
        {
            if (IsInitialized)
            {
                return;
            }
            var dbFields = (IEnumerable<DbField>)null;
            if (Connection != null)
            {
                dbFields = DbFieldCache.Get(Connection, ClassMappedNameCache.Get<TEntity>(), Transaction);
            }
            InitializeInternal(dbFields);
        }

        /// <summary>
        /// Initializes the current instance of <see cref="DataEntityDataReader{TEntity}"/> object.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            if (IsInitialized)
            {
                return;
            }
            var dbFields = (IEnumerable<DbField>)null;
            if (Connection != null)
            {
                dbFields = await DbFieldCache.GetAsync(Connection, ClassMappedNameCache.Get<TEntity>(), Transaction, cancellationToken);
            }
            InitializeInternal(dbFields);
        }

        /// <summary>
        /// Initializes the current instance of <see cref="DataEntityDataReader{TEntity}"/> object.
        /// </summary>
        /// <param name="dbFields">The list of the <see cref="DbField"/> objects.</param>
        private void InitializeInternal(IEnumerable<DbField> dbFields)
        {
            if (IsInitialized)
            {
                return;
            }
            if (dbFields?.Any() == true)
            {
                Properties = PropertyCache.Get<TEntity>()
                    .Where(p => dbFields.FirstOrDefault(f => string.Equals(f.Name.AsQuoted(DbSetting), p.GetMappedName().AsQuoted(DbSetting), StringComparison.OrdinalIgnoreCase)) != null)
                    .AsList();
            }
            else
            {
                Properties = PropertyCache.Get<TEntity>().AsList();
            }
            fieldCount = Properties.Count;
            IsInitialized = true;
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection of data entity objects.
        /// </summary>
        /// <returns>The enumerator object of the current collection.</returns>
        public override IEnumerator GetEnumerator() =>
            Entities?.GetEnumerator();

        /// <summary>
        /// Gets the instance of <see cref="IDbConnection"/> in used.
        /// </summary>
        public IDbConnection Connection { get; }

        /// <summary>
        /// Gets the instance of <see cref="IDbTransaction"/> in used.
        /// </summary>
        public IDbTransaction Transaction { get; }

        /// <summary>
        /// Gets a value that indicates whether the current instance of <see cref="DataEntityDataReader{TEntity}"/> object has already been initialized.
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Returns the database setting that is currently in used.
        /// </summary>
        private IDbSetting DbSetting { get; }

        /// <summary>
        /// Gets the instance of enumerator that iterates through a collection of data entity objects.
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
        public int Position { get { return position; } }

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
        public override bool IsClosed =>
            isClosed;

        /// <summary>
        /// Gets the value that indicates whether the current reader is already disposed.
        /// </summary>
        public bool IsDisposed =>
            isDisposed;

        /// <summary>
        /// Gets the number of rows affected by the iteration.
        /// </summary>
        public override int RecordsAffected =>
            recordsAffected;

        /// <summary>
        /// Gets the number of properties the data entity object has.
        /// </summary>
        public override int FieldCount =>
            fieldCount;

        /// <summary>
        /// Gets a value that signify whether the current data reader has data entities.
        /// </summary>
        public override bool HasRows =>
            Entities?.Any() == true;

        /// <summary>
        /// Closes the current data reader.
        /// </summary>
        public override void Close()
        {
            isClosed = true;
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
            isDisposed = true;
        }

        /// <summary>
        /// Resets the pointer of the position to the beginning.
        /// </summary>
        public void Reset()
        {
            ThrowExceptionIfNotInitializedOrNotAvailable();
            Enumerator = Entities.GetEnumerator();
            position = -1;
            recordsAffected = -1;
        }

        /// <summary>
        /// Gets the boolean value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public override bool GetBoolean(int i)
        {
            ThrowExceptionIfNotInitializedOrNotAvailable();
            return Converter.ToType<bool>(GetValue(i));
        }

        /// <summary>
        /// Gets the byte value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public override byte GetByte(int i)
        {
            ThrowExceptionIfNotInitializedOrNotAvailable();
            return Converter.ToType<byte>(GetValue(i));
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
            ThrowExceptionIfNotInitializedOrNotAvailable();
            throw new NotSupportedException("This is not supported by this data reader.");
        }

        /// <summary>
        /// Gets the char value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public override char GetChar(int i)
        {
            ThrowExceptionIfNotInitializedOrNotAvailable();
            return Converter.ToType<char>(GetValue(i));
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
            ThrowExceptionIfNotInitializedOrNotAvailable();
            throw new NotSupportedException("This is not supported by this data reader.");
        }

        /// <summary>
        /// GetData
        /// </summary>
        /// <param name="i">Int</param>
        /// <returns>Int</returns>
        public new IDataReader GetData(int i)
        {
            ThrowExceptionIfNotInitializedOrNotAvailable();
            throw new NotSupportedException("This is not supported by this data reader.");
        }

        /// <summary>
        /// Gets the name of the property data type from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The property type name from the property index.</returns>
        public override string GetDataTypeName(int i)
        {
            ThrowExceptionIfNotInitializedOrNotAvailable();
            return Properties[i].PropertyInfo.PropertyType.Name;
        }

        /// <summary>
        /// Gets the date time value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public override DateTime GetDateTime(int i)
        {
            ThrowExceptionIfNotInitializedOrNotAvailable();
            return Converter.ToType<DateTime>(GetValue(i));
        }

        /// <summary>
        /// Gets the decimal value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public override decimal GetDecimal(int i)
        {
            ThrowExceptionIfNotInitializedOrNotAvailable();
            return Converter.ToType<decimal>(GetValue(i));
        }

        /// <summary>
        /// Gets the double value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public override double GetDouble(int i)
        {
            ThrowExceptionIfNotInitializedOrNotAvailable();
            return Converter.ToType<double>(GetValue(i));
        }

        /// <summary>
        /// Gets the type of the property from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The property type from the property index.</returns>
        public override Type GetFieldType(int i)
        {
            ThrowExceptionIfNotInitializedOrNotAvailable();
            return Properties[i].PropertyInfo.PropertyType;
        }

        /// <summary>
        /// Gets the float value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public override float GetFloat(int i)
        {
            ThrowExceptionIfNotInitializedOrNotAvailable();
            return Converter.ToType<float>(GetValue(i));
        }

        /// <summary>
        /// Gets the Guid value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public override Guid GetGuid(int i)
        {
            ThrowExceptionIfNotInitializedOrNotAvailable();
            return Guid.Parse(GetValue(i)?.ToString());
        }

        /// <summary>
        /// Gets the short value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public override short GetInt16(int i)
        {
            ThrowExceptionIfNotInitializedOrNotAvailable();
            return Converter.ToType<short>(GetValue(i));
        }

        /// <summary>
        /// Gets the int value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public override int GetInt32(int i)
        {
            ThrowExceptionIfNotInitializedOrNotAvailable();
            return Converter.ToType<int>(GetValue(i));
        }

        /// <summary>
        /// Gets the long value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public override long GetInt64(int i)
        {
            ThrowExceptionIfNotInitializedOrNotAvailable();
            return Converter.ToType<long>(GetValue(i));
        }

        /// <summary>
        /// Gets the name of the property from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The name from the property index.</returns>
        public override string GetName(int i)
        {
            ThrowExceptionIfNotInitializedOrNotAvailable();
            return Properties[i].GetMappedName();
        }

        /// <summary>
        /// Gets the index of the property based on the property name.
        /// </summary>
        /// <param name="name">The index of the property.</param>
        /// <returns>The index of the property from property name.</returns>
        public override int GetOrdinal(string name)
        {
            ThrowExceptionIfNotInitializedOrNotAvailable();
            return Properties.IndexOf(Properties.FirstOrDefault(p => p.GetMappedName() == name));
        }

        /// <summary>
        /// Gets the table schema.
        /// </summary>
        /// <returns>An instance of the <see cref="DataTable"/> with the table schema.</returns>
        public override DataTable GetSchemaTable()
        {
            ThrowExceptionIfNotInitializedOrNotAvailable();
            throw new NotSupportedException("This is not supported by this data reader.");
        }

        /// <summary>
        /// Gets the string value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public override string GetString(int i)
        {
            ThrowExceptionIfNotInitializedOrNotAvailable();
            return Converter.ToType<string>(GetValue(i));
        }

        /// <summary>
        /// Gets the current value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public override object GetValue(int i)
        {
            ThrowExceptionIfNotInitializedOrNotAvailable();
            return Properties[i].PropertyInfo.GetValue(Enumerator.Current);
        }

        /// <summary>
        /// Populates the values of the array of the current values of the current row.
        /// </summary>
        /// <param name="values">The array variable on which to populate the data.</param>
        /// <returns></returns>
        public override int GetValues(object[] values)
        {
            ThrowExceptionIfNotInitializedOrNotAvailable();
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
            ThrowExceptionIfNotInitializedOrNotAvailable();
            return GetValue(i) == DBNull.Value;
        }

        /// <summary>
        /// Forwards the data reader to the next result.
        /// </summary>
        /// <returns>Returns true if the forward operation is successful.</returns>
        public override bool NextResult()
        {
            ThrowExceptionIfNotInitializedOrNotAvailable();
            throw new NotSupportedException("This is not supported by this data reader.");
        }

        /// <summary>
        /// Forward the pointer into the next record.
        /// </summary>
        /// <returns>A value that indicates whether the movement is successful.</returns>
        public override bool Read()
        {
            ThrowExceptionIfNotInitializedOrNotAvailable();
            position++;
            recordsAffected++;
            return Enumerator.MoveNext();
        }

        /// <summary>
        /// Throws an exception if the current data reader has not yet been initialized or is not available anymore.
        /// </summary>
        private void ThrowExceptionIfNotInitializedOrNotAvailable()
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException("The reader is not yet initialized.");
            }
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
