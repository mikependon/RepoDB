using RepoDb.Attributes.Parameter.ClickHouse;
using System;

namespace RepoDb.Attributes
{
    /// <summary>
    /// An attribute that is used to define a mapping of .NET CLR <see cref="Type"/> into its equivalent ClickHouse type name.
    /// </summary>
    [Obsolete("Use the RepoDb.Attributes.Parameter.ClickHouse.ClickHouseTypeAttribute instead.")]
    public class ClickHouseTypeMapAttribute : ClickHouseTypeAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="ClickHouseTypeMapAttribute"/> class.
        /// </summary>
        /// <param name="clickHouseType">A target ClickHouse type name.</param>
        public ClickHouseTypeMapAttribute(string clickHouseType)
            : base(clickHouseType)
        { }
    }
}
