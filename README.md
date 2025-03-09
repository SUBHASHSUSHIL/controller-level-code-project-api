# Controller-Level Code Project API
![GitHub Repo Stars](https://img.shields.io/github/stars/SUBHASHSUSHIL/controller-level-code-project-api?style=social)
![GitHub Forks](https://img.shields.io/github/forks/SUBHASHSUSHIL/controller-level-code-project-api?style=social)

## Description
This repository contains a **.NET Core API project** where all **business logic and database operations** are handled directly within controllers. This approach was chosen to **speed up API development** when working under strict time constraints.

## Features
- 🚀 **Fast API development** with direct controller-level logic
- 🔗 **Direct database access** without separate service/repository layers
- ⚡ **Minimal setup required** for quick implementation
- 🛠️ **Standard CRUD operations** implemented directly in controllers
- 🔒 **Basic authentication & security measures**

## Technologies Used
- **.NET Core 8**
- **Entity Framework Core** (Direct DB operations inside controllers)
- **SQL Server / MySQL** (Depending on configuration)
- **Swagger UI** for API testing
- **JWT Authentication** (if applicable)

## Project Structure
```
/Controller-Level-API-Project
│── Controllers/
│   ├── UserController.cs
│   ├── ProductController.cs
│   ├── OrderController.cs
│── Models/
│── appsettings.json
│── Program.cs
```

## Installation & Usage
1. Clone the repository:
   ```sh
   git clone https://github.com/SUBHASHSUSHIL/controller-level-code-project-api.git
   ```
2. Open the project in **Visual Studio / VS Code**.
3. Configure the **database connection** in `appsettings.json`.
4. Run the migrations (if using EF Core):
   ```sh
   dotnet ef database update
   ```
5. Start the API:
   ```sh
   dotnet run
   ```
6. Access Swagger UI at: `http://localhost:5000/swagger`

## When to Use This Approach
✔️ **Fast development required** – When there's not enough time to structure a layered architecture.
✔️ **Small or temporary projects** – Where maintainability is not a major concern.
✔️ **Prototype or MVP APIs** – Quick testing and iteration before scaling up.

## Limitations
⚠️ **Not suitable for large-scale applications** – Lack of separation of concerns.
⚠️ **Code duplication & maintainability issues** – Business logic directly inside controllers.
⚠️ **Difficult to test** – Since logic isn't separated into services or repositories.

## Contribution
If you have improvements or optimizations, feel free to contribute:
1. Fork the repository
2. Create a new branch (`git checkout -b refactor-api`)
3. Make your changes and commit (`git commit -m 'Refactored API structure'`)
4. Push to your branch (`git push origin refactor-api`)
5. Create a Pull Request

## Contact
For any queries, reach out to: [Sushil Thakur](mailto:sushilthakur9792@gmail.com)
