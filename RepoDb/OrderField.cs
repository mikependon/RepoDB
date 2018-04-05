using RepoDb.Enumerations;
using RepoDb.Interfaces;

namespace RepoDb
{
    public class OrderField : Field, IOrderField
    {
        public OrderField(string name, Order order)
            : base(name)
        {
            Order = order;
        }

        public Order Order { get; set; }
    }
}