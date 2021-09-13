using System.Data.SQLite;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="SQLiteParameter.TypeName"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class SQLiteParameterTypeNameAttribute : ParameterPropertyValueSetterAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="SQLiteParameterTypeNameAttribute"/> class.
        /// </summary>
        /// <param name="typeName">The name of the target type.</param>
        public SQLiteParameterTypeNameAttribute(string typeName)
            : base(typeof(SQLiteParameter), nameof(SQLiteParameter.TypeName), typeName)
        { }

        /// <summary>
        /// Gets the mapped type name of the parameter.
        /// </summary>
        public string TypeName => (string)Value;
    }
}