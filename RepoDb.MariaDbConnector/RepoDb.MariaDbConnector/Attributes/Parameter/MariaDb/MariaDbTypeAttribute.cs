using RepoDb.Connector.MariaDbConnector;

namespace RepoDb.Attributes.Parameter.MariaDb
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="MariaDbParameter.MariaDbType"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class MariaDbTypeAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="MariaDbTypeAttribute"/> class.
        /// </summary>
        /// <param name="mariaDbType">A target <see cref="global::RepoDb.Connector.MariaDbConnector.MariaDbType"/> value.</param>
        public MariaDbTypeAttribute(MariaDbType mariaDbType)
            : base(typeof(MariaDbParameter), nameof(MariaDbParameter.MariaDbType), mariaDbType)
        { }

        /// <summary>
        /// Gets the mapped <see cref="global::RepoDb.Connector.MariaDbConnector.MariaDbType"/> value of the parameter.
        /// </summary>
        public MariaDbType MariaDbType => (MariaDbType)Value;
    }
}
