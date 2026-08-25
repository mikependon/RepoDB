using RepoDb.Interfaces;

namespace RepoDb.ClickHouse.Interfaces
{
    /// <summary>
    /// Defines a contract for the ClickHouse database setting.
    /// </summary>
    public interface IClickHouseDbSetting : IDbSetting
    {
        /// <summary>
        /// Gets or sets a value indicating whether waiting for mutations to complete is enabled for the ClickHouse database.
        /// </summary>
        public bool IsWaitForMutationsEnabled { get; set;}
    }
}
