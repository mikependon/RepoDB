using System;
using System.Diagnostics;

namespace CompareGuidPerformance
{
    class Program
    {
        private const string STATIC_GUID = "D84AB1C6-44EA-4350-AF17-E7351B4AA4F5";

        static void Main(string[] args)
        {
            TestGuidParse();
            TestNewGuid();
            Console.ReadLine();
        }

        private static void TestGuidParse()
        {
            var stopwatch = new Stopwatch();
            var count = 5000000;
            stopwatch.Start();
            for (var i = 0; i < count; i++)
            {
                var key = Guid.Parse(STATIC_GUID);
            }
            stopwatch.Stop();
            Console.WriteLine($"Guid.Parse(): {stopwatch.Elapsed.TotalMilliseconds}(s) milliseconds lapsed for {count} iterations.");
        }

        private static void TestNewGuid()
        {
            var stopwatch = new Stopwatch();
            var count = 5000000;
            stopwatch.Start();
            for (var i = 0; i < count; i++)
            {
                var key = new Guid(STATIC_GUID);
            }
            stopwatch.Stop();
            Console.WriteLine($"new Guid(): {stopwatch.Elapsed.TotalMilliseconds}(s) milliseconds lapsed for {count} iterations.");
        }
    }
}
