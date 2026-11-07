#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.Core.UnitTests.CustomObjects
{
    /// <summary>
    /// A formatter of a made-up database that the tests of the core use. It writes the ANSI kinds: the current date and time.
    /// </summary>
    public class CustomSchemaValueFormatter : SchemaValueFormatter
    {
        #region Public Methods

        /// <summary>
        /// Writes a kind of default value in the ANSI format.
        /// </summary>
        /// <param name="kind">The kind of the default value.</param>
        /// <param name="destinationType">The type of the column.</param>
        /// <param name="warning">The reason why the kind was changed, or <c>null</c>.</param>
        /// <returns>The text that follows <c>DEFAULT</c>, or <c>null</c> if there is no equivalent.</returns>
        public override string FormatKind(ColumnDefaultValue kind,
            string destinationType,
            out string warning)
        {
            warning = null;
            switch (kind)
            {
                case ColumnDefaultValue.CurrentTimestamp:
                    return "CURRENT_TIMESTAMP";
                case ColumnDefaultValue.CurrentDate:
                    return "CURRENT_DATE";
                case ColumnDefaultValue.CurrentTime:
                    return "CURRENT_TIME";
                default:
                    return null;
            }
        }

        #endregion
    }
}
