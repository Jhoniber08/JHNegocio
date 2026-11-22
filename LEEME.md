# JH Negocio — Consumir un servicio REST API desde .NET MAUI
**Unidad 2 · Actividad 1** — Autor: Jhoniber De la Cruz

## Estructura de la solución
```
JHNegocio.sln
├── JHNegocio.Api/                  ← SERVICIO REST (ASP.NET Core Web API, .NET 10)
│   ├── Controllers/ProductosController.cs   GET, POST, PUT, DELETE /api/productos
│   ├── Models/Producto.cs                   Producto, ProductoDto, ResumenInventario
│   ├── Data/ProductoRepository.cs           Datos en memoria (7 productos de ejemplo)
│   ├── Program.cs                           Configuración + Swagger
│   └── JHNegocio.Api.http                   Pruebas listas para Visual Studio
│
└── JHNegocio.Movil/                ← APP .NET MAUI QUE CONSUME LA API (MVVM)
    ├── Models/Producto.cs                   Modelos que reciben el JSON
    ├── Services/ApiConfig.cs                URL de la API (10.0.2.2 en Android)
    ├── Services/ProductoService.cs          HttpClient: GET, POST, DELETE
    ├── ViewModels/                          BaseViewModel, ProductosViewModel, NuevoProductoViewModel
    ├── Views/ProductosPage.xaml             Lista + resumen + búsqueda + estado de conexión
    ├── Views/NuevoProductoPage.xaml         Formulario que hace POST
    └── MauiProgram.cs                       Inyección de dependencias
```

## Endpoints del servicio
| Método | Ruta | Qué hace |
|---|---|---|
| GET | /api/productos | Lista productos (acepta `?buscar=`) |
| GET | /api/productos/{id} | Un producto |
| GET | /api/productos/resumen | Totales del inventario |
| POST | /api/productos | Crea un producto (201 Created) |
| PUT | /api/productos/{id} | Actualiza |
| DELETE | /api/productos/{id} | Elimina (204 No Content) |

## Cómo ejecutarlo (Visual Studio 2022 17.14+ o 2026, con carga de trabajo .NET MAUI y SDK .NET 10)
1. Descomprime el .zip y abre `JHNegocio.sln`.
2. Clic derecho en la **Solución** → **Configurar proyectos de inicio** → **Varios proyectos de inicio**.
   Pon **JHNegocio.Api = Iniciar** y **JHNegocio.Movil = Iniciar**. Aceptar.
3. Arriba, en el selector de destino de JHNegocio.Movil, elige **Android Emulator** (o **Windows Machine**).
4. Presiona **F5**. Se abre el navegador con Swagger (la API en http://localhost:5080) y luego la app.

> Si el emulador muestra "Sin conexión con la API", confirma que la API está corriendo en el puerto 5080.

## Capturas para el PDF
1. Solution Explorer mostrando los dos proyectos.
2. `ProductosController.cs` abierto.
3. `ProductoService.cs` abierto (el HttpClient que consume la API).
4. Swagger en el navegador con los endpoints.
5. Swagger: **GET /api/productos → Try it out → Execute** con respuesta **200** y el JSON.
6. La app mostrando la lista con el punto verde **"Conectado"**.
7. La app: formulario **Nuevo producto** lleno.
8. La app: alerta **"Producto guardado — La API creó el producto con Id 8"**.
9. La lista actualizada con el producto nuevo.

---

## Opción sin PC: GitHub (todo desde el celular)

### A. Ejecutar la API en GitHub Codespaces
1. En el repositorio: botón verde **Code** → pestaña **Codespaces** → **Create codespace on main**.
   (En el celular, activa "Sitio de escritorio" en el navegador.)
2. Espera a que cargue VS Code. En la **Terminal** escribe:
   ```
   dotnet run --project JHNegocio.Api
   ```
3. Cuando diga `Now listening on: http://localhost:5080`, abre la pestaña **Ports (Puertos)**.
   Clic derecho en el puerto **5080** → **Port Visibility → Public**.
4. Abre la dirección del puerto (termina en `-5080.app.github.dev`) y agrega `/swagger`.
   → Captura: Swagger con los endpoints y una prueba **GET → 200 OK**.

### B. Generar el APK con GitHub Actions
1. Pestaña **Actions** → **Compilar APK JH Negocio** → **Run workflow**.
2. Cuando termine (✓ verde, unos 10–15 min), entra a la ejecución y descarga **JHNegocio-APK**.
3. Descomprime e instala el `.apk` en tu Android (permite "instalar apps desconocidas").

### C. Conectar la app con la API
1. Abre JH Negocio en el celular → botón **Servidor**.
2. Pega la URL pública del Codespace, ej. `https://xxxx-5080.app.github.dev`.
3. La app consulta la API: punto verde **Conectado**, lista de productos, agregar y eliminar.
   → Capturas reales de la app consumiendo el servicio.

### D. Evidencia extra: prueba automática de la API
Pestaña **Actions** → **Probar API JH Negocio** → **Run workflow**.
El log muestra cada petición (GET, POST, DELETE) con su código HTTP y el JSON de respuesta.

---

## Unidad 2 · Actividad 2 — Reproducir multimedia
Pantalla **Multimedia** (botón en la barra superior del inventario) con `MediaElement` del .NET MAUI Community Toolkit:
- `Resources/Raw/promo_jhnegocio.mp4`: video promocional original (20 s, H.264 + AAC).
- `Resources/Raw/jingle_jhnegocio.mp3`: jingle original (18 s).
- Controles: reproducir/pausar, detener, barra de progreso con búsqueda, volumen y estado.
- No necesita la API: los archivos van dentro del APK.
