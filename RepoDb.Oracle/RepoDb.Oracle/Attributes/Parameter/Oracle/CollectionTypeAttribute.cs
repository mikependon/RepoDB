using Oracle.ManagedDataAccess.Client;

namespace RepoDb.Attributes.Parameter.Oracle
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="OracleParameter.CollectionType"/>
    /// property via an entity property before the actual execution. Set this to
    /// <see cref="OracleCollectionType.PLSQLAssociativeArray"/> together with <see cref="ArrayBindSizeAttribute"/>
    /// when binding a PL/SQL associative array (e.g. for array-bind based batch execution).
    /// </summary>
    public class CollectionTypeAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="CollectionTypeAttribute"/> class.
        /// </summary>
        /// <param name="collectionType">The value of the target <see cref="OracleCollectionType"/>.</param>
        public CollectionTypeAttribute(OracleCollectionType collectionType)
            : base(typeof(OracleParameter), nameof(OracleParameter.CollectionType), collectionType)
        { }

        /// <summary>
        /// Gets the mapped <see cref="OracleCollectionType"/> value of the parameter.
        /// </summary>
        public OracleCollectionType CollectionType => (OracleCollectionType)Value;
    }
}
