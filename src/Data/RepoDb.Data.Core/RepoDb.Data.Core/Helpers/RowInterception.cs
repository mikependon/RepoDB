#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using RepoDb.Data.Interfaces;
using RepoDb.Data.Models;

namespace RepoDb.Data.Helpers
{
    /// <summary>
    /// Runs the interceptors of the copy over a row of the source: the row is read as a <see cref="Row"/>, then every interceptor is called in the order that they were given,
    /// and each one receives what the previous one returned. What the last one returns is the row that is copied.
    /// </summary>
    internal static class RowInterception
    {
        #region Public Methods

        /// <summary>
        /// Checks the interceptors that the user gave.
        /// </summary>
        /// <param name="interceptors">The interceptors, or <c>null</c>.</param>
        /// <returns>The interceptors, or <c>null</c> if there are none.</returns>
        /// <exception cref="ArgumentException">An interceptor is <c>null</c>.</exception>
        public static IList<IDataInterceptor<Row>> Normalize(IList<IDataInterceptor<Row>> interceptors)
        {
            if (interceptors == null || interceptors.Count == 0)
            {
                return null;
            }
            foreach (var interceptor in interceptors)
            {
                if (interceptor == null)
                {
                    throw new ArgumentException("The data interceptors must not contain a null item.", "dataInterceptors");
                }
            }
            return interceptors;
        }

        /// <summary>
        /// Gets the columns of the rows of a reader: their names and types. The list is shared by the rows that are read with it.
        /// </summary>
        /// <param name="record">The reader (or the record) that has the rows.</param>
        /// <returns>The columns.</returns>
        public static List<Column> GetColumns(IDataRecord record)
        {
            var columns = new List<Column>(record.FieldCount);
            for (var i = 0; i < record.FieldCount; i++)
            {
                columns.Add(new Column(record.GetName(i), record.GetFieldType(i)));
            }
            return columns;
        }

        /// <summary>
        /// Reads the current row of the reader as a <see cref="Row"/> (a database null is a <c>null</c> value).
        /// </summary>
        /// <param name="record">The reader (or the record) that is on the row.</param>
        /// <param name="columns">The columns of the reader (see <see cref="GetColumns"/>).</param>
        /// <returns>The row.</returns>
        public static Row Read(IDataRecord record,
            List<Column> columns)
        {
            var values = new object[columns.Count];
            record.GetValues(values);
            for (var i = 0; i < values.Length; i++)
            {
                if (values[i] is DBNull)
                {
                    values[i] = null;
                }
            }
            return new Row(columns, values);
        }

        /// <summary>
        /// Runs the interceptors over the row, in order: each one receives the row that the previous one returned.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="interceptors">The interceptors.</param>
        /// <returns>The row that the last interceptor returned.</returns>
        /// <exception cref="InvalidOperationException">An interceptor returned <c>null</c>.</exception>
        public static Row Apply(Row row,
            IList<IDataInterceptor<Row>> interceptors)
        {
            for (var i = 0; i < interceptors.Count; i++)
            {
                row = interceptors[i].Transform(row) ?? throw new InvalidOperationException($"The data interceptor at the position {i} returned null: it must return the row that is copied.");
            }
            return row;
        }

        #endregion
    }
}
