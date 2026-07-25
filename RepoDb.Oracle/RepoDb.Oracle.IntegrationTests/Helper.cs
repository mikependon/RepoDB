using RepoDb.Oracle.IntegrationTests.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace RepoDb.Oracle.IntegrationTests
{
    public static class Helper
    {
        private static readonly Random m_random = new();

        public static IEnumerable<CompleteTable> CreateCompleteTables(int count)
        {
            for (var i = 0; i < count; i++)
            {
                yield return new CompleteTable
                {
                    SessionId = Guid.NewGuid(),
                    ColumnVarchar = $"Value-{m_random.Next(int.MaxValue)}",
                    ColumnNumber = Math.Round(Convert.ToDecimal(m_random.NextDouble() * 1000), 12),
                    ColumnDate = DateTime.UtcNow.Date,
                    ColumnTimestamp = DateTime.UtcNow
                };
            }
        }

        public static void AssertPropertiesEquality<T>(T t1,
            T t2)
        {
            if (ReferenceEquals(t1, t2))
            {
                return;
            }
            if (t1 == null || t2 == null)
            {
                throw new InvalidOperationException("One of the objects is null.");
            }
            foreach (var property in typeof(T).GetProperties())
            {
                var value1 = property.GetValue(t1);
                var value2 = property.GetValue(t2);
                if (value1 is DateTime d1 && value2 is DateTime d2)
                {
                    // Oracle DATE/TIMESTAMP (without "WITH TIME ZONE") store no timezone info, so
                    // ODP.NET always reads them back as Kind=Unspecified. Normalize both sides to
                    // Unspecified before formatting, so a Utc-vs-Unspecified Kind mismatch doesn't
                    // surface as a false "O"-format ('Z' suffix) inequality.
                    // User of the library should handle their own type in their Oracle database.
                    (value1, value2) = (
                        DateTime.SpecifyKind(d1, DateTimeKind.Unspecified).ToString("O", CultureInfo.InvariantCulture),
                        DateTime.SpecifyKind(d2, DateTimeKind.Unspecified).ToString("O", CultureInfo.InvariantCulture));
                }
                Assert(Equals(value1, value2), property.Name, value1, value2);
            }
        }

        private static void Assert(bool condition,
            string propertyName,
            object value1,
            object value2)
        {
            if (!condition)
            {
                throw new InvalidOperationException($"The property '{propertyName}' values '{value1}' and '{value2}' are not equal.");
            }
        }
    }
}
