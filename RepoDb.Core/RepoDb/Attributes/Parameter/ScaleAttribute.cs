using System.Data.Common;

namespace RepoDb.Attributes.Parameter
{
    /// <summary>
    /// An attribute that is being used to define a value to the <see cref="DbParameter.Scale"/>
    /// property via a class property mapping..
    /// </summary>
    public class ScaleAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="ScaleAttribute"/> class.
        /// </summary>
        /// <param name="scale">The scale of the parameter.</param>
        public ScaleAttribute(byte scale)
            : base(typeof(DbParameter), nameof(DbParameter.Scale), scale)
        { }

        /// <summary>
        /// Gets the mapped scale value of the parameter.
        /// </summary>
        public byte Scale => (byte)Value;
    }
}