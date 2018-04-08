RepoDb

A dynamic ORM library used to create an Entity-Based Repositories when accessing data from the database.

Please visit nuget.org for the documentation and package.
Link: https://www.nuget.org/packages/RepoDb

Updates (v1.0.6):
- Support dynamic query objects for QueryGroup
- MemoryCache (ICache) object
- Trace (ITrace) object
- Added Constant class
- Added support for Operation.Like and Operation.NotLike
- Allow Querying, Deleting, Updating by PrimaryKey (when the value is passed in the method of 'where' argument)
- Support of Operation.Between and Operation.NotBetween
- Support Operation.In and Operation.NotIn
- Optimized Statement Builder (SqlDbStatementBuilder)
- Injectable Statement Builder
- Support TOP and ORDER BY
	- Order.Ascending and Order.Descending
- Support StatementBuilderMapper, IStatementBuilderMapper, StatementBuilderMap, StatementBuilderMap
- Support ORDER BY parsing of the dynamic objects
- Added support for the new Operation.Any and Operation.All

Removed:
- EventNotifier class has been obsoleted by the Trace class