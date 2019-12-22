## Introduction

In this page, you will learn the following.

- [Creating a custom cache object](https://github.com/mikependon/RepoDb/wiki/Customizing-a-Cache#creating-a-custom-cache-object)
- [Implementing the cache operations](https://github.com/mikependon/RepoDb/wiki/Customizing-a-Cache#implementing-the-cache-operations)
- [Injecting a customized cache object](https://github.com/mikependon/RepoDb/wiki/Customizing-a-Cache#injecting-a-customized-cache-object)
- [Why always use a singleton cache object?](https://github.com/mikependon/RepoDb/wiki/Customizing-a-Cache#why-always-use-a-singleton-cache-object)

## Before we begin

The programming language we will be using is *C#*. Please have at least *Visual Studio 2017* installed in your machine.

This topic is a bit advance, so we expect that you have already experienced using the *RepoDb* library.

We also expect that you already have created your own *Project/Solution* for this tutorial.

## What is Cache?

In general terms, a *Cache* is a component that stores an object (or its states) that is accessible for future used. The object that is being stored can be a result of *computational*, *operational*, *inputs/outputs* or *analytical* operations and calculations.

Usually, it is implemented as a *2nd-layer* data storage to provide fast accessibility to the requestor of the data. It is by design to prevent frequent access to the underlying data-store, thus helps provide maximum performance to the software.

In *RepoDb*, by default, the *Cache* is implemented as a storage in computer memory. By nature, it is simply a *dictionary* object that holds the *Key* that represents as the pointer to the actual *Data* in the cache storage. It is persisting the data in the cache storage for *180 minutes*. But, the user can manually set the time of the persistency.

**Note**: The objects that are not frequently changing but is mostly in used in the application are the candidate for caching.

## Creating a custom cache object

To create a custom *Cache* object, you must inherit from the *ICache* interface. This interface is the contract within the library to mark your class as *Cache* object.

After implementing the *ICache* interface, you have to implementation the required methods for you to be able to manipulate your own caches.

Let say our target is to create a *FileCache* object that will do cache in the file system. Mostly, below are the requirements:

- *Filename* - the key of the cache.
- *Extension* - the type of cache.
- *Content* - the actual object value saved as serialized file.
- *Attributes* - the creation or the expiration date.

For the actual implementation, you can follow the steps below.

### Create a FileCache class

- Add a new class in your *Project/Solution*.
- Implement the class as follows.
```csharp
public class FileCache : ICache
{
	public FileCache()
	{
		Extension = "repodbcache";
		Location = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\LocalCache\\"
	}

	public string Extension { get; }
	public string Location { get; }
}
```
- Then, implement the *ICache* methods below.
```csharp
void Add(string key,
	object value,
	int expiration = Constant.DefaultCacheItemExpirationInMinutes,
	bool throwException = true);

void Add(CacheItem item,
	bool throwException = true);

void Clear();

bool Contains(string key);

CacheItem Get(string key,
	bool throwException = true);

void Remove(string key,
	bool throwException = true);
```

### Method Definitions

- *Add* - adds a cache item value.
- *Clear* - clears the collection of the cache.
- *Contains* - checks whether the key is present in the collection.
- *Get* - gets an object from the cache collection.
- *Remove* - removes the item from the cache collection.

## Implementing the cache operations

As mentioned in the previous section definitions, each method defined must be implemented.

### Pre-requisites

The following library and namespaces must be used.
- [System.IO](https://www.nuget.org/packages/System.IO/)
- [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/)

This method must be implemented as a helper-method. Please copy the code snippets below and paste it just after the *Location* property.

```csharp
private string GetFileName(stirng key)
{
	return $"{Location}\\{key}.{Extension}";
}
```

Then, the following methods below must be implemented right after each other.

### Add

In this method, the object must be serialized as a *JSON* object and must be saved as the content of the file. See the implementation below.

```csharp
public void Add(string key,
	object value,
	int expiration = Constant.DefaultCacheItemExpirationInMinutes,
	bool throwException = true)
{
	var item = new CacheItem
	(
		key,
		value,
		expiration
	);
	Add(item, throwException);
}

public void Add(CacheItem item,
	bool throwException = true)
{
	var filename = GetFileName(item.Key);
	if (File.Exists(filename))
	{
		if (throwException)
		{
			throw new IOException($"The '{filename}' is already exists.");
		}
		return;
	}
	var content = JsonConvert.SerializeObject(item);
	File.WriteAllText(filename, content);
}
```

### Clear

In this method, we will clear all the files in the cache location. See the implementation below.

```csharp
public void Clear()
{
	var files = Directory.GetFiles(Location);
	foreach (var file in files)
	{
		File.Delete(file);
	}
}
```

### Contains

In this method, we will return *True* if the *Key* has exists as file. See the implementation below.

```csharp
public bool Contains(string key)
{
	var filename = GetFileName(item.Key);
	return File.Exists(filename);
}
```

### Get

In this method, we will return the actual *CacheItem* object as a deserialized object from the file. See the implementation below.

```csharp
public CacheItem Get(string key,
	bool throwException = true)
{
	var filename = GetFileName(item.Key);
	if (!File.Exists(filename))
	{
		if (throwException)
		{
			throw new IOException($"The '{filename}' is not found.");
		}
		return null;
	}
	var content = File.ReadAllText(filename);
	var item = JsonConvert.DeserializeObject<CacheItem>(content);
	return item;
}
```

### Remove

In this method, we will delete the actual file if present by *Key*. See the implementation below.

```csharp
public void Remove(string key,
	bool throwException = true)
{
	var filename = GetFileName(item.Key);
	if (!File.Exists(filename))
	{
		if (throwException)
		{
			throw new IOException($"The '{filename}' is not found.");
		}
		return null;
	}
	File.Delete(filename);
}
```

At to this point, the class *FileCache* is now in shape to be used as a *Cache* object.

## Injecting a customized cache object

In this section, we will teach you how to inject your customized *Cache* object when calling the *Query* operation or when creating a *Repository* object.

### Injecting to a Query operation

The *DbConnection* extended method named *Query<TEntity>* is accepting an argument *ICache*. If the arguments *Key* and *ICache* are both present, the operation *Query<TEntity>* will then check whether the value of the *key* is present on the defined *Cache* object.

If the value is present, then the value of the cache is returned. However, if the value is not present, then the value from the database will be added as a new cache and will be returned.

Below is the snippets on how to pass the actual cache object during the call.

```csharp
var cache = new FileCache();
using (var connection = new SqlConnection(ConnectionString))
{
	connection.Query<Customer>(e => e.Id == 10045, cacheKey: $"Customer{10045}", cache);
}
```

### Injecting to a Repository

Both the *BaseRepository* and the *DbRepository* are accepting the *ICache* interface as part of the constructor. When you are creating your repository by inheritting any of the 2 *Repository* object, then you have an opportunity to pass your customized *FileCache* object.

Let us say, you are creating a *Repository* object that is inheritting from the *DbRepository* object. See the code snippets below.

```csharp
public class InventoryRepository : DbRepository<SqlConnection>
{
	public InventoryRepository(string connectionString, ICache cache)
		: base(connectionString, cache)
	{
	}
}
```

or, you can even force it like below.

```csharp
public class InventoryRepository : DbRepository<SqlConnection>
{
	public InventoryRepository(string connectionString)
		: base(connectionString, new FileCache())
	{
	}
}
```

So, every time you create a new instance of *InventoryRepository* class, then you are also using the *FileCache* cache object.

However, the ideal implementation is to use the first option so you can also make your *Cache* object *dependency-injectable*.

### Why always use a singleton cache object?

A *Cache* is equivalent to a *Local Temporary Storage* that would help you minimizes the round trips from the database.

By making it *Singleton*, you are holding a single pointer of the *Cache* object and is reusing the same instance in many ways. Those making sure that every time there is a *database activity* that requires a cache operation, only *single* instance is catering the requests and have the full control of the cache items.

By adding a static *Instance* property of the same object, then you can control how the instance is being created. See sample code below.

```csharp
public class FileCache
{
	private static FileCache _fileCache = null;
	private static object _syncLock = new object();
	
	public static FileCache Instance
	{
		get { return GetInstance(); }
	}
	
	private static FileCache GetInstance()
	{
		if (_fileCache == null)
		{
			lock (_syncLock)
			{
				_fileCache = new FileCache();
			}
		}
		return _fileCache;
	}
}
```

Then pass it every time you call the *Query* method like below.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	connection.Query<Customer>(e => e.Id == 10045, cacheKey: $"Customer{10045}", FileCache.Instance);
}
```

Or, when creating an instance of *Repository* like below.

```csharp
public class InventoryRepository : DbRepository<SqlConnection>
{
	public InventoryRepository(string connectionString)
		: base(connectionString, FileCache.Instance)
	{
	}
}
```

### Thread Safeness

The cache thread-safeness varies on the cache-store.

- *Database is thread-safe.*
- *File system is not thread-safe.*
- *Memory is not thread-safe.*

The latter 2 can be thread-safe if implemented in a different way (with thread-safeness handles).

Mostly, an exception will be thrown if the operation is called in parallel.

--------

**Voila! You have completed this tutorial.**