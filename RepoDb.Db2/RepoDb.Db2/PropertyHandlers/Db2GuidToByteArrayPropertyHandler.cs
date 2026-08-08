using RepoDb.Interfaces;
using RepoDb.Options;
using System;

namespace RepoDb.Db2.PropertyHandlers
{
    /// <summary>
    /// A <see cref="IPropertyHandler{TInput, TResult}"/> that converts a <see cref="Guid"/> data entity
    /// property to/from a <see cref="byte[]"/> for binding against a Db2 <c>CHAR(16) FOR BIT DATA</c> column.
    /// </summary>
    /// <remarks>
    /// Db2 has no native GUID/UNIQUEIDENTIFIER type, so the idiomatic storage for a GUID is a
    /// fixed-length 16-byte <c>CHAR(16) FOR BIT DATA</c> column.
    /// <para>
    /// Use this handler if needed as it is intentionally NOT registered automatically for every <see cref="Guid"/> property,
    /// since <see cref="PropertyHandlerMapper"/> registrations keyed by CLR type are global across the
    /// whole process — auto-registering it would also affect
    /// unrelated connections. Register it explicitly, scoped to the specific entity property that maps
    /// to a <c>CHAR(16) FOR BIT DATA</c> column:
    /// <code>
    /// PropertyHandlerMapper.Add&lt;CompleteTable, Db2GuidToByteArrayPropertyHandler&gt;(
    ///     e => e.SessionId, new Db2GuidToByteArrayPropertyHandler(), true);
    /// </code>
    /// </para>
    /// </remarks>
    public class Db2GuidToByteArrayPropertyHandler : IPropertyHandler<byte[], Guid>
    {
        /// <summary>
        /// Converts the <see cref="byte[]"/> value read back from the <c>CHAR(16) FOR BIT DATA</c> column into a <see cref="Guid"/>.
        /// </summary>
        public Guid Get(byte[] input,
            PropertyHandlerGetOptions options) =>
            input == null || input.Length == 0 ? Guid.Empty : new Guid(input);

        /// <summary>
        /// Converts the <see cref="Guid"/> data entity property value into a <see cref="byte[]"/> before it is
        /// bound to the underlying <c>DB2Parameter</c>.
        /// </summary>
        public byte[] Set(Guid input,
            PropertyHandlerSetOptions options) =>
            input.ToByteArray();
    }
}
