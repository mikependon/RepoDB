using RepoDb.Attributes;
using RepoDb.Enumerations;
using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
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

        // Static Methods

        public static IEnumerable<IOrderField> Parse(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            var list = new List<IOrderField>();
            obj
                .GetType()
                .GetProperties()
                .ToList()
                .ForEach(property =>
                {
                    if (property.PropertyType != typeof(Order))
                    {
                        throw new InvalidOperationException($"The type of field '{property.Name}' must be of '{typeof(Order).FullName}'.");
                    }
                    var order = (Order)property.GetValue(obj);
                    list.Add(new OrderField(property.Name, order));
                });
            return list;
        }
    }
}