using System;
using System.Linq.Expressions;

namespace DynamicsComparer
{
    class Program
    {
        static void Main(string[] args)
        {
            IsEqual(new { Id = 1 }, new { Id = 1 });
            IsEqual(new { Id = 1, name = "B" }, new { Id = 2, Name = "A" });
            IsEqual(new { Name = 1 }, new { Id = 1 });
            IsEqual(new { Name = "A" }, new { Name = "A" });
            IsEqual(new { Name = "A" }, new { Name = "B" });
            Console.ReadLine();
        }

        static void IsEqual(object objA, object objB)
        {
            var isEqual = DynamicComparer.ArePropertiesEqual(objA, objB);
            Console.WriteLine($"IsEquals: {isEqual}");
        }
    }
}
