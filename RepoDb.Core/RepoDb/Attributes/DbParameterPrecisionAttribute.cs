using System.Data.Common;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="DbParameter.Precision"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class DbParameterPrecisionAttribute : ParameterPropertyValueSetterAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="DbParameterPrecisionAttribute"/> class.
        /// </summary>
        /// <param name="precision">The precision of the parameter.</param>
        public DbParameterPrecisionAttribute(byte precision)
            : base(typeof(DbParameter), nameof(DbParameter.Precision), precision)
        { }

        /// <summary>
        /// Gets the mapped precision value of the parameter.
        /// </summary>
        public byte Precision => (byte)Value;
    }
}