using RepoDb.Reflection;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace RepoDb
{
    /// <summary>
    /// A class used to extract the multiple resultsets of the query operation.
    /// </summary>
    public class QueryMultipleExtractor : IDisposable
    {
        private DbDataReader m_reader = null;

        /// <summary>
        /// Creates a new instance of <see cref="QueryMultipleExtractor"/> class.
        /// </summary>
        /// <param name="reader">The data reader to be used for extraction.</param>
        internal QueryMultipleExtractor(DbDataReader reader)
        {
            m_reader = reader;
            Position = 0;
        }

        /// <summary>
        /// Disposes the current instance of <see cref="QueryMultipleExtractor"/>.
        /// </summary>
        public void Dispose()
        {
            m_reader?.Dispose();
        }

        /// <summary>
        /// Extract the <see cref="DbDataReader"/> object into an enumerable of data entity objects.
        /// </summary>
        /// <typeparam name="TEntity">The type of data entity to be extracted.</typeparam>
        /// <returns>An enumerable of extracted data entity.</returns>
        public IEnumerable<TEntity> Extract<TEntity>() where TEntity : class
        {
            var result = DataReaderConverter.ToEnumerable<TEntity>(m_reader, true).ToList();

            // Move to next result
            NextResult();

            // Return the result
            return result;
        }

        /// <summary>
        /// Converts the first column of the first row of the <see cref="DbDataReader"/> to an object.
        /// </summary>
        /// <returns>An instance of extracted object as value result.</returns>
        public object Scalar()
        {
            var value = (object)null;

            // Only if there are record
            if (m_reader.Read())
            {
                value = ObjectConverter.DbNullToNull(m_reader[0]);
            }

            // Move to next result
            NextResult();

            // Return the result
            return value;
        }

        /// <summary>
        /// Converts the first column of the first row of the <see cref="DbDataReader"/> to an object.
        /// </summary>
        /// <typeparam name="T">The target return type.</typeparam>
        /// <returns>An instance of extracted object as value result.</returns>
        public T Scalar<T>()
        {
            var value = default(T);

            // Only if there are record
            if (m_reader.Read())
            {
                value = (T)ObjectConverter.DbNullToNull(m_reader[0]);
            }

            // Move to next result
            NextResult();

            // Return the result
            return value;
        }

        /// <summary>
        /// Gets the position of the <see cref="DbDataReader"/>.
        /// </summary>
        public int Position { get; private set; }

        /// <summary>
        /// Advances the <see cref="DbDataReader"/> object to the next result.
        /// <returns>True if there are more result sets; otherwise false.</returns>
        /// </summary>
        public bool NextResult()
        {
            return (Position = m_reader.NextResult() ? Position + 1 : -1) >= 0;
        }
    }
}
