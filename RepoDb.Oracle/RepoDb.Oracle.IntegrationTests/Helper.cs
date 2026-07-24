using RepoDb.Oracle.IntegrationTests.Models;
using System;
using System.Collections.Generic;
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
                    ColumnNumber = Convert.ToDecimal(m_random.NextDouble() * 1000),
                    ColumnDate = DateTime.UtcNow.Date,
                    ColumnTimestamp = DateTime.UtcNow
                };
            }
        }

        public static void AssertPropertiesEquality(CompleteTable t1, CompleteTable t2)
        {
            if (t1 == t2)
            {
                return;
            }
            if (t1 == null || t2 == null)
            {
                throw new InvalidOperationException("One of the objects is null.");
            }
            Assert(t1.SessionId == t2.SessionId, nameof(CompleteTable.SessionId));
            Assert(t1.ColumnVarchar == t2.ColumnVarchar, nameof(CompleteTable.ColumnVarchar));
            Assert(t1.ColumnNumber == t2.ColumnNumber, nameof(CompleteTable.ColumnNumber));
        }

        private static void Assert(bool condition, string propertyName)
        {
            if (!condition)
            {
                throw new InvalidOperationException($"The property '{propertyName}' is not equal.");
            }
        }
    }
}
