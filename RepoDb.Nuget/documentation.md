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
```
public class Customer
{
	public int Id { get; set; }
	public string Name {get; set; }
	public CreatedDate { get; set; }
}
```
Create Shared-Repository via DbRepository:
```
public class NorthwindDbRepository : DbRepository<SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;")
{
}
```
Create Shared-Repository via BaseRepository:
```
public class CustomerRepository : BaseRepository<Customer, SqlConnection>(@"Server=.;Database=Northwind;Integrated Security=SSPI;")
{
}
```
Query Operation:
```
var repository = new NorthwindDbRepository<SqlConnection>();
var customer = repository.Query<Customer>(new { Id = 10045 });
```
Insert Operation:
```
var repository = new CustomerRepository();
var customerId = repository.Insert(new Customer
{
	Name = "Anna Fullerton",
	CreatedDate = DateTime.UtcNow
});
```
Update Operation:
```
var repository = new CustomerRepository();
var customer = repository.Quer>(new { Id = 10045 });
customer.Name = "Anna Fullerton";
customer.UpdateDate = DateTime.UtcNow;
var affectedRows = repository.Update(customer);
```
Inline Operation:
```
var repository = new CustomerRepository();
var affectedRows = repository.InlineUpdate(new
{
	Name = "Anna Fullerton", UpdatedDate = DateTime.UtcNow
},
new { Id = 10045 };
```
Delete Operation:
```
var repository = new CustomerRepository();
var affectedRows = repository.Update(new { Id = 10045 };
```
Merge Operation:
```
var repository = new CustomerRepository();
var customer = repository.Query(new { Id = 10045 });
customer.Name = "Anna Albert Fullerton";
var affectedRows = repository.Merge(customer,
	Field.Parse(new { customer.Id } ));
```