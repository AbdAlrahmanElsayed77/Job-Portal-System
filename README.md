# Job Portal System — ASP.NET Core MVC

A professional, full-stack Job Portal built with ASP.NET Core MVC, designed to connect **Employers**, **Job Seekers**, and **Administrators** through a modern and secure web platform.
The system supports **role-based authentication**, **job posting and browsing**, **company management**, **CV uploads**, and **application tracking** with dashboards for each role.

---

## 🏗️ Project Overview

This project implements a complete job portal system that enables different types of users to interact efficiently:

* **Admin:** Manage users, companies, jobs, and system settings.
* **Employer:** Create and manage company profiles, post jobs, and handle applications.
* **Job Seeker:** Browse jobs, upload CVs, and apply to jobs seamlessly.

---

## ⚙️ Technologies Used

* **ASP.NET Core MVC 9.0**
* **Entity Framework Core** for ORM
* **SQL Server** for the database
* **Identity Framework** for authentication and authorization
* **Razor Views** for frontend rendering
* **Bootstrap 5** + **CSS** + **JavaScript** for responsive UI
* **LINQ**, **Repository Pattern**, **Dependency Injection**
* **IIS Express** / **Kestrel Server** for hosting

---

## 📂 Project Structure

```
PortalSystemProject/
│
├── 📁 Presentation Layer (Web - ASP.NET Core MVC)
│   │
│   ├── Areas/
│   │   └── Admin/                    → The only Area in the project (dedicated to administrators)
│   │       ├── Controllers/          → e.g., DashboardController, UserManagementController
│   │       ├── Views/                → Razor views for admin pages (Dashboard, User list, etc.)
│   │       ├── Models/               → Admin-specific ViewModels
│   │       └── wwwroot/              → Admin assets (CSS, JS, images)
│   │
│   ├── Controllers/                  → Main controllers for other roles (not Areas)
│   │   ├── HomeController.cs
│   │   ├── AccountController.cs
│   │   ├── EmployerController.cs
│   │   ├── JobSeekerController.cs
│   │   └── ApplicationController.cs
│   │
│   ├── Models/                       → Shared and role-based models used by controllers
│   │   ├── Job/
│   │   ├── Company/
│   │   ├── Admin/
│   │   └── Shared/
│   │
│   ├── Views/                        → Razor Views for each controller
│   │   ├── Home/
│   │   ├── Employer/
│   │   ├── JobSeeker/
│   │   ├── Account/
│   │   └── Shared/
│   │
│   ├── wwwroot/                      → Static files
│   │   ├── css/
│   │   ├── js/
│   │   ├── images/
│   │   └── uploads/
│   │
│   ├── ViewModels/                   → MVC ViewModels used across roles
│   ├── DTOs/                         → Data transfer objects for Web ↔ BL communication
│   ├── Program.cs / Startup.cs
│   └── appsettings.json
│
├── 📁 Business Logic Layer (BL)
│   ├── Contracts/                    → Interfaces (e.g., IJobService, ICompanyService)
│   ├── Services/                     → Implementations (JobService, EmployerService, etc.)
│   ├── Helpers/                      → Utility classes, validation logic, extensions
│   └── Mappers/                      → Maps Entities ↔ DTOs ↔ ViewModels
│
├── 📁 Data Access Layer (DAL)
│   ├── ApplicationDbContext.cs       → EF Core context, DbSets, configurations
│   ├── Entities/                     → Domain entities (JobPost, Company, Application, etc.)
│   ├── Repositories/                 → Repository pattern implementations
│   │   ├── GenericRepository.cs
│   │   ├── JobRepository.cs
│   │   ├── CompanyRepository.cs
│   │   └── ApplicationRepository.cs
│   ├── SeedData.cs                   → Seeds roles, admin user, and sample jobs
│   └── Migrations/                   → EF Core migrations
│
└── 📁 Domains/
    ├── UserModel/                    → Extended ASP.NET Identity models
    ├── JobModel/                     → Job, JobType, JobCategory entities
    └── CompanyModel/                 → Company, Department, etc.
```

---

## 👥 Roles and Features

### 🔹 Admin

* Manage all users (Employers & Job Seekers)
* Approve or reject companies and job posts
* Manage job categories and job types
* View system statistics and dashboards
* Handle content moderation and reports

### 🔹 Employer

* Register and create a company profile
* Post new job listings with categories and job types
* View and manage job applications
* Shortlist or reject candidates
* Edit or delete job posts
* View analytics on job performance
* **Download applicant list as Excel or PDF** for easy data management and reporting

### 🔹 Job Seeker

* Register, log in, and manage personal profile
* Upload CV or resume
* Browse and search for jobs by category or company
* Apply to jobs directly
* View application history and statuses
* Save favorite jobs

---

## 🧠 Database Design

Key entities include:

* **User (IdentityUser)** — Manages authentication and role mapping.
* **Company** — Contains employer details.
* **Job** — Represents job listings.
* **Application** — Connects job seekers with job posts.
* **JobCategory** and **JobType** — For job classification.

<img width="1460" height="1678" alt="Untitled (1)" src="https://github.com/user-attachments/assets/5ddd381e-d42a-4ee7-8a3c-2b000acde640" />


Each entity is connected using **EF Core relationships** with proper foreign keys and navigation properties.

---

## 🎥 Live Demo

Watch a full walkthrough of the Job Portal System here:
👉 [**Live Demo Video**](https://drive.google.com/file/d/1UK30y6nocgIrN1IUEdVriUz3WYb4kAoq/view?usp=sharing)

---

## 🚀 How to Run the Project

### 1️⃣ Prerequisites

Ensure you have installed:

* [.NET SDK 9.0+](https://dotnet.microsoft.com/)
* SQL Server (LocalDB or SQL Express)
* Visual Studio 2022 (or later)

### 2️⃣ Clone the Repository

```bash
git clone https://github.com/AbdAlrahmanElsayed77/Job-Portal-System.git
cd Job-Portal-System
```

### 3️⃣ Configure Database

Edit your **`appsettings.json`** file to include your SQL Server connection string:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=JobPortalDB;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

### 4️⃣ Apply Migrations and Create Database

```bash
dotnet ef database update
```

Or use Visual Studio **Package Manager Console**:

```powershell
Update-Database
```

### 5️⃣ Run the Application

You can run it using:

```bash
dotnet run
```

Or from Visual Studio, press **F5** to start with IIS Express.

### 6️⃣ Access the Application

* Home: `https://localhost:xxxx`
* Admin Area: `/Admin`
* Employer Area: `/Employer`
* JobSeeker Area: `/JobSeeker`

---

## 📈 Future Scalability (Optional)

* Add notifications and messaging system between employers and job seekers.
* Implement API endpoints for mobile integration.
* Add advanced search and filtering with pagination.
* Include subscription or payment modules.

---


