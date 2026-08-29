using ClickHouse.Driver.ADO.Parameters;

namespace RepoDb.Attributes.Parameter.ClickHouse
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="ClickHouseDbParameter.ClickHouseType"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class ClickHouseTypeAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="ClickHouseTypeAttribute"/> class.
        /// </summary>
        /// <param name="clickHouseType">A target ClickHouse type name (e.g. <c>"UInt64"</c>, <c>"Nullable(String)"</c>).</param>
        public ClickHouseTypeAttribute(string clickHouseType)
            : base(typeof(ClickHouseDbParameter), nameof(ClickHouseDbParameter.ClickHouseType), clickHouseType)
        { }

        /// <summary>
        /// Gets the mapped ClickHouse type name of the parameter.
        /// </summary>
        public string ClickHouseType => (string)Value;
    }
}
