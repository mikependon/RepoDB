using Microsoft.Data.SqlClient;
using System.Data;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="SqlParameter.LocaleId"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class SqlParameterLocaleIdAttribute : ParameterPropertyValueSetterAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="SqlParameterLocaleIdAttribute"/> class.
        /// </summary>
        /// <param name="localeId">The value of the locale identifier.</param>
        public SqlParameterLocaleIdAttribute(SqlDbType localeId)
            : base(typeof(SqlParameter), nameof(SqlParameter.LocaleId), localeId)
        { }

        /// <summary>
        /// Gets the mapped value of the local identifier.
        /// </summary>
        public int LocaleId => (int)Value;
    }
}