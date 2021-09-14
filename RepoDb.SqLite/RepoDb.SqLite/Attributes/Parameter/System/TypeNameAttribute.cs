using System.Data.SQLite;

namespace RepoDb.Attributes.Parameter.SQLite
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="SQLiteParameter.TypeName"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class TypeNameAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="TypeNameAttribute"/> class.
        /// </summary>
        /// <param name="typeName">The name of the target type.</param>
        public TypeNameAttribute(string typeName)
            : base(typeof(SQLiteParameter), nameof(SQLiteParameter.TypeName), typeName)
        { }

        /// <summary>
        /// Gets the mapped type name of the parameter.
        /// </summary>
        public string TypeName => (string)Value;
    }
}