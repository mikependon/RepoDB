using System;
using System.Data.Common;
using System.Dynamic;

namespace RepoDb.Reflection.Delegates
{
    /// <summary>
    /// A compiled function that is used to map the <see cref="DbDataReader"/> into data entity object.
    /// </summary>
    /// <typeparam name="TEntity">The data entity object to map.</typeparam>
    /// <returns>An instance of data entity object containing the values from the <see cref="DbDataReader"/> object.</returns>
    public delegate Func<DbDataReader, TEntity> DataReaderToDataEntityFunction<TEntity>()
        where TEntity : class;

    /// <summary>
    /// A compiled function that is used to map the <see cref="DbDataReader"/> into <see cref="ExpandoObject"/> object.
    /// </summary>
    /// <returns>An instance of <see cref="ExpandoObject"/> object containing the values from the <see cref="DbDataReader"/> object.</returns>
    public delegate Func<DbDataReader, ExpandoObject> DataReaderToExpandoObjectFunction();
}
