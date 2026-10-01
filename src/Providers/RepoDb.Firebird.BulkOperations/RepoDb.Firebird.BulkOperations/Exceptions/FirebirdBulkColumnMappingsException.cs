#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Enumerations.Firebird;
using RepoDb.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RepoDb.Exceptions
{
    /// <summary>
    /// An exception that is being thrown by a bulk operation for Firebird when the columns of the source are not aligned
    /// with the columns of the destination table, as required by the <see cref="FirebirdBulkColumnMappingsBehavior"/> in used.
    /// It is thrown before any data is written.
    /// </summary>
    public class FirebirdBulkColumnMappingsException : Exception
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="FirebirdBulkColumnMappingsException"/> class.
        /// </summary>
        public FirebirdBulkColumnMappingsException()
            : this("The columns of the source are not aligned with the columns of the destination table.")
        { }

        /// <summary>
        /// Creates a new instance of <see cref="FirebirdBulkColumnMappingsException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public FirebirdBulkColumnMappingsException(string message)
            : base(message)
        {
            SourceColumnsNotInDestination = Array.Empty<string>();
            DestinationColumnsNotInSource = Array.Empty<string>();
        }

        /// <summary>
        /// Creates a new instance of <see cref="FirebirdBulkColumnMappingsException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public FirebirdBulkColumnMappingsException(string message,
            Exception innerException)
            : base(message, innerException)
        {
            SourceColumnsNotInDestination = Array.Empty<string>();
            DestinationColumnsNotInSource = Array.Empty<string>();
        }

        /// <summary>
        /// Creates a new instance of <see cref="FirebirdBulkColumnMappingsException"/> class.
        /// </summary>
        /// <param name="tableName">The name of the destination table.</param>
        /// <param name="behavior">The column mappings behavior that was violated.</param>
        /// <param name="sourceColumnsNotInDestination">The columns of the source that do not exist on the destination table.</param>
        /// <param name="destinationColumnsNotInSource">The columns of the destination table that are not supplied by the source.</param>
        /// <param name="isOrderMismatch">The value that indicates whether the columns exist on both sides but in a different order.</param>
        /// <param name="sourceColumns">The columns of the source, in order (used to describe an order mismatch).</param>
        /// <param name="destinationColumns">The columns of the destination table, in order (used to describe an order mismatch).</param>
        public FirebirdBulkColumnMappingsException(string tableName,
            FirebirdBulkColumnMappingsBehavior behavior,
            IEnumerable<string> sourceColumnsNotInDestination,
            IEnumerable<string> destinationColumnsNotInSource,
            bool isOrderMismatch = false,
            IEnumerable<string> sourceColumns = null,
            IEnumerable<string> destinationColumns = null)
            : base(CreateMessage(tableName,
                behavior,
                sourceColumnsNotInDestination,
                destinationColumnsNotInSource,
                isOrderMismatch,
                sourceColumns,
                destinationColumns))
        {
            TableName = tableName;
            Behavior = behavior;
            SourceColumnsNotInDestination = sourceColumnsNotInDestination?.ToArray() ?? Array.Empty<string>();
            DestinationColumnsNotInSource = destinationColumnsNotInSource?.ToArray() ?? Array.Empty<string>();
            IsOrderMismatch = isOrderMismatch;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the destination table.
        /// </summary>
        public string TableName { get; }

        /// <summary>
        /// Gets the column mappings behavior that was violated.
        /// </summary>
        public FirebirdBulkColumnMappingsBehavior Behavior { get; }

        /// <summary>
        /// Gets the columns of the source that do not exist on the destination table.
        /// </summary>
        public IReadOnlyList<string> SourceColumnsNotInDestination { get; }

        /// <summary>
        /// Gets the columns of the destination table that are not supplied by the source.
        /// </summary>
        public IReadOnlyList<string> DestinationColumnsNotInSource { get; }

        /// <summary>
        /// Gets the value that indicates whether the columns exist on both sides but in a different order.
        /// </summary>
        public bool IsOrderMismatch { get; }

        #endregion

        #region Helpers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="behavior"></param>
        /// <param name="sourceColumnsNotInDestination"></param>
        /// <param name="destinationColumnsNotInSource"></param>
        /// <param name="isOrderMismatch"></param>
        /// <param name="sourceColumns"></param>
        /// <param name="destinationColumns"></param>
        /// <returns></returns>
        private static string CreateMessage(string tableName,
            FirebirdBulkColumnMappingsBehavior behavior,
            IEnumerable<string> sourceColumnsNotInDestination,
            IEnumerable<string> destinationColumnsNotInSource,
            bool isOrderMismatch,
            IEnumerable<string> sourceColumns,
            IEnumerable<string> destinationColumns)
        {
            static string Quote(IEnumerable<string> columns) =>
                columns.Select(column => $"'{column}'").Join(", ");

            var builder = new StringBuilder()
                .Append($"The columns of the source are not aligned with the columns of the destination table '{tableName}' ")
                .Append($"(column mappings behavior: '{behavior}').");

            if (sourceColumnsNotInDestination?.Any() == true)
            {
                builder.Append($" Source column(s) that do not exist on the destination table: {Quote(sourceColumnsNotInDestination)}.");
            }

            if (destinationColumnsNotInSource?.Any() == true)
            {
                builder.Append($" Destination column(s) that are not supplied by the source: {Quote(destinationColumnsNotInSource)}.");
            }

            if (isOrderMismatch)
            {
                builder.Append(" The source columns must be in the same order as the destination columns.");
                if (destinationColumns != null && sourceColumns != null)
                {
                    builder.Append($" Expected: {Quote(destinationColumns)}. Actual: {Quote(sourceColumns)}.");
                }
            }

            return builder.ToString();
        }

        #endregion
    }
}
