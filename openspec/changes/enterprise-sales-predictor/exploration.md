## Exploration: Enterprise Sales Predictor

### Current State
El workspace está en fase documental: no se detectó solución .NET, proyectos `.csproj` ni código fuente en `Codigo`.
La base de la exploración vive en tres prompts fuente externos al repo, que definen el alcance funcional, la arquitectura objetivo y la experiencia visual deseada.
El archivo `openspec/config.yaml` confirma que el proyecto está en bootstrap documental, con stack previsto .NET 8 + ASP.NET MVC Razor + Tailwind CSS + Vanilla JS, pero sin runner ni pruebas inicializadas.

### Affected Areas
- `openspec/config.yaml` — fija las restricciones SDD actuales y confirma que no hay implementación aún.
- `openspec/changes/enterprise-sales-predictor/exploration.md` — artefacto de exploración para el cambio.
- `../Prompts/1-PromptMaestroRequerimientos.txt` — define el alcance funcional y de negocio.
- `../Prompts/2-PromptMaestroArquitectura.txt` — define el stack, capas y reglas arquitectónicas.
- `../Prompts/3-PromptMaestroDiseno.txt` — define UX/UI, navegación y pantallas objetivo.
- `MiApp.Domain`, `MiApp.Application`, `MiApp.Infrastructure`, `MiApp.Web` — capas previstas que quedarían impactadas por la futura implementación.

### Approaches
1. **Implementación integral de alto alcance** — intentar cubrir seguridad, carga, analítica, dashboards, exportación e IA en una sola entrega amplia.
   - Pros: una sola visión de producto; menos fragmentación inicial.
   - Cons: alto riesgo de alcance, validación tardía de datos, mayor carga de revisión y probabilidad de re-trabajo.
   - Effort: High

2. **Entrega por fases con MVP estricto** — construir primero la base de datos funcional, la carga confiable y la consulta analítica mínima; luego sumar reportes, exportación, auditoría e IA.
   - Pros: reduce riesgo; permite validar datos y UX antes de invertir en predicción; alinea seguridad y gobierno de información desde el inicio.
   - Cons: entrega valor completo más lentamente; exige disciplina para no expandir el MVP.
   - Effort: Medium

### Recommendation
Recomiendo la **entrega por fases con MVP estricto**.

**Fase 0 — Alineación y definición**
- Cerrar alcance funcional, fuentes de datos, granularidad de ventas, roles y métricas prioritarias.
- Definir contrato de datos mínimo para Excel y archivos planos `;`.
- Confirmar si habrá IA local, externa o solo analítica estadística al inicio.

**Fase 1 — Fundación operativa**
- Seguridad base: login, roles, permisos y auditoría mínima.
- Carga de archivos, validación, normalización, registro histórico y manejo de errores.
- Modelo canónico de ventas y persistencia inicial.

**Fase 2 — Analítica núcleo**
- Consultas por periodo, cliente, producto, proveedor y segmentaciones básicas.
- Dashboard gerencial con KPIs y visualizaciones principales.
- Reportes operativos y comerciales más usados.

**Fase 3 — Gobierno y distribución**
- Exportación a Excel, trazabilidad ampliada, control de permisos sobre exportaciones.
- Optimización de rendimiento, paginación, caché selectiva y endurecimiento de seguridad.

**Fase 4 — IA asistida**
- Proyecciones, alertas de baja rotación, riesgo de agotamiento y recomendaciones de abastecimiento.
- Toda recomendación debe requerir revisión humana antes de convertirse en acción.

Este orden es coherente con las dependencias: primero calidad y trazabilidad de datos, luego analítica, después distribución y finalmente predicción.

### Risks
- Ambigüedad en la estructura real de los archivos de entrada y en la calidad de datos histórica.
- Alcance excesivo si se intenta incluir IA y predicciones antes de estabilizar la base transaccional.
- Dependencia fuerte de decisiones de negocio aún no confirmadas: KPIs, roles, periodicidad, y criterios de abastecimiento.
- Riesgo de sobre-diseñar la UX sin validar qué pantallas usarán realmente los usuarios gerenciales.

### Ready for Proposal
Sí, pero con una condición: primero hay que cerrar el alcance del MVP, el diccionario mínimo de datos y la prioridad de reportes.
