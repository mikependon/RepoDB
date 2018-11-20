using RepoDb.Interfaces;
using RepoDb.Enumerations;

namespace RepoDb
{
    /// <summary>
    /// A map item between the <see cref="Provider"/>and <see cref="IStatementBuilder"/>.
    /// </summary>
    public class StatementBuilderMapItem
    {
        /// <summary>
        /// Creates a new instance of <see cref="StatementBuilderMapItem"/> object.
        /// </summary>
        /// <param name="provider">The target provider.</param>
        /// <param name="statementBuilder">The statement builder to be used for mapping.</param>
        public StatementBuilderMapItem(Provider provider, IStatementBuilder statementBuilder)
        {
            Provider = provider;
            StatementBuilder = statementBuilder;
        }

        /// <summary>
        /// Gets the provider.
        /// </summary>
        public Provider Provider { get; }

        /// <summary>
        /// Gets the statement builder object that is being used for mapping.
        /// </summary>
        public IStatementBuilder StatementBuilder { get; internal set; }
    }
}
