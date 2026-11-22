# Learnify

Learnify is a full-stack online course marketplace. Students can browse and
search courses, add courses to a basket, complete Stripe Checkout, and access
purchased course lectures. A registered instructor can create courses and add
sections and lectures.

## Features

- Browse courses and filter them by category
- Search courses and sort or paginate results
- Register and sign in with ASP.NET Core Identity
- JWT-based authentication for protected API endpoints
- Maintain a basket before and after sign-in
- Purchase courses through Stripe PaymentIntents
- View purchased courses from the student dashboard
- Resume the latest lecture for a purchased course
- Create unpublished courses and add sections and lectures as an instructor
- Seeded demo users, categories, courses, and course content

## Technology stack

### Web application

- React 17
- TypeScript
- Redux Toolkit
- React Router
- Ant Design
- Axios
- Stripe Elements
- Sass

### API

- ASP.NET Core 6
- Entity Framework Core
- ASP.NET Core Identity
- JWT Bearer authentication
- AutoMapper
- PostgreSQL in the configured application startup
- Stripe.net
- Swagger in Development

The solution is divided into three projects:

- `API` - HTTP controllers, authentication, middleware, DTOs, and application startup
- `Infrastructure` - EF Core context, repositories, migrations, seed data, and
  Token/Stripe services
- `Entity` - domain entities and repository/specification abstractions

## How it works

The React app starts by loading the current user, basket, and categories. Axios
sends the JWT from Redux state in the `Authorization: Bearer <token>` header.
Development API calls are directed to `http://localhost:5000/api`; the
production build uses relative `/api/` URLs and is served by ASP.NET Core.

Course, category, basket, lecture, and user operations are exposed through
controllers under `/api`. Protected actions require a JWT, and course creation
and publishing require the `Instructor` role.

An anonymous basket is identified with an essential `clientId` cookie. After
login, `UsersController` merges or replaces the cookie basket with the signed-in
user basket. Purchased course IDs are stored in the `UserCourses` join entity.

For checkout, the API creates or updates a Stripe PaymentIntent from the
basket total. The React app collects card details through Stripe Elements.
After payment, the user is recorded as having purchased the basket's courses.

On startup the API runs EF Core migrations and seeds demo data when the relevant
tables are empty. Seed JSON files live in `Infrastructure/SeedData`.

## Demo accounts

The seed process creates:

| Role | Username | Email | Password |
| --- | --- | --- | --- |
| Student | `student` | `student@test.com` | `Password@123` |
| Instructor | `instructor` | `instructor@test.com` | `Password@123` |

The instructor account also has the Student role.

## Prerequisites

- .NET 6 SDK
- Node.js and npm
- PostgreSQL for the database configuration used by `API/Program.cs`
- A Stripe secret key for checkout

The repository also contains legacy SQLite configuration in
`API/Startup.cs`, but the active top-level `API/Program.cs` configures
`Npgsql`. The PostgreSQL EF provider package must be present before the API can
be built or run.

## Configuration

Create a local app settings file or user secrets for the API. In Development,
`ASPNETCORE_ENVIRONMENT=Development` is set by the project launch profile.
Configure at least:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=learnify;Username=YOUR_DB_USER;Password=YOUR_DB_PASSWORD"
  },
  "JWT": {
    "TokenKey": "A-long-random-secret-for-signing-tokens"
  },
  "Stripe": {
    "ClientSecret": "sk_test_..."
  }
}
```

The following values can be supplied with .NET user secrets:

```bash
cd API
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=learnify;Username=YOUR_DB_USER;Password=YOUR_DB_PASSWORD"
dotnet user-secrets set "JWT:TokenKey" "A-long-random-secret-for-signing-tokens"
dotnet user-secrets set "Stripe:ClientSecret" "sk_test_..."
```

For deployments outside Development, `API/Program.cs` reads `DATABASE_URL` and
expects a PostgreSQL URL such as `postgres://user:password@host:5432/database`.

The React client can override the API base URL with
`client/.env.development`:

```text
REACT_APP_BASE_URL=http://localhost:5000/api
```

`client/.env.production` uses the relative `/api/` route.

## Run locally

### 1. Install the client dependencies

```bash
cd client
npm install
```

### 2. Start PostgreSQL

Create a database named `learnify` or choose another name in the connection
string. Create the required PostgreSQL EF provider package if it is not already
added to `API/API.csproj` or `Infrastructure/Infrastructure.csproj`:

```bash
cd API
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
```

### 3. Start the API

In a separate terminal, from the repository root:

```bash
dotnet run --project API
```

The API uses `http://localhost:5000` from `API/Properties/launchSettings.json`.
Swagger is available at `http://localhost:5000/swagger` in Development.

### 4. Start the React app

In another terminal:

```bash
cd client
npm start
```

Open `http://localhost:3000`.

## Run as a combined build

Build the React app into `API/wwwroot`, then run the ASP.NET Core application:

```bash
cd client
npm install
npm run build

cd ..
dotnet run --project API
```

The production React build uses relative `/api/` requests and ASP.NET Core serves
the built SPA through `app.UseDefaultFiles`, `app.UseStaticFiles`, and the
fallback controller.

## Available scripts

### React

```bash
npm start   # Start the development server
npm run build
npm test
npm run eject
```

### ASP.NET Core

```bash
dotnet restore
dotnet build
dotnet run --project API
dotnet ef migrations add <MigrationName> --project Infrastructure --startup-project API
dotnet ef database update --project Infrastructure --startup-project API
```

## Main API routes

| Route | Purpose |
| --- | --- |
| `POST /api/users/login` | Sign in |
| `POST /api/users/register` | Register a student account |
| `GET /api/users/currentUser` | Load the current user |
| `GET /api/courses` | Search, filter, and paginate courses |
| `GET /api/courses/{id}` | Get course details |
| `POST /api/courses` | Create a course (Instructor) |
| `POST /api/courses/publish/{courseId}` | Publish a course (Instructor) |
| `GET /api/categories` | List categories |
| `GET /api/categories/{id}` | Get a category and its courses |
| `GET /api/basket` | Read the current basket |
| `POST /api/basket?courseId={id}` | Add a course to the basket |
| `DELETE /api/basket?courseId={id}` | Remove a course from the basket |
| `POST /api/payments` | Create or update a Stripe PaymentIntent |
| `POST /api/users/purchaseCourses` | Record purchased courses |
| `GET /api/lectures/{courseId}` | Get course sections and lectures |
| `PUT /api/lectures/setCurrentLecture` | Update lecture progress |

## Repository layout

```text
API/                 ASP.NET Core API
Entity/              Domain models and specifications
Infrastructure/      EF Core, repositories, migrations, seeds, services
client/              React and TypeScript client
learnify.sln         Visual Studio solution
```
