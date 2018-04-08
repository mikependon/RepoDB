using RepoDb.Attributes;
using RepoDb.Enumerations;
using RepoDb.Interfaces;
using System;
using System.Linq;
using System.Reflection;

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

        public string GetOrderText()
        {
            var textAttribute = typeof(Order)
                .GetMembers()
                .First(member => string.Equals(member.Name, Order.ToString(), StringComparison.InvariantCultureIgnoreCase))
                .GetCustomAttribute<TextAttribute>();
            return textAttribute.Text;
        }
    }
}