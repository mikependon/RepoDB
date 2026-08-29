#region Copyright Attributions

// Copyright (c) 2018 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Data.Common;
using System.Dynamic;

namespace RepoDb.Reflection.Delegates
{
    /// <summary>
    /// A delegate used to map the <see cref="DbDataReader"/> into data entity object.
    /// </summary>
    /// <typeparam name="TEntity">The data entity object to map.</typeparam>
    /// <param name="dataReader">An instance of <see cref="DbDataReader"/> to be mapped.</param>
    /// <returns>An instance of data entity object containing the values from the <see cref="DbDataReader"/> object.</returns>
    public delegate TEntity DataReaderToDataEntityDelegate<TEntity>(DbDataReader dataReader)
        where TEntity : class;

    /// <summary>
    /// A delegate used to map the <see cref="DbDataReader"/> into <see cref="ExpandoObject"/> object.
    /// </summary>
    /// <param name="dataReader">An instance of <see cref="DbDataReader"/> to be mapped.</param>
    /// <returns>An instance of <see cref="ExpandoObject"/> object containing the values from the <see cref="DbDataReader"/> object.</returns>
    public delegate ExpandoObject DataReaderToExpandoObjectDelegate(DbDataReader dataReader);
}
