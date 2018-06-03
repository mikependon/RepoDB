namespace RepoDb.Interfaces
{
    /// <summary>
    /// An interface used to implement to mark a class to be a cancellable tracing log object used in the tracing operations.
    /// </summary>
    public interface ICancellableTraceLog: ITraceLog
    {
        /// <summary>
        /// Cancel the current executing repository operation.
        /// </summary>
        /// <param name="throwException">If true, an exception will be thrown.</param>
        void Cancel(bool throwException);

        /// <summary>
        /// Gets a value whether the operation is cancelled.
        /// </summary>
        bool IsCancelled { get; }

        /// <summary>
        /// Gets a value whether an exception will be thrown after the <i>Cancel</i> method was called.
        /// </summary>
        bool IsThrowException { get; }
    }
}