Property Handler
================

Is a feature that is being used to manually handle the property transformation between the database and the client application.

IPropertyHandler
----------------

An interface that is used to implement to mark the class as property handler. This interface has `TInput` and `TResult` generic type in which maps to a method for property `Getter` and `Setter`.

Let us say, the classes below is present.

.. code-block:: c#
	:linenos:
	
	public class CustomerExtraInfo
	{
		public string CompleteAddress { get; set; }
		public string Description { get; set; }
		public string Notes { get; set; }
		public string Preferrences { get; set; }
	}
	
	public class Customer
	{
		public int Id { get; set; }
		public string Name { get; set; }
		[PropertyHandler(typeof(CustomerExtraInfoHandler)]
		public CustomerExtraInfo ExtraInfo { get; set; }
	}

And the database table below is present.

.. code-block:: sql
	:linenos:

	CREATE TABLE [dbo].[Customer]
	(
		[Id] BIGINT PRIMARY KEY IDENTITY(1, 1)
		, [Name] NVARCHAR(128)
		, [ExtranInfo] NVARCHAR(MAX)
	)
	ON [PRIMARY];

Then, below is a sample property handler for the `CustomerExtraInfo` class of the `Customer` data entity.

.. code-block:: c#

    public class CustomerExtraInfoHandler : IPropertyHandler<string, CustomerExtraInfo>
    {
        public CustomerExtraInfo Get(string input, ClassProperty property)
        {
            return JsonConvert.DeserializeObject<PersonExtendedInfo>(input);
        }

        public string Set(CustomerExtraInfo input, ClassProperty property)
        {
            return JsonConvert.SerializeObject(input);
        }
    }

In which the value of the `ExtraInfo` property will be pushed as `NVARCHAR(MAX)` in the database during `Insert`, `Update` and `Merge`; the value of `ExtraInfo` column from the database will be extracted as `CustomerExtraInfo` object in the client during `Read` operations.

**Note:** The argument of type *ClassProperty* is provided to both `Get` and `Set` method to add an additional context on the current execution.

PropertyTypeHandlerMapper
-------------------------

A class that is used to map a property handler into a .NET CLR Type. This is usually used for the property-type level transformation scenarios (ie: converting the `DateTime` object `Kind` to `Utc`).

A class can be access anywhere in the application as it is implemented as static.

Let us say, the scenario is to convert all the `DateTime.Kind` properties to `Utc` in all read operations.

.. code-block:: c#
	
    public class DateTimeKindToUtcPropertyHandler : IPropertyHandler<DateTime, DateTime?>
    {
        public DateTime? Get(DateTime input, ClassProperty property)
        {
            return DateTime.SpecifyKind(input, DateTimeKind.Utc);
        }

        public DateTime Set(DateTime? input, ClassProperty property)
        {
            return DateTime.SpecifyKind(input.GetValueOrDefault(), DateTimeKind.Unspecified);
        }
    }

Then, simply call the `Add` method of the `PropertyTypeHandlerMapper` class to add a mapping directly to `DateTime` type.

.. code-block:: sql
	:linenos:

	PropertyTypeHandlerMapper.Add(typeof(DateTime), new DateTimeKindToUtcPropertyHandler());

In the side of the library, every transformation for `DateTime` type will always trigger the `Get` and `Set` method of the `DateTimeKindToUtcPropertyHandler` class.

Scenarios
---------

In reality, with this feature, the scenarios are unlimitted. See some of the known scenarios below.

- Converting a `JSON` column into a `Class` object.
- Handling the correct `DateTime` objects `Kind`.
- Overriding the monetary columns conversion into a specific .NET type.
- Querying a child records of the parent rows.
- Updating a record as a reaction to the transformation.
- Can be used as trigger.
- Manually override the default handler for the `Enumerations`.
- and many more...