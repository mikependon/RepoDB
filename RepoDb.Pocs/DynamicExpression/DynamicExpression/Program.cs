using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DynamicExpression
{
    class Program
    {
        static void Main(string[] args)
        {
            var rows = 200000;
            Console.WriteLine($"Elapse Normal: {ExtractNormal(rows).TotalMilliseconds} millisecond(s).");
            Console.WriteLine($"Elapse Expression: {ExtractExpression(rows).TotalMilliseconds} millisecond(s).");
            Console.ReadLine();
        }

        private static TimeSpan ExtractNormal(int rows)
        {
            var watch = new Stopwatch();
            var obj = new
            {
                Id = 1,
                FirstName = "Michael",
                LastName = "Pendon",
                Description = "The quick brown fox jumps over the lazy dog.",
                CreateDate = DateTime.UtcNow,
                Updated = DateTime.UtcNow,
                LastUserId = Environment.UserName
            };
            for (var i = 0; i < rows; i++)
            {
                watch.Start();
                var result = new List<object>();
                obj?.GetType().GetProperties().ToList().ForEach(property =>
                {
                    result.Add(property.GetValue(obj));
                });
                watch.Stop();
            }
            return watch.Elapsed;
        }

        private static TimeSpan ExtractExpression(int rows)
        {
            var watch = new Stopwatch();
            var obj = new
            {
                Id = 1,
                FirstName = "Michael",
                LastName = "Pendon",
                Description = "The quick brown fox jumps over the lazy dog.",
                CreateDate = DateTime.UtcNow,
                Updated = DateTime.UtcNow,
                LastUserId = Environment.UserName
            };
            for (var i = 0; i < rows; i++)
            {
                watch.Start();
                var value = DynamicExpression.Extract(obj);
                watch.Stop();
            }
            return watch.Elapsed;
        }
    }
}
