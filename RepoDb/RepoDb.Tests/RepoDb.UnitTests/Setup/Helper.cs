using RepoDb.Interfaces;

namespace RepoDb.UnitTests.Setup
{
    public static class Helper
    {
        public static IDbSetting DbSetting => new CustomDbSetting();
    }
}
