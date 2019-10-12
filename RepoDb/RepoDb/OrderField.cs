using RepoDb.Attributes;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace RepoDb
{
    /// <summary>
    /// An object that holds a field for ordering purposes.
    /// </summary>
    public class OrderField
    {
        private TextAttribute m_orderTextAttribute = null;
        private int m_hashCode = 0;

        ///// <summary>
        ///// Creates a new instance of <see cref="OrderField"/> object.
        ///// </summary>
        ///// <param name="name">The name of the field to be ordered.</param>
        ///// <param name="order">The ordering direction of the field.</param>
        //public OrderField(string name,
        //    Order order)
        //    : this(name, order, null) { }

        /// <summary>
        /// Creates a new instance of <see cref="OrderField"/> object.
        /// </summary>
        /// <param name="name">The name of the field to be ordered.</param>
        /// <param name="order">The ordering direction of the field.</param>
        /// <param name="dbSetting">The database setting that is currently in used.</param>
        public OrderField(string name,
            Order order,
            IDbSetting dbSetting)
        {
            // Name is required
            if (string.IsNullOrEmpty(name))
            {
                throw new NullReferenceException(name);
            }

            // Set the properties
            DbSetting = dbSetting;
            Name = name.AsQuoted(true, dbSetting);
            UnquotedName = name.AsUnquoted(true, dbSetting);
            Order = order;

            // Set the hashcode here
            m_hashCode = name.GetHashCode() + (int)order;
            if (dbSetting != null)
            {
                m_hashCode += dbSetting.GetHashCode();
            }
        }

        /// <summary>
        /// Gets the quoted name of the order field.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the unquoted name of the order field.
        /// </summary>
        public string UnquotedName { get; }

        /// <summary>
        /// Gets the order direction of the field.
        /// </summary>
        public Order Order { get; }

        /// <summary>
        /// Gets the database setting currently in used.
        /// </summary>
        public IDbSetting DbSetting { get; }

        /// <summary>
        /// Gets the value of the <see cref="TextAttribute.Text"/> thas was implemented on the ordering direction.
        /// </summary>
        /// <returns>The string containing the text value of the ordering direction.</returns>
        public string GetOrderText()
        {
            if (m_orderTextAttribute == null)
            {
                m_orderTextAttribute = typeof(Order)
                    .GetMembers()
                    .First(member => member.Name.ToLower() == Order.ToString().ToLower())
                    .GetCustomAttribute<TextAttribute>();
            }
            return m_orderTextAttribute.Text;
        }

        #region Static Methods

        /// <summary>
        /// Parses a property from the data entity object based on the given <see cref="Expression"/> and converts the result 
        /// to <see cref="OrderField"/> object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that contains the property to be parsed.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        /// <param name="order">The order of the property.</param>
        /// <param name="dbSetting">The database setting that is currently in used.</param>
        /// <returns>An instance of <see cref="OrderField"/> object.</returns>
        public static OrderField Parse<TEntity>(Expression<Func<TEntity, object>> expression,
            Order order,
            IDbSetting dbSetting)
            where TEntity : class
        {
            if (expression.Body.IsUnary())
            {
                var unary = expression.Body.ToUnary();
                if (unary.Operand.IsMember())
                {
                    return new OrderField(unary.Operand.ToMember().Member.Name, order, dbSetting);
                }
                else if (unary.Operand.IsBinary())
                {
                    return new OrderField(unary.Operand.ToBinary().GetName(dbSetting), order, dbSetting);
                }
            }
            if (expression.Body.IsMember())
            {
                return new OrderField(expression.Body.ToMember().Member.Name, order, dbSetting);
            }
            if (expression.Body.IsBinary())
            {
                return new OrderField(expression.Body.ToBinary().GetName(dbSetting), order, dbSetting);
            }
            throw new InvalidQueryExpressionException($"Expression '{expression.ToString()}' is invalid.");
        }

        /// <summary>
        /// Parses a property from the data entity object based on the given <see cref="Expression"/> and converts the result 
        /// to <see cref="OrderField"/> object with <see cref="Order.Ascending"/> value.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that contains the property to be parsed.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        /// <param name="dbSetting">The database setting that is currently in used.</param>
        /// <returns>An instance of <see cref="OrderField"/> object with <see cref="Order.Ascending"/> value.</returns>
        public static OrderField Ascending<TEntity>(Expression<Func<TEntity, object>> expression,
            IDbSetting dbSetting)
            where TEntity : class
        {
            return Parse<TEntity>(expression, Order.Ascending, dbSetting);
        }

        /// <summary>
        /// Parses a property from the data entity object based on the given <see cref="Expression"/> and converts the result 
        /// to <see cref="OrderField"/> object with <see cref="Order.Descending"/> value.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that contains the property to be parsed.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        /// <param name="dbSetting">The database setting that is currently in used.</param>
        /// <returns>An instance of <see cref="OrderField"/> object with <see cref="Order.Descending"/> value.</returns>
        public static OrderField Descending<TEntity>(Expression<Func<TEntity, object>> expression,
            IDbSetting dbSetting)
            where TEntity : class
        {
            return Parse<TEntity>(expression, Order.Descending, dbSetting);
        }

        /// <summary>
        /// Parse an object properties to be used for ordering. The object can have multiple properties for ordering and each property must have
        /// a value of <see cref="Enumerations.Order"/> enumeration.
        /// </summary>
        /// <param name="obj">An object to be parsed.</param>
        /// <param name="dbSetting">The database setting that is currently in used.</param>
        /// <returns>An enumerable of <see cref="OrderField"/> object that holds the ordering values for every field.</returns>
        public static IEnumerable<OrderField> Parse(object obj,
            IDbSetting dbSetting)
        {
            if (obj == null)
            {
                throw new NullReferenceException("The 'obj' must not be null.");
            }
            var list = new List<OrderField>();
            foreach (var property in obj.GetType().GetProperties())
            {
                if (property.PropertyType != typeof(Order))
                {
                    throw new InvalidOperationException($"The type of field '{property.Name}' must be of '{typeof(Order).FullName}'.");
                }
                var order = (Order)property.GetValue(obj);
                list.Add(new OrderField(property.Name, order, dbSetting));
            }
            return list;
        }

        #endregion

        #region Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <see cref="OrderField"/>.
        /// </summary>
        /// <returns>The hashcode value.</returns>
        public override int GetHashCode()
        {
            return m_hashCode;
        }

        /// <summary>
        /// Compares the <see cref="OrderField"/> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equals.</returns>
        public override bool Equals(object obj)
        {
            return obj?.GetHashCode() == GetHashCode();
        }

        /// <summary>
        /// Compares the <see cref="OrderField"/> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(OrderField other)
        {
            return other?.GetHashCode() == GetHashCode();
        }

        /// <summary>
        /// Compares the equality of the two <see cref="OrderField"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="OrderField"/> object.</param>
        /// <param name="objB">The second <see cref="OrderField"/> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(OrderField objA, OrderField objB)
        {
            if (ReferenceEquals(null, objA))
            {
                return ReferenceEquals(null, objB);
            }
            return objB?.GetHashCode() == objA.GetHashCode();
        }

        /// <summary>
        /// Compares the inequality of the two <see cref="OrderField"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="OrderField"/> object.</param>
        /// <param name="objB">The second <see cref="OrderField"/> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(OrderField objA, OrderField objB)
        {
            return (objA == objB) == false;
        }

        #endregion
    }
}