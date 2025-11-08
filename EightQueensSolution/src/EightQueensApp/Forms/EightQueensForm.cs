using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EightQueensApp.Models;
using EightQueensApp.Solvers;

namespace EightQueensApp.Forms
{
    public sealed class EightQueensForm : Form
    {
        private const int DefaultN = 8;
        private readonly Panel _boardPanel = new Panel();
        private readonly NumericUpDown _numN = new NumericUpDown();
        private readonly Button _btnSolve = new Button();
        private readonly Button _btnPrev = new Button();
        private readonly Button _btnNext = new Button();
        private readonly Button _btnRandom = new Button();
        private readonly Button _btnReset = new Button();
        private readonly Label _lblStatus = new Label();

        private readonly IQueenSolver _solver = new BacktrackingSolver();
        private IReadOnlyList<Board>? _solutions;
        private int _solutionIndex = -1;
        private int _n = DefaultN;

        public EightQueensForm()
        {
            Text = "N-Reinas (8-Queens)";
            MinimumSize = new Size(800, 600);
            StartPosition = FormStartPosition.CenterScreen;

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1
            };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            Controls.Add(root);

            // Board panel
            _boardPanel.Dock = DockStyle.Fill;
            _boardPanel.BackColor = Color.White;
            _boardPanel.Paint += BoardPanel_Paint;
            _boardPanel.Resize += (_, __) => _boardPanel.Invalidate();
            root.Controls.Add(_boardPanel, 0, 0);

            // Right panel
            var right = new Panel { Dock = DockStyle.Fill };
            root.Controls.Add(right, 1, 0);

            var y = 16;
            var marginX = 16;

            var lblN = new Label { Text = "Tamaño N:", AutoSize = true, Location = new Point(marginX, y) };
            right.Controls.Add(lblN);

            _numN.Minimum = 4;
            _numN.Maximum = 14;
            _numN.Value = DefaultN;
            _numN.Location = new Point(marginX + 100, y - 2);
            _numN.Width = 80;
            _numN.ValueChanged += (_, __) => { _n = (int)_numN.Value; ClearSolutions(); };
            right.Controls.Add(_numN);

            y += 40;
            _btnSolve.Text = "Resolver";
            _btnSolve.Size = new Size(180, 36);
            _btnSolve.Location = new Point(marginX, y);
            _btnSolve.Click += (_, __) => Solve();
            right.Controls.Add(_btnSolve);

            y += 46;
            _btnPrev.Text = "Anterior";
            _btnPrev.Size = new Size(85, 36);
            _btnPrev.Location = new Point(marginX, y);
            _btnPrev.Click += (_, __) => ShowRelative(-1);
            right.Controls.Add(_btnPrev);

            _btnNext.Text = "Siguiente";
            _btnNext.Size = new Size(85, 36);
            _btnNext.Location = new Point(marginX + 95, y);
            _btnNext.Click += (_, __) => ShowRelative(+1);
            right.Controls.Add(_btnNext);

            y += 46;
            _btnRandom.Text = "Aleatoria";
            _btnRandom.Size = new Size(180, 36);
            _btnRandom.Location = new Point(marginX, y);
            _btnRandom.Click += (_, __) => ShowRandom();
            right.Controls.Add(_btnRandom);

            y += 46;
            _btnReset.Text = "Limpiar";
            _btnReset.Size = new Size(180, 36);
            _btnReset.Location = new Point(marginX, y);
            _btnReset.Click += (_, __) => { ClearSolutions(); _boardPanel.Invalidate(); };
            right.Controls.Add(_btnReset);

            y += 50;
            _lblStatus.Text = "Listo";
            _lblStatus.AutoSize = true;
            _lblStatus.MaximumSize = new Size(240, 0);
            _lblStatus.Location = new Point(marginX, y);
            right.Controls.Add(_lblStatus);
        }

        private void Solve()
        {
            ToggleControls(false);
            _lblStatus.Text = "Resolviendo...";
            try
            {
                _solutions = _solver.Solve(_n);
                if (_solutions == null || _solutions.Count == 0)
                {
                    _solutionIndex = -1;
                    _lblStatus.Text = $"N={_n}: sin soluciones";
                }
                else
                {
                    _solutionIndex = 0;
                    _lblStatus.Text = $"N={_n}: {_solutions.Count} soluciones. Mostrando 1/{_solutions.Count}";
                }
            }
            catch (Exception ex)
            {
                _solutions = null;
                _solutionIndex = -1;
                _lblStatus.Text = "Error: " + ex.Message;
            }
            finally
            {
                ToggleControls(true);
                _boardPanel.Invalidate();
            }
        }

        private void ShowRelative(int delta)
        {
            if (_solutions == null || _solutions.Count == 0) return;
            _solutionIndex = (_solutionIndex + delta) % _solutions.Count;
            if (_solutionIndex < 0) _solutionIndex += _solutions.Count;
            _lblStatus.Text = $"N={_n}: {_solutions.Count} soluciones. Mostrando {_solutionIndex + 1}/{_solutions.Count}";
            _boardPanel.Invalidate();
        }

        private void ShowRandom()
        {
            if (_solutions == null || _solutions.Count == 0) return;
            var rnd = new Random();
            _solutionIndex = rnd.Next(_solutions.Count);
            _lblStatus.Text = $"N={_n}: {_solutions.Count} soluciones. Mostrando {_solutionIndex + 1}/{_solutions.Count}";
            _boardPanel.Invalidate();
        }

        private void ClearSolutions()
        {
            _solutions = null;
            _solutionIndex = -1;
            _lblStatus.Text = "Listo";
        }

        private void BoardPanel_Paint(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(Color.White);

            int n = _n;
            int size = Math.Min(_boardPanel.ClientSize.Width, _boardPanel.ClientSize.Height) - 20;
            size = Math.Max(size, 100);
            int x0 = (_boardPanel.ClientSize.Width - size) / 2;
            int y0 = (_boardPanel.ClientSize.Height - size) / 2;
            float cell = size / (float)n;

            // Draw chessboard
            for (int r = 0; r < n; r++)
            {
                for (int c = 0; c < n; c++)
                {
                    var rect = new RectangleF(x0 + c * cell, y0 + r * cell, cell, cell);
                    bool dark = ((r + c) % 2 == 1);
                    g.FillRectangle(dark ? Brushes.DarkSlateGray : Brushes.Beige, rect);
                }
            }
            g.DrawRectangle(Pens.Black, x0, y0, size, size);

            // Draw queens (if any)
            Board? board = (_solutions != null && _solutionIndex >= 0 && _solutionIndex < _solutions.Count)
                ? _solutions[_solutionIndex]
                : null;

            if (board != null)
            {
                var queens = board.Queens; // index=row, value=col
                using var queenFont = new Font("Segoe UI Symbol", Math.Max(12, (int)(cell * 0.6f)), FontStyle.Bold);
                for (int r = 0; r < n; r++)
                {
                    int col = queens[r];
                    if (col < 0) continue;
                    float cx = x0 + col * cell + cell / 2f;
                    float cy = y0 + r * cell + cell / 2f;
                    var text = "♛"; // Unicode black queen
                    var sz = g.MeasureString(text, queenFont);
                    g.DrawString(text, queenFont, Brushes.Black, cx - sz.Width / 2f, cy - sz.Height / 2f);
                }
            }
        }

        private void ToggleControls(bool enabled)
        {
            _btnSolve.Enabled = enabled;
            _btnPrev.Enabled = enabled;
            _btnNext.Enabled = enabled;
            _btnRandom.Enabled = enabled;
            _btnReset.Enabled = enabled;
            _numN.Enabled = enabled;
        }
    }
}
