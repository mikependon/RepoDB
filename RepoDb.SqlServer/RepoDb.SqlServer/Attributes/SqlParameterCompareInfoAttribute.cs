using Microsoft.Data.SqlClient;
using System.Data.SqlTypes;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="SqlParameter.CompareInfo"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class SqlParameterCompareInfoAttribute : ParameterPropertyValueSetterAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="SqlParameterCompareInfoAttribute"/> class.
        /// </summary>
        /// <param name="compareInfo">The value that determines how the string comparission is being defined.</param>
        public SqlParameterCompareInfoAttribute(SqlCompareOptions compareInfo)
            : base(typeof(SqlParameter), nameof(SqlParameter.CompareInfo), compareInfo)
        { }

        /// <summary>
        /// Gets the mapped value that determines how the string comparission is being defined on the parameter.
        /// </summary>
        public SqlCompareOptions CompareInfo => (SqlCompareOptions)Value;
    }
}