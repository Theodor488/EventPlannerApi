# EventPlannerApi

EventPlannerApi is a RESTful API for creating, managing, and registering for events, built using ASP.NET Core, Entity Framework Core, and JWT-based authentication. The API allows users to login and create their own events, register for existing events, and update/delete owned events.

## Features

- **JWT Authentication**: Secures the API with JWT tokens, allowing only authenticated users to perform certain actions.
- **Role-Based Authorization**: Restricts access to updating or deleting events to admins or event hosts.
- **CRUD Operations**: Provides endpoints to create, read, update, register/rsvp for and delete events.
- **Validation**: Ensures proper data input through model validation.
- **Swagger Integration**: Interactive API documentation and testing HTTP request methods via a web-based UI.

## Technologies Used

- **ASP.NET Core**: For building the API.
- **Entity Framework Core**: For database management.
- **SQL Server**: Database backend.
- **JWT**: For authentication.
- **Swagger**: For API documentation and HTTP request method testing.

### Clone the Repository
```bash
git clone https://github.com/Theodor488/EventPlannerApi.git
cd EventPlannerApi
```

### Configure the Database
1. Open `appsettings.json` or `appsettings.Development.json`.
2. Update the connection strings for `DefaultConnection` and `AuthConnection` to match your SQL Server instance:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=EventsDB;Trusted_Connection=True;",
  "AuthConnection": "Server=YOUR_SERVER;Database=AuthDB;Trusted_Connection=True;"
}
```

### Events
- **GET** `/api/Events`: Retrieve all events.
- **GET** `/api/Events/{eventId}`: Retrieve a specific event by ID.
- **POST** `/api/Events`: Create a new event (requires authentication).
- **PUT** `/api/Events/{eventId}`: Update an existing event (requires authentication, admin or host access).
- **DELETE** `/api/Events/{eventId}`: Delete an event (requires authentication, admin access or host access).
- **POST** `/api/Events/{eventId}/registerEvent`: Register/RSVP for an existing event.
- **GET** `/api/Events/{eventId}/attendees`: Retrieve all users registered for an event. 

## Example Request

### Creating an Event
#### Request
```http
POST /api/Events HTTP/1.1
Host: localhost:7027
Authorization: Bearer <your-jwt-token>
Content-Type: application/json

{
  "name": "Birthday Party",
  "description": "A fun birthday celebration",
  "date": "2025-01-28T14:00:00",
  "location": "New York City"
}
```
#### Response
```json
{
  "name": "Birthday Party",
  "description": "A fun birthday celebration",
  "date": "2025-01-28T14:00:00",
  "location": "New York City"
}
```

### Registering for an Event
#### Request
```http
POST /api/Events HTTP/1.1
Host: localhost:7027
Authorization: Bearer <your-jwt-token>
Content-Type: application/json

{
  "eventId": "{eventId Guid}",
  "userId": "{userId Guid}",
}
```
#### Response
```json
{
  {
    "eventId": "{eventId Guid},
    "userId": "{userId Guid}",
    "userName": "Celsus4",
    "name": "Celsus",
    "eventName": "28th Birthday"
  },
  {
    "eventId": "{eventId Guid}",
    "userId": "{userId Guid}",
    "userName": "Theodor",
    "name": "Theo",
    "eventName": "28th Birthday"
  },
  {
    "eventId": "{eventId Guid}",
    "userId": "{userId Guid}",
    "userName": "John488",
    "name": "John",
    "eventName": "28th Birthday"
  }
}
```
