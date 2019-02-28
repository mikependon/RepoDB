using RepoDb.Reflection;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace RepoDb
{
    /// <summary>
    /// A class used to extract the multiple result of the query operation.
    /// </summary>
    public class QueryMultipleExtractor : IDisposable
    {
        private DbDataReader m_reader = null;
        private bool m_isScalarForwarded = false;

        /// <summary>
        /// Creates a new instance of <see cref="QueryMultipleExtractor"/> class.
        /// </summary>
        /// <param name="reader">The data reader to be used for extraction.</param>
        internal QueryMultipleExtractor(DbDataReader reader)
        {
            m_reader = reader;
        }

        /// <summary>
        /// Disposes the current instance of <see cref="QueryMultipleExtractor"/>.
        /// </summary>
        public void Dispose()
        {
            m_reader?.Dispose();
        }

        /// <summary>
        /// Extract the data reader to an enumerable of target data entity type.
        /// </summary>
        /// <typeparam name="TEntity">The target data entity to extract.</typeparam>
        /// <param name="yielded">Determines whether the extraction of data is being streamed directly from the data reader.</param>
        /// <returns>An enumerable of target data entity.</returns>
        public IEnumerable<TEntity> Extract<TEntity>(bool yielded = false) where TEntity : class
        {
            if (yielded)
            {
                return DataReaderConverter.ToEnumerable<TEntity>(m_reader, true);
            }
            else
            {
                return DataReaderConverter.ToEnumerable<TEntity>(m_reader, true).ToList();
            }
        }

        /// <summary>
        /// Advances the pointer of the data reader to the next resultset and extract it to an enumerable of 
        /// target data entity type.
        /// </summary>
        /// <typeparam name="TEntity">The target data entity to extract.</typeparam>
        /// <param name="yielded">Determines whether the extraction of data is being streamed directly from the data reader.</param>
        /// <param name="throwException">A value that indicates whether to throw an exception if there is no next resultset. Default is <see cref="bool"/>.</param>
        /// <returns>An enumerable of target data entity.</returns>
        public IEnumerable<TEntity> ExtractNext<TEntity>(bool yielded = false, bool throwException = true) where TEntity : class
        {
            if (NextResult())
            {
                if (yielded)
                {
                    return DataReaderConverter.ToEnumerable<TEntity>(m_reader, true);
                }
                else
                {
                    return DataReaderConverter.ToEnumerable<TEntity>(m_reader, true).ToList();
                }
            }
            else
            {
                if (throwException)
                {
                    throw new InvalidOperationException("The current data reader does not contain further resultset.");
                }
            }
            return null;
        }

        /// <summary>
        /// Converts the first column of the first row of the data reader to an object.
        /// </summary>
        /// <returns>An instance of extracted object as value result.</returns>
        public object Scalar()
        {
            if (m_isScalarForwarded == true)
            {
                return ObjectConverter.DbNullToNull(m_reader[0]);
            }
            else
            {
                if (m_reader.Read())
                {
                    return ObjectConverter.DbNullToNull(m_reader[0]);
                }
            }
            return null;
        }

        /// <summary>
        /// Advances the pointer of the data reader to the next resultset and converts the first column of the first row of the data reader to an object.
        /// </summary>
        /// <param name="throwException">A value that indicates whether to throw an exception if there is no next resultset. Default is <see cref="bool"/>.</param>
        /// <returns>An instance of extracted object as value result.</returns>
        public object ScalarNext(bool throwException = true)
        {
            if (NextResult())
            {
                if (m_reader.Read())
                {
                    return ObjectConverter.DbNullToNull(m_reader[0]);
                }
            }
            else
            {
                if (throwException)
                {
                    throw new InvalidOperationException("The current data reader does not contain further resultset.");
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the current position of the data reader based on the multiple query result.
        /// </summary>
        public int Position { get; private set; }

        /// <summary>
        /// Advances the current data reader object to the next result.
        /// <returns>True if there are more result sets; otherwise false.</returns>
        /// </summary>
        public bool NextResult()
        {
            var result = m_reader.NextResult();
            if (result)
            {
                Position++;
                m_isScalarForwarded = true;
            }
            return result;
        }
    }
}
