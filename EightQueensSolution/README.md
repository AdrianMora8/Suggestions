# Problemas Suplementarios - Soluciones en C# con Inteligencia Artificial

Proyecto que implementa 4 problemas clásicos usando C#, POO, principios SOLID, patrones de software y **algoritmos de Inteligencia Artificial**.

## 📋 Problemas Implementados

### 1. **8 Reinas** (Backtracking)
Resuelve el problema de las N-reinas usando backtracking (algoritmo de IA).

**Estructura:**
- `Models/Board.cs` — modelo del tablero con validación
- `Solvers/IQueenSolver.cs` — interfaz (DIP)
- `Solvers/BacktrackingSolver.cs` — algoritmo de backtracking
- `Utils/SolutionPrinter.cs` — impresión (SRP)

**Ejecutar:**
```powershell
dotnet run --project src\EightQueensApp\EightQueensApp.csproj -- 8
```

---

### 2. **8 Puzzle** (A* con Heurística Manhattan)
Puzzle deslizante 3x3 con modo interactivo y solver automático usando A* (algoritmo de búsqueda informada).

**Estructura:**
- `Models/PuzzleState.cs` — estado del puzzle
- `Solvers/IPuzzleSolver.cs` — interfaz (DIP)
- `Solvers/AStarSolver.cs` — algoritmo A* con búsqueda heurística
- `Solvers/Heuristics/Heuristics.cs` — heurística Manhattan

**Características IA:**
- ✅ A* con heurística Manhattan para búsqueda óptima
- ✅ Validación de estados solucionables
- ✅ Modo interactivo y resolución automática

**Ejecutar:**
```powershell
dotnet run --project src\EightPuzzleApp\EightPuzzleApp.csproj
```

---

### 3. **Cookie Game** (Greedy + Programación Dinámica) 🆕
Juego tipo Cookie Clicker con **algoritmos de IA** para optimización de estrategias.

**Estructura:**
- `Models/Producer.cs` — clase abstracta (herencia y polimorfismo)
- `Models/Cursor.cs`, `Grandma.cs` — productores concretos
- `Models/ProducerFactory.cs` — patrón Factory
- `Models/GameEngine.cs` — lógica del juego
- `Solvers/ICookieSolver.cs` — interfaz para solvers de IA (DIP)
- `Solvers/GreedySolver.cs` — **algoritmo Greedy con heurística de eficiencia**
- `Solvers/DynamicProgrammingSolver.cs` — **programación dinámica con memoización**

**Características IA:**
- ✅ **Algoritmo Greedy**: Búsqueda heurística que calcula eficiencia (CPS/costo)
- ✅ **Programación Dinámica**: Encuentra estrategias óptimas con memoización
- ✅ Comando `suggest` - IA recomienda mejor compra
- ✅ Comando `auto` - IA juega automáticamente
- ✅ Comando `strategy` - IA calcula plan completo para alcanzar objetivo
- ✅ Comando `solver` - Cambiar entre algoritmos (greedy/dynamic)

**Ejecutar:**
```powershell
dotnet run --project src\CookieGameApp\CookieGameApp.csproj
```

**Comandos disponibles:**
- `click` - Obtener 1 cookie manual
- `buy <id>` - Comprar productor
- `wait <s>` - Avanzar tiempo
- `suggest` - **IA sugiere mejor compra**
- `auto [n]` - **IA ejecuta n compras automáticamente**
- `strategy <num>` - **IA calcula estrategia óptima**
- `solver <tipo>` - Cambiar algoritmo (greedy/dynamic)
- `status` - Ver estado actual

---

### 4. **Sudoku** (Backtracking)
Juego de Sudoku con modo interactivo y solver automático usando backtracking.

**Estructura:**
- `Models/SudokuBoard.cs` — tablero con validación
- `Solvers/ISudokuSolver.cs` — interfaz (DIP)
- `Solvers/BacktrackingSudokuSolver.cs` — algoritmo de backtracking
- `Utils/SudokuPrinter.cs` — impresión formateada

**Ejecutar:**
```powershell
dotnet run --project src\SudokuApp\SudokuApp.csproj
```

---

## 🎯 Principios SOLID Aplicados

1. **SRP (Single Responsibility Principle)**: Cada clase tiene una única responsabilidad
   - Printers solo imprimen
   - Solvers solo resuelven
   - Models solo representan datos

2. **OCP (Open/Closed Principle)**: Abierto a extensión, cerrado a modificación
   - Nuevos solvers sin modificar código existente

3. **DIP (Dependency Inversion Principle)**: Dependencia de abstracciones
   - Interfaces: `IQueenSolver`, `IPuzzleSolver`, `ISudokuSolver`, `ICookieSolver`

## 🏗️ Patrones de Software

1. **Factory Pattern**: `ProducerFactory` para crear productores
2. **Strategy Pattern**: Interfaces de solvers permiten intercambiar algoritmos
3. **Template Method**: Clase abstracta `Producer` define estructura común

## 🧬 POO Aplicado

- ✅ **Herencia**: `Producer` → `Cursor`, `Grandma`
- ✅ **Polimorfismo**: Interfaces y clases abstractas
- ✅ **Encapsulación**: Propiedades privadas con acceso controlado
- ✅ **Abstracción**: Interfaces ocultan detalles de implementación

## 🤖 Algoritmos de IA Implementados

1. **Backtracking** (8 Reinas, Sudoku): Búsqueda exhaustiva con poda
2. **A* con Heurística Manhattan** (8 Puzzle): Búsqueda informada óptima
3. **Greedy con Heurística** (Cookie Game): Búsqueda voraz eficiente
4. **Programación Dinámica** (Cookie Game): Optimización con memoización

---

## 🚀 Compilar y Ejecutar Todo

```powershell
# Desde la raíz del proyecto
cd EightQueensSolution

# Compilar toda la solución
dotnet build

# Ejecutar cada aplicación
dotnet run --project src\EightQueensApp\EightQueensApp.csproj
dotnet run --project src\EightPuzzleApp\EightPuzzleApp.csproj
dotnet run --project src\CookieGameApp\CookieGameApp.csproj
dotnet run --project src\SudokuApp\SudokuApp.csproj
```

## 📚 Referencia

**Libro base**: "Design Patterns for Searching in C#" - Fred Mellender
