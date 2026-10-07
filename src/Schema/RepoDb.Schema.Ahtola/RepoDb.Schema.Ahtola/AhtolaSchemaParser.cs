#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RepoDb.Schema
{
    /// <summary>
    /// A class that reads the objects that SQLite does not expose through its <c>pragma</c> functions (the names of the constraints, the check constraints,
    /// the expressions of the generated columns, the collations and the filters of the indexes) from the SQL text that created them.
    /// </summary>
    internal static class AhtolaSchemaParser
    {
        #region Types

        /// <summary>
        /// The kind of a lexical token.
        /// </summary>
        private enum TokenKind
        {
            Word,
            Quoted,
            Text,
            Open,
            Close,
            Comma,
            Other
        }

        /// <summary>
        /// A lexical token of an SQL text.
        /// </summary>
        private sealed class Token
        {
            public TokenKind Kind { get; set; }

            public string Value { get; set; }

            public int Start { get; set; }

            public int End { get; set; }

            public bool Is(string word) =>
                Kind == TokenKind.Word && string.Equals(Value, word, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Reads the definition of the columns and the constraints from the <c>CREATE TABLE</c> statement.
        /// </summary>
        /// <param name="sql">The statement that created the table.</param>
        /// <returns>The parsed definition. It is empty if the statement is empty.</returns>
        public static AhtolaTableDefinition ParseTable(string sql)
        {
            var definition = new AhtolaTableDefinition();
            if (string.IsNullOrWhiteSpace(sql))
            {
                return definition;
            }

            var tokens = Tokenize(sql);
            var open = tokens.FindIndex(t => t.Kind == TokenKind.Open);
            if (open < 0)
            {
                return definition;
            }
            var close = FindClose(tokens, open);
            foreach (var (start, end) in Split(tokens, open + 1, close))
            {
                if (start >= end)
                {
                    continue;
                }
                if (IsTableConstraint(tokens[start]))
                {
                    ParseTableConstraint(sql, tokens, start, end, definition);
                }
                else
                {
                    ParseColumn(sql, tokens, start, end, definition);
                }
            }
            return definition;
        }

        /// <summary>
        /// Reads the filter (the <c>WHERE</c> clause) of a partial index from the <c>CREATE INDEX</c> statement.
        /// </summary>
        /// <param name="sql">The statement that created the index.</param>
        /// <returns>The expression of the filter, or <c>null</c> if the index has no filter.</returns>
        public static string ParseIndexFilter(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
            {
                return null;
            }

            var tokens = Tokenize(sql);
            var open = tokens.FindIndex(t => t.Kind == TokenKind.Open);
            if (open < 0)
            {
                return null;
            }
            var close = FindClose(tokens, open);
            if (close + 1 >= tokens.Count || !tokens[close + 1].Is("WHERE"))
            {
                return null;
            }
            var filter = sql.Substring(tokens[close + 1].End).Trim().TrimEnd(';').Trim();
            return filter.Length == 0 ? null : filter;
        }

        /// <summary>
        /// Splits the definition of a type (i.e.: <c>decimal(18, 2)</c>) into its name and its arguments.
        /// </summary>
        /// <param name="declaredType">The type that is declared on the column.</param>
        /// <returns>The name of the type in lower case (without the arguments), and the numeric arguments.</returns>
        public static (string Name, IList<int> Arguments) ParseType(string declaredType)
        {
            if (string.IsNullOrWhiteSpace(declaredType))
            {
                return (string.Empty, new List<int>());
            }

            var open = declaredType.IndexOf('(');
            var name = (open < 0 ? declaredType : declaredType.Substring(0, open)).Trim().ToLowerInvariant();
            var arguments = new List<int>();
            if (open >= 0)
            {
                var close = declaredType.IndexOf(')', open);
                var inner = close < 0 ? declaredType.Substring(open + 1) : declaredType.Substring(open + 1, close - open - 1);
                foreach (var part in inner.Split(','))
                {
                    if (int.TryParse(part.Trim(), out var value))
                    {
                        arguments.Add(value);
                    }
                }
            }
            return (name, arguments);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Splits the text into its tokens (the comments are ignored).
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private static List<Token> Tokenize(string sql)
        {
            var tokens = new List<Token>();
            var i = 0;
            while (i < sql.Length)
            {
                var c = sql[i];
                if (char.IsWhiteSpace(c))
                {
                    i++;
                }
                else if (c == '-' && i + 1 < sql.Length && sql[i + 1] == '-')
                {
                    while (i < sql.Length && sql[i] != '\n')
                    {
                        i++;
                    }
                }
                else if (c == '/' && i + 1 < sql.Length && sql[i + 1] == '*')
                {
                    var end = sql.IndexOf("*/", i + 2, StringComparison.Ordinal);
                    i = end < 0 ? sql.Length : end + 2;
                }
                else if (c == '"' || c == '`' || c == '\'' || c == '[')
                {
                    var close = c == '[' ? ']' : c;
                    var start = i;
                    var value = new StringBuilder();
                    i++;
                    while (i < sql.Length)
                    {
                        if (sql[i] == close)
                        {
                            if (close != ']' && i + 1 < sql.Length && sql[i + 1] == close)
                            {
                                value.Append(close);
                                i += 2;
                                continue;
                            }
                            break;
                        }
                        value.Append(sql[i]);
                        i++;
                    }
                    i++;
                    tokens.Add(new Token { Kind = c == '\'' ? TokenKind.Text : TokenKind.Quoted, Value = value.ToString(), Start = start, End = i });
                }
                else if (c == '(' || c == ')' || c == ',')
                {
                    tokens.Add(new Token { Kind = c == '(' ? TokenKind.Open : c == ')' ? TokenKind.Close : TokenKind.Comma, Value = c.ToString(), Start = i, End = i + 1 });
                    i++;
                }
                else if (char.IsLetterOrDigit(c) || c == '_')
                {
                    var start = i;
                    while (i < sql.Length && (char.IsLetterOrDigit(sql[i]) || sql[i] == '_'))
                    {
                        i++;
                    }
                    tokens.Add(new Token { Kind = TokenKind.Word, Value = sql.Substring(start, i - start), Start = start, End = i });
                }
                else
                {
                    tokens.Add(new Token { Kind = TokenKind.Other, Value = c.ToString(), Start = i, End = i + 1 });
                    i++;
                }
            }
            return tokens;
        }

        /// <summary>
        /// Finds the token that closes the group that is opened by the token.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="open"></param>
        /// <returns></returns>
        private static int FindClose(IList<Token> tokens,
            int open)
        {
            var depth = 0;
            for (var i = open; i < tokens.Count; i++)
            {
                if (tokens[i].Kind == TokenKind.Open)
                {
                    depth++;
                }
                else if (tokens[i].Kind == TokenKind.Close && --depth == 0)
                {
                    return i;
                }
            }
            return tokens.Count - 1;
        }

        /// <summary>
        /// Splits the tokens that are between the positions by the commas that are not inside a group.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        private static IEnumerable<(int Start, int End)> Split(IList<Token> tokens,
            int from,
            int to)
        {
            var depth = 0;
            var start = from;
            for (var i = from; i < to; i++)
            {
                if (tokens[i].Kind == TokenKind.Open)
                {
                    depth++;
                }
                else if (tokens[i].Kind == TokenKind.Close)
                {
                    depth--;
                }
                else if (tokens[i].Kind == TokenKind.Comma && depth == 0)
                {
                    yield return (start, i);
                    start = i + 1;
                }
            }
            yield return (start, to);
        }

        /// <summary>
        /// Checks whether the item that starts with the token is a constraint of the table (and not a column).
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private static bool IsTableConstraint(Token token) =>
            token.Is("CONSTRAINT") || token.Is("PRIMARY") || token.Is("UNIQUE") || token.Is("CHECK") || token.Is("FOREIGN");

        /// <summary>
        /// Gets the names of the columns that are listed between the parentheses that start at the position.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="open"></param>
        /// <returns></returns>
        private static IList<string> GetColumns(IList<Token> tokens,
            int open)
        {
            var columns = new List<string>();
            var close = FindClose(tokens, open);
            var expectName = true;
            for (var i = open + 1; i < close; i++)
            {
                if (tokens[i].Kind == TokenKind.Comma)
                {
                    expectName = true;
                }
                else if (expectName && (tokens[i].Kind == TokenKind.Word || tokens[i].Kind == TokenKind.Quoted))
                {
                    columns.Add(tokens[i].Value);
                    expectName = false;
                }
            }
            return columns;
        }

        /// <summary>
        /// Reads a constraint of the table (<c>[CONSTRAINT name] PRIMARY KEY | UNIQUE | CHECK | FOREIGN KEY</c>).
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="tokens"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="definition"></param>
        private static void ParseTableConstraint(string sql,
            IList<Token> tokens,
            int start,
            int end,
            AhtolaTableDefinition definition)
        {
            var i = start;
            string name = null;
            if (tokens[i].Is("CONSTRAINT") && i + 1 < end)
            {
                name = tokens[i + 1].Value;
                i += 2;
            }
            if (i >= end)
            {
                return;
            }

            var open = -1;
            for (var j = i; j < end; j++)
            {
                if (tokens[j].Kind == TokenKind.Open)
                {
                    open = j;
                    break;
                }
            }
            if (open < 0)
            {
                return;
            }

            if (tokens[i].Is("CHECK"))
            {
                definition.Constraints.Add(new AhtolaConstraintDefinition { Kind = AhtolaConstraintKind.Check, Name = name, Expression = GetExpression(sql, tokens, open) });
            }
            else if (tokens[i].Is("PRIMARY"))
            {
                definition.Constraints.Add(new AhtolaConstraintDefinition { Kind = AhtolaConstraintKind.PrimaryKey, Name = name, Columns = GetColumns(tokens, open) });
            }
            else if (tokens[i].Is("UNIQUE"))
            {
                definition.Constraints.Add(new AhtolaConstraintDefinition { Kind = AhtolaConstraintKind.Unique, Name = name, Columns = GetColumns(tokens, open) });
            }
            else if (tokens[i].Is("FOREIGN"))
            {
                definition.Constraints.Add(new AhtolaConstraintDefinition { Kind = AhtolaConstraintKind.ForeignKey, Name = name, Columns = GetColumns(tokens, open) });
            }
        }

        /// <summary>
        /// Reads a column: its constraints, its generated expression, its collation and whether it is auto incremented.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="tokens"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="definition"></param>
        private static void ParseColumn(string sql,
            IList<Token> tokens,
            int start,
            int end,
            AhtolaTableDefinition definition)
        {
            var column = new AhtolaColumnDefinition { Name = tokens[start].Value };
            string pendingName = null;
            var i = start + 1;
            while (i < end)
            {
                var token = tokens[i];
                if (token.Kind == TokenKind.Open)
                {
                    i = FindClose(tokens, i) + 1;
                    continue;
                }
                if (token.Kind != TokenKind.Word)
                {
                    i++;
                    continue;
                }

                if (token.Is("CONSTRAINT") && i + 1 < end)
                {
                    pendingName = tokens[i + 1].Value;
                    i += 2;
                    continue;
                }
                if (token.Is("PRIMARY"))
                {
                    definition.Constraints.Add(new AhtolaConstraintDefinition { Kind = AhtolaConstraintKind.PrimaryKey, Name = pendingName, Columns = new List<string> { column.Name } });
                    pendingName = null;
                }
                else if (token.Is("UNIQUE"))
                {
                    definition.Constraints.Add(new AhtolaConstraintDefinition { Kind = AhtolaConstraintKind.Unique, Name = pendingName, Columns = new List<string> { column.Name } });
                    pendingName = null;
                }
                else if (token.Is("REFERENCES"))
                {
                    definition.Constraints.Add(new AhtolaConstraintDefinition { Kind = AhtolaConstraintKind.ForeignKey, Name = pendingName, Columns = new List<string> { column.Name } });
                    pendingName = null;
                }
                else if (token.Is("AUTOINCREMENT"))
                {
                    column.IsAutoIncrement = true;
                }
                else if (token.Is("COLLATE") && i + 1 < end)
                {
                    column.Collation = tokens[i + 1].Value;
                    i++;
                }
                else if ((token.Is("CHECK") || token.Is("AS")) && i + 1 < end && tokens[i + 1].Kind == TokenKind.Open)
                {
                    var expression = GetExpression(sql, tokens, i + 1);
                    var close = FindClose(tokens, i + 1);
                    if (token.Is("CHECK"))
                    {
                        definition.Constraints.Add(new AhtolaConstraintDefinition { Kind = AhtolaConstraintKind.Check, Name = pendingName, Expression = expression });
                        pendingName = null;
                    }
                    else
                    {
                        column.GeneratedExpression = expression;
                        column.IsStored = close + 1 < end && tokens[close + 1].Is("STORED");
                    }
                    i = close;
                }
                i++;
            }
            definition.Columns[column.Name] = column;
        }

        /// <summary>
        /// Gets the text that is between the parentheses that start at the position.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="tokens"></param>
        /// <param name="open"></param>
        /// <returns></returns>
        private static string GetExpression(string sql,
            IList<Token> tokens,
            int open)
        {
            var close = FindClose(tokens, open);
            return sql.Substring(tokens[open].End, tokens[close].Start - tokens[open].End).Trim();
        }

        #endregion
    }

    /// <summary>
    /// The kind of a constraint that is read from the SQL text of a table.
    /// </summary>
    internal enum AhtolaConstraintKind
    {
        PrimaryKey,
        Unique,
        Check,
        ForeignKey
    }

    /// <summary>
    /// A constraint that is read from the SQL text of a table.
    /// </summary>
    internal sealed class AhtolaConstraintDefinition
    {
        public AhtolaConstraintKind Kind { get; set; }

        public string Name { get; set; }

        public IList<string> Columns { get; set; } = new List<string>();

        public string Expression { get; set; }

        /// <summary>
        /// Checks whether the constraint is defined on exactly the columns (case insensitive, in the same order).
        /// </summary>
        /// <param name="columns">The names of the columns.</param>
        /// <returns><c>true</c> if the constraint is on the columns; otherwise, <c>false</c>.</returns>
        public bool IsOn(IEnumerable<string> columns) =>
            Columns.SequenceEqual(columns, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// A column that is read from the SQL text of a table.
    /// </summary>
    internal sealed class AhtolaColumnDefinition
    {
        public string Name { get; set; }

        public string GeneratedExpression { get; set; }

        public bool IsStored { get; set; }

        public bool IsAutoIncrement { get; set; }

        public string Collation { get; set; }
    }

    /// <summary>
    /// The columns and the constraints that are read from the SQL text of a table.
    /// </summary>
    internal sealed class AhtolaTableDefinition
    {
        public IDictionary<string, AhtolaColumnDefinition> Columns { get; } = new Dictionary<string, AhtolaColumnDefinition>(StringComparer.OrdinalIgnoreCase);

        public IList<AhtolaConstraintDefinition> Constraints { get; } = new List<AhtolaConstraintDefinition>();

        /// <summary>
        /// Finds the constraint of the kind that is defined on the columns.
        /// </summary>
        /// <param name="kind">The kind of the constraint.</param>
        /// <param name="columns">The names of the columns.</param>
        /// <returns>The constraint, or <c>null</c> if it is not found.</returns>
        public AhtolaConstraintDefinition Find(AhtolaConstraintKind kind,
            IEnumerable<string> columns) =>
            Constraints.FirstOrDefault(c => c.Kind == kind && c.IsOn(columns));
    }
}
