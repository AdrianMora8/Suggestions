# EightPuzzleApp GUI

Interfaz gráfica para el 8-puzzle (3x3) sin modificar el backend.

## Características
- Tablero 3x3 con botones (1..8) y un hueco.
- Click en una ficha adyacente para moverla al hueco.
- Barajar (genera un estado válido al aplicar movimientos aleatorios).
- Reiniciar al estado inicial.
- Resolver con A* usando el solver existente (animación paso a paso).
- Contador de movimientos y mensajes de estado.

## Cómo ejecutar

```pwsh
# En Windows PowerShell
dotnet run --project .\EightQueensSolution\src\EightPuzzleApp\EightPuzzleApp.csproj
```

## Uso
- "Barajar": genera un rompecabezas válido.
- "Reiniciar": vuelve al estado inicial actual.
- "Resolver (A*)": calcula y anima la solución.
- Cuando el rompecabezas queda resuelto, verás un mensaje de éxito.

## Notas
- El backend (Models/Solvers/Utils) permanece intacto.
- El barajado se hace aplicando movimientos legales desde el estado objetivo, garantizando solvencia.
- La animación puede tardar si la instancia es compleja, pero el 8-puzzle es manejable.
