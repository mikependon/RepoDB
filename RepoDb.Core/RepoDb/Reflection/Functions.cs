using System;
using System.Data.Common;

namespace RepoDb.Reflection.Delegates
{
    /// <summary>
    /// A function used to map the <see cref="DbDataReader"/> into data entity object.
    /// </summary>
    /// <typeparam name="TEntity">The data entity object to map.</typeparam>
    /// <returns>An instance of data entity object containing the values from the <see cref="DbDataReader"/> object.</returns>
    public delegate Func<DbDataReader, TEntity> DataReaderToDataEntityFunction<TEntity>()
        where TEntity : class;
}
