#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// Collects the errors and the warnings that the strict checks find in a definition.
    /// An error is a violation that makes the definition invalid. A warning is content that some databases allow, or that cannot be proven.
    /// </summary>
    internal sealed class DefinitionFindings
    {
        #region Properties

        /// <summary>
        /// Gets the violations that make the definition invalid.
        /// </summary>
        internal IList<string> Errors { get; } = new List<string>();

        /// <summary>
        /// Gets the suspicious content that does not make the definition invalid.
        /// </summary>
        internal IList<string> Warnings { get; } = new List<string>();

        #endregion

        #region Internal Methods

        /// <summary>
        /// Gets the readable name of a table, qualified with its schema if it has one.
        /// </summary>
        /// <param name="table">The identity of the table.</param>
        /// <returns>The readable name of the table.</returns>
        internal static string Describe(TableInfo table) =>
            string.IsNullOrWhiteSpace(table.Schema) ? table.Name : string.Concat(table.Schema, ".", table.Name);

        /// <summary>Identifies a named object, or its one-based position when unnamed.</summary>
        internal static string ObjectName(string name, int position) =>
            string.IsNullOrWhiteSpace(name) ? $"#{position + 1} (unnamed)" : $"'{name}'";

        /// <summary>Finds an object's position without conflating equal unnamed objects.</summary>
        internal static int Position<T>(IList<T> items, T item) where T : class
        {
            for (var index = 0; index < items.Count; index++)
                if (ReferenceEquals(items[index], item))
                    return index;
            return -1;
        }

        /// <summary>Builds a location within the definition model.</summary>
        internal static string Location(TableSchema table, string collection, string name = null, string property = null) =>
            $"Tables > {Describe(table.Table)} > {collection}" +
            (name == null ? "" : $" > {name}") + (property == null ? "" : $" > {property}");

        /// <summary>
        /// Adds an error that was found in a table.
        /// </summary>
        /// <param name="table">The table that has the problem.</param>
        /// <param name="message">The description of the problem.</param>
        internal void AddError(TableSchema table,
            string message)
        {
            Errors.Add(Format(table, message));
        }

        /// <summary>
        /// Adds an error that is about more than one table, or about the definition as a whole.
        /// </summary>
        /// <param name="message">The description of the problem.</param>
        internal void AddError(string message)
        {
            Errors.Add(message);
        }

        /// <summary>
        /// Adds a warning that was found in a table.
        /// </summary>
        /// <param name="table">The table that has the concern.</param>
        /// <param name="message">The description of the concern.</param>
        internal void AddWarning(TableSchema table,
            string message)
        {
            Warnings.Add(Format(table, message));
        }

        /// <summary>
        /// Adds a warning that is about more than one table, or about the definition as a whole.
        /// </summary>
        /// <param name="message">The description of the concern.</param>
        internal void AddWarning(string message)
        {
            Warnings.Add(message);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Prefixes the message with the table that it is about.
        /// </summary>
        /// <param name="table">The table that the message is about.</param>
        /// <param name="message">The message.</param>
        /// <returns>The message, prefixed with the readable name of the table.</returns>
        private static string Format(TableSchema table,
            string message) =>
            string.Concat("Table '", Describe(table.Table), "': ", message);

        #endregion
    }
}
