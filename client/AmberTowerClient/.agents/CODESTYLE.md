# AmberTower Unity Client Code Style

## Mandatory Rule

- Follow this code style document for every client change.
- Code style requirements here are mandatory, not optional suggestions.

## C# Field Naming

- Use `_fieldName` for private fields.
- Exception: if a private field is marked with `[SerializeField]`, keep its name without the `_` prefix.

Examples:

```csharp
private readonly BackendApi _backendApi = new BackendApi();

[SerializeField]
private string defaultHealthUrl = "http://localhost:8080/health";
```

## Class Placement

- Each standalone class must be placed in its own file.
- Do not keep unrelated response, request, DTO, or helper classes as secondary classes inside a service file when they represent their own entity.

## Infrastructure DTO Organization

- When request/response objects appear in the infrastructure layer, keep them in dedicated folders:

```text
Assets/
  Src/
    Infrastructure/
      Requests/
      Responses/
```

- Do not mix request and response classes directly into the main API service class file unless the object is truly trivial and explicitly accepted as a local exception.

## Composition Root Rule

- Keep composition root files small and declarative.
- Do not place feature behavior, screen logic, or networking logic inline in startup/bootstrap entry files.
- Entry files should only compose existing services, presenters, views, and configuration objects through clearly named methods.
