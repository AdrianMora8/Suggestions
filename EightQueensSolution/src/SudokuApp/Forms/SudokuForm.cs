using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Media;
using SudokuApp.Models;
using SudokuApp.Solvers;

namespace SudokuApp.Forms
{
    /// <summary>
    /// Interfaz gráfica para jugar Sudoku con un solo jugador.
    /// Permite ingresar números, resolver automáticamente y cargar ejemplos.
    /// </summary>
    public class SudokuForm : Form
    {
        private SudokuBoard _board;
        private SudokuBoard _initialBoard; // Para marcar las celdas originales
        private ISudokuSolver _solver;
        private TextBox[,] _cells;
        private const int CELL_SIZE = 50;
        private const int GRID_MARGIN = 20;
        
        // UI Controls
        private Panel _gridPanel;
        private Button _btnSolve;
        private Button _btnClear;
        private Button _btnCheck;
        private Button _btnLoadExample;
        private Button _btnHint;
        private Label _lblStatus;
        private Label _lblTitle;

        public SudokuForm()
        {
            _board = new SudokuBoard();
            _initialBoard = _board.Clone();
            _solver = new BacktrackingSudokuSolver();
            _cells = new TextBox[9, 9];
            
            InitializeUI();
            CreateGrid();
        }

        private void InitializeUI()
        {
            Text = "Sudoku - Resolver con Backtracking";
            Size = new Size(700, 750);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(240, 240, 245);
            DoubleBuffered = true;

            // Título
            _lblTitle = new Label
            {
                Text = "🧩 SUDOKU SOLVER",
                Location = new Point(20, 15),
                Size = new Size(660, 35),
                Font = new Font("Arial", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 50, 100),
                TextAlign = ContentAlignment.MiddleCenter
            };
            Controls.Add(_lblTitle);

            // Panel del grid
            _gridPanel = new Panel
            {
                Location = new Point(GRID_MARGIN, 60),
                Size = new Size(CELL_SIZE * 9 + 10, CELL_SIZE * 9 + 10),
                BackColor = Color.Black
            };
            _gridPanel.Paint += GridPanel_Paint;
            Controls.Add(_gridPanel);

            int btnY = _gridPanel.Bottom + 20;
            int btnX = GRID_MARGIN;

            // Botón cargar ejemplo
            _btnLoadExample = new Button
            {
                Text = "📋 Cargar Ejemplo",
                Location = new Point(btnX, btnY),
                Size = new Size(140, 40),
                BackColor = Color.LightBlue,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            _btnLoadExample.FlatAppearance.BorderColor = Color.DarkBlue;
            _btnLoadExample.Click += BtnLoadExample_Click;
            Controls.Add(_btnLoadExample);

            // Botón resolver
            _btnSolve = new Button
            {
                Text = "🤖 Resolver",
                Location = new Point(btnX + 150, btnY),
                Size = new Size(120, 40),
                BackColor = Color.LightGreen,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            _btnSolve.FlatAppearance.BorderColor = Color.DarkGreen;
            _btnSolve.Click += BtnSolve_Click;
            Controls.Add(_btnSolve);

            // Botón pista
            _btnHint = new Button
            {
                Text = "💡 Pista",
                Location = new Point(btnX + 280, btnY),
                Size = new Size(100, 40),
                BackColor = Color.Gold,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            _btnHint.FlatAppearance.BorderColor = Color.DarkGoldenrod;
            _btnHint.Click += BtnHint_Click;
            Controls.Add(_btnHint);

            // Botón verificar
            _btnCheck = new Button
            {
                Text = "✓ Verificar",
                Location = new Point(btnX + 390, btnY),
                Size = new Size(110, 40),
                BackColor = Color.LightCoral,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            _btnCheck.FlatAppearance.BorderColor = Color.DarkRed;
            _btnCheck.Click += BtnCheck_Click;
            Controls.Add(_btnCheck);

            // Botón limpiar
            _btnClear = new Button
            {
                Text = "🗑️ Limpiar",
                Location = new Point(btnX, btnY + 50),
                Size = new Size(140, 40),
                BackColor = Color.Orange,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            _btnClear.FlatAppearance.BorderColor = Color.DarkOrange;
            _btnClear.Click += BtnClear_Click;
            Controls.Add(_btnClear);

            // Label de estado
            _lblStatus = new Label
            {
                Location = new Point(GRID_MARGIN, btnY + 100),
                Size = new Size(660, 60),
                Font = new Font("Arial", 10),
                ForeColor = Color.FromArgb(80, 80, 80),
                Text = "Ingresa números del 1-9 en las celdas vacías.\n" +
                       "Las celdas originales están marcadas en azul.",
                TextAlign = ContentAlignment.TopLeft,
                BackColor = Color.FromArgb(250, 250, 250),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10)
            };
            Controls.Add(_lblStatus);

            // Instrucciones
            var lblInstructions = new Label
            {
                Text = "CONTROLES:\n" +
                       "• Escribe números 1-9\n" +
                       "• Presiona Backspace/Delete para borrar\n" +
                       "• Tab/Flechas para navegar\n" +
                       "• 'Resolver' usa algoritmo Backtracking",
                Location = new Point(520, 60),
                Size = new Size(160, 450),
                Font = new Font("Arial", 8),
                ForeColor = Color.FromArgb(100, 100, 100),
                BackColor = Color.FromArgb(250, 250, 255),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10)
            };
            Controls.Add(lblInstructions);
        }

        private void CreateGrid()
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    var textBox = new TextBox
                    {
                        Location = new Point(col * CELL_SIZE + 5, row * CELL_SIZE + 5),
                        Size = new Size(CELL_SIZE - 2, CELL_SIZE - 2),
                        Font = new Font("Arial", 20, FontStyle.Bold),
                        TextAlign = HorizontalAlignment.Center,
                        MaxLength = 1,
                        BorderStyle = BorderStyle.FixedSingle,
                        BackColor = Color.White,
                        Tag = new Point(row, col)
                    };

                    textBox.KeyPress += Cell_KeyPress;
                    textBox.TextChanged += Cell_TextChanged;
                    textBox.Enter += Cell_Enter;

                    _cells[row, col] = textBox;
                    _gridPanel.Controls.Add(textBox);
                }
            }

            UpdateCellsFromBoard();
        }

        private void GridPanel_Paint(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            
            // Dibujar líneas gruesas para las cajas 3x3
            using (var pen = new Pen(Color.Black, 3))
            {
                for (int i = 0; i <= 9; i += 3)
                {
                    // Líneas horizontales
                    g.DrawLine(pen, 5, i * CELL_SIZE + 5, 9 * CELL_SIZE + 5, i * CELL_SIZE + 5);
                    // Líneas verticales
                    g.DrawLine(pen, i * CELL_SIZE + 5, 5, i * CELL_SIZE + 5, 9 * CELL_SIZE + 5);
                }
            }
        }

        private void Cell_KeyPress(object? sender, KeyPressEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            var pos = (Point)textBox.Tag;
            
            // Solo permitir números 1-9, backspace, delete
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                return;
            }

            if (char.IsDigit(e.KeyChar) && e.KeyChar == '0')
            {
                e.Handled = true;
                return;
            }

            // No permitir editar celdas iniciales
            if (!_initialBoard.IsEmpty(pos.X, pos.Y))
            {
                e.Handled = true;
                SystemSounds.Beep.Play();
            }
        }

        private void Cell_TextChanged(object? sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            var pos = (Point)textBox.Tag;
            
            if (string.IsNullOrEmpty(textBox.Text))
            {
                _board.Set(pos.X, pos.Y, 0);
                return;
            }

            if (int.TryParse(textBox.Text, out int value) && value >= 1 && value <= 9)
            {
                _board.Set(pos.X, pos.Y, value);
                
                // Validar si es correcto
                if (!_board.IsValidPlacement(pos.X, pos.Y, value))
                {
                    textBox.ForeColor = Color.Red;
                    _lblStatus.Text = "⚠️ Número inválido según las reglas del Sudoku";
                    _lblStatus.ForeColor = Color.Red;
                }
                else
                {
                    textBox.ForeColor = Color.Black;
                    _lblStatus.Text = "✓ Movimiento válido";
                    _lblStatus.ForeColor = Color.Green;
                    
                    // Verificar si está completo
                    CheckIfComplete();
                }
            }
        }

        private void Cell_Enter(object? sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                textBox.SelectAll();
            }
        }

        private void UpdateCellsFromBoard()
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    int value = _board.Get(row, col);
                    var textBox = _cells[row, col];
                    
                    textBox.Text = value == 0 ? "" : value.ToString();
                    
                    // Marcar celdas iniciales con color diferente
                    if (!_initialBoard.IsEmpty(row, col))
                    {
                        textBox.BackColor = Color.FromArgb(200, 220, 255);
                        textBox.ForeColor = Color.DarkBlue;
                        textBox.Font = new Font("Arial", 20, FontStyle.Bold);
                        textBox.ReadOnly = true;
                    }
                    else
                    {
                        textBox.BackColor = Color.White;
                        textBox.ForeColor = Color.Black;
                        textBox.Font = new Font("Arial", 20, FontStyle.Regular);
                        textBox.ReadOnly = false;
                    }
                }
            }
        }

        private void BtnLoadExample_Click(object? sender, EventArgs e)
        {
            // Cargar ejemplo medio
            int[][] rows = new int[9][]
            {
                new[] {5,3,0,0,7,0,0,0,0},
                new[] {6,0,0,1,9,5,0,0,0},
                new[] {0,9,8,0,0,0,0,6,0},
                new[] {8,0,0,0,6,0,0,0,3},
                new[] {4,0,0,8,0,3,0,0,1},
                new[] {7,0,0,0,2,0,0,0,6},
                new[] {0,6,0,0,0,0,2,8,0},
                new[] {0,0,0,4,1,9,0,0,5},
                new[] {0,0,0,0,8,0,0,7,9}
            };

            _board = SudokuBoard.FromRows(rows);
            _initialBoard = _board.Clone();
            UpdateCellsFromBoard();
            _gridPanel.Invalidate();
            
            _lblStatus.Text = "✓ Ejemplo cargado. ¡Intenta resolverlo!";
            _lblStatus.ForeColor = Color.Blue;
        }

        private void BtnSolve_Click(object? sender, EventArgs e)
        {
            _lblStatus.Text = "🔄 Resolviendo con algoritmo Backtracking...";
            _lblStatus.ForeColor = Color.Orange;
            Application.DoEvents();

            var solved = _solver.Solve(_board);
            
            if (solved == null)
            {
                MessageBox.Show(
                    "No se encontró solución para este Sudoku.\n\n" +
                    "Verifica que los números ingresados sean válidos.",
                    "Sin Solución",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                
                _lblStatus.Text = "❌ No se encontró solución";
                _lblStatus.ForeColor = Color.Red;
            }
            else
            {
                _board = solved;
                UpdateCellsFromBoard();
                
                MessageBox.Show(
                    "✓ ¡Sudoku resuelto correctamente!\n\n" +
                    "Algoritmo: Backtracking\n" +
                    "El tablero ahora muestra la solución completa.",
                    "Resuelto",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                
                _lblStatus.Text = "✓ ¡Sudoku resuelto con éxito!";
                _lblStatus.ForeColor = Color.Green;
            }
        }

        private void BtnHint_Click(object? sender, EventArgs e)
        {
            // Resolver y dar una pista para una celda vacía
            var solved = _solver.Solve(_board);
            
            if (solved == null)
            {
                MessageBox.Show(
                    "No se puede dar una pista.\n\n" +
                    "El estado actual no tiene solución válida.",
                    "Sin Pista",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // Encontrar una celda vacía aleatoria
            var emptyCells = _board.EmptyCells().ToList();
            if (!emptyCells.Any())
            {
                MessageBox.Show("El tablero ya está completo.", "Pista", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var random = new Random();
            var cell = emptyCells[random.Next(emptyCells.Count)];
            int hintValue = solved.Get(cell.row, cell.col);

            // Resaltar la celda con la pista
            var textBox = _cells[cell.row, cell.col];
            textBox.BackColor = Color.LightYellow;

            MessageBox.Show(
                $"💡 Pista:\n\n" +
                $"Fila {cell.row + 1}, Columna {cell.col + 1}\n" +
                $"Número sugerido: {hintValue}",
                "Pista del Solver",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            // Restaurar color después de 3 segundos
            System.Threading.Tasks.Task.Delay(3000).ContinueWith(_ =>
            {
                if (InvokeRequired)
                    Invoke(new Action(() => { if (textBox.BackColor == Color.LightYellow) textBox.BackColor = Color.White; }));
            });
        }

        private void BtnCheck_Click(object? sender, EventArgs e)
        {
            // Verificar si el estado actual es válido
            bool hasErrors = false;
            int filledCells = 0;

            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    int value = _board.Get(row, col);
                    if (value != 0)
                    {
                        filledCells++;
                        // Temporalmente limpiar y validar
                        _board.Set(row, col, 0);
                        if (!_board.IsValidPlacement(row, col, value))
                        {
                            hasErrors = true;
                            _cells[row, col].BackColor = Color.FromArgb(255, 200, 200);
                        }
                        _board.Set(row, col, value);
                    }
                }
            }

            if (hasErrors)
            {
                MessageBox.Show(
                    "❌ El tablero tiene errores.\n\n" +
                    "Las celdas con conflictos están marcadas en rojo.",
                    "Errores Encontrados",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                
                _lblStatus.Text = "❌ Hay errores en el tablero";
                _lblStatus.ForeColor = Color.Red;

                // Restaurar colores después de 3 segundos
                System.Threading.Tasks.Task.Delay(3000).ContinueWith(_ =>
                {
                    if (InvokeRequired)
                        Invoke(new Action(UpdateCellsFromBoard));
                });
            }
            else if (filledCells == 81)
            {
                MessageBox.Show(
                    "🎉 ¡FELICIDADES!\n\n" +
                    "Has completado el Sudoku correctamente.\n" +
                    "Todos los números están bien colocados.",
                    "¡Sudoku Completo!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                
                _lblStatus.Text = "🎉 ¡Sudoku completado correctamente!";
                _lblStatus.ForeColor = Color.Green;
            }
            else
            {
                MessageBox.Show(
                    $"✓ El tablero es válido hasta ahora.\n\n" +
                    $"Celdas completadas: {filledCells}/81\n" +
                    $"Faltan: {81 - filledCells} celdas",
                    "Verificación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                
                _lblStatus.Text = $"✓ Válido ({filledCells}/81 celdas)";
                _lblStatus.ForeColor = Color.Green;
            }
        }

        private void BtnClear_Click(object? sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "¿Estás seguro de que quieres limpiar el tablero?\n\n" +
                "Esto borrará todo el progreso actual.",
                "Confirmar",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                _board = new SudokuBoard();
                _initialBoard = _board.Clone();
                UpdateCellsFromBoard();
                _gridPanel.Invalidate();
                
                _lblStatus.Text = "Tablero limpio. Ingresa un nuevo Sudoku.";
                _lblStatus.ForeColor = Color.Blue;
            }
        }

        private void CheckIfComplete()
        {
            var emptyCells = _board.EmptyCells().ToList();
            if (!emptyCells.Any())
            {
                // Verificar si está correcto
                bool allValid = true;
                for (int row = 0; row < 9; row++)
                {
                    for (int col = 0; col < 9; col++)
                    {
                        int value = _board.Get(row, col);
                        _board.Set(row, col, 0);
                        if (!_board.IsValidPlacement(row, col, value))
                        {
                            allValid = false;
                        }
                        _board.Set(row, col, value);
                    }
                }

                if (allValid)
                {
                    MessageBox.Show(
                        "🎉 ¡FELICIDADES!\n\n" +
                        "Has completado el Sudoku correctamente.",
                        "¡Ganaste!",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    
                    _lblStatus.Text = "🎉 ¡Sudoku completado!";
                    _lblStatus.ForeColor = Color.Green;
                }
            }
        }
    }
}
