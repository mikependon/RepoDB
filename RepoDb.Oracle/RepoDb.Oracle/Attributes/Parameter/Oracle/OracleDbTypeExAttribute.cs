using Oracle.ManagedDataAccess.Client;

namespace RepoDb.Attributes.Parameter.Oracle
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="OracleParameter.OracleDbTypeEx"/>
    /// property via an entity property before the actual execution. Unlike <see cref="OracleDbTypeAttribute"/>,
    /// binding via this property causes the output value to be returned as a plain .NET type instead of an
    /// Oracle-specific wrapper type (e.g. plain <see cref="System.DateTime"/> instead of <c>OracleDate</c>).
    /// </summary>
    public class OracleDbTypeExAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="OracleDbTypeExAttribute"/> class.
        /// </summary>
        /// <param name="oracleDbType">The value of the target <see cref="Oracle.ManagedDataAccess.Client.OracleDbType"/>.</param>
        public OracleDbTypeExAttribute(OracleDbType oracleDbType)
            : base(typeof(OracleParameter), nameof(OracleParameter.OracleDbTypeEx), oracleDbType)
        { }

        /// <summary>
        /// Gets the mapped <see cref="Oracle.ManagedDataAccess.Client.OracleDbType"/> value of the parameter.
        /// </summary>
        public OracleDbType OracleDbType => (OracleDbType)Value;
    }
}
