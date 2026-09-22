#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Interfaces;
using RepoDb.Options;
using System;
using System.IO;

namespace RepoDb.PropertyHandlers.DuckDb
{
    /// <summary>
    /// A property handler that maps a DuckDB <c>BLOB</c> column onto a <see cref="byte"/> array property.
    /// DuckDB.NET returns an unmanaged <see cref="Stream"/> when reading a <c>BLOB</c> value, so a
    /// <see cref="byte"/> array-typed property has no direct match against the returned <see cref="Stream"/>
    /// without this handler.
    /// </summary>
    public class DuckDbStreamToByteArrayPropertyHandler : IPropertyHandler<object, byte[]>
    {
        /// <summary>
        /// Reads the <see cref="Stream"/> value, as returned by the driver, into a <see cref="byte"/> array.
        /// </summary>
        /// <param name="input">The value of the column, as returned by the driver (a <see cref="Stream"/>).</param>
        /// <param name="options">The options of the property handler for reading the value.</param>
        /// <returns>The contents of the <see cref="Stream"/> as a <see cref="byte"/> array, or an empty array when the value is neither a <see cref="Stream"/> nor a <see cref="byte"/> array.</returns>
        public byte[] Get(object input,
            PropertyHandlerGetOptions options)
        {
            if (input is byte[] bytes)
            {
                return bytes;
            }
            if (input is Stream stream)
            {
                using var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
            return Array.Empty<byte>();
        }

        /// <summary>
        /// Passes the <see cref="byte"/> array through unchanged, to be written into a <c>BLOB</c> column.
        /// </summary>
        /// <param name="input">The <see cref="byte"/> array to write.</param>
        /// <param name="options">The options of the property handler for writing the value.</param>
        /// <returns>The <see cref="byte"/> array (boxed).</returns>
        public object Set(byte[] input,
            PropertyHandlerSetOptions options) =>
            input;
    }
}
