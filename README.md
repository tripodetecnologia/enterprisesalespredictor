# Enterprise Sales Predictor

Enterprise Sales Predictor es una plataforma web para gestionar, analizar y proyectar ventas empresariales. La solucion permite cargar historicos de ventas, consultar informacion comercial, visualizar indicadores, generar reportes, exportar datos, proyectar demanda y obtener recomendaciones de abastecimiento con trazabilidad y control de acceso.

La aplicacion esta separada en dos aplicaciones desplegables: una Web MVC con Razor Views para la experiencia de usuario y una API REST para autenticacion, procesamiento, persistencia, analitica y reglas de negocio.

## Acceso de Prueba

La aplicacion web se encuentra desplegada en Azure App Service:

```text
https://enterprisesalespredictor-cbcrfgdkdjgdfacv.canadacentral-01.azurewebsites.net/
```

Credenciales de prueba:

| Campo | Valor |
|-------|-------|
| Usuario | `admin` |
| Contrasena | `bigadmin@123` |

## Descripcion General

El proyecto implementa una solucion empresarial orientada a la toma de decisiones comerciales y operativas. Su objetivo principal es centralizar historicos de ventas y convertirlos en informacion util para analisis, reportes, forecasting y abastecimiento.

El sistema esta organizado bajo principios de Clean Architecture, separando presentacion, API, casos de uso, reglas de dominio e infraestructura. Esta separacion permite que la interfaz web no procese logica de negocio pesada ni acceda directamente a la base de datos; esas responsabilidades viven en la API y en las capas de aplicacion, dominio e infraestructura.

Flujo general de uso:

1. El usuario inicia sesion en la aplicacion Web.
2. La Web consume endpoints protegidos de la API.
3. La API valida autenticacion, autorizacion y entradas.
4. La capa Application coordina contratos, DTOs, validaciones y resultados.
5. La capa Infrastructure persiste y consulta informacion en MySQL.
6. La Web muestra resultados, reportes, estados, tablas y acciones disponibles.

## Stack Tecnologico

| Area | Tecnologia |
|------|------------|
| Runtime | .NET 8 |
| Backend | ASP.NET Core Web API |
| Frontend web | ASP.NET Core MVC + Razor Views |
| Arquitectura | Clean Architecture, CQRS, Result, Unit of Work |
| Persistencia | Entity Framework Core 8 |
| Base de datos | MySQL |
| Provider MySQL | Pomelo.EntityFrameworkCore.MySql |
| Autenticacion API | JWT Bearer |
| Autenticacion Web | Cookies de ASP.NET Core |
| Autorizacion | Politicas por permisos |
| Documentacion API | Swagger / Swashbuckle |
| Estilos | Tailwind CSS |
| JavaScript | Vanilla JS organizado por paginas, modulos y utilidades |
| Archivos Excel | ClosedXML |
| Testing | NUnit, Moq, Microsoft.AspNetCore.Mvc.Testing, EF Core InMemory |
| Despliegue | Azure App Service |

## Despliegue en Azure

El proyecto contempla dos App Services independientes:

| Componente | Descripcion | URL |
|------------|-------------|-----|
| Web | Sitio MVC/Razor usado por los usuarios finales | `https://enterprisesalespredictor-cbcrfgdkdjgdfacv.canadacentral-01.azurewebsites.net/` |
| API | Backend HTTP consumido por la Web | `https://apienterprisesalespredictor-dbcda0degzczcbbc.eastus-01.azurewebsites.net` |

La Web consume la API mediante la configuracion `Api:BaseUrl`. En el entorno desplegado, este valor apunta al App Service de la API.

La API utiliza MySQL como almacenamiento persistente. Al iniciar, si el proveedor de base de datos es relacional, la aplicacion ejecuta las migraciones de Entity Framework Core mediante `MigrateAsync()` y luego ejecuta el bootstrap de seguridad para asegurar roles, permisos y usuarios configurados.

## Instalacion Local

### Requisitos Previos

Para ejecutar el proyecto localmente se necesita:

| Herramienta | Uso |
|-------------|-----|
| .NET SDK 8 o superior | Compilar y ejecutar la solucion |
| Node.js y npm | Instalar Tailwind CSS y generar estilos |
| MySQL 8 | Base de datos local o remota |
| Visual Studio 2022, Rider o VS Code | Entorno de desarrollo recomendado |

### Clonar el Repositorio

```powershell
git clone <url-del-repositorio>
cd Codigo
```

### Restaurar Dependencias .NET

```powershell
dotnet restore EnterpriseSalesPredictor.slnx
```

### Instalar Dependencias Frontend

```powershell
cd src/EnterpriseSalesPredictor.Web
npm install
npm run build:css
cd ../..
```

El comando `npm run build:css` genera el archivo CSS final usado por la aplicacion Web a partir de Tailwind CSS.

### Configurar la API

La API lee su configuracion desde `src/EnterpriseSalesPredictor.Api/appsettings.json`, `appsettings.Development.json`, variables de entorno o configuracion del host.

Valores principales:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=<host>;Port=3306;Database=<database>;User Id=<user>;Password=<password>;"
  },
  "Database": {
    "Provider": "MySql",
    "CommandTimeoutSeconds": 30
  },
  "Authentication": {
    "Jwt": {
      "Issuer": "EnterpriseSalesPredictor",
      "Audience": "EnterpriseSalesPredictor.Clients",
      "SigningKey": "<clave-segura-de-al-menos-32-caracteres>",
      "ExpirationMinutes": 60
    }
  }
}
```

En ambientes distintos de Development, la API requiere una cadena de conexion valida y una clave JWT segura de al menos 32 caracteres.

### Configurar la Web

La Web lee su configuracion desde `src/EnterpriseSalesPredictor.Web/appsettings.json`, `appsettings.Development.json`, variables de entorno o configuracion del host.

Valores principales:

```json
{
  "Api": {
    "BaseUrl": "https://localhost:7197"
  },
  "Authentication": {
    "Jwt": {
      "Issuer": "EnterpriseSalesPredictor",
      "Audience": "EnterpriseSalesPredictor.Clients",
      "SigningKey": "<misma-clave-o-configuracion-compatible-con-la-api>"
    }
  }
}
```

Para ejecutar localmente, `Api:BaseUrl` debe apuntar a la URL local donde se este ejecutando `EnterpriseSalesPredictor.Api`.

## Ejecucion Local

### Ejecutar la API

Desde la raiz del repositorio:

```powershell
dotnet run --project src/EnterpriseSalesPredictor.Api/EnterpriseSalesPredictor.Api.csproj --launch-profile https
```

Perfiles locales disponibles para la API:

| Perfil | URL |
|--------|-----|
| `http` | `http://localhost:5150` |
| `https` | `https://localhost:7197` y `http://localhost:5150` |

Swagger queda disponible en:

```text
https://localhost:7197/swagger
```

### Ejecutar la Web

En otra terminal, desde la raiz del repositorio:

```powershell
dotnet run --project src/EnterpriseSalesPredictor.Web/EnterpriseSalesPredictor.Web.csproj --launch-profile https
```

Perfiles locales disponibles para la Web:

| Perfil | URL |
|--------|-----|
| `http` | `http://localhost:5131` |
| `https` | `https://localhost:7198` y `http://localhost:5131` |

La pantalla de login local queda disponible en:

```text
https://localhost:7198/Auth/Login
```

## Compilacion y Pruebas

### Compilar la Solucion

```powershell
dotnet build EnterpriseSalesPredictor.slnx
```

### Ejecutar Pruebas

```powershell
dotnet test EnterpriseSalesPredictor.slnx
```

El repositorio incluye proyectos de pruebas unitarias e integracion:

| Proyecto | Proposito |
|----------|-----------|
| `EnterpriseSalesPredictor.Tests.Unit` | Pruebas de reglas de dominio, servicios, validaciones y componentes de infraestructura aislados |
| `EnterpriseSalesPredictor.Tests.Integration` | Pruebas de integracion sobre API, configuracion y persistencia con EF Core InMemory |

## Estructura del Proyecto

```text
Codigo/
  EnterpriseSalesPredictor.slnx
  dotnet-tools.json
  README.md
  openspec/
    changes/
    specs/
  src/
    EnterpriseSalesPredictor.Api/
    EnterpriseSalesPredictor.Application/
    EnterpriseSalesPredictor.Domain/
    EnterpriseSalesPredictor.Infrastructure/
    EnterpriseSalesPredictor.Web/
  tests/
    EnterpriseSalesPredictor.Tests.Integration/
    EnterpriseSalesPredictor.Tests.Unit/
```

### Capas Principales

| Proyecto | Responsabilidad |
|----------|-----------------|
| `EnterpriseSalesPredictor.Domain` | Entidades, reglas de negocio, invariantes y politicas de dominio |
| `EnterpriseSalesPredictor.Application` | DTOs, contratos, interfaces, validadores, resultados y casos de uso |
| `EnterpriseSalesPredictor.Infrastructure` | EF Core, MySQL, repositorios, Unit of Work, seguridad, parsing de archivos, auditoria, reportes, forecasting y exportacion |
| `EnterpriseSalesPredictor.Api` | Endpoints HTTP, autenticacion JWT, autorizacion, Swagger y middlewares |
| `EnterpriseSalesPredictor.Web` | Controllers MVC, Razor Views, ViewModels, clientes HTTP, estilos y JavaScript de interfaz |
| `EnterpriseSalesPredictor.Tests.Unit` | Pruebas unitarias |
| `EnterpriseSalesPredictor.Tests.Integration` | Pruebas de integracion |

### Estructura Web

```text
src/EnterpriseSalesPredictor.Web/
  Controllers/
  Views/
  ViewModels/
  Services/
  Configuration/
  Styles/
  wwwroot/
    css/
    js/
      pages/
      modules/
      utils/
```

La Web utiliza controllers MVC y vistas Razor. El JavaScript esta separado por paginas, modulos reutilizables y utilidades generales.

### Estructura API

```text
src/EnterpriseSalesPredictor.Api/
  Authorization/
  Controllers/
  Middlewares/
  Program.cs
```

La API expone endpoints para autenticacion, seguridad, cargas, ventas, dashboard, reportes, exportaciones, forecasting, abastecimiento, auditoria y salud del servicio.

### Estructura Infrastructure

```text
src/EnterpriseSalesPredictor.Infrastructure/
  Auditing/
  Dashboard/
  Exports/
  FileProcessing/
  Forecasting/
  Persistence/
    Configurations/
    Migrations/
  Replenishment/
  Reports/
  Repositories/
  Sales/
  Security/
```

Infrastructure concentra las implementaciones concretas de persistencia, archivos, reportes, seguridad y servicios operativos.

## Funcionalidades Principales

### Autenticacion y Autorizacion

El sistema permite iniciar sesion y acceder a modulos segun roles y permisos. La API protege los endpoints mediante JWT Bearer y politicas de autorizacion por permisos. La Web usa cookies para mantener la sesion del usuario.

Modulos relacionados:

| Modulo | Descripcion |
|--------|-------------|
| Login | Autenticacion de usuarios |
| Usuarios | Creacion y administracion de usuarios |
| Roles | Administracion de roles |
| Permisos | Asignacion de permisos a roles |

### Carga de Historicos

La aplicacion permite cargar historicos de ventas desde archivos Excel y archivos delimitados por punto y coma (`;`). La API valida extension, estructura, contenido, datos obligatorios, duplicados e inconsistencias antes de persistir registros validos.

Capacidades principales:

| Capacidad | Descripcion |
|-----------|-------------|
| Carga Excel | Procesamiento de archivos Excel mediante ClosedXML |
| Carga delimitada | Procesamiento de archivos separados por `;` |
| Validacion | Control de encabezados, fechas, campos requeridos y datos invalidos |
| Historial | Registro del estado de cada carga |
| Errores | Consulta de errores detectados durante el procesamiento |

### Auditoria

El sistema registra operaciones relevantes para mantener trazabilidad funcional y tecnica.

Eventos auditables:

| Evento | Ejemplo |
|--------|---------|
| Cargas | Archivos procesados, estados y errores |
| Exportaciones | Reportes o datos descargados |
| Forecasting | Proyecciones generadas |
| Abastecimiento | Recomendaciones y revisiones |
| Accesos protegidos | Operaciones restringidas por permisos |

### Consultas de Ventas

La aplicacion permite consultar ventas con filtros y paginacion. Los criterios principales incluyen rango de fechas y dimensiones comerciales.

Dimensiones soportadas:

| Dimension | Uso |
|-----------|-----|
| Cliente | Analisis de ventas por cliente |
| Producto | Analisis por producto o referencia |
| Proveedor | Analisis por proveedor |
| Vendedor | Analisis por responsable comercial |
| Ciudad | Agrupacion geografica |
| Zona | Segmentacion territorial |

### Dashboard

La pantalla principal presenta indicadores de negocio, comparativos, alertas y desgloses para facilitar una lectura rapida del estado comercial.

Elementos principales:

| Elemento | Descripcion |
|----------|-------------|
| KPIs | Indicadores sinteticos de ventas y desempeno |
| Comparativos | Evolucion y contraste entre periodos |
| Alertas | Senales sobre variaciones o condiciones relevantes |
| Desgloses | Distribucion por dimensiones comerciales |

### Reportes

El sistema genera reportes para distintas necesidades de negocio.

Tipos de reportes:

| Tipo | Enfoque |
|------|---------|
| Gerenciales | Vision ejecutiva del negocio |
| Comerciales | Ventas, clientes, productos y vendedores |
| Operativos | Informacion de soporte operativo |
| Abastecimiento | Insumos para decisiones de inventario |
| Predictivos | Informacion derivada de forecasting |

### Exportaciones

Los usuarios autorizados pueden exportar informacion filtrada y reportes. Las exportaciones se generan desde la API y quedan auditadas.

La funcionalidad de exportacion usa ClosedXML para generar archivos Excel.

### Forecasting de Ventas

El modulo de forecasting permite generar proyecciones de ventas a partir de historicos disponibles y filtros definidos por el usuario.

Reglas principales:

| Regla | Valor |
|-------|-------|
| Horizonte minimo | 1 dia |
| Horizonte maximo | 1 ano |
| Salida esperada | Proyeccion interpretable para negocio |
| Auditoria | Registro de generacion de proyeccion |

### Recomendaciones de Abastecimiento

El sistema genera recomendaciones de abastecimiento considerando historicos de venta, stock actual, tendencia y horizonte de analisis.

Flujo principal:

1. El usuario genera una recomendacion.
2. El sistema calcula la recomendacion y la registra.
3. La recomendacion queda en estado pendiente.
4. Un usuario autorizado puede aprobarla, rechazarla o marcarla para analisis.

Regla de negocio destacada:

| Accion | Restriccion |
|--------|-------------|
| Aprobar recomendacion | Solo perfiles autorizados, como gerente de compras o jefe de almacen |

## Endpoints Principales de la API

| Area | Ruta base | Descripcion |
|------|-----------|-------------|
| Salud | `/api/health` | Verificacion basica del servicio |
| Autenticacion | `/api/auth` | Login y emision de token |
| Acceso | `/api/access` | Usuarios, roles y permisos |
| Cargas | `/api/uploads` | Carga, procesamiento, historial y errores |
| Ventas | `/api/sales` | Consultas analiticas de ventas |
| Dashboard | `/api/dashboard` | KPIs, alertas y desgloses |
| Reportes | `/api/reports` | Reportes gerenciales y operativos |
| Exportaciones | `/api/exports` | Generacion de archivos exportables |
| Forecasting | `/api/forecasts` | Proyecciones de ventas |
| Abastecimiento | `/api/replenishment` | Recomendaciones y aprobaciones |
| Auditoria | `/api/audit` | Consulta de eventos auditados |

## Seguridad

La seguridad se aplica principalmente en la API. La Web puede ocultar acciones segun permisos, pero las restricciones reales se validan en los endpoints protegidos.

Controles relevantes:

| Control | Descripcion |
|---------|-------------|
| JWT Bearer | Protege la API |
| Cookies | Mantiene la sesion Web |
| Politicas por permiso | Restringen endpoints segun capacidad del usuario |
| Validacion de archivos | Evita procesar archivos invalidos o inconsistentes |
| Auditoria | Registra operaciones criticas |
| Validacion de configuracion | Rechaza claves JWT inseguras o configuraciones incompletas fuera de Development |

## Base de Datos

La persistencia principal se realiza sobre MySQL mediante Entity Framework Core y Pomelo.EntityFrameworkCore.MySql.

Entidades principales:

| Entidad | Proposito |
|---------|-----------|
| `Users` | Usuarios del sistema |
| `Roles` | Roles asignables |
| `Permissions` | Permisos disponibles |
| `UserRoles` | Relacion usuarios-roles |
| `RolePermissions` | Relacion roles-permisos |
| `Customers` | Clientes |
| `Products` | Productos |
| `Suppliers` | Proveedores |
| `Sellers` | Vendedores |
| `Sales` | Historico de ventas |
| `UploadedFiles` | Sesiones de carga |
| `UploadErrors` | Errores de carga |
| `AuditLogs` | Auditoria |
| `Forecasts` | Proyecciones generadas |
| `ReplenishmentRecommendations` | Recomendaciones de abastecimiento |

## Convenciones del Proyecto

| Convencion | Descripcion |
|------------|-------------|
| Codigo fuente | Ingles |
| Documentacion funcional y SDD | Espanol |
| Web | No contiene logica de negocio ni acceso directo a MySQL |
| API | Centraliza endpoints, seguridad y coordinacion de casos de uso |
| Application | Expone contratos, DTOs y resultados |
| Domain | Contiene reglas e invariantes |
| Infrastructure | Implementa persistencia, repositorios, archivos, reportes y seguridad |

## Notas Importantes

No se recomienda versionar secretos reales como cadenas de conexion, passwords productivos o claves JWT. En ambientes locales, de prueba y produccion, estos valores deberian configurarse mediante variables de entorno, secretos de usuario, Azure App Service Configuration o un servicio seguro de gestion de secretos.

La URL publica de prueba y las credenciales incluidas en este README corresponden al acceso demo indicado para validar el funcionamiento de la aplicacion desplegada.
