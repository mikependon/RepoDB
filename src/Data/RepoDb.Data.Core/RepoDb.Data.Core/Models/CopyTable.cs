#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;

namespace RepoDb.Data.Models
{
    /// <summary>
    /// A class that represents the collection of <see cref="CopyRow"/> objects of the data being copied.
    /// </summary>
    public class CopyTable
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="CopyTable"/> class.
        /// </summary>
        /// <param name="dataRows">The list of <see cref="CopyRow"/> objects.</param>
        public CopyTable(IList<CopyRow> dataRows)
        {
            DataRows = dataRows ?? throw new ArgumentNullException(nameof(dataRows));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the list of <see cref="CopyRow"/> objects.
        /// </summary>
        public IList<CopyRow> DataRows { get; }

        #endregion
    }
}
