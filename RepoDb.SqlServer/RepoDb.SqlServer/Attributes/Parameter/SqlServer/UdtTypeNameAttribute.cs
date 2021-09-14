using Microsoft.Data.SqlClient;

namespace RepoDb.Attributes.Parameter.SqlServer
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="SqlParameter.UdtTypeName"/> property via an entity property
    /// before the actual execution.
    /// </summary>
    public class UdtTypeNameAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="UdtTypeNameAttribute"/> class.
        /// </summary>
        /// <param name="udtTypeName">The name of the user-defined type.</param>
        public UdtTypeNameAttribute(string udtTypeName)
            : base(typeof(SqlParameter), nameof(SqlParameter.UdtTypeName), udtTypeName)
        { }

        /// <summary>
        /// Gets the name of the currently mapped user-defined type of the parameter.
        /// </summary>
        public string UdtTypeName => (string)Value;
    }
}