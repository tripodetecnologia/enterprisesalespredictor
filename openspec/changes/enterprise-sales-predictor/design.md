# Design - Enterprise Sales Predictor

## Resumen ejecutivo

La solucion se implementara como una plataforma en .NET 8 con arquitectura limpia y topologia separada entre frontend Razor y backend API. Esta separacion permite mantener la experiencia de usuario en `Web` y delegar el procesamiento, las consultas, el forecasting y el abastecimiento a `Api`.

## Topologia de la solucion

```text
EnterpriseSalesPredictor.sln
src/
  EnterpriseSalesPredictor.Domain
  EnterpriseSalesPredictor.Application
  EnterpriseSalesPredictor.Infrastructure
  EnterpriseSalesPredictor.Web
  EnterpriseSalesPredictor.Api
tests/
  EnterpriseSalesPredictor.Tests.Unit
  EnterpriseSalesPredictor.Tests.Integration
```

## Responsabilidades por proyecto

### Domain
- entidades
- value objects
- reglas de negocio
- invariantes
- eventos de dominio si aplican

### Application
- commands
- queries
- handlers
- validators
- DTOs
- contratos de repositorio y servicios
- respuestas `Result<T>`

### Infrastructure
- EF Core
- MySQL
- repositorios
- UnitOfWork
- parsing de Excel y archivos `;`
- exportacion a Excel
- forecasting y servicios predictivos
- logging y auditoria

### Web
- controllers MVC
- vistas Razor
- view models
- view components
- experiencia visual
- JS modular por vista
- consumo de `Api`

### Api
- controllers HTTP
- endpoints internos de negocio
- autenticacion y autorizacion backend
- coordinacion de commands y queries
- procesamiento de archivos
- consultas analiticas
- forecasting
- abastecimiento

## Flujo principal

1. El usuario interactua con `Web`.
2. `Web` consume `Api` para procesamiento o consulta.
3. `Api` delega en `Application`.
4. `Application` usa contratos implementados por `Infrastructure`.
5. `Infrastructure` consulta o persiste en MySQL.
6. `Api` devuelve resultados.
7. `Web` representa la respuesta al usuario.

## Principios tecnicos

- Clean Architecture
- SOLID
- DRY
- CQRS
- Result<T>
- UnitOfWork
- bajo acoplamiento
- alta cohesion

## Reglas arquitectonicas

- `Web` no contiene logica de negocio.
- `Web` no accede a MySQL ni a `DbContext`.
- `Api` no contiene logica de dominio en controllers.
- toda lectura y escritura pasa por `Application`
- las operaciones de negocio retornan `Result<T>` o `Result`
- las excepciones se reservan para fallos tecnicos o inesperados

## Estructura sugerida por capas

### Application
```text
Commands/
Queries/
Handlers/
DTOs/
Validators/
Interfaces/
Results/
Services/
```

### Infrastructure
```text
Persistence/
Repositories/
UnitOfWork/
FileProcessing/
Excel/
DelimitedFiles/
Exports/
Forecasting/
Security/
Audit/
```

### Web
```text
Controllers/
Views/
ViewModels/
ViewComponents/
wwwroot/js/pages/
wwwroot/js/modules/
wwwroot/js/utils/
wwwroot/css/
```

### Api
```text
Controllers/
Contracts/
RequestModels/
ResponseModels/
Filters/
Middlewares/
Configuration/
```

## Diseño de persistencia MySQL

### Entidades principales
- Users
- Roles
- Permissions
- UserRoles
- RolePermissions
- Customers
- Products
- Suppliers
- Sellers
- Sales
- UploadedFiles
- UploadErrors
- AuditLogs
- ExportLogs
- Forecasts
- ReplenishmentRecommendations
- RecommendationReviews
- SystemParameters

### Indices sugeridos
- `Sales(SaleDate)`
- `Sales(CustomerId, SaleDate)`
- `Sales(ProductId, SaleDate)`
- `Sales(SupplierId, SaleDate)`
- `Sales(SellerId, SaleDate)`
- `Products(Reference)`
- `Customers(Identification)`
- `Suppliers(Identification)`

## Diseño de seguridad

### Web
- protege navegacion
- oculta acciones segun permisos
- presenta errores controlados

### Api
- autentica y autoriza cada endpoint protegido
- valida payloads
- audita accesos y operaciones
- protege exportaciones y endpoints internos

## Diseño de carga de archivos

### Web
- formulario de carga
- selector o drag and drop
- resumen visual de procesamiento
- historial y detalle de errores

### Api
- valida extension, tamaño y estructura
- parsea Excel y archivos `;`
- normaliza registros
- detecta duplicados
- persiste validos
- registra errores y resumen

## Diseño de consultas y reportes

### Api
- endpoints por rango, cliente, producto, proveedor, vendedor, ciudad o zona
- endpoints de KPIs y dashboard
- endpoints de reportes gerenciales, comerciales, operativos, de abastecimiento y predictivos

### Web
- vistas de consulta y filtros
- dashboard Razor
- tablas y graficas
- paneles de alertas y accesos rapidos

## Diseño de forecasting

### Entradas minimas
- historicos de ventas
- producto o conjunto filtrado
- rango de fechas
- comportamiento temporal

### Salidas
- proyeccion
- explicacion resumida
- nivel de confianza si aplica
- evidencia auditable

### Restricciones
- horizonte entre 1 dia y 1 año
- sin acciones automaticas de compra

## Diseño de abastecimiento

### Insumos
- historicos de venta
- stock actual
- tendencia
- horizonte solicitado

### Flujo
1. generar recomendacion
2. registrar recomendacion
3. dejarla en estado pendiente
4. permitir aprobacion, rechazo o revision

### Regla
Solo `gerente de compras` y `jefe de almacen` pueden aprobar.

## Diseño visual

La `Web` seguira una interfaz administrativa empresarial con:

- sidebar fija o colapsable
- header superior
- breadcrumbs
- dashboard con KPI cards
- vistas de consulta y filtros
- formularios administrativos
- pantallas de forecasting y abastecimiento

## Testing previsto

### Unit tests
- Domain
- Application
- Validators
- Result<T>
- reglas de forecasting
- reglas de abastecimiento

### Integration tests
- Api endpoints
- persistencia MySQL
- carga Excel y `;`
- exportacion
- autorizacion
- auditoria

## Decisiones basadas en prompts vs repositorio

### Basadas en prompts fuente
- stack .NET 8, Razor, Tailwind y Vanilla JS
- Clean Architecture, CQRS, Result<T>, UnitOfWork
- cobertura funcional completa del dominio de ventas, reportes e IA asistida

### Basadas en decisiones del usuario en esta sesion
- MySQL como persistencia
- separacion formal entre `Web` y `Api`
- forecasting por rango de fechas de 1 dia a 1 año
- abastecimiento con stock actual y aprobacion humana dual

### Basadas en evidencia del repositorio
- el workspace sigue en bootstrap documental
- no existe solucion .NET implementada todavia

## Resultado esperado

El diseño deja la base para implementar una solucion mantenible, escalable y alineada con los requerimientos de seguridad, analitica y gobierno de datos del producto.
