namespace CookieGameApp.Models
{
    /// <summary>
    /// Representa un cuadro (caja) formado por 4 líneas.
    /// Un cuadro es completado cuando sus 4 lados están marcados.
    /// </summary>
    public sealed class Box
    {
        public int Row { get; }
        public int Col { get; }
        public Player? Owner { get; private set; }

        // Referencias a las 4 líneas que forman el cuadro
        public Line TopLine { get; }
        public Line BottomLine { get; }
        public Line LeftLine { get; }
        public Line RightLine { get; }

        public Box(int row, int col, Line top, Line bottom, Line left, Line right)
        {
            Row = row;
            Col = col;
            TopLine = top;
            BottomLine = bottom;
            LeftLine = left;
            RightLine = right;
            Owner = null;
        }

        /// <summary>
        /// Verifica si el cuadro está completamente cerrado.
        /// </summary>
        public bool IsCompleted => !TopLine.IsAvailable && !BottomLine.IsAvailable && 
                                   !LeftLine.IsAvailable && !RightLine.IsAvailable;

        /// <summary>
        /// Cuenta cuántos lados están ya marcados.
        /// </summary>
        public int SidesMarked => 
            (TopLine.IsAvailable ? 0 : 1) +
            (BottomLine.IsAvailable ? 0 : 1) +
            (LeftLine.IsAvailable ? 0 : 1) +
            (RightLine.IsAvailable ? 0 : 1);

        /// <summary>
        /// Intenta reclamar el cuadro para un jugador si está completado.
        /// </summary>
        public bool TryClaimFor(Player player)
        {
            if (IsCompleted && Owner == null)
            {
                Owner = player;
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            return $"Box({Row},{Col}) - Sides:{SidesMarked}/4 - {(Owner != null ? $"Owned by {Owner.Name}" : "Unclaimed")}";
        }
    }
}
