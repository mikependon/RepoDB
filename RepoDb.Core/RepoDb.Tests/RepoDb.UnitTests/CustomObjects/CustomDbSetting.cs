using System;
using RepoDb.Interfaces;

namespace RepoDb.UnitTests.CustomObjects
{
    public class CustomDbSetting : IDbSetting
    {
        public bool IsMultipleStatementExecutionSupported => true;
        public bool IsUseUpsertForMergeOperation => false;
        public bool AreTableHintsSupported => true;
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
        public bool IsUseUpsertForMergeOperation => false;
        public bool AreTableHintsSupported => false;
        public string OpeningQuote => "[";
        public string ClosingQuote => "]";
        public string ParameterPrefix => "@";
        public string SchemaSeparator => ".";
        public string DefaultSchema => "dbo";
        public Type DefaultAverageableType => typeof(double);
    }

    public class CustomSingleStatementSupportDbSetting : IDbSetting
    {
        public bool IsMultipleStatementExecutionSupported => false;
        public bool IsUseUpsertForMergeOperation => false;
        public bool AreTableHintsSupported => true;
        public string OpeningQuote => "[";
        public string ClosingQuote => "]";
        public string ParameterPrefix => "@";
        public string SchemaSeparator => ".";
        public string DefaultSchema => "dbo";
        public Type DefaultAverageableType => typeof(double);
    }
}
