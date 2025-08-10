# Code Generator

## Project Overview
Code Generator is a powerful tool designed to automate the generation of Data Access Layer (DAL) and Business Layer (BL) code for CRUD operations. Instead of manually writing repetitive methods for each database table, this project generates the complete DAL and BL classes in seconds, saving hours of development time and minimizing human errors.

For example, generating DAL and BL for 20 tables manually might take around 60 hours; this tool accomplishes it in about 3 seconds.

## Features

- **Automatic Code Generation**  
  Generates DAL and BL classes with complete CRUD methods: Add, Update, Delete, GetAll, GetByID, and advanced Search.

- **Professional Search Handling**  
  Implements a robust search method that prevents SQL injection by analyzing and sanitizing input strings. Supports multiple search modes such as Anywhere, StartsWith, EndsWith, and ExactMatch.

- **ADO.NET-based DAL**  
  Generates DAL classes using ADO.NET with parameterized queries and stored procedures for secure and efficient database access.

- **Advanced Business Layer (BL)**  
  Generates BL classes with all constructors and fields, handling nullable columns properly.  
  Automatically creates object relationships between tables, enabling intuitive navigation such as `Session.ClientInfo.PeopleInfo.FirstName`.  
  Supports both instance and static method usage for flexible integration.

- **Robust Stored Procedure (SP) Handling**  
  Produces professional SP code for complex data operations.

- **Error Handling and Logging**  
  Captures runtime errors from methods or SPs and logs them into an `ErrorLog` table inside the database.  
  Also stores detailed error information in JSON files using a Publisher-Subscriber design pattern, allowing easy future extensions for error tracking or notification.

- **User-Friendly UI**  
  Simple and clean interface for:  
  - Connecting to SQL Server by entering credentials  
  - Selecting target database and tables for code generation (select all or specific tables)  
  - Choosing options like generating static methods or object relations.

## Technologies Used

- C# (.NET Framework)  
- SQL Server  
- ADO.NET  
- GunaUI (for modern UI components)  
- Newtonsoft.Json (for error serialization)

## System Requirements

- Visual Studio Community 2022 (or later)  
- SQL Server instance  
- GunaUI library  
- Newtonsoft.Json NuGet package

## Installation & Setup

1. Download or clone the repository.  
2. Configure the JSON error files storage path in the application settings.  
3. Build and run the application.

## Usage

1. Launch the application.  
2. Log in with your SQL Server username and password.  
3. Select the target database.  
4. Choose tables to generate DAL and BL for (you can select all or specific tables).  
5. Configure generation options (static methods, object relations).  
6. Generate the code and integrate it into your projects.

## Contact & Support

For questions, support, or contributions, feel free to reach out via LinkedIn:  
[Zakaria Sakalli Housaini](https://www.linkedin.com/in/zakaria-sakalli-housaini-1a782b289)

---

*This tool significantly reduces development time and errors in database-driven applications by automating repetitive code generation tasks with professional quality.*
