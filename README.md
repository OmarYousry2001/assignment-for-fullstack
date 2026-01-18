# Assignment for Full Stack – Backend API

## 📌 Project Overview
This project is an **E-Commerce Backend API** built using **ASP.NET Core** and follows the **N-Tier Architecture** pattern.

The solution is designed with scalability, maintainability, and security in mind, utilizing modern backend best practices such as JWT authentication, logging, caching, and clean architecture principles.

---

## 🧱 Architecture
The project follows **N-Tier Architecture** with clear separation of concerns:

- **API** → Controllers & configuration  
- **BL (Business Layer)** → Business logic & services  
- **DAL (Data Access Layer)** → Repositories & database operations  
- **Domains** → Entities, helpers, and shared models  

---

## 🛠️ Technologies & Tools
- ASP.NET Core Web API  
- Entity Framework Core  
- SQL Server  
- ASP.NET Core Identity  
- JWT Authentication  
- Serilog (Logging to SQL Server & Console)  
- Redis (Caching)  
- AutoMapper  
- Swagger (OpenAPI)  
- Rate Limiting  
- Response Compression (Gzip)

---

## 🔐 Authentication
- JWT-based authentication  
- Access Token & Refresh Token support  
- Tokens stored in HTTP Cookies  
- Configurable token validation options  

---

## 📦 Design Patterns Used
- Repository Pattern  
- Generic Repository  
- Unit of Work Pattern  
- Dependency Injection  
- Service Layer Pattern  

---

## 📊 Logging
- **Serilog** is used for logging  
- Logs are stored in:
  - SQL Server (`Log` table – auto-created)
  - Console (for development)

---

## ⚡ Performance & Security
- Rate Limiting enabled  
- In-Memory Cache & Redis Cache  
- Gzip Response Compression  
- CORS Configuration for Angular frontend  

---

## 🌱 Data Seeding
- Initial data is seeded automatically on database creation  
- Includes essential entities such as:
  - Roles
  - Admin user
  - Sample data for testing

---

## ⚙️ Configuration

### 🔗 Connection String (`appsettings.json`)
```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=.;Initial Catalog=assignment-for-fullstack;Integrated Security=True;Encrypt=True;Trust Server Certificate=True;"
}

## Setup Steps
# 1. Clone the repository
git clone https://github.com/OmarYousry2001/assignment-for-fullstack.git

# 2. Navigate to the project folder
cd assignment-for-fullstack

# 3. Install dependencies
dotnet restore

# 4. Update the database
dotnet ef database update

# 5. Run the project
dotnet run

## Test Credentials

Administrator:
Email: admin@gmail.com
Password: Admin-123


