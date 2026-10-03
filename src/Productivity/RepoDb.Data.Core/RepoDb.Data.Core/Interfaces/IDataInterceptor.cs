#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Data
{
    /// <summary>
    /// An interface that is used to transform the data of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the data to be transformed.</typeparam>
    public interface IDataInterceptor<T>
    {
        /// <summary>
        /// Transforms the data.
        /// </summary>
        /// <param name="data">The data to be transformed.</param>
        /// <returns>The transformed data.</returns>
        T Transform(T data);
    }
}
