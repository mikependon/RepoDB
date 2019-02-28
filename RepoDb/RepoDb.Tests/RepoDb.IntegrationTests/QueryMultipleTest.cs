using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace RepoDb.IntegrationTests
{
    [TestClass]
    public class QueryMultipleTest
    {
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            TypeMapper.AddMap(typeof(DateTime), DbType.DateTime2, true);
            SetupHelper.InitDatabase();
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            SetupHelper.CleanDatabase();
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultiple()
        {
            // Setup
            var dog = new Animal
            {
                Id = Guid.NewGuid(),
                Name = "Dog"
            };
            var cat = new Animal
            {
                Id = Guid.NewGuid(),
                Name = "Cat"
            };
            var lion = new Animal
            {
                Id = Guid.NewGuid(),
                Name = "Lion"
            };

            using (var connection = new SqlConnection(Constants.TestDatabase))
            {
                // Act (Insert)
                connection.DeleteAll<Animal>();
                connection.Insert(dog);
                connection.Insert(cat);
                connection.Insert(lion);

                // Act (Query)
                var result = connection.ExecuteQueryMultiple(@"
                    SELECT * FROM [dbo].[Animal] WHERE Id = @Id;
                    SELECT * FROM [dbo].[Animal];
                    SELECT COUNT(*) FROM [dbo].[Animal];", new { dog.Id });

                // Act (Extract)
                var dogs = result.Extract<Animal>();
                result.NextResult();
                var animals = result.Extract<Animal>();
                result.NextResult();
                var count = (int)result.Scalar();

                // Assert (Insert)
                Assert.AreEqual(dogs.First().Id, dog.Id);
                Assert.AreEqual(animals.Count(), 3);
                Assert.AreEqual(count, 3);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleUsingNextMethods()
        {
            // Setup
            var dog = new Animal
            {
                Id = Guid.NewGuid(),
                Name = "Dog"
            };
            var cat = new Animal
            {
                Id = Guid.NewGuid(),
                Name = "Cat"
            };
            var lion = new Animal
            {
                Id = Guid.NewGuid(),
                Name = "Lion"
            };

            using (var connection = new SqlConnection(Constants.TestDatabase))
            {
                // Act (Insert)
                connection.DeleteAll<Animal>();
                connection.Insert(dog);
                connection.Insert(cat);
                connection.Insert(lion);

                // Act (Query)
                var result = connection.ExecuteQueryMultiple(@"
                    SELECT * FROM [dbo].[Animal] WHERE Id = @Id;
                    SELECT * FROM [dbo].[Animal];
                    SELECT COUNT(*) FROM [dbo].[Animal];", new { dog.Id });

                // Act (Extract)
                var dogs = result.Extract<Animal>();
                var animals = result.ExtractNext<Animal>();
                var count = (int)result.ScalarNext();

                // Assert (Insert)
                Assert.AreEqual(dogs.First().Id, dog.Id);
                Assert.AreEqual(animals.Count(), 3);
                Assert.AreEqual(count, 3);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryMultipleUntilT2()
        {
            // Setup
            var animals = new List<Animal>();
            for (var i = 1; i <= 2; i++)
            {
                var animal = new Animal
                {
                    Id = Guid.NewGuid(),
                    Name = $"Animal{i}"
                };
                animals.Add(animal);
            }

            using (var connection = new SqlConnection(Constants.TestDatabase))
            {
                // Act (Insert)
                connection.DeleteAll<Animal>();
                animals.ForEach(animal => connection.Insert(animal));

                // Act (Query)
                var result = connection.QueryMultiple<Animal, Animal>(
                    where1: a => a.Name != null, top1: 1, orderBy1: OrderField.Parse(new { Name = Enumerations.Order.Ascending }),
                    where2: a => a.Name != null, top2: 2, orderBy2: OrderField.Parse(new { Name = Enumerations.Order.Ascending }));

                // Assert (Insert)
                Assert.AreEqual(result.Item1.Count(), 1);
                Assert.AreEqual(result.Item2.Count(), 2);

                // Assert (Values)
                Assert.AreEqual(result.Item1.Last().Name, "Animal1");
                Assert.AreEqual(result.Item2.Last().Name, "Animal2");
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryMultipleUntilT3()
        {
            // Setup
            var animals = new List<Animal>();
            for (var i = 1; i <= 3; i++)
            {
                var animal = new Animal
                {
                    Id = Guid.NewGuid(),
                    Name = $"Animal{i}"
                };
                animals.Add(animal);
            }

            using (var connection = new SqlConnection(Constants.TestDatabase))
            {
                // Act (Insert)
                connection.DeleteAll<Animal>();
                animals.ForEach(animal => connection.Insert(animal));

                // Act (Query)
                var result = connection.QueryMultiple<Animal, Animal, Animal>(
                    where1: a => a.Name != null, top1: 1, orderBy1: OrderField.Parse(new { Name = Enumerations.Order.Ascending }),
                    where2: a => a.Name != null, top2: 2, orderBy2: OrderField.Parse(new { Name = Enumerations.Order.Ascending }),
                    where3: a => a.Name != null, top3: 3, orderBy3: OrderField.Parse(new { Name = Enumerations.Order.Ascending }));

                // Assert (Insert)
                Assert.AreEqual(result.Item1.Count(), 1);
                Assert.AreEqual(result.Item2.Count(), 2);
                Assert.AreEqual(result.Item3.Count(), 3);

                // Assert (Values)
                Assert.AreEqual(result.Item1.Last().Name, "Animal1");
                Assert.AreEqual(result.Item2.Last().Name, "Animal2");
                Assert.AreEqual(result.Item3.Last().Name, "Animal3");
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryMultipleUntilT4()
        {
            // Setup
            var animals = new List<Animal>();
            for (var i = 1; i <= 4; i++)
            {
                var animal = new Animal
                {
                    Id = Guid.NewGuid(),
                    Name = $"Animal{i}"
                };
                animals.Add(animal);
            }

            using (var connection = new SqlConnection(Constants.TestDatabase))
            {
                // Act (Insert)
                connection.DeleteAll<Animal>();
                animals.ForEach(animal => connection.Insert(animal));

                // Act (Query)
                var result = connection.QueryMultiple<Animal, Animal, Animal, Animal>(
                    where1: a => a.Name != null, top1: 1, orderBy1: OrderField.Parse(new { Name = Enumerations.Order.Ascending }),
                    where2: a => a.Name != null, top2: 2, orderBy2: OrderField.Parse(new { Name = Enumerations.Order.Ascending }),
                    where3: a => a.Name != null, top3: 3, orderBy3: OrderField.Parse(new { Name = Enumerations.Order.Ascending }),
                    where4: a => a.Name != null, top4: 4, orderBy4: OrderField.Parse(new { Name = Enumerations.Order.Ascending }));

                // Assert (Insert)
                Assert.AreEqual(result.Item1.Count(), 1);
                Assert.AreEqual(result.Item2.Count(), 2);
                Assert.AreEqual(result.Item3.Count(), 3);
                Assert.AreEqual(result.Item4.Count(), 4);

                // Assert (Values)
                Assert.AreEqual(result.Item1.Last().Name, "Animal1");
                Assert.AreEqual(result.Item2.Last().Name, "Animal2");
                Assert.AreEqual(result.Item3.Last().Name, "Animal3");
                Assert.AreEqual(result.Item4.Last().Name, "Animal4");
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryMultipleUntilT5()
        {
            // Setup
            var animals = new List<Animal>();
            for (var i = 1; i <= 5; i++)
            {
                var animal = new Animal
                {
                    Id = Guid.NewGuid(),
                    Name = $"Animal{i}"
                };
                animals.Add(animal);
            }

            using (var connection = new SqlConnection(Constants.TestDatabase))
            {
                // Act (Insert)
                connection.DeleteAll<Animal>();
                animals.ForEach(animal => connection.Insert(animal));

                // Act (Query)
                var result = connection.QueryMultiple<Animal, Animal, Animal, Animal, Animal>(
                    where1: a => a.Name != null, top1: 1, orderBy1: OrderField.Parse(new { Name = Enumerations.Order.Ascending }),
                    where2: a => a.Name != null, top2: 2, orderBy2: OrderField.Parse(new { Name = Enumerations.Order.Ascending }),
                    where3: a => a.Name != null, top3: 3, orderBy3: OrderField.Parse(new { Name = Enumerations.Order.Ascending }),
                    where4: a => a.Name != null, top4: 4, orderBy4: OrderField.Parse(new { Name = Enumerations.Order.Ascending }),
                    where5: a => a.Name != null, top5: 5, orderBy5: OrderField.Parse(new { Name = Enumerations.Order.Ascending }));

                // Assert (Insert)
                Assert.AreEqual(result.Item1.Count(), 1);
                Assert.AreEqual(result.Item2.Count(), 2);
                Assert.AreEqual(result.Item3.Count(), 3);
                Assert.AreEqual(result.Item4.Count(), 4);
                Assert.AreEqual(result.Item5.Count(), 5);

                // Assert (Values)
                Assert.AreEqual(result.Item1.Last().Name, "Animal1");
                Assert.AreEqual(result.Item2.Last().Name, "Animal2");
                Assert.AreEqual(result.Item3.Last().Name, "Animal3");
                Assert.AreEqual(result.Item4.Last().Name, "Animal4");
                Assert.AreEqual(result.Item5.Last().Name, "Animal5");
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryMultipleUntilT6()
        {
            // Setup
            var animals = new List<Animal>();
            for (var i = 1; i <= 6; i++)
            {
                var animal = new Animal
                {
                    Id = Guid.NewGuid(),
                    Name = $"Animal{i}"
                };
                animals.Add(animal);
            }

            using (var connection = new SqlConnection(Constants.TestDatabase))
            {
                // Act (Insert)
                connection.DeleteAll<Animal>();
                animals.ForEach(animal => connection.Insert(animal));

                // Act (Query)
                var result = connection.QueryMultiple<Animal, Animal, Animal, Animal, Animal, Animal>(
                    where1: a => a.Name != null, top1: 1, orderBy1: OrderField.Parse(new { Name = Enumerations.Order.Ascending }),
                    where2: a => a.Name != null, top2: 2, orderBy2: OrderField.Parse(new { Name = Enumerations.Order.Ascending }),
                    where3: a => a.Name != null, top3: 3, orderBy3: OrderField.Parse(new { Name = Enumerations.Order.Ascending }),
                    where4: a => a.Name != null, top4: 4, orderBy4: OrderField.Parse(new { Name = Enumerations.Order.Ascending }),
                    where5: a => a.Name != null, top5: 5, orderBy5: OrderField.Parse(new { Name = Enumerations.Order.Ascending }),
                    where6: a => a.Name != null, top6: 6, orderBy6: OrderField.Parse(new { Name = Enumerations.Order.Ascending }));

                // Assert (Insert)
                Assert.AreEqual(result.Item1.Count(), 1);
                Assert.AreEqual(result.Item2.Count(), 2);
                Assert.AreEqual(result.Item3.Count(), 3);
                Assert.AreEqual(result.Item4.Count(), 4);
                Assert.AreEqual(result.Item5.Count(), 5);
                Assert.AreEqual(result.Item6.Count(), 6);

                // Assert (Values)
                Assert.AreEqual(result.Item1.Last().Name, "Animal1");
                Assert.AreEqual(result.Item2.Last().Name, "Animal2");
                Assert.AreEqual(result.Item3.Last().Name, "Animal3");
                Assert.AreEqual(result.Item4.Last().Name, "Animal4");
                Assert.AreEqual(result.Item5.Last().Name, "Animal5");
                Assert.AreEqual(result.Item6.Last().Name, "Animal6");
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryMultipleUntilT7()
        {
            // Setup
            var animals = new List<Animal>();
            for (var i = 1; i <= 7; i++)
            {
                var animal = new Animal
                {
                    Id = Guid.NewGuid(),
                    Name = $"Animal{i}"
                };
                animals.Add(animal);
            }

            using (var connection = new SqlConnection(Constants.TestDatabase))
            {
                // Act (Insert)
                connection.DeleteAll<Animal>();
                animals.ForEach(animal => connection.Insert(animal));

                // Act (Query)
                var result = connection.QueryMultiple<Animal, Animal, Animal, Animal, Animal, Animal, Animal>(
                    where1: a => a.Name != null, top1: 1, orderBy1: OrderField.Parse(new { Name = Enumerations.Order.Ascending }),
                    where2: a => a.Name != null, top2: 2, orderBy2: OrderField.Parse(new { Name = Enumerations.Order.Ascending }),
                    where3: a => a.Name != null, top3: 3, orderBy3: OrderField.Parse(new { Name = Enumerations.Order.Ascending }),
                    where4: a => a.Name != null, top4: 4, orderBy4: OrderField.Parse(new { Name = Enumerations.Order.Ascending }),
                    where5: a => a.Name != null, top5: 5, orderBy5: OrderField.Parse(new { Name = Enumerations.Order.Ascending }),
                    where6: a => a.Name != null, top6: 6, orderBy6: OrderField.Parse(new { Name = Enumerations.Order.Ascending }),
                    where7: a => a.Name != null, top7: 7, orderBy7: OrderField.Parse(new { Name = Enumerations.Order.Ascending }));

                // Assert (Insert)
                Assert.AreEqual(result.Item1.Count(), 1);
                Assert.AreEqual(result.Item2.Count(), 2);
                Assert.AreEqual(result.Item3.Count(), 3);
                Assert.AreEqual(result.Item4.Count(), 4);
                Assert.AreEqual(result.Item5.Count(), 5);
                Assert.AreEqual(result.Item6.Count(), 6);
                Assert.AreEqual(result.Item7.Count(), 7);

                // Assert (Values)
                Assert.AreEqual(result.Item1.Last().Name, "Animal1");
                Assert.AreEqual(result.Item2.Last().Name, "Animal2");
                Assert.AreEqual(result.Item3.Last().Name, "Animal3");
                Assert.AreEqual(result.Item4.Last().Name, "Animal4");
                Assert.AreEqual(result.Item5.Last().Name, "Animal5");
                Assert.AreEqual(result.Item6.Last().Name, "Animal6");
                Assert.AreEqual(result.Item7.Last().Name, "Animal7");
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryMultipleUntilT2()
        {
            // Setup
            var animals = new List<Animal>();
            for (var i = 1; i <= 2; i++)
            {
                var animal = new Animal
                {
                    Id = Guid.NewGuid(),
                    Name = $"Animal{i}"
                };
                animals.Add(animal);
            }

            using (var repository = new DbRepository<SqlConnection>(Constants.TestDatabase))
            {
                // Act (Insert)
                repository.DeleteAll<Animal>();
                animals.ForEach(animal => repository.Insert(animal));

                // Act (Query)
                var result = repository.QueryMultiple<Animal, Animal>(
                    where1: a => a.Name != null, top1: 1, orderBy1: OrderField.Parse(new { Name = Enumerations.Order.Ascending }),
                    where2: a => a.Name != null, top2: 2, orderBy2: OrderField.Parse(new { Name = Enumerations.Order.Ascending }));

                // Assert (Insert)
                Assert.AreEqual(result.Item1.Count(), 1);
                Assert.AreEqual(result.Item2.Count(), 2);

                // Assert (Values)
                Assert.AreEqual(result.Item1.Last().Name, "Animal1");
                Assert.AreEqual(result.Item2.Last().Name, "Animal2");
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryMultipleUntilT3()
        {
            // Setup
            var animals = new List<Animal>();
            for (var i = 1; i <= 3; i++)
            {
                var animal = new Animal
                {
                    Id = Guid.NewGuid(),
                    Name = $"Animal{i}"
                };
                animals.Add(animal);
            }

            using (var repository = new DbRepository<SqlConnection>(Constants.TestDatabase))
            {
                // Act (Insert)
                repository.DeleteAll<Animal>();
                animals.ForEach(animal => repository.Insert(animal));

                // Act (Query)
                var result = repository.QueryMultiple<Animal, Animal, Animal>(
                    where1: a => a.Name != null, top1: 1, orderBy1: OrderField.Parse(new { Name = Enumerations.Order.Ascending }),
                    where2: a => a.Name != null, top2: 2, orderBy2: OrderField.Parse(new { Name = Enumerations.Order.Ascending }),
                    where3: a => a.Name != null, top3: 3, orderBy3: OrderField.Parse(new { Name = Enumerations.Order.Ascending }));

                // Assert (Insert)
                Assert.AreEqual(result.Item1.Count(), 1);
                Assert.AreEqual(result.Item2.Count(), 2);
                Assert.AreEqual(result.Item3.Count(), 3);

                // Assert (Values)
                Assert.AreEqual(result.Item1.Last().Name, "Animal1");
                Assert.AreEqual(result.Item2.Last().Name, "Animal2");
                Assert.AreEqual(result.Item3.Last().Name, "Animal3");
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryMultipleUntilT4()
        {
            // Setup
            var animals = new List<Animal>();
            for (var i = 1; i <= 4; i++)
            {
                var animal = new Animal
                {
                    Id = Guid.NewGuid(),
                    Name = $"Animal{i}"
                };
                animals.Add(animal);
            }

            using (var repository = new DbRepository<SqlConnection>(Constants.TestDatabase))
            {
                // Act (Insert)
                repository.DeleteAll<Animal>();
                animals.ForEach(animal => repository.Insert(animal));

                // Act (Query)
                var result = repository.QueryMultiple<Animal, Animal, Animal, Animal>(
                    where1: a => a.Name != null, top1: 1, orderBy1: OrderField.Parse(new { Name = Enumerations.Order.Ascending }),
                    where2: a => a.Name != null, top2: 2, orderBy2: OrderField.Parse(new { Name = Enumerations.Order.Ascending }),
                    where3: a => a.Name != null, top3: 3, orderBy3: OrderField.Parse(new { Name = Enumerations.Order.Ascending }),
                    where4: a => a.Name != null, top4: 4, orderBy4: OrderField.Parse(new { Name = Enumerations.Order.Ascending }));

                // Assert (Insert)
                Assert.AreEqual(result.Item1.Count(), 1);
                Assert.AreEqual(result.Item2.Count(), 2);
                Assert.AreEqual(result.Item3.Count(), 3);
                Assert.AreEqual(result.Item4.Count(), 4);

                // Assert (Values)
                Assert.AreEqual(result.Item1.Last().Name, "Animal1");
                Assert.AreEqual(result.Item2.Last().Name, "Animal2");
                Assert.AreEqual(result.Item3.Last().Name, "Animal3");
                Assert.AreEqual(result.Item4.Last().Name, "Animal4");
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryMultipleUntilT5()
        {
            // Setup
            var animals = new List<Animal>();
            for (var i = 1; i <= 5; i++)
            {
                var animal = new Animal
                {
                    Id = Guid.NewGuid(),
                    Name = $"Animal{i}"
                };
                animals.Add(animal);
            }

            using (var repository = new DbRepository<SqlConnection>(Constants.TestDatabase))
            {
                // Act (Insert)
                repository.DeleteAll<Animal>();
                animals.ForEach(animal => repository.Insert(animal));

                // Act (Query)
                var result = repository.QueryMultiple<Animal, Animal, Animal, Animal, Animal>(
                    where1: a => a.Name != null, top1: 1, orderBy1: OrderField.Parse(new { Name = Enumerations.Order.Ascending }),
                    where2: a => a.Name != null, top2: 2, orderBy2: OrderField.Parse(new { Name = Enumerations.Order.Ascending }),
                    where3: a => a.Name != null, top3: 3, orderBy3: OrderField.Parse(new { Name = Enumerations.Order.Ascending }),
                    where4: a => a.Name != null, top4: 4, orderBy4: OrderField.Parse(new { Name = Enumerations.Order.Ascending }),
                    where5: a => a.Name != null, top5: 5, orderBy5: OrderField.Parse(new { Name = Enumerations.Order.Ascending }));

                // Assert (Insert)
                Assert.AreEqual(result.Item1.Count(), 1);
                Assert.AreEqual(result.Item2.Count(), 2);
                Assert.AreEqual(result.Item3.Count(), 3);
                Assert.AreEqual(result.Item4.Count(), 4);
                Assert.AreEqual(result.Item5.Count(), 5);

                // Assert (Values)
                Assert.AreEqual(result.Item1.Last().Name, "Animal1");
                Assert.AreEqual(result.Item2.Last().Name, "Animal2");
                Assert.AreEqual(result.Item3.Last().Name, "Animal3");
                Assert.AreEqual(result.Item4.Last().Name, "Animal4");
                Assert.AreEqual(result.Item5.Last().Name, "Animal5");
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryMultipleUntilT6()
        {
            // Setup
            var animals = new List<Animal>();
            for (var i = 1; i <= 6; i++)
            {
                var animal = new Animal
                {
                    Id = Guid.NewGuid(),
                    Name = $"Animal{i}"
                };
                animals.Add(animal);
            }

            using (var repository = new DbRepository<SqlConnection>(Constants.TestDatabase))
            {
                // Act (Insert)
                repository.DeleteAll<Animal>();
                animals.ForEach(animal => repository.Insert(animal));

                // Act (Query)
                var result = repository.QueryMultiple<Animal, Animal, Animal, Animal, Animal, Animal>(
                    where1: a => a.Name != null, top1: 1, orderBy1: OrderField.Parse(new { Name = Enumerations.Order.Ascending }),
                    where2: a => a.Name != null, top2: 2, orderBy2: OrderField.Parse(new { Name = Enumerations.Order.Ascending }),
                    where3: a => a.Name != null, top3: 3, orderBy3: OrderField.Parse(new { Name = Enumerations.Order.Ascending }),
                    where4: a => a.Name != null, top4: 4, orderBy4: OrderField.Parse(new { Name = Enumerations.Order.Ascending }),
                    where5: a => a.Name != null, top5: 5, orderBy5: OrderField.Parse(new { Name = Enumerations.Order.Ascending }),
                    where6: a => a.Name != null, top6: 6, orderBy6: OrderField.Parse(new { Name = Enumerations.Order.Ascending }));

                // Assert (Insert)
                Assert.AreEqual(result.Item1.Count(), 1);
                Assert.AreEqual(result.Item2.Count(), 2);
                Assert.AreEqual(result.Item3.Count(), 3);
                Assert.AreEqual(result.Item4.Count(), 4);
                Assert.AreEqual(result.Item5.Count(), 5);
                Assert.AreEqual(result.Item6.Count(), 6);

                // Assert (Values)
                Assert.AreEqual(result.Item1.Last().Name, "Animal1");
                Assert.AreEqual(result.Item2.Last().Name, "Animal2");
                Assert.AreEqual(result.Item3.Last().Name, "Animal3");
                Assert.AreEqual(result.Item4.Last().Name, "Animal4");
                Assert.AreEqual(result.Item5.Last().Name, "Animal5");
                Assert.AreEqual(result.Item6.Last().Name, "Animal6");
            }
        }

        [TestMethod]
        public void TestDbRepositoryQueryMultipleUntilT7()
        {
            // Setup
            var animals = new List<Animal>();
            for (var i = 1; i <= 7; i++)
            {
                var animal = new Animal
                {
                    Id = Guid.NewGuid(),
                    Name = $"Animal{i}"
                };
                animals.Add(animal);
            }

            using (var repository = new DbRepository<SqlConnection>(Constants.TestDatabase))
            {
                // Act (Insert)
                repository.DeleteAll<Animal>();
                animals.ForEach(animal => repository.Insert(animal));

                // Act (Query)
                var result = repository.QueryMultiple<Animal, Animal, Animal, Animal, Animal, Animal, Animal>(
                    where1: a => a.Name != null, top1: 1, orderBy1: OrderField.Parse(new { Name = Enumerations.Order.Ascending }),
                    where2: a => a.Name != null, top2: 2, orderBy2: OrderField.Parse(new { Name = Enumerations.Order.Ascending }),
                    where3: a => a.Name != null, top3: 3, orderBy3: OrderField.Parse(new { Name = Enumerations.Order.Ascending }),
                    where4: a => a.Name != null, top4: 4, orderBy4: OrderField.Parse(new { Name = Enumerations.Order.Ascending }),
                    where5: a => a.Name != null, top5: 5, orderBy5: OrderField.Parse(new { Name = Enumerations.Order.Ascending }),
                    where6: a => a.Name != null, top6: 6, orderBy6: OrderField.Parse(new { Name = Enumerations.Order.Ascending }),
                    where7: a => a.Name != null, top7: 7, orderBy7: OrderField.Parse(new { Name = Enumerations.Order.Ascending }));

                // Assert (Insert)
                Assert.AreEqual(result.Item1.Count(), 1);
                Assert.AreEqual(result.Item2.Count(), 2);
                Assert.AreEqual(result.Item3.Count(), 3);
                Assert.AreEqual(result.Item4.Count(), 4);
                Assert.AreEqual(result.Item5.Count(), 5);
                Assert.AreEqual(result.Item6.Count(), 6);
                Assert.AreEqual(result.Item7.Count(), 7);

                // Assert (Values)
                Assert.AreEqual(result.Item1.Last().Name, "Animal1");
                Assert.AreEqual(result.Item2.Last().Name, "Animal2");
                Assert.AreEqual(result.Item3.Last().Name, "Animal3");
                Assert.AreEqual(result.Item4.Last().Name, "Animal4");
                Assert.AreEqual(result.Item5.Last().Name, "Animal5");
                Assert.AreEqual(result.Item6.Last().Name, "Animal6");
                Assert.AreEqual(result.Item7.Last().Name, "Animal7");
            }
        }
    }
}