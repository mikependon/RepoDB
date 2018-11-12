Recursive
=========

A recursive query is used to auto-mapped a child-referenced objects into the current target objects.

RecursiveManager
----------------

A manager class for recursive query. Full namespace at `RepoDb.RecursiveManager`. Below is the definition.

.. highlight:: c#

::

	public static class RecursionManager
	{
		void SetRecursiveMaximumRecursion(int maximumRecursion);
		void SetRecursiveQueryBatchCount(int batchCount);
	}

RecursiveQueryBatchCount
------------------------

The batches count used by the repository (recursive query) operation. The default value is equals to `Constant.DefaultRecursiveQueryBatchCount` value.

SetRecursiveQueryBatchCount
---------------------------

Call the `RecursiveManager.SetRecursiveQueryBatchCount()` method.

.. highlight:: c#

::

	RecursiveManager.SetRecursiveQueryBatchCount(5);

RecursiveQueryMaxRecursion
--------------------------

The maximum recursion that the repository (recursion depth) can execute. The default value is equals to `Constant.DefaultRecursiveQueryMaxRecursion` value.

SetRecursiveMaximumRecursion
----------------------------

Call the `RecursiveManager.SetRecursiveMaximumRecursion()` method.

.. highlight:: c#

::

	RecursiveManager.SetRecursiveMaximumRecursion(5);

Creating a DataEntity
---------------------

A parent data entity must have an `IEnumerable<TEntity>` property of the child data entity.

.. highlight:: c#

::

	// This is a child data entity
	public class Child
	{
		...
	}

	// This is a parent data entity
	public class Parent
	{
		public IEnumerable<Child> Children { get; set; }
	}

Note: The property must be `public` with `get/set` accessor. Furthermore, it should be an `IEnumerable<TEntity>` CLR types, or else, an exception will be thrown when calling the `Query` method.

Below is a sample relationships for `Customer`, `Order`, `OrderItem` and `Product` objects.

::

	public class Product
	{
		public int Id { get; set; }
		public int Name { get; set; }
		public double Price { get; set; }
	}

	public class OrderItem
	{
		public int Id { get; set; }
		public int ProductId { get; set; }
		public int OrderId { get; set; }
		public IEnumerable<Product> Products { get; set; } // Parent to Products
	}

	public class Order
	{
		public int Id { get; set; }
		public int CustomerId { get; set; }
		public int Quantity { get; set; }
		public DateTime OrderDate { get; set; }
		public IEnumerable<OrderItem> OrderItems { get; set; } // Parent to OrderItems
	}

	public class Customer
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public IEnumerable<Order> Orders { get; set; } // Parent to Orders
	}

Querying a Data
---------------

Call the `Query` operation of the connection/repository object by passing a `true` value to the `recursive` argument.

.. highlight:: c#

::

	using (var connection = new SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;").EnsureOpen())
	{
		connection.Query<Customer>(new { CustomerId = Id }, recursive: true);
	}

The query above will return the customer where (Id = 10045) including all the information of its orders, order items and products.

Customizing a Field
-------------------

By default, the child data entities will be linked to the parent based on its (mapped) `Name` + `Id`.

Class below will use a property named `CustomerId` when querying an `Order` records.

.. highlight:: c#

::

	public class Customer 
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public IEnumerable<Order> Orders { get; set; } // Class name is Customer plus word 'Id' = CustomerId, if the Map attribute is used, then it will the mapped name
	}

What if an `Order` field name is not named as `CustomerId`? (Let us say: `ParentId`)

::

	public class Order
	{
		public int Id { get; set; }
		public int ParentId { get; set; } // Not named as CustomerId, but stands as [dbo].[Customer].[Id]
		public int Quantity { get; set; }
		public DateTime OrderDate { get; set; }
	}

To solve this, use the `Foreign` attribute in the parent class.

::

	public class Customer 
	{
		public int Id { get; set; }
		public string Name { get; set; }
		[Foreign("ParentId")] // It will use the ParentId property when querying the child entities
		public IEnumerable<Order> Orders { get; set; }
	}

You can as well define the parent field, not just the child field.

::

	public class Customer 
	{
		public int SomeId { get; set; } // Not actually named as Id
		public string Name { get; set; }
		[Foreign("SomeId", "ParentId")] // Use the second constructor to solve this
		public IEnumerable<Order> Orders { get; set; }
	}

Traversing the Parent
---------------------

With the use of `Foreign` attribute, we can traverse the parent data entity object when calling the `Query` operation.

.. highlight:: c#

::

	public class Order
	{
		public int Id { get; set; }
		public int CustomerId { get; set; }
		public int Quantity { get; set; }
		public DateTime OrderDate { get; set; }
		[Foreign("CustomerId", "Id")] // This attribute at the child entity says that it is a parent of Customer object
		public IEnumerable<Customer> Customer { get; set; }
	}

	public class Customer 
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public IEnumerable<Order> Orders { get; set; }
	}



