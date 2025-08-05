# LogIn-Advanced API

This repository now contains an ASP.NET Core Web API (`Api` project) alongside the original WPF application.
The API exposes the same user management logic through OpenAPI (Swagger) so endpoints can be tested via tools such as Postman.

## Building the API

```bash
# build only the API project
dotnet build Api/Api.csproj -c Release
```

## Running

```bash
# run the API
cd Api
dotnet run
```

When running, the API serves a static OpenAPI document with Swagger UI at `http://localhost:5000/`.

## Available Endpoints

- `POST /login` – verify username and password.
- `GET /users` – list all users.
- `GET /users/{username}` – get a specific user.
- `POST /users` – create a new user.
- `PUT /users/{id}` – update an existing user.
- `DELETE /users/{username}` – delete a user.
- `PUT /users/{id}/description` – update only the description field.

The same models and data access logic from the WPF application were kept so behavior matches the UI version.
