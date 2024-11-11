# Start instances of all database engines in clean docker instances
docker compose down
docker compose up -d

# Build all test project
foreach ($test in $tests) {
  dotnet build $test -f $framework
  if(-not $?){
    Exit(1)
  }
}

# SqlServer SA
$env:REPODB_SQLSERVER_CONSTR_MASTER="Server=tcp:127.0.0.1,41433;Database=master;User ID=sa;Password=ddd53e85-b15e-4da8-91e5-a7d3b00a0ab2;TrustServerCertificate=True;"
# RepoDb common integration tests
$env:REPODB_SQLSERVER_CONSTR_REPODB="Server=tcp:127.0.0.1,41433;Database=RepoDb;User ID=sa;Password=ddd53e85-b15e-4da8-91e5-a7d3b00a0ab2;TrustServerCertificate=True;"
# SqlServer Tests
$env:REPODB_SQLSERVER_CONSTR_REPODBTEST="Server=tcp:127.0.0.1,41433;Database=RepoDbTest;User ID=sa;Password=ddd53e85-b15e-4da8-91e5-a7d3b00a0ab2;TrustServerCertificate=True;"
$env:REPODB_POSTGRESQL_CONSTR_POSTGRESDB="Server=127.0.0.1;Port=45432;Database=postgres;User Id=postgres;Password=ddd53e85-b15e-4da8-91e5-a7d3b00a0ab2;"
$env:REPODB_POSTGRESQL_CONSTR="Server=127.0.0.1;Port=45432;Database=RepoDb;User Id=postgres;Password=ddd53e85-b15e-4da8-91e5-a7d3b00a0ab2;"
$env:REPODB_POSTGRESQL_CONSTR_BULK="Server=127.0.0.1;Port=45432;Database=RepoDbBulk;User Id=postgres;Password=ddd53e85-b15e-4da8-91e5-a7d3b00a0ab2;"
$env:REPODB_MYSQL_CONSTR_SYS="Server=127.0.0.1;Port=43306;Database=sys;User ID=root;Password=ddd53e85-b15e-4da8-91e5-a7d3b00a0ab2;"
# Mysql Tests
$env:REPODB_MYSQL_CONSTR_REPODB="Server=127.0.0.1;Port=43306;Database=RepoDb;User ID=root;Password=ddd53e85-b15e-4da8-91e5-a7d3b00a0ab2;"
# MySqlConnector Tests
$env:REPODB_MYSQL_CONSTR_REPODBTEST="Server=127.0.0.1;Port=43306;Database=RepoDbTest;User ID=root;Password=ddd53e85-b15e-4da8-91e5-a7d3b00a0ab2;"

dotnet test $test -f $framework --no-build -p:TestingPlatformShowTestsFailure=true -p:TestingPlatformCaptureOutput=false
if(-not $?){
  Exit(1)
}
