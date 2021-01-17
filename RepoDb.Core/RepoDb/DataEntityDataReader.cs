using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Collections;
using RepoDb.Extensions;

namespace RepoDb
{
    /// <summary>
    /// A data reader object that is used to manipulate the enumerable list of data entity objects.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity</typeparam>
    public class DataEntityDataReader<TEntity> : DbDataReader
        where TEntity : class
    {
        #region Fields

        private string tableName = null;
        private int fieldCount = 0;
        private bool isClosed = false;
        private bool isDisposed = false;
        private int position = -1;
        private int recordsAffected = -1;
        private bool isDictionaryStringObject = false;

        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="DataEntityDataReader{TEntity}"/> object.
        /// </summary>
        /// <param name="entities">The list of the data entity object to be used for manipulation.</param>
        public DataEntityDataReader(IEnumerable<TEntity> entities)
            : this(entities, null, null)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="DataEntityDataReader{TEntity}"/> object.
        /// </summary>
        /// <param name="entities">The list of the data entity object to be used for manipulation.</param>
        /// <param name="connection">The actual <see cref="IDbConnection"/> object used.</param>
        public DataEntityDataReader(IEnumerable<TEntity> entities,
            IDbConnection connection)
            : this(entities, connection, null)
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
            : this(null, entities, connection, transaction)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="DataEntityDataReader{TEntity}"/> object.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="entities">The list of the data entity object to be used for manipulation.</param>
        /// <param name="connection">The actual <see cref="IDbConnection"/> object used.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        public DataEntityDataReader(string tableName,
            IEnumerable<TEntity> entities,
            IDbConnection connection,
            IDbTransaction transaction)
            : this(tableName, entities, connection, transaction, false)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="DataEntityDataReader{TEntity}"/> object.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="entities">The list of the data entity object to be used for manipulation.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <param name="connection">The actual <see cref="IDbConnection"/> object used.</param>
        /// <param name="hasOrderingColumn">The value that signifies whether the ordering column will be defined.</param>
        public DataEntityDataReader(string tableName,
            IEnumerable<TEntity> entities,
            IDbConnection connection,
            IDbTransaction transaction,
            bool hasOrderingColumn)
        {
            if (entities == null)
            {
                throw new NullReferenceException("The entities could not be null.");
            }

            // Fields
            this.tableName = tableName ?? ClassMappedNameCache.Get<TEntity>();
            isClosed = false;
            isDisposed = false;
            position = -1;
            recordsAffected = -1;

            // Type
            var entityType = typeof(TEntity);
            EntityType = entityType == StaticType.Object ?
                (entities.FirstOrDefault()?.GetType() ?? entityType) :
                entityType;
            isDictionaryStringObject = EntityType.IsDictionaryStringObject();

            // Properties
            Connection = connection;
            Transaction = transaction;
            HasOrderingColumn = hasOrderingColumn;
            Enumerator = entities.GetEnumerator();
            Entities = entities;
            Properties = GetClassProperties().AsList();
            Fields = GetFields(Entities?.FirstOrDefault() as IDictionary<string, object>).AsList();
            fieldCount = isDictionaryStringObject ? Fields.Count : Properties.Count;
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
        /// Gets the instance of enumerator that iterates through a collection of data entity objects.
        /// </summary>
        public IEnumerator<TEntity> Enumerator { get; private set; }

        /// <summary>
        /// Gets the list of data entity objects.
        /// </summary>
        public IEnumerable<TEntity> Entities { get; private set; }

        /// <summary>
        /// Gets the current position of the enumerator.
        /// </summary>
        public int Position =>
            position;

        /// <summary>
        /// Gets the type of the entities.
        /// </summary>
        private Type EntityType { get; set; }

        /// <summary>
        /// Gets the properties of data entity object.
        /// </summary>
        private IList<ClassProperty> Properties { get; set; }

        /// <summary>
        /// Gets the fields of the dictionary.
        /// </summary>
        private IList<Field> Fields { get; set; }

        /// <summary>
        /// Gets a value that indicates whether the ordering column is defined.
        /// </summary>
        private bool HasOrderingColumn { get; }

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
            ThrowExceptionIfNotAvailable();
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
            ThrowExceptionIfNotAvailable();
            return Converter.ToType<bool>(GetValue(i));
        }

        /// <summary>
        /// Gets the byte value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public override byte GetByte(int i)
        {
            ThrowExceptionIfNotAvailable();
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
            return Converter.ToType<DateTime>(GetValue(i));
        }

        /// <summary>
        /// Gets the decimal value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public override decimal GetDecimal(int i)
        {
            ThrowExceptionIfNotAvailable();
            return Converter.ToType<decimal>(GetValue(i));
        }

        /// <summary>
        /// Gets the double value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public override double GetDouble(int i)
        {
            ThrowExceptionIfNotAvailable();
            return Converter.ToType<double>(GetValue(i));
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
            return Converter.ToType<float>(GetValue(i));
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
            return Converter.ToType<short>(GetValue(i));
        }

        /// <summary>
        /// Gets the int value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public override int GetInt32(int i)
        {
            ThrowExceptionIfNotAvailable();
            return Converter.ToType<int>(GetValue(i));
        }

        /// <summary>
        /// Gets the long value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public override long GetInt64(int i)
        {
            ThrowExceptionIfNotAvailable();
            return Converter.ToType<long>(GetValue(i));
        }

        /// <summary>
        /// Gets the name of the property from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The name from the property index.</returns>
        public override string GetName(int i) =>
            isDictionaryStringObject ? GetNameForDictionaryStringObject(i) : GetNameForEntities(i);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private string GetNameForEntities(int i)
        {
            ThrowExceptionIfNotAvailable();
            if (i == Properties.Count)
            {
                return "__RepoDb_OrderColumn";
            }
            return Properties[i].GetMappedName();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private string GetNameForDictionaryStringObject(int i)
        {
            ThrowExceptionIfNotAvailable();
            if (i == Fields.Count)
            {
                return "__RepoDb_OrderColumn";
            }
            return Fields[i].Name;
        }

        /// <summary>
        /// Gets the index of the property based on the property name.
        /// </summary>
        /// <param name="name">The index of the property.</param>
        /// <returns>The index of the property from property name.</returns>
        public override int GetOrdinal(string name) =>
            isDictionaryStringObject ? GetOrdinalForDictionaryStringObject(name) : GetOrdinalForEntities(name);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private int GetOrdinalForEntities(string name)
        {
            ThrowExceptionIfNotAvailable();
            if (HasOrderingColumn && string.Equals(name, "__RepoDb_OrderColumn", StringComparison.OrdinalIgnoreCase))
            {
                return Properties.Count;
            }
            else
            {
                var property = Properties.FirstOrDefault(p => string.Equals(p.GetMappedName(), name, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(p.PropertyInfo.Name, name, StringComparison.OrdinalIgnoreCase));
                return Properties.IndexOf(property);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private int GetOrdinalForDictionaryStringObject(string name)
        {
            ThrowExceptionIfNotAvailable();
            if (HasOrderingColumn && string.Equals(name, "__RepoDb_OrderColumn", StringComparison.OrdinalIgnoreCase))
            {
                return Fields.Count;
            }
            else
            {
                return Fields.IndexOf(Fields.FirstOrDefault(f =>
                    string.Equals(f.Name, name, StringComparison.OrdinalIgnoreCase)));
            }
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
            return Converter.ToType<string>(GetValue(i));
        }

        /// <summary>
        /// Gets the current value from the defined property index.
        /// </summary>
        /// <param name="i">The index of the property.</param>
        /// <returns>The value from the property index.</returns>
        public override object GetValue(int i) =>
            isDictionaryStringObject ? GetValueForDictionaryStringObject(i) : GetValueForEntities(i);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public object GetValueForEntities(int i)
        {
            ThrowExceptionIfNotAvailable();
            if (i == Properties.Count)
            {
                return position;
            }
            else
            {
                return Properties[i].PropertyInfo.GetValue(Enumerator.Current);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public object GetValueForDictionaryStringObject(int i)
        {
            ThrowExceptionIfNotAvailable();
            if (i == Fields.Count)
            {
                return position;
            }
            else
            {
                var dictionary = Enumerator.Current as IDictionary<string, object>;
                return dictionary?[Fields[i].Name];
            }
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
            position++;
            recordsAffected++;
            return Enumerator.MoveNext();
        }

        /// <summary>
        /// 
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IEnumerable<ClassProperty> GetClassProperties()
        {
            if (isDictionaryStringObject)
            {
                return Enumerable.Empty<ClassProperty>();
            }
            return PropertyCache.Get(EntityType)?.AsList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        private IEnumerable<Field> GetFields(IDictionary<string, object> dictionary)
        {
            if (dictionary != null)
            {
                foreach (var kvp in dictionary)
                {
                    yield return new Field(kvp.Key, kvp.Value?.GetType());
                }
            }
        }
    }
}
