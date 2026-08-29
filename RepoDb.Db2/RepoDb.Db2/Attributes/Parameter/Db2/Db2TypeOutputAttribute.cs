using IBM.Data.Db2;

namespace RepoDb.Attributes.Parameter.Db2
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="DB2Parameter.Db2TypeOutput"/>
    /// property via an entity property before the actual execution. Determines whether the
    /// output-only parameter value is returned as a native Db2 data type (a class or struct in
    /// the <c>IBM.Data.Db2Types</c> namespace) instead of a plain .NET type.
    /// </summary>
    public class Db2TypeOutputAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="Db2TypeOutputAttribute"/> class.
        /// </summary>
        /// <param name="db2TypeOutput">The value that indicates whether the output value is returned as a native Db2 data type.</param>
        public Db2TypeOutputAttribute(bool db2TypeOutput)
            : base(typeof(DB2Parameter), nameof(DB2Parameter.DB2TypeOutput), db2TypeOutput)
        { }

        /// <summary>
        /// Gets the mapped value that indicates whether the output value is returned as a native Db2 data type.
        /// </summary>
        public bool Db2TypeOutput => (bool)Value;
    }
}
