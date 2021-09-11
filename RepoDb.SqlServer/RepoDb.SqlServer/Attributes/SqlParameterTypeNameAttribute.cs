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
        /// <param name="typeName">The name of the type.</param>
        public SqlParameterTypeNameAttribute(string typeName)
            : base(typeof(SqlParameter), nameof(SqlParameter.TypeName), typeName)
        { }

        /// <summary>
        /// Gets a type name that is currently mapped.
        /// </summary>
        public string TypeName => (string)Value;
    }
}