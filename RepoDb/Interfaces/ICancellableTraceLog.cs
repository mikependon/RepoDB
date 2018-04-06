namespace RepoDb.Interfaces
{
    public interface ICancellableTraceLog: ITraceLog
    {
        void Cancel(bool throwException);
        bool IsCancelled { get; }
        bool IsThrowException { get; }
    }
}