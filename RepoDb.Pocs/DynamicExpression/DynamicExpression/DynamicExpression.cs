using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;

namespace DynamicExpression
{
    public static class DynamicExpression
    {
        public static IEnumerable<object> Extract(object obj)
        {
            return ExtractActual((dynamic)obj);
        }

        private static IEnumerable<object> ExtractActual<T>(T obj)
        {
            return Extractor<T>.Extract(obj);
        }

        private static class Extractor<T>
        {
            private static readonly Func<T, IEnumerable<object>> m_func;

            static Extractor()
            {
                // Set the value
                var type = typeof(T);
                var properties = type.GetProperties();

                // Expressions
                var listType = typeof(List<object>);
                var method = listType.GetMethod("Add", new[] { typeof(object) });
                var newList = Expression.New(listType);
                var param = Expression.Parameter(typeof(T), "obj");

                // Set the body
                var body = Expression.ListInit(
                    newList,
                    properties.Select(property =>
                    {
                        return Expression.ElementInit(method,
                            Expression.Convert(Expression.Property(param, property), typeof(object)));
                    }));

                // Set the function value
                m_func = Expression
                    .Lambda<Func<T, IEnumerable<object>>>(body, param)
                    .Compile();
            }

            public static IEnumerable<object> Extract(T obj)
            {
                return m_func(obj);
            }
        }
    }
}
