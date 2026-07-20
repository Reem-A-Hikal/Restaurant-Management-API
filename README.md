@ -1,116 +1 @@
# 🍽️ Restro — Restaurant Management System (Backend API)

> A production-style restaurant management platform backend, built with **ASP.NET Core 9**
> using **Clean Architecture** and **Domain-Driven Design (DDD)**. Handles the full
> operational lifecycle of a restaurant: menu management, order processing, kitchen
> workflow, payments, and delivery dispatch — with role-based access for Admins, Chefs,
> Delivery Personnel, and Customers.

## 📌 Project Status

Actively developed. Core modules (Users, Categories, Products, Orders, Payments,
Delivery, Reviews, Addresses, Dashboard) are functionally complete and wired to a real
Angular frontend. See [Roadmap](#roadmap--known-issues) below for what's next.

## Tech Stack

| Layer        | Technology                                                 |
| ------------ | ---------------------------------------------------------- |
| Runtime      | .NET 9 / ASP.NET Core Web API                              |
| Data Access  | Entity Framework Core 9 (SQL Server, Lazy Loading Proxies) |
| Auth         | ASP.NET Core Identity + JWT Bearer                         |
| Mapping      | AutoMapper                                                 |
| API Docs     | Swashbuckle (Swagger/OpenAPI) with XML comments            |
| Architecture | Clean Architecture (4-layer) + Domain-Driven Design        |

## 🏗️ Architecture

The solution follows **Clean Architecture**: dependencies point inward, and the
business logic at the core knows nothing about the database, the web framework, or any
external concern.

```
┌─────────────────────────────────────────────────────┐
│  Rest.API            Controllers · Middleware · DI   │  ← outermost
├─────────────────────────────────────────────────────┤
│  Rest.Infrastructure  EF Core · Repositories · Migrations
├─────────────────────────────────────────────────────┤
│  Rest.Application     Services · DTOs · Interfaces    │
├─────────────────────────────────────────────────────┤
│  Rest.Domain          Entities · Enums · Exceptions   │  ← innermost, zero dependencies
└─────────────────────────────────────────────────────┘
```

- **Rest.Domain** has no project references at all — pure C# business rules
- **Rest.Application** depends only on Domain, and defines _interfaces_
  (`IRepositories`, `IServices`) that outer layers implement
- **Rest.Infrastructure** implements those interfaces (EF Core repositories, image
  upload service, auth service) and depends on both Application and Domain
- **Rest.API** is the composition root — this is the only place where everything gets
  wired together (`Program.cs`)

### Domain-Driven Design in practice

- Entities use **private setters** and expose behavior through **domain methods**
  (`Order.Confirm()`, `Product.Deactivate()`, `Category.Archive()`) instead of public
  property setters — invalid states are structurally impossible to create
- **Factory methods** (`Entity.Create(...)`) enforce validation at construction time
- Stateful entities (`Order`, `Payment`, `Delivery`) implement explicit **state
  machines** with an allowed-transitions map, rejecting invalid status changes with a
  domain exception rather than silently allowing them
- **No Data Annotations** on entities — validation lives in the domain, and database
  constraints are configured separately via **Fluent API**
- Custom domain exceptions (`NotFoundException`, `BusinessException`,
  `ValidationException`, `ForbiddenException`) are caught by a global
  `ExceptionHandlingMiddleware` and translated into a consistent API response shape

### Design Patterns Used

- **Repository + Unit of Work** — every aggregate has its own repository interface;
  `IUnitOfWork` composes them and coordinates `SaveChangesAsync()`
- **Strategy Pattern** — role-specific user creation/update/enrichment logic
  (`IRoleStrategy` per role: Admin, Chef, DeliveryPerson, Customer) instead of
  conditional branching scattered across services
- **State Machine** — Order, Payment, and Delivery each encapsulate their own valid
  transitions

## ✨ Features

- 🔐 **JWT authentication** with role-based authorization (Admin / Chef /
  DeliveryPerson / Customer)
- 👥 **User management** — role-specific fields (Chef specialization, DeliveryPerson
  vehicle info), soft-delete via status rather than hard delete
- 🍔 **Menu management** — categories and products with discounts, promotion flags,
  availability status, and image uploads (with magic-bytes validation against spoofed
  file extensions)
- 📦 **Full order lifecycle** — New → Confirmed → Preparing → Ready → Out for Delivery
  → Delivered / Canceled, with price snapshots taken at order time (so later price
  changes never affect historical orders)
- 💳 **Payments** — supports multiple payment attempts per order (a failed attempt
  doesn't block retrying), refund flow, Cash and Stripe methods
- 🚚 **Delivery dispatch** — auto-assign or manual override to available delivery
  personnel, live location updates, full delivery history per order
- ⭐ **Reviews** — customers can review delivered orders only, one review per order
- 📍 **Multiple addresses per customer** with a single default
- 📊 **Aggregated dashboard endpoint** — KPIs, order status breakdown, revenue trend,
  and top-selling dishes, computed server-side in one call for the Admin dashboard

## Roadmap / Known Issues

- [ ] Add refresh-token support (currently any 401 forces a full re-login)

## Running Locally

```bash
dotnet restore
dotnet ef database update --project Rest.Infrastructure --startup-project Rest.API
dotnet run --project Rest.API
```

Swagger UI: `https://localhost:<port>/swagger`

Default seeded admin account: `admin@restora.com` / `Admin@123456`

## 🔗 Related

Frontend (Angular 19): see the companion `Restaurant-Management-UI` repository.
# Restaurant Management API