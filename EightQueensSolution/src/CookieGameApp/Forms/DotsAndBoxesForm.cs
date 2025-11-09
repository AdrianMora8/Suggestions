using System;
using System.Drawing;
using System.Windows.Forms;
using CookieGameApp.Models;
using CookieGameApp.Solvers;

namespace CookieGameApp.Forms
{
    /// <summary>
    /// Formulario principal del juego Dots and Boxes.
    /// Dibuja el tablero con puntos y líneas, gestiona la interacción del usuario.
    /// </summary>
    public class DotsAndBoxesForm : Form
    {
        private GameBoard _board;
        private IDotsBoxesSolver _solver;
        
        // Constantes de dibujo
        private const int DOT_SIZE = 8;
        private const int CELL_SIZE = 60;
        private const int MARGIN = 80;
        private const int LINE_THICKNESS = 4;
        private const int HIGHLIGHT_THICKNESS = 6;
        
        // UI Controls
        private Panel _gamePanel;
        private Label _lblPlayer1Score;
        private Label _lblPlayer2Score;
        private Label _lblCurrentTurn;
        private Label _lblGameStatus;
        private Button _btnNewGame;
        private Button _btnAISuggestion;
        private Button _btnAutoPlay;
        private CheckBox _chkPlayer2AI;
        private Line? _hoveredLine;
        private Line? _suggestedLine;

        public DotsAndBoxesForm()
        {
            InitializeGame();
            InitializeUI();
        }

        private void InitializeGame()
        {
            var player1 = new Player("Jugador 1", Color.Red, isAI: false);
            var player2 = new Player("Jugador 2", Color.Blue, isAI: false);
            
            _board = new GameBoard(9, 9, player1, player2); // Tablero 9x9 para rombo perfecto con 4 puntas cerradas
            _solver = new StrategicDotsBoxesSolver();
        }

        private void InitializeUI()
        {
            Text = "Juego de la Galleta";
            Size = new Size(900, 750);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            DoubleBuffered = true;

            // Panel principal del juego (donde se dibuja el tablero)
            _gamePanel = new Panel
            {
                Location = new Point(MARGIN - 40, MARGIN - 40),
                Size = new Size(CELL_SIZE * _board.Cols + 80, CELL_SIZE * _board.Rows + 80),
                BackColor = Color.White
            };
            _gamePanel.Paint += GamePanel_Paint;
            _gamePanel.MouseMove += GamePanel_MouseMove;
            _gamePanel.MouseClick += GamePanel_MouseClick;
            _gamePanel.MouseLeave += (s, e) => { _hoveredLine = null; _gamePanel.Invalidate(); };
            Controls.Add(_gamePanel);

            // Panel de información (derecha)
            int infoPanelX = _gamePanel.Right + 20;

            // Título
            var lblTitle = new Label
            {
                Text = "� JUEGO DE LA GALLETA",
                Location = new Point(infoPanelX, 20),
                Size = new Size(250, 35),
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64)
            };
            Controls.Add(lblTitle);

            // Información de jugadores
            var lblPlayer1Label = new Label
            {
                Text = "● Jugador 1 (Rojo)",
                Location = new Point(infoPanelX, 70),
                Size = new Size(200, 25),
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = Color.Red
            };
            Controls.Add(lblPlayer1Label);

            _lblPlayer1Score = new Label
            {
                Text = "Cuadros: 0",
                Location = new Point(infoPanelX + 15, 95),
                Size = new Size(200, 20),
                Font = new Font("Arial", 10)
            };
            Controls.Add(_lblPlayer1Score);

            var lblPlayer2Label = new Label
            {
                Text = "● Jugador 2 (Azul)",
                Location = new Point(infoPanelX, 130),
                Size = new Size(200, 25),
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = Color.Blue
            };
            Controls.Add(lblPlayer2Label);

            _lblPlayer2Score = new Label
            {
                Text = "Cuadros: 0",
                Location = new Point(infoPanelX + 15, 155),
                Size = new Size(200, 20),
                Font = new Font("Arial", 10)
            };
            Controls.Add(_lblPlayer2Score);

            // Turno actual
            _lblCurrentTurn = new Label
            {
                Location = new Point(infoPanelX, 195),
                Size = new Size(250, 60),
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = Color.Red,
                Text = "Turno: Jugador 1",
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(255, 240, 240),
                TextAlign = ContentAlignment.MiddleCenter
            };
            Controls.Add(_lblCurrentTurn);

            // Estado del juego
            _lblGameStatus = new Label
            {
                Location = new Point(infoPanelX, 270),
                Size = new Size(250, 50),
                Font = new Font("Arial", 9),
                Text = "Haz clic en una línea\npara marcarla",
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(100, 100, 100)
            };
            Controls.Add(_lblGameStatus);

            // Opciones de IA
            var grpAI = new GroupBox
            {
                Text = "Asistencia de IA",
                Location = new Point(infoPanelX, 340),
                Size = new Size(250, 150),
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            Controls.Add(grpAI);

            _chkPlayer2AI = new CheckBox
            {
                Text = "Jugador 2 es IA",
                Location = new Point(15, 25),
                Size = new Size(220, 25),
                Font = new Font("Arial", 9)
            };
            _chkPlayer2AI.CheckedChanged += ChkPlayer2AI_CheckedChanged;
            grpAI.Controls.Add(_chkPlayer2AI);

            _btnAISuggestion = new Button
            {
                Text = "💡 Sugerir Jugada",
                Location = new Point(15, 60),
                Size = new Size(220, 35),
                BackColor = Color.LightGreen,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            _btnAISuggestion.FlatAppearance.BorderColor = Color.Green;
            _btnAISuggestion.Click += BtnAISuggestion_Click;
            grpAI.Controls.Add(_btnAISuggestion);

            _btnAutoPlay = new Button
            {
                Text = "🤖 IA Juega Turno",
                Location = new Point(15, 105),
                Size = new Size(220, 35),
                BackColor = Color.LightBlue,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 9, FontStyle.Bold),
                Enabled = false
            };
            _btnAutoPlay.FlatAppearance.BorderColor = Color.Blue;
            _btnAutoPlay.Click += BtnAutoPlay_Click;
            grpAI.Controls.Add(_btnAutoPlay);

            // Botón nuevo juego
            _btnNewGame = new Button
            {
                Text = "🔄 Nuevo Juego",
                Location = new Point(infoPanelX, 510),
                Size = new Size(250, 45),
                BackColor = Color.Orange,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = Color.White
            };
            _btnNewGame.FlatAppearance.BorderColor = Color.DarkOrange;
            _btnNewGame.Click += BtnNewGame_Click;
            Controls.Add(_btnNewGame);

            // Instrucciones
            var lblInstructions = new Label
            {
                Text = "CÓMO JUGAR:\n\n" +
                       "• Haz clic en las líneas\n" +
                       "  entre los puntos del\n" +
                       "  rombo (galleta)\n\n" +
                       "• Completa cuadros para\n" +
                       "  ganar puntos\n\n" +
                       "• Si cierras un cuadro,\n" +
                       "  juegas de nuevo\n\n" +
                       "• Gana quien tenga más\n" +
                       "  cuadros al final",
                Location = new Point(infoPanelX, 575),
                Size = new Size(250, 150),
                Font = new Font("Arial", 8),
                ForeColor = Color.FromArgb(120, 120, 120)
            };
            Controls.Add(lblInstructions);

            UpdateScoreDisplay();
        }

        private void GamePanel_Paint(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Dibujar líneas del tablero
            DrawLines(g);

            // Dibujar cuadros completados
            DrawCompletedBoxes(g);

            // Dibujar puntos
            DrawDots(g);

            // Dibujar línea sugerida por la IA
            if (_suggestedLine != null && _suggestedLine.IsAvailable)
            {
                DrawLine(g, _suggestedLine, Color.Gold, HIGHLIGHT_THICKNESS + 2, dashed: true);
            }

            // Dibujar línea bajo el cursor
            if (_hoveredLine != null && _hoveredLine.IsAvailable)
            {
                DrawLine(g, _hoveredLine, Color.FromArgb(100, 100, 100), HIGHLIGHT_THICKNESS, dashed: true);
            }
        }

        private void DrawDots(Graphics g)
        {
            using (var brush = new SolidBrush(Color.Black))
            {
                // Obtener solo los puntos válidos del rombo
                var validPoints = _board.GetValidPoints();
                
                foreach (var (row, col) in validPoints)
                {
                    int x = MARGIN + col * CELL_SIZE - DOT_SIZE / 2;
                    int y = MARGIN + row * CELL_SIZE - DOT_SIZE / 2;
                    g.FillEllipse(brush, x, y, DOT_SIZE, DOT_SIZE);
                }
            }
        }

        private void DrawLines(Graphics g)
        {
            foreach (var line in _board.Lines)
            {
                if (!line.IsAvailable)
                {
                    // Si la línea no tiene dueño, es parte del contorno del rombo (dibujar en negro)
                    // Si tiene dueño, usar el color del jugador
                    var color = line.Owner?.Color ?? Color.Black;
                    var thickness = line.Owner == null ? LINE_THICKNESS + 1 : LINE_THICKNESS;
                    DrawLine(g, line, color, thickness);
                }
            }
        }

        private void DrawLine(Graphics g, Line line, Color color, int thickness, bool dashed = false)
        {
            using (var pen = new Pen(color, thickness))
            {
                if (dashed)
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                }

                int x1, y1, x2, y2;

                if (line.IsHorizontal)
                {
                    x1 = MARGIN + line.Col * CELL_SIZE;
                    y1 = MARGIN + line.Row * CELL_SIZE;
                    x2 = x1 + CELL_SIZE;
                    y2 = y1;
                }
                else
                {
                    x1 = MARGIN + line.Col * CELL_SIZE;
                    y1 = MARGIN + line.Row * CELL_SIZE;
                    x2 = x1;
                    y2 = y1 + CELL_SIZE;
                }

                g.DrawLine(pen, x1, y1, x2, y2);
            }
        }

        private void DrawCompletedBoxes(Graphics g)
        {
            foreach (var box in _board.Boxes)
            {
                if (box.Owner != null)
                {
                    int x = MARGIN + box.Col * CELL_SIZE + 5;
                    int y = MARGIN + box.Row * CELL_SIZE + 5;
                    int size = CELL_SIZE - 10;

                    using (var brush = new SolidBrush(Color.FromArgb(100, box.Owner.Color)))
                    {
                        g.FillRectangle(brush, x, y, size, size);
                    }

                    // Dibujar inicial del jugador
                    using (var font = new Font("Arial", 16, FontStyle.Bold))
                    using (var textBrush = new SolidBrush(box.Owner.Color))
                    {
                        string initial = box.Owner.Name.Substring(0, 1);
                        var textSize = g.MeasureString(initial, font);
                        float textX = x + (size - textSize.Width) / 2;
                        float textY = y + (size - textSize.Height) / 2;
                        g.DrawString(initial, font, textBrush, textX, textY);
                    }
                }
            }
        }

        private void GamePanel_MouseMove(object? sender, MouseEventArgs e)
        {
            if (_board.IsGameOver)
                return;

            var line = GetLineAtPosition(e.X, e.Y);
            if (line != _hoveredLine)
            {
                _hoveredLine = line;
                _gamePanel.Invalidate();
            }

            // Cambiar cursor
            _gamePanel.Cursor = (line != null && line.IsAvailable) ? Cursors.Hand : Cursors.Default;
        }

        private void GamePanel_MouseClick(object? sender, MouseEventArgs e)
        {
            if (_board.IsGameOver)
                return;

            // Si el jugador actual es IA, no permitir clicks manuales
            if (_board.CurrentPlayer.IsAI)
                return;

            var line = GetLineAtPosition(e.X, e.Y);
            if (line != null && line.IsAvailable)
            {
                MakeMove(line);
            }
        }

        private Line? GetLineAtPosition(int mouseX, int mouseY)
        {
            const int CLICK_TOLERANCE = 15;

            foreach (var line in _board.Lines)
            {
                if (!line.IsAvailable)
                    continue;

                int x1, y1, x2, y2;

                if (line.IsHorizontal)
                {
                    x1 = MARGIN + line.Col * CELL_SIZE;
                    y1 = MARGIN + line.Row * CELL_SIZE;
                    x2 = x1 + CELL_SIZE;
                    y2 = y1;
                }
                else
                {
                    x1 = MARGIN + line.Col * CELL_SIZE;
                    y1 = MARGIN + line.Row * CELL_SIZE;
                    x2 = x1;
                    y2 = y1 + CELL_SIZE;
                }

                // Verificar si el click está cerca de la línea
                double distance = DistanceFromPointToSegment(mouseX, mouseY, x1, y1, x2, y2);
                if (distance < CLICK_TOLERANCE)
                {
                    return line;
                }
            }

            return null;
        }

        private double DistanceFromPointToSegment(int px, int py, int x1, int y1, int x2, int y2)
        {
            double A = px - x1;
            double B = py - y1;
            double C = x2 - x1;
            double D = y2 - y1;

            double dot = A * C + B * D;
            double lenSq = C * C + D * D;
            double param = (lenSq != 0) ? dot / lenSq : -1;

            double xx, yy;

            if (param < 0)
            {
                xx = x1;
                yy = y1;
            }
            else if (param > 1)
            {
                xx = x2;
                yy = y2;
            }
            else
            {
                xx = x1 + param * C;
                yy = y1 + param * D;
            }

            double dx = px - xx;
            double dy = py - yy;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        private void MakeMove(Line line)
        {
            bool gotExtraTurn = _board.TryMarkLine(line);
            _suggestedLine = null; // Limpiar sugerencia
            _gamePanel.Invalidate();
            UpdateScoreDisplay();

            if (_board.IsGameOver)
            {
                ShowGameOver();
            }
            else if (_board.CurrentPlayer.IsAI && !gotExtraTurn)
            {
                // Si ahora toca la IA, que juegue automáticamente
                System.Threading.Tasks.Task.Delay(500).ContinueWith(_ => 
                {
                    if (InvokeRequired)
                        Invoke(new Action(PlayAITurn));
                    else
                        PlayAITurn();
                });
            }
        }

        private void PlayAITurn()
        {
            if (_board.IsGameOver || !_board.CurrentPlayer.IsAI)
                return;

            var move = _solver.SuggestBestMove(_board);
            if (move != null)
            {
                MakeMove(move);
            }
        }

        private void UpdateScoreDisplay()
        {
            _lblPlayer1Score.Text = $"Cuadros: {_board.Player1.Score}";
            _lblPlayer2Score.Text = $"Cuadros: {_board.Player2.Score}";

            _lblCurrentTurn.Text = $"Turno: {_board.CurrentPlayer.Name}";
            _lblCurrentTurn.ForeColor = _board.CurrentPlayer.Color;
            _lblCurrentTurn.BackColor = Color.FromArgb(20, _board.CurrentPlayer.Color);

            int totalBoxes = _board.Boxes.Count;
            int claimedBoxes = _board.Boxes.Count(b => b.Owner != null);
            _lblGameStatus.Text = $"Cuadros completados:\n{claimedBoxes} de {totalBoxes}";

            _btnAutoPlay.Enabled = _board.CurrentPlayer.IsAI;
        }

        private void ShowGameOver()
        {
            var winner = _board.GetWinner();
            string message;

            if (winner == null)
            {
                message = $"¡EMPATE!\n\nAmbos jugadores: {_board.Player1.Score} cuadros";
            }
            else
            {
                message = $"¡{winner.Name} GANA!\n\n" +
                         $"{winner.Name}: {winner.Score} cuadros\n" +
                         $"{(winner == _board.Player1 ? _board.Player2.Name : _board.Player1.Name)}: " +
                         $"{(winner == _board.Player1 ? _board.Player2.Score : _board.Player1.Score)} cuadros";
            }

            _lblGameStatus.Text = "¡Juego terminado!";
            MessageBox.Show(message, "Fin del Juego", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnNewGame_Click(object? sender, EventArgs e)
        {
            _board.Reset();
            _hoveredLine = null;
            _suggestedLine = null;
            _gamePanel.Invalidate();
            UpdateScoreDisplay();
        }

        private void BtnAISuggestion_Click(object? sender, EventArgs e)
        {
            if (_board.IsGameOver)
                return;

            _suggestedLine = _solver.SuggestBestMove(_board);
            _gamePanel.Invalidate();

            if (_suggestedLine != null)
            {
                MessageBox.Show(
                    $"La IA sugiere marcar la línea:\n\n" +
                    $"Posición: Fila {_suggestedLine.Row}, Columna {_suggestedLine.Col}\n" +
                    $"Orientación: {(_suggestedLine.IsHorizontal ? "Horizontal" : "Vertical")}\n\n" +
                    $"(Se muestra en dorado en el tablero)",
                    "Sugerencia de IA",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void BtnAutoPlay_Click(object? sender, EventArgs e)
        {
            PlayAITurn();
        }

        private void ChkPlayer2AI_CheckedChanged(object? sender, EventArgs e)
        {
            var newPlayer2 = new Player("Jugador 2", Color.Blue, isAI: _chkPlayer2AI.Checked);
            _board.SetPlayer2(newPlayer2);
            
            // Si ahora toca la IA, que juegue
            if (_chkPlayer2AI.Checked && _board.CurrentPlayer == _board.Player2)
            {
                System.Threading.Tasks.Task.Delay(500).ContinueWith(_ =>
                {
                    if (InvokeRequired)
                        Invoke(new Action(PlayAITurn));
                    else
                        PlayAITurn();
                });
            }

            UpdateScoreDisplay();
        }
    }
}
