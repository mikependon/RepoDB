using System;
using RepoDb.Interfaces;

namespace RepoDb.UnitTests.CustomObjects
{
    public class CustomDbSetting : IDbSetting
    {
        public bool IsMultipleStatementExecutionSupported => true;
        public bool AreTableHintsSupported => true;
        public bool IsCountBigSupported => true;
        public string OpeningQuote => "[";
        public string ClosingQuote => "]";
        public string ParameterPrefix => "@";
        public string SchemaSeparator => ".";
        public string DefaultSchema => "dbo";
        public Type DefaultAverageableType => typeof(double);
    }

    public class CustomNonHintsSupportingDbSetting : IDbSetting
    {
        public bool IsMultipleStatementExecutionSupported => true;
        public bool AreTableHintsSupported => false;
        public bool IsCountBigSupported => true;
        public string OpeningQuote => "[";
        public string ClosingQuote => "]";
        public string ParameterPrefix => "@";
        public string SchemaSeparator => ".";
        public string DefaultSchema => "dbo";
        public Type DefaultAverageableType => typeof(double);
    }
}
