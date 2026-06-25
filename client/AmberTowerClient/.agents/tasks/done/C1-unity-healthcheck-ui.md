# Unity Healthcheck UI

## Goal

Create a minimal UI Toolkit screen for checking backend availability from the Unity client.

## Outcome

1. A form exists with:
   - address input field
   - health-check button
   - status indicator with yellow / green / red states
2. Backend request logic:
   - reads the address from the input field
   - sends an HTTP request
   - updates the status indicator based on the result
3. The panel handles failed requests and request exceptions without hanging in the loading state.
4. Client-side code style rules were documented in `.agents/CODESTYLE.md` and applied to the current implementation.
