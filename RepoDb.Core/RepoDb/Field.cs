using System.Collections.Generic;
using System.Linq;
using System;
using RepoDb.Extensions;
using System.Linq.Expressions;
using System.Reflection;
using RepoDb.Exceptions;

namespace RepoDb
{
    /// <summary>
    /// An object that is used to signify a field in the query statement. It is also used as a common object in relation to the context of field object.
    /// </summary>
    public class Field : IEquatable<Field>
    {
        private int? hashCode = null;

        /// <summary>
        /// Creates a new instance of <see cref="Field"/> object.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        public Field(string name) :
            this(name, null)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="Field"/> object.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        /// <param name="type">The type of the field.</param>
        public Field(string name,
            Type type)
        {
            // Name is required
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new NullReferenceException(name);
            }

            // Set the name
            Name = name;

            // Set the type
            Type = type;
        }

        #region Properties

        /// <summary>
        /// Gets the quoted name of the field.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets or sets the type of the field.
        /// </summary>
        public Type Type { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Stringify the current field object.
        /// </summary>
        /// <returns>The string value equivalent to the name of the field.</returns>
        public override string ToString() =>
            string.Concat(Name, ", ", Type?.FullName, " (", hashCode, ")");


        #endregion

        #region Static Methods

        /// <summary>
        /// Creates an enumerable of <see cref="Field"/> objects that derived from the string value.
        /// </summary>
        /// <param name="name">The enumerable of string values that signifies the name of the fields (for each item).</param>
        /// <returns>An enumerable of <see cref="Field"/> object.</returns>
        public static IEnumerable<Field> From(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new NullReferenceException("The field name must be null or empty.");
            }
            return From(new[] { name });
        }

        /// <summary>
        /// Creates an enumerable of <see cref="Field"/> objects that derived from the given array of string values.
        /// </summary>
        /// <param name="fields">The enumerable of string values that signifies the name of the fields (for each item).</param>
        /// <returns>An enumerable of <see cref="Field"/> object.</returns>
        public static IEnumerable<Field> From(params string[] fields)
        {
            if (fields == null)
            {
                throw new NullReferenceException("The list of fields must not be null.");
            }
            if (fields.Any(field => string.IsNullOrWhiteSpace(field)))
            {
                throw new NullReferenceException("The field name must be null or empty.");
            }
            foreach (var field in fields)
            {
                yield return new Field(field);
            }
        }

        /// <summary>
        /// Parses an object and creates an enumerable of <see cref="Field"/> objects.
        /// </summary>
        /// <param name="obj">An object to be parsed.</param>
        /// <returns>An enumerable of <see cref="Field"/> objects.</returns>
        public static IEnumerable<Field> Parse(object obj) =>
            obj?.GetType().IsDictionaryStringObject() == true ?
                ParseDictionaryStringObject((IDictionary<string, object>)obj) : Parse(obj?.GetType());

        /// <summary>
        /// Parses an object and creates an enumerable of <see cref="Field"/> objects.
        /// </summary>
        /// <typeparam name="TEntity">The target type.</typeparam>
        /// <returns>An enumerable of <see cref="Field"/> objects.</returns>
        public static IEnumerable<Field> Parse<TEntity>()
            where TEntity : class =>
            Parse(typeof(TEntity));

        /// <summary>
        /// Parses a type and creates an enumerable of <see cref="Field"/> objects.
        /// </summary>
        /// <returns>An enumerable of <see cref="Field"/> objects.</returns>
        public static IEnumerable<Field> Parse(Type type)
        {
            if (type != null)
            {
                foreach (var property in type.GetProperties())
                {
                    yield return property.AsField();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static IEnumerable<Field> ParseDictionaryStringObject(IDictionary<string, object> obj)
        {
            if (obj != null)
            {
                foreach (var kvp in obj)
                {
                    yield return new Field(kvp.Key, (kvp.Value?.GetType() ?? StaticType.Object));
                }
            }
        }

        /// <summary>
        /// Parses a property from the data entity object based on the given <see cref="Expression"/> and converts the result 
        /// to <see cref="Field"/> object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that contains the property to be parsed.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        /// <returns>An enumerable list of <see cref="Field"/> objects.</returns>
        public static IEnumerable<Field> Parse<TEntity>(Expression<Func<TEntity, object>> expression)
            where TEntity : class =>
            Parse<TEntity, object>(expression);

        /// <summary>
        /// Parses a property from the data entity object based on the given <see cref="Expression"/> and converts the result 
        /// to <see cref="Field"/> object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that contains the property to be parsed.</typeparam>
        /// <typeparam name="TResult">The type of the result and the property to be parsed.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        /// <returns>An enumerable list of <see cref="Field"/> objects.</returns>
        public static IEnumerable<Field> Parse<TEntity, TResult>(Expression<Func<TEntity, TResult>> expression)
            where TEntity : class
        {
            var result = (IEnumerable<Field>)null;

            if (expression.Body.IsUnary())
            {
                result = Parse<TEntity>(expression.Body.ToUnary());
            }
            else if (expression.Body.IsMember())
            {
                result = Parse<TEntity>(expression.Body.ToMember());
            }
            else if (expression.Body.IsBinary())
            {
                result = Parse<TEntity>(expression.Body.ToBinary());
            }
            else if (expression.Body.IsNew())
            {
                result = Parse<TEntity>(expression.Body.ToNew());
            }
            if (result == null)
            {
                throw new InvalidExpressionException($"Expression '{expression}' is invalid.");
            }

            return result;
        }

        /// <summary>
        /// Parses a property from the data entity object based on the given <see cref="UnaryExpression"/> and converts the result 
        /// to <see cref="Field"/> object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that contains the property to be parsed.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        /// <returns>An enumerable list of <see cref="Field"/> objects.</returns>
        internal static IEnumerable<Field> Parse<TEntity>(UnaryExpression expression)
            where TEntity : class
        {
            if (expression.Operand.IsMember())
            {
                return Parse<TEntity>(expression.Operand.ToMember());
            }
            else if (expression.Operand.IsBinary())
            {
                return Parse<TEntity>(expression.Operand.ToBinary());
            }
            return null;
        }

        /// <summary>
        /// Parses a property from the data entity object based on the given <see cref="MemberExpression"/> and converts the result 
        /// to <see cref="Field"/> object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that contains the property to be parsed.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        /// <returns>An enumerable list of <see cref="Field"/> objects.</returns>
        internal static IEnumerable<Field> Parse<TEntity>(MemberExpression expression)
            where TEntity : class
        {
            if (expression.Member is PropertyInfo)
            {
                return expression.Member.ToPropertyInfo().AsField().AsEnumerable();
            }
            else
            {
                return (new Field(expression.Member.Name)).AsEnumerable();
            }
        }

        /// <summary>
        /// Parses a property from the data entity object based on the given <see cref="BinaryExpression"/> and converts the result 
        /// to <see cref="Field"/> object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that contains the property to be parsed.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        /// <returns>An enumerable list of <see cref="Field"/> objects.</returns>
        internal static IEnumerable<Field> Parse<TEntity>(BinaryExpression expression)
            where TEntity : class =>
            (new Field(expression.GetName())).AsEnumerable();

        /// <summary>
        /// Parses a property from the data entity object based on the given <see cref="NewExpression"/> and converts the result 
        /// to <see cref="Field"/> object.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity that contains the property to be parsed.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        /// <returns>An enumerable list of <see cref="Field"/> objects.</returns>
        internal static IEnumerable<Field> Parse<TEntity>(NewExpression expression)
            where TEntity : class
        {
            if (expression.Members?.Count >= 0)
            {
                var properties = expression
                    .Members
                    .WithType<PropertyInfo>();
                var classProperties = PropertyCache.Get<TEntity>()?
                    .Where(classProperty =>
                        properties?.FirstOrDefault(property => string.Equals(property.Name, classProperty.PropertyInfo.Name, StringComparison.OrdinalIgnoreCase)) != null)
                    .Select(classProperty => classProperty.PropertyInfo);
                return (classProperties ?? properties).Select(property => property.AsField());
            }
            return null;
        }

        #endregion

        #region Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <see cref="Field"/>.
        /// </summary>
        /// <returns>The hashcode value.</returns>
        public override int GetHashCode()
        {
            if (this.hashCode != null)
            {
                return this.hashCode.Value;
            }

            var hashCode = 0;

            // Set the hash code
            hashCode = Name.GetHashCode();
            if (Type != null)
            {
                hashCode += Type.GetHashCode();
            }

            // Set and return the hashcode
            return (this.hashCode = hashCode).Value;
        }

        /// <summary>
        /// Compares the <see cref="Field"/> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equals.</returns>
        public override bool Equals(object obj) =>
            obj?.GetHashCode() == GetHashCode();

        /// <summary>
        /// Compares the <see cref="Field"/> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(Field other) =>
            other?.GetHashCode() == GetHashCode();

        /// <summary>
        /// Compares the equality of the two <see cref="Field"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="Field"/> object.</param>
        /// <param name="objB">The second <see cref="Field"/> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(Field objA,
            Field objB)
        {
            if (ReferenceEquals(null, objA))
            {
                return ReferenceEquals(null, objB);
            }
            return objB?.GetHashCode() == objA.GetHashCode();
        }

        /// <summary>
        /// Compares the inequality of the two <see cref="Field"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="Field"/> object.</param>
        /// <param name="objB">The second <see cref="Field"/> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(Field objA,
            Field objB) =>
            (objA == objB) == false;

        #endregion
    }
}
