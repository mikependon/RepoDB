using RepoDb.Enumerations;

namespace RepoDb.Interfaces
{
    public interface IOrderField : IField
    {
        Order Order { get; set; }
        string GetOrderText();
    }
}