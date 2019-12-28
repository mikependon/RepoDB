## Introduction

In this page, you will learn the following.

- [Field Mapping](https://github.com/mikependon/RepoDb/wiki/Advance-Field-and-Type-Mapping-Implementation#field-mapping)
- [Type Mapping](https://github.com/mikependon/RepoDb/wiki/Advance-Field-and-Type-Mapping-Implementation#type-mapping)
- [Enumeration Mapping](https://github.com/mikependon/RepoDb/wiki/Advance-Field-and-Type-Mapping-Implementation#high-level-mapping)
- [Conversion Type](https://github.com/mikependon/RepoDb/wiki/Advance-Field-and-Type-Mapping-Implementation#conversion-type-auto-mapping)

## Before we begin

The programming language we will be using is *C#* and the database provider we will be using is *SQL Server*.

We also would like you visits the Microsoft [documentation](https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-data-type-mappings) with regards to *C#* and *SQL Server* type mappings.

**Note**: This feature is not limited to *SQL Server* RDBMS Data Provider.

## Field Mapping

A *field mapping* is the process of forcing *Specific Class Property* of the class to be mapped to a *Specific Database Type*. The mapping can be done through *TypeMap* attribute.

Let us say you want to force map the *OrderDateUtc* property of *Order* class to *DbType.DateTime2* database type.

Then, this can be done through the code snippets below.

```csharp
public class Order
{
	public long Id { get; set; }
	public long ProductId { get; set; }
	public long CustomerId { get; set; }
	[TypeMap(DbType.DateTime2)]
	public DateTime OrderDateUtc { get; set; }
	public int Quantity { get; set; }
	public DateTime DateInsertedUtc { get; set; }
	public DateTime DateModifiedUtc { get; set; }
	public string ModifiedBy { get; set; }
}
```

By default, all *System.DateTime* type is mapped to *DbType.DateTime* database type by ADO.Net.

This scenario is very useful if you are working with *byte[]* (*blobs* or *spatials*) data types.

#### What happened behind the scene of field mapping?

Before the library executes the actual SQL Statements, it sets the *DbCommand.DbType* property of the *@OrderDateUtc* parameter to *DbType.DateTime2*.

Please see the pseudo code below.

```csharp
var parameter = command.CreateParameter();
parameter.Name = "@OrderDateUtc";
parameter.DbType = DbType.DateTime2;
command.Parameters.Add(parameter);
var result = command.ExecuteScalar();
```

## High-Level Mapping

A *type mapping* is the process of forcing *Specific .NET CLR Type* to be mapped to a *Specific Database Type*. The mapping can be done through *TypeMapper* class.

Let us say you want to force map all the *System.DateTime* CLR Types to *DbType.DateTime2* database type.

Then, this can be done through the code snippets below.

```csharp
TypeMapper.Map(typeof(DateTime), DbType.DateTime2);
```

You can read more about this on our [documentation](https://repodb.readthedocs.io/en/latest/pages/typemapping.html).

#### What happened behind the scene of type mapping?

Before the library executes the actual SQL Statements, it sets the *DbCommand.DbType* property of the all date parameters to *DbType.DateTime2*.

Please see the pseudo code below.

```csharp
foreach (var property in properties)
{
	if (property.PropertyType == typeof(DateTime))
	{
		var parameter = command.CreateParameter();
		parameter.Name = $"@{property.Name}";
		parameter.DbType = DbType.DateTime2;
		command.Parameters.Add(parameter);
	}
}
var result = command.ExecuteScalar();
```

## Enumeration Mapping

By default, if you pass the *Enumeration* instance as the value of the *DbParameter* when saving to the database, the data type is automatically mapped by ADO.Net based on the database type of the destination database field.

Let us say, you have a field named *Direction* of type *NVARCHAR(32)* in the database and you created the *DbParameter* object as follows:

```csharp
var parameter = command.CreateParameter();
parameter.Name = "@Direction";
parameter.Value = Direction.East;
command.Parameters.Add(parameter);
command.ExecuteNonQuery();
```

The value will be saved as *String* of which a literal string *East*.

However, if the database type of field *Direction* is *INT*, then the value will be saved as *Integer* (ie: 1, 2 or etc).

### Mapping the Enumeration to specific Database Type

RepoDb provides a feature to allow the mappings of the *Enumerations*. You can this both *Field* and *Enum Type* level.

Let us say, you have the following table in the database.

```
CREATE TABLE [dbo].[Supplier]
(
	[Id] BIGINT IDENTITY(1,1) 
	, [Name] NVARCHAR(128) NOT NULL
	, [State] NVARCHAR(32) NULL,
	, [DateInsertedUtc] DATETIME2(5) NOT NULL
	, [DateModifiedUtc] DATETIME2(5) NOT NULL
	, [ModifiedBy] NVARCHAR(64) NOT NULL
	, CONSTRAINT [PK_Supplier] PRIMARY KEY CLUSTERED ([Id] ASC )
)
ON [PRIMARY];
```

And you have the following *Enum* and *Class*.

```csharp
public enum State
{
	NewYork = 1,
	Chicago = 2,
	Kansas = 3,
	California = 4,
	Washington = 5
}

public class Supplier
{
	public long Id { get; set; }
	public string Name { get; set; }
	public State State { get; set; }
	public DateTime DateInsertedUtc { get; set; }
	public DateTime DateModifiedUtc { get; set; }
	public string ModifiedBy { get; set; }
}
```

Then you call the *Insert* operation as follows.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var supplier = new Supplier
	{
		Name = "Anna Trujillo",
		State = State.Washington,
		DateInsertedUtc = DateTime.UtcNow,
		DateModifiedUtc = DateTime.UtcNow,
		ModifiedBy = "ClientApp"
	};
	var customer = connection.Insert<Supplier>(supplier);
}
```

The actual value of the *State* field in the database table will be *Washington*.

### Enum Field Mapping

However, if you specify the *TypeMap* attribute in the *State* property of the *Supplier* class, then you can override this behavior. See sample code snippets below.

```csharp
public class Supplier
{
	public long Id { get; set; }
	public string Name { get; set; }
	[TypeMap(DbType.Int32)]
	public State State { get; set; }
	public DateTime DateInsertedUtc { get; set; }
	public DateTime DateModifiedUtc { get; set; }
	public string ModifiedBy { get; set; }
}
```

And by calling the same *Insert* operation below.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var supplier = new Supplier
	{
		Name = "Anna Trujillo",
		State = State.Washington,
		DateInsertedUtc = DateTime.UtcNow,
		DateModifiedUtc = DateTime.UtcNow,
		ModifiedBy = "ClientApp"
	};
	var customer = connection.Insert<Supplier>(supplier);
}
```

Would save a value of *5* in the database.

### Enum Type Mapping

On the other hand, the *Type Mapping* works exactly the same as in the previous section. The only difference is that you are targeting the specific type of the *Enumeration* as below.

```csharp
TypeMap.Map(typeof(State), DbType.Int32);
```

## Conversion Type (Auto Mapping)

A *Conversion Type* is a feature that would automate the *.NET* and *Database* type mapping value conversion. By default, .NET does not do any implicit conversion of most types between the application and database.

To elaborate futher, let us say you have a column *RowGuid NVARCHAR(64)* that holds a *GUID* values. Then, you have a class property named *RowGuid* (of type *System.Guid*).

When you call the *Query* operation, the call will fail due to the fact that the column *RowGuid NVARCHAR(64)* is not covertible to *System.Guid*.

In *RepoDb*, there is term called *ConversionType* in which it allows the developer to override this behavior.

These behaviours can be applied on the following type conversions:

- *Strings* to *Numbers* (vice versa)
- *Numbers* to *Decimals* (vice versa)
- *Guids* to *Strings* (vice versa)
- *Dates* to *Strings* (vice versa)
- *Decimals* to *Strings* (vice versa)

### ConversionType Enum

This is an *enum* that is used to define the conversion logic when converting an instance of *DbDataReader* into a .NET CLR class.

**The following are the values:**

- Default

	The conversion is *strict* and there is *no* additional implied logic during the conversion of *DbDataReader* object into its destination *.NET CLR type*.

- Automatic

	The data type conversion is *not strict*. An additional logic from *System.Convert* object will be used to properly map the *DbDataReader* data type into its destination *.NET CLR type*. The operation will only succeed if the data types are convertible.

By setting the value of the *ConversionType* to *Automatic* will solve the problem mentioned above.

### Setting the ConversionType

To set the value of *ConversionType*, simply set the *TypeMapper* property value as follows.

```csharp
TypeMapper.ConversionType = ConversionType.Automatic;
```

**Note**: Using the *Automatic* conversion would affect the performance of the library.

---------

**Voila! You have completed this tutorial!**
