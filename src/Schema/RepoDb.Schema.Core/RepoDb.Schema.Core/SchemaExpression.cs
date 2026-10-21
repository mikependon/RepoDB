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
    public static class SchemaExpression
    {
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
            foreach (var name in (functions ?? Enumerable.Empty<string>()).Where(item => Uses(expression, item, true))
                .Concat((keywords ?? Enumerable.Empty<string>()).Where(item => Uses(expression, item, false))))
            {
                return name;
            }
            return null;
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
