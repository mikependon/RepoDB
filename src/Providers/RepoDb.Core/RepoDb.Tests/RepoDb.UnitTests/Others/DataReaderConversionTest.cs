#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Data;
using System.Linq.Expressions;
using System.Linq;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Reflection;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Others
{
    [TestClass]
    [DoNotParallelize]
    public class DataReaderConversionTest : RepositoryOperationTestBase
    {
        [TestMethod]
        public void TestStringToIntMaterializesProperty()
        {
            // Setup
            using var table = new DataTable();
            table.Columns.Add("Value", typeof(string));
            table.Rows.Add("42");
            using var reader = table.CreateDataReader();

            // Act
            var actual = DataReader.ToEnumerable<ValueEntity<int>>(reader).Single();

            // Assert
            Assert.AreEqual(42, actual.Value);
        }

        [TestMethod]
        public void TestStringToNullableIntMaterializesProperty()
        {
            // Setup
            using var table = new DataTable();
            table.Columns.Add("Value", typeof(string));
            table.Rows.Add("42");
            using var reader = table.CreateDataReader();

            // Act
            var actual = DataReader.ToEnumerable<ValueEntity<int?>>(reader).Single();

            // Assert
            Assert.AreEqual(42, actual.Value);
        }

        [TestMethod]
        public void TestNullToNullableIntMaterializesProperty()
        {
            // Setup
            using var table = new DataTable();
            table.Columns.Add("Value", typeof(int));
            table.Rows.Add(DBNull.Value);
            using var reader = table.CreateDataReader();

            // Act
            var actual = DataReader.ToEnumerable<ValueEntity<int?>>(reader).Single();

            // Assert
            Assert.IsNull(actual.Value);
        }

        [TestMethod]
        public void TestNullToStringMaterializesProperty()
        {
            // Setup
            using var table = new DataTable();
            table.Columns.Add("Value", typeof(string));
            table.Rows.Add(DBNull.Value);
            using var reader = table.CreateDataReader();

            // Act
            var actual = DataReader.ToEnumerable<ValueEntity<string>>(reader).Single();

            // Assert
            Assert.IsNull(actual.Value);
        }

        [TestMethod]
        public void TestIntToLongMaterializesProperty()
        {
            // Setup
            using var table = new DataTable();
            table.Columns.Add("Value", typeof(int));
            table.Rows.Add(42);
            using var reader = table.CreateDataReader();

            // Act
            var actual = DataReader.ToEnumerable<ValueEntity<long>>(reader).Single();

            // Assert
            Assert.AreEqual(42L, actual.Value);
        }

        [TestMethod]
        public void TestIntToDecimalMaterializesProperty()
        {
            // Setup
            using var table = new DataTable();
            table.Columns.Add("Value", typeof(int));
            table.Rows.Add(42);
            using var reader = table.CreateDataReader();

            // Act
            var actual = DataReader.ToEnumerable<ValueEntity<decimal>>(reader).Single();

            // Assert
            Assert.AreEqual(42M, actual.Value);
        }

        [TestMethod]
        public void TestLongToIntMaterializesProperty()
        {
            // Setup
            using var table = new DataTable();
            table.Columns.Add("Value", typeof(long));
            table.Rows.Add(42L);
            using var reader = table.CreateDataReader();

            // Act
            var actual = DataReader.ToEnumerable<ValueEntity<int>>(reader).Single();

            // Assert
            Assert.AreEqual(42, actual.Value);
        }

        [TestMethod]
        public void TestDecimalToDoubleMaterializesProperty()
        {
            // Setup
            using var table = new DataTable();
            table.Columns.Add("Value", typeof(decimal));
            table.Rows.Add(42M);
            using var reader = table.CreateDataReader();

            // Act
            var actual = DataReader.ToEnumerable<ValueEntity<double>>(reader).Single();

            // Assert
            Assert.AreEqual(42D, actual.Value);
        }

        [TestMethod]
        public void TestStringToGuidMaterializesProperty()
        {
            // Setup
            using var table = new DataTable();
            table.Columns.Add("Value", typeof(string));
            table.Rows.Add("6f9619ff-8b86-d011-b42d-00c04fc964ff");
            using var reader = table.CreateDataReader();

            // Act
            var actual = DataReader.ToEnumerable<ValueEntity<Guid>>(reader).Single();

            // Assert
            Assert.AreEqual(new Guid("6f9619ff-8b86-d011-b42d-00c04fc964ff"), actual.Value);
        }

        [TestMethod]
        public void TestGuidToStringMaterializesProperty()
        {
            // Setup
            using var table = new DataTable();
            table.Columns.Add("Value", typeof(Guid));
            table.Rows.Add(new Guid("6f9619ff-8b86-d011-b42d-00c04fc964ff"));
            using var reader = table.CreateDataReader();

            // Act
            var actual = DataReader.ToEnumerable<ValueEntity<string>>(reader).Single();

            // Assert
            Assert.AreEqual("6f9619ff-8b86-d011-b42d-00c04fc964ff", actual.Value, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDateTimeToDateOnlyMaterializesProperty()
        {
            // Setup
            using var table = new DataTable();
            table.Columns.Add("Value", typeof(DateTime));
            table.Rows.Add(new DateTime(2026, 1, 2, 3, 4, 5));
            using var reader = table.CreateDataReader();

            // Act
            var actual = DataReader.ToEnumerable<ValueEntity<DateOnly>>(reader).Single();

            // Assert
            Assert.AreEqual(new DateOnly(2026, 1, 2), actual.Value);
        }

        [TestMethod]
        public void TestDateTimeToNullableDateOnlyMaterializesProperty()
        {
            // Setup
            using var table = new DataTable();
            table.Columns.Add("Value", typeof(DateTime));
            table.Rows.Add(new DateTime(2026, 1, 2));
            using var reader = table.CreateDataReader();

            // Act
            var actual = DataReader.ToEnumerable<ValueEntity<DateOnly?>>(reader).Single();

            // Assert
            Assert.AreEqual(new DateOnly(2026, 1, 2), actual.Value);
        }

        [TestMethod]
        public void TestDateOnlyToDateTimeCompilesConversion()
        {
            // Setup
            var input = Expression.Constant(new DateOnly(2026, 1, 2), typeof(DateOnly));
            var conversion = Compiler.ConvertExpressionToDateOnlyToDateTimeExpression(input);
            var convert = Expression.Lambda<Func<DateTime>>(conversion).Compile();

            // Act
            var actual = convert();

            // Assert
            Assert.AreEqual(new DateTime(2026, 1, 2), actual);
        }

        [TestMethod]
        public void TestDateOnlyToNullableDateTimeCompilesConversion()
        {
            // Setup
            var input = Expression.Constant((DateOnly?)new DateOnly(2026, 1, 2), typeof(DateOnly?));
            var conversion = Compiler.ConvertExpressionToDateOnlyToDateTimeExpression(input);
            var convert = Expression.Lambda<Func<DateTime?>>(conversion).Compile();

            // Act
            var actual = convert();

            // Assert
            Assert.AreEqual(new DateTime(2026, 1, 2), actual);
        }

        [TestMethod]
        public void TestTimeSpanToDateTimeCompilesConversion()
        {
            // Setup
            var input = Expression.Constant(TimeSpan.FromHours(3), typeof(TimeSpan));
            var conversion = Compiler.ConvertExpressionToTimeSpanToDateTimeExpression(input);
            var convert = Expression.Lambda<Func<DateTime>>(conversion).Compile();

            // Act
            var actual = convert();

            // Assert
            Assert.AreEqual(new DateTime(TimeSpan.FromHours(3).Ticks), actual);
        }

        [TestMethod]
        public void TestDateTimeToTimeSpanMaterializesProperty()
        {
            // Setup
            using var table = new DataTable();
            table.Columns.Add("Value", typeof(DateTime));
            table.Rows.Add(new DateTime(2026, 1, 2, 3, 0, 0));
            using var reader = table.CreateDataReader();

            // Act
            var actual = DataReader.ToEnumerable<ValueEntity<TimeSpan>>(reader).Single();

            // Assert
            Assert.AreEqual(TimeSpan.FromHours(3), actual.Value);
        }

        [TestMethod]
        public void TestStringToEnumMaterializesProperty()
        {
            // Setup
            using var table = new DataTable();
            table.Columns.Add("Value", typeof(string));
            table.Rows.Add("Monday");
            using var reader = table.CreateDataReader();

            // Act
            var actual = DataReader.ToEnumerable<ValueEntity<DayOfWeek>>(reader).Single();

            // Assert
            Assert.AreEqual(DayOfWeek.Monday, actual.Value);
        }

        [TestMethod]
        public void TestIntToEnumMaterializesProperty()
        {
            // Setup
            using var table = new DataTable();
            table.Columns.Add("Value", typeof(int));
            table.Rows.Add(1);
            using var reader = table.CreateDataReader();

            // Act
            var actual = DataReader.ToEnumerable<ValueEntity<DayOfWeek>>(reader).Single();

            // Assert
            Assert.AreEqual(DayOfWeek.Monday, actual.Value);
        }

        [TestMethod]
        public void TestStringToNullableEnumMaterializesProperty()
        {
            // Setup
            using var table = new DataTable();
            table.Columns.Add("Value", typeof(string));
            table.Rows.Add("Monday");
            using var reader = table.CreateDataReader();

            // Act
            var actual = DataReader.ToEnumerable<ValueEntity<DayOfWeek?>>(reader).Single();

            // Assert
            Assert.AreEqual(DayOfWeek.Monday, actual.Value);
        }

        [TestMethod]
        public void TestNullToNullableEnumMaterializesProperty()
        {
            // Setup
            using var table = new DataTable();
            table.Columns.Add("Value", typeof(int));
            table.Rows.Add(DBNull.Value);
            using var reader = table.CreateDataReader();

            // Act
            var actual = DataReader.ToEnumerable<ValueEntity<DayOfWeek?>>(reader).Single();

            // Assert
            Assert.IsNull(actual.Value);
        }

        [TestMethod]
        public void TestEnumUnderlyingIntToStringMaterializesProperty()
        {
            // Setup
            using var table = new DataTable();
            table.Columns.Add("Value", typeof(int));
            table.Rows.Add(1);
            using var reader = table.CreateDataReader();

            // Act
            var actual = DataReader.ToEnumerable<ValueEntity<string>>(reader).Single();

            // Assert
            Assert.AreEqual("1", actual.Value, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestEnumToIntMaterializesProperty()
        {
            // Setup
            using var table = new DataTable();
            table.Columns.Add("Value", typeof(DayOfWeek));
            table.Rows.Add(DayOfWeek.Monday);
            using var reader = table.CreateDataReader();

            // Act
            var actual = DataReader.ToEnumerable<ValueEntity<int>>(reader).Single();

            // Assert
            Assert.AreEqual(1, actual.Value);
        }

        [TestMethod]
        public void TestStringToBoolMaterializesProperty()
        {
            // Setup
            using var table = new DataTable();
            table.Columns.Add("Value", typeof(string));
            table.Rows.Add("True");
            using var reader = table.CreateDataReader();

            // Act
            var actual = DataReader.ToEnumerable<ValueEntity<bool>>(reader).Single();

            // Assert
            Assert.AreEqual(true, actual.Value);
        }

        [TestMethod]
        public void TestIntToStringMaterializesProperty()
        {
            // Setup
            using var table = new DataTable();
            table.Columns.Add("Value", typeof(int));
            table.Rows.Add(42);
            using var reader = table.CreateDataReader();

            // Act
            var actual = DataReader.ToEnumerable<ValueEntity<string>>(reader).Single();

            // Assert
            Assert.AreEqual("42", actual.Value, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestInvalidStringRejectsInt()
        {
            // Setup
            using var table = new DataTable();
            table.Columns.Add("Value", typeof(string));
            table.Rows.Add("invalid");
            using var reader = table.CreateDataReader();

            // Act
            Action act = () => DataReader.ToEnumerable<ValueEntity<int>>(reader).ToList();

            // Assert
            Assert.ThrowsExactly<FormatException>(act);
        }

        [TestMethod]
        public void TestInvalidStringRejectsGuid()
        {
            // Setup
            using var table = new DataTable();
            table.Columns.Add("Value", typeof(string));
            table.Rows.Add("invalid");
            using var reader = table.CreateDataReader();

            // Act
            Action act = () => DataReader.ToEnumerable<ValueEntity<Guid>>(reader).ToList();

            // Assert
            Assert.ThrowsExactly<FormatException>(act);
        }

        [TestMethod]
        public void TestImmutableConstructorMaterialization()
        {
            // Setup
            using var table = new DataTable();
            table.Columns.Add("Value", typeof(int));
            table.Rows.Add(42);
            using var reader = table.CreateDataReader();

            // Act
            var actual = DataReader.ToEnumerable<ImmutableValue>(reader).Single();

            // Assert
            Assert.AreEqual(42, actual.Value);
        }

        public class ValueEntity<T>
        {
            public T Value { get; set; }
        }

        public class ImmutableValue
        {
            public ImmutableValue(int value)
            {
                Value = value;
            }

            public int Value { get; }
        }
    }
}
