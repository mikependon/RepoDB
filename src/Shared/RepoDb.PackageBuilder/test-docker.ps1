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

# Connection Strings
$env:REPODB_SQLSVR_CONSTR_MASTER="Server=tcp:127.0.0.1,41433;Database=master;User ID=sa;Password=RepoDB2026;TrustServerCertificate=True;"
$env:REPODB_SQLSVR_CONSTR="Server=tcp:127.0.0.1,41433;Database=RepoDb;User ID=sa;Password=RepoDB2026;TrustServerCertificate=True;"
$env:REPODB_PGSQL_CONSTR_POSTGRES="Server=127.0.0.1;Port=45432;Database=postgres;User Id=postgres;Password=RepoDB2026;"
$env:REPODB_PGSQL_CONSTR="Server=127.0.0.1;Port=45432;Database=RepoDb;User Id=postgres;Password=RepoDB2026;"
$env:REPODB_MYSQL_CONSTR_SYSTEM="Server=127.0.0.1;Port=43306;Database=sys;User ID=root;Password=RepoDB2026;"
$env:REPODB_MYSQL_CONSTR="Server=127.0.0.1;Port=43306;Database=RepoDb;User ID=root;Password=RepoDB2026;"

dotnet test $test -f $framework --no-build -p:TestingPlatformShowTestsFailure=true -p:TestingPlatformCaptureOutput=false
if(-not $?){
  Exit(1)
}
