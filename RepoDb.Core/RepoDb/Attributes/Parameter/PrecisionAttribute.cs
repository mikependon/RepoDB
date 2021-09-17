using System.Data.Common;

namespace RepoDb.Attributes.Parameter
{
    /// <summary>
    /// An attribute that is being used to define a value to the <see cref="DbParameter.Precision"/>
    /// property via a class property mapping..
    /// </summary>
    public class PrecisionAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="PrecisionAttribute"/> class.
        /// </summary>
        /// <param name="precision">The precision of the parameter.</param>
        public PrecisionAttribute(byte precision)
            : base(typeof(DbParameter), nameof(DbParameter.Precision), precision)
        { }

        /// <summary>
        /// Gets the mapped precision value of the parameter.
        /// </summary>
        public byte Precision => (byte)Value;
    }
}