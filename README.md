# Code Generator
![Code Generator Screenshot](<img width="1150" height="525" alt="image" src="https://github.com/user-attachments/assets/d9f02c8c-14ca-4e86-ac16-5c87acf17324" />)
<img width="1274" height="715" alt="image" src="https://github.com/user-attachments/assets/91ec6c99-2a3d-41cd-b75c-4e8138a9e218" />


## Project Overview
Code Generator is a powerful tool designed to automate the generation of Data Access Layer (DAL) and Business Layer (BL) code for CRUD operations. Instead of manually writing repetitive methods for each database table, this project generates the complete DAL, BL, DTO, API, and Migration layers in seconds, saving hours of development time and minimizing human errors.

For example, generating the full data stack (DAL + BL + DTO + API + SPs) for 20 tables manually might take around 60 hours; this tool accomplishes it in about 3 seconds.

## Features

- **Automatic Code Generation**  
  Generates DAL and BL classes with complete CRUD methods: Add, Update, Delete, GetAll, GetByID, and advanced Search.

- **DTO Layer Generation (New)**  
  Optionally generates a clean **DTO (Data Transfer Object) layer** for every table. When enabled, the DAL and BL are generated to work with strongly-typed DTOs instead of raw DataTables, giving you type safety and a proper separation of concerns.

- **API Layer Generation (New)**  
  Generates a complete **ASP.NET Core API controller layer** with production-ready endpoints:
  - `GET` – GetAll and GetByID (+ advanced GetByID with added parameters)
  - `POST` – AddNew
  - `PUT` / `PATCH` – Update
  - `DELETE` – Delete
  - `GET` – Search endpoint  
  Endpoints are decorated with `[HttpGet]`, `[ProducesResponseType]`, and route constraints typed from the actual SQL column data types. The API layer requires the DTO layer to be enabled.

- **Migration Layer Generation (new)**  
  Generates a **DbUp-based migration layer** (`clsDbMigrator.cs`) that includes:
  - `EnsureDatabase.For.SqlDatabase(...)` to create the target database if it does not exist
  - Automatic discovery and execution of every migration script under the `Migrations` folder
  - Console logging of each migration step and clear failure reporting

- **SQL Stored Procedure Generation**  
  Produces professional `CREATE` / `ALTER` SP code for all CRUD operations plus a configurable `SP_HandleError` procedure. SPs are written under a `SPTables` folder.

- **Auto Execute SP**  
  Toggle the **Auto Execute SP** switch so every generated `CREATE` or `ALTER` stored procedure is executed directly against the connected SQL Server.

- **Professional Search Handling**  
  Implements a robust search method that prevents SQL injection by analyzing and sanitizing input strings. Supports multiple search modes such as Anywhere, StartsWith, EndsWith, and ExactMatch.

- **ADO.NET-based DAL**  
  Generates DAL classes using ADO.NET with fully parameterized queries (dynamic SQL or stored procedures) for secure and efficient database access.

- **Advanced Business Layer (BL)**  
  Generates BL classes with all constructors and fields, handling nullable columns properly.  
  Automatically creates object relationships between tables, enabling intuitive navigation such as `Session.ClientInfo.PeopleInfo.FirstName`.  
  Supports both instance and static method usage for flexible integration.

- **Lazy Load Support for Related Entities (Lazy Load Design Pattern)**  
  The generated classes initialize foreign key references (e.g., Reservations, Guests, Users) using **Lazy<T>**, so related data is loaded **only when accessed**. This improves performance and reduces unnecessary database calls.

- **AI-Powered Code Documentation (with LM Studio) (new)**  
  After generating your code, Code Generator can run every generated `.cs` file through a **local, offline LLM** (Qwen2.5 Coder) hosted by **LM Studio** and automatically insert professional XML documentation comments (`<summary>`, `<param>`, `<returns>`, `<typeparam>`, `<remarks>`, `<exception>`, `<see cref>`) into classes, constructors, interfaces, enums, properties, methods, and DTOs — all without sending your code to any cloud service.

- **Async Code Generation + Live Progress (new)**  
  Generation runs **asynchronously** (no frozen UI) and reports the elapsed time in milliseconds. Live progress is shown while files are processed.

- **Project Name Support (new)**  
  You can specify a **Project Name** — all generated folders, namespaces, and the connection-string settings class are named after it.

- **Error Handling and Logging (Publisher-Subscriber design pattern)**  
  Captures runtime errors from methods or SPs and logs them into an `ErrorLog` table inside the database.  
  Also stores detailed error information in JSON files using a Publisher-Subscriber design pattern, allowing easy future extensions for error tracking or notification.

- **User-Friendly UI**  
  Simple and clean interface for:
  - Connecting to SQL Server by entering credentials
  - Selecting target database and tables for code generation (select all or specific tables)
  - Choosing options like generating static methods, object relations, DTOs, an API layer, auto-executing SPs, and AI documentation.

## Technologies Used

- C# (.NET Framework)  
- SQL Server  
- ADO.NET  
- .NET Core (for the generated API layer)  
- DbUp (for the generated migration layer)
- LM Studio (local AI model server)  
- GunaUI (for modern UI components)  
- Newtonsoft.Json (for error serialization)

## System Requirements

- Visual Studio Community 2022 (or later)  
- SQL Server instance  
- GunaUI library  
- Newtonsoft.Json NuGet package  
- **LM Studio** (recommended, for the AI documentation feature)

## Installation & Setup

1. Download or clone the repository.  
2. Configure the JSON error files storage path in the application settings.  
3. Build and run the application.

## Usage

1. Launch the application.  
2. Log in with your SQL Server username and password.  
3. Enter a **Project Name** and choose the folder where the code should be generated.  
4. Select the target database.  
5. Choose tables to generate code for (you can select all or specific tables).  
6. Configure generation options (static methods, object relations, DTO layer, API layer, auto execute SP, AI documentation).  
7. Generate the code and integrate it into your projects.

---

## AI Documentation with LM Studio (Qwen2.5 Coder)

The **AI code documentation** feature lets you automatically add professional XML documentation comments to the generated C# source files using a **local LLM** that runs entirely on your own machine (LM Studio). No code ever leaves your computer.

### Architecture

```
Code Generator  ──HTTP──▶  LM Studio (localhost:1234)
   │                         │
   │  POST /v1/chat/completions   │  serves the loaded model
   │  { "model": "qwen2.5-coder-1.5b-instruct-finetuned-qwq.gguf", ... }
   │                         ▼
   └──── writes doc comments   Qwen2.5-Coder-1.5B-Instruct (local)
```

The app talks to LM Studio's OpenAI-compatible endpoint at `http://127.0.0.1:1234/v1/chat/completions` and asks the model to return only documented C# code.

### Prerequisites

- [LM Studio](https://lmstudio.ai/) desktop app installed on Windows.
- At least ~2–4 GB of free disk space for the model files.
- The Code Generator app must be running on the same machine as LM Studio (localhost).

### Step 1 – Install LM Studio

1. Download LM Studio from [https://lmstudio.com](https://lmstudio.com).
2. Run the installer and complete the setup (no admin privileges required).
3. Launch LM Studio and sign in (optional, needed for some model downloads).

### Step 2 – Download the qwen2.5-coder-1.5b-instruct model

1. Open the **Search** tab in the left sidebar.
2. In the search box type: `qwen2.5-coder-1.5b` and press Enter.
3. From the results, select **`Qwen/Qwen2.5-Coder-1.5B-Instruct-GGUF`** — this is the exact model the Code Generator expects to load under the name:
   ```
   qwen2.5-coder-1.5b-instruct-finetuned-qwq.gguf
   ```
4. Download the model repository (the `.gguf` file). Downloading multiple files or the fp16 version is **not** required — a single Q4_K_M (or f16) quant (around 1 GB) is plenty for this task.

> If you prefer a different stem of Qwen2.5-Coder (e.g. a larger 7B quant), make sure you update the model ID both here and in `DocumentationGenerator/AIDocumentationGenerator.cs` (see Configuration below).

### Step 3 – Load the model in LM Studio

1. Open the **My Models** / **Models** tab.
2. Click **Load** on the downloaded `qwen2.5-coder-1.5b` file.  
   The model now occupies RAM / VRAM and is ready when its tab shows a green **Ready** state.
3. (Optional) Open the **Chat** tab and send a test prompt like `Hello, write a C# comment.` to confirm the model answers.

### Step 4 – Start the local server

1. Click **← Server** (or `Server` in LM Studio's left sidebar — the "Developer" tab).
2. Under **Server**, press **Start Server**.
3. Confirm the default base URL: `http://127.0.0.1:1234` (and port stays `1234`).
4. The status line shows **`Server is running`** / the model is loaded. Keep the window open while you run the Code Generator.


### Step 5 – Use AI documentation in the app

1. Complete a normal code generation (select database + project name + tables).
2. Tick the **"AI Code Documentation"** option (`ckAiCodeDocs`) before clicking the generate button.
    - The app first generates DAL / BL / DTO / API / SP / Migration files, then scans the entire output folder.
3. Watch the progress bar — the app processes each `.cs` file (skipping `bin`, `obj`, `.git`, `.vs` directories and `AssemblyInfo.cs`, `Program.cs`, `Startup.cs`, `*.Designer.cs`, `*.g.cs`).
4. When done, a message box confirms **"Documentation generated successfully"**. Open any generated `.cs` file and verify the XML comments were inserted.

> Every generated file is sent individually to LM Studio, so a large project may take a few minutes — the progress bar shows per-file progress.

## Troubleshooting / FAQ

**LM Studio server isn't reachable (connection refused / timeout).**
- Make sure LM Studio is running and the **Server is started** (green).
- Open `http://127.0.0.1:1234/v1/models` (v1 or just base URL) in a browser — if it answers with a JSON list of models, the server is up. If not, restart the server.
- Confirm the base URL in `AIDocumentationGenerator.cs` matches `http://127.0.0.1:1234` and there is no trailing slash and no `/v1` in `BaseAddress`.

**Model not found (404 “model … not found”).**
The `model` string must exactly match the model ID in LM Studio. Copy it from the Server page's loaded model dropdown (it includes `.gguf`).

**Wrong type of docs (completely empty / freeform text instead of code).**
The model sometimes returns JSON or markdown wrapping the C# code. Retry that single file, or switch to the Q4_K_M quant of `Qwen2.5-Coder-1.5B-Instruct` (the most stable for this workflow).

**AI documentation never runs.**
The AI documentation step is only triggered when the **"AI Code Documentation"** checkbox is ticked and a generation succeeds first. Ensure the output folder contains `.cs` files (non-empty source).

**Docs appear on a single managed file but similar files were skipped.**
`DocumentationProcessor` intentionally ignores `bin`, `obj`, `.git`, `.vs`, `Program.cs`, `*.Designer.cs`, and `*.g.cs`. If you need them documented, remove the matching name from `IgnoredFiles`.

**Performance / memory usage.**
Qwen2.5-Coder-1.5B is a small model and runs even on modest machines. If it is slow, close other apps or use a smaller quantization. For very large projects, pre-select only the tables you need.

---

## Generated Output Structure

After generating from the project name such as… `MyProject`, the app creates these folders under the output path:

```
MyProject_DataAccess/     DAL (or DTO-based), ConnectionString class, ErrorHandling, SPTables
MyProject_Business/       Business Layer
MyProject_DTO/            DTO classes (when DTO enabled)
MyProject_API/            ASP.NET Core controllers (when API enabled)
MyProject_Migration/      clsDbMigrator.cs + Migrations folder
```

## Contact & Support

For questions, support, or contributions, feel free to reach out via LinkedIn:  
[Zakaria Sakalli Housaini](https://www.linkedin.com/in/zakaria-skalli-housaini-1a782b289)
[Mohamed Ouaalane](https://www.linkedin.com/in/mohamed-ouaalane-82758129b)

📹 **Project Demo Video (Version: 8/2026 – with major updates):**  
[Watch Here](https://drive.google.com/file/d/1UoHtK0V07jpzq3qof5heyD3q9Zz2l4VT/view?usp=drive_link)

---

*This tool significantly reduces development time and errors in database-driven applications by automating repetitive code generation tasks with professional quality.*
