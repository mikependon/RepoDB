#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using System.Linq;
using RepoDb.Interfaces;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// Adapts the check constraints of a table that comes from another database: the quoted names get the quotes of the destination database,
    /// and a check constraint that uses what is not portable is left out and reported as a warning, so the table is still created.
    /// </summary>
    internal static class SchemaExpressionAdapter
    {
        #region Private Methods

        /// <summary>
        /// Adapts a check constraint.
        /// </summary>
        /// <param name="table">The state of the table, which receives the warning.</param>
        /// <param name="checkConstraint">The check constraint of the source table.</param>
        /// <param name="quotes">The identifier quotes of the source database.</param>
        /// <param name="destination">The setting of the destination database.</param>
        /// <returns>The check constraint for the destination database, or <c>null</c> if it is left out.</returns>
        private static CheckConstraintInfo AdaptCheckConstraint(PlanTable table,
            CheckConstraintInfo checkConstraint,
            IReadOnlyList<(char Open, char Close)> quotes,
            IDbSetting destination)
        {
            if (string.IsNullOrWhiteSpace(checkConstraint.Expression))
            {
                return checkConstraint;
            }

            var tokens = SchemaExpressionTokenizer.Tokenize(checkConstraint.Expression,
                quotes);
            var usage = SchemaExpressionTranslator.FindNonPortable(tokens);
            if (usage == null)
            {
                return new CheckConstraintInfo(checkConstraint.Name)
                {
                    Expression = SchemaExpressionTranslator.Translate(tokens,
                        destination)
                };
            }

            table.Warnings.Add($"Skipped the check constraint '{checkConstraint.Name}': its expression uses {usage}, which is not portable from {table.Schema.Table.Provider} to {destination.Provider}.");
            return null;
        }

        /// <summary>
        /// Creates the schema of the table with the check constraints.
        /// </summary>
        /// <param name="schema">The schema of the table.</param>
        /// <param name="checkConstraints">The check constraints.</param>
        /// <returns>The new schema of the table.</returns>
        private static TableSchema WithCheckConstraints(TableSchema schema,
            IList<CheckConstraintInfo> checkConstraints) =>
            new TableSchema(schema.Table)
            {
                CheckConstraints = checkConstraints,
                Columns = schema.Columns,
                ForeignKeys = schema.ForeignKeys,
                Indexes = schema.Indexes,
                PrimaryKey = schema.PrimaryKey,
                UniqueConstraints = schema.UniqueConstraints
            };

        #endregion

        #region Public Methods

        /// <summary>
        /// Adapts the check constraints of the table to the destination database, if the table comes from a database with another dialect.
        /// The check constraints are left as they are if the provider of the table or of the destination is not known, or if both have the same dialect.
        /// The schema that was read is not changed: the table gets a new schema.
        /// </summary>
        /// <param name="table">The state of the table.</param>
        /// <param name="destination">The setting of the destination database, which gives the provider and the quotes.</param>
        public static void Adapt(PlanTable table,
            IDbSetting destination)
        {
            var source = SchemaExpressionDialect.GetFamily(table.Schema.Table.Provider);
            var target = SchemaExpressionDialect.GetFamily(destination?.Provider);
            if (source == null || target == null || source == target || table.Schema.CheckConstraints.Count == 0)
            {
                return;
            }

            var quotes = SchemaExpressionDialect.GetQuotes(source);
            table.Schema = WithCheckConstraints(table.Schema,
                table.Schema.CheckConstraints.Select(checkConstraint => AdaptCheckConstraint(table,
                    checkConstraint,
                    quotes,
                    destination)).Where(checkConstraint => checkConstraint != null).ToList());
        }

        #endregion
    }
}
