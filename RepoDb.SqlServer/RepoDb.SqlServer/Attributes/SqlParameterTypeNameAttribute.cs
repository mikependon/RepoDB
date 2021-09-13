using Microsoft.Data.SqlClient;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="SqlParameter.TypeName"/> property via an entity property
    /// before the actual execution.
    /// </summary>
    public class SqlParameterTypeNameAttribute : ParameterPropertyValueSetterAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="SqlParameterTypeNameAttribute"/> class.
        /// </summary>
        /// <param name="typeName">The type name of the table-valued parameter (TVP) object.</param>
        public SqlParameterTypeNameAttribute(string typeName)
            : base(typeof(SqlParameter), nameof(SqlParameter.TypeName), typeName)
        { }

        /// <summary>
        /// Gets the name of the mapped table-valued parameter object (TVP) of the parameter.
        /// </summary>
        public string TypeName => (string)Value;
    }
}