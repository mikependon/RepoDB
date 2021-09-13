using System.Data.Common;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="DbParameter.ParameterName"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class DbParameterNameAttribute : ParameterPropertyValueSetterAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="DbParameterNameAttribute"/> class.
        /// </summary>
        /// <param name="name">The target name of the parameter.</param>
        public DbParameterNameAttribute(string name)
            : base(typeof(DbParameter), nameof(DbParameter.ParameterName), name)
        { }

        /// <summary>
        /// Gets the mapped name of the parameter.
        /// </summary>
        public string Name => (string)Value;
    }
}