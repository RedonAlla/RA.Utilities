---
title: Getting Started
sidebar_position: 2
---

# 🚀 Getting Started
This guide will walk you through installing and using the template to create a new Web API project.

## 1. Prerequisites

*   [.NET SDK 10.0+](https://dotnet.microsoft.com/download)
*   An IDE such as [Visual Studio 2022+](https://visualstudio.microsoft.com/), [JetBrains Rider](https://www.jetbrains.com/rider/), or [Visual Studio Code](https://code.visualstudio.com/docs/languages/dotnet).


## 2. Installation
You can install the template from NuGet.org or by building it locally from the source code.

*   **Local Installation (from source)**
    Clone the repository and run the following command from the root directory of the template project:
    ```bash
    dotnet new install .
    ```

*   **NuGet Installation:**
    Once published, you can install it using this command:
    ```bash
    dotnet new install RA.CleanArchitecture.Template
    ```

### 3. Creating a New Project
You can use the .NET CLI, Visual Studio, or Rider to create a new solution from this template.

#### Using the .NET CLI
```bash
dotnet new RA.Template -n YourProjectName
```
This will create a new solution in a folder named `YourProjectName` with the default settings (JWT Authorization, EF Core with SQL Server, Scalar UI, and HTTP Integrations).

### 4. Customizing Your Project with Parameters
You can customize the generated project by passing parameters to the `dotnet new` command.

**Example 1: Project with Dapper, Oracle, and no Authorization**

This command scaffolds a project that uses Dapper for data access with an Oracle database and disables JWT authorization.

```bash
dotnet new RA.Template -n MyDapperApi --UsePersistence DapperOracle --UseAuthorization false
```
**Example 2: Project with both EF Core and Dapper for SQL Server**

The template supports multiple persistence options.
This is useful if you need to use EF for some parts of your application and Dapper for performance-critical queries.

```bash
dotnet new RA.Template -n MyHybridApi --UsePersistence EfSqlServer --UsePersistence DapperSqlServer
```

**Example 3: A minimal API without Persistence or Integrations**

```bash
dotnet new RA.Template -n MyMinimalApi --UsePersistence "" --UseIntegrations false
```

### 5. Running Your New Application

1.  **Navigate to the project directory**:
    ```bash
    cd YourProjectName
    ```
2.  **Restore Dependencies**:
    ```bash
    dotnet restore
    ```

3.  **Configure Settings**: Open `src/Web/YourProjectName.Api/appsettings.Development.json` and update the `ConnectionStrings` section if you are using a persistence layer.

4.  **Run the application**:
    ```bash
    dotnet run --project src/Web/YourProjectName.Api/YourProjectName.Api.csproj
    ```
5.  **Access the API**: The application will be running on the configured port (e.g., `https://localhost:7001`). You can access the OpenAPI documentation at `https://localhost:7001/openapi-ui`.

### 6. Uninstalling the Template

To remove the template from your machine, run the following command:
```bash
dotnet new uninstall RA.CleanArchitecture.Template
```

## ⚙️ Template Parameters

| Parameter |	Display Name |	Description |	Type	| Default Value |	Available Choices |
| --------- | ------------ | ------------ | ----- | ------------- | ----------------- |
| **Framework** |	.NET Target Framework	| Select the target framework for the project.	| Choice	| `.NET 10` |	`.NET 10` |
| **UseAuthorization** |	JWT Authorization	| Includes JWT-based authorization services and middleware.	| Boolean	| `true` |	`true`, `false` |
| **UseIntegrations** |	Use HTTP Client Integration?	| Adds infrastructure for building and consuming external HTTP services.	| Boolean	| `true` |	`true`, `false` |
| **OpenApiUI** |	OpenApi documentation UI. |	Selects the user interface for the OpenAPI (Swagger) documentation.	| Choice	| `scalar` |	`scalar`, `swagger` |
| **UsePersistence** |	Persistence Layer	| Selects the data access technology. Multiple choices can be selected.	| Choice	| `EfSqlServer` |	`EF with SQL Server`, `EF with Oracle`, `Dapper with SQL Server`, `Dapper with Oracle` |

These parameters allow you to customize the generated solution when creating a new project from the template.
