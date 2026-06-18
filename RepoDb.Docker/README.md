# SQL Server

Follow the steps from the official Docker website for [SQL Server](https://hub.docker.com/r/microsoft/mssql-server).

Alternatively, run the command below.

```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=RepoDB2026" -e "MSSQL_PID=Evaluation" -p 1433:1433  --name sql2025 --hostname sql2025 -d mcr.microsoft.com/mssql/server:2025-latest
```