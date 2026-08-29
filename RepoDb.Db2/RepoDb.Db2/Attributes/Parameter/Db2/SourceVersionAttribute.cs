using System.Data;
using IBM.Data.Db2;

namespace RepoDb.Attributes.Parameter.Db2
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="DB2Parameter.SourceVersion"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class SourceVersionAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="SourceVersionAttribute"/> class.
        /// </summary>
        /// <param name="sourceVersion">The value of the target <see cref="DataRowVersion"/>.</param>
        public SourceVersionAttribute(DataRowVersion sourceVersion)
            : base(typeof(DB2Parameter), nameof(DB2Parameter.SourceVersion), sourceVersion)
        { }

        /// <summary>
        /// Gets the mapped <see cref="DataRowVersion"/> value of the parameter.
        /// </summary>
        public DataRowVersion SourceVersion => (DataRowVersion)Value;
    }
}
