
# Shop Negotiation API

A RESTful API that allows customers to negotiate prices for products. Built with ASP.NET Core, this API provides a flexible way to manage products and price negotiations in an e-commerce environment.

## Features

- **Product Management**: Create, read, update, and delete products
- **Negotiation System**: Allow users to negotiate prices for products
- **Authentication**: JWT-based authentication for secure API access
- **Role-based Authorization**: Different access levels for customers and employees
- **Automatic Negotiation Expiration**: Background service to manage negotiation lifecycles

## Technologies Used

- **ASP.NET Core 9.0**
- **Entity Framework Core** with In-Memory Database
- **JWT Authentication**
- **FluentValidation**
- **Background Services**
- **Swagger/OpenAPI**
- **xUnit** for testing

## API Documentation




### Authentication

#### Emmployee credentials
Login with these credentials to acquire the JWT Token of an employee.
```json
{
  "username": "employee",
  "password": "password"
}
```

#### Register User

```
POST /api/auth/register
```

Request Body:
```json
{
  "username": "string",
  "password": "string"
}
```

Response: 200 OK

#### Login

```
POST /api/auth/login
```

Request Body:
```json
{
  "username": "string",
  "password": "string"
}
```

Response:
```json
{
  "token": "string"
}
```

### Products

#### Get All Products

```
GET /api/products
```

Response:
```json
[
  {
    "id": 0,
    "productName": "string",
    "description": "string",
    "price": 0,
    "quantity": 0
  }
]
```

#### Get Product by ID

```
GET /api/products/{id}
```

Response:
```json
{
  "id": 0,
  "productName": "string",
  "description": "string",
  "price": 0,
  "quantity": 0
}
```

#### Create Product

```
POST /api/products
```

Request Body:
```json
{
  "productName": "string",
  "description": "string",
  "price": 0,
  "quantity": 0
}
```

Response: Returns the created product with status 201



### Negotiations



### Get All Negotiations

```
GET /api/negotiations
```

Returns all negotiations in the system.

**Authorization**: Requires "EmployeesOnly" policy

**Response**: 200 OK - Array of Negotiation objects

### Get Negotiation by ID

```
GET /api/negotiations/{id}
```

Returns a specific negotiation by ID.

**Parameters**:
- `id` (path parameter) - The ID of the negotiation

**Response**:
- 200 OK - Negotiation object
- 404 Not Found - When negotiation doesn't exist
- 500 Internal Server Error - When an error occurs

### Create Negotiation

```
POST /api/negotiations
```

Creates a new price negotiation for a product.

**Request Body**:
```json
{
  "productId": 1,
  "negotiatorName": "Customer Name",
  "proposedPrice": 85.50
}
```

**Response**:
- 201 Created - New negotiation created successfully
- 400 Bad Request - When product doesn't exist or request is invalid
- 500 Internal Server Error - When an error occurs

### Respond to Negotiation

```
PUT /api/negotiations/{id}/respond
```

Staff can accept or reject a negotiation.

**Authorization**: Requires "EmployeesOnly" policy

**Parameters**:
- `id` (path parameter) - The ID of the negotiation

**Request Body**:
```json
{
  "isAccepted": true
}
```

**Response**:
- 200 OK - Updated negotiation object
- 400 Bad Request - When negotiation is not in pending status or invalid request
- 404 Not Found - When negotiation doesn't exist
- 500 Internal Server Error - When an error occurs

### Make Counter Offer

```
PUT /api/negotiations/{id}/counteroffer
```

Allows a customer to make a counter-offer for a rejected negotiation.

**Parameters**:
- `id` (path parameter) - The ID of the negotiation

**Request Body**:
```json
{
  "proposedPrice": 95.00
}
```

**Response**:
- 200 OK - Updated negotiation object
- 400 Bad Request - When negotiation is not in rejected status, max attempts reached, expired, or invalid request
- 404 Not Found - When negotiation doesn't exist
- 500 Internal Server Error - When an error occurs

## Negotiation Workflow

1. Customer creates a negotiation with a proposed price
2. The negotiation starts with "Pending" status
3. Staff can accept or reject the negotiation
4. If rejected, customer can submit a counter-offer (up to 3 total attempts)
5. Negotiations expire after 7 days if not finalized
6. After max attempts, negotiations are automatically closed

## Objects

### Negotiation
- `id` - Unique identifier
- `productId` - ID of the product being negotiated
- `negotiatorName` - Name of the customer negotiating
- `initialPrice` - Original price of the product
- `proposedPrice` - Current proposed price
- `finalPrice` - Final agreed price (if accepted)
- `status` - Current status (Pending, Accepted, Rejected, Expired, Closed)
- `attemptsCount` - Number of negotiation attempts
- `negotiationDate` - Date when negotiation was created or last updated
- `lastResponseDate` - Date of the last staff response
- `expirationDate` - Date when the negotiation will expire
## Project Structure

The solution follows a clean architecture approach with the following structure:

- **ShopNegotiationAPI.API**: API controllers, validators and DTOs
- **ShopNegotiationAPI.Application**: Business logic, interfaces
- **ShopNegotiationAPI.Domain**: Domain models and entities
- **ShopNegotiationAPI.Infrastructure**: Data access, repositories, background services
- **ShopNegotiationAPI.Test**: Unit and integration tests

## Running the Application

1. Clone the repository
2. Navigate to the project directory
3. Run `dotnet restore` to restore dependencies
4. Run `dotnet build` to build the application
5. Run `dotnet run --project ShopNegotiationAPI.API` to start the API
6. Access the Swagger UI at `https://localhost:5001/swagger`

## Testing

The project includes both unit tests and integration tests to ensure the reliability of the code.

Run tests with:

```
dotnet test
```

## Authentication and Authorization

The API uses JWT (JSON Web Tokens) for authentication. To access protected endpoints:

1. Register a new user or use the default employee account:
   - Username: employee
   - Password: password
2. Login to obtain a JWT token
3. Include the token in the Authorization header of subsequent requests:
   ```
   Authorization: Bearer your_token_here
   ```

## License

MIT License
