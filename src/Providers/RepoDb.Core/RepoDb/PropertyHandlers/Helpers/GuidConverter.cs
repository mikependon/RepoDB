#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;

namespace RepoDb.PropertyHandlers
{
    /// <summary>
    /// A helper that converts the value of a text-based UUID column (for example, the MariaDB <c>UUID</c> type), as returned by the driver, into a <see cref="Guid"/>.
    /// </summary>
    internal static class GuidConverter
    {
        /// <summary>
        /// Converts the value returned by the driver (a <see cref="Guid"/> or a <see cref="string"/> in the canonical 36-character form) into a <see cref="Guid"/>.
        /// </summary>
        /// <param name="value">The value returned by the driver.</param>
        /// <returns>The <see cref="Guid"/>, or <c>null</c> when the value is <c>null</c>, <see cref="DBNull"/> or empty.</returns>
        /// <exception cref="ArgumentException">Thrown when the type of the value is not supported.</exception>
        /// <exception cref="FormatException">Thrown when the text is not a valid UUID.</exception>
        internal static Guid? ToGuid(object value)
        {
            switch (value)
            {
                case null:
                case DBNull _:
                    return null;
                case Guid guid:
                    return guid;
                case string text:
                    return text.Length == 0 ? (Guid?)null : Guid.Parse(text);
                default:
                    throw new ArgumentException($"The type '{value.GetType()}' is not a supported UUID value.", nameof(value));
            }
        }
    }
}
