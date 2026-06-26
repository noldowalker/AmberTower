# AmberTower Backend Code Style

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
