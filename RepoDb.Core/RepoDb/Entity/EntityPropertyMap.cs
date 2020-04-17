using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb.Entity
{
    internal class EntityPropertyMap<TEntity> : IPropertyMap<TEntity> where TEntity : class
    {
        private readonly IDictionary<string, IConverter> m_propertyMap = new ConcurrentDictionary<string, IConverter>();

        /// <summary>
        /// Find the sql property from the property info map.
        /// </summary>
        /// <param name="prop">property info</param>
        /// <returns></returns>
        public IConverter Find(PropertyInfo prop)
        {
            return m_propertyMap.TryGetValue(prop.Name, out var value) ? value : default;
        }

        /// <summary>
        /// Map property info.
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public IConverter Map(PropertyInfo propertyInfo)
        {
            var mapper = m_propertyMap.GetOrAdd(propertyInfo.Name, new EntityPropertyConverter());
            return mapper;
        }

        /// <summary>
        /// Map property by expression.
        /// </summary>
        /// <typeparam name="TProperty">type of the property</typeparam>
        /// <param name="expression">mapping expression</param>
        /// <returns>mapper</returns>
        public IConverter Map<TProperty>(Expression<Func<TEntity, TProperty>> expression)
        {
            var info = (PropertyInfo) GetMemberInfo(expression);
            return Map(info);
        }

        /// <summary>
        /// Returns the <see cref="T:System.Reflection.MemberInfo"/> for the specified lambda expression.
        /// </summary>
        /// <param name="lambda">A lambda expression containing a MemberExpression.</param>
        /// <returns>A MemberInfo object for the member in the specified lambda expression.</returns>
        private static MemberInfo GetMemberInfo(LambdaExpression lambda)
        {
            Expression expr = lambda;
            while (true)
            {
                switch (expr.NodeType)
                {
                    case ExpressionType.Lambda:
                        expr = ((LambdaExpression) expr).Body;
                        break;

                    case ExpressionType.Convert:
                        expr = ((UnaryExpression) expr).Operand;
                        break;

                    case ExpressionType.MemberAccess:
                        var memberExpression = (MemberExpression) expr;
                        var baseMember = memberExpression.Member;

                        while (memberExpression != null)
                        {
                            var type = memberExpression.Type;
                            if (type.GetMembers().Any(member => member.Name == baseMember.Name))
                            {
                                return type.GetMember(baseMember.Name).First();
                            }

                            memberExpression = memberExpression.Expression as MemberExpression;
                        }

                        // Make sure we get the property from the derived type.
                        var paramType = lambda.Parameters[0].Type;
                        return paramType.GetMember(baseMember.Name).First();

                    default:
                        return null;
                }
            }
        }
    }
}