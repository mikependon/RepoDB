RepoDb

A dynamic ORM .Net Library used to create an entity-based repository classes when accessing data from the database.

Please visit nuget.org for the documentation and package.
Link: https://www.nuget.org/packages/RepoDb

Updates (v1.0.9):
- Renamed ICache.Has to ICache.Contains
- Removed ICache.GetAll and implement the IEnumerable interface instead
- Optimized the mapping for SqlBulkCopy for 'BulkInsert'
