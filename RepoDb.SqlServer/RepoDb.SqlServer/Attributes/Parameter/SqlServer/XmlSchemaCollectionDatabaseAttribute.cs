using Microsoft.Data.SqlClient;

namespace RepoDb.Attributes.Parameter.SqlServer
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="SqlParameter.XmlSchemaCollectionDatabase"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class XmlSchemaCollectionDatabaseAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="XmlSchemaCollectionDatabaseAttribute"/> class.
        /// </summary>
        /// <param name="databaseName">The name of the database where the schema collection is located.</param>
        public XmlSchemaCollectionDatabaseAttribute(string databaseName)
            : base(typeof(SqlParameter), nameof(SqlParameter.XmlSchemaCollectionDatabase), databaseName)
        { }

        /// <summary>
        /// Gets the mapped name of the database where the schema collection is located for the parameter.
        /// </summary>
        public string XmlSchemaCollectionDatabase => (string)Value;
    }
}