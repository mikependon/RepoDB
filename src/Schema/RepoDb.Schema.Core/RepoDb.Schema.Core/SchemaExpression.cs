#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RepoDb.Schema
{
    /// <summary>
    /// A class that inspects the SQL expressions of the schema objects (i.e.: the expression of a check constraint), used by the composers to find what
    /// the destination database does not accept.
    /// </summary>
    internal static class SchemaExpression
    {
        #region Private Variables

        /// <summary>
        /// The functions that are not deterministic (the date and the time, the random values, the session and the server), in the languages of the databases.
        /// The databases that do not allow them in a <c>CHECK</c> constraint, as it must give the same answer for the same row every time, are listed in <see cref="ICheckConstraintSupport"/>.
        /// </summary>
        private static readonly string[] NonDeterministicFunctions =
        {
            "now",
            "curdate",
            "curtime",
            "utc_date",
            "utc_time",
            "utc_timestamp",
            "unix_timestamp",
            "rand",
            "random",
            "randomblob",
            "uuid",
            "uuid_short",
            "gen_random_uuid",
            "uuid_generate_v4",
            "connection_id",
            "user",
            "session_user",
            "system_user",
            "database",
            "schema",
            "version",
            "last_insert_id",
            "last_insert_rowid",
            "row_count",
            "found_rows",
            "changes",
            "total_changes",
            "getdate",
            "getutcdate",
            "sysdatetime",
            "sysutcdatetime",
            "sysdatetimeoffset",
            "newid",
            "newsequentialid",
            "sys_guid",
            "sys_context",
            "userenv",
            "uid",
            "clock_timestamp",
            "statement_timestamp",
            "transaction_timestamp",
            "timeofday"
        };

        /// <summary>
        /// The keywords and the special registers that are not deterministic, which are used without parentheses.
        /// </summary>
        private static readonly string[] NonDeterministicKeywords =
        {
            "current_timestamp",
            "current_date",
            "current_time",
            "current_user",
            "localtime",
            "localtimestamp",
            "sysdate",
            "systimestamp",
            "dbtimezone",
            "sessiontimezone",
            "current timestamp",
            "current date",
            "current time",
            "current user"
        };

        #endregion

        #region Public Methods

        /// <summary>
        /// Finds the first function, or keyword, of the list that the expression uses. The names are matched as whole words and without case;
        /// a name that is quoted (<c>[now]</c>, <c>`now`</c>, <c>"now"</c>) or is the part of a qualified name (<c>t.now</c>) is a column, so it is not a match.
        /// </summary>
        /// <param name="expression">The SQL expression.</param>
        /// <param name="functions">The names of the functions, which are only matched when they are called (followed by a parenthesis), i.e.: <c>now</c> in <c>now()</c>.</param>
        /// <param name="keywords">The names of the keywords that are used without parentheses, i.e.: <c>CURRENT_TIMESTAMP</c>.</param>
        /// <returns>The name that the expression uses (as it is in the list), or <c>null</c> if it uses none of them.</returns>
        public static string FindUsage(string expression,
            IEnumerable<string> functions,
            IEnumerable<string> keywords)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                return null;
            }

            foreach (var name in (functions ?? Enumerable.Empty<string>()).Where(item => Uses(expression,
                item,
                true)).Concat((keywords ?? Enumerable.Empty<string>()).Where(item => Uses(expression,
                    item,
                    false))))
            {
                return name;
            }

            return null;
        }

        /// <summary>
        /// Finds the function, or the keyword, that is not deterministic (the date and the time, the random values, the session and the server) that the expression uses.
        /// </summary>
        /// <param name="expression">The SQL expression.</param>
        /// <returns>The function (i.e.: <c>now()</c>) or the keyword (i.e.: <c>current_timestamp</c>) that the expression uses, or <c>null</c> if it uses none of them.</returns>
        public static string FindNonDeterministic(string expression)
        {
            var function = FindUsage(expression,
                NonDeterministicFunctions,
                null);
            return function != null ? function + "()" : FindUsage(expression,
                null,
                NonDeterministicKeywords);
        }

        /// <summary>
        /// Gets the reason why a database does not allow the check constraint when its expression is not deterministic, so it can be left out when the table is created
        /// (see <see cref="ICheckConstraintSupport"/>).
        /// </summary>
        /// <param name="checkConstraint">The check constraint of the source table.</param>
        /// <param name="database">The name of the destination database, as it is said in the reason (i.e.: <c>MySQL</c>).</param>
        /// <returns>The reason (i.e.: <c>its expression uses now(), which MySQL does not allow in a CHECK constraint</c>), or <c>null</c> if the expression is deterministic.</returns>
        public static string GetNonDeterministicReason(Models.CheckConstraintInfo checkConstraint,
            string database)
        {
            var usage = FindNonDeterministic(checkConstraint?.Expression);
            return usage == null ? null : $"its expression uses {usage}, which {database} does not allow in a CHECK constraint";
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Checks whether the expression uses the name.
        /// </summary>
        /// <param name="expression">The SQL expression.</param>
        /// <param name="name">The name of a function or a keyword.</param>
        /// <param name="isFunction"><c>true</c> if the name must be called (followed by a parenthesis).</param>
        /// <returns><c>true</c> if the expression uses the name.</returns>
        private static bool Uses(string expression,
            string name,
            bool isFunction) =>
            Regex.IsMatch(expression,
                $@"(?<![\w\[\]`""'.]){Regex.Escape(name)}(?![\w\]`""'])" + (isFunction ? @"(?=\s*\()" : string.Empty),
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant,
                TimeSpan.FromSeconds(1));

        #endregion
    }
}
