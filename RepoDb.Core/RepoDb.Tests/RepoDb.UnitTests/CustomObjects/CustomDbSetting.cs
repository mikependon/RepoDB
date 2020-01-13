using System;
using RepoDb.Interfaces;

namespace RepoDb.UnitTests.CustomObjects
{
    public class CustomDbSetting : IDbSetting
    {
        public bool AreTableHintsSupported => true;
        public Type DefaultAverageableType => typeof(double);
        public string ClosingQuote => "]";
        public string DefaultSchema => "dbo";
        public bool IsDbParameterDirectionSettingSupported => false;
        public bool IsDisposeDbCommandAfterExecuteReader => true;
        public bool IsMultipleStatementExecutionSupported => true;
        public bool IsPreparable => true;
        public bool IsUseUpsertForMergeOperation => false;
        public string OpeningQuote => "[";
        public string ParameterPrefix => "@";
        public string SchemaSeparator => ".";
    }

    public class CustomNonHintsSupportingDbSetting : IDbSetting
    {
        public bool AreTableHintsSupported => false;
        public Type DefaultAverageableType => typeof(double);
        public string ClosingQuote => "]";
        public string DefaultSchema => "dbo";
        public bool IsDbParameterDirectionSettingSupported => false;
        public bool IsDisposeDbCommandAfterExecuteReader => true;
        public bool IsMultipleStatementExecutionSupported => true;
        public bool IsPreparable => true;
        public bool IsUseUpsertForMergeOperation => false;
        public string OpeningQuote => "[";
        public string ParameterPrefix => "@";
        public string SchemaSeparator => ".";
    }

    public class CustomSingleStatementSupportDbSetting : IDbSetting
    {
        public bool AreTableHintsSupported => true;
        public Type DefaultAverageableType => typeof(double);
        public string ClosingQuote => "]";
        public string DefaultSchema => "dbo";
        public bool IsDbParameterDirectionSettingSupported => false;
        public bool IsDisposeDbCommandAfterExecuteReader => true;
        public bool IsMultipleStatementExecutionSupported => false;
        public bool IsPreparable => true;
        public bool IsUseUpsertForMergeOperation => false;
        public string OpeningQuote => "[";
        public string ParameterPrefix => "@";
        public string SchemaSeparator => ".";
    }
}
