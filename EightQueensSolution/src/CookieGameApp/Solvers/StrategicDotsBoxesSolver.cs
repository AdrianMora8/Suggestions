using System;
using System.Collections.Generic;
using System.Linq;
using CookieGameApp.Models;

namespace CookieGameApp.Solvers
{
    /// <summary>
    /// Solver estratégico para Dots and Boxes usando heurísticas avanzadas.
    /// Implementa una estrategia basada en:
    /// 1. Completar cuadros si es posible (ganar puntos inmediatos)
    /// 2. Evitar dar cuadros al oponente (no marcar la 3ra línea de un cuadro)
    /// 3. Control de cadenas largas
    /// 4. Evaluación Minimax simplificada
    /// </summary>
    public sealed class StrategicDotsBoxesSolver : IDotsBoxesSolver
    {
        private const int MAX_DEPTH = 3; // Profundidad de búsqueda Minimax

        public Line? SuggestBestMove(GameBoard board)
        {
            if (board == null || board.IsGameOver)
                return null;

            var availableLines = board.GetAvailableLines();
            if (!availableLines.Any())
                return null;

            // Estrategia 1: Si puedo completar un cuadro, hazlo
            var completingMoves = FindBoxCompletingMoves(board);
            if (completingMoves.Any())
            {
                return completingMoves.First(); // Tomar el primero
            }

            // Estrategia 2: Evitar dar cuadros gratis al oponente
            var safeMoves = FindSafeMoves(board, availableLines);
            
            // Si hay movimientos seguros, elegir el mejor usando evaluación
            if (safeMoves.Any())
            {
                return safeMoves.OrderByDescending(l => EvaluateMove(board, l)).First();
            }

            // Estrategia 3: Si no hay movimientos seguros, minimizar el daño
            // Elegir la línea que complete menos cuadros para el oponente
            return availableLines.OrderBy(l => CountBoxesCompletedByLine(board, l))
                                 .ThenByDescending(l => EvaluateMove(board, l))
                                 .First();
        }

        public int EvaluateMove(GameBoard board, Line line)
        {
            if (!line.IsAvailable)
                return int.MinValue;

            int score = 0;

            // Factor 1: Completar cuadros es muy bueno (+100 por cuadro)
            int boxesCompleted = CountBoxesCompletedByLine(board, line);
            score += boxesCompleted * 100;

            // Factor 2: Evitar crear oportunidades para el oponente (-50 por cuadro con 3 lados)
            int dangerousBoxes = CountBoxesWithThreeSidesAfterMove(board, line);
            score -= dangerousBoxes * 50;

            // Factor 3: Preferir movimientos en el centro del tablero (control territorial)
            int centerDistance = Math.Abs(line.Row - board.Rows / 2) + Math.Abs(line.Col - board.Cols / 2);
            score += (board.Rows + board.Cols - centerDistance) * 2;

            // Factor 4: Preferir líneas que no avanzan mucho el juego al inicio
            int totalBoxes = board.Boxes.Count;
            int claimedBoxes = board.Boxes.Count(b => b.Owner != null);
            double gameProgress = (double)claimedBoxes / totalBoxes;
            
            if (gameProgress < 0.3) // Fase temprana
            {
                // Preferir movimientos conservadores
                if (dangerousBoxes == 0)
                    score += 20;
            }

            return score;
        }

        /// <summary>
        /// Encuentra todas las líneas que completarían un cuadro inmediatamente.
        /// </summary>
        private List<Line> FindBoxCompletingMoves(GameBoard board)
        {
            var completingMoves = new List<Line>();
            var boxesWithThreeSides = board.GetBoxesWithSides(3);

            foreach (var box in boxesWithThreeSides)
            {
                // Encontrar cuál es la línea faltante
                if (box.TopLine.IsAvailable)
                    completingMoves.Add(box.TopLine);
                if (box.BottomLine.IsAvailable)
                    completingMoves.Add(box.BottomLine);
                if (box.LeftLine.IsAvailable)
                    completingMoves.Add(box.LeftLine);
                if (box.RightLine.IsAvailable)
                    completingMoves.Add(box.RightLine);
            }

            return completingMoves.Distinct().ToList();
        }

        /// <summary>
        /// Encuentra movimientos "seguros" que no dan cuadros al oponente.
        /// </summary>
        private List<Line> FindSafeMoves(GameBoard board, List<Line> availableLines)
        {
            var safeMoves = new List<Line>();

            foreach (var line in availableLines)
            {
                // Un movimiento es seguro si no crea un cuadro con 3 lados
                if (CountBoxesWithThreeSidesAfterMove(board, line) == 0)
                {
                    safeMoves.Add(line);
                }
            }

            return safeMoves;
        }

        /// <summary>
        /// Cuenta cuántos cuadros se completarían al marcar esta línea.
        /// </summary>
        private int CountBoxesCompletedByLine(GameBoard board, Line line)
        {
            int count = 0;

            foreach (var box in board.Boxes)
            {
                if (box.Owner != null) continue;

                // Verificar si esta línea es parte del cuadro y si completaría el cuadro
                bool isPartOfBox = box.TopLine.Equals(line) || box.BottomLine.Equals(line) ||
                                   box.LeftLine.Equals(line) || box.RightLine.Equals(line);

                if (isPartOfBox && box.SidesMarked == 3)
                {
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Cuenta cuántos cuadros quedarían con 3 lados (listos para cerrar) después de marcar esta línea.
        /// </summary>
        private int CountBoxesWithThreeSidesAfterMove(GameBoard board, Line line)
        {
            int count = 0;

            foreach (var box in board.Boxes)
            {
                if (box.Owner != null) continue;

                // Verificar si esta línea es parte del cuadro
                bool isPartOfBox = box.TopLine.Equals(line) || box.BottomLine.Equals(line) ||
                                   box.LeftLine.Equals(line) || box.RightLine.Equals(line);

                // Si marca esta línea, el cuadro tendría 3 lados (peligroso para el oponente)
                if (isPartOfBox && box.SidesMarked == 2)
                {
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Implementación simplificada de Minimax para evaluación más profunda.
        /// </summary>
        private int Minimax(GameBoard board, int depth, bool isMaximizing, Player originalPlayer)
        {
            // Caso base: profundidad máxima o juego terminado
            if (depth == 0 || board.IsGameOver)
            {
                return EvaluatePosition(board, originalPlayer);
            }

            var availableLines = board.GetAvailableLines();
            if (!availableLines.Any())
                return EvaluatePosition(board, originalPlayer);

            if (isMaximizing)
            {
                int maxEval = int.MinValue;
                foreach (var line in availableLines.Take(5)) // Limitar ramificación
                {
                    int eval = EvaluateMove(board, line);
                    maxEval = Math.Max(maxEval, eval);
                }
                return maxEval;
            }
            else
            {
                int minEval = int.MaxValue;
                foreach (var line in availableLines.Take(5))
                {
                    int eval = EvaluateMove(board, line);
                    minEval = Math.Min(minEval, eval);
                }
                return minEval;
            }
        }

        /// <summary>
        /// Evalúa la posición actual del tablero desde la perspectiva de un jugador.
        /// </summary>
        private int EvaluatePosition(GameBoard board, Player player)
        {
            var opponent = player == board.Player1 ? board.Player2 : board.Player1;
            return player.Score * 100 - opponent.Score * 100;
        }
    }
}
