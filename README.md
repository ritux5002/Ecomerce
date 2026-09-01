# MiEcommerce

API de e-commerce (Clean Architecture + CQRS/MediatR) que delega el procesamiento de pagos
a un segundo microservicio independiente, **PaymentService** ([repo aparte](../PaymentService)).

> Este trabajo entrega **dos repos separados**: este (`Ecomerce`, el e-commerce) y
> `PaymentService` (el simulador de pasarela de pagos), cada uno con su propio `.git`,
> `README.md` y ciclo de vida. Se comunican únicamente por HTTP en runtime — no comparten
> código ni base de datos.

## Arquitectura

Clean Architecture en 4 capas, igual en ambos repos:

```
Domain          → entidades, value objects, excepciones, interfaces de repositorio (sin dependencias externas)
Application     → Commands/Queries + Handlers (MediatR), validators (FluentValidation), puertos hacia Infrastructure
Infrastructure  → EF Core, repositorios, JWT, hashing, HttpClient hacia PaymentService
WebApi          → Controllers, Program.cs, Swagger, middleware
```

CQRS real con MediatR: cada Command/Query tiene su propio `IRequestHandler`, con
`ValidationBehavior` y `LoggingBehavior` como pipeline behaviors.

## Puertos

| Servicio | Perfil | URL |
|---|---|---|
| **MiEcommerce.WebApi** | http | `http://localhost:8080` |
| **MiEcommerce.WebApi** | https | `https://localhost:7156` |
| **PaymentService.WebApi** | http | `http://localhost:5080` |
| **PaymentService.WebApi** | https | `https://localhost:7080` |

La URL base de PaymentService se configura en
[`src/MiEcommerce.WebApi/appsettings.json`](src/MiEcommerce.WebApi/appsettings.json):

```json
"PaymentService": { "BaseUrl": "http://localhost:5080/" }
```

## Usuario Admin de prueba

Sembrado por migración de EF Core ([`UserConfiguration.cs`](src/MiEcommerce.Infrastructure/Persistence/Configurations/UserConfiguration.cs)),
disponible apenas se aplican las migraciones:

```
email:    admin@ecommerce.com
password: admin123
```

Los endpoints de escritura de `Orders` (confirm/ship/deliver/listar todas), `Products` y
`Categories` requieren rol `Admin` (`[Authorize(Roles = "Admin")]`). `POST /api/auth/register`
solo crea usuarios con rol `Customer`; hoy no hay un endpoint para crear Admins adicionales
en runtime, el Admin de prueba sembrado es el único.

## Qué opción elegimos y regla de negocio del segundo servicio

**PaymentService** simula una pasarela de pagos (sin integrar Stripe/MercadoPago ni ninguna
pasarela real). La regla de aprobación es local y determinística, vive en
[`PaymentService.Domain.Entities.Payment`](../PaymentService/src/PaymentService.Domain/Entities/Payment.cs):

> Se **aprueba** el pago si el monto es **estrictamente menor a $100.000**.
> Se **rechaza** si es **igual o mayor a $100.000**.

Detalle completo (endpoint, DTOs, cómo se integra, ejemplos) en el
[README de PaymentService](../PaymentService/README.md).

## Cómo levantar ambos servicios

Requiere .NET 8 SDK y SQL Server LocalDB (o ajustar la cadena de conexión).

```bash
# Terminal 1 — PaymentService
cd ../PaymentService
dotnet run --project src/PaymentService.WebApi

# Terminal 2 — MiEcommerce (aplica migraciones si es la primera vez)
cd Ecomerce
dotnet ef database update --project src/MiEcommerce.Infrastructure --startup-project src/MiEcommerce.WebApi
dotnet run --project src/MiEcommerce.WebApi
```

## Flujo end-to-end (reproducible por Swagger)

Con ambos servicios arriba, abrir `http://localhost:8080/swagger`:

1. `POST /api/auth/login` con `admin@ecommerce.com` / `admin123` → tomar el token y pulsar **Authorize**.
2. `POST /api/customers/register` → crear un cliente.
3. `POST /api/products` → crear un producto con `price < 100000` (camino *Approved*) y otro con `price >= 100000` (camino *Rejected*).
4. `POST /api/orders` → crear una orden con ese `customerId`.
5. `POST /api/orders/{orderId}/items` → agregar el producto.
6. `POST /api/orders/{orderId}/confirm` → reserva stock, llama a PaymentService y devuelve `status: "Paid"` o `"PaymentRejected"` + `transactionId`.

Si `PaymentService` está apagado en el paso 6, `MiEcommerce` devuelve `503 Service Unavailable`
y no persiste ningún cambio (ni el estado `Confirmed` ni la reserva de stock): la orden queda
intacta en `Draft`, lista para reintentar. Ver
[`PaymentServiceClient`](src/MiEcommerce.Infrastructure/Services/PaymentServiceClient.cs) y
[`GlobalExceptionHandler`](src/MiEcommerce.WebApi/Middleware/GlobalExceptionHandler.cs).

El log de consola de PaymentService muestra cada `ProcessPaymentCommand` recibido, confirmando
la comunicación entre los dos procesos independientes.
