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
    /// A class that represents the collection of <see cref="CopyDataRow"/> objects of the data being copied.
    /// </summary>
    public class CopyDataTable
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="CopyDataTable"/> class.
        /// </summary>
        /// <param name="dataRows">The list of <see cref="CopyDataRow"/> objects.</param>
        public CopyDataTable(IList<CopyDataRow> dataRows)
        {
            DataRows = dataRows ?? throw new ArgumentNullException(nameof(dataRows));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the list of <see cref="CopyDataRow"/> objects.
        /// </summary>
        public IList<CopyDataRow> DataRows { get; }

        #endregion
    }
}
