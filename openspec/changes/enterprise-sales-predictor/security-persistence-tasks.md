# Security Persistence Tasks

## Objetivo

Migrar autenticacion, usuarios, roles y permisos desde configuracion/en memoria hacia persistencia real en base de datos.

## Tareas

- [x] Crear entidades `User`, `Role`, `Permission`, `UserRole`, `RolePermission`
- [x] Agregar `DbSet<>` de seguridad en `AppDbContext`
- [x] Crear configuraciones EF Core de seguridad
- [x] Agregar indices y restricciones unicas para usuarios, roles y permisos
- [x] Implementar hashing de password persistido
- [x] Implementar `DbCredentialValidator`
- [x] Implementar `DbAccessManagementService`
- [x] Reemplazar registros DI en memoria por versiones persistidas
- [x] Implementar seed inicial de permisos, rol administrador y usuario admin
- [x] Crear migracion EF para tablas de seguridad
- [x] Agregar tests unitarios para autenticacion persistida
- [x] Agregar tests de integracion para login y access management persistidos
- [x] Verificar build y tests tras la migracion
