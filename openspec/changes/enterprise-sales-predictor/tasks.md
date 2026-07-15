# Tasks - Enterprise Sales Predictor

## Contexto

Este backlog de tareas documenta la implementacion planificada de `Enterprise Sales Predictor` con una topologia separada entre:

- `EnterpriseSalesPredictor.Web` para vistas Razor y experiencia de usuario
- `EnterpriseSalesPredictor.Api` para procesamiento, endpoints y logica expuesta
- `EnterpriseSalesPredictor.Domain` para negocio puro
- `EnterpriseSalesPredictor.Application` para casos de uso
- `EnterpriseSalesPredictor.Infrastructure` para persistencia e integraciones

## Backlog Scrum

### Epica 1 - Fundacion de la plataforma

#### Feature 1.1 - Estructura de solucion
- [x] Crear solucion `.NET 8`
- [x] Crear proyectos `Domain`, `Application`, `Infrastructure`, `Web`, `Api`, `Tests.Unit`, `Tests.Integration`
- [x] Configurar referencias entre capas
- [x] Configurar DI base

#### Feature 1.2 - Configuracion base
- [x] Configurar `appsettings` por ambiente
- [x] Configurar conexion a MySQL
- [x] Configurar Tailwind en `Web`
- [x] Configurar estructura JS modular en `Web`
- [x] Configurar middlewares base en `Api`

#### Feature 1.3 - Componentes transversales
- [x] Implementar `Result<T>`
- [x] Implementar `UnitOfWork`
- [x] Implementar contratos base de repositorio
- [x] Implementar validaciones compartidas
- [x] Implementar manejo global de errores tecnicos

### Epica 2 - Seguridad, usuarios, roles y permisos

#### Feature 2.1 - Seguridad en Api
- [x] Implementar autenticacion backend
- [x] Implementar autorizacion por roles/permisos
- [x] Crear endpoints de login/autorizacion si aplica
- [x] Proteger endpoints internos
- [x] Implementar validacion de acceso por modulo/accion

#### Feature 2.2 - Seguridad en Web
- [x] Diseñar pantalla login Razor
- [x] Implementar formulario login
- [x] Implementar logout
- [x] Implementar navegacion protegida
- [x] Implementar paginas 403, 404 y error
- [x] Ocultar opciones visuales segun permisos

#### Feature 2.3 - Usuarios y roles
- [x] Implementar endpoints de usuarios, roles y permisos en `Api`
- [x] Implementar validaciones backend
- [x] Implementar vistas de usuarios, roles y permisos en `Web`
- [x] Implementar formularios y tablas administrativas

### Epica 3 - Modelo comercial y persistencia del dominio

#### Feature 3.1 - Dominio comercial
- [x] Modelar entidades `Customer`, `Product`, `Supplier`, `Seller`, `Sale`, `UploadedFile`, `UploadError`, `AuditLog`, `Forecast`, `ReplenishmentRecommendation`
- [x] Definir reglas de negocio
- [x] Definir DTOs y contratos de consulta/escritura

#### Feature 3.2 - Persistencia MySQL
- [x] Configurar `DbContext`
- [x] Configurar Fluent API
- [x] Crear migracion inicial
- [x] Crear tablas del dominio comercial
- [x] Crear indices iniciales
- [x] Implementar repositorios
- [x] Implementar `UnitOfWork` sobre EF Core

### Epica 4 - Carga de historicos de ventas

#### Feature 4.1 - Procesamiento de carga en Api
- [x] Implementar endpoint de carga Excel
- [x] Implementar endpoint de carga archivo `;`
- [x] Implementar validacion de extension, tamaño y encabezados
- [x] Implementar validacion de campos obligatorios
- [x] Implementar parser Excel
- [x] Implementar parser delimitado `;`
- [x] Implementar normalizacion de registros
- [x] Implementar deteccion de duplicados
- [x] Implementar registro de errores de carga
- [x] Implementar persistencia de datos validos
- [x] Implementar resumen de procesamiento

#### Feature 4.2 - Experiencia de carga en Web
- [x] Crear vista de carga de archivos
- [x] Mostrar formatos permitidos y limites de tamaño
- [x] Implementar selector o drag and drop
- [x] Mostrar progreso
- [x] Mostrar resumen de carga
- [x] Mostrar errores por procesamiento
- [x] Crear vista de historial de cargas
- [x] Crear vista de detalle de errores

### Epica 5 - Historial y trazabilidad operativa

#### Feature 5.1 - Auditoria en Api
- [x] Registrar historial de cargas
- [x] Registrar usuario ejecutor
- [x] Registrar fecha y estado
- [x] Registrar exportaciones
- [x] Registrar generacion de forecasting
- [x] Registrar recomendaciones de abastecimiento
- [x] Registrar aprobaciones y rechazos

#### Feature 5.2 - Auditoria en Web
- [x] Crear vista de auditoria de cargas
- [x] Crear vista de auditoria de exportaciones
- [x] Crear vista de auditoria funcional
- [x] Agregar filtros para auditoria

### Epica 6 - Consultas analiticas de ventas

#### Feature 6.1 - Endpoints analiticos en Api
- [x] Implementar endpoint de ventas por rango de fechas
- [x] Implementar endpoint por cliente
- [x] Implementar endpoint por producto
- [x] Implementar endpoint por proveedor
- [x] Implementar endpoint por vendedor
- [x] Implementar endpoint por zona o ciudad
- [x] Implementar endpoint de comparativos por año, mes, trimestre y semestre
- [x] Implementar paginacion, ordenamiento y filtros seguros
- [x] Optimizar consultas base

#### Feature 6.2 - Pantallas de consulta en Web
- [x] Crear vista de consulta de ventas
- [x] Crear panel de filtros
- [x] Crear tabla de resultados
- [x] Implementar estados de carga, vacio y error
- [x] Integrar consumo dinamico con `Api`

### Epica 7 - Dashboard y reportes empresariales

#### Feature 7.1 - Dashboard en Api
- [x] Implementar endpoint de KPIs principales
- [x] Implementar endpoint de top clientes
- [x] Implementar endpoint de top productos
- [x] Implementar endpoint de ventas por linea
- [x] Implementar endpoint de ventas por proveedor
- [x] Implementar endpoint de alertas comerciales

#### Feature 7.2 - Dashboard en Web
- [x] Construir dashboard Razor principal
- [x] Implementar KPI cards
- [x] Implementar graficas
- [x] Implementar paneles comparativos
- [x] Implementar accesos rapidos
- [x] Implementar panel de alertas

#### Feature 7.3 - Reportes en Api
- [x] Implementar endpoints de reportes gerenciales
- [x] Implementar endpoints de reportes comerciales
- [x] Implementar endpoints de reportes operativos
- [x] Implementar endpoints de reportes de abastecimiento
- [x] Implementar endpoints de reportes predictivos

#### Feature 7.4 - Reportes en Web
- [x] Crear vista de reportes gerenciales
- [x] Crear vista de reportes comerciales
- [x] Crear vista de reportes operativos
- [x] Crear vista de reportes de abastecimiento
- [x] Crear vista de reportes predictivos
- [x] Implementar filtros comunes reutilizables

### Epica 8 - Exportacion y gobierno de informacion

#### Feature 8.1 - Exportacion en Api
- [x] Seleccionar libreria de exportacion Excel
- [x] Implementar endpoint de exportacion de reportes
- [x] Implementar endpoint de exportacion de datos filtrados
- [x] Implementar endpoint de exportacion de datos base
- [x] Aplicar permisos por rol
- [x] Registrar auditoria de exportacion

#### Feature 8.2 - Exportacion en Web
- [x] Agregar botones de exportacion en vistas relevantes
- [x] Implementar confirmaciones visuales
- [x] Implementar estados de descarga
- [x] Implementar manejo de errores de exportacion

### Epica 9 - Forecasting de ventas

#### Feature 9.1 - Proyeccion en Api
- [x] Definir estrategia inicial de forecasting
- [x] Implementar servicio de proyeccion
- [x] Permitir rango de 1 dia a 1 año
- [x] Exponer endpoint de proyeccion
- [x] Persistir resultados de forecasting
- [x] Registrar auditoria
- [x] Exponer explicacion del resultado
- [x] Exponer nivel de confianza si aplica

#### Feature 9.2 - Proyeccion en Web
- [x] Crear pantalla Razor de proyecciones
- [x] Implementar filtros por rango de fechas
- [x] Implementar visualizacion grafica del resultado
- [x] Implementar resumen textual
- [x] Implementar estado de carga y error
- [x] Presentar confianza o explicacion

### Epica 10 - Recomendaciones de abastecimiento asistidas

#### Feature 10.1 - Recomendaciones en Api
- [x] Diseñar regla o modelo inicial de recomendacion
- [x] Consumir historicos de venta
- [x] Consumir stock actual
- [x] Calcular necesidad estimada
- [x] Detectar riesgo de agotamiento
- [x] Detectar baja rotacion
- [x] Generar recomendacion persistible
- [x] Exponer endpoint de recomendaciones

#### Feature 10.2 - Aprobacion en Api
- [x] Implementar endpoint para aprobar recomendacion
- [x] Implementar endpoint para rechazar recomendacion
- [x] Implementar endpoint para marcar analisis
- [x] Restringir aprobacion a gerente de compras y jefe de almacen
- [x] Auditar decision tomada

#### Feature 10.3 - Abastecimiento en Web
- [x] Crear vista de recomendaciones
- [x] Crear vista de detalle de recomendacion
- [x] Implementar acciones de aprobar, rechazar y revisar
- [x] Implementar estados pendiente, aprobada, rechazada, baja confianza y vencida

### Epica 11 - UX/UI empresarial

#### Feature 11.1 - Layout base
- [x] Implementar `_Layout.cshtml`
- [x] Implementar sidebar
- [x] Implementar header
- [x] Implementar breadcrumbs
- [x] Implementar footer
- [x] Implementar panel de notificaciones

#### Feature 11.2 - Componentes Web
- [x] Implementar botones, inputs, selects y filtros
- [x] Implementar cards y KPI cards
- [x] Implementar tablas y modales
- [x] Implementar toasts y alertas
- [x] Implementar loading states, empty states y error states

#### Feature 11.3 - JavaScript modular
- [x] Crear estructura `wwwroot/js/pages`
- [x] Crear estructura `wwwroot/js/modules`
- [x] Crear estructura `wwwroot/js/utils`
- [x] Aplicar convencion namespace por vista
- [x] Implementar helpers de Fetch API
- [x] Implementar manejo de errores visuales
- [x] Implementar actualizacion dinamica de graficos y tablas

### Epica 12 - Calidad, pruebas y endurecimiento

#### Feature 12.1 - Tests unitarios
- [x] Cubrir `Domain`
- [x] Cubrir `Application`
- [x] Cubrir `Validators`
- [x] Cubrir `Result<T>`
- [x] Cubrir reglas de forecasting
- [x] Cubrir reglas de abastecimiento

#### Feature 12.2 - Tests de integracion
- [x] Probar endpoints de `Api`
- [x] Probar persistencia MySQL
- [x] Probar carga Excel
- [x] Probar carga archivo `;`
- [x] Probar exportacion
- [x] Probar autorizacion
- [x] Probar auditoria

#### Feature 12.3 - Calidad final
- [x] Revisar cobertura minima
- [x] Revisar performance de consultas
- [x] Revisar seguridad base
- [x] Revisar estabilidad de flujos criticos
- [x] Revisar UX de pantallas clave

## Orden recomendado de implementacion

1. Solucion base, `Web`, `Api`, MySQL, seguridad, `Result<T>` y `UnitOfWork`
2. Dominio comercial, carga de archivos, validacion, historial y errores
3. Consultas analiticas, dashboard y reportes base
4. Exportacion, auditoria ampliada, seguridad reforzada y performance
5. Forecasting, alertas, abastecimiento y aprobacion humana
6. Testing integral, hardening y cierre
