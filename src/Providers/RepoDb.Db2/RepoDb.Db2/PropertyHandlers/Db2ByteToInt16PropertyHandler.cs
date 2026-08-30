#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Interfaces;
using RepoDb.Options;

namespace RepoDb.Db2.PropertyHandlers
{
    /// <summary>
    /// A property handler that handles the converstions of <see cref="short"/> and <see cref="byte"/>.
    /// </summary>
    public class Db2ByteToInt16PropertyHandler : IPropertyHandler<short, byte>
    {
        /// <summary>
        /// Converts the <see cref="short"/> value read back from the <c>SMALLINT</c> column into a <see cref="byte"/>.
        /// </summary>
        public byte Get(short input,
            PropertyHandlerGetOptions options) =>
            (byte)input;

        /// <summary>
        /// Converts the <see cref="byte"/> data entity property value into a <see cref="short"/> before it is
        /// bound to the underlying <c>DB2Parameter</c>.
        /// </summary>
        public short Set(byte input,
            PropertyHandlerSetOptions options) =>
            input;
    }
}
