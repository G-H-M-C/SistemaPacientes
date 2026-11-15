# Sistema de Pacientes

CRUD de un sistema de pacientes hecho con una API en .NET 10 organizada en capas y un frontend en React + TypeScript que la consume.

## TecnologÃ­as

- **Backend:** ASP.NET Core Web API (.NET 10), Entity Framework Core 10 (Code First), SQL Server LocalDB, Swagger
- **Frontend:** React + TypeScript, Vite, React Router
- **Base de datos:** `PacientesDb` (10 tablas)

## Arquitectura en capas

```
backend/
  SistemaPacientes.Domain          Entidades e interfaces de repositorios
  SistemaPacientes.Application     Servicios, DTOs, interfaces de servicios y ServiceResult
  SistemaPacientes.Infrastructure  DbContext, migraciones e implementaciÃ³n de repositorios
  SistemaPacientes.API             Controllers, configuraciÃ³n (Program.cs) y Swagger
frontend/                          AplicaciÃ³n React desacoplada de la API
```

Flujo de una peticiÃ³n: `Controller â†’ Service â†’ Repository â†’ DbContext â†’ SQL Server`

| Capa | QuÃ© contiene | Depende de |
|---|---|---|
| Domain | `BaseEntity`, las 10 entidades, `IRepository<T>` y repositorios especÃ­ficos (`IPacienteRepository`, `IMedicoRepository`, `ICitaRepository`) | nada |
| Application | `ICrudService` y una interfaz por entidad, servicios con la lÃ³gica y las validaciones, DTOs (Create, Update y Response), `ServiceResult<T>` | Domain |
| Infrastructure | `ApplicationDbContext` (Fluent API y datos iniciales), `Repository<T>` genÃ©rico y repositorios especÃ­ficos | Domain |
| API | Un controller por entidad que traduce el resultado del servicio a cÃ³digo HTTP | Application, Infrastructure |

### Interfaces e inyecciÃ³n de dependencias

Los controllers dependen de interfaces de servicio (`IPacienteService`, etc.) y los servicios dependen de interfaces de repositorio (`IRepository<T>`). Las implementaciones se registran en `Program.cs`, por lo que cada capa conoce solo el contrato de la siguiente y no su implementaciÃ³n.

### ServiceResult&lt;T&gt;

Los servicios no conocen HTTP. Devuelven un `ServiceResult<T>` con un tipo (`Ok`, `Created`, `NoContent`, `Invalid`, `NotFound`), los datos y un mensaje de error. El controller lo traduce a 200, 201, 204, 400 o 404.

## Modelo de datos

10 tablas: `Especialidades`, `Medicos`, `Pacientes`, `Citas`, `Consultas`, `Diagnosticos`, `Tratamientos`, `Medicamentos`, `Recetas` y `DetallesReceta`.

- Un mÃ©dico pertenece a una especialidad.
- Una cita relaciona un paciente con un mÃ©dico.
- Una consulta pertenece a una cita; de ella salen diagnÃ³sticos y recetas.
- Un tratamiento pertenece a un diagnÃ³stico.
- Una receta tiene varios detalles, cada uno con un medicamento.

Las eliminaciones usan `Restrict` (salvo los detalles de una receta, que se borran con la receta). Hay Ã­ndices Ãºnicos en cÃ©dula, licencia, nombre de especialidad y nombre de medicamento.

## Reglas de negocio

- La cÃ©dula del paciente y la licencia del mÃ©dico no se pueden repetir.
- La fecha de nacimiento no puede ser futura.
- Una cita nueva debe tener fecha futura y un mÃ©dico no puede tener dos citas (no canceladas) a la misma hora.
- Una cita solo puede tener una consulta.
- No se puede eliminar un registro que tenga datos dependientes (por ejemplo, un paciente con citas).

## CÃ³mo ejecutarlo

Requisitos: .NET 10 SDK, SQL Server LocalDB y Node.js.

### 1. Backend

```
cd backend
dotnet ef database update --project .\SistemaPacientes.Infrastructure --startup-project .\SistemaPacientes.API
dotnet run --project .\SistemaPacientes.API
```

La cadena de conexiÃ³n estÃ¡ en `backend/SistemaPacientes.API/appsettings.json`. La API queda en `http://localhost:5032` y Swagger en `http://localhost:5032/swagger`.

### 2. Frontend

```
cd frontend
npm install
npm run dev
```

La aplicaciÃ³n queda en `http://localhost:5173`. La direcciÃ³n de la API se configura en `frontend/.env` con la variable `VITE_API_URL`. La API tiene CORS habilitado para ese origen.

## Endpoints

Cada recurso (`Pacientes`, `Medicos`, `Citas`, `Especialidades`, `Consultas`, `Diagnosticos`, `Tratamientos`, `Medicamentos`, `Recetas`, `DetallesReceta`) tiene:

| MÃ©todo | Ruta | DescripciÃ³n |
|---|---|---|
| GET | `/api/{recurso}` | Lista todos |
| GET | `/api/{recurso}/{id}` | Obtiene uno |
| POST | `/api/{recurso}` | Crea |
| PUT | `/api/{recurso}/{id}` | Actualiza |
| DELETE | `/api/{recurso}/{id}` | Elimina |

## Frontend

MÃ³dulos: **Panel** (totales y prÃ³ximas citas), **Pacientes**, **MÃ©dicos** y **Citas**, cada uno con listado, creaciÃ³n, ediciÃ³n y eliminaciÃ³n. Incluye bÃºsqueda, estados de carga, error y lista vacÃ­a, y confirmaciÃ³n antes de eliminar.

## Capturas

![Swagger](docs/swagger.png)
![Panel](docs/Panel.png)
