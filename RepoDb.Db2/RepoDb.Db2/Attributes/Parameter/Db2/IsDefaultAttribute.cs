using IBM.Data.Db2;

namespace RepoDb.Attributes.Parameter.Db2
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="DB2Parameter.IsDefault"/>
    /// property via an entity property before the actual execution. Indicates whether the
    /// parameter uses the default value assigned by the data server.
    /// </summary>
    public class IsDefaultAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="IsDefaultAttribute"/> class.
        /// </summary>
        /// <param name="isDefault">The value that indicates whether the parameter uses the default value assigned by the data server.</param>
        public IsDefaultAttribute(bool isDefault)
            : base(typeof(DB2Parameter), nameof(DB2Parameter.IsDefault), isDefault)
        { }

        /// <summary>
        /// Gets the mapped value that indicates whether the parameter uses the default value assigned by the data server.
        /// </summary>
        public bool IsDefault => (bool)Value;
    }
}
