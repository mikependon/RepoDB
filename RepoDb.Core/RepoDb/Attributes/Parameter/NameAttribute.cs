using RepoDb.Extensions;
using System.Data.Common;

namespace RepoDb.Attributes.Parameter
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="DbParameter.ParameterName"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class NameAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="NameAttribute"/> class.
        /// </summary>
        /// <param name="name">The name of the mapping that is equivalent to the database object/field.</param>
        public NameAttribute(string name)
            : base(typeof(DbParameter), nameof(DbParameter.ParameterName), name.AsParameter())
        { }

        /// <summary>
        /// Gets the mapped name of the equivalent database object/field.
        /// </summary>
        public string Name => (string)Value;
    }
}