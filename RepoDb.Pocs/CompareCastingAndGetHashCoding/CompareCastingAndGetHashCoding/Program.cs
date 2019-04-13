using System;
using System.Diagnostics;

namespace CompareCastingAndGetHashCoding
{
    public enum Operation
    {
        Equal = 1,
        NotEqual = 2
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine((3 ^ 5));
            TestGetHashCode();
            TestCast();
            Console.ReadLine();
        }

        private static void TestGetHashCode()
        {
            var stopwatch = new Stopwatch();
            var value = 0;
            stopwatch.Start();
            for (var i = 0; i < 2000000; i++)
            {
                value = Operation.Equal.GetHashCode() ^ Operation.NotEqual.GetHashCode();
            }
            stopwatch.Stop();
            Console.WriteLine($"GetHashCode: {stopwatch.Elapsed.TotalSeconds} second(s)");
        }

        private static void TestCast()
        {
            var stopwatch = new Stopwatch();
            var value = 0;
            stopwatch.Start();
            for (var i = 0; i < 2000000; i++)
            {
                value = (int)Operation.Equal ^ (int)Operation.NotEqual;
            }
            stopwatch.Stop();
            Console.WriteLine($"Cast: {stopwatch.Elapsed.TotalSeconds} second(s)");
        }
    }
}
