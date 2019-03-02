using System.Data;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to extract the result of the asynchronous method. This class assures that the 
    /// <see cref="IDbConnection"/> object used during the asynchronous operation within the repositories will be disposed.
    /// </summary>
    /// <typeparam name="T">The type of the result set.</typeparam>
    public class AsyncResultExtractor<T>
    {
        /// <summary>
        /// Creates a new instance of <see cref="AsyncResultExtractor{T}"/> class.
        /// </summary>
        /// <param name="result">The type of the result.</param>
        internal AsyncResultExtractor(Task<T> result) : this(result, null) { }

        /// <summary>
        /// Creates a new instance of <see cref="AsyncResultExtractor{T}"/> class.
        /// </summary>
        /// <param name="result">The type of the result.</param>
        /// <param name="dbConnection">The instance of <see cref="IDbConnection"/> object used.</param>
        internal AsyncResultExtractor(Task<T> result, IDbConnection dbConnection)
        {
            DbConnection = dbConnection;
            Result = result;
        }

        /// <summary>
        /// Gets the instance of the <see cref="IDbConnection"/> object used.
        /// </summary>
        private IDbConnection DbConnection { get; }

        /// <summary>
        /// Gets the current result.
        /// </summary>
        private Task<T> Result { get; }

        /// <summary>
        /// Extract the actual result of the asynchronous operation and disposes the associated <see cref="IDbConnection"/> object used in the operation.
        /// </summary>
        /// <returns>The type of T object.</returns>
        public T Extract()
        {
            using (DbConnection)
            {
                return Result.Result;
            };
        }
    }
}
