#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// Checks the SQL expressions of a table (the check constraints, the computed columns and the index filters) against its columns.
    /// The expressions are native SQL that is not parsed, so only the quoted identifiers can be checked, and those findings are warnings.
    /// </summary>
    internal static class SchemaDefinitionExpressionRules
    {
        #region Internal Methods

        /// <summary>
        /// Checks the expressions of the table.
        /// </summary>
        /// <param name="table">The table to be checked.</param>
        /// <param name="findings">The findings that receive the problems.</param>
        internal static void Check(TableSchema table,
            DefinitionFindings findings)
        {
            var columns = new HashSet<string>(table.Columns.Select(column => column.Field.Name),
                StringComparer.OrdinalIgnoreCase);
            CheckCheckConstraints(table,
                columns,
                findings);
            CheckComputedColumns(table,
                columns,
                findings);
            CheckIndexFilters(table,
                columns,
                findings);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Checks the check constraints: each one must have an expression, and its quoted identifiers should be columns.
        /// </summary>
        /// <param name="table">The table that has the check constraints.</param>
        /// <param name="columns">The names of the columns of the table.</param>
        /// <param name="findings">The findings that receive the problems.</param>
        private static void CheckCheckConstraints(TableSchema table,
            HashSet<string> columns,
            DefinitionFindings findings)
        {
            foreach (var constraint in table.CheckConstraints)
            {
                if (string.IsNullOrWhiteSpace(constraint.Expression))
                {
                    findings.AddError(table, $"The check constraint '{constraint.Name}' has no expression.");
                    continue;
                }
                CheckExpression(table,
                    columns,
                    constraint.Expression,
                    $"check constraint '{constraint.Name}'",
                    findings);
            }
        }

        /// <summary>
        /// Checks the quoted identifiers of the expressions of the computed columns.
        /// </summary>
        /// <param name="table">The table that has the computed columns.</param>
        /// <param name="columns">The names of the columns of the table.</param>
        /// <param name="findings">The findings that receive the problems.</param>
        private static void CheckComputedColumns(TableSchema table,
            HashSet<string> columns,
            DefinitionFindings findings)
        {
            foreach (var column in table.Columns.Where(column => !string.IsNullOrWhiteSpace(column.ComputedExpression)))
            {
                CheckExpression(table,
                    columns,
                    column.ComputedExpression,
                    $"computed column '{column.Field.Name}'",
                    findings);
            }
        }

        /// <summary>
        /// Checks the quoted identifiers of the filters of the indexes.
        /// </summary>
        /// <param name="table">The table that has the indexes.</param>
        /// <param name="columns">The names of the columns of the table.</param>
        /// <param name="findings">The findings that receive the problems.</param>
        private static void CheckIndexFilters(TableSchema table,
            HashSet<string> columns,
            DefinitionFindings findings)
        {
            foreach (var index in table.Indexes.Where(index => !string.IsNullOrWhiteSpace(index.Filter)))
            {
                CheckExpression(table,
                    columns,
                    index.Filter,
                    $"filter of the index '{index.Name}'",
                    findings);
            }
        }

        /// <summary>
        /// Reports the quoted identifiers of an expression that are not columns of the table.
        /// </summary>
        /// <param name="table">The table that has the expression.</param>
        /// <param name="columns">The names of the columns of the table.</param>
        /// <param name="expression">The SQL expression.</param>
        /// <param name="owner">The description of what has the expression (i.e.: check constraint 'CK_Age').</param>
        /// <param name="findings">The findings that receive the problems.</param>
        private static void CheckExpression(TableSchema table,
            HashSet<string> columns,
            string expression,
            string owner,
            DefinitionFindings findings)
        {
            var unknown = SchemaDefinitionExpressionScanner.GetQuotedIdentifiers(expression)
                .Where(identifier => !columns.Contains(identifier));
            foreach (var identifier in unknown)
            {
                findings.AddWarning(table, $"The {owner} refers to '{identifier}', which is not a column of the table. The expression is not parsed, so it may be something else.");
            }
        }

        #endregion
    }
}
