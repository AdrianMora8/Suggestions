# 🧩 Sudoku Solver con Interfaz Gráfica

## Descripción

Aplicación Sudoku con interfaz gráfica que permite a un jugador resolver puzzles de Sudoku manualmente o con ayuda del algoritmo de **Backtracking**.

## Características ✨

### Interfaz Gráfica
- **Tablero 9x9** interactivo con celdas editables
- **Líneas gruesas** que delimitan las cajas 3x3
- **Celdas originales** marcadas en azul (no editables)
- **Celdas jugables** en blanco (editables)
- **Validación en tiempo real** de movimientos
- **Navegación** con Tab o flechas del teclado

### Funcionalidades

#### 📋 **Cargar Ejemplo**
Carga un Sudoku de dificultad media para resolver.

#### 🤖 **Resolver**
Utiliza el algoritmo de **Backtracking** para resolver automáticamente el Sudoku actual.
- Algoritmo: Búsqueda exhaustiva con retroceso
- Encuentra solución completa si existe
- Muestra mensaje si no hay solución válida

#### 💡 **Pista**
Solicita una sugerencia al solver para una celda vacía aleatoria.
- Resalta la celda en amarillo temporalmente
- Muestra el número correcto para esa posición

#### ✓ **Verificar**
Comprueba si el estado actual del tablero es válido.
- Marca celdas con errores en rojo
- Muestra contador de celdas completadas
- Detecta cuando se completa correctamente

#### 🗑️ **Limpiar**
Borra todo el tablero para empezar un nuevo Sudoku.

## Controles del Teclado ⌨️

- **Números 1-9**: Ingresar valor en celda seleccionada
- **Backspace/Delete**: Borrar celda
- **Tab**: Avanzar a siguiente celda
- **Shift + Tab**: Retroceder a celda anterior
- **Flechas**: Navegar por el tablero

## Reglas del Sudoku 📐

1. **Filas**: Cada fila debe contener los números 1-9 sin repetir
2. **Columnas**: Cada columna debe contener los números 1-9 sin repetir
3. **Cajas 3x3**: Cada caja debe contener los números 1-9 sin repetir

## Backend (Sin Modificar) 🔧

El backend existente se mantiene intacto:

### Modelos
- **`SudokuBoard`**: Representa el tablero 9x9
  - `Get(row, col)`: Obtiene valor de celda
  - `Set(row, col, value)`: Establece valor
  - `IsEmpty(row, col)`: Verifica si celda está vacía
  - `IsValidPlacement(row, col, value)`: Valida reglas Sudoku
  - `EmptyCells()`: Retorna celdas vacías
  - `Clone()`: Crea copia del tablero

### Solvers
- **`ISudokuSolver`**: Interfaz para solvers
- **`BacktrackingSudokuSolver`**: Implementación con backtracking
  - Algoritmo recursivo
  - Búsqueda exhaustiva con retroceso
  - Complejidad: O(9^n) donde n = celdas vacías

## Arquitectura de la Solución 🏗️

```
SudokuApp/
├── Models/
│   └── SudokuBoard.cs          (Backend original - NO modificado)
├── Solvers/
│   ├── ISudokuSolver.cs        (Backend original - NO modificado)
│   └── BacktrackingSudokuSolver.cs  (Backend original - NO modificado)
├── Utils/
│   └── SudokuPrinter.cs        (Consola - ya no se usa en GUI)
├── Forms/
│   └── SudokuForm.cs           (NUEVO - Interfaz gráfica)
└── Program.cs                  (Modificado para WinForms)
```

## Algoritmo Backtracking 🧠

### Funcionamiento

```
1. Encontrar primera celda vacía
2. Si no hay celdas vacías → RESUELTO
3. Para cada número del 1 al 9:
   a. Si el número es válido en esa posición:
      - Colocar número
      - Llamar recursivamente
      - Si retorna éxito → RESUELTO
      - Si falla → Borrar número (backtrack)
4. Si ningún número funciona → FALLÓ (retroceder)
```

### Ventajas
- ✅ Encuentra solución garantizada si existe
- ✅ No requiere heurísticas complejas
- ✅ Implementación simple y elegante

### Desventajas
- ❌ Puede ser lento en casos complejos
- ❌ Complejidad exponencial en peor caso

## Ejemplo de Uso 🎮

### Caso 1: Resolver Sudoku Manualmente
1. Click en "📋 Cargar Ejemplo"
2. Observa las celdas azules (fijas) y blancas (editables)
3. Haz click en una celda vacía
4. Escribe un número del 1-9
5. El color cambia a rojo si es inválido
6. Continúa hasta completar
7. Click en "✓ Verificar" para comprobar

### Caso 2: Usar Solver Automático
1. Click en "📋 Cargar Ejemplo"
2. Click en "🤖 Resolver"
3. El algoritmo completa el tablero en segundos
4. ¡Sudoku resuelto!

### Caso 3: Jugar con Ayuda
1. Carga un ejemplo
2. Completa algunas celdas manualmente
3. Click en "💡 Pista" cuando te atores
4. Recibe sugerencia para una celda
5. Continúa hasta completar

## Validaciones Implementadas ✅

### En Tiempo Real
- ❌ No permite números repetidos en fila
- ❌ No permite números repetidos en columna  
- ❌ No permite números repetidos en caja 3x3
- ❌ Solo acepta números 1-9
- ❌ No permite editar celdas originales

### Al Verificar
- ✓ Comprueba todas las reglas
- ✓ Marca errores visualmente
- ✓ Cuenta celdas completadas
- ✓ Detecta victoria

## Tecnologías Utilizadas 💻

- **.NET 9.0** (Windows Forms)
- **C#** con nullable reference types
- **Algoritmo Backtracking** para resolución
- **Interfaz gráfica** con WinForms
- **Arquitectura SOLID** (backend original)

## Compilar y Ejecutar 🚀

```bash
# Compilar
dotnet build SudokuApp.csproj

# Ejecutar
dotnet run --project SudokuApp.csproj
```

O simplemente presiona **F5** en Visual Studio.

## Código Original Preservado 📦

El siguiente código del backend **NO fue modificado**:

✅ `Models/SudokuBoard.cs`  
✅ `Solvers/ISudokuSolver.cs`  
✅ `Solvers/BacktrackingSudokuSolver.cs`  
✅ `Utils/SudokuPrinter.cs` (aún disponible para consola)

Solo se agregaron:

➕ `Forms/SudokuForm.cs` (interfaz gráfica nueva)  
🔄 `Program.cs` (modificado para lanzar GUI)  
🔄 `SudokuApp.csproj` (convertido a WinForms)

## Mejoras Futuras (Opcionales) 🔮

- ⭐ Diferentes niveles de dificultad
- ⭐ Cronómetro
- ⭐ Sistema de puntuación
- ⭐ Undo/Redo
- ⭐ Notas en celdas (números candidatos pequeños)
- ⭐ Guardar/Cargar partidas
- ⭐ Generador de Sudokus aleatorios
- ⭐ Modos de juego (Classic, X-Sudoku, etc.)
- ⭐ Estadísticas de juego
- ⭐ Visualización animada del backtracking

## Principios de Diseño 🎯

### SOLID (Mantenido del Backend Original)
- **S**ingle Responsibility: Cada clase tiene una responsabilidad única
- **O**pen/Closed: ISudokuSolver permite extensión
- **L**iskov Substitution: Solvers son intercambiables
- **I**nterface Segregation: Interfaces específicas
- **D**ependency Inversion: Dependencia de abstracciones

### Separación de Capas
- **Modelo** (SudokuBoard): Lógica de negocio
- **Vista** (SudokuForm): Presentación
- **Controlador** (Solver): Algoritmos

## Notas Técnicas 📝

- Las advertencias CS8618 son benignas (los campos se inicializan en `InitializeUI()`)
- El tablero usa coordenadas 0-8 internamente (mostradas como 1-9 al usuario)
- La validación distingue entre estado inicial y jugadas del usuario
- SystemSounds.Beep se usa para feedback auditivo cuando intenta editar celda fija

---

**¡Disfruta jugando y resolviendo Sudokus!** 🎉
