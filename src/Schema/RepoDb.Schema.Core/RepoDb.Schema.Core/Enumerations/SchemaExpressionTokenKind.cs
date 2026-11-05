#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Schema.Enumerations
{
    /// <summary>
    /// An enumeration that defines the kind of a token of a SQL expression.
    /// </summary>
    internal enum SchemaExpressionTokenKind
    {
        /// <summary>
        /// Any other text, like the operators, the numbers, the parentheses and the white spaces.
        /// </summary>
        Other = 0,

        /// <summary>
        /// A name that is not quoted, like a keyword, a function or a column.
        /// </summary>
        Word,

        /// <summary>
        /// A text between single quotes.
        /// </summary>
        Literal,

        /// <summary>
        /// A name that is quoted with the identifier quotes of the source database.
        /// </summary>
        Identifier
    }
}
