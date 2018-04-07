namespace RepoDb.Interfaces
{
    public interface ITraceLog
    {
        object Result { get; }
        object Parameter { get; set; }
        string Statement { get; set; }
    }
}