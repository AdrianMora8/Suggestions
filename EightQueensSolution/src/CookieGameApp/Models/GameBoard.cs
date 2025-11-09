using System;
using System.Collections.Generic;
using System.Linq;

namespace CookieGameApp.Models
{
    /// <summary>
    /// Tablero del juego Dots and Boxes.
    /// Gestiona líneas, cuadros, jugadores y el estado del juego.
    /// Principio Open/Closed: extensible sin modificar el código base.
    /// </summary>
    public sealed class GameBoard
    {
        public int Rows { get; }
        public int Cols { get; }
        public List<Line> Lines { get; }
        public List<Box> Boxes { get; }
        public Player Player1 { get; private set; }
        public Player Player2 { get; private set; }
        public Player CurrentPlayer { get; private set; }
        public bool IsGameOver { get; private set; }

        public void SetPlayer2(Player player)
        {
            Player2 = player;
            if (CurrentPlayer == Player2)
                CurrentPlayer = player;
        }

        public GameBoard(int rows, int cols, Player player1, Player player2)
        {
            if (rows < 2 || cols < 2)
                throw new ArgumentException("El tablero debe tener al menos 2x2 cuadros");

            Rows = rows;
            Cols = cols;
            Player1 = player1;
            Player2 = player2;
            CurrentPlayer = player1;
            IsGameOver = false;

            Lines = new List<Line>();
            Boxes = new List<Box>();

            InitializeBoard();
        }

        private void InitializeBoard()
        {
            // Crear forma de rombo (dos pirámides unidas)
            // Para un tablero de tamaño N, crear un rombo con puntos válidos
            
            // Determinar qué puntos son válidos en la forma de rombo
            HashSet<(int row, int col)> validPoints = GetDiamondPoints();

            // Crear líneas horizontales solo entre puntos válidos consecutivos
            for (int r = 0; r <= Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    // Verificar si ambos extremos de la línea horizontal son puntos válidos
                    if (validPoints.Contains((r, c)) && validPoints.Contains((r, c + 1)))
                    {
                        var line = new Line(r, c, isHorizontal: true);
                        
                        // Si es línea de borde, marcarla como ya ocupada (contorno del rombo)
                        if (IsRomboBorder(line))
                        {
                            line.Claim(null); // Marcar como ocupada sin dueño (es borde fijo)
                        }
                        
                        Lines.Add(line);
                    }
                }
            }

            // Crear líneas verticales solo entre puntos válidos consecutivos
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c <= Cols; c++)
                {
                    // Verificar si ambos extremos de la línea vertical son puntos válidos
                    if (validPoints.Contains((r, c)) && validPoints.Contains((r + 1, c)))
                    {
                        var line = new Line(r, c, isHorizontal: false);
                        
                        // Si es línea de borde, marcarla como ya ocupada (contorno del rombo)
                        if (IsRomboBorder(line))
                        {
                            line.Claim(null); // Marcar como ocupada sin dueño (es borde fijo)
                        }
                        
                        Lines.Add(line);
                    }
                }
            }

            // Crear cuadros solo dentro del rombo
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    var top = GetLine(r, c, true);
                    var bottom = GetLine(r + 1, c, true);
                    var left = GetLine(r, c, false);
                    var right = GetLine(r, c + 1, false);

                    // Solo crear el cuadro si las 4 líneas existen (están dentro del rombo)
                    if (top != null && bottom != null && left != null && right != null)
                    {
                        Boxes.Add(new Box(r, c, top, bottom, left, right));
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene los puntos que forman la figura de rombo (dos pirámides unidas).
        /// Rombo perfecto con las 4 puntas CERRADAS (cada punta forma un cuadrado).
        /// </summary>
        private HashSet<(int row, int col)> GetDiamondPoints()
        {
            var points = new HashSet<(int row, int col)>();
            
            // Rombo centrado con las 4 puntas cerradas:
            //          • •           Fila 0: columnas 4,5 (punta superior)
            //        • • • •         Fila 1: columnas 3,4,5,6
            //      • • • • • •       Fila 2: columnas 2,3,4,5,6,7
            //    • • • • • • • •     Fila 3: columnas 1,2,3,4,5,6,7,8
            //  • • • • • • • • • •   Fila 4: columnas 0,1,2,3,4,5,6,7,8,9 (centro)
            //    • • • • • • • •     Fila 5: columnas 1,2,3,4,5,6,7,8
            //      • • • • • •       Fila 6: columnas 2,3,4,5,6,7
            //        • • • •         Fila 7: columnas 3,4,5,6
            //          • •           Fila 8: columnas 4,5 (punta inferior)
            
            // Fila 0: punta superior (2 puntos)
            points.Add((0, 4));
            points.Add((0, 5));
            
            // Fila 1: 4 puntos
            points.Add((1, 3));
            points.Add((1, 4));
            points.Add((1, 5));
            points.Add((1, 6));
            
            // Fila 2: 6 puntos
            points.Add((2, 2));
            points.Add((2, 3));
            points.Add((2, 4));
            points.Add((2, 5));
            points.Add((2, 6));
            points.Add((2, 7));
            
            // Fila 3: 8 puntos
            points.Add((3, 1));
            points.Add((3, 2));
            points.Add((3, 3));
            points.Add((3, 4));
            points.Add((3, 5));
            points.Add((3, 6));
            points.Add((3, 7));
            points.Add((3, 8));
            
            // Fila 4: 10 puntos (centro más ancho)
            points.Add((4, 0));
            points.Add((4, 1));
            points.Add((4, 2));
            points.Add((4, 3));
            points.Add((4, 4));
            points.Add((4, 5));
            points.Add((4, 6));
            points.Add((4, 7));
            points.Add((4, 8));
            points.Add((4, 9));
            
            // Fila 5: 8 puntos
            points.Add((5, 1));
            points.Add((5, 2));
            points.Add((5, 3));
            points.Add((5, 4));
            points.Add((5, 5));
            points.Add((5, 6));
            points.Add((5, 7));
            points.Add((5, 8));
            
            // Fila 6: 6 puntos
            points.Add((6, 2));
            points.Add((6, 3));
            points.Add((6, 4));
            points.Add((6, 5));
            points.Add((6, 6));
            points.Add((6, 7));
            
            // Fila 7: 4 puntos
            points.Add((7, 3));
            points.Add((7, 4));
            points.Add((7, 5));
            points.Add((7, 6));
            
            // Fila 8: punta inferior (2 puntos)
            points.Add((8, 4));
            points.Add((8, 5));
            
            return points;
        }

        /// <summary>
        /// Verifica si una línea es parte del borde externo del rombo.
        /// </summary>
        private bool IsRomboBorder(Line line)
        {
            int centerCol = 4;
            int centerRow = 4;
            
            if (line.IsHorizontal)
            {
                int r = line.Row;
                int c = line.Col;
                
                // Punta superior (fila 0)
                if (r == 0)
                    return (c == centerCol - 1 || c == centerCol + 1);
                
                // Parte superior expandiendo (filas 1-4)
                if (r >= 1 && r <= centerRow)
                {
                    int startCol = centerCol - r + 1;
                    int width = r + 1 + r;
                    return (c == startCol - 1 || c == startCol + width - 1);
                }
                
                // Parte inferior contrayendo (filas 5-7)
                if (r >= 5 && r <= 7)
                {
                    int offset = r - centerRow;
                    int width = 6 - offset;
                    int startCol = centerCol - (3 - offset) + 1;
                    return (c == startCol - 1 || c == startCol + width - 1);
                }
                
                // Punta inferior (fila 8)
                if (r == 8)
                    return (c == centerCol - 1 || c == centerCol + 1);
            }
            else // Vertical
            {
                int r = line.Row;
                int c = line.Col;
                
                // Punta superior
                if (r == 0)
                    return (c == centerCol || c == centerCol + 1);
                
                // Bordes laterales parte superior (filas 1-4)
                if (r >= 1 && r <= centerRow)
                {
                    int startCol = centerCol - r + 1;
                    int width = r + 1 + r;
                    return (c == startCol || c == startCol + width);
                }
                
                // Bordes laterales parte inferior (filas 4-7)
                if (r >= centerRow && r <= 7)
                {
                    int offset = r - centerRow;
                    int width = 6 - offset;
                    int startCol = centerCol - (3 - offset) + 1;
                    return (c == startCol || c == startCol + width);
                }
                
                // Punta inferior
                if (r == 7)
                    return (c == centerCol || c == centerCol + 1);
            }
            
            return false;
        }

        /// <summary>
        /// Obtiene una línea específica del tablero.
        /// </summary>
        public Line? GetLine(int row, int col, bool isHorizontal)
        {
            return Lines.FirstOrDefault(l => l.Row == row && l.Col == col && l.IsHorizontal == isHorizontal);
        }

        /// <summary>
        /// Intenta marcar una línea para el jugador actual.
        /// Retorna true si se completó al menos un cuadro (y el jugador juega de nuevo).
        /// </summary>
        public bool TryMarkLine(Line line)
        {
            if (line == null || !line.IsAvailable || IsGameOver)
                return false;

            // Marcar la línea
            line.Claim(CurrentPlayer);

            // Verificar si se completaron cuadros
            bool boxCompleted = CheckAndClaimCompletedBoxes();

            // Si no se completó ningún cuadro, cambiar de turno
            if (!boxCompleted)
            {
                SwitchPlayer();
            }

            // Verificar si el juego terminó
            CheckGameOver();

            return boxCompleted;
        }

        /// <summary>
        /// Verifica y reclama cuadros que se completaron con la última jugada.
        /// </summary>
        private bool CheckAndClaimCompletedBoxes()
        {
            bool anyCompleted = false;

            foreach (var box in Boxes)
            {
                if (box.TryClaimFor(CurrentPlayer))
                {
                    CurrentPlayer.AddPoint();
                    anyCompleted = true;
                }
            }

            return anyCompleted;
        }

        /// <summary>
        /// Cambia el turno al otro jugador.
        /// </summary>
        private void SwitchPlayer()
        {
            CurrentPlayer = CurrentPlayer == Player1 ? Player2 : Player1;
        }

        /// <summary>
        /// Verifica si el juego ha terminado (todas las líneas marcadas).
        /// </summary>
        private void CheckGameOver()
        {
            IsGameOver = Lines.All(l => !l.IsAvailable);
        }

        /// <summary>
        /// Obtiene el ganador del juego (o null si es empate).
        /// </summary>
        public Player? GetWinner()
        {
            if (!IsGameOver)
                return null;

            if (Player1.Score > Player2.Score)
                return Player1;
            else if (Player2.Score > Player1.Score)
                return Player2;
            else
                return null; // Empate
        }

        /// <summary>
        /// Reinicia el juego.
        /// </summary>
        public void Reset()
        {
            Player1.ResetScore();
            Player2.ResetScore();
            CurrentPlayer = Player1;
            IsGameOver = false;
            Lines.Clear();
            Boxes.Clear();
            InitializeBoard();
        }

        /// <summary>
        /// Obtiene todas las líneas disponibles.
        /// </summary>
        public List<Line> GetAvailableLines()
        {
            return Lines.Where(l => l.IsAvailable).ToList();
        }

        /// <summary>
        /// Obtiene los cuadros que tienen exactamente N lados marcados.
        /// </summary>
        public List<Box> GetBoxesWithSides(int sideCount)
        {
            return Boxes.Where(b => b.SidesMarked == sideCount && b.Owner == null).ToList();
        }

        /// <summary>
        /// Obtiene todos los puntos válidos en la forma de rombo.
        /// </summary>
        public HashSet<(int row, int col)> GetValidPoints()
        {
            return GetDiamondPoints();
        }
    }
}
