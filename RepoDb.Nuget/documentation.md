### Why RepoDb

 - Only 30 seconds to Setup.
 - Massive ORM Operation supports.
 - Seriously fast because of IL.
 - Less Code and Fluent.
 - Unique and Developer Friendly Expression Tree.

### Documentation
Click [here](https://repodb.readthedocs.io/en/latest/index.html) for the complete documentation.

### Snippets

Write less optimal codes.

Query Operation:
```
var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
var customer = repository.Query<Customer>(new { Id = 10045 });
```
Or
```
var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
var customer = repository.Query<Customer>(new
{
	LastName = { Operation = Operation.Like, Value = "Ander%" },
	top: 100,
	orderBy: OrderField.Parse(new { FirstName = Order.Ascending, BirtDate = Order.Descending })
});
```
Insert Operation:
```
var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
var customerId = repository.Insert(new Customer { Name = "Anna Fullerton", CreatedDate = DateTime.UtcNow });
```
Update Operation:
```
var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
var customer = repository.Query<Customer>(new { Id = 10045 });
customer.Name = "Anna Fullerton";
customer.UpdateDate = DateTime.UtcNow;
var affectedRows = repository.Update(customer);
```
Inline Operation:
```
var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
var affectedRows = repository.InlineUpdate<Customer>(new
{
	Name = "Anna Fullerton", UpdatedDate = DateTime.UtcNow
},
new { Id = 10045 };
```
Delete Operation:
```
var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
var affectedRows = repository.Update<Customer>(new { Id = 10045 };
```
Merge Operation:
```
var repository = new DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;");
var customer = repository.Query<Customer>(new { Id = 10045 });
customer.Name = "Anna Albert Fullerton";
var affectedRows = repository.Merge(customer, Field.Parse(new { customer.Id } ));
```