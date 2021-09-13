using System.Data.Common;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="DbParameter.Scale"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class DbParameterScaleAttribute : ParameterPropertyValueSetterAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="DbParameterScaleAttribute"/> class.
        /// </summary>
        /// <param name="scale">The scale of the parameter.</param>
        public DbParameterScaleAttribute(byte scale)
            : base(typeof(DbParameter), nameof(DbParameter.Scale), scale)
        { }

        /// <summary>
        /// Gets the mapped scale value of the parameter.
        /// </summary>
        public byte Scale => (byte)Value;
    }
}