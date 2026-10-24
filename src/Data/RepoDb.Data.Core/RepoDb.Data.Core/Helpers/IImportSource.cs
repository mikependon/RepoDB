#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb.Data.Helpers
{
    /// <summary>
    /// A file that holds the rows of a table (JSON, XML, CSV), read forward only without loading it in memory. The <c>ImportDataFrom*</c> operations load the rows
    /// of any source the same way (see <see cref="ImportPipeline"/>).
    /// </summary>
    internal interface IImportSource
    {
        /// <summary>
        /// Gets a value that indicates whether <see cref="ReadColumns"/> gives every column of the file (a CSV header does, while the columns of the rows of a JSON or XML file
        /// are found by looking at the first rows only). A column that a mapping names must be one of them only if they are every column.
        /// </summary>
        bool HasAllColumns { get; }

        /// <summary>
        /// Validates the options of the source (i.e.: the separator of a CSV file).
        /// </summary>
        /// <exception cref="ArgumentException">An option of the source is not valid.</exception>
        void Validate();

        /// <summary>
        /// Reads the file to find its columns: the names without repeating a name (the case is ignored), in the order that they were found.
        /// </summary>
        /// <returns>The names of the columns.</returns>
        IList<string> ReadColumns();

        /// <summary>
        /// Reads the file to find its columns in an asynchronous way.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the names of the columns.</returns>
        Task<IList<string>> ReadColumnsAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Counts the rows of the file.
        /// </summary>
        /// <returns>The number of rows.</returns>
        long CountRows();

        /// <summary>
        /// Counts the rows of the file in an asynchronous way.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the number of rows.</returns>
        Task<long> CountRowsAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Opens the file to read its rows.
        /// </summary>
        /// <returns>The reader of the rows, to be disposed by the caller.</returns>
        IImportRowReader Open();
    }

    /// <summary>
    /// Reads the rows of a file forward only.
    /// </summary>
    internal interface IImportRowReader : IDisposable
    {
        /// <summary>
        /// Gets the number of the row that was read last (the rows are counted from 1).
        /// </summary>
        long RowNumber { get; }

        /// <summary>
        /// Reads the next row.
        /// </summary>
        /// <param name="index">The position of each column (by its name, without the case) in the array of the values. The columns that are not in it are left out.</param>
        /// <param name="width">The number of columns in the array of the values.</param>
        /// <returns>The values by the position of the columns (<c>null</c> for a missing value), or <c>null</c> if there is no other row.</returns>
        /// <exception cref="FormatException">The file is not valid, or a row is not valid.</exception>
        object[] Next(IDictionary<string, int> index,
            int width);

        /// <summary>
        /// Reads the next row in an asynchronous way.
        /// </summary>
        /// <param name="index">The position of each column (by its name, without the case) in the array of the values. The columns that are not in it are left out.</param>
        /// <param name="width">The number of columns in the array of the values.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the values by the position of the columns, or <c>null</c> if there is no other row.</returns>
        /// <exception cref="FormatException">The file is not valid, or a row is not valid.</exception>
        Task<object[]> NextAsync(IDictionary<string, int> index,
            int width,
            CancellationToken cancellationToken);
    }
}
