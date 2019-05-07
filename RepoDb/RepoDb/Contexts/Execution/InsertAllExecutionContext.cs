using System;
using System.Collections.Generic;
using System.Data.Common;

namespace RepoDb.Contexts.Execution
{
    /// <summary>
    /// An execution context class used by insert-all operation.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    internal class InsertAllExecutionContext<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// The execution command text.
        /// </summary>
        public string CommandText { get; set; }

        /// <summary>
        /// The list of the input <see cref="DbField"/> objects to be included in the execution.
        /// </summary>
        public IEnumerable<DbField> InputFields { get; set; }

        /// <summary>
        /// The list of the output <see cref="DbField"/> objects to be included in the execution.
        /// </summary>
        public IEnumerable<DbField> OutputFields { get; set; }

        /// <summary>
        /// The batch size of the execution.
        /// </summary>
        public int BatchSize { get; set; }

        /// <summary>
        /// The actual compiled function.
        /// </summary>
        public Action<DbCommand, IList<TEntity>> Func { get; set; }

        /// <summary>
        /// The actual setters of the identity function.
        /// </summary>
        public IEnumerable<Action<TEntity, DbCommand>> IdentitySetters { get; set; }
    }
}
