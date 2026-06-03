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

# SỬA TẠI ĐÂY: Copy từng file .csproj vào đúng thư mục con của nó để không bị mất cấu trúc
COPY backend/src/InventoryDashboard.Domain/InventoryDashboard.Domain.csproj backend/src/InventoryDashboard.Domain/
COPY backend/src/InventoryDashboard.Application/InventoryDashboard.Application.csproj backend/src/InventoryDashboard.Application/
COPY backend/src/InventoryDashboard.Infrastructure/InventoryDashboard.Infrastructure.csproj backend/src/InventoryDashboard.Infrastructure/
COPY backend/src/InventoryDashboard.API/InventoryDashboard.API.csproj backend/src/InventoryDashboard.API/

# Bây giờ lệnh restore này sẽ chạy thành công vì file đã nằm đúng chỗ
RUN dotnet restore backend/src/InventoryDashboard.API/InventoryDashboard.API.csproj

COPY backend/src/ ./backend/src/
WORKDIR /src/backend/src/InventoryDashboard.API
RUN dotnet publish "InventoryDashboard.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
COPY --from=frontend-build /app/frontend/dist ./wwwroot

# Render thường dùng cổng 8080 cho môi trường mạng, bạn có thể để 80 hoặc đổi thành 8080 nhé
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "InventoryDashboard.API.dll"]