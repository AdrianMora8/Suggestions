# 🎮 Dots and Boxes - Juego de la Galleta

## Descripción del Juego

**Dots and Boxes** (conocido popularmente como "Juego de la Galleta") es un juego de estrategia para dos jugadores donde el objetivo es cerrar más cuadros que el oponente.

### Reglas del Juego

1. **Tablero**: Cuadrícula de puntos (por defecto 5x5) que forman cuadros
2. **Turnos**: Los jugadores alternan marcando líneas entre puntos adyacentes
3. **Captura de cuadros**: Cuando un jugador completa el cuarto lado de un cuadro, lo captura y puede jugar de nuevo
4. **Victoria**: Gana quien haya capturado más cuadros al final

### Colores de Jugadores

- **Jugador 1**: Rojo 🔴
- **Jugador 2**: Azul 🔵

## 🏗️ Arquitectura del Código (Principios SOLID)

### 📦 Modelos (Models/)

#### `Player.cs`
- **Responsabilidad**: Representa un jugador con nombre, color, puntuación e indicador de IA
- **Principio SRP**: Solo gestiona información del jugador

#### `Line.cs`
- **Responsabilidad**: Representa una línea entre dos puntos (horizontal o vertical)
- **Estado**: Disponible o reclamada por un jugador
- **Inmutabilidad**: Una vez reclamada, no puede cambiar de dueño

#### `Box.cs`
- **Responsabilidad**: Representa un cuadro formado por 4 líneas
- **Relaciones**: Mantiene referencias a sus 4 líneas (top, bottom, left, right)
- **Lógica**: Detecta cuándo está completo y permite reclamarlo

#### `GameBoard.cs`
- **Responsabilidad**: Motor central del juego
- **Gestiona**:
  - Inicialización del tablero (líneas y cuadros)
  - Turnos de jugadores
  - Validación de movimientos
  - Detección de fin de juego
  - Determinación del ganador
- **Principio OCP**: Extensible sin modificar código base

### 🤖 Solvers (Solvers/)

#### `IDotsBoxesSolver.cs` (Interfaz)
- **Principio DIP**: Inversión de dependencias
- **Métodos**:
  - `SuggestBestMove()`: Sugiere la mejor línea
  - `EvaluateMove()`: Evalúa calidad de una jugada

#### `StrategicDotsBoxesSolver.cs`
- **Algoritmo**: Heurísticas avanzadas + Minimax simplificado
- **Estrategias**:
  1. **Oportunista**: Si puede completar un cuadro, lo hace
  2. **Defensiva**: Evita dar cuadros al oponente (no marca 3ª línea)
  3. **Territorial**: Controla el centro del tablero
  4. **Minimax**: Evaluación profunda de consecuencias

### 🎨 Interfaz Gráfica (Forms/)

#### `DotsAndBoxesForm.cs`
- **Responsabilidad**: Vista y control de la interfaz
- **Características**:
  - Dibujo del tablero con puntos y líneas
  - Detección de clicks en líneas
  - Visualización de cuadros completados
  - Panel de información (puntuaciones, turno)
  - Asistencia de IA (sugerencias)
  - Modo IA automática para Jugador 2

## 🎯 Estrategia de la IA

### Evaluación de Movimientos

La IA utiliza varios factores para evaluar cada movimiento:

```
Puntuación = 
  + (Cuadros completados × 100)
  - (Cuadros con 3 lados creados × 50)
  + (Control territorial × 2)
  + (Bonificación por seguridad en fase temprana)
```

### Prioridades

1. **Alta prioridad**: Completar cuadros disponibles
2. **Media prioridad**: Movimientos seguros (no dan cuadros)
3. **Baja prioridad**: Minimizar daño si no hay movimientos seguros

### Fases del Juego

- **Fase temprana** (< 30% del juego): Movimientos conservadores
- **Fase media**: Balance entre ataque y defensa
- **Fase final**: Agresiva, toma todos los cuadros posibles

## 🚀 Cómo Ejecutar

```bash
dotnet run --project CookieGameApp.csproj
```

O compilar y ejecutar:

```bash
dotnet build CookieGameApp.csproj
dotnet run
```

## 🎮 Controles del Juego

### Jugador Humano
- **Click izquierdo** en una línea para marcarla
- Las líneas disponibles se resaltan al pasar el mouse

### Botones de Control

- **💡 Sugerir Jugada**: La IA muestra en dorado la mejor línea
- **🤖 IA Juega Turno**: La IA ejecuta un movimiento (solo si está habilitada)
- **🔄 Nuevo Juego**: Reinicia el tablero

### Opciones

- **☑️ Jugador 2 es IA**: Activa/desactiva el modo automático para el jugador azul

## 📊 Ejemplo de Partida

```
Tablero inicial (5x5):
● — ● — ● — ● — ● — ●
|   |   |   |   |   |
● — ● — ● — ● — ● — ●
|   |   |   |   |   |
● — ● — ● — ● — ● — ●
|   |   |   |   |   |
● — ● — ● — ● — ● — ●
|   |   |   |   |   |
● — ● — ● — ● — ● — ●

Resultado ejemplo:
Rojo: 13 cuadros
Azul: 12 cuadros
¡Rojo gana! 🎉
```

## 🔧 Extensibilidad

### Añadir Nuevo Solver

```csharp
public class MySolver : IDotsBoxesSolver
{
    public Line? SuggestBestMove(GameBoard board)
    {
        // Tu algoritmo aquí
    }

    public int EvaluateMove(GameBoard board, Line line)
    {
        // Tu evaluación aquí
    }
}
```

### Cambiar Tamaño del Tablero

En `DotsAndBoxesForm.cs`, línea ~59:

```csharp
_board = new GameBoard(5, 5, player1, player2); // Cambiar dimensiones
```

### Añadir Más Jugadores (Futuro)

La arquitectura actual soporta 2 jugadores. Para más jugadores:
1. Modificar `GameBoard` para lista de jugadores
2. Ajustar lógica de turnos
3. Asignar colores adicionales

## 📝 Principios de Diseño Aplicados

### SOLID

✅ **S**ingle Responsibility: Cada clase tiene una responsabilidad única
✅ **O**pen/Closed: Extensible mediante interfaces (IDotsBoxesSolver)
✅ **L**iskov Substitution: Los solvers son intercambiables
✅ **I**nterface Segregation: Interfaces específicas y pequeñas
✅ **D**ependency Inversion: Dependencia de abstracciones, no implementaciones

### Patrones de Diseño

- **Strategy Pattern**: IDotsBoxesSolver permite diferentes estrategias de IA
- **Observer Pattern**: UI reacciona a cambios en el modelo
- **Factory Method**: Creación de jugadores con diferentes configuraciones

### POO

- **Encapsulación**: Propiedades privadas con getters públicos
- **Abstracción**: Interfaces ocultan implementación
- **Herencia**: Posible extender clases base
- **Polimorfismo**: Múltiples solvers intercambiables

## 🐛 Testing (Sugerencias)

```csharp
[Test]
public void TestBoxCompletion()
{
    var board = new GameBoard(2, 2, player1, player2);
    var line1 = board.GetLine(0, 0, true);
    var line2 = board.GetLine(0, 0, false);
    var line3 = board.GetLine(1, 0, true);
    var line4 = board.GetLine(0, 1, false);
    
    board.TryMarkLine(line1);
    board.TryMarkLine(line2);
    board.TryMarkLine(line3);
    board.TryMarkLine(line4);
    
    Assert.AreEqual(1, player1.Score);
}
```

## 📚 Referencias

- [Dots and Boxes - Wikipedia](https://en.wikipedia.org/wiki/Dots_and_Boxes)
- [Minimax Algorithm](https://en.wikipedia.org/wiki/Minimax)
- [SOLID Principles](https://en.wikipedia.org/wiki/SOLID)

## 👥 Créditos

Implementación del juego clásico Dots and Boxes con IA estratégica, siguiendo principios de ingeniería de software profesional.

---

**Nota**: Las advertencias de compilación CS8618 son benignas - todos los campos se inicializan correctamente en los constructores.
