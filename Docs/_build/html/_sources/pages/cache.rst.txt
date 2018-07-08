Working with Cache
==================

.. highlight:: c#

The library supports caching when querying a data from the database. By the default, the `RepoDb.MemoryCache` is being used by the library. Given the name itself, the library only works with memory caching by default. A cache is only working on `Query` operation of the repository.

A cache key is important in order for the caching to cache the object. It should be unique to every cache item.

Below are the methods of `Cache` object.

- **Add**: accepts an `item` or a `key` and `value` pair parameters. It adds an item to the `Cache` object. If an item is already existing, the item will be overriden.
- **Clear**: clear all items from the cache.
- **Contains**: accepts a `key` parameter. Checks whether the `Cache` object contains an item with the defined key.
- **Get**: accepts a `key` parameter. Returns a cached item object.
- **GetEnumerator**: returns an enumerator for `IEnumerable<CacheItem>` objects. It contains all the cached items from the `Cache` object.
- **Remove**: accepts a `key` parameter. Removes an entry from the `Cache` object.

One important object when manipulating a cache is the `CacheItem` object (implements `RepoDb.CacheItem`). It acts as the cache item entry for the cache object. The default expiration of the `CacheItem` is 180 minutes.

Below are the constructor arguments of the `CacheItem` object.

- **key**: the key of the cache.
- **value**: the value object of the cache.
- **expirationInMinutes**: The expiration in minutes of the cache item.

Below are the properties of `CacheItem` object.

- **Key**: the key of the cache. It returns a `System.String` type.
- **Value**: the cached object of the item. It returns a `System.Object` type. It can be casted back to a defined object type.
- **CreatedDate**: the created timestamp of this cache item. By default, it is equals to the time of when this cache item object has been instantiated. It returns a `System.DateTime` object.
- **Expiration**: the expiration date of this cache item. It returns a `System.DateTime` object.

Below are the methods of `CacheItem` object.

- **IsExpired**: Checks whether the cached item is expired. It returns a `System.Boolean` value.

The repository caching operation is of the `pseudo` below.

.. highlight:: none

::

	VAR item = null
	IF ($cacheKey is not null) THEN
		set $item = get value from cache where the key equals to $cacheKey
		IF ($item is not null) THEN
			RETURN item
		END IF
	END IF
	VAR $result = query the data from the database
	IF ($result is not null AND $cacheKey is not null) THEN
		Add cache item where:
			Key = $cacheKey
			Value = $result
	END IF
	RETURN $result

Creating a Cache Entry
----------------------

The snippets below declared a variable named `cacheKey`. The value of this variable acts as the key value of the items to be cached by the repository.

.. highlight:: c#

::

	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
	var cacheKey = "CacheKey.Customers.StartsWith.Anna";
	var result = repository.Query<Customer>(new { Name = new { Operation = Operation.Like, Value = "Anna%" } }, cacheKey: cacheKey);

First, it wil query the data from the database where the `Name` is started at `Anna`. Then, the operation will cache the result into the `Cache` object with the given key at the variable named `cacheKey` (valued `CacheKey.Customers.StartsWith.Anna`).

The next time the same query is executed, the repository automatically returns the cached item if the same key is passed.

Please note that the cache object of the repository is immutable per instance, this means that accessing the cache object directly passing the same cache key would return the same result.

Codes below will return the same result as above assuming the same repository object is used.

::

	var customers = (IEnumerable<Customer>)repository.Cache.Get("CacheKey.Customers.StartsWith.Anna").Value;

Checking a Cache Entry
----------------------

.. highlight:: c#

Code below is the way on how to check if the cached item is present on the `Cache` object, assuming that a repository object has been created already.

::

	var isExists = repository.Cache.Contains("CacheKey");

Checking a Cache Expiration
---------------------------

.. highlight:: c#

Code below is the way on how to check if the cached item is expired already, assuming that a repository object has been created already.

::

	var isExpired = repository.Cache.Get("CacheKey").IsExpired();

Setting the Cache Expiration
----------------------------

.. highlight:: c#

Code below is the way on how to set cached item is expiration, assuming that a repository object has been created already.

::

	repository.Cache.Get("CacheKey").Expiration = DateTime.UtcNow.Date.AddHours(3);

Iterating the Cache Entries
---------------------------

.. highlight:: c#

Code below is the way on how to retrieve or iterate all the cached items from the `Cache` object, assuming that a repository object has been created already.

::

	// Let's expect that the repository is meant for Customer data entity
	foreach (var item in repository.Cache)
	{
		var item = (IEnumerable<Customer>)item.Value;
		// Process the item here
	}

Removing or Clearing a Cache
----------------------------

.. highlight:: c#

By default, the library does not support the auto-flush of the cache. Those forcing the developers to handle the flushing on its way.

Clearing or removing an entry from a cache is the only way to flush the cached objects.

See below on how to clear the cached item from the `Cache` object, assuming that a repository object has been created already.

::

	repository.Cache.Clear();

Below is the way to remove specific cache item.

::

	repository.Cache.Remove("CacheKey");


Injecting a Custom Cache Object
-------------------------------

.. highlight:: c#

The library supports a cache object injection in the repository level. As mentioned earlier, by default, the library is using the `RepoDb.MemoryCache` object. It can overriden by creating a class and implements the `RepoDb.Interfaces.ICache` interface, and passed it to the `cache` argument of the repository constructor.

Below is the way on how to create a custom `Cache` object.

::

	public class FileCache : ICache
	{
		public FileCache(string location)
		{
			// Add a logic on the constructor
		}

		public void Add(string key, object value)
		{
			// Serialize to a File
		}

		public void Add(CacheItem item)
		{
			// Serialize to a File
		}

		public void Clear()
		{
			// Delete the Files
		}

		public bool Contains(string key)
		{
			// Check if the Filename exists by Key
		}

		public CacheItem Get(string key)
		{
			// Deserialize the File where the FileName is equals to Key, return the object
		}

		public IEnumerator<CacheItem> GetEnumerator()
		{
			// Get the File.ParentFolder.Files enumerator and deserialize each file
		}

		public void Remove(string key)
		{
			// Delete the File where the FileName is equals to Key
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			// Get the File.ParentFolder.Files enumerator and deserialize each file
		}
	}

The snippets above creates a class named `FileCache` that implements the `ICache` interfaces. By implementing the said interface, the class is now qualified to become a library `Cache` object.

Below is the way on how to inject the custom `Cache` object to a repository.

::

	var fileCache = new FileCache();
	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;",fileCache);

Upon creating a repository, the `fileCache` variable is being passed in the `cache` parameter. This signals the repository to use the `FileCache` class as the `Cache` object manager of the `Query` operation.

**Note:** The caller can activate a debugger on the `FileCache` class to enable debugging. When the callers call the `Query` method and passed a `cacheKey` value on it, the breakpoint will be hitted by the debugger if it is placed inside `Add` method of the `FileCache` class.