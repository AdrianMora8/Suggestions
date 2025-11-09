namespace CookieGameApp.Models
{
    /// <summary>
    /// Representa una línea entre dos puntos en el tablero.
    /// Una línea puede ser horizontal o vertical.
    /// </summary>
    public sealed class Line
    {
        public int Row { get; }
        public int Col { get; }
        public bool IsHorizontal { get; }
        public Player? Owner { get; private set; }

        public Line(int row, int col, bool isHorizontal)
        {
            Row = row;
            Col = col;
            IsHorizontal = isHorizontal;
            Owner = null;
        }

        public bool IsAvailable => Owner == null;

        public void Claim(Player? player)
        {
            if (Owner == null)
            {
                Owner = player;
            }
        }

        public override string ToString()
        {
            return $"Line({Row},{Col},{(IsHorizontal ? "H" : "V")}) - {(IsAvailable ? "Available" : $"Claimed by {Owner?.Name}")}";
        }

        public override bool Equals(object? obj)
        {
            if (obj is Line other)
            {
                return Row == other.Row && Col == other.Col && IsHorizontal == other.IsHorizontal;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Row, Col, IsHorizontal);
        }
    }
}
