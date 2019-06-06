using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Enumerations;
using RepoDb.IntegrationTests.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
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
            fromType.GetProperties().AsList().ForEach(property =>
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
        /// Asserts the properties equality of 2 types.
        /// </summary>
        /// <typeparam name="T1">The type of first object.</typeparam>
        /// <typeparam name="T2">The type of second object.</typeparam>
        /// <param name="t1">The instance of first object.</param>
        /// <param name="t2">The instance of second object.</param>
        public static void AssertPropertiesEquality<T1, T2>(T1 t1, T2 t2)
        {
            var propertiesOfType1 = typeof(T1).GetProperties();
            var propertiesOfType2 = typeof(T2).GetProperties();
            propertiesOfType1.AsList().ForEach(propertyOfType1 =>
            {
                if (propertyOfType1.Name == "Id")
                {
                    return;
                }
                var propertyOfType2 = propertiesOfType2.FirstOrDefault(p => p.Name == propertyOfType1.Name);
                if (propertyOfType2 == null)
                {
                    return;
                }
                var value1 = propertyOfType1.GetValue(t1);
                var value2 = propertyOfType2.GetValue(t2);
                Assert.AreEqual(value1, value2,
                    $"Assert failed for '{propertyOfType1.Name}'. The values are '{value1}' and '{value2}'.");
            });
        }

        /// <summary>
        /// Asserts the members equality of 2 object and <see cref="ExpandoObject"/>.
        /// </summary>
        /// <typeparam name="T">The type of first object.</typeparam>
        /// <param name="obj">The instance of first object.</param>
        /// <param name="expandoObj">The instance of second object.</param>
        public static void AssertMembersEquality(object obj, ExpandoObject expandoObj)
        {
            var properties = obj.GetType().GetProperties();
            var dictionary = expandoObj as IDictionary<string, object>;
            properties.AsList().ForEach(property =>
            {
                if (property.Name == "Id")
                {
                    return;
                }
                if (dictionary.ContainsKey(property.Name))
                {
                    var value1 = property.GetValue(obj);
                    var value2 = dictionary[property.Name];
                    Assert.AreEqual(
                        Convert.ChangeType(value1, property.PropertyType.GetUnderlyingType()),
                        Convert.ChangeType(value2, property.PropertyType.GetUnderlyingType()),
                        $"Assert failed for '{property.Name}'. The values are '{value1}' and '{value2}'.");
                }
            });

        }

        #region IdentityTable

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

        #endregion

        #region NonIdentityTable

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

        #endregion

        #region WithExtraFieldsIdentityTable

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
                    ColumnNVarChar = $"NVARCHAR{index}",
                    ExtraField = $"ExtraField{index}",
                    IdentityTables = new[]
                    {
                        CreateIdentityTable(),
                        CreateIdentityTable()
                    }
                });
            }
            return tables;
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

        #endregion

        #region IdentityTableWithDifferentPrimary

        /// <summary>
        /// Creates a list of <see cref="IdentityTableWithDifferentPrimary"/> objects.
        /// </summary>
        /// <param name="count">The number of rows.</param>
        /// <returns>A list of <see cref="IdentityTableWithDifferentPrimary"/> objects.</returns>
        public static List<IdentityTableWithDifferentPrimary> CreateIdentityTableWithDifferentPrimaries(int count)
        {
            var tables = new List<IdentityTableWithDifferentPrimary>();
            for (var i = 0; i < count; i++)
            {
                var index = i + 1;
                tables.Add(new IdentityTableWithDifferentPrimary
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
        /// Creates an instance of <see cref="IdentityTableWithDifferentPrimary"/> object.
        /// </summary>
        /// <returns>A new created instance of <see cref="IdentityTableWithDifferentPrimary"/> object.</returns>
        public static IdentityTableWithDifferentPrimary CreateIdentityTableWithDifferentPrimary()
        {
            var random = new Random();
            return new IdentityTableWithDifferentPrimary
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

        #endregion

        #region EnumCompleteTable

        /// <summary>
        /// Creates a list of <see cref="EnumCompleteTable"/> objects.
        /// </summary>
        /// <param name="count">The number of rows.</param>
        /// <returns>A list of <see cref="EnumCompleteTable"/> objects.</returns>
        public static List<EnumCompleteTable> CreateEnumCompleteTables(int count)
        {
            var tables = new List<EnumCompleteTable>();
            for (var i = 0; i < count; i++)
            {
                var index = i + 1;
                tables.Add(new EnumCompleteTable
                {
                    SessionId = Guid.NewGuid(),
                    ColumnBit = BooleanValue.True,
                    ColumnNVarChar = Direction.West,
                    ColumnInt = Direction.West,
                    ColumnBigInt = Direction.West,
                    ColumnSmallInt = Direction.West
                });
            }
            return tables;
        }

        /// <summary>
        /// Creates an instance of <see cref="EnumCompleteTable"/> object.
        /// </summary>
        /// <returns>A new created instance of <see cref="EnumCompleteTable"/> object.</returns>
        public static EnumCompleteTable CreateEnumCompleteTable()
        {
            return new EnumCompleteTable
            {
                SessionId = Guid.NewGuid(),
                ColumnBit = BooleanValue.True,
                ColumnNVarChar = Direction.West,
                ColumnInt = Direction.West,
                ColumnBigInt = Direction.West,
                ColumnSmallInt = Direction.West
            };
        }

        #endregion

        #region EnumAsIntForStringCompleteTable

        /// <summary>
        /// Creates a list of <see cref="EnumAsIntForStringCompleteTable"/> objects.
        /// </summary>
        /// <param name="count">The number of rows.</param>
        /// <returns>A list of <see cref="EnumAsIntForStringCompleteTable"/> objects.</returns>
        public static List<EnumAsIntForStringCompleteTable> CreateEnumAsIntForStringCompleteTables(int count)
        {
            var tables = new List<EnumAsIntForStringCompleteTable>();
            for (var i = 0; i < count; i++)
            {
                var index = i + 1;
                tables.Add(new EnumAsIntForStringCompleteTable
                {
                    SessionId = Guid.NewGuid(),
                    ColumnNVarChar = Direction.West
                });
            }
            return tables;
        }

        /// <summary>
        /// Creates an instance of <see cref="EnumAsIntForStringCompleteTable"/> object.
        /// </summary>
        /// <returns>A new created instance of <see cref="EnumAsIntForStringCompleteTable"/> object.</returns>
        public static EnumAsIntForStringCompleteTable CreateEnumAsIntForStringCompleteTable()
        {
            return new EnumAsIntForStringCompleteTable
            {
                SessionId = Guid.NewGuid(),
                ColumnNVarChar = Direction.West
            };
        }

        #endregion

        #region TypeLevelMappedForStringEnumCompleteTable

        /// <summary>
        /// Creates a list of <see cref="TypeLevelMappedForStringEnumCompleteTable"/> objects.
        /// </summary>
        /// <param name="count">The number of rows.</param>
        /// <returns>A list of <see cref="TypeLevelMappedForStringEnumCompleteTable"/> objects.</returns>
        public static List<TypeLevelMappedForStringEnumCompleteTable> CreateTypeLevelMappedForStringEnumCompleteTables(int count)
        {
            var tables = new List<TypeLevelMappedForStringEnumCompleteTable>();
            for (var i = 0; i < count; i++)
            {
                var index = i + 1;
                tables.Add(new TypeLevelMappedForStringEnumCompleteTable
                {
                    SessionId = Guid.NewGuid(),
                    ColumnNVarChar = Continent.Asia
                });
            }
            return tables;
        }

        /// <summary>
        /// Creates an instance of <see cref="TypeLevelMappedForStringEnumCompleteTable"/> object.
        /// </summary>
        /// <returns>A new created instance of <see cref="TypeLevelMappedForStringEnumCompleteTable"/> object.</returns>
        public static TypeLevelMappedForStringEnumCompleteTable CreateTypeLevelMappedForStringEnumCompleteTable()
        {
            return new TypeLevelMappedForStringEnumCompleteTable
            {
                SessionId = Guid.NewGuid(),
                ColumnNVarChar = Continent.Asia
            };
        }

        #endregion

        #region Dynamics

        #region IdentityTable

        /// <summary>
        /// Creates a an instance of dynamic object for [sc].[IdentityTable].
        /// </summary>
        /// <param name="count">The number of rows.</param>
        /// <returns>A dynamic for [sc].[IdentityTable].</returns>
        public static dynamic CreateDynamicIdentityTable()
        {
            return new
            {
                Id = 1,
                RowGuid = Guid.NewGuid(),
                ColumnBit = true,
                ColumnDateTime = EpocDate.AddDays(1),
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnDecimal = Convert.ToDecimal(1),
                ColumnFloat = Convert.ToSingle(1),
                ColumnInt = 1,
                ColumnNVarChar = $"NVARCHAR{1}"
            };
        }

        /// <summary>
        /// Creates a list of dynamic objects for [sc].[IdentityTable].
        /// </summary>
        /// <param name="count">The number of rows.</param>
        /// <returns>A list of dynamic objects.</returns>
        public static List<dynamic> CreateDynamicIdentityTables(int count)
        {
            var tables = new List<dynamic>();
            for (var i = 0; i < count; i++)
            {
                var index = i + 1;
                tables.Add(new
                {
                    Id = index,
                    RowGuid = Guid.NewGuid(),
                    ColumnBit = true,
                    ColumnDateTime = EpocDate.AddDays(index),
                    ColumnDateTime2 = DateTime.UtcNow,
                    ColumnDecimal = Convert.ToDecimal(index),
                    ColumnFloat = Convert.ToSingle(index),
                    ColumnInt = index,
                    ColumnNVarChar = $"NVARCHAR{index}"
                });
            }
            return tables;
        }

        /// <summary>
        /// Creates a list of dynamic objects for [sc].[IdentityTable] with limited columns.
        /// </summary>
        /// <param name="count">The number of rows.</param>
        /// <returns>A list of dynamic objects.</returns>
        public static Tuple<List<dynamic>, IEnumerable<Field>> CreateDynamicIdentityTablesWithLimitedColumns(int count)
        {
            var tables = new List<dynamic>();
            var fields = Field.From(
                "RowGuid",
                "ColumnBit",
                "ColumnDateTime2",
                "ColumnNVarChar");
            for (var i = 0; i < count; i++)
            {
                var index = i + 1;
                tables.Add(new
                {
                    Id = index,
                    RowGuid = Guid.NewGuid(),
                    ColumnBit = true,
                    ColumnDateTime2 = DateTime.UtcNow,
                    ColumnNVarChar = $"NVARCHAR{index}"
                });
            }
            return new Tuple<List<dynamic>, IEnumerable<Field>>(tables, fields);
        }

        #endregion

        #region NonIdentityTable

        /// <summary>
        /// Creates a an instance of dynamic object for [sc].[NonIdentityTable].
        /// </summary>
        /// <param name="count">The number of rows.</param>
        /// <returns>A dynamic for [sc].[NonIdentityTable].</returns>
        public static dynamic CreateDynamicNonIdentityTable()
        {
            return new
            {
                Id = Guid.NewGuid(),
                ColumnBit = true,
                ColumnDateTime = EpocDate.AddDays(1),
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnDecimal = Convert.ToDecimal(1),
                ColumnFloat = Convert.ToSingle(1),
                ColumnInt = 1,
                ColumnNVarChar = $"NVARCHAR{1}"
            };
        }

        /// <summary>
        /// Creates a list of dynamic objects for [dbo].[NonIdentityTable].
        /// </summary>
        /// <param name="count">The number of rows.</param>
        /// <returns>A list of dynamic objects.</returns>
        public static List<dynamic> CreateDynamicNonIdentityTables(int count)
        {
            var tables = new List<dynamic>();
            for (var i = 0; i < count; i++)
            {
                var index = i + 1;
                tables.Add(new
                {
                    Id = Guid.NewGuid(),
                    ColumnBit = true,
                    ColumnDateTime = EpocDate.AddDays(index),
                    ColumnDateTime2 = DateTime.UtcNow,
                    ColumnDecimal = Convert.ToDecimal(index),
                    ColumnFloat = Convert.ToSingle(index),
                    ColumnInt = index,
                    ColumnNVarChar = $"NVARCHAR{index}"
                });
            }
            return tables;
        }

        /// <summary>
        /// Creates a list of dynamic objects for [dbo].[NonIdentityTable] with limited columns.
        /// </summary>
        /// <param name="count">The number of rows.</param>
        /// <returns>A list of dynamic objects.</returns>
        public static Tuple<List<dynamic>, IEnumerable<Field>> CreateDynamicNonIdentityTablesWithLimitedColumns(int count)
        {
            var tables = new List<dynamic>();
            var fields = Field.From(
                "Id",
                "ColumnBit",
                "ColumnDateTime2",
                "ColumnNVarChar");
            for (var i = 0; i < count; i++)
            {
                var index = i + 1;
                tables.Add(new
                {
                    Id = Guid.NewGuid(),
                    ColumnBit = true,
                    ColumnDateTime2 = DateTime.UtcNow,
                    ColumnNVarChar = $"NVARCHAR{index}"
                });
            }
            return new Tuple<List<dynamic>, IEnumerable<Field>>(tables, fields);
        }

        #endregion

        #endregion
    }
}
