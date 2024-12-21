dotnet ef dbcontext scaffold "Host=my_host;Database=my_db;Username=my_user;Password=my_pw" Npgsql.EntityFrameworkCore.PostgreSQL

dotnet ef migrations add Init --verbose --project ./Infrastructure//Infrastructure.csproj   --startup-project ./Api/Api.csproj
dotnet ef database update --verbose --project ./Infrastructure//Infrastructure.csproj   --startup-project ./Api/Api.csproj
