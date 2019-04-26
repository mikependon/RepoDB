using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RepoDb.IntegrationTests
{
    /// <summary>
    /// A helper class for the integration testing.
    /// </summary>
    public static class Helper
    {
        static Helper()
        {
            EpocDate = new DateTime(1970, 1, 1, 0, 0, 0);
        }

        /// <summary>
        /// Gets the value of the Epoc date.
        /// </summary>
        public static DateTime EpocDate { get; }

        /// <summary>
        /// Gets the description of the library.
        /// </summary>
        /// <returns>The description of the library.</returns>
        public static string GetAssemblyDescription()
        {
            /* RepoDb: A dynamic, lightweight, and very fast ORM .NET Library. */
            return typeof(TypeMapper)
                .Assembly
                .GetCustomAttributes(typeof(AssemblyDescriptionAttribute))?
                .OfType<AssemblyDescriptionAttribute>()?
                .FirstOrDefault()?
                .Description;
        }

        /// <summary>
        /// Gets a <see cref="Guid"/>-based random string.
        /// </summary>
        /// <returns>A <see cref="Guid"/>-based random string.</returns>
        public static string GetGuidBasedRandomString()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Gets a unicode string.
        /// </summary>
        /// <returns>A unicode string.</returns>
        public static string GetUnicodeString()
        {
            return "ÀÆÇÈËÌÏÐÑÒØÙÜÝÞß";
        }

        /// <summary>
        /// Converts the object into a type.
        /// </summary>
        /// <typeparam name="T">The target type to convert to.</typeparam>
        /// <param name="obj">The object instance.</param>
        /// <param name="strict">True if to be strict on the conversion.</param>
        /// <returns>The instance of the converted object.</returns>
        public static T ConverToType<T>(object obj, bool strict = true)
        {
            var fromType = obj.GetType();
            var toTypeProperties = typeof(T).GetProperties();
            var result = default(T);
            fromType.GetProperties().ToList().ForEach(property =>
            {
                var toProperty = toTypeProperties.FirstOrDefault(p => p.Name == property.Name);
                if (strict)
                {
                    if (toProperty == null)
                    {
                        throw new NullReferenceException(property.Name);
                    }
                }
                if (toProperty == null)
                {
                    return;
                }
                if (result == null)
                {
                    result = Activator.CreateInstance<T>();
                }
                toProperty.SetValue(result, property.GetValue(obj));
            });
            return result;
        }

        /// <summary>
        /// Creates a list of <see cref="IdentityTable"/> objects.
        /// </summary>
        /// <param name="count">The number of rows.</param>
        /// <returns>A list of <see cref="IdentityTable"/> objects.</returns>
        public static List<IdentityTable> CreateIdentityTables(int count)
        {
            var tables = new List<IdentityTable>();
            for (var i = 0; i < count; i++)
            {
                var index = i + 1;
                tables.Add(new IdentityTable
                {
                    RowGuid = Guid.NewGuid(),
                    ColumnBit = true,
                    ColumnDateTime = EpocDate.AddDays(index),
                    ColumnDateTime2 = DateTime.UtcNow,
                    ColumnDecimal = index,
                    ColumnFloat = index,
                    ColumnInt = index,
                    ColumnNVarChar = $"NVARCHAR{index}"
                });
            }
            return tables;
        }

        /// <summary>
        /// Creates a list of <see cref="WithExtraFieldsIdentityTable"/> objects.
        /// </summary>
        /// <param name="count">The number of rows.</param>
        /// <returns>A list of <see cref="IdentityTable"/> objects.</returns>
        public static List<WithExtraFieldsIdentityTable> CreateWithExtraFieldsIdentityTables(int count)
        {
            var tables = new List<WithExtraFieldsIdentityTable>();
            for (var i = 0; i < count; i++)
            {
                var index = i + 1;
                tables.Add(new WithExtraFieldsIdentityTable
                {
                    RowGuid = Guid.NewGuid(),
                    ColumnBit = true,
                    ColumnDateTime = EpocDate.AddDays(index),
                    ColumnDateTime2 = DateTime.UtcNow,
                    ColumnDecimal = index,
                    ColumnFloat = index,
                    ColumnInt = index,
                    ColumnNVarChar = $"NVARCHAR{index}"
                });
            }
            return tables;
        }

        /// <summary>
        /// Creates a list of <see cref="NonIdentityTable"/> objects.
        /// </summary>
        /// <param name="count">The number of rows.</param>
        /// <returns>A list of <see cref="IdentityTable"/> objects.</returns>
        public static List<NonIdentityTable> CreateNonIdentityTables(int count)
        {
            var tables = new List<NonIdentityTable>();
            for (var i = 0; i < count; i++)
            {
                var index = i + 1;
                tables.Add(new NonIdentityTable
                {
                    Id = Guid.NewGuid(),
                    ColumnBit = true,
                    ColumnDateTime = EpocDate.AddDays(index),
                    ColumnDateTime2 = DateTime.UtcNow,
                    ColumnDecimal = index,
                    ColumnFloat = index,
                    ColumnInt = index,
                    ColumnNVarChar = $"NVARCHAR{index}"
                });
            }
            return tables;
        }

        /// <summary>
        /// Creates an instance of <see cref="IdentityTable"/> object.
        /// </summary>
        /// <returns>A new created instance of <see cref="IdentityTable"/> object.</returns>
        public static IdentityTable CreateIdentityTable()
        {
            var random = new Random();
            return new IdentityTable
            {
                RowGuid = Guid.NewGuid(),
                ColumnBit = true,
                ColumnDateTime = EpocDate,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnDecimal = Convert.ToDecimal(random.Next(int.MinValue, int.MaxValue)),
                ColumnFloat = Convert.ToSingle(random.Next(int.MinValue, int.MaxValue)),
                ColumnInt = random.Next(int.MinValue, int.MaxValue),
                ColumnNVarChar = Guid.NewGuid().ToString()
            };
        }

        /// <summary>
        /// Creates an instance of <see cref="NonIdentityTable"/> object.
        /// </summary>
        /// <returns>A new created instance of <see cref="NonIdentityTable"/> object.</returns>
        public static NonIdentityTable CreateNonIdentityTable()
        {
            var random = new Random();
            return new NonIdentityTable
            {
                Id = Guid.NewGuid(),
                ColumnBit = true,
                ColumnDateTime = EpocDate,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnDecimal = Convert.ToDecimal(random.Next(int.MinValue, int.MaxValue)),
                ColumnFloat = Convert.ToSingle(random.Next(int.MinValue, int.MaxValue)),
                ColumnInt = random.Next(int.MinValue, int.MaxValue),
                ColumnNVarChar = Guid.NewGuid().ToString()
            };
        }

        /// <summary>
        /// Creates an instance of <see cref="WithExtraFieldsIdentityTable"/> object.
        /// </summary>
        /// <returns>A new created instance of <see cref="NonIdentityTable"/> object.</returns>
        public static WithExtraFieldsIdentityTable CreateWithExtraFieldsIdentityTable()
        {
            var random = new Random();
            return new WithExtraFieldsIdentityTable
            {
                RowGuid = Guid.NewGuid(),
                ColumnBit = true,
                ColumnDateTime = EpocDate,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnDecimal = Convert.ToDecimal(random.Next(int.MinValue, int.MaxValue)),
                ColumnFloat = Convert.ToSingle(random.Next(int.MinValue, int.MaxValue)),
                ColumnInt = random.Next(int.MinValue, int.MaxValue),
                ColumnNVarChar = Guid.NewGuid().ToString()
            };
        }

        /// <summary>
        /// Asserts the equalify of 2 types.
        /// </summary>
        /// <typeparam name="T1">The type of first object.</typeparam>
        /// <typeparam name="T2">The type of second object.</typeparam>
        /// <param name="t1">The instance of first object.</param>
        /// <param name="t2">The instance of second object.</param>
        public static void AssertPropertiesEquality<T1, T2>(T1 t1, T2 t2)
        {
            var propertiesOfType1 = typeof(T1).GetProperties();
            var propertiesOfType2 = typeof(T2).GetProperties();
            propertiesOfType1.ToList().ForEach(propertyOfType1 =>
            {
                if (propertyOfType1.Name == "Id")
                {
                    return;
                }
                var propertyOfType2 = propertiesOfType2.First(p => p.Name == propertyOfType1.Name);
                var value1 = propertyOfType1.GetValue(t1);
                var value2 = propertyOfType2.GetValue(t2);
                Assert.AreEqual(value1, value2, $"Assert failed for '{propertyOfType1.Name}'. The values are '{value1}' and '{value2}'.");
            });
        }
    }
}
