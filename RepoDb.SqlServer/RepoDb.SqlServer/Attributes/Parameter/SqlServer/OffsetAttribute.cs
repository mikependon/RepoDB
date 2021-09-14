using Microsoft.Data.SqlClient;

namespace RepoDb.Attributes.Parameter.SqlServer
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="SqlParameter.Offset"/> property via an entity property
    /// before the actual execution.
    /// </summary>
    public class OffsetAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="OffsetAttribute"/> class.
        /// </summary>
        /// <param name="offset">The offset value.</param>
        public OffsetAttribute(int offset)
            : base(typeof(SqlParameter), nameof(SqlParameter.Offset), offset)
        { }

        /// <summary>
        /// Gets the mapped offset value of the parameter.
        /// </summary>
        public int Offset => (int)Value;
    }
}