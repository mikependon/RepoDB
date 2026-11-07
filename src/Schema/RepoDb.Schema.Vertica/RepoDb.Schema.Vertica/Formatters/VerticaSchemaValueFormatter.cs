#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema
{
    /// <summary>
    /// A <see cref="SchemaValueFormatter"/> that writes the typed values and the kinds of default values in the format of Vertica.
    /// </summary>
    public sealed class VerticaSchemaValueFormatter : SchemaValueFormatter
    {
        #region Properties

        /// <summary>
        /// Gets whether the formatter writes the kinds of default values too, which it does.
        /// </summary>
        public override bool HandlesKinds => true;

        #endregion

        #region Public Methods

        /// <summary>
        /// Writes a kind of default value in the format of Vertica.
        /// </summary>
        /// <param name="kind">The kind of the default value.</param>
        /// <param name="destinationType">The type of the column.</param>
        /// <param name="warning">The reason why the kind was changed, or <c>null</c>.</param>
        /// <returns>The text that follows <c>DEFAULT</c>, or <c>null</c> if Vertica has no equivalent.</returns>
        public override string FormatKind(ColumnDefaultValue kind,
            string destinationType,
            out string warning)
        {
            warning = null;
            switch (kind)
            {
                case ColumnDefaultValue.CurrentTimestamp:
                case ColumnDefaultValue.CurrentTimestampWithOffset:
                    return "CURRENT_TIMESTAMP";
                case ColumnDefaultValue.CurrentTimestampUtc:
                    return "(CURRENT_TIMESTAMP AT TIME ZONE 'UTC')";
                case ColumnDefaultValue.CurrentDate:
                    return "CURRENT_DATE";
                case ColumnDefaultValue.CurrentTime:
                    return "CURRENT_TIME";
                case ColumnDefaultValue.NewGuid:
                case ColumnDefaultValue.NewSequentialGuid:
                    return "UUID_GENERATE()";
                case ColumnDefaultValue.RandomNumber:
                    return "RANDOM()";
                case ColumnDefaultValue.CurrentUser:
                    return "CURRENT_USER";
                case ColumnDefaultValue.CurrentDatabase:
                    return "CURRENT_DATABASE()";
                default:
                    return null;
            }
        }

        #endregion
    }
}
