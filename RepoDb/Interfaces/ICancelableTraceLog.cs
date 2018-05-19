namespace RepoDb.Interfaces
{
    public interface ICancelableTraceLog: ITraceLog
    {
        void Cancel(bool throwException);
        bool IsCanceled { get; }
        bool IsThrowException { get; }
    }
}