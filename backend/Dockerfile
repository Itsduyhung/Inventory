FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/InventoryDashboard.API/InventoryDashboard.API.csproj", "/src/InventoryDashboard.API/"]
COPY ["src/InventoryDashboard.Application/InventoryDashboard.Application.csproj", "/src/InventoryDashboard.Application/"]
COPY ["src/InventoryDashboard.Domain/InventoryDashboard.Domain.csproj", "/src/InventoryDashboard.Domain/"]
COPY ["src/InventoryDashboard.Infrastructure/InventoryDashboard.Infrastructure.csproj", "/src/InventoryDashboard.Infrastructure/"]

RUN dotnet restore "/src/InventoryDashboard.API/InventoryDashboard.API.csproj"

COPY src/ /src/
WORKDIR /src/InventoryDashboard.API
RUN dotnet publish "InventoryDashboard.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 80
ENV ASPNETCORE_ENVIRONMENT=Production
ENV PORT=10000
ENTRYPOINT ["sh", "-c", "dotnet InventoryDashboard.API.dll --urls http://0.0.0.0:$PORT"]
