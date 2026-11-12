#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

#if !NETSTANDARD2_0

using System.Collections.Generic;
using System.Linq.Expressions;

namespace RepoDb.Models
{
    /// <summary>
    /// The access to a value inside a JSON column, as found in a query expression.
    /// </summary>
    public sealed class JsonAccess
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="JsonAccess"/> class.
        /// </summary>
        /// <param name="column">The expression of the JSON column.</param>
        /// <param name="segments">The property and array index segments inside the JSON.</param>
        public JsonAccess(MemberExpression column,
            IReadOnlyList<string> segments)
        {
            Column = column;
            Segments = segments;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the expression of the JSON column.
        /// </summary>
        public MemberExpression Column { get; }

        /// <summary>
        /// Gets the property and array index segments inside the JSON.
        /// </summary>
        public IReadOnlyList<string> Segments { get; }

        #endregion
    }
}

#endif
