#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;

namespace RepoDb.Data.Models
{
    /// <summary>
    /// A class that represents a column (name and type) of the data being copied.
    /// </summary>
    public class CopyDataColumn
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="CopyDataColumn"/> class.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <param name="type">The type of the column.</param>
        public CopyDataColumn(string name,
            Type type)
        {
            Name = name;
            Type = type;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the type of the column.
        /// </summary>
        public Type Type { get; }

        #endregion
    }
}
