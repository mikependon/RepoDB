using RepoDb.Attributes;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RepoDb
{
    /// <summary>
    /// An object that holds a field for ordering purposes.
    /// </summary>
    public class OrderField
    {
        /// <summary>
        /// Creates a new instance of <i>RepoDb.OrderField</i> object.
        /// </summary>
        /// <param name="name">The name of the field to be ordered.</param>
        /// <param name="order">The ordering direction of the field.</param>
        public OrderField(string name, Order order)
        {
            Name = name;
            Order = order;
        }

        /// <summary>
        /// Gets the name of the current order field.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the order direction of the field.
        /// </summary>
        public Order Order { get; }

        /// <summary>
        /// Gets the value of the <i>RepoDb.Attributes.TextAttribute.Text</i> thas was implemented on the ordering direction.
        /// </summary>
        /// <returns>The string containing the text value of the ordering direction.</returns>
        public string GetOrderText()
        {
            var textAttribute = typeof(Order)
                .GetTypeInfo()
                .GetMembers()
                .First(member => member.Name.ToLower() == Order.ToString().ToLower())
                .GetCustomAttribute<TextAttribute>();
            return textAttribute.Text;
        }

        // Static Methods

        /// <summary>
        /// Parse an object to be used for ordering. The object can have multiple properties for ordering and each property must have
        /// a value of <i>RepoDb.Enumerations.Order</i> enumeration.
        /// </summary>
        /// <param name="obj">
        /// An object to be parsed. Ex:
        /// <i>new { LastName = Order.Descending, FirstName = Order.Ascending }</i>
        /// </param>
        /// <returns>An enumerable of <i>RepoDb.OrderField</i> object that holds the ordering values for every field.</returns>
        public static IEnumerable<OrderField> Parse(object obj)
        {
            if (obj == null)
            {
                throw new NullReferenceException("The 'obj' must not be null.");
            }
            var list = new List<OrderField>();
            obj
                .GetType()
                .GetTypeInfo()
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

        // Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <i>OrderField</i>.
        /// </summary>
        /// <returns>The hashcode value.</returns>
        public override int GetHashCode()
        {
            return Name.GetHashCode() + Order.GetHashCode();
        }

        /// <summary>
        /// Compares the <i>OrderField</i> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equals.</returns>
        public override bool Equals(object obj)
        {
            return GetHashCode() == obj?.GetHashCode();
        }

        /// <summary>
        /// Compares the <i>OrderField</i> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(OrderField other)
        {
            return GetHashCode() == other?.GetHashCode();
        }

        /// <summary>
        /// Compares the equality of the two <i>OrderField</i> objects.
        /// </summary>
        /// <param name="objA">The first <i>OrderField</i> object.</param>
        /// <param name="objB">The second <i>OrderField</i> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(OrderField objA, OrderField objB)
        {
            if (ReferenceEquals(null, objA))
            {
                return ReferenceEquals(null, objB);
            }
            return objA?.GetHashCode() == objB?.GetHashCode();
        }

        /// <summary>
        /// Compares the inequality of the two <i>OrderField</i> objects.
        /// </summary>
        /// <param name="objA">The first <i>OrderField</i> object.</param>
        /// <param name="objB">The second <i>OrderField</i> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(OrderField objA, OrderField objB)
        {
            return (objA == objB) == false;
        }
    }
}