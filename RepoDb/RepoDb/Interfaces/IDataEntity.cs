namespace RepoDb.Interfaces
{
    public interface IDataEntity
    {
        object GetValue(string property);

        T GetValue<T>(string property);
    }
}
