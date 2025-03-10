# ProductAPI

## Overview
ProductAPI is a simple ASP.NET Core Web API for managing products. It provides endpoints to perform CRUD operations on products, including stock management. The project uses Entity Framework Core with SQL Server as the database.

## Features
- Retrieve all products
- Get product details by ID
- Add new products
- Update existing products
- Delete products
- Increment or decrement stock levels

## Technologies Used
- **C#** (.NET 8)
- **ASP.NET Core Web API**
- **Entity Framework Core**
- **SQL Server**
- **xUnit** (for unit testing)

## Prerequisites
Ensure you have the following installed:
- .NET SDK 8.0
- SQL Server
- Visual Studio or VS Code
- Postman (optional, for API testing)

## Setup Instructions

### 1. Clone the Repository
```sh
git clone https://github.com/yourusername/ProductAPI.git
cd ProductAPI
```

### 2. Configure the Database
Update `appsettings.json` with your SQL Server connection string:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=ProductDB;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
}
```

### 3. Apply Migrations & Seed Data
```sh
dotnet ef database update
```

### 4. Run the Application
```sh
dotnet run
```

### 5. Test the API Endpoints
Use Postman or Swagger UI (`/swagger/index.html`) to test the endpoints.

## API Endpoints
| Method | Endpoint | Description |
|--------|---------|-------------|
| GET | /api/products | Get all products |
| GET | /api/products/{id} | Get product by ID |
| POST | /api/products | Add a new product |
| PUT | /api/products/{id} | Update a product |
| DELETE | /api/products/{id} | Delete a product |
| PUT | /api/products/decrement-stock/{id}/{quantity} | Reduce product stock |
| PUT | /api/products/add-to-stock/{id}/{quantity} | Increase product stock |

## Running Unit Tests
To execute unit tests, run:
```sh
dotnet test
```

## Contributing
Feel free to fork this repository and submit pull requests with improvements!

## License
This project is licensed under the MIT License.

