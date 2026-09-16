#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to hold the <see cref="RepoDb.QueryGroup"/> object type mapping. This class has been introduced
    /// to support the needs of the multi-resultsets query operation.
    /// </summary>
    /// <remarks>
    /// Creates an instance of <see cref="QueryGroupTypeMap"/> class.
    /// </remarks>
    /// <param name="queryGroup">The <see cref="RepoDb.QueryGroup"/> object.</param>
    /// <param name="type">The type where the <see cref="RepoDb.QueryGroup"/> object is mapped.</param>
    internal readonly struct QueryGroupTypeMap(QueryGroup queryGroup,
        Type type)
    {

        /// <summary>
        /// Gets the current associated <see cref="RepoDb.QueryGroup"/> object.
        /// </summary>
        public QueryGroup QueryGroup { get; } = queryGroup;

        /// <summary>
        /// Gets the type where the current <see cref="RepoDb.QueryGroup"/> is mapped.
        /// </summary>
        public Type MappedType { get; } = type;
    }
}
