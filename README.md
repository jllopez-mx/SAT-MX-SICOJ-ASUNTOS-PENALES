# Proyecto Back que implementa un API para el sistema AppAPI... escrito en su mayoría en C#

## Acerca de la plataforma y estructura del desarrollo

 Este proyecto se encuentra desarrollado con Net 6

[Más información sobre net](https://dotnet.microsoft.com/download)

[Instalación de net 6 en Linux](https://learn.microsoft.com/en-us/dotnet/core/install/linux)

[Instalación de net 6 en Windows](https://learn.microsoft.com/en-us/dotnet/core/install/windows?tabs=net60)

Proyectos incluidos en la solución ***AppAPI.sln***

| Directorio | Descripción |
| ----------- | ----------- |
| [AppAPI](AppAPI/README.md) | Proyecto AppAPI - Implementa un API Rest para el consumo por medio de HTTP |
| libs (libs) | Librerias dll externas necesarias para la ejecución del proyecto |

## Estructura del proyecto

| Directorio | Descripción | 
| ----------- | ----------- | 
| 📂 Api/V1/Controllers | Clases que implementan un controlador HTTP con el fin de filtrar los accesos y direcionar las peticiones a las diferentes funcionalidades implementadas | 
| 📂 Helpers | Contiene las clases genericas al proyecto  |
| 📂 Middleware | Contiene clases que implementan middlewares (Intercambio de información con otros sistemas) |
| 📂 Util | Contiene clases con metodos genericos al proyecto |
| 📂 Model | Capeta que contiene todo el "negocio" del sistema  |
| 📂 IDAO | Contiene las interfaces que definen las operaciones que define el negocio de la aplicación  |
| 📂 DAO - ServicesDAO | Data Access Object: Clases que implementan el acceso al repositorio (DB)  |
| 📂 DTO | Data Transfer Object: Clases que sirven como auxiliar para obtener información entrante desde los controladores  |
| 📂 Entities | Entidades relacionadas al negocio, normalmente es un mapeo de los objetos del repositorio |
| 📂 ViewModels | Clases auxiliares que sirven para trasmitir información en una interfaz |

Dependencias 

| Libreria | Descripción |
| ----------- | ----------- |
| ConnectionTools.dll | Librería para implementar conexiónes a repositorios |
| Util.dll | Librería con funciones varias necesarias para los diferentes proyectos |

   
## Arquitectura de software implementada en el proyecto

![Alt Arquitectura de software propuesta](/DOCUMENTOS/arquitectura/arquitectura.svg?raw=true "Arquitectura de software implementada")

### Desarrollo.
<hr>
  
Restaurar dependencias

```bat
DOTNET_SYSTEM_NET_HTTP_USESOCKETSHTTPHANDLER=0 dotnet restore
```

Iniciar proyecto

```bat
DOTNET_SYSTEM_NET_HTTP_USESOCKETSHTTPHANDLER=0 dotnet run

or

DOTNET_SYSTEM_NET_HTTP_USESOCKETSHTTPHANDLER=0 dotnet watch run

```

Compilar proyecto para linux

```bat
DOTNET_SYSTEM_NET_HTTP_USESOCKETSHTTPHANDLER=0 dotnet publish -o ../releases/release -c Release -r linux-x64
```


### Documentación del API   

<hr>

http://host-de-ejecución:puerto/documentation

<br>
<hr>
<br>

### No olvidar implementar   
   
<div style="color: #664d03;background-color: #fff3cd;border-color: #c3e6cb;position: relative;
    padding: .75rem 1.25rem;
    margin-bottom: 1rem;
    border: 1px solid transparent;
    border-radius: .25rem;">
  <svg style="width: .6875em;" aria-hidden="true" focusable="false" data-prefix="fas" data-icon="lightbulb" role="img" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 352 512" data-fa-i2svg=""><path fill="currentColor" d="M96.06 454.35c.01 6.29 1.87 12.45 5.36 17.69l17.09 25.69a31.99 31.99 0 0 0 26.64 14.28h61.71a31.99 31.99 0 0 0 26.64-14.28l17.09-25.69a31.989 31.989 0 0 0 5.36-17.69l.04-38.35H96.01l.05 38.35zM0 176c0 44.37 16.45 84.85 43.56 115.78 16.52 18.85 42.36 58.23 52.21 91.45.04.26.07.52.11.78h160.24c.04-.26.07-.51.11-.78 9.85-33.22 35.69-72.6 52.21-91.45C335.55 260.85 352 220.37 352 176 352 78.61 272.91-.3 175.45 0 73.44.31 0 82.97 0 176zm176-80c-44.11 0-80 35.89-80 80 0 8.84-7.16 16-16 16s-16-7.16-16-16c0-61.76 50.24-112 112-112 8.84 0 16 7.16 16 16s-7.16 16-16 16z"></path></svg>
  <strong>Nota:</strong> Si existe alguna propuesta es mejor agregarla como issue en el gestor del proyecto indicado (gitlab, taiga) para su seguimiento antes de incluirla en esta lista.
</div>

> **TODO**
> 
>     - [ ] Implementar archivos de configuración.
>     - [ ] Implementar OAUTH 2.0 del SAT.
>     - [ ] Encriptación de parametros "sensibles" dentro de archivos de configuración donde las claves o la estructura completa se obtenga desde Vault.
>     - [ ] Configuración para documentación con OpenAPI.
>     - [ ] Registro de Dlls externas en archivo de configuración del proyecto.
