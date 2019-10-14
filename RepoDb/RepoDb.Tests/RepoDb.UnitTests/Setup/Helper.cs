using RepoDb.Interfaces;

namespace RepoDb.UnitTests.Setup
{
    public static class Helper
    {
        static Helper()
        {
            DbSetting = new CustomDbSetting();
        }
        public static IDbSetting DbSetting { get; }
    }
}
