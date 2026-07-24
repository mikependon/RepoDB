using Oracle.ManagedDataAccess.Client;

namespace RepoDb.Attributes.Parameter.Oracle
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="OracleParameter.OracleDbType"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class OracleDbTypeAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="OracleDbTypeAttribute"/> class.
        /// </summary>
        /// <param name="oracleDbType">The value of the target <see cref="Oracle.ManagedDataAccess.Client.OracleDbType"/>.</param>
        public OracleDbTypeAttribute(OracleDbType oracleDbType)
            : base(typeof(OracleParameter), nameof(OracleParameter.OracleDbType), oracleDbType)
        { }

        /// <summary>
        /// Gets the mapped <see cref="Oracle.ManagedDataAccess.Client.OracleDbType"/> value of the parameter.
        /// </summary>
        public OracleDbType OracleDbType => (OracleDbType)Value;
    }
}
