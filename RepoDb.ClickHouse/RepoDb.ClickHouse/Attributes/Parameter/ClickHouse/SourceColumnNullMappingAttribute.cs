using ClickHouse.Driver.ADO.Parameters;

namespace RepoDb.Attributes.Parameter.ClickHouse
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="ClickHouseDbParameter.SourceColumnNullMapping"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class SourceColumnNullMappingAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="SourceColumnNullMappingAttribute"/> class.
        /// </summary>
        /// <param name="sourceColumnNullMapping">The value that indicates whether the source column is nullable.</param>
        public SourceColumnNullMappingAttribute(bool sourceColumnNullMapping)
            : base(typeof(ClickHouseDbParameter), nameof(ClickHouseDbParameter.SourceColumnNullMapping), sourceColumnNullMapping)
        { }

        /// <summary>
        /// Gets the mapped value that indicates whether the source column is nullable.
        /// </summary>
        public bool SourceColumnNullMapping => (bool)Value;
    }
}
