#  QuizMaster - Event-Driven Microservices Platform 

**QuizMaster** is a high-performance, scalable online assessment platform built using a **Microservices Architecture** with **.NET 10**. The system demonstrates advanced backend patterns, including asynchronous messaging, synchronous internal communication, and transactional reliability.

---

##  Architecture Overview

The system consists of five specialized services designed with **Clean Architecture** principles:

1. **Identity.API**: Manages users via **ASP.NET Core Identity** and **PostgreSQL**. Handles registration, real-world email confirmation (via **Resend**), and issues **JWT Bearer Tokens**.
2. **Quiz.API**: Core service for quiz management using **MongoDB**. Handles content CRUD and submissions. Implements the **Transactional Outbox** pattern.
3. **Grading.Worker**: A background service that calculates scores by communicating with `Quiz.API` via **gRPC**. It persists results in **PostgreSQL** and uses **Inbox/Outbox** for reliable processing.
4. **Certificate.API**: Listens for completion events, generates professional PDF certificates using **QuestPDF**, and stores them in **MinIO (S3 Compatible Storage)**.
5. **API Gateway (YARP)**: The central entry point. Routes traffic to downstream services and enforces **JWT Authentication** policies.

---

##  Tech Stack

* **Runtime**: .NET 10 (C# 13)
* **Messaging**: RabbitMQ with **MassTransit**
* **Internal RPC**: gRPC
* **Databases**: PostgreSQL (Relational), MongoDB (NoSQL)
* **Object Storage**: MinIO (S3)
* **Security**: JWT Bearer Auth, ASP.NET Identity (Guid-based PK)
* **Observability**: Serilog, **Seq** (Centralized Structured Logging)
* **Reporting**: QuestPDF
* **Reverse Proxy**: YARP (Yet Another Reverse Proxy)

---

##  Reliability & Design Patterns

* **Transactional Outbox Pattern**: Implemented in `Quiz.API` and `Grading.Worker`. Ensures that events (like `QuizPassedEvent`) are never lost. If the Message Broker is down, messages are stored in the DB and dispatched automatically upon reconnection.
* **Inbox Pattern (Idempotent Consumer)**: Implemented in `Grading.Worker` and `Certificate.API`. Prevents duplicate processing (e.g., prevents sending multiple certificate emails for a single test attempt).
* **Service-specific Databases**: Strictly follows the "Database per Service" rule to ensure independent scalability and data isolation.
* **Thin Controllers**: All business logic is encapsulated in Services, using **IUserContext** abstractions for clean DI.

---

##  Getting Started

### Prerequisites
* [Docker Desktop](https://www.docker.com/products/docker-desktop/)
* [.NET 10 SDK](https://dotnet.microsoft.com/download)
* Visual Studio 2022 / JetBrains Rider

### Installation & Run

1.  **Clone the repo:**
    ```bash
    git clone [https://github.com/yourusername/QuizMaster.git](https://github.com/yourusername/QuizMaster.git)
    cd QuizMaster
    ```

2.  **Start Infrastructure** Launch databases, broker, and monitoring tools:
    ```bash
    docker-compose up -d
    ```

3.  **Configure User Secrets** Set sensitive keys for each service:

    **Identity.API:**
    ```bash
    cd src/Services/Identity.API
    dotnet user-secrets set "JwtSettings:Secret" "YOUR_LONG_PERSONALIZED_SECRET_KEY_FOR_JWT_TOKEN_GENERATION"
    dotnet user-secrets set "Resend:ApiKey" "re_your_api_key"
    ```

    **QuizMaster.Gateway:**
    ```bash
    cd ../../../src/Gateways/QuizMaster.Gateway
    dotnet user-secrets set "JwtSettings:Secret" "YOUR_LONG_PERSONALIZED_SECRET_KEY_FOR_JWT_TOKEN_GENERATION"
    ```

4.  **Run the Project**
    * Open `QuizMaster.sln` in Visual Studio.
    * Set **Multiple Startup Projects** to `Start`: Gateway, Identity, Quiz, Grading, and Certificate.
    * Press **F5**.

---

##  Monitoring & Docs

| Service | URL | Note |
| :--- | :--- | :--- |
| **Gateway Entry** | `https://localhost:7075` | Primary API endpoint |
| **API Docs** | `https://localhost:7075/scalar/v1` | Interactive Scalar UI |
| **Seq UI** | `http://localhost:5341` | Centralized Log Dashboard |
| **MinIO Console** | `http://localhost:9001` | `admin` / `minio_password` |
| **RabbitMQ** | `http://localhost:15672` | `guest` / `guest` |

## Certificate example
<img width="894" height="635" alt="image" src="https://github.com/user-attachments/assets/a9f38e84-75e5-4a3c-8ae2-367a0c2e9d69" />

