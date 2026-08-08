using IBM.Data.Db2;

namespace RepoDb.Attributes.Parameter.Db2
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="DB2Parameter.DB2Type"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class Db2TypeAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="Db2TypeAttribute"/> class.
        /// </summary>
        /// <param name="db2Type">The value of the target <see cref="IBM.Data.Db2.DB2Type"/>.</param>
        public Db2TypeAttribute(DB2Type db2Type)
            : base(typeof(DB2Parameter), nameof(DB2Parameter.DB2Type), db2Type)
        { }

        /// <summary>
        /// Gets the mapped <see cref="IBM.Data.Db2.DB2Type"/> value of the parameter.
        /// </summary>
        public DB2Type DB2Type => (DB2Type)Value;
    }
}
