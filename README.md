# API Granja Lechera

API Web en .NET 10 con arquitectura en capas (Domain, Infrastructure, Services, Web) para una granja lechera: registra la producción diaria de leche (n × l litros), la transforma en queso o yogurt, lleva el inventario por lotes con vencimiento, vende siempre el producto más antiguo no vencido (FIFO) y genera reportes de ventas, leche, producto y vacas.

## Ejecutar

Desde la carpeta `Web` (la migración se crea una sola vez):

```powershell
dotnet restore .\ApiSampleFinal.sln
dotnet ef migrations add Inicial `
  --project ..\Infrastructure\Infrastructure\Infrastructure.csproj `
  --startup-project .\ApiSampleFinal.csproj `
  --context Infrastructure.Data.AppDbContext `
  --output-dir Data/Migrations
dotnet run --project .\ApiSampleFinal.csproj
```

Al arrancar se aplican las migraciones y se crea la granja con los valores sugeridos del taller (SQLite, archivo `granja.db`). Swagger: `https://localhost:57199/swagger`.

## Probar

- Swagger, o `Web/ApiSampleFinal.http`, que recorre el escenario de referencia del taller.
- Pruebas unitarias: `dotnet test .\ApiSampleFinal.sln`
