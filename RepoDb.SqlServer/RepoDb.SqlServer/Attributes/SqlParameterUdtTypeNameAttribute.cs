using Microsoft.Data.SqlClient;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="SqlParameter.UdtTypeName"/> property via an entity property
    /// before the actual execution.
    /// </summary>
    public class SqlParameterUdtTypeNameAttribute : ParameterPropertyValueSetterAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="SqlParameterTypeNameAttribute"/> class.
        /// </summary>
        /// <param name="udtTypeName">The name of the UTD.</param>
        public SqlParameterUdtTypeNameAttribute(string udtTypeName)
            : base(typeof(SqlParameter), nameof(SqlParameter.UdtTypeName), udtTypeName)
        { }

        /// <summary>
        /// Gets a type name that is currently mapped.
        /// </summary>
        public string UdtTypeName => (string)Value;
    }
}