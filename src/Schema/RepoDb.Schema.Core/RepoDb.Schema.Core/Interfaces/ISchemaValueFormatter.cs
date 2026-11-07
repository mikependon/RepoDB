#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// An interface that is used to define the formatter of a database, which writes a typed value, or a kind of default value, in the format that the database accepts.
    /// </summary>
    public interface ISchemaValueFormatter
    {
        #region Properties

        /// <summary>
        /// Gets whether the formatter writes the kinds of default values (i.e.: the current date and time) too. If it does not, the composer of the database writes them.
        /// </summary>
        bool HandlesKinds { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Writes a typed value in the format of the database.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="destinationType">The type of the column in the database (i.e.: <c>timestamp(3)</c>), as the composer writes it.</param>
        /// <param name="warning">The reason why the value was changed when it can not be written as it is (i.e.: its offset), or <c>null</c>.</param>
        /// <returns>The text that follows <c>DEFAULT</c>, or <c>null</c> if the database can not hold the value.</returns>
        string Format(SchemaValue value,
            string destinationType,
            out string warning);

        /// <summary>
        /// Writes a kind of default value (i.e.: the current date and time) in the format of the database.
        /// </summary>
        /// <param name="kind">The kind of the default value.</param>
        /// <param name="destinationType">The type of the column in the database, as the composer writes it.</param>
        /// <param name="warning">The reason why the kind was changed when it can not be written as it is, or <c>null</c>.</param>
        /// <returns>The text that follows <c>DEFAULT</c>, or <c>null</c> if the database has no equivalent.</returns>
        string FormatKind(ColumnDefaultValue kind,
            string destinationType,
            out string warning);

        #endregion
    }
}
