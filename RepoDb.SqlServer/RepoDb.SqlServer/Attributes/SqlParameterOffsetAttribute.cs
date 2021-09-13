using Microsoft.Data.SqlClient;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="SqlParameter.Offset"/> property via an entity property
    /// before the actual execution.
    /// </summary>
    public class SqlParameterOffsetAttribute : ParameterPropertyValueSetterAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="SqlParameterOffsetAttribute"/> class.
        /// </summary>
        /// <param name="offset">The offset value.</param>
        public SqlParameterOffsetAttribute(int offset)
            : base(typeof(SqlParameter), nameof(SqlParameter.Offset), offset)
        { }

        /// <summary>
        /// Gets the mapped offset value.
        /// </summary>
        public int Offset => (int)Value;
    }
}