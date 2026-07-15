# Reglas de Implementacion - Enterprise Sales Predictor

## Objetivo

Estas reglas deben respetarse durante toda la implementacion de `Enterprise Sales Predictor`.

## Arquitectura objetivo

La solucion debe separarse en los siguientes proyectos:

- `EnterpriseSalesPredictor.Domain`
- `EnterpriseSalesPredictor.Application`
- `EnterpriseSalesPredictor.Infrastructure`
- `EnterpriseSalesPredictor.Web`
- `EnterpriseSalesPredictor.Api`
- `EnterpriseSalesPredictor.Tests.Unit`
- `EnterpriseSalesPredictor.Tests.Integration`

## Topologia

- `Web` contiene vistas Razor, navegacion, formularios, dashboards y experiencia de usuario.
- `Api` contiene procesamiento, endpoints, carga de archivos, consultas analiticas, forecasting y abastecimiento.
- `Domain` contiene entidades, reglas de negocio e invariantes.
- `Application` contiene commands, queries, handlers, validators, DTOs y contratos.
- `Infrastructure` contiene persistencia MySQL, EF Core, repositorios, UnitOfWork, parsing, exportacion e integraciones.

## Reglas obligatorias

### 1. Separacion de responsabilidades
- `Web` NO debe contener logica de negocio.
- `Web` NO debe acceder directamente a MySQL ni a `DbContext`.
- `Web` NO debe procesar archivos pesados.
- `Api` NO debe contener logica de dominio dentro de controllers.
- Toda logica de negocio debe vivir en `Application` y `Domain`.
- Toda persistencia e integracion debe vivir en `Infrastructure`.

### 2. Patrones obligatorios
- Usar `Clean Architecture`.
- Usar `CQRS`.
- Usar `Result<T>` o `Result` para flujo de negocio esperado.
- Usar `UnitOfWork`.
- Evitar excepciones para flujo normal de negocio.

### 3. Stack tecnico obligatorio
- Runtime: `.NET 8`
- Frontend: `ASP.NET MVC + Razor Views`
- Backend: `ASP.NET Core Web API`
- Base de datos: `MySQL`
- CSS: `Tailwind CSS`
- JavaScript: `Vanilla JS` con module pattern y namespaces explicitos
- Testing: `NUnit + Moq`

### 4. Convenciones de lenguaje
- Codigo fuente en ingles.
- Documentacion y artefactos SDD en español.

### 5. Reglas para Web
- Cada vista Razor debe tener su archivo JS propio.
- El JavaScript debe estar organizado por vista y por modulos reutilizables.
- No usar frameworks SPA.
- No mezclar logica de negocio en vistas.
- `Web` consume `Api` para consultas dinamicas, procesamiento y operaciones de negocio.

### 6. Reglas para Api
- La `Api` debe validar autenticacion y autorizacion real.
- La `Api` debe centralizar procesamiento de archivos.
- La `Api` debe exponer endpoints para:
  - seguridad
  - usuarios
  - roles
  - permisos
  - cargas
  - consultas de ventas
  - dashboard
  - reportes
  - exportacion
  - forecasting
  - abastecimiento
  - auditoria

### 7. Reglas de persistencia
- Toda persistencia va sobre `MySQL`.
- Diseñar indices para fechas, cliente, producto, proveedor y vendedor.
- Aplicar constraints de integridad.
- Toda escritura debe pasar por repositorios o `UnitOfWork`.

### 8. Reglas de seguridad
- No confiar solo en restricciones visuales de `Web`.
- La seguridad real se impone en `Api`.
- Validar entradas, archivos y permisos.
- Proteger exportaciones, endpoints internos y operaciones sensibles.
- Auditar accesos, cargas, exportaciones, proyecciones y aprobaciones.

### 9. Reglas de carga de archivos
- Soportar Excel y archivos delimitados por `;`.
- Validar extension, tamaño, estructura y contenido.
- Normalizar datos antes de persistir.
- Detectar duplicados e inconsistencias.
- Registrar historial y errores de carga.

### 10. Reglas de analitica y reportes
- Soportar consultas por rango de fechas.
- Soportar analisis por cliente, producto, proveedor, vendedor, ciudad y zona.
- Entregar reportes:
  - gerenciales
  - comerciales
  - operativos
  - de abastecimiento
  - predictivos con IA
- Usar paginacion y filtros en consultas intensivas.

### 11. Reglas de forecasting
- Permitir proyecciones por rango de fechas.
- Horizonte minimo: `1 dia`.
- Horizonte maximo: `1 año`.
- La proyeccion debe entregar salida interpretable para negocio.
- Registrar auditoria de generacion.

### 12. Reglas de abastecimiento
- Las recomendaciones deben considerar:
  - historicos de venta
  - stock actual
  - tendencia
- Las recomendaciones NO generan compras automaticas.
- Solo pueden aprobar:
  - `gerente de compras`
  - `jefe de almacen`

### 13. Reglas de testing
- Probar `Domain`, `Application`, validadores, reglas de forecasting y reglas de abastecimiento.
- Probar integracion de `Api`, persistencia MySQL, carga de archivos y autorizacion.
- Mantener pruebas alineadas con `NUnit + Moq`.

## Orden de implementacion

1. Base tecnica y estructura de solucion
2. Seguridad y control de acceso
3. Modelo comercial y persistencia MySQL
4. Carga y validacion de historicos
5. Historial y auditoria
6. Consultas analiticas
7. Dashboard y reportes
8. Exportacion y gobierno
9. Forecasting
10. Recomendaciones de abastecimiento
11. Testing y hardening final

## Regla final

Si una implementacion viola la separacion entre `Web`, `Api`, `Application`, `Domain` e `Infrastructure`, debe corregirse antes de continuar.
