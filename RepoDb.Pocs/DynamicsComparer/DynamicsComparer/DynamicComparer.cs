using System;
using System.Linq;
using System.Linq.Expressions;

namespace DynamicsComparer
{
    /*
    * Class take at StackOverflow, Marc Gravell Post.
    * https://stackoverflow.com/questions/16517392/is-it-safe-to-use-gethashcode-to-compare-identical-anonymous-types
    */

    /// <summary>
    /// A class used to compare the dynamic objects.
    /// </summary>
    public static partial class DynamicComparer
    {
        /// <summary>
        /// Checks whether the two dynamic object properties are equal.
        /// </summary>
        /// <param name="objA">The first dynamic object.</param>
        /// <param name="objB">The second dynamic object.</param>
        /// <returns>True if the properties are equal.</returns>
        public static bool ArePropertiesEqual(object objA, object objB)
        {
            if (ReferenceEquals(null, objA))
            {
                return ReferenceEquals(null, objB);
            }
            if (ReferenceEquals(objB, null))
            {
                return false;
            }
            return ArePropertiesReallyEqual((dynamic)objA, (dynamic)objB);
        }

        /// <summary>
        /// Checks whether the two dynamic object properties are equal.
        /// </summary>
        /// <typeparam name="TypeA">The type of the first dynamic object.</typeparam>
        /// <typeparam name="TypeB">The type of the second dynamic object.</typeparam>
        /// <param name="objA">The first dynamic object.</param>
        /// <param name="objB">The second dynamic object.</param>
        /// <returns>True if the properties are equal.</returns>
        private static bool ArePropertiesReallyEqual<TypeA, TypeB>(TypeA objA, TypeB objB)
        {
            return DynamicPropertiesComparerCache<TypeA, TypeB>.IsEqual(objA, objB);
        }

        /// <summary>
        /// A cache class for the dynamic object properties comparer.
        /// </summary>
        /// <typeparam name="TypeA">The type of the first dynamic object.</typeparam>
        /// <typeparam name="TypeB">The type of the second dynamic object.</typeparam>
        static class DynamicPropertiesComparerCache<TypeA, TypeB>
        {
            private static readonly Func<TypeA, TypeB, bool> m_func;

            static DynamicPropertiesComparerCache()
            {
                // Parameters
                var objA = Expression.Parameter(typeof(TypeA), "objA");
                var objB = Expression.Parameter(typeof(TypeB), "objB");

                // Get the properties
                var isEqual = false;
                var propertiesOfTypeA = typeof(TypeA)
                    .GetProperties()
                    .Select(p => p.Name);
                var propertiesOfTypeB = typeof(TypeB)
                    .GetProperties()
                    .Select(p => p.Name);

                // Check the count
                isEqual = (propertiesOfTypeA.Count() > 0) &&
                    (propertiesOfTypeA.Count() == propertiesOfTypeB.Count()) &&
                    (true == propertiesOfTypeA?.All(propertyA =>
                        propertiesOfTypeB?.FirstOrDefault(propertyB => propertyB == propertyA) != null));

                // Expression variables
                var body = (Expression)Expression.Constant(isEqual);

                // Set the function value
                m_func = Expression.Lambda<Func<TypeA, TypeB, bool>>(
                    body,
                    objA,
                    objB).Compile();
            }

            /// <summary>
            /// Checks whether the two dynamic objects are equal.
            /// </summary>
            /// <param name="objA">The first dynamic object.</param>
            /// <param name="objB">The second dynamic object.</param>
            /// <returns>True if the properties are equal.</returns>
            public static bool IsEqual(TypeA objA, TypeB objB)
            {
                return m_func(objA, objB);
            }
        }
    }
}
