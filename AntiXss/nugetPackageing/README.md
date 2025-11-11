# AntiXssAdvanced

Advanced Anti-XSS Helper middleware.

## Usage

```csharp
app.UseAntiXss(o =>
    o.excludedApis = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "/GetData",
        "/Controller/AddData"
    }
);
