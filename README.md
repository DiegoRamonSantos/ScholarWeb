# 🎓 ScholarWeb

A web application built with **ASP.NET Core MVC** for academic and administrative management of an educational institution. The project centralizes registrations, enrollments, grades, academic terms, and financial records in a simple, organized, and responsive dashboard.

## 📌 About the Project

**ScholarWeb** is an academic management system designed to simplify the administration of school information in a web environment. The application includes administrator authentication, a dashboard with key metrics, and management modules for students, teachers, courses, classes, subjects, enrollments, grades, and financial operations.

The project was developed with a strong focus on organization, usability, and maintainability, using the MVC architecture, Entity Framework Core, data validation, and local persistence with SQLite.

## ✨ Key Features

✅ Dashboard with statistics for students, teachers, courses, and classes  
✅ Financial overview with received, pending, and overdue amounts  
✅ Create, edit, view details, and delete/deactivate records  
✅ Student management with CPF, email, phone number, address, and status  
✅ Teacher management with education, specialization, and status  
✅ Course, subject, class, and academic term registration  
✅ Student enrollments linked to courses, classes, and academic terms  
✅ Grade management with automatic final average calculation  
✅ Automatic student status determination: approved, recovery, or failed  
✅ Financial control by student and enrollment  
✅ Automatic financial status updates: pending, paid, overdue, or canceled  
✅ Search functionality across core administrative modules  
✅ Client-side and server-side validation  
✅ Responsive interface with sidebar navigation and admin dashboard  
✅ Local data persistence using SQLite

## 🛠️ Technologies Used

### Backend

- **ASP.NET Core 10.0**
- **C#**
- **ASP.NET Core MVC**
- **Entity Framework Core 10.0**
- **SQLite**

### Frontend

- **Razor Views**
- **HTML5**
- **Custom CSS**
- **Bootstrap**
- **JavaScript**
- **Font Awesome**
- **Google Fonts – Inter**

### Development Tools

- **.NET 10 SDK**
- **Entity Framework Core Tools**
- **Entity Framework Migrations**
- **Visual Studio 2026** or **Visual Studio Code**

## 🧱 Project Architecture and Structure

The application follows a well-organized MVC structure:

- **Controllers**: Controllers responsible for routes and module actions
- **Controllers/Admin**: Reusable administrative CRUD controller
- **Models**: Academic and financial domain entities
- **ViewModels**: Models dedicated to views, dashboard, and administrative components
- **Views**: Razor application pages
- **Views/Shared/AdminCrud**: Shared views for listing, creating, editing, viewing details, and deleting records
- **Data**: Entity Framework context, migrations, factory, and database configuration
- **wwwroot**: Static assets such as CSS, JavaScript, Bootstrap, jQuery, and icons

## 📚 System Modules

- **Dashboard**: General statistics, financial overview, and recently registered students
- **Students**: Complete student registration and management
- **Teachers**: Teacher registration and management
- **Courses**: Course management, workload, and duration
- **Classes**: Class organization by course, academic term, schedule, and capacity
- **Subjects**: Assignment of courses, teachers, and workload
- **Enrollments**: Student association with courses, classes, and academic terms
- **Grades**: Grade entry and automatic average calculation
- **Financial**: Management of tuition fees, enrollments, charges, materials, and other financial records
- **Academic Terms**: Registration and management of academic periods

## ⚙️ System Requirements

Before running the project, make sure you have installed:

- **.NET 10 SDK**
- **Visual Studio 2026** with the **ASP.NET and Web Development** workload (or **Visual Studio Code**)
- **Git** (optional, to clone the repository)
- A modern web browser

## 🚀 Running Locally

### Clone the Repository

```bash
git clone https://github.com/DiegoRamonSantos/ScholarWeb.git
cd ScholarWeb
```

Alternatively, download the project as a ZIP file and extract it to a local folder.

### Open the Project in Visual Studio

Install **Visual Studio 2026** with the **ASP.NET and Web Development** workload and the **.NET 10 SDK**. Open the `ScholarWeb.slnx` file located in the project folder. Visual Studio will automatically restore the NuGet dependencies when loading or building the solution.

### Run the Application

In Visual Studio, select the `https` launch profile in the toolbar and press **F5**. To run without debugging, press **Ctrl+F5**.

The application will open in your browser. With the `https` profile, the configured URLs are:

```text
https://localhost:7224
http://localhost:5239
```

### Sign In

Access the application's home page and log in using the demo administrator credentials:

- **Email:** `admin@scholarweb.local`
- **Password:** `Admin@123`

After signing in, the system will redirect you to the dashboard.

> **Warning:** These credentials are public and exist exclusively for demonstration, testing, and educational purposes. Do not use this password in production environments. In a real-world deployment, change the password and keep credentials out of the source code.

### Basic Usage

1. Register academic terms.
2. Register courses, teachers, and subjects.
3. Create classes linked to courses and academic terms.
4. Register students.
5. Enroll students in classes.
6. Enter grades and monitor academic performance.
7. Record financial transactions and track payments through the dashboard.

---

## 🧠 Concepts Applied

- **MVC Architecture**
- Separation of **Models**, **Views**, **Controllers**, and **ViewModels**
- Reusable CRUD structure for administrative modules
- Entity Framework Core with relationships and migrations
- Validation using Data Annotations
- Native ASP.NET Core dependency injection
- Business rules implemented within the data context
- Automatic average grade and academic status calculation
- Automatic financial status updates
- Responsive and component-based interface

## 📝 Important Notes

- The system uses a local SQLite database and does not depend on external services for data persistence.
- Access is designed for an administrator user.
- The project is intended for learning purposes, portfolio projects, and future expansion into a more comprehensive academic management system.
- The credentials above are provided exclusively for project demonstration purposes and do not represent real or production credentials.

## 👨‍💻 Author

**Diego Ramon**# 