using System.Drawing;

namespace CookieGameApp.Models
{
    /// <summary>
    /// Representa un jugador en el juego Dots and Boxes.
    /// Principio Single Responsibility: solo maneja información del jugador.
    /// </summary>
    public sealed class Player
    {
        public string Name { get; }
        public Color Color { get; }
        public int Score { get; private set; }
        public bool IsAI { get; }

        public Player(string name, Color color, bool isAI = false)
        {
            Name = name;
            Color = color;
            Score = 0;
            IsAI = isAI;
        }

        public void AddPoint()
        {
            Score++;
        }

        public void ResetScore()
        {
            Score = 0;
        }
    }
}
