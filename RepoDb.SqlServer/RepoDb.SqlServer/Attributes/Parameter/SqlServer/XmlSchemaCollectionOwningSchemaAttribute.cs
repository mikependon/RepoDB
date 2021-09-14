using Microsoft.Data.SqlClient;

namespace RepoDb.Attributes.Parameter.SqlServer
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="SqlParameter.XmlSchemaCollectionOwningSchema"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class XmlSchemaCollectionOwningSchemaAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="XmlSchemaCollectionOwningSchemaAttribute"/> class.
        /// </summary>
        /// <param name="owningSchema">The value of the owning relational schema.</param>
        public XmlSchemaCollectionOwningSchemaAttribute(string owningSchema)
            : base(typeof(SqlParameter), nameof(SqlParameter.XmlSchemaCollectionOwningSchema), owningSchema)
        { }

        /// <summary>
        /// Gets the mapped value of the owning relation schema of the parameter.
        /// </summary>
        public string XmlSchemaCollectionOwningSchema => (string)Value;
    }
}