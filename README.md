# 🛍️ ECommerce API - Backend

[![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-8.0-purple?logo=.net&logoColor=white)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12-239120?logo=csharp&logoColor=white)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-Latest-336791?logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![Entity Framework Core](https://img.shields.io/badge/EF_Core-9.0-512BD4?logo=entityframework&logoColor=white)](https://docs.microsoft.com/en-us/ef/core/)

Una **API RESTful profesional** construida con **ASP.NET Core 8** que implementa patrones de arquitectura moderna, principios SOLID y buenas prácticas de desarrollo. Este backend potencia una plataforma de ecommerce colaborando perfectamente con un frontend desarrollado en **Next.js + React 19**.

---

## 📋 Tabla de Contenidos

- [Descripción General](#descripción-general)
- [Stack Tecnológico](#-stack-tecnológico)
- [Características Principales](#-características-principales)
- [Estructura del Proyecto](#-estructura-del-proyecto)
- [Instalación y Configuración](#-instalación-y-configuración)
- [Archivos de Entorno](#-archivos-de-entorno)
- [Arquitectura](#-arquitectura)
- [Patrones Implementados](#-patrones-implementados)
- [Endpoints API](#-endpoints-api)
- [Autenticación JWT](#-autenticación-jwt)
- [Integración con MercadoPago](#-integración-with-mercadopago)
- [Sistema de Reservas](#-sistema-de-reservas)
- [Rate Limiting](#-rate-limiting)
- [Base de Datos](#-base-de-datos)
- [Seguridad](#-seguridad)
- [Integración Frontend](#-integración-con-frontend)
- [Puntos Destacados](#-puntos-destacados)
- [Guía de Desarrollo](#-guía-de-desarrollo)

---

## 📝 Descripción General

**ECommerce API** es un backend robusto y escalable que gestiona todas las operaciones de una plataforma de comercio electrónico moderna. Desde la autenticación de usuarios hasta la procesión de pagos, pasando por un sofisticado sistema de reservas de inventario, esta API demuestra arquitectura profesional y patrones de diseño implementados correctamente.

El proyecto fue desarrollado siguiendo principios de **Clean Architecture**, **SOLID principles** y **Domain-Driven Design**, lo que garantiza código mantenible, testeable y escalable.

---

## 🚀 Stack Tecnológico

### Backend
- **Framework**: ASP.NET Core 8.0 (LTS)
- **Lenguaje**: C# 12
- **ORM**: Entity Framework Core 9.0
- **Base de Datos**: PostgreSQL
- **Autenticación**: JWT (JSON Web Tokens)
- **Integración de Pagos**: MercadoPago SDK
- **Almacenamiento de Imágenes**: Cloudinary
- **Documentación API**: Swagger/OpenAPI
- **Validación**: Data Annotations + Custom Validators
- **Rate Limiting**: ASP.NET Core Rate Limiting
- **CSRF Protection**: CSRF Token Validation

### Frontend (Integración)
- **Framework**: Next.js 15+
- **Librería UI**: React 19
- **Lenguaje**: TypeScript
- **Estilos**: Tailwind CSS

### DevOps
- **Containerización**: Docker
- **Hosting**: Render / Vercel
- **Control de Versiones**: Git

---

## ✨ Características Principales

### 🔐 Autenticación y Autorización
- Registro e inicio de sesión seguros con **JWT**
- Roles de usuario: **User** y **Admin**
- Almacenamiento seguro de contraseñas con **BCrypt**
- Cookies HttpOnly con tokens JWT
- Protección CSRF (Cross-Site Request Forgery)
- Rate limiting específico para endpoints de autenticación

### 📦 Gestión de Productos
- CRUD completo de productos
- Categorización de productos
- Búsqueda y filtrado avanzado (por precio, categoría, nombre)
- Paginación eficiente
- Cálculo de inventario con stock y reservas
- Métricas de ventas (total vendido)
- Carga de imágenes a través de Cloudinary

### 🛒 Carrito de Compras
- Gestión de carritos por usuario
- Agregar, actualizar y remover items
- Protección contra eliminación de productos en uso
- Validación de stock antes de compra

### 📋 Gestión de Órdenes
- Creación y seguimiento de órdenes
- Sistema de estados: `AwaitingPayment` → `Paid` → `Preparing` → `Shipped` → `Delivered`
- También: `Cancelled` y `Expired`
- Paginación de órdenes del usuario
- Panel admin para visualizar todas las órdenes
- Histórico completo de compras

### 💳 Integración con MercadoPago
- Creación de preferencias de pago
- Procesamiento de webhooks
- Validación de pagos
- Estados sincronizados con MercadoPago
- Idempotencia garantizada (evita pagos duplicados)

### ⏱️ Sistema de Reservas Inteligente
- Reserva automática de stock al crear orden
- Liberación de reservas tras expiración (15 minutos)
- Limpieza automática de reservas vencidas
- Servicio en background (`ExpiredReservationsCleanupService`)
- Previene sobreventa

### 👤 Gestión de Usuarios
- Perfiles de usuario
- Actualización de datos (dirección, teléfono)
- Soft delete (deshabilitar usuarios sin borrar)
- Validación de email único
- Búsqueda y filtrado para administradores

### 📊 Panel Administrativo
- Endpoints específicos para administradores
- Gestión de usuarios (listar, habilitar/deshabilitar)
- Gestión de órdenes (ver todas, cambiar estado)
- Gestión de productos (crear, editar, eliminar)
- Gestión de categorías

### 🔒 Seguridad
- Validación CSRF Token
- Cookies seguras (HttpOnly, Secure, SameSite)
- Rate Limiting global y por endpoint
- Encriptación de contraseñas
- Índices únicos en emails y payment IDs
- Comportamiento de eliminación restringido en relaciones

---

## 📁 Estructura del Proyecto

```
ECommerceAPI/
├── 📄 Program.cs                          # Configuración de la aplicación
├── 📄 ECommerceAPI.csproj                # Definición del proyecto
├── 📄 ECommerceAPI.http                  # Endpoints para testing
├── 📄 Dockerfile                          # Configuración de contenedor
├── 📄 appsettings.json                   # Configuración base
├── 📄 appsettings.Development.json       # Configuración local
│
├── 📂 Controllers/                        # ⚡ Endpoints de la API
│   ├── AuthController.cs                 # Registro, login, CSRF
│   ├── ProductController.cs              # CRUD de productos
│   ├── CartController.cs                 # Gestión de carrito
│   ├── CategoryController.cs             # Gestión de categorías
│   ├── OrderController.cs                # Órdenes del usuario
│   ├── PaymentController.cs              # Integración MercadoPago
│   ├── UserController.cs                 # Perfil de usuario
│   ├── AdminOrdersController.cs          # Órdenes (admin)
│   └── AdminUsersController.cs           # Usuarios (admin)
│
├── 📂 Services/                           # 🔧 Lógica de negocio
│   ├── Auth/
│   │   ├── IAuthService.cs
│   │   └── AuthService.cs                # Registro, login, validación
│   ├── Users/
│   │   └── UserService.cs                # Gestión de usuarios
│   ├── Products/
│   │   └── ProductService.cs             # Búsqueda, filtrado, CRUD
│   ├── Categories/
│   │   └── CategoryService.cs            # Gestión de categorías
│   ├── Cart/
│   │   └── CartService.cs                # Carrito de compras
│   ├── Orders/
│   │   ├── IOrderService.cs
│   │   ├── OrderService.cs               # Órdenes del usuario
│   │   └── AdminOrderService.cs          # Órdenes (admin)
│   ├── Payment/
│   │   ├── PaymentService.cs             # MercadoPago integration
│   │   └── ExpiredReservationsCleanupService.cs # Limpieza automática
│   ├── Jwt/
│   │   ├── IJwtService.cs
│   │   └── JwtService.cs                 # Generación de tokens
│   └── Cloudinary/
│       ├── ICloudinaryService.cs
│       └── CloudinaryService.cs          # Carga de imágenes
│
├── 📂 Models/                             # 📊 Modelos de dominio
│   ├── User.cs                           # Usuario (con soft delete)
│   ├── Product.cs                        # Producto (stock + reservas)
│   ├── Category.cs                       # Categoría
│   ├── CartItem.cs                       # Item del carrito
│   ├── Order.cs                          # Orden (con MercadoPago)
│   └── OrderItem.cs                      # Item de orden
│
├── 📂 DTOs/                               # 📮 Transfer Objects
│   ├── Auth/
│   │   ├── UserRegisterDto.cs
│   │   ├── UserLoginDto.cs
│   │   └── AuthResponseDto.cs
│   ├── Product/
│   │   ├── ProductCreateDto.cs
│   │   ├── ProductUpdateDto.cs
│   │   └── ProductResponseDto.cs
│   ├── Cart/
│   │   ├── CartAddDto.cs
│   │   └── CartResponseDto.cs
│   ├── Order/
│   │   └── [Order DTOs]
│   ├── Category/
│   │   └── [Category DTOs]
│   ├── User/
│   │   └── [User DTOs]
│   └── Payment/
│       └── [Payment DTOs]
│
├── 📂 Data/                               # 🗄️ Acceso a datos
│   ├── AppDbContext.cs                   # DbContext (EF Core)
│   └── DbDefaultProducts.cs              # Seed de datos
│
├── 📂 Migrations/                         # 📈 Historial de BD
│   └── [Historia de cambios de esquema]
│
├── 📂 Settings/                           # ⚙️ Configuración
│   ├── JwtSettings.cs                    # Config de JWT
│   ├── CloudinarySettings.cs             # Config de Cloudinary
│   └── MercadoPagoSettings.cs            # Config de MercadoPago
│
├── 📂 Extensions/                         # 🔌 Extensiones
│   └── ClaimsPrincipalExtensions.cs      # Helper para User.GetUserId()
│
├── 📂 Helpers/                            # 🛠️ Utilidades
│   └── OrderStatusHelper.cs              # Estados de órdenes
│
├── 📂 Constants/                          # 📌 Constantes
│   └── OrderStatuses.cs                  # Estados globales
│
├── 📂 Properties/
│   └── launchSettings.json               # Configuración de launch
│
├── 📂 bin/                                # Compilados
└── 📂 obj/                                # Objetos de compilación
```

---

## 🔧 Instalación y Configuración

### Requisitos Previos
- **.NET 8 SDK** (o superior)
- **PostgreSQL 12+**
- **Git**
- **Docker** (opcional, para containerización)
- Credenciales de **Cloudinary** (para imágenes)
- Token de **MercadoPago** (para pagos)

### Pasos de Instalación

#### 1. Clonar el repositorio
```bash
git clone https://github.com/tu-usuario/ecommerce-backend.git
cd ecommerce-backend/ECommerceAPI
```

#### 2. Restaurar dependencias
```bash
dotnet restore
```

#### 3. Configurar archivo de entorno
Crear archivo `appsettings.Development.json`:

```json
{
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "Microsoft.AspNetCore": "Warning"
        }
    },
    "ConnectionStrings": {
        "DefaultConnection": "Host=localhost;Database=ecommerce_db;Username=postgres;Password=tu_contraseña"
    },
    "Jwt": {
        "Key": "TuClaveSecretaDeJWTConMasDe32Caracteres",
        "Issuer": "ECommerceAPI",
        "Audience": "ECommerceAPIUsers",
        "ExpiresInMinutes": 10080
    },
    "Cloudinary": {
        "CloudName": "tu_cloud_name",
        "ApiKey": "tu_api_key",
        "ApiSecret": "tu_api_secret"
    },
    "MercadoPago": {
        "AccessToken": "tu_token_mercadopago",
        "WebhookBaseUrl": "https://tu-backend.com",
        "FrontendBaseUrl": "http://localhost:3000"
    }
}
```

#### 4. Crear y migrar la base de datos
```bash
# Crear la BD
dotnet ef database create

# Aplicar migraciones
dotnet ef database update
```

#### 5. Ejecutar la aplicación
```bash
# Modo desarrollo
dotnet run

# Con watch (reinicia al detectar cambios)
dotnet watch run
```

La API estará disponible en: `https://localhost:5001`
Swagger UI: `https://localhost:5001/swagger`

---

## 🌍 Archivos de Entorno

### Variables Requeridas

```
# Base de Datos
ConnectionStrings__DefaultConnection=Host=localhost;Database=ecommerce_db;Username=postgres;Password=xxxxx

# JWT
Jwt__Key=ClaveSecretaDeJWTConMasDe32Caracteres
Jwt__Issuer=ECommerceAPI
Jwt__Audience=ECommerceAPIUsers
Jwt__ExpiresInMinutes=10080

# Cloudinary
Cloudinary__CloudName=tu_cloud_name
Cloudinary__ApiKey=tu_api_key
Cloudinary__ApiSecret=tu_api_secret

# MercadoPago
MercadoPago__AccessToken=APP_USR-xxxxxxxxxxxxxxxxxxxxxxxxxxxx
MercadoPago__WebhookBaseUrl=https://tu-backend.com
MercadoPago__FrontendBaseUrl=https://tu-frontend.com

# Entorno
APP_ENV=development|production
```

### Seguridad de Credenciales
- ⚠️ **NUNCA** committer credenciales reales al repositorio
- Usar `appsettings.Development.json` y agregarlo a `.gitignore`
- En producción, usar variables de entorno del servidor
- Rotar tokens regularmente

---

## 🏗️ Arquitectura

### Patrones de Arquitectura Implementados

#### **1. Clean Architecture / Layered Architecture**
La aplicación está organizada en capas claramente separadas:

```
┌─────────────────────────────────┐
│    Presentation (Controllers)    │  ← Endpoints HTTP
├─────────────────────────────────┤
│    Business Logic (Services)     │  ← Lógica de negocio
├─────────────────────────────────┤
│    Data Access (DbContext)       │  ← Persistencia
├─────────────────────────────────┤
│    Database (PostgreSQL)         │  ← Datos
└─────────────────────────────────┘
```

#### **2. Repository Pattern** (Implícito mediante EF Core)
Entity Framework Core actúa como repositorio, abstrayendo el acceso a datos.

#### **3. Dependency Injection (DI)**
Todas las dependencias se registran en `Program.cs`:
```csharp
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductService, ProductService>();
// ...
```

#### **4. DTO Pattern (Data Transfer Objects)**
- Separación entre modelos de dominio y API
- Validación de entrada
- Seguridad (no exponiendo todos los campos)

#### **5. Service Layer**
- Encapsula toda la lógica de negocio
- Reutilizable desde múltiples controladores
- Fácil de testear

---

## 🎯 Patrones Implementados

### SOLID Principles ✅

#### **S - Single Responsibility Principle**
Cada clase tiene UNA sola razón para cambiar:
- `AuthService` solo maneja autenticación
- `ProductService` solo maneja productos
- `PaymentService` solo maneja pagos

#### **O - Open/Closed Principle**
Las clases están abiertas para extensión, cerradas para modificación:
```csharp
public interface IAuthService { }
public interface IProductService { }
// Nuevas implementaciones sin tocar controladores
```

#### **L - Liskov Substitution Principle**
Las interfaces garantizan que cualquier implementación puede reemplazar a otra:
```csharp
IAuthService authService = new AuthService(db, jwtService);
// Podría ser cualquier otra implementación
```

#### **I - Interface Segregation Principle**
Interfaces específicas y pequeñas:
```csharp
public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(UserRegisterDto dto);
    Task<AuthResponseDto> LoginAsync(UserLoginDto dto);
}
```

#### **D - Dependency Inversion Principle**
Dependen de abstracciones, no de implementaciones concretas:
```csharp
public class OrderService
{
    private readonly IProductService _productService; // Inyección
    // No: private readonly ProductService _productService;
}
```

### Otros Patrones 🎨

#### **Middleware Pattern**
Rate limiting, CORS, autenticación como middleware:
```csharp
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
```

#### **Strategy Pattern** (Implícito)
Diferentes estrategias de pago, validación, etc.

#### **Hosting Service Pattern**
Servicio en background para limpiar reservas:
```csharp
builder.Services.AddHostedService<ExpiredReservationsCleanupService>();
```

#### **Guard Clauses**
Validacion early return en servicios

#### **Async/Await Pattern**
Operaciones asincrónicas en toda la API

---

## 🔌 Endpoints API

### Autenticación 🔐

| Método | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| POST | `/api/auth/register` | Registrar nuevo usuario | ❌ No |
| POST | `/api/auth/login` | Iniciar sesión | ❌ No |
| GET | `/api/auth/csrf` | Obtener token CSRF | ❌ No |

### Productos 📦

| Método | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| GET | `/api/product` | Listar productos con paginación | ❌ No |
| GET | `/api/product/{id}` | Obtener detalles de producto | ❌ No |
| POST | `/api/product` | Crear producto | ✅ Admin |
| PUT | `/api/product/{id}` | Actualizar producto | ✅ Admin |
| DELETE | `/api/product/{id}` | Eliminar producto | ✅ Admin |
| POST | `/api/product/upload-image` | Subir imagen a Cloudinary | ✅ Admin |

**Query Parameters para GET /api/product:**
```
?page=1                    # Página (default: 1)
&pageSize=25              # Items por página (default: 25)
&sort=price_asc|price_desc|newest  # Ordenamiento
&categoryId=5             # Filtrar por categoría
&minPrice=10              # Precio mínimo
&maxPrice=1000            # Precio máximo
&search=iphone            # Búsqueda de texto
```

### Categorías 🏷️

| Método | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| GET | `/api/category` | Listar categorías | ❌ No |
| POST | `/api/category` | Crear categoría | ✅ Admin |
| PUT | `/api/category/{id}` | Actualizar categoría | ✅ Admin |
| DELETE | `/api/category/{id}` | Eliminar categoría | ✅ Admin |

### Carrito 🛒

| Método | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| GET | `/api/cart` | Ver carrito | ✅ Sí |
| POST | `/api/cart` | Agregar al carrito | ✅ Sí |
| PUT | `/api/cart/{cartItemId}` | Actualizar cantidad | ✅ Sí |
| DELETE | `/api/cart/{cartItemId}` | Remover del carrito | ✅ Sí |

### Órdenes 📋

| Método | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| GET | `/api/order` | Mis órdenes (paginadas) | ✅ Sí |
| GET | `/api/order/{orderId}` | Detalles de oder | ✅ Sí |
| POST | `/api/order` | Crear orden | ✅ Sí |

### Pagos 💳

| Método | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| POST | `/api/payments/create-preference` | Crear preferencia MercadoPago | ✅ Sí |
| POST | `/api/payments/webhook` | Webhook de MercadoPago | ❌ No |

### Usuario 👤

| Método | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| GET | `/api/user` | Ver perfil | ✅ Sí |
| PUT | `/api/user` | Actualizar perfil | ✅ Sí |

### Administración (Admin) ⚙️

| Método | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| GET | `/api/adminorders` | Todas las órdenes (paginadas) | ✅ Admin |
| PUT | `/api/adminorders/{orderId}/status` | Cambiar estado de orden | ✅ Admin |
| GET | `/api/adminusers` | Listar usuarios | ✅ Admin |
| PUT | `/api/adminusers/{userId}/disable` | Deshabilitar usuario | ✅ Admin |
| PUT | `/api/adminusers/{userId}/enable` | Habilitar usuario | ✅ Admin |

---

## 🔐 Autenticación JWT

### Flujo de Autenticación

```
1. Usuario se registra/login
   └─> Servicio genera JWT Token
   └─> Token se devuelve en respuesta
   └─> Frontend guarda en cookie HttpOnly

2. Cliente realiza request autenticado
   └─> Envía token en Authorization header o cookie
   └─> Middleware valida JWT
   └─> Claims se extraen (UserId, Rol, etc)

3. Controller accede a usuario autenticado
   └─> User.GetUserId()
   └─> User.IsInRole("Admin")
```

### Estructura del JWT

```
Header:
{
  "alg": "HS256",
  "typ": "JWT"
}

Payload:
{
  "sub": "1",
  "name": "Juan Pérez",
  "email": "juan@example.com",
  "role": "Admin",
  "iat": 1234567890,
  "exp": 1234651290
}

Signature:
HMACSHA256(header + payload + secret)
```

### Validación JWT

- ✅ Firma válida
- ✅ Token no expirado
- ✅ Issuer válido
- ✅ Audience válido
- ✅ Claims correctos

---

## 💳 Integración con MercadoPago

### Flujo de Pago

```
1. Usuario crea orden desde carrito
   └─> Stock se reserva (15 min)

2. Frontend solicita crear preferencia
   └─> API integrada con MercadoPago
   └─> Se devuelve ID de preferencia

3. Usuario paga en MercadoPago
   └─> Realiza transacción segura

4. MercadoPago envía webhook
   └─> API valida y confirma pago
   └─> Estado de orden cambia a "Paid"
   └─> Stock se actualiza

5. Admin cambia estado manualmente
   └─> Preparing → Shipped → Delivered
```

### Detalles de Integración

- **Idempotencia**: Índice único en `MercadoPagoPaymentId` evita procesar el mismo pago dos veces
- **Webhooks**: Endpoint seguro `/api/payments/webhook` recibe notificaciones
- **Estados Sincronizados**: La orden siempre refleja el estado de MercadoPago
- **Validación**: Cada webhook se valida antes de procesarse

---

## ⏱️ Sistema de Reservas

### ¿Por qué Reservas?

Evita overselling (vender más del stock disponible) en operaciones concurrentes:

```
❌ Sin reservas (problema):
1. Usuario A compra 5 unidades (stock: 10 → 5)
2. Usuario B compra 8 unidades (stock: 5 → -3) ← ¡ERROR!

✅ Con reservas (solución):
1. Usuario A crea orden (reserva: 5, stock disponible: 5)
2. Usuario B crea orden (reserva: 8, ¡fallaría si stock < 8!)
```

### Flujo de Reservas

```
Orden creada:
  Product.ReservedStock += cantidad
  Product.Stock -= cantidad (lógicamente)

Pago confirmado:
  Reserva se mantiene (se convierte en venta)
  Product.TotalSold += cantidad

Reserva expira (15 min):
  ExpiredReservationsCleanupService libera
  Product.ReservedStock -= cantidad
  Product.Stock += cantidad (vuelve disponible)

Usuario cancela:
  Libera reserva manualmente
```

### Validación

- Stock debe estar disponible al crear orden
- No se puede reservar más del disponible
- Las reservas vencidas se limpian automáticamente cada minuto

---

## 🚦 Rate Limiting

### Estrategia de Rate Limiting

Protege la API contra abuso y DDoS:

| Endpoint | Límite | Ventana |
|----------|--------|---------|
| **Global** | 100 requests | 1 minuto |
| **Auth** | 6 requests | 1 minuto |
| **Perfil (ver)** | 10 requests | 1 minuto |
| **Perfil (actualizar)** | 5 requests | 30 segundos |
| **Catálogo** | 60 requests | 30 segundos |
| **Pagos** | 5 requests | 1 minuto |

### Implementación

```csharp
options.AddPolicy("auth", context =>
    RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: context.Connection.RemoteIpAddress?.ToString(),
        factory: _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 6,
            Window = TimeSpan.FromMinutes(1)
        }
    )
);
```

---

## 🗄️ Base de Datos

### Diagrama de Relaciones

```
┌─────────────┐
│    Users    │◄──┐
└──────┬──────┘   │ (One-to-Many)
       │          │
       ├──────────┴─ CartItems
       │
       └──────────┐
                  │ (One-to-Many)
                  ▼
            ┌─────────────┐
            │   Orders    │
            └──────┬──────┘
                   │ (One-to-Many)
                   ▼
            ┌─────────────────┐
            │   OrderItems    │
            └────────┬────────┘
                     │ (Many-to-One)
                     ▼
            ┌─────────────────┐
            │   Products      │◄──┐
            └────────┬────────┘   │
                     │        (Many-to-One)
                     ▼            │
            ┌──────────────────┐  │
            │   Categories     │──┘
            └──────────────────┘
```

### Índices Optimizados

```
✅ Users.Email (UNIQUE)
✅ Orders.MercadoPagoPaymentId (UNIQUE)
✅ Orders.Status (búsquedas y filtros)
✅ Orders.ReservationExpiresAt (limpieza automática)
```

### Modelos Principales

#### **User**
- Soft delete (IsDisabled)
- Email único
- Roles: User, Admin
- Relaciones: CartItems, Orders

#### **Product**
- Stock disponible y reservado
- Total vendido (métricas)
- Relación con Category
- Protección contra eliminación

#### **Order**
- Estados: AwaitingPayment, Paid, Preparing, Shipped, Delivered, Cancelled, Expired
- Integración MercadoPago
- Items: OrderItem (One-to-Many con Cascade)
- Timestamps para auditoría

#### **CartItem**
- Referencia a Product y User
- Cantidad
- Cálculo automático de subtotal

---

## 🔒 Seguridad

### Implementada ✅

1. **Encriptación de Contraseñas**
   - BCrypt con salt automático
   - No se almacenan contraseñas en texto plano

2. **CORS (Cross-Origin Resource Sharing)**
   - Solo dominios permitidos (localhost, Vercel)
   - Credentials permitidas

3. **CSRF Protection**
   - Tokens CSRF validados automáticamente
   - Cookies HttpOnly

4. **Rate Limiting**
   - Protección global y por endpoint
   - IP-based partitioning

5. **JWT Security**
   - Firma HS256
   - Validación de issuer y audience
   - Clock skew = 0 (sin tolerancia)

6. **SQL Injection Prevention**
   - Entity Framework Core (parameterized queries)
   - No queries raw concatenadas

7. **HTTPS Enforcement**
   - Certificados válidos en producción
   - HSTS headers

8. **Soft Delete**
   - Usuarios pueden deshabilitar sin borrar
   - Preserva datos para auditoría

9. **Comportamiento de Eliminación**
   - Restrict en productos usados
   - Cascade en items de orden

---

## 🔄 Integración con Frontend

### Flujo de Comunicación

```
Frontend (Next.js/React)          Backend (ASP.NET Core)
        │                         │
        ├──────────── POST ───────▶ /api/auth/register
        │                         │
        ◀──────── JWT Token ──────┤
        │                         │
        ├──────────── GET ────────▶ /api/product?page=1
        │                         │
        ◀─── Product List JSON ───┤
        │                         │
        ├──────────── POST ───────▶ /api/order
        │  (con Authorization)     │
        ◀──────── Order ID ───────┤
        │                         │
        ├──────────── POST ───────▶ /api/payments/create-preference
        │                         │
        ◀─ MercadoPago Preference ┤
```

### CORS Configurado

```javascript
// Desde el frontend pueden hacer requests a:
Allowed Origins: [
    "http://localhost:3000",
    "https://ecommerce-git-main-gastonreds-projects.vercel.app"
]

Métodos: GET, POST, PUT, DELETE, PATCH
Headers: Authorization, Content-Type, Content-Length, X-XSRF-TOKEN
Credentials: true (con cookies)
```

---

## 🌟 Puntos Destacados del Proyecto

### Para Recruiters de .NET Junior

✨ **Arquitectura Profesional**
- Clean Architecture con separación de responsabilidades clara
- SOLID Principles aplicados correctamente
- Layered architecture bien definida

✨ **Patrones de Diseño**
- Repository Pattern (EF Core)
- Dependency Injection completo
- DTO Pattern para seguridad
- Service Layer pattern
- Middleware pattern

✨ **Prácticas de Desarrollo**
- Entity Framework Core 9 (ORM moderno)
- Async/await en todas las operaciones I/O
- Migrations versionadas
- Validación de datos
- Manejo de errores robusto

✨ **Seguridad**
- Autenticación JWT correctamente implementada
- CORS restrictivo
- CSRF Protection
- Rate limiting granular
- Bcrypt para contraseñas

✨ **Escalabilidad**
- Paginación en todos los listados
- Índices de base de datos optimizados
- Servicios en background (Hosted Services)
- Rate limiting contra abuso

✨ **Productividad**
- Swagger/OpenAPI documentado
- Code organization intuitiva
- Reutilización de servicios
- Fácil de testear y mantener

✨ **Integraciones Reales**
- Integración completa con MercadoPago
- Cloudinary para CDN de imágenes
- PostgreSQL como BD profesional
- Docker ready

✨ **Características Avanzadas**
- Sistema de reservas de inventario
- Idempotencia en pagos
- Webhooks seguros
- Soft delete
- Servicios en background

---

## 🛠️ Guía de Desarrollo

### Ejecutar en Modo Desarrollo

```bash
# 1. Restaurar dependencias
dotnet restore

# 2. Crear BD
dotnet ef database create

# 3. Ejecutar con watch (reload automático)
dotnet watch run

# 4. Acceder a Swagger
https://localhost:5001/swagger
```

### Crear una Nueva Migración

```bash
# Crear migración
dotnet ef migrations add NombreMigracion -p ECommerceAPI

# Aplicar cambios
dotnet ef database update
```

### Estructura de un Nuevo Feature

Ejemplo: Agregar sistema de **Reviews** de productos

```
1. Crear Modelo
   📄 Models/Review.cs

2. Actualizar DbContext
   📄 Data/AppDbContext.cs
   └─> public DbSet<Review> Reviews { get; set; }

3. Crear DTOs
   📄 DTOs/Review/ReviewCreateDto.cs
   📄 DTOs/Review/ReviewResponseDto.cs

4. Crear Servicio
   📄 Services/Reviews/IReviewService.cs
   📄 Services/Reviews/ReviewService.cs

5. Registrar Servicio
   📄 Program.cs
   └─> builder.Services.AddScoped<IReviewService, ReviewService>();

6. Crear Controller
   📄 Controllers/ReviewController.cs

7. Crear Migración
   ➜ dotnet ef migrations add AddReviews
   ➜ dotnet ef database update

8. Documentar en Swagger
```

### Testing

```bash
# Si hay proyecto de tests
dotnet test

# Con coverage
dotnet test /p:CollectCoverage=true
```

### Build y Publicación

```bash
# Release build
dotnet build -c Release

# Publicar
dotnet publish -c Release -o ./publish
```

### Docker

```bash
# Construir imagen (Dev)
docker build -t ecommerceapi:dev --target base .

# Construir imagen (Producción)
docker build -t ecommerceapi:latest .

# Ejecutar contenedor
docker run -p 5001:8080 ecommerceapi:latest
```

---

## 📞 Información Adicional

### Tecnologías Versiones
- .NET 8.0 (LTS)
- C# 12
- Entity Framework Core 9.0
- PostgreSQL 12+
- ASP.NET Core 8.0

### Dependencias Principales
```xml
<PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
<PackageReference Include="CLoudinaryDotNet" Version="1.27.9" />
<PackageReference Include="mercadopago-sdk" Version="2.11.0" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.*" />
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="9.0.4" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="10.0.1" />
```

### Contacto y Recursos
- 📚 [Documentación ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/)
- 📚 [Entity Framework Core Docs](https://docs.microsoft.com/en-us/ef/core/)
- 🔐 [JWT.io](https://jwt.io/)
- 💳 [MercadoPago Docs](https://www.mercadopago.com.ar/developers/en)

---