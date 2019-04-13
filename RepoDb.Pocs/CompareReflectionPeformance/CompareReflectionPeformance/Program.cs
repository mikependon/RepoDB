using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;

namespace CompareReflectionPeformance
{
    class Program
    {
        static void Main(string[] args)
        {
            TestStringDollarFormatting();
            TestStringConcatenation();
            Console.ReadLine();
        }

        private static void TestStringDollarFormatting()
        {
            var value1 = "Value1";
            var value2 = "Value2";
            var stopwatch = new Stopwatch();
            var count = 0;
            stopwatch.Start();
            for (var i = 0; i < 1000000; i++)
            {
                var a = $"{value1}:{value2}";
            }
            stopwatch.Stop();
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {stopwatch.Elapsed.TotalSeconds}(s) lapsed for {count} iterations.");
        }

        private static void TestStringConcatenation()
        {
            var value1 = "Value1";
            var value2 = "Value2";
            var stopwatch = new Stopwatch();
            var count = 0;
            stopwatch.Start();
            for (var i = 0; i < 1000000; i++)
            {
                var a = string.Concat(value1, ":", value2);
            }
            stopwatch.Stop();
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {stopwatch.Elapsed.TotalSeconds}(s) lapsed for {count} iterations.");
        }

        private static void TestIsComparisson()
        {
            var stopwatch = new Stopwatch();
            var count = 0;
            using (var connection = new SqlConnection("Server=.;Database=Northwind;Integrated Security=True;"))
            {
                stopwatch.Start();
                for (var i = 0; i < 1000000; i++)
                {
                    if (connection is SqlConnection)
                    {
                        count++;
                    }
                }
            }
            stopwatch.Stop();
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {stopwatch.Elapsed.TotalSeconds}(s) lapsed for {count} iterations.");
        }

        private static void TestTypeOfComparisson()
        {
            var stopwatch = new Stopwatch();
            var count = 0;
            using (var connection = new SqlConnection("Server=.;Database=Northwind;Integrated Security=True;"))
            {
                stopwatch.Start();
                for (var i = 0; i < 1000000; i++)
                {
                    if (connection?.GetType() == typeof(SqlConnection))
                    {
                        count++;
                    }
                }
            }
            stopwatch.Stop();
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {stopwatch.Elapsed.TotalSeconds}(s) lapsed for {count} iterations.");
        }

        public enum Provider
        {
            Sql = 1
        }

        private static void TestEnumConcatComparisson(Provider provider)
        {
            var stopwatch = new Stopwatch();
            var count = 0;
            using (var connection = new SqlConnection("Server=.;Database=Northwind;Integrated Security=True;"))
            {
                stopwatch.Start();
                for (var i = 0; i < 1000000; i++)
                {
                    if ("SqlConnection".Equals(connection?.GetType().Name))
                    {
                        count++;
                    }
                }
            }
            stopwatch.Stop();
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {stopwatch.Elapsed.TotalSeconds}(s) lapsed for {count} iterations.");
        }
    }
}
