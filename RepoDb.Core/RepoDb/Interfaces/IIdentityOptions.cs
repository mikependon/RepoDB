namespace RepoDb.Interfaces
{
    public interface IIdentityOptions<T> where T : class
    {
        IIdentityOptions<T> Column(string column);
    }
}