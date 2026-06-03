# ==========================================
# STAGE 1: BUILD FRONTEND (Vite)
# ==========================================
FROM node:20-alpine AS frontend-build
WORKDIR /app/frontend
COPY frontend/package*.json ./
RUN npm ci
COPY frontend/ .
ARG VITE_API_BASE_URL=
ENV VITE_API_BASE_URL=$VITE_API_BASE_URL
RUN npm run build

# ==========================================
# STAGE 2: BUILD BACKEND (.NET)
# ==========================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Sao chép và giữ đúng cấu trúc các tầng dự án của bạn
COPY backend/src/InventoryDashboard.Domain/InventoryDashboard.Domain.csproj backend/src/InventoryDashboard.Domain/
COPY backend/src/InventoryDashboard.Application/InventoryDashboard.Application.csproj backend/src/InventoryDashboard.Application/
COPY backend/src/InventoryDashboard.Infrastructure/InventoryDashboard.Infrastructure.csproj backend/src/InventoryDashboard.Infrastructure/
COPY backend/src/InventoryDashboard.API/InventoryDashboard.API.csproj backend/src/InventoryDashboard.API/

# Tiến hành khôi phục các thư viện NuGet
RUN dotnet restore backend/src/InventoryDashboard.API/InventoryDashboard.API.csproj

# Copy toàn bộ code và thực hiện publish ra file chạy chính thức
COPY backend/src/ ./backend/src/
WORKDIR /src/backend/src/InventoryDashboard.API
RUN dotnet publish "InventoryDashboard.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ==========================================
# STAGE 3: RUNTIME IMAGE (GOM CHUNG CHẠY THỰC TẾ)
# ==========================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy-chiseled AS runtime
WORKDIR /app

# Tắt triệt để cơ chế File Watcher trên môi trường Linux nhằm chặn lỗi Status 139
ENV DOTNET_USE_POLLING_FILE_WATCHER=1
ENV ASPNETCORE_HOSTINGSTARTUPASSEMBLIES=""

# Copy sản phẩm Backend .NET đã build
COPY --from=build /app/publish .

# Copy toàn bộ file tĩnh của Frontend (Vite) vào thư mục wwwroot của .NET
COPY --from=frontend-build /app/frontend/dist ./wwwroot

# Cấu hình cổng chạy mặc định tương thích tốt với Render
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "InventoryDashboard.API.dll"]