using System;

namespace RepoDb
{
    /// <summary>
    /// A manager class for recursive query.
    /// </summary>
    public static class RecursionManager
    {
        static RecursionManager()
        {
            RecursiveQueryBatchCount = Constant.DefaultRecursiveQueryBatchCount;
            RecursiveQueryMaxRecursion = Constant.DefaultRecursiveQueryMaxRecursion;
        }

        /// <summary>
        /// Gets the batches count used by the repository (recursive query) operation. The default value is
        /// equals to <see cref="Constant.DefaultRecursiveQueryBatchCount"/> value.
        /// </summary>
        public static int RecursiveQueryBatchCount { get; private set; }

        /// <summary>
        /// Gets the maximum recursion that the repository (recursion depth) can execute. The default value is
        /// equals to <see cref="Constant.DefaultRecursiveQueryMaxRecursion"/> value.
        /// </summary>
        public static int RecursiveQueryMaxRecursion { get; private set; }

        /// <summary>
        /// Sets the maximum recursion value of the repository (recursion depth) execution.
        /// </summary>
        /// <param name="maximumRecursion">The value of the maximum recursion.</param>
        public static void SetRecursiveMaximumRecursion(int maximumRecursion)
        {
            // Validate negative values
            if (maximumRecursion < 0)
            {
                throw new InvalidOperationException("The recursive maximum recursion value must not be negative values.");
            }

            // Set the value
            RecursiveQueryMaxRecursion = maximumRecursion;
        }

        /// <summary>
        /// Sets the batches count to be used by the repository (recursion query) operation.
        /// </summary>
        /// <param name="batchCount">The value of the batches count.</param>
        public static void SetRecursiveQueryBatchCount(int batchCount)
        {
            // Validate negative values
            if (batchCount < 0)
            {
                throw new InvalidOperationException("The recursive query batch count value must not be negative values.");
            }

            // Set the value
            RecursiveQueryBatchCount = batchCount;
        }
    }
}
