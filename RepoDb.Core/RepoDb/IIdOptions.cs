namespace RepoDb
{
    public interface IIdOptions<T> where T : class
    {
        IIdOptions<T> Column(string column);
    }
}