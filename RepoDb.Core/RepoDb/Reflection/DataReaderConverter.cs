using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;

namespace RepoDb.Reflection
{
    /// <summary>
    /// A static classed used to convert the <see cref="DbDataReader"/> into data entity or dynamic objects.
    /// </summary>
    [Obsolete("Use the DataReader class.")]
    public static class DataReaderConverter
    {
        /// <summary>
        /// Converts the <see cref="DbDataReader"/> into an enumerable of data entity object.
        /// </summary>
        /// <typeparam name="TEntity">The data entity type to convert.</typeparam>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="connection">The used <see cref="IDbConnection"/> object.</param>
        /// <returns>An array of data entity objects.</returns>
        [Obsolete("Use the DataReader.ToEnumerable<TEntity>() method.")]
        public static IEnumerable<TEntity> ToEnumerable<TEntity>(DbDataReader reader, IDbConnection connection)
            where TEntity : class
        {
            return DataReader.ToEnumerable<TEntity>(reader, connection, false);
        }

        /// <summary>
        /// Converts the <see cref="DbDataReader"/> into an enumerable of <see cref="ExpandoObject"/> object.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <returns>An array of <see cref="ExpandoObject"/> objects.</returns>
        [Obsolete("Use the DataReader.ToEnumerable() method.")]
        public static IEnumerable<dynamic> ToEnumerable(DbDataReader reader)
        {
            return DataReader.ToEnumerable(reader);
        }
    }
}
