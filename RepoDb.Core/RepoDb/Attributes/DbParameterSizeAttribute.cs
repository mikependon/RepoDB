using System.Data.Common;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="DbParameter.Size"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class DbParameterSizeAttribute : ParameterPropertyValueSetterAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="DbParameterSizeAttribute"/> class.
        /// </summary>
        /// <param name="size">The size of the parameter.</param>
        public DbParameterSizeAttribute(int size)
            : base(typeof(DbParameter), nameof(DbParameter.Size), size)
        { }

        /// <summary>
        /// Gets the mapped size value of the parameter.
        /// </summary>
        public int Size => (int)Value;
    }
}