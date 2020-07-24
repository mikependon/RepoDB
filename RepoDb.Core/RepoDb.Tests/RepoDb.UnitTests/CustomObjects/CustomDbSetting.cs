using RepoDb.DbSettings;

namespace RepoDb.UnitTests.CustomObjects
{
    public class CustomDbSetting : BaseDbSetting
    {
        public CustomDbSetting()
        {
            AreTableHintsSupported = true;
            AverageableType = typeof(double);
            ClosingQuote = "]";
            DefaultSchema = "dbo";
            IsDirectionSupported = false;
            IsExecuteReaderDisposable = true;
            IsMultiStatementExecutable = true;
            IsPreparable = true;
            IsUseUpsert = false;
            OpeningQuote = "[";
            ParameterPrefix = "@";
        }
    }

    public class CustomNonHintsSupportingDbSetting : BaseDbSetting
    {
        public CustomNonHintsSupportingDbSetting()
        {
            AreTableHintsSupported = false;
            AverageableType = typeof(double);
            ClosingQuote = "]";
            DefaultSchema = "dbo";
            IsDirectionSupported = false;
            IsExecuteReaderDisposable = true;
            IsMultiStatementExecutable = true;
            IsPreparable = true;
            IsUseUpsert = false;
            OpeningQuote = "[";
            ParameterPrefix = "@";

        }
    }

    public class CustomSingleStatementSupportDbSetting : BaseDbSetting
    {
        public CustomSingleStatementSupportDbSetting()
        {
            AreTableHintsSupported = true;
            AverageableType = typeof(double);
            ClosingQuote = "]";
            DefaultSchema = "dbo";
            IsDirectionSupported = false;
            IsExecuteReaderDisposable = true;
            IsMultiStatementExecutable = false;
            IsPreparable = true;
            IsUseUpsert = false;
            OpeningQuote = "[";
            ParameterPrefix = "@";
        }
    }
}
