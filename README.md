# SmartClinicQueue 🏥

## Smart Clinic Queue Management System

**SmartClinicQueue** is an ASP.NET Core Web API designed to manage clinic queues digitally and organize the interaction between **patients, doctors, and receptionists**.

The project focuses on replacing traditional clinic waiting queues with a structured digital workflow that provides secure queue management, real-time updates, automatic queue cleanup, and a scalable backend architecture.

> **Project Status:** In Development
> The project is being continuously extended to cover more real-world clinic and doctor requirements and provide a complete clinic queue management workflow.

---

# 📌 Problem Statement

Traditional clinic queue management can lead to several problems:

* Patients may not know their current position in the queue.
* Receptionists need to manage queues manually.
* Doctors do not have a structured way to call and manage the next patient.
* Patients with different priorities may not be ordered correctly.
* Queue positions can become outdated when the queue changes.
* Old waiting tickets may remain active after the clinic day ends.
* APIs need consistent error handling and response formats.
* High-frequency authentication endpoints need protection against excessive requests.
* Real-time queue changes should be communicated to connected clients without continuously polling the server.

SmartClinicQueue addresses these problems through a centralized backend that manages the complete queue lifecycle.

---

# 💡 Solution

The system provides a digital queue workflow:

```text
Patient arrives at the clinic
        ↓
Receptionist selects:
Patient + Doctor + Clinic + Priority
        ↓
Patient joins the doctor's queue
        ↓
Ticket is created with Waiting status
        ↓
Doctor calls the next patient
        ↓
Called
        ↓
Consultation starts
        ↓
InConsultation
        ↓
Consultation completes
        ↓
Completed
```

The system also handles:

* Ticket cancellation
* Patient no-show
* Queue priority
* Dynamic queue positions
* Real-time queue updates using SignalR
* Automatic cleanup of old waiting tickets using Hangfire
* Authentication and role-based authorization
* Rate limiting
* Centralized exception handling
* Request logging
* Consistent API responses

---

# 🎯 Main Features

## 1. Authentication & Authorization

The system uses:

* ASP.NET Core Identity
* JWT Authentication
* Role-Based Authorization

Supported roles:

* **Patient**
* **Doctor**
* **Receptionist**

### Registration

New users are registered as **Patients by default**.

Users do not select their role during registration.

### JWT

After successful authentication, the API returns a JWT containing the authenticated user's information and role.

Protected endpoints use:

```csharp
[Authorize]
```

or role-based authorization such as:

```csharp
[Authorize(Roles = Roles.Doctor)]
```

---

# 👥 User Roles

## Patient

Patients can:

* Authenticate into the system.
* View their active queue ticket.
* See their ticket number.
* See their current queue status.
* See their current position.
* See the number of patients before them.
* See the currently serving ticket.

Patients do not select their queue priority.

---

## Receptionist

The receptionist is responsible for registering patients into the queue.

The receptionist selects:

* Patient
* Doctor
* Clinic
* Priority

and then creates the queue ticket.

Receptionists can also:

* Cancel waiting tickets.

---

## Doctor

Doctors can:

* View their active queue.
* Call the next patient.
* Start a consultation.
* Complete a consultation.
* Mark a called patient as a no-show.

---

# 🎫 Queue Management

Each queue ticket contains:

* Ticket Number
* Patient
* Doctor
* Clinic
* Priority
* Status
* Created At
* Called At
* Started At
* Completed At

### Queue Priority

```text
Normal = 1
Urgent = 2
Emergency = 3
```

Higher priority patients are served first.

Within the same priority level, earlier tickets are served first.

The queue is ordered using:

```csharp
.OrderByDescending(t => t.Priority)
.ThenBy(t => t.CreatedAt)
```

---

# 🔄 Queue Ticket Lifecycle

A ticket can move through different states:

```text
Waiting
   ↓
Called
   ↓
InConsultation
   ↓
Completed
```

Other possible terminal states include:

```text
Waiting → Cancelled

Called → NoShow
```

The allowed transitions are controlled by the corresponding CQRS commands.

### Queue Commands

* `JoinQueueCommand`
* `CallNextPatientCommand`
* `StartConsultationCommand`
* `CompleteConsultationCommand`
* `CancelTicketCommand`
* `MarkNoShowCommand`

---

# 📊 Dynamic Queue Position

The patient's queue position is **not stored in the database**.

Instead, it is calculated dynamically based on the current queue ordering.

This prevents stale position values when:

* A patient is called.
* A patient is cancelled.
* A consultation starts.
* A patient is marked as no-show.
* A higher-priority patient joins the queue.

For example:

```text
Patient A → Emergency
Patient B → Urgent
Patient C → Normal
```

The queue is calculated dynamically as:

```text
1. Patient A
2. Patient B
3. Patient C
```

---

# ⚡ Real-Time Queue Updates with SignalR

The project uses **ASP.NET Core SignalR** to provide real-time queue notifications.

SignalR is used to notify connected clients whenever the queue changes.

It does **not** calculate the queue position.

The backend query remains responsible for calculating the actual queue state.

### Flow

```text
Doctor calls next patient
        ↓
Database is updated
        ↓
SignalR sends QueueUpdated
        ↓
Connected client receives notification
        ↓
Client requests the latest queue information
        ↓
Backend recalculates the current position
```

This avoids relying on constant polling from the client.

---

## SignalR Hub

The project contains:

```text
/hubs/queue
```

Clients can join a doctor-specific group:

```text
doctor-{doctorId}
```

For example:

```text
doctor-5
```

When the queue changes, the notification is sent to the corresponding doctor queue group.

The hub provides:

* `JoinDoctorQueue`
* `LeaveDoctorQueue`

---

# 🧩 CQRS with MediatR

The project uses **CQRS (Command Query Responsibility Segregation)** for queue operations.

The main idea is to separate:

### Commands

Operations that change data:

```text
JoinQueue
CallNextPatient
StartConsultation
CompleteConsultation
CancelTicket
MarkNoShow
```

### Queries

Operations that retrieve data:

```text
GetQueue
GetMyTicket
```

MediatR is used to dispatch commands and queries to their corresponding handlers.

Example:

```text
QueueController
      ↓
IMediator.Send()
      ↓
Command / Query
      ↓
Handler
      ↓
Repository / Service
      ↓
Database
```

This keeps controllers thin and moves business logic into dedicated handlers.

---

# 🏗️ Clean Architecture

The project follows a **Clean Architecture** approach.

The solution is separated into:

```text
SmartClinicQueue
│
├── SmartClinicQueue.Domain
├── SmartClinicQueue.Application
├── SmartClinicQueue.Infrastructure
└── SmartClinicQueue.API
```

## Domain Layer

Contains the core business entities and domain concepts.

Examples:

* `ApplicationUser`
* `ApplicationRole`
* `Doctor`
* `Clinic`
* `QueueTicket`
* `RequestLog`
* Queue enums
* Role constants

The Domain layer remains independent of infrastructure technologies.

---

## Application Layer

Contains application-level business logic and abstractions.

Examples:

* CQRS Commands
* CQRS Queries
* MediatR Handlers
* DTOs
* Service interfaces
* Repository interfaces
* Background job abstractions
* Queue notification abstraction
* API response abstraction

Examples:

```text
IQueueTicketRepository
IAuthService
IJwtService
IQueueNotificationService
IQueueCleanupService
IBackgroundJobScheduler
```

The Application layer defines **what the system needs**, without depending directly on external infrastructure implementations.

---

## Infrastructure Layer

Contains implementations that depend on external technologies.

Examples:

* Entity Framework Core
* SQL Server
* Repositories
* Hangfire implementation
* Queue cleanup implementation
* Database persistence

Examples:

```text
QueueTicketRepository
QueueCleanupService
HangfireBackgroundJobScheduler
```

The Infrastructure layer implements the abstractions defined by Application.

---

## API Layer

Contains the HTTP/API-specific components.

Examples:

* Controllers
* Middleware
* SignalR Hubs
* SignalR notification implementation
* Dependency Injection configuration
* Authentication configuration
* Authorization
* Rate Limiting
* Swagger/OpenAPI configuration

---

# 🗄️ Entity Framework Core

The project uses:

* Entity Framework Core
* SQL Server
* ASP.NET Core Identity

The main application DbContext inherits from:

```csharp
IdentityDbContext<ApplicationUser, ApplicationRole, int>
```

This allows Identity and application entities to use the same database context.

---

# 📦 Repository Pattern

The project uses a Generic Repository abstraction along with a specialized queue repository.

The queue repository provides operations such as:

```text
GetActiveTicketByPatientAsync
GetLastTicketNumberAsync
GetActiveQueueByDoctorAsync
GetCurrentServingAsync
GetWaitingTicketsFromPreviousDaysAsync
```

The specialized repository contains queue-specific data access logic instead of putting database queries directly inside controllers.

---

# 🔐 Security

The API includes multiple security mechanisms.

## JWT Authentication

JWT Bearer authentication protects secured endpoints.

## Role-Based Authorization

Different operations are restricted based on the user's role.

Examples:

```text
Receptionist → Join Queue
Doctor       → Manage Queue
Patient      → View Own Ticket
```

## Password Security

ASP.NET Core Identity handles password hashing and validation.

## Authorization Header Protection

The request logging middleware intentionally excludes the `Authorization` header from stored request headers.

This prevents JWT tokens from being stored in request logs.

---

# 🚦 Rate Limiting

The project uses ASP.NET Core Rate Limiting to protect authentication endpoints.

The authentication policy is:

```text
AuthPolicy
Permit Limit: 5 requests
Window: 1 minute
Queue Limit: 0
```

It is applied to the authentication controller:

```csharp
[EnableRateLimiting("AuthPolicy")]
```

This helps reduce excessive requests against:

```text
POST /api/Auth/register
POST /api/Auth/login
```

---

# 🛡️ Centralized Exception Handling

The project uses a custom:

```text
ExceptionHandlingMiddleware
```

Instead of handling exceptions separately in every controller, exceptions are intercepted centrally.

The middleware maps common exceptions to appropriate HTTP responses.

Examples:

```text
KeyNotFoundException
        ↓
404 Not Found

InvalidOperationException
        ↓
400 Bad Request

ArgumentException
        ↓
400 Bad Request

UnauthorizedAccessException
        ↓
403 Forbidden

DbUpdateException
        ↓
400 Bad Request

Unhandled Exception
        ↓
500 Internal Server Error
```

Database exceptions are also translated into more meaningful messages for cases such as:

* Foreign key violations
* Duplicate/unique constraint violations
* Other database save failures

Development environments can expose additional exception details for debugging.

---

# 📋 Unified API Response

All API responses follow a consistent response structure using:

```text
ApiResponse<T>
```

Controllers do not return unrelated response formats for every endpoint.

Examples:

```csharp
ApiResponse<int>.Success(...)
```

```csharp
ApiResponse<MyTicketDto>.Success(...)
```

```csharp
ApiResponse<bool>.Success(...)
```

and failures use:

```csharp
ApiResponse<string>.Failure(...)
```

The centralized exception middleware also uses the same `ApiResponse` structure.

This provides a consistent contract for frontend clients.

---

# 📝 Request Logging

The project includes a custom:

```text
RequestLoggingMiddleware
```

It records important information about incoming API requests.

Logged information includes:

* HTTP Method
* Request URL
* Query String
* Relevant Headers
* IP Address
* HTTP Status Code
* Response Time
* Authenticated User ID
* Creation timestamp

The middleware measures request execution time using `Stopwatch`.

Example:

```text
Request
   ↓
Start Stopwatch
   ↓
Execute API
   ↓
Stop Stopwatch
   ↓
Store RequestLog
```

The `Authorization` header is excluded from stored headers to avoid logging JWT tokens.

If request logging itself fails, the failure is logged without breaking the original API request.

---

# ⏰ Background Jobs with Hangfire

The project uses **Hangfire** for recurring background processing.

## Why Hangfire?

Some operations should happen automatically without requiring a user or API request to trigger them.

The implemented background job performs daily queue cleanup.

## Daily Queue Cleanup

At the end of the queue day, patients who still have tickets in:

```text
Waiting
```

from previous days should not remain in the active queue.

Hangfire runs a recurring job that:

1. Finds previous days' waiting tickets.
2. Changes their status from:

```text
Waiting → NoShow
```

3. Saves the changes to the database.

Old tickets are **not deleted**, so queue history remains available.

## Background Job Architecture

The project separates scheduling from business logic.

```text
IBackgroundJobScheduler
        ↓
HangfireBackgroundJobScheduler
        ↓
Hangfire
        ↓
IQueueCleanupService
        ↓
QueueCleanupService
        ↓
QueueTicketRepository
```

This keeps Hangfire-specific implementation inside Infrastructure instead of coupling the Application layer directly to Hangfire.

The recurring job uses a stable identifier:

```text
daily-queue-cleanup
```

so Hangfire can create or update the scheduled job.

---

# 🔄 Ticket Numbering

Ticket numbers are generated per doctor per day.

The repository retrieves the latest ticket number created for the current day:

```text
Max(TicketNumber)
```

If no ticket exists for the current day:

```text
Max = 0
```

The first ticket therefore receives:

```text
Ticket #1
```

There is no need to manually reset the counter because the query is scoped to the current day.

---

# 📡 API Endpoints

## Authentication

### Register

```http
POST /api/Auth/register
```

Registers a new user as a Patient.

### Login

```http
POST /api/Auth/login
```

Authenticates a user and returns a JWT token.

Authentication endpoints are protected with the `AuthPolicy` rate limiter.

---

# Queue Endpoints

## Join Queue

```http
POST /api/Queue/join
```

**Role:** Receptionist

Creates a new queue ticket for:

* Patient
* Doctor
* Clinic
* Priority

---

## Get My Ticket

```http
GET /api/Queue/my-ticket/{patientId}
```

**Role:** Patient

Returns the patient's active ticket and current queue information.

Includes:

* Ticket ID
* Ticket Number
* Priority
* Status
* Position
* Patients Before You
* Now Serving Ticket Number

---

## Get Doctor Queue

```http
GET /api/Queue/doctor/{doctorId}
```

**Role:** Doctor

Returns the active waiting queue for the doctor.

The queue is ordered by:

1. Priority descending
2. Creation time ascending

---

## Call Next Patient

```http
POST /api/Queue/doctor/{doctorId}/call-next
```

**Role:** Doctor

Calls the next patient according to the queue ordering.

The ticket changes:

```text
Waiting → Called
```

---

## Start Consultation

```http
POST /api/Queue/{ticketId}/start-consultation
```

**Role:** Doctor

Changes:

```text
Called → InConsultation
```

---

## Complete Consultation

```http
POST /api/Queue/{ticketId}/complete-consultation
```

**Role:** Doctor

Changes:

```text
InConsultation → Completed
```

---

## Cancel Ticket

```http
POST /api/Queue/{ticketId}/cancel
```

**Role:** Receptionist

Changes:

```text
Waiting → Cancelled
```

---

## Mark Patient as No-Show

```http
POST /api/Queue/{ticketId}/no-show
```

**Role:** Doctor

Changes:

```text
Called → NoShow
```

---

# 📋 Current Queue Workflow

```text
                  ┌──────────────┐
                  │ Receptionist │
                  └──────┬───────┘
                         │
                         ▼
                 Create Queue Ticket
                         │
                         ▼
                     [Waiting]
                      /     \
                     /       \
                    ▼         ▼
               [Cancelled]   Doctor
                              │
                              ▼
                           [Called]
                           /      \
                          /        \
                         ▼          ▼
                    [NoShow]   [InConsultation]
                                    │
                                    ▼
                                [Completed]
```

---

# 🔔 Real-Time Workflow

When the queue changes:

```text
Queue Command
     ↓
Database Update
     ↓
QueueNotificationService
     ↓
SignalR
     ↓
QueueUpdated Event
     ↓
Connected Client
```

The client can then request the latest queue data.

This keeps queue information synchronized without requiring continuous polling.

---

# 🧱 Main Components

| Component             | Responsibility                        |
| --------------------- | ------------------------------------- |
| ASP.NET Core Web API  | Exposes HTTP endpoints                |
| ASP.NET Core Identity | User management and password security |
| JWT                   | Authentication                        |
| Role Authorization    | Controls access to operations         |
| EF Core               | Database access                       |
| SQL Server            | Data persistence                      |
| Generic Repository    | Common data access                    |
| Queue Repository      | Queue-specific queries                |
| MediatR               | CQRS request dispatching              |
| CQRS                  | Separates commands and queries        |
| SignalR               | Real-time queue notifications         |
| Hangfire              | Background and recurring jobs         |
| Rate Limiting         | Protects authentication endpoints     |
| Middleware            | Centralized request processing        |
| Swagger/OpenAPI       | API documentation and testing         |
| ApiResponse           | Unified API response structure        |

---

# 🧰 Technologies

* C#
* .NET / ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* ASP.NET Core Identity
* JWT Bearer Authentication
* MediatR
* CQRS
* SignalR
* Hangfire
* ASP.NET Core Rate Limiting
* Swagger / OpenAPI
* Clean Architecture
* Repository Pattern
* Dependency Injection
* Middleware

---

# 📁 Architecture Overview

```text
SmartClinicQueue.API
│
├── Controllers
│   ├── AuthController
│   └── QueueController
│
├── Hubs
│   └── QueueHub
│
├── Middleware
│   ├── ExceptionHandlingMiddleware
│   └── RequestLoggingMiddleware
│
└── Services
    └── QueueNotificationService


SmartClinicQueue.Application
│
├── Common
│   └── ApiResponse
│
├── DTOs
│
├── Features
│   └── Queue
│       ├── Commands
│       └── Queries
│
└── Interfaces
    ├── IRepositories
    └── IServices


SmartClinicQueue.Domain
│
├── Entities
├── Enums
└── Constants


SmartClinicQueue.Infrastructure
│
├── Persistence
│
├── Repositories
│
└── Services
    ├── QueueCleanupService
    └── HangfireBackgroundJobScheduler
```

---

# 🔐 Security Architecture

```text
Client
   ↓
JWT Authentication
   ↓
Role Authorization
   ↓
Rate Limiting
   ↓
Controller
   ↓
Application Logic
   ↓
Database
```

Errors are handled centrally through middleware, while requests are logged through the request logging middleware.

---

# 📈 Future Development

The project is still under development and will continue to evolve to satisfy broader real-world clinic requirements.

Planned areas include:

* Expanding clinic and doctor management.
* More complete patient management.
* More detailed doctor workflows.
* Additional queue management scenarios.
* Improved real-time patient/doctor screens.
* More comprehensive reporting and dashboard functionality.
* Additional operational features.
* Further validation and business rules.
* Expanded testing coverage.
* Production deployment and operational configuration.

The architecture is designed to make these additions easier without tightly coupling the different parts of the system.

---

# 🎯 Project Goals

The main goals of SmartClinicQueue are:

* Digitize traditional clinic queues.
* Reduce manual queue management.
* Provide clear responsibilities for patients, doctors, and receptionists.
* Keep queue ordering dynamic and accurate.
* Provide real-time queue updates.
* Automatically handle expired waiting tickets.
* Protect authentication endpoints.
* Provide consistent API responses.
* Centralize error handling.
* Maintain request logging.
* Keep business logic separated from infrastructure concerns.
* Build a maintainable and scalable backend architecture.

---

# 🚀 Getting Started

## Prerequisites

Make sure you have:

* .NET SDK
* SQL Server
* Visual Studio or another compatible IDE
* SQL Server connection
* Required NuGet packages

---

## Configuration

Configure the database connection and JWT settings through the application's configuration/user secrets.

Example structure:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "...",
    "HangfireConnection": "..."
  },
  "Jwt": {
    "Key": "...",
    "Issuer": "SmartClinicQueue",
    "Audience": "SmartClinicQueueClient",
    "DurationInDays": 1
  }
}
```

> Secrets and production credentials should not be committed to source control.

---

# ▶️ Running the Project

1. Configure the SQL Server connection.
2. Configure JWT settings securely.
3. Configure the Hangfire database connection.
4. Apply EF Core migrations.
5. Run the application.
6. Open Swagger to explore and test the API.

---

# 📚 API Documentation

Swagger/OpenAPI is included to provide an interactive API documentation interface.

It allows developers to:

* Explore available endpoints.
* View request and response models.
* Test API endpoints.
* Test authenticated endpoints using JWT.

---

# 🏁 Project Vision

SmartClinicQueue is being developed as a scalable backend foundation for a complete digital clinic queue system.

The current implementation establishes the core infrastructure for:

```text
Authentication
      +
Authorization
      +
Queue Management
      +
CQRS
      +
Real-Time Communication
      +
Background Processing
      +
Rate Limiting
      +
Centralized Error Handling
      +
Request Logging
      +
Clean Architecture
```

The project will continue to evolve to cover additional clinic, doctor, receptionist, and patient requirements while keeping the architecture maintainable and extensible.
