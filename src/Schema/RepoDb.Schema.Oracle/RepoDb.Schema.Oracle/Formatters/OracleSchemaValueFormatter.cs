#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema
{
    /// <summary>
    /// A <see cref="SchemaValueFormatter"/> that writes the typed values and the kinds of default values in the format of Oracle.
    /// </summary>
    public sealed class OracleSchemaValueFormatter : SchemaValueFormatter
    {
        #region Properties

        /// <summary>
        /// Gets whether the formatter writes the kinds of default values too, which it does.
        /// </summary>
        public override bool HandlesKinds => true;

        #endregion

        #region Public Methods

        /// <summary>
        /// Writes a kind of default value in the format of Oracle.
        /// </summary>
        /// <param name="kind">The kind of the default value.</param>
        /// <param name="destinationType">The type of the column.</param>
        /// <param name="warning">The reason why the kind was changed, or <c>null</c>.</param>
        /// <returns>The text that follows <c>DEFAULT</c>, or <c>null</c> if Oracle has no equivalent.</returns>
        public override string FormatKind(ColumnDefaultValue kind,
            string destinationType,
            out string warning)
        {
            warning = null;
            switch (kind)
            {
                case ColumnDefaultValue.CurrentTimestamp:
                    return "SYSDATE";
                case ColumnDefaultValue.CurrentTimestampUtc:
                    return "SYS_EXTRACT_UTC(SYSTIMESTAMP)";
                case ColumnDefaultValue.CurrentTimestampWithOffset:
                    return "SYSTIMESTAMP";
                case ColumnDefaultValue.CurrentDate:
                    return "TRUNC(SYSDATE)";
                case ColumnDefaultValue.NewGuid:
                case ColumnDefaultValue.NewSequentialGuid:
                    return "SYS_GUID()";
                case ColumnDefaultValue.CurrentUser:
                    return "SYS_CONTEXT('USERENV', 'SESSION_USER')";
                case ColumnDefaultValue.CurrentDatabase:
                    return "SYS_CONTEXT('USERENV', 'DB_NAME')";
                default:
                    return null;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Writes a boolean as <c>1</c> or <c>0</c>, the values of a <c>NUMBER(1)</c>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="destinationType">The type of the column.</param>
        /// <returns>The text.</returns>
        protected override string FormatBoolean(bool value,
            string destinationType) => value ? "1" : "0";

        /// <summary>
        /// Writes no time of the day, as Oracle has no type for it.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="destinationType">The type of the column.</param>
        /// <returns>The text.</returns>
        protected override string FormatTime(TimeSpan value,
            string destinationType) => null;

        #endregion
    }
}
