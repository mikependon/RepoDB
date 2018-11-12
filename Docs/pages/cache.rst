Cache
=====

.. highlight:: c#

The library supports caching when querying a data from the database. By the default, the `RepoDb.MemoryCache` is being used by the library. A cache is only working on `Query` operation of the repository.

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

Creation
--------

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

Contains
--------

.. highlight:: c#

Code below is the way on how to check if the cached item is present on the `Cache` object, assuming that a repository object has been created already.

::

	var isExists = repository.Cache.Contains("CacheKey");

IsExpired
---------

.. highlight:: c#

Code below is the way on how to check if the cached item is expired already, assuming that a repository object has been created already.

::

	var isExpired = repository.Cache.Get("CacheKey").IsExpired();

Expiration
----------

.. highlight:: c#

Code below is the way on how to set cached item is expiration, assuming that a repository object has been created already.

::

	repository.Cache.Get("CacheKey").Expiration = DateTime.UtcNow.Date.AddHours(5);

The default expiration of the `CacheItem` is 180 minutes.

Iteration
---------

.. highlight:: c#

Code below is the way on how to retrieve or iterate all the cached items from the `Cache` object, assuming that a repository object has been created already.

::

	// Let's expect that the repository is meant for Customer data entity
	foreach (var item in repository.Cache)
	{
		var item = (IEnumerable<Customer>)item.Value;
		// Process the item here
	}

Remove
------

.. highlight:: c#

By default, the library does not support the auto-flush of the cache. Those forcing the developers to handle the flushing on its way.

Clearing or removing an entry from a cache is the only way to flush the cached objects.

See below on how to clear the cached item from the `Cache` object, assuming that a repository object has been created already.

::

	repository.Cache.Clear();

Below is the way to remove specific cache item.

::

	repository.Cache.Remove("CacheKey");


ICache
------

.. highlight:: c#

The library supports a cache object injection in the repository level. As mentioned earlier, by default, the library is using the `RepoDb.MemoryCache` object. It can overriden by creating a class and implements the `RepoDb.Interfaces.ICache` interface, and passed it to the `cache` argument of the repository constructor.

Below is the way on how to create a custom `Cache` object.

::

	public class FileCache : ICache
	{
		...
	}

The snippets above creates a class named `FileCache` that implements the `ICache` interfaces. By implementing the said interface, the class is now qualified to become a library `Cache` object.

Below is the way on how to inject the custom `Cache` object to a repository.

::

	var fileCache = new FileCache();
	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;", fileCache);

Upon creating a repository, the `fileCache` variable is being passed in the `cache` parameter. This signals the repository to use the `FileCache` class as the `Cache` object manager of the `Query` operation.

**Note:** The caller can activate a debugger on the `FileCache` class to enable debugging. When the callers call the `Query` method and passed a `cacheKey` value on it, the breakpoint will be hitted by the debugger if it is placed inside `Add` method of the `FileCache` class.