namespace RepoDb.Interfaces
{
    public interface ITraceLog
    {
        object Parameter { get; }
        string Statement { get; }
        object Result { get; }
    }
}