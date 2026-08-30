#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Interfaces;
using RepoDb.Options;
using System;

namespace RepoDb.Oracle.PropertyHandlers
{
    /// <summary>
    /// A <see cref="IPropertyHandler{TInput, TResult}"/> that converts a <see cref="Guid"/> data entity
    /// property to/from a <see cref="byte[]"/> for binding against an Oracle <c>RAW(16)</c> column.
    /// </summary>
    /// <remarks>
    /// Oracle has no native GUID/UNIQUEIDENTIFIER type, and ODP.NET's <c>OracleParameter.Value</c> setter
    /// does not accept a raw <see cref="Guid"/> value (unlike other DB providers),
    /// throwing <c>ArgumentException: Value does not fall within the expected range.</c> if one is assigned
    /// directly. The idiomatic Oracle storage for a GUID is a 16-byte <c>RAW(16)</c> column.
    /// <para>
    /// Use this handler if needed as it is intentionally NOT registered automatically for every <see cref="Guid"/> property,
    /// since <see cref="PropertyHandlerMapper"/> registrations keyed by CLR type are global across the
    /// whole process — auto-registering it would also affect
    /// unrelated connections. Register it explicitly, scoped to the specific entity property that maps
    /// to a <c>RAW(16)</c> column:
    /// <code>
    /// PropertyHandlerMapper.Add&lt;CompleteTable, GuidToByteArrayPropertyHandler&gt;(
    ///     e => e.SessionId, new GuidToByteArrayPropertyHandler(), true);
    /// </code>
    /// </para>
    /// </remarks>
    public class OracleGuidToByteArrayPropertyHandler : IPropertyHandler<byte[], Guid>
    {
        /// <summary>
        /// Converts the <see cref="byte[]"/> value read back from the <c>RAW(16)</c> column into a <see cref="Guid"/>.
        /// </summary>
        public Guid Get(byte[] input,
            PropertyHandlerGetOptions options) =>
            input == null || input.Length == 0 ? Guid.Empty : new Guid(input);

        /// <summary>
        /// Converts the <see cref="Guid"/> data entity property value into a <see cref="byte[]"/> before it is
        /// bound to the underlying <c>OracleParameter</c>.
        /// </summary>
        public byte[] Set(Guid input,
            PropertyHandlerSetOptions options) =>
            input.ToByteArray();
    }
}
