# AmberTower Backend Code Style

## Mandatory Rule

- Follow this code style document for every backend change.
- Code style requirements here are mandatory, not optional suggestions.

## Program.cs And Endpoint Mapping

- `Program.cs` must not contain endpoint declarations.
- Do not place `MapGet`, `MapPost`, `MapPatch`, `MapPut`, `MapDelete`, `MapMethods`, `MapGrpcService`, or route group declarations directly in `Program.cs`.
- `Program.cs` may only wire application setup together by calling extension methods such as `Add...` and `Map...`.
- Service-level root endpoints such as `/` and `/health` must also be moved into extension methods and must not stay inline in `Program.cs`.
- For backend services and ApiGateway, keep endpoint mapping in dedicated extension classes under an `Extensions/` folder or in the endpoint group root when that matches the existing local pattern.

## API Gateway Endpoint Organization

- Keep each endpoint group in its own domain folder under the owning API project.
- Keep the endpoint mapping class in the domain folder root.
- Keep request DTOs in a `Requests/` subfolder.
- Keep response DTOs in a `Responses/` subfolder.
- Use matching namespaces for these subfolders.

Example:

```text
src/
  ApiGateway/
    Auth/
      AuthEndpoints.cs
      Requests/
        LoginHttpRequest.cs
      Responses/
        LoginHttpResponse.cs
        ApiErrorResponse.cs
```

Expected namespaces:

```csharp
namespace ApiGateway.Auth;
namespace ApiGateway.Auth.Requests;
namespace ApiGateway.Auth.Responses;
```
