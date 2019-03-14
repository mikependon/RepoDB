Cache
=====

.. highlight:: c#

It is a feature used to cache the result set from the database. By the default, the `RepoDb.MemoryCache` is being used by the library. A cache is only working on the `Query` operation.

The caching is of the `pseudo` below.

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

To cache a result, simply pass a value in the `cacheKey` argument of the `Query` operation.

.. highlight:: c#

::

	using (var connection = new SqlConnection(@"Server=.;Database=Northwind;Integrated Security=SSPI;"))
	{
		var result = repository.Query<Customer>(c => c.Name.StartsWith("Anna"), cacheKey: "KeyToTheCache");
	}

Codes below will return the same result as above assuming the same repository object is used.

::

	var customers = (IEnumerable<Customer>)repository.Cache.Get("KeyToTheCache").Value;

Contains
--------

Checks whether the key is present in the collection.

.. highlight:: c#

::

	var exists = repository.Cache.Contains("KeyToTheCache");

IsExpired
---------

Gets a value whether the cache item is expired.

.. highlight:: c#

::

	var isExpired = repository.Cache.Get("KeyToTheCache").IsExpired();

Expiration
----------

Code below is the way on how to set cached item expiration, assuming that a repository object has been created already.

.. highlight:: c#

::

	repository.Cache.Get("KeyToTheCache").Expiration = DateTime.UtcNow.Date.AddHours(5);

The default expiration of the `CacheItem` is 180 minutes. See `Constants.DefaultCacheItemExpirationInMinutes`.

Iteration
---------

.. highlight:: c#

Code below is the way on how to retrieve or iterate all the cached items from the `Cache` object, assuming that a repository object has been created already.

::

	// Let`s expect that the repository is meant for Customer data entity
	foreach (var item in repository.Cache)
	{
		var item = (IEnumerable<Customer>)item.Value;
		// Process the item here
	}

Remove
------

Removes an item from the cache collection.

.. highlight:: c#

::

	repository.Cache.Clear();

Below is the way to remove specific cache item.

::

	repository.Cache.Remove("KeyToTheCache");


ICache
------

Is an interface used to create a cache object.

.. highlight:: c#

::

	public class FileCache : ICache
	{
		...
	}

Below is the way on how to inject the custom `Cache` object to a repository.

::

	var fileCache = new FileCache();
	var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;", fileCache);

Upon creating a repository, the `fileCache` variable is being passed in the `cache` parameter. This signals the repository to use the `FileCache` class as the `Cache` object manager of the `Query` operation.

**Note:** The caller can activate a debugger on the `FileCache` class to enable debugging. When the callers call the `Query` method and passed a `cacheKey` value on it, the breakpoint will be hitted by the debugger.