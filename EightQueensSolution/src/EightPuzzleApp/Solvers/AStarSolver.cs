using System;
using System.Collections.Generic;
using System.Linq;
using EightPuzzleApp.Models;

namespace EightPuzzleApp.Solvers
{
    /// <summary>
    /// Implementación de A* para el 8-puzzle usando heurística Manhattan.
    /// </summary>
    public sealed class AStarSolver : IPuzzleSolver
    {
        public IList<PuzzleState> Solve(PuzzleState start)
        {
            if (start == null) throw new ArgumentNullException(nameof(start));
            if (start.IsGoal()) return new List<PuzzleState> { start.Clone() };

            var goal = new PuzzleState(new byte[] {1,2,3,4,5,6,7,8,0});

            var open = new PriorityQueue<PuzzleState, int>();
            var gScore = new Dictionary<string, int>();
            var fScore = new Dictionary<string, int>();
            var cameFrom = new Dictionary<string, (string parentKey, string action)>();

            string startKey = start.Key();
            gScore[startKey] = 0;
            fScore[startKey] = Heuristics.Manhattan(start);
            open.Enqueue(start, fScore[startKey]);

            var visited = new HashSet<string>();

            while (open.Count > 0)
            {
                open.TryDequeue(out var current, out _);
                var curKey = current.Key();
                if (visited.Contains(curKey)) continue;
                visited.Add(curKey);

                if (current.IsGoal())
                {
                    return ReconstructPath(cameFrom, curKey, startKey);
                }

                foreach (var (neighbor, action) in current.GetNeighbors())
                {
                    var nk = neighbor.Key();
                    int tentativeG = gScore[curKey] + 1;
                    if (!gScore.TryGetValue(nk, out var existingG) || tentativeG < existingG)
                    {
                        cameFrom[nk] = (curKey, action);
                        gScore[nk] = tentativeG;
                        int h = Heuristics.Manhattan(neighbor);
                        fScore[nk] = tentativeG + h;
                        open.Enqueue(neighbor, fScore[nk]);
                    }
                }
            }

            return null; // no encontrado
        }

        private IList<PuzzleState> ReconstructPath(Dictionary<string, (string parentKey, string action)> cameFrom, string curKey, string startKey)
        {
            var path = new List<PuzzleState>();
            string key = curKey;
            // we need a mapping from key->state for final path; we will reconstruct states from keys
            // For simplicity, reconstruct by parsing the key (csv)
            while (true)
            {
                var state = KeyToState(key);
                path.Add(state);
                if (key == startKey) break;
                if (!cameFrom.TryGetValue(key, out var parent)) break;
                key = parent.parentKey;
            }
            path.Reverse();
            return path;
        }

        private PuzzleState KeyToState(string key)
        {
            var parts = key.Split(',', StringSplitOptions.RemoveEmptyEntries);
            var bytes = parts.Select(s => byte.Parse(s)).ToArray();
            return new PuzzleState(bytes);
        }
    }
}
