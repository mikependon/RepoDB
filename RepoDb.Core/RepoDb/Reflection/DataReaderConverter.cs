using System.Collections.Generic;
using System.Data.Common;
using System.Dynamic;

namespace RepoDb.Reflection
{
    /// <summary>
    /// A static classed used to convert the <see cref="DbDataReader"/> into data entity object.
    /// </summary>
    public static class DataReaderConverter
    {
        /// <summary>
        /// Converts the <see cref="DbDataReader"/> into an enumerable of data entity object.
        /// </summary>
        /// <typeparam name="TEntity">The data entity type to convert.</typeparam>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <returns>An array of data entity objects.</returns>
        public static IEnumerable<TEntity> ToEnumerable<TEntity>(DbDataReader reader)
            where TEntity : class
        {
            return ToEnumerable<TEntity>(reader, false);
        }

        /// <summary>
        /// Converts the <see cref="DbDataReader"/> into an enumerable of data entity object.
        /// </summary>
        /// <typeparam name="TEntity">The data entity type to convert.</typeparam>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="basedOnFields">Check whether to create a delegate based on the data reader fields.</param>
        /// <returns>An array of data entity objects.</returns>
        internal static IEnumerable<TEntity> ToEnumerable<TEntity>(DbDataReader reader, bool basedOnFields = false)
            where TEntity : class
        {
            if (reader != null && reader.IsClosed == false && reader.HasRows)
            {
                var func = FunctionCache.GetDataReaderToDataEntityFunction<TEntity>(reader, basedOnFields);
                while (reader.Read())
                {
                    yield return func(reader);
                }
            }
        }

        /// <summary>
        /// Converts the <see cref="DbDataReader"/> into an enumerable of <see cref="ExpandoObject"/> object.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <returns>An array of <see cref="ExpandoObject"/> objects.</returns>
        public static IEnumerable<dynamic> ToEnumerable(DbDataReader reader)
        {
            return ToEnumerable(reader, false);
        }

        /// <summary>
        /// Converts the <see cref="DbDataReader"/> into an enumerable of <see cref="ExpandoObject"/> object.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <param name="basedOnFields">Check whether to create a delegate based on the data reader fields.</param>
        /// <returns>An array of <see cref="ExpandoObject"/> objects.</returns>
        internal static IEnumerable<dynamic> ToEnumerable(DbDataReader reader, bool basedOnFields = false)
        {
            if (reader != null && reader.HasRows)
            {
                var @delegate = DelegateCache.GetDataReaderToExpandoObjectDelegate(reader, basedOnFields);
                while (reader.Read())
                {
                    yield return @delegate(reader);
                }
            }
        }
    }
}
