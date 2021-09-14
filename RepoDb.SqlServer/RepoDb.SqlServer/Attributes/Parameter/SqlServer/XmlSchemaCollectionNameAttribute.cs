using Microsoft.Data.SqlClient;

namespace RepoDb.Attributes.Parameter.SqlServer
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="SqlParameter.XmlSchemaCollectionName"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class XmlSchemaCollectionNameAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="XmlSchemaCollectionNameAttribute"/> class.
        /// </summary>
        /// <param name="collectionName">The value of the schema collection.</param>
        public XmlSchemaCollectionNameAttribute(string collectionName)
            : base(typeof(SqlParameter), nameof(SqlParameter.XmlSchemaCollectionName), collectionName)
        { }

        /// <summary>
        /// Gets the mapped value of the schema collection of the parameter.
        /// </summary>
        public string XmlSchemaCollectionName => (string)Value;
    }
}