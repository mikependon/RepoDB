using System.Data.Common;

namespace RepoDb.Attributes.Parameter
{
    /// <summary>
    /// An attribute that is being used to define a value to the <see cref="DbParameter.Size"/>
    /// property via a class property mapping..
    /// </summary>
    public class SizeAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="SizeAttribute"/> class.
        /// </summary>
        /// <param name="size">The size of the parameter.</param>
        public SizeAttribute(int size)
            : base(typeof(DbParameter), nameof(DbParameter.Size), size)
        { }

        /// <summary>
        /// Gets the mapped size value of the parameter.
        /// </summary>
        public int Size => (int)Value;
    }
}