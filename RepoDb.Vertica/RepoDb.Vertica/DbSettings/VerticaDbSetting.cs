using Vertica.Data.VerticaClient;

namespace RepoDb.DbSettings
{
    /// <summary>
    /// A setting class used for <see cref="VerticaConnection"/> data provider.
    /// </summary>
    public sealed class VerticaDbSetting : BaseDbSetting
    {
        /// <summary>
        /// Creates a new instance of <see cref="VerticaDbSetting"/> class.
        /// </summary>
        public VerticaDbSetting()
        {
            AreTableHintsSupported = false;
            ClosingQuote = "\"";
            DefaultSchema = null;
            IsAffectedRowsSupported = true;
            IsDirectionSupported = false;
            IsExecuteReaderDisposable = false;
            IsMultiStatementExecutable = false;
            IsPreparable = true;
            IsTransactionSupported = true;
            IsUseUpsert = false;
            RequiresDbTypeBeforeValue = true;

            // Vertica's driver (SCommand.GetParameterMappings) strictly validates that the number of bound
            // parameters matches the number of placeholders it finds in the command text, and throws "Too many
            // parameters in the parameter collection" otherwise. A null-valued equality filter renders as a
            // literal "IS NULL" with no placeholder (see QueryField.GetString()), so the corresponding parameter
            // must not be bound - see IDbSetting.SkipsUnreferencedParameters.
            SkipsUnreferencedParameters = true;

            MaxParameterCount = 1500;
            MultiStatementSeparator = ";";
            OpeningQuote = "\"";
            ParameterPrefix = "@";
            SqlTextParameterPrefix = "@";
        }
    }
}
