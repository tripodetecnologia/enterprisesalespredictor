# Spec - Enterprise Sales Core

## Objetivo

Definir los requerimientos normativos y escenarios principales para `Enterprise Sales Predictor`.

## Requerimientos funcionales

### Requirement 1 - Seguridad y acceso
El sistema MUST autenticar usuarios y autorizar acceso segun rol y permiso.

#### Escenarios

##### Scenario: Login exitoso
- **Given** un usuario valido
- **When** inicia sesion con credenciales correctas
- **Then** el sistema permite acceso solo a modulos autorizados

##### Scenario: Acceso restringido
- **Given** un usuario autenticado sin permiso suficiente
- **When** intenta acceder a un modulo restringido
- **Then** el sistema bloquea la operacion y registra el evento si corresponde

### Requirement 2 - Gestion de usuarios, roles y permisos
El sistema MUST permitir administrar usuarios, roles y permisos.

#### Escenarios

##### Scenario: Crear usuario
- **Given** un administrador autorizado
- **When** registra un nuevo usuario valido
- **Then** el sistema crea el usuario y lo deja disponible para asociacion de roles

##### Scenario: Asignar permisos a un rol
- **Given** un rol existente
- **When** un administrador asigna permisos validos
- **Then** el sistema guarda la configuracion y la aplica a las operaciones protegidas

### Requirement 3 - Carga de historicos desde Excel y archivos `;`
El sistema MUST aceptar carga de archivos Excel y archivos delimitados por `;` y procesarlos en `Api`.

#### Escenarios

##### Scenario: Carga valida de Excel
- **Given** un archivo Excel con estructura valida
- **When** el usuario lo carga
- **Then** la `Api` valida, procesa y persiste los registros validos

##### Scenario: Carga valida de archivo delimitado
- **Given** un archivo delimitado por `;` con estructura valida
- **When** el usuario lo carga
- **Then** la `Api` valida, procesa y persiste los registros validos

##### Scenario: Carga con errores
- **Given** un archivo con encabezados invalidos o datos inconsistentes
- **When** el usuario lo carga
- **Then** el sistema rechaza o marca los errores de forma controlada y registra el resultado

### Requirement 4 - Validacion y normalizacion de datos
El sistema MUST validar campos obligatorios, detectar duplicados y normalizar registros antes de persistirlos.

#### Escenarios

##### Scenario: Registro invalido
- **Given** un registro con fecha invalida o campo obligatorio vacio
- **When** se procesa el archivo
- **Then** el sistema registra el error y evita persistir ese registro como valido

##### Scenario: Registro duplicado
- **Given** un registro identificado como duplicado segun la politica vigente
- **When** se procesa el archivo
- **Then** el sistema lo trata conforme a la politica definida y deja trazabilidad del resultado

### Requirement 5 - Historial de cargas y auditoria
El sistema MUST registrar historial de cargas, exportaciones, proyecciones y decisiones de abastecimiento.

#### Escenarios

##### Scenario: Consulta de historial de carga
- **Given** que existen cargas previas registradas
- **When** un usuario autorizado consulta el historial
- **Then** el sistema muestra fecha, usuario, estado, conteo de registros y errores

### Requirement 6 - Consultas analiticas de ventas
El sistema MUST permitir consultas de ventas por rango de fechas y por multiples dimensiones.

#### Escenarios

##### Scenario: Consulta por rango de fechas
- **Given** un rango de fechas valido
- **When** el usuario consulta ventas
- **Then** el sistema devuelve resultados filtrados y paginados

##### Scenario: Consulta por dimension
- **Given** una dimension valida como cliente, producto o proveedor
- **When** el usuario ejecuta la consulta
- **Then** el sistema devuelve resultados agregados o detallados segun el caso de uso

### Requirement 7 - Dashboard y reportes
El sistema MUST ofrecer dashboard y reportes gerenciales, comerciales, operativos, de abastecimiento y predictivos.

#### Escenarios

##### Scenario: Visualizacion de dashboard
- **Given** datos historicos disponibles
- **When** el usuario abre el dashboard principal
- **Then** el sistema muestra KPIs, comparativos y paneles principales

##### Scenario: Consulta de reportes
- **Given** filtros validos
- **When** el usuario genera un reporte
- **Then** el sistema muestra resultados acordes a la categoria solicitada

### Requirement 8 - Exportacion controlada
El sistema MUST permitir exportar informacion filtrada y reportes solo a usuarios autorizados.

#### Escenarios

##### Scenario: Exportacion autorizada
- **Given** un usuario con permiso de exportacion
- **When** solicita exportar un reporte
- **Then** el sistema genera el archivo y audita la operacion

##### Scenario: Exportacion no autorizada
- **Given** un usuario sin permiso de exportacion
- **When** intenta exportar un reporte
- **Then** el sistema bloquea la accion

### Requirement 9 - Forecasting de ventas
El sistema MUST generar proyecciones de ventas por rango de fechas en horizontes entre 1 dia y 1 año.

#### Escenarios

##### Scenario: Proyeccion valida
- **Given** datos historicos suficientes y un rango valido
- **When** el usuario solicita una proyeccion
- **Then** el sistema devuelve una proyeccion con explicacion resumida

##### Scenario: Horizonte fuera de rango
- **Given** un horizonte menor a 1 dia o mayor a 1 año
- **When** el usuario solicita la proyeccion
- **Then** el sistema rechaza la solicitud con un error controlado

### Requirement 10 - Recomendaciones de abastecimiento
El sistema MUST generar recomendaciones de abastecimiento usando historicos y stock actual.

#### Escenarios

##### Scenario: Recomendacion generada
- **Given** historicos de venta y unidades disponibles
- **When** se ejecuta el analisis
- **Then** el sistema genera una recomendacion y la deja en estado pendiente

##### Scenario: Aprobacion humana
- **Given** una recomendacion pendiente
- **When** el gerente de compras o el jefe de almacen la revisa
- **Then** puede aprobarla, rechazarla o marcarla para analisis

### Requirement 11 - Separacion entre Web y Api
La solucion MUST separar presentacion y procesamiento entre `Web` y `Api`.

#### Escenarios

##### Scenario: Procesamiento de archivo
- **Given** un usuario usa la pantalla de carga en `Web`
- **When** envia el archivo
- **Then** la `Web` delega el procesamiento a `Api` y no ejecuta logica de negocio pesada localmente

##### Scenario: Consulta dinamica
- **Given** una vista con filtros dinamicos
- **When** el usuario cambia filtros
- **Then** la `Web` consume endpoints de `Api` para actualizar datos y visualizaciones

## Requerimientos no funcionales

### Requirement 12 - Arquitectura
La implementacion SHALL seguir Clean Architecture, CQRS, Result<T> y UnitOfWork.

### Requirement 13 - Persistencia
La solucion SHALL usar MySQL como almacenamiento persistente principal.

### Requirement 14 - Lenguaje y convenciones
El codigo fuente SHALL estar en ingles y los artefactos SDD SHALL mantenerse en español.

### Requirement 15 - Seguridad
La solucion SHOULD aplicar controles OWASP, validacion estricta de archivos y auditoria de operaciones criticas.

### Requirement 16 - Rendimiento
La solucion SHOULD aplicar paginacion, filtros obligatorios e indices para consultas intensivas.

### Requirement 17 - Testing
La solucion MAY ampliarse con pruebas unitarias e integracion usando NUnit y Moq una vez exista la solucion implementada.
