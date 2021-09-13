using Npgsql;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="NpgsqlParameter.DataTypeName"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class NpgsqlParameterDataTypeNameAttribute : ParameterPropertyValueSetterAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="NpgsqlParameterDataTypeNameAttribute"/> class.
        /// </summary>
        /// <param name="dataTypeName">The name of the PostgreSQL type.</param>
        public NpgsqlParameterDataTypeNameAttribute(string dataTypeName)
            : base(typeof(NpgsqlParameter), nameof(NpgsqlParameter.DataTypeName), dataTypeName)
        { }

        /// <summary>
        /// Gets the mapped name of the PostgreSQL type of the parameter.
        /// </summary>
        public string DataTypeName => (string)Value;
    }
}