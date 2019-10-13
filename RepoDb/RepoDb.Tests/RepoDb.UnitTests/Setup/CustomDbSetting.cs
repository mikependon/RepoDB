using RepoDb.Interfaces;

namespace RepoDb.UnitTests.Setup
{
    public class CustomDbSetting : IDbSetting
    {
        public bool IsMultipleStatementExecutionSupported => true;

        public string OpeningQuote => "[";

        public string ClosingQuote => "]";

        public string ParameterPrefix => "@";

        public string SchemaSeparator => ".";

        public string DefaultSchema => "dbo";
    }
}
