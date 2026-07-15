# Proposal - Enterprise Sales Predictor

## Resumen

`Enterprise Sales Predictor` propone construir una plataforma web empresarial para cargar historicos de ventas, validarlos, persistirlos en MySQL, analizarlos por multiples dimensiones, emitir reportes ejecutivos y generar proyecciones y recomendaciones de abastecimiento con aprobacion humana.

La solucion se implementara con una topologia separada entre:

- `EnterpriseSalesPredictor.Web` para vistas Razor y experiencia de usuario
- `EnterpriseSalesPredictor.Api` para procesamiento, endpoints internos y logica expuesta

## Problema

El negocio necesita consolidar historicos de ventas provenientes de archivos Excel y archivos delimitados por `;`, asegurar su calidad, reducir procesos manuales de analitica y soportar decisiones comerciales, gerenciales y de abastecimiento con mayor trazabilidad.

Hoy el mayor riesgo no es solo la falta de reportes; tambien existe dependencia de archivos externos, variabilidad en la calidad de datos, baja trazabilidad operacional y ausencia de soporte sistematico para proyecciones y recomendaciones.

## Objetivos

- Centralizar carga y persistencia de historicos de ventas en MySQL.
- Separar claramente presentacion y procesamiento mediante proyectos `Web` y `Api`.
- Habilitar seguridad por usuarios, roles y permisos.
- Entregar consultas analiticas, dashboard y reportes gerenciales, comerciales, operativos, de abastecimiento y predictivos.
- Incorporar forecasting por rango de fechas, desde 1 dia hasta 1 año.
- Incorporar recomendaciones de abastecimiento usando historicos y stock actual.
- Exigir revision humana para cualquier recomendacion de abastecimiento.

## Usuarios objetivo

- Gerentes
- Vendedores
- Asistentes
- Jefe de almacen
- Cartera
- Gerente de compras
- Administrador del sistema

## Alcance MVP

El MVP prioriza tres capacidades:

1. Seguridad, usuarios, roles y permisos.
2. Cargue de historicos de ventas desde Excel y archivos delimitados por `;`.
3. Reportes base y proyecciones de ventas.

## Alcance total

El alcance total del cambio incluye:

- autenticacion y autorizacion
- gestion de usuarios, roles y permisos
- carga y validacion de archivos
- historial de cargas y auditoria
- consultas analiticas por multiples dimensiones
- dashboard principal
- reportes gerenciales, comerciales, operativos, de abastecimiento y predictivos
- exportacion a Excel
- forecasting por rango de fechas
- alertas de comportamiento atipico
- recomendaciones de abastecimiento con aprobacion humana

## No objetivos del primer corte

- integraciones complejas con ERP o CRM
- compras automaticas sin revision humana
- modelos avanzados no explicables antes de estabilizar los datos
- optimizaciones prematuras de escala antes de validar uso real

## Datos base confirmados

Los campos minimos confirmados para el dominio incluyen:

- numero de factura
- nombre e identificacion de cliente
- direccion, ciudad, telefono y zona del cliente
- tipo de producto, producto, referencia y marca
- precio de compra y precio de venta
- unidades disponibles
- cantidad vendida y valor de venta
- fecha de venta
- nombre e identificacion de vendedor
- nombre e identificacion de proveedor
- direccion, telefono y ciudad del proveedor
- medio de pago de factura

## Restricciones tecnicas

- Runtime: .NET 8
- Presentacion: ASP.NET MVC con Razor
- API de procesamiento: ASP.NET Core Web API
- Persistencia: MySQL
- Estilos: Tailwind CSS
- JavaScript: Vanilla JS con module pattern
- Testing: NUnit con Moq
- Arquitectura: Clean Architecture
- Patrones: CQRS, Result<T>, UnitOfWork

## Decisiones funcionales cerradas

- Las proyecciones deben operar por rango de fechas.
- El horizonte de proyeccion va de 1 dia a 1 año.
- Las recomendaciones de abastecimiento deben considerar stock actual.
- Las recomendaciones no ejecutan compras automaticamente.
- La aprobacion humana corresponde a gerente de compras y jefe de almacen.

## Entrega por fases

### Fase 0 - Definicion y preparacion
- cerrar diccionario de datos
- cerrar reglas de carga y duplicados
- cerrar matriz de permisos

### Fase 1 - Fundacion operativa
- seguridad
- persistencia MySQL
- carga de archivos
- historial y errores

### Fase 2 - Analitica base
- consultas por rango y dimension
- dashboard
- reportes gerenciales, comerciales y operativos

### Fase 3 - Gobierno y distribucion
- exportacion
- auditoria ampliada
- endurecimiento de seguridad
- optimizacion y paginacion

### Fase 4 - Forecasting y abastecimiento asistido
- proyecciones
- alertas
- recomendaciones de abastecimiento
- aprobacion humana

## Riesgos principales

- calidad irregular de archivos fuente
- ambiguedad en reglas de limpieza y deduplicacion
- sobredimensionar IA antes de estabilizar la base historica
- consultas lentas si no se diseñan indices y filtros desde el inicio
- mezclar responsabilidades entre `Web` y `Api`

## Reversion o descarte

Si una decision compromete seguridad, integridad de datos o mantenibilidad, el cambio debe volver al ultimo artefacto aceptado y reevaluarse antes de iniciar implementacion.

## Resultado esperado

Al finalizar el cambio, la organizacion debera contar con una plataforma trazable, segura y orientada a datos para ventas, analitica y abastecimiento asistido.

## Siguiente paso recomendado

Completar especificacion formal con requerimientos normativos y escenarios Given/When/Then, seguida del diseño tecnico detallado y backlog verificable.
