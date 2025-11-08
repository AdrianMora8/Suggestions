using CookieGameApp.Models;

namespace CookieGameApp.Solvers
{
    /// <summary>
    /// Interfaz para solvers que toman decisiones inteligentes sobre qué productor comprar.
    /// Aplica DIP (Dependency Inversion Principle) permitiendo diferentes estrategias de IA.
    /// </summary>
    public interface ICookieSolver
    {
        /// <summary>
        /// Sugiere el mejor productor para comprar dado el estado actual del juego.
        /// </summary>
        /// <param name="engine">Motor del juego con el estado actual</param>
        /// <returns>El productor sugerido o null si no hay ninguno viable</returns>
        Producer? SuggestNextPurchase(GameEngine engine);

        /// <summary>
        /// Calcula una secuencia de compras optimizadas para alcanzar un objetivo.
        /// </summary>
        /// <param name="engine">Motor del juego</param>
        /// <param name="targetCookies">Cantidad objetivo de cookies</param>
        /// <param name="timeLimit">Tiempo límite en segundos (opcional)</param>
        /// <returns>Lista de IDs de productores a comprar en orden</returns>
        string[] CalculateOptimalStrategy(GameEngine engine, double targetCookies, double timeLimit = 3600);
    }
}
