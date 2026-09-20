#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Interfaces;
using RepoDb.Options;
using System;

namespace RepoDb.PropertyHandlers
{
    /// <summary>
    /// A property handler that maps a text-based UUID column (for example, the MariaDB <c>UUID</c> type) into a <see cref="Nullable{T}"/> of <see cref="Guid"/> property.
    /// </summary>
    public class StringToNullableGuidPropertyHandler : IPropertyHandler<object, Guid?>
    {
        /// <summary>
        /// Converts the UUID value, as returned by the driver, into a <see cref="Nullable{T}"/> of <see cref="Guid"/>.
        /// </summary>
        /// <param name="input">The value of the column, as returned by the driver (a <see cref="string"/> in the canonical 36-character form, or a <see cref="Guid"/>).</param>
        /// <param name="options">The options of the property handler for reading the value.</param>
        /// <returns>The <see cref="Guid"/>, or <c>null</c> when the value is <c>null</c> or empty.</returns>
        public Guid? Get(object input,
            PropertyHandlerGetOptions options) =>
            GuidConverter.ToGuid(input);

        /// <summary>
        /// Converts the <see cref="Guid"/> into its canonical 36-character text (for example <c>6f9619ff-8b86-d011-b42d-00c04fc964ff</c>), to be written into the column.
        /// </summary>
        /// <param name="input">The <see cref="Guid"/> to write.</param>
        /// <param name="options">The options of the property handler for writing the value.</param>
        /// <returns>The canonical text of the <see cref="Guid"/> (as a <see cref="string"/>), or <c>null</c> when the value is <c>null</c>.</returns>
        public object Set(Guid? input,
            PropertyHandlerSetOptions options) =>
            input?.ToString("D");
    }
}
