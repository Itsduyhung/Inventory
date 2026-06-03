# Build frontend
FROM node:20-alpine AS frontend-build
WORKDIR /app/frontend
COPY frontend/package*.json ./
RUN npm ci
COPY frontend/ .
ARG VITE_API_BASE_URL=
ENV VITE_API_BASE_URL=$VITE_API_BASE_URL
RUN npm run build

# Build backend
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY backend/src/InventoryDashboard.Application/InventoryDashboard.Application.csproj backend/src/InventoryDashboard.Domain/InventoryDashboard.Domain.csproj backend/src/InventoryDashboard.Infrastructure/InventoryDashboard.Infrastructure.csproj backend/src/InventoryDashboard.API/InventoryDashboard.API.csproj ./
RUN dotnet restore backend/src/InventoryDashboard.API/InventoryDashboard.API.csproj
COPY backend/src/ ./backend/src/
WORKDIR /src/backend/src/InventoryDashboard.API
RUN dotnet publish "InventoryDashboard.API.csproj" -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
COPY --from=frontend-build /app/frontend/dist ./wwwroot
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "InventoryDashboard.API.dll"]
