using RepoDb.Interfaces;
using RepoDb.Options;

namespace RepoDb.Db2.PropertyHandlers
{
    /// <summary>
    /// A <see cref="IPropertyHandler{TInput, TResult}"/> that converts a <see cref="byte"/> data entity
    /// property to/from a <see cref="short"/> for binding against a Db2 <c>SMALLINT</c> column.
    /// </summary>
    /// <remarks>
    /// Db2 for Linux/UNIX/Windows has no native 8-bit TINYINT type, so the idiomatic storage for one is
    /// a <c>SMALLINT</c> column - and the underlying IBM Data Server .NET Provider does not marshal a
    /// raw, boxed <see cref="byte"/> parameter value cleanly against it: even with the parameter's
    /// <c>DB2Type</c>/<c>DbType</c> both correctly resolved to <c>SmallInt</c>/<c>Int16</c> (see
    /// <c>Db2TypeAttribute</c> and Db2's schema-driven <c>DbField.Type</c> resolution), passing a
    /// <see cref="byte"/>-typed <c>.Value</c> still fails at execution time with
    /// <c>System.InvalidCastException: Unable to cast object of type 'System.Byte' to type
    /// 'System.Byte[]'</c> - the driver appears to special-case a bare CLR <see cref="byte"/> value as
    /// 1-byte binary data rather than a numeric scalar, regardless of the declared parameter type.
    /// Converting to <see cref="short"/> before the value ever reaches the <c>DB2Parameter</c> sidesteps
    /// that ambiguity entirely, since <see cref="short"/> has no such special-cased dual meaning.
    /// <para>
    /// Use this handler if needed as it is intentionally NOT registered automatically for every
    /// <see cref="byte"/> property, since <see cref="PropertyHandlerMapper"/> registrations keyed by
    /// CLR type are global across the whole process - auto-registering it would also affect unrelated
    /// connections. Register it explicitly, scoped to the specific entity property that maps to a
    /// Db2 <c>SMALLINT</c>-backed "tiny int" column:
    /// <code>
    /// PropertyHandlerMapper.Add&lt;CompleteTable, Db2ByteToInt16PropertyHandler&gt;(
    ///     e => e.ColumnTinyInt, new Db2ByteToInt16PropertyHandler(), true);
    /// </code>
    /// </para>
    /// </remarks>
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
