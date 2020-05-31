using System.Collections;

namespace RepoDb.SqlServer.BulkOperations
{
    /// <summary>
    /// A class that is being used as a return of the bulk operations that requires the result of the identities.
    /// </summary>
    internal class BulkOperationIdentitiesResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="BulkOperationIdentitiesResult"/> object.
        /// </summary>
        /// <param name="count">The count of the affected rows.</param>
        /// <param name="identityPropertyName">The name of the identity property.</param>
        /// <param name="identities">The value of the identity results.</param>
        public BulkOperationIdentitiesResult(int count,
            string identityPropertyName,
            IEnumerable identities)
        {
            Count = count;
            IdentityPropertyName = identityPropertyName;
            Identities = identities;
        }

        /// <summary>
        /// Gets the count of the affected rows.
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Gets the identity property name.
        /// </summary>
        public string IdentityPropertyName { get; }

        /// <summary>
        /// Gets ths list of the identities returned.
        /// </summary>
        public IEnumerable Identities { get; }
    }
}
