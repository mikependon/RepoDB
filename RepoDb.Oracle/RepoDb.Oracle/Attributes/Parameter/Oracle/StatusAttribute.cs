using Oracle.ManagedDataAccess.Client;

namespace RepoDb.Attributes.Parameter.Oracle
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="OracleParameter.Status"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class StatusAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="StatusAttribute"/> class.
        /// </summary>
        /// <param name="status">The value of the target <see cref="OracleParameterStatus"/>.</param>
        public StatusAttribute(OracleParameterStatus status)
            : base(typeof(OracleParameter), nameof(OracleParameter.Status), status)
        { }

        /// <summary>
        /// Gets the mapped <see cref="OracleParameterStatus"/> value of the parameter.
        /// </summary>
        public OracleParameterStatus Status => (OracleParameterStatus)Value;
    }
}
