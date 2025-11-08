using CookieGameApp.Models;

namespace CookieGameApp.Solvers
{
    /// <summary>
    /// Interfaz para solvers del juego Dots and Boxes.
    /// Principio de Inversión de Dependencias (DIP): 
    /// Los módulos de alto nivel no dependen de implementaciones concretas.
    /// </summary>
    public interface IDotsBoxesSolver
    {
        /// <summary>
        /// Sugiere la mejor línea para marcar dado el estado actual del tablero.
        /// </summary>
        /// <param name="board">Tablero actual del juego</param>
        /// <returns>La línea sugerida, o null si no hay movimientos disponibles</returns>
        Line? SuggestBestMove(GameBoard board);

        /// <summary>
        /// Evalúa qué tan buena es una jugada potencial.
        /// </summary>
        /// <param name="board">Tablero actual</param>
        /// <param name="line">Línea a evaluar</param>
        /// <returns>Puntaje de la jugada (mayor es mejor)</returns>
        int EvaluateMove(GameBoard board, Line line);
    }
}
