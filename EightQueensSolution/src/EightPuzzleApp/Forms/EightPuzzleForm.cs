using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using EightPuzzleApp.Models;
using EightPuzzleApp.Solvers;

namespace EightPuzzleApp.Forms
{
    public sealed class EightPuzzleForm : Form
    {
        private const int SizeN = PuzzleState.Size; // 3
        private readonly Button[,] _tiles = new Button[SizeN, SizeN];
        private readonly TableLayoutPanel _grid = new TableLayoutPanel();
        private readonly Panel _rightPanel = new Panel();
        private readonly Button _btnShuffle = new Button();
        private readonly Button _btnReset = new Button();
        private readonly Button _btnSolve = new Button();
        private readonly Label _lblMoves = new Label();
        private readonly Label _lblTitle = new Label();
        private readonly Label _lblStatus = new Label();
    private readonly System.Windows.Forms.Timer _animTimer = new System.Windows.Forms.Timer();

        private PuzzleState _initial = new PuzzleState(new byte[] {1,2,3,4,5,6,7,8,0});
        private PuzzleState _current = new PuzzleState(new byte[] {1,2,3,4,5,6,7,8,0});
        private int _moveCount = 0;

        private readonly IPuzzleSolver _solver = new AStarSolver();
        private IList<PuzzleState>? _solutionPath;
        private int _solutionIndex = 0;

        public EightPuzzleForm()
        {
            Text = "8-Puzzle";
            MinimumSize = new Size(720, 520);
            StartPosition = FormStartPosition.CenterScreen;

            // Layout root: grid left, panel right
            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
            };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
            Controls.Add(root);

            // Grid setup
            _grid.Dock = DockStyle.Fill;
            _grid.ColumnCount = SizeN;
            _grid.RowCount = SizeN;
            for (int i = 0; i < SizeN; i++)
            {
                _grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / SizeN));
                _grid.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / SizeN));
            }
            root.Controls.Add(_grid, 0, 0);

            // Right panel controls
            _rightPanel.Dock = DockStyle.Fill;
            root.Controls.Add(_rightPanel, 1, 0);

            _lblTitle.Text = "8-Puzzle (rompecabezas 3x3)";
            _lblTitle.Font = new Font(Font, FontStyle.Bold);
            _lblTitle.AutoSize = true;
            _lblTitle.Location = new Point(16, 16);
            _rightPanel.Controls.Add(_lblTitle);

            _btnShuffle.Text = "Barajar";
            _btnShuffle.Size = new Size(150, 36);
            _btnShuffle.Location = new Point(16, 60);
            _btnShuffle.Click += (_, __) => Shuffle(30);
            _rightPanel.Controls.Add(_btnShuffle);

            _btnReset.Text = "Reiniciar";
            _btnReset.Size = new Size(150, 36);
            _btnReset.Location = new Point(16, 106);
            _btnReset.Click += (_, __) => Reset();
            _rightPanel.Controls.Add(_btnReset);

            _btnSolve.Text = "Resolver (A*)";
            _btnSolve.Size = new Size(150, 36);
            _btnSolve.Location = new Point(16, 152);
            _btnSolve.Click += async (_, __) => await SolveAndAnimateAsync();
            _rightPanel.Controls.Add(_btnSolve);

            _lblMoves.Text = "Movimientos: 0";
            _lblMoves.AutoSize = true;
            _lblMoves.Location = new Point(16, 210);
            _rightPanel.Controls.Add(_lblMoves);

            _lblStatus.Text = "Listo";
            _lblStatus.AutoSize = true;
            _lblStatus.MaximumSize = new Size(240, 0);
            _lblStatus.Location = new Point(16, 240);
            _rightPanel.Controls.Add(_lblStatus);

            // Animation timer
            _animTimer.Interval = 300;
            _animTimer.Tick += AnimTimer_Tick;

            CreateGridButtons();
            RenderBoard();
        }

        private void CreateGridButtons()
        {
            _grid.SuspendLayout();
            _grid.Controls.Clear();
            for (int r = 0; r < SizeN; r++)
            {
                for (int c = 0; c < SizeN; c++)
                {
                    var btn = new Button
                    {
                        Dock = DockStyle.Fill,
                        Margin = new Padding(6),
                        Font = new Font("Segoe UI", 24f, FontStyle.Bold, GraphicsUnit.Point),
                        BackColor = Color.WhiteSmoke,
                        Tag = (r, c)
                    };
                    btn.Click += Tile_Click;
                    _tiles[r, c] = btn;
                    _grid.Controls.Add(btn, c, r);
                }
            }
            _grid.ResumeLayout();
        }

        private void Tile_Click(object? sender, EventArgs e)
        {
            if (sender is not Button btn) return;
            var (r, c) = ((int row, int col))btn.Tag!;
            TryMove(r, c);
        }

        private void TryMove(int r, int c)
        {
            // Find blank position
            int bi = _current.BlankIndex;
            int br = bi / SizeN, bc = bi % SizeN;
            // Is clicked tile adjacent to blank?
            if (Math.Abs(br - r) + Math.Abs(bc - c) == 1)
            {
                // Determine action from blank to tile so that tile slides into blank
                string action;
                if (r == br - 1 && c == bc) action = "Up";       // blank moves up -> tile moves down
                else if (r == br + 1 && c == bc) action = "Down"; // blank moves down -> tile moves up
                else if (c == bc - 1 && r == br) action = "Left"; // blank left -> tile right
                else if (c == bc + 1 && r == br) action = "Right";// blank right -> tile left
                else return;

                _current = _current.Move(action);
                _moveCount++;
                _lblMoves.Text = $"Movimientos: {_moveCount}";
                RenderBoard();
                if (_current.IsGoal())
                {
                    _lblStatus.Text = "¡Felicidades! Resuelto.";
                    System.Media.SystemSounds.Asterisk.Play();
                }
            }
            else
            {
                System.Media.SystemSounds.Beep.Play();
            }
        }

        private void RenderBoard()
        {
            var cells = _current.Cells;
            for (int i = 0; i < PuzzleState.Count; i++)
            {
                int r = i / SizeN, c = i % SizeN;
                var btn = _tiles[r, c];
                int val = cells[i];
                btn.Text = val == 0 ? string.Empty : val.ToString();
                btn.Enabled = val != 0; // do not click the blank
                btn.BackColor = val == 0 ? Color.Gainsboro : Color.White;
            }
        }

        private void Shuffle(int steps)
        {
            StopAnimation();
            var rnd = new Random();
            var s = new PuzzleState(new byte[] {1,2,3,4,5,6,7,8,0});
            for (int i = 0; i < steps; i++)
            {
                var neigh = s.GetNeighbors().ToList();
                var pick = neigh[rnd.Next(neigh.Count)];
                s = pick.Next;
            }
            _initial = s.Clone();
            _current = s.Clone();
            _moveCount = 0;
            _lblMoves.Text = "Movimientos: 0";
            _lblStatus.Text = "Barajado";
            RenderBoard();
        }

        private void Reset()
        {
            StopAnimation();
            _current = _initial.Clone();
            _moveCount = 0;
            _lblMoves.Text = "Movimientos: 0";
            _lblStatus.Text = "Reiniciado";
            RenderBoard();
        }

        private async Task SolveAndAnimateAsync()
        {
            if (_current.IsGoal()) { _lblStatus.Text = "Ya está resuelto"; return; }
            StopAnimation();
            SetControlsEnabled(false);
            _lblStatus.Text = "Resolviendo con A*...";
            try
            {
                var start = _current.Clone();
                var path = await Task.Run(() => _solver.Solve(start));
                if (path == null || path.Count == 0)
                {
                    _lblStatus.Text = "No se encontró solución";
                    System.Media.SystemSounds.Hand.Play();
                }
                else
                {
                    _solutionPath = path;
                    _solutionIndex = 0;
                    _lblStatus.Text = $"Solución: {path.Count - 1} pasos";
                    _animTimer.Start();
                }
            }
            catch (Exception ex)
            {
                _lblStatus.Text = "Error: " + ex.Message;
            }
            finally
            {
                if (_solutionPath == null) SetControlsEnabled(true);
            }
        }

        private void AnimTimer_Tick(object? sender, EventArgs e)
        {
            if (_solutionPath == null) { StopAnimation(); return; }
            if (_solutionIndex >= _solutionPath.Count)
            {
                StopAnimation();
                return;
            }
            _current = _solutionPath[_solutionIndex++].Clone();
            RenderBoard();
            if (_solutionIndex >= _solutionPath.Count)
            {
                StopAnimation();
                _lblStatus.Text = "Animación finalizada";
                SetControlsEnabled(true);
            }
        }

        private void StopAnimation()
        {
            _animTimer.Stop();
            _solutionPath = null;
            _solutionIndex = 0;
        }

        private void SetControlsEnabled(bool enabled)
        {
            _btnShuffle.Enabled = enabled;
            _btnReset.Enabled = enabled;
            _btnSolve.Enabled = enabled;
        }
    }
}
