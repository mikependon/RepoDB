using RepoDb.Extensions;
using System.Data.Common;

namespace RepoDb.Attributes.Parameter
{
    /// <summary>
    /// An attribute that is being used to define a value to the <see cref="DbParameter.ParameterName"/>
    /// property via a class property mapping..
    /// </summary>
    public class NameAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="NameAttribute"/> class.
        /// </summary>
        /// <param name="name">The name of the mapping that is equivalent to the database object/field.</param>
        public NameAttribute(string name)
            : base(typeof(DbParameter), nameof(DbParameter.ParameterName), name, false)
        { }

        /// <summary>
        /// Gets the mapped name of the equivalent database object/field.
        /// </summary>
        public string Name => (string)Value;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal override object GetValue() => Name.AsParameter();
    }
}