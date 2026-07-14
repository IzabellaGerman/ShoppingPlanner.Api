FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY ShoppingPlanner.slnx .
COPY src/ShoppingPlanner.Api/ShoppingPlanner.Api.csproj src/ShoppingPlanner.Api/
COPY tests/ShoppingPlanner.Api.Tests/ShoppingPlanner.Api.Tests.csproj tests/ShoppingPlanner.Api.Tests/
RUN dotnet restore src/ShoppingPlanner.Api/ShoppingPlanner.Api.csproj
COPY . .
RUN dotnet publish src/ShoppingPlanner.Api/ShoppingPlanner.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ShoppingPlanner.Api.dll"]