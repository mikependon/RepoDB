#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Data.Models
{
    /// <summary>
    /// A class that represents a single value and its column within a row of data.
    /// </summary>
    public class CopyRow
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="CopyRow"/> class.
        /// </summary>
        /// <param name="column">The column of the value.</param>
        /// <param name="value">The value of the column.</param>
        public CopyRow(CopyColumn column,
            object value)
        {
            Column = column;
            Value = value;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the column of the value.
        /// </summary>
        public CopyColumn Column { get; }

        /// <summary>
        /// Gets the value of the column.
        /// </summary>
        public object Value { get; }

        #endregion
    }
}
