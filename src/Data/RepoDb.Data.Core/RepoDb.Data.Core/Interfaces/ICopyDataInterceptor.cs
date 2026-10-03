#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Data.Interfaces
{
    /// <summary>
    /// An interface that is used to transform the data being copied, either per row or as a whole table.
    /// </summary>
    public interface ICopyDataInterceptor :
        IDataInterceptor<CopyDataRow>,
        IDataInterceptor<CopyDataTable>
    {
    }
}
