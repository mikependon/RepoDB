using RepoDb.Interfaces;

namespace RepoDb.ClickHouse.Interfaces
{
    /// <summary>
    /// Defines a contract for the ClickHouse database setting.
    /// </summary>
    public interface IClickHouseDbSetting : IDbSetting
    {
        public bool IsWaitForMutationsEnabled { get; set;}
    }
}
