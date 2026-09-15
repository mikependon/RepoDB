#region Copyright Attributions

// Copyright (c) 2026 bpaolo71 and Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Data;
using RepoDb.Enumerations;

namespace RepoDb.Extensions.QueryFields
{
    public class ReplaceQueryField : FunctionalQueryField
    {
        /// <summary>
        /// A functional-based <see cref="QueryField"/> object that is using the REPLACE function.
        /// </summary>
        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="operation">The operation to be used for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        /// <param name="searchValue">The value to be searched for in the query expression.</param>
        /// <param name="replaceValue">The value to replace the search value with in the query expression.</param>
        public ReplaceQueryField(
            string fieldName,
            Operation operation,
            object value,
            string searchValue,
            string replaceValue)
            : this(fieldName, operation, value, null, searchValue, replaceValue)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReplaceQueryField"/> class.
        /// </summary>
        /// <param name="fieldName">The name of the field for the query expression.</param>
        /// <param name="operation">The operation to be used for the query expression.</param>
        /// <param name="value">The value to be used for the query expression.</param>
        /// <param name="dbType">The database type of the value.</param>
        /// <param name="searchValue">The value to be searched for in the query expression.</param>
        /// <param name="replaceValue">The value to replace the search value with in the query expression.</param>
        public ReplaceQueryField(
            string fieldName,
            Operation operation,
            object value,
            DbType? dbType,
            string searchValue,
            string replaceValue)
            : base(fieldName, operation, value, dbType, $"REPLACE({{0}}, '{searchValue}', '{replaceValue}')")
        { }
    }
}
