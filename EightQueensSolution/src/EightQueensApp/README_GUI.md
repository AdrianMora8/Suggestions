# EightQueensApp GUI

Interfaz gráfica para el problema de N-Reinas (por defecto 8) sin modificar el backend.

## Características
- Tablero dibujado con estilo ajedrez.
- Selector de tamaño N (4 a 14), por defecto 8.
- Resolver con backtracking (usa `BacktrackingSolver`).
- Navegación por soluciones: Anterior, Siguiente y Aleatoria.
- Indicador de estado: cuántas soluciones y cuál se muestra.

## Ejecutar
```pwsh
dotnet run --project .\EightQueensSolution\src\EightQueensApp\EightQueensApp.csproj
```

## Notas
- El backend (Models y Solvers) permanece intacto.
- El render usa el símbolo Unicode ♛ para las reinas; si en tu sistema se ve distinto, es normal.
