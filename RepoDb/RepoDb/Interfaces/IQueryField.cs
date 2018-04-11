using RepoDb.Enumerations;

namespace RepoDb.Interfaces
{
    public interface IQueryField
    {
        IField Field { get; }
        Operation Operation { get; }
        IParameter Parameter { get; }
        string GetOperationText();
    }
}
