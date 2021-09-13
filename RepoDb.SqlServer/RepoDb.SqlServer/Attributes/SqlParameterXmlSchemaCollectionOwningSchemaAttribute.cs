using Microsoft.Data.SqlClient;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="SqlParameter.XmlSchemaCollectionOwningSchema"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class SqlParameterXmlSchemaCollectionOwningSchemaAttribute : ParameterPropertyValueSetterAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="SqlParameterXmlSchemaCollectionOwningSchemaAttribute"/> class.
        /// </summary>
        /// <param name="owningSchema">The value of the owning relational schema.</param>
        public SqlParameterXmlSchemaCollectionOwningSchemaAttribute(string owningSchema)
            : base(typeof(SqlParameter), nameof(SqlParameter.XmlSchemaCollectionOwningSchema), owningSchema)
        { }

        /// <summary>
        /// Gets the mapped value of the owning relation schema of the parameter.
        /// </summary>
        public string XmlSchemaCollectionOwningSchema => (string)Value;
    }
}