using System.Data;
using System.Data.Common;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="DbParameter.Direction"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class DbParameterDirectionAttribute : ParameterPropertyValueSetterAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="DbParameterDbTypeAttribute"/> class.
        /// </summary>
        /// <param name="direction">The value that indicates the direction of the parameter.</param>
        public DbParameterDirectionAttribute(ParameterDirection direction)
            : base(typeof(DbParameter), nameof(DbParameter.Direction), direction)
        { }

        /// <summary>
        /// Gets the mapped value that indicates whether the parameter is input, output, bidirectional 
        /// or a return value from the stored procedure.
        /// </summary>
        public ParameterDirection Direction => (ParameterDirection)Value;
    }
}