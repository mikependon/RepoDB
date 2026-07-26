using Oracle.ManagedDataAccess.Client;

namespace RepoDb.Attributes.Parameter.Oracle
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="OracleParameter.Offset"/> property via
    /// an entity property before the actual execution. Specifies the offset into the <c>Value</c>
    /// property (or into each element of the <c>Value</c> property, for array binds).
    /// </summary>
    public class OffsetAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="OffsetAttribute"/> class.
        /// </summary>
        /// <param name="offset">The offset value.</param>
        public OffsetAttribute(int offset)
            : base(typeof(OracleParameter), nameof(OracleParameter.Offset), offset)
        { }

        /// <summary>
        /// Gets the mapped offset value of the parameter.
        /// </summary>
        public int Offset => (int)Value;
    }
}
