<p align="center">
  <img src="https://img.icons8.com/3d-fluency/94/lock-2.png" width="80" />
</p>

<h1 align="center">OtpModule</h1>

<p align="center">
  <strong>A plug-and-play OTP (One-Time Password) module for ASP.NET Core applications</strong>
</p>

<p align="center">
  <a href="#-features">Features</a> •
  <a href="#-quick-start">Quick Start</a> •
  <a href="#-configuration">Configuration</a> •
  <a href="#-api-endpoints">API</a> •
  <a href="#-architecture">Architecture</a> •
  <a href="#-license">License</a>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" />
  <img src="https://img.shields.io/badge/C%23-13-239120?style=for-the-badge&logo=csharp&logoColor=white" />
  <img src="https://img.shields.io/badge/EF%20Core-10.0-purple?style=for-the-badge&logo=dotnet&logoColor=white" />
  <img src="https://img.shields.io/badge/License-MIT-yellow?style=for-the-badge" />
</p>

---

## 📖 Overview

**OtpModule** is a reusable, modular OTP infrastructure library for ASP.NET Core. It handles the full OTP lifecycle — generation, hashing, persistence, delivery, and verification — so you can add secure one-time password functionality to any project with just **one line of code**.

```csharp
builder.Services.AddOtpModule();
```

The module ships with a built-in API controller, SHA-256 code hashing, configurable expiry/rate-limiting, and a clean abstraction layer that lets you plug in any delivery channel (email, SMS, push, etc.).

---

## ✨ Features

| Feature | Description |
|---|---|
| 🔐 **Secure Hashing** | OTP codes are stored as SHA-256 hashes — plain codes are never persisted |
| ⏱️ **Expiry Control** | Configurable TTL for OTP codes (default: 5 minutes) |
| 🚫 **Brute-Force Protection** | Maximum attempt limit per OTP (default: 5 attempts) |
| 🔄 **Resend Throttling** | Cooldown period between resend requests (default: 60 seconds) |
| 🔌 **Plug & Play** | Single extension method to register all services |
| 🎯 **Built-in Controller** | Ready-to-use `api/otp/send` and `api/otp/verify` endpoints |
| 🧩 **Extensible Design** | Interface-based architecture — swap any component easily |
| 📦 **NuGet Ready** | Configured for NuGet package generation |

---

## 🚀 Quick Start

### 1. Install the package

```bash
dotnet add package OtpModule
```

### 2. Implement `IOtpDbContext`

Create a DbContext in your project that implements `IOtpDbContext`:

```csharp
using Microsoft.EntityFrameworkCore;
using OtpModule.Abstractions;
using OtpModule.Core;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options), IOtpDbContext
{
    public DbSet<OtpEntry> OtpEntries => Set<OtpEntry>();
}
```

### 3. Implement `IOtpSender`

Create a sender that delivers the OTP code (email, SMS, etc.):

```csharp
using OtpModule.Abstractions;

public class EmailOtpSender : IOtpSender
{
    public async Task SendAsync(string key, string code)
    {
        // Your email/SMS delivery logic here
        // 'key' is the destination (email, phone, etc.)
        // 'code' is the plain OTP code to send
    }
}
```

### 4. Register services in `Program.cs`

```csharp
// Database
builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlite("Data Source=otp.db"));

// Bind IOtpDbContext to your DbContext
builder.Services.AddScoped<IOtpDbContext>(x =>
    x.GetRequiredService<AppDbContext>());

// Register your OTP sender
builder.Services.AddScoped<IOtpSender, EmailOtpSender>();

// Register the OTP module (one line!)
builder.Services.AddOtpModule();
```

### 5. Run the app

```bash
dotnet run
```

The endpoints `POST /api/otp/send` and `POST /api/otp/verify` are now available.

---

## ⚙️ Configuration

Customize the OTP behavior by passing options to `AddOtpModule()`:

```csharp
builder.Services.AddOtpModule(options =>
{
    options.ExpireMinutes = 10;   // OTP validity duration (default: 5)
    options.MaxAttempts   = 3;    // Max verification attempts (default: 5)
    options.ResendSeconds = 120;  // Cooldown between resends (default: 60)
});
```

### Options Reference

| Property | Type | Default | Description |
|---|---|---|---|
| `ExpireMinutes` | `int` | `5` | Minutes until the OTP code expires |
| `MaxAttempts` | `int` | `5` | Maximum allowed verification attempts |
| `ResendSeconds` | `int` | `60` | Minimum seconds between resend requests |

---

## 🌐 API Endpoints

The module auto-registers an `OtpController` with two endpoints:

### Send OTP

```http
POST /api/otp/send?key={destination}
```

| Parameter | Location | Description |
|---|---|---|
| `key` | Query | The destination identifier (email, phone, etc.) |

**Responses:**
- `200 OK` — OTP sent successfully
- `500` — Resend cooldown not elapsed

---

### Verify OTP

```http
POST /api/otp/verify?key={destination}&code={otp_code}
```

| Parameter | Location | Description |
|---|---|---|
| `key` | Query | The destination identifier |
| `code` | Query | The OTP code to verify |

**Responses:**
- `200 OK` — Code is valid
- `400 Bad Request` — Invalid code, expired, or max attempts exceeded

---

## 🏗️ Architecture

```
OtpModule/
├── Abstractions/          # Interfaces (contracts)
│   ├── IHashService       # Hashing abstraction
│   ├── IOtpDbContext       # Database abstraction
│   ├── IOtpGenerator       # Code generation abstraction
│   ├── IOtpSender          # Delivery channel abstraction
│   └── IOtpService         # Main service abstraction
├── Core/                  # Domain models & config
│   ├── OtpEntry            # Database entity
│   └── OtpOptions          # Configuration options
├── Infrastructure/        # Default implementations
│   ├── DefaultOtpGenerator # 6-digit random code generator
│   ├── OtpService          # Core OTP logic
│   └── Sha256HashService   # SHA-256 hashing
├── Controllers/
│   └── OtpController       # REST API endpoints
└── Extensions/
    └── ServiceCollectionExtensions  # DI registration
```

### How It Works

```
┌─────────┐     ┌──────────────┐     ┌──────────────┐     ┌───────────┐
│  Client  │────▶│ OtpController │────▶│  OtpService   │────▶│ IOtpSender │
└─────────┘     └──────────────┘     └──────┬───────┘     └───────────┘
                                            │
                              ┌─────────────┼─────────────┐
                              ▼             ▼             ▼
                      ┌──────────┐  ┌────────────┐  ┌───────────┐
                      │ IOtpDb   │  │ IOtpGen    │  │ IHashSvc  │
                      │ Context  │  │ erator     │  │           │
                      └──────────┘  └────────────┘  └───────────┘
```

1. **Send Flow:** Client calls `/send` → `OtpService` generates a 6-digit code → hashes it with SHA-256 → stores the entry in DB → sends the plain code via `IOtpSender`
2. **Verify Flow:** Client calls `/verify` → `OtpService` looks up the latest unused entry → checks expiry & attempt limits → compares hash → marks as used if valid

---

## 🧩 Extensibility

You only need to implement **two interfaces** in your host application:

| Interface | Purpose | You Provide |
|---|---|---|
| `IOtpDbContext` | Database access | Your `DbContext` implementing this interface |
| `IOtpSender` | Code delivery | Email, SMS, Push, or any delivery mechanism |

The following are provided by the module but can be overridden:

| Interface | Default Implementation | Purpose |
|---|---|---|
| `IOtpGenerator` | `DefaultOtpGenerator` | Generates 6-digit numeric codes |
| `IHashService` | `Sha256HashService` | SHA-256 hash for code storage |
| `IOtpService` | `OtpService` | Core send/verify logic |

---

## 📝 Full Example

See the [OtpModule.TestApi](./OtpModule.TestApi) project for a complete working example with:
- SQLite database
- Email OTP delivery via SMTP
- Swagger UI
- User secrets for credentials

---

## 🛠️ Tech Stack

- **.NET 10.0**
- **Entity Framework Core 10.0**
- **ASP.NET Core MVC**
- **SHA-256** cryptographic hashing

---

## 📄 License

This project is open source and available under the [MIT License](LICENSE).

---

<p align="center">
  Made with ❤️ by <a href="https://github.com/isaaholic">@isaaholic</a>
</p>
