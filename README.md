# userAuthentication
Creación y Autenticación de usuarios x JWT

# Clonar repositorio
```
git clone https://github.com/luisgprez/userAuthentication.git
ct tu_repositorio
```

# Antes de iniciar el proyecto
-Ejecutar script adjunto llamado UserAuthentication.sql que se encuentra en DB/ y crea la base de datos correspondiente, junto con sus tablas y valores iniciales que se requieren.
IMPORTANTE no alterar nombres o valores (En caso de realizarlo se requerira actualizar el o los modelos correspondientes).

# Iniciando el proyecto
Actualizar archivo appsettings.json que se encuentra en raiz
En el apartado de ConnectionStrings, se debe de adecuar la coneccion con el nombre UserAuthConection.
Si se tiene autenticacion por windows unicamente se debe de incluir el nombre en en apartado de data source y sustituir SERVER por el nombre del servidor correspondiente donde se creo UserAuthentication
ej.data source=LOCAL_SERVER;initial catalog=UserAuthentication;Trusted_Connection=True;TrustServerCertificate=True
En caso de que se tenga autenticacion por SQL server authentication se requiere suprimir Trusted_Connection=True y agregar los atritutos user id y password con sus respectivos datos
ej.data source=LOCAL_SERVER;initial catalog=UserAuthentication;user id=USUARIO;password=CONTRASEÑA;TrustServerCertificate=True


# Instalar dependencias
Abre la terminal en la raíz del proyecto y ejecuta:
```
dotnet restore
```

# Ejecutar el proyecto
```bash
dotnet run
```

# Pruebas
Puedes realizar pruebas directas desde el swagger que lanza el proyecto en 
`https://localhost:5001/swagger`
Que son los puertos por defecto, la API estará disponible en:  
```
https://localhost:5001`
http://localhost:5000
```
Si llegan a cambiar estos puertos adecuarlos
