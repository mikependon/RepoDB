#region Copyright Attributions

// Copyright (c) 2021 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.PostgreSql.BulkOperations
{
    /// <summary>
    /// 
    /// </summary>
    internal class IdentityResult
    {
        public int Index { get; set; }
        public long Identity { get; set; }
    }
}
