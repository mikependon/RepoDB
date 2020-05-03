namespace RepoDb.Interfaces
{
    public interface IIdentityOptions<T> where T : class
    {
        /*
         * Column
         */

        IIdentityOptions<T> Column(string column);

        IIdentityOptions<T> Column(string column,
            bool force);
    }
}