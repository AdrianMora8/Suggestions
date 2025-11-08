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
            // Crear todas las líneas horizontales
            for (int r = 0; r <= Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    Lines.Add(new Line(r, c, isHorizontal: true));
                }
            }

            // Crear todas las líneas verticales
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c <= Cols; c++)
                {
                    Lines.Add(new Line(r, c, isHorizontal: false));
                }
            }

            // Crear todos los cuadros y vincularlos con sus líneas
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    var top = GetLine(r, c, true);
                    var bottom = GetLine(r + 1, c, true);
                    var left = GetLine(r, c, false);
                    var right = GetLine(r, c + 1, false);

                    if (top != null && bottom != null && left != null && right != null)
                    {
                        Boxes.Add(new Box(r, c, top, bottom, left, right));
                    }
                }
            }
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
    }
}
