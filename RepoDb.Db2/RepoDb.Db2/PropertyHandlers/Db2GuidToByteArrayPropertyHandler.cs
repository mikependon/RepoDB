#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Interfaces;
using RepoDb.Options;
using System;

namespace RepoDb.Db2.PropertyHandlers
{
    /// <summary>
    /// A property handler that handles the converstions of array of <see cref="byte"/> and <see cref="System.Guid"/>.
    /// </summary>
    public class Db2GuidToByteArrayPropertyHandler : IPropertyHandler<byte[], Guid>
    {
        /// <summary>
        /// Converts the array of <see cref="byte"/> value read back from the <c>CHAR(16) FOR BIT DATA</c> column into a <see cref="Guid"/>.
        /// </summary>
        public Guid Get(byte[] input,
            PropertyHandlerGetOptions options) =>
            input == null || input.Length == 0 ? Guid.Empty : new Guid(input);

        /// <summary>
        /// Converts the <see cref="Guid"/> data entity property value into an array of <see cref="byte"/> before it is
        /// bound to the underlying <c>DB2Parameter</c>.
        /// </summary>
        public byte[] Set(Guid input,
            PropertyHandlerSetOptions options) =>
            input.ToByteArray();
    }
}
