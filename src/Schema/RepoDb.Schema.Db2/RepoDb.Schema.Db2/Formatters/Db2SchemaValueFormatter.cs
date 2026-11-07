#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema
{
    /// <summary>
    /// A <see cref="SchemaValueFormatter"/> that writes the typed values and the kinds of default values in the format of Db2.
    /// </summary>
    public sealed class Db2SchemaValueFormatter : SchemaValueFormatter
    {
        #region Properties

        /// <summary>
        /// Gets whether the formatter writes the kinds of default values too, which it does.
        /// </summary>
        public override bool HandlesKinds => true;

        #endregion

        #region Public Methods

        /// <summary>
        /// Writes a kind of default value in the format of Db2.
        /// </summary>
        /// <param name="kind">The kind of the default value.</param>
        /// <param name="destinationType">The type of the column.</param>
        /// <param name="warning">The reason why the kind was changed, or <c>null</c>.</param>
        /// <returns>The text that follows <c>DEFAULT</c>, or <c>null</c> if Db2 has no equivalent.</returns>
        public override string FormatKind(ColumnDefaultValue kind,
            string destinationType,
            out string warning)
        {
            warning = null;
            switch (kind)
            {
                case ColumnDefaultValue.CurrentTimestamp:
                    return "CURRENT TIMESTAMP";
                case ColumnDefaultValue.CurrentTimestampUtc:
                    return "CURRENT TIMESTAMP - CURRENT TIMEZONE";
                case ColumnDefaultValue.CurrentTimestampWithOffset:
                    warning = "Db2 has no type with an offset, so the date and time is in the time zone of the server";
                    return "CURRENT TIMESTAMP";
                case ColumnDefaultValue.CurrentDate:
                    return "CURRENT DATE";
                case ColumnDefaultValue.CurrentTime:
                    return "CURRENT TIME";
                case ColumnDefaultValue.CurrentUser:
                    return "CURRENT USER";
                case ColumnDefaultValue.CurrentDatabase:
                    return "CURRENT SERVER";
                default:
                    return null;
            }
        }

        #endregion
    }
}
