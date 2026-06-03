# Intelligent Inventory Dashboard

Real-time dealership inventory dashboard with aging stock identification and actionable insights.

## Architecture

### Backend (ASP.NET 8 — Clean Architecture)

```
backend/src/
├── InventoryDashboard.Domain/        # Entities, repository interfaces
├── InventoryDashboard.Application/   # DTOs, services, use cases
├── InventoryDashboard.Infrastructure/# EF Core, PostgreSQL, repositories
└── InventoryDashboard.API/           # REST controllers, DI, CORS
```

### Frontend (React + TypeScript — Clean Architecture)

```
frontend/src/
├── common/       # Types, constants, utilities (no UI)
├── api/          # Axios client & API mapping
├── components/   # Reusable UI components (may use libraries)
└── features/     # Feature modules (inventory dashboard)
```

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/)
- PostgreSQL 17 (pgAdmin 4) running on `localhost:5432`
- User: `postgres` / Password: `2402`

## Database Setup

The API uses **Code First** with EF Core migrations. On first run it will:

1. Create database `InventoryDb` automatically
2. Apply migrations
3. Seed sample vehicle inventory

Connection string (in `appsettings.json`):

```
Host=localhost;Port=5432;Database=InventoryDb;Username=postgres;Password=2402
```

Manual migration (optional):

```bash
cd backend
dotnet tool restore
dotnet tool run dotnet-ef database update --project src/InventoryDashboard.Infrastructure --startup-project src/InventoryDashboard.API
```

## Authentication

Login page (`/login`) for everyone: **Sign In**, **Forgot / Reset password** only.

- **Staff accounts** are created by **Admin** via **+ Staff account** in the admin header (`POST /api/admin/users/staff`).
- **JWT access token** (60 min) + **refresh token** (7 days), auto-refresh on 401
- `POST /api/auth/login`, `refresh`, `forgot-password`, `reset-password`
- Forgot password returns a reset token in the API response when `Auth:ReturnResetTokenInResponse` is `true` (demo; no email server)

## Login Accounts

| Role | Username | Password |
|------|----------|----------|
| Admin | `admin` | `admin123` |
| Staff | `staff` | `staff123` |

### Admin
- Tạo tài khoản **Staff** (username, email, mật khẩu tạm)
- **Staff sales** — xem staff nào đã bán xe nào (lọc theo staff, xem chi tiết xe)
- CRUD xe (Create, Read, Update, **Disable** — không xóa cứng)
- Phân trang, tìm kiếm, xem chi tiết xe (màu, mileage, engine, mô tả...)
- Trường **Note** thay cho status cũ

### Staff (separate UI — teal sidebar, card grid)
1. **Inventory** — Card layout, filters, record sale, vehicle details
2. **Sales history** — Timeline-style sale cards with vehicle detail

## Run Backend

```bash
cd backend/src/InventoryDashboard.API
dotnet run --launch-profile http
```

API: http://localhost:5272  
Swagger: http://localhost:5272/swagger

## Run Frontend

```bash
cd frontend
npm install
npm run dev
```

Dashboard: http://localhost:5173

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/vehicles` | List vehicles (filters: `make`, `model`, `minAgeDays`, `maxAgeDays`, `agingOnly`) |
| GET | `/api/vehicles/summary` | Inventory summary stats |
| GET | `/api/vehicles/{id}` | Get vehicle by ID |
| PUT | `/api/vehicles/{id}/aging-action` | Log action for aging stock (>90 days) |

## Features

1. **Inventory Visualization** — Filterable vehicle list by make, model, and age (days on lot)
2. **Aging Stock Identification** — Vehicles in inventory >90 days are highlighted
3. **Actionable Insights** — Managers can log and persist status/actions for aging vehicles

## Tech Stack

- **Backend:** ASP.NET 8, EF Core 8, PostgreSQL (Npgsql)
- **Frontend:** React 19, TypeScript, Vite, TanStack Query, Axios
