using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CookieGameApp.Models;
using CookieGameApp.Solvers;

namespace CookieGameApp.Forms
{
    public class MainForm : Form
    {
        private readonly GameEngine _engine;
        private readonly ICookieSolver _greedySolver;
        private readonly ICookieSolver _dynamicSolver;
        private ICookieSolver _currentSolver;
        private System.Windows.Forms.Timer _gameTimer;

        // UI Controls
        private Label _lblCookies;
        private Label _lblCPS;
        private Label _lblTitle;
        private Button _btnClick;
        private GroupBox _grpProducers;
        private GroupBox _grpAI;
        private GroupBox _grpManual;
        private TextBox _txtLog;
        private RadioButton _rbGreedy;
        private RadioButton _rbDynamic;
        private NumericUpDown _numAutoSteps;
        private NumericUpDown _numWaitSeconds;
        private NumericUpDown _numStrategyTarget;
        private Button _btnSuggest;
        private Button _btnAuto;
        private Button _btnStrategy;
        private Button _btnWait;
        private Panel _panelProducers;

        public MainForm()
        {
            var producers = ProducerFactory.CreateDefaultProducers();
            _engine = new GameEngine(producers);
            _greedySolver = new GreedySolver();
            _dynamicSolver = new DynamicProgrammingSolver();
            _currentSolver = _greedySolver;

            InitializeUI();
            InitializeTimer();
            UpdateDisplay();
        }

        private void InitializeUI()
        {
            // Form settings
            Text = "Cookie Clicker con IA";
            Size = new Size(900, 700);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(240, 240, 240);

            // Title
            _lblTitle = new Label
            {
                Text = "🍪 Cookie Clicker Game 🍪",
                Font = new Font("Arial", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(139, 69, 19),
                AutoSize = false,
                Size = new Size(880, 40),
                Location = new Point(10, 10),
                TextAlign = ContentAlignment.MiddleCenter
            };
            Controls.Add(_lblTitle);

            // Cookies display
            _lblCookies = new Label
            {
                Text = "Cookies: 0",
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.DarkGreen,
                AutoSize = false,
                Size = new Size(350, 30),
                Location = new Point(20, 60),
                TextAlign = ContentAlignment.MiddleLeft
            };
            Controls.Add(_lblCookies);

            // CPS display
            _lblCPS = new Label
            {
                Text = "CPS: 0.0",
                Font = new Font("Arial", 14),
                ForeColor = Color.DarkBlue,
                AutoSize = false,
                Size = new Size(300, 25),
                Location = new Point(20, 95),
                TextAlign = ContentAlignment.MiddleLeft
            };
            Controls.Add(_lblCPS);

            // Manual controls group
            _grpManual = new GroupBox
            {
                Text = "Control Manual",
                Font = new Font("Arial", 10, FontStyle.Bold),
                Location = new Point(20, 130),
                Size = new Size(420, 150)
            };
            Controls.Add(_grpManual);

            // Click button (big cookie)
            _btnClick = new Button
            {
                Text = "🍪\nCLICK!",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Size = new Size(150, 110),
                Location = new Point(15, 25),
                BackColor = Color.SandyBrown,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _btnClick.FlatAppearance.BorderSize = 3;
            _btnClick.FlatAppearance.BorderColor = Color.Chocolate;
            _btnClick.Click += BtnClick_Click;
            _grpManual.Controls.Add(_btnClick);

            // Wait controls
            var lblWait = new Label
            {
                Text = "Esperar:",
                Location = new Point(180, 35),
                AutoSize = true,
                Font = new Font("Arial", 10)
            };
            _grpManual.Controls.Add(lblWait);

            _numWaitSeconds = new NumericUpDown
            {
                Location = new Point(180, 55),
                Size = new Size(80, 25),
                Minimum = 1,
                Maximum = 3600,
                Value = 10,
                DecimalPlaces = 0
            };
            _grpManual.Controls.Add(_numWaitSeconds);

            var lblWaitUnit = new Label
            {
                Text = "segundos",
                Location = new Point(265, 57),
                AutoSize = true,
                Font = new Font("Arial", 9)
            };
            _grpManual.Controls.Add(lblWaitUnit);

            _btnWait = new Button
            {
                Text = "⏩ Avanzar Tiempo",
                Location = new Point(180, 85),
                Size = new Size(220, 40),
                BackColor = Color.LightBlue,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            _btnWait.FlatAppearance.BorderColor = Color.DarkBlue;
            _btnWait.Click += BtnWait_Click;
            _grpManual.Controls.Add(_btnWait);

            // Producers group
            _grpProducers = new GroupBox
            {
                Text = "Productores Disponibles",
                Font = new Font("Arial", 10, FontStyle.Bold),
                Location = new Point(20, 290),
                Size = new Size(420, 200)
            };
            Controls.Add(_grpProducers);

            _panelProducers = new Panel
            {
                Location = new Point(10, 25),
                Size = new Size(400, 165),
                AutoScroll = true
            };
            _grpProducers.Controls.Add(_panelProducers);

            CreateProducerButtons();

            // AI controls group
            _grpAI = new GroupBox
            {
                Text = "Inteligencia Artificial",
                Font = new Font("Arial", 10, FontStyle.Bold),
                Location = new Point(455, 130),
                Size = new Size(420, 360)
            };
            Controls.Add(_grpAI);

            // Solver selection
            var lblSolver = new Label
            {
                Text = "Algoritmo de IA:",
                Location = new Point(15, 25),
                AutoSize = true,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            _grpAI.Controls.Add(lblSolver);

            _rbGreedy = new RadioButton
            {
                Text = "🚀 Greedy (Rápido, Heurístico)",
                Location = new Point(25, 50),
                Size = new Size(250, 25),
                Checked = true,
                Font = new Font("Arial", 9)
            };
            _rbGreedy.CheckedChanged += SolverRadioButton_CheckedChanged;
            _grpAI.Controls.Add(_rbGreedy);

            _rbDynamic = new RadioButton
            {
                Text = "🎯 Dynamic Programming (Óptimo)",
                Location = new Point(25, 75),
                Size = new Size(300, 25),
                Font = new Font("Arial", 9)
            };
            _rbDynamic.CheckedChanged += SolverRadioButton_CheckedChanged;
            _grpAI.Controls.Add(_rbDynamic);

            // Suggest button
            _btnSuggest = new Button
            {
                Text = "💡 Sugerir Siguiente Compra",
                Location = new Point(15, 115),
                Size = new Size(390, 45),
                BackColor = Color.LightGreen,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 11, FontStyle.Bold)
            };
            _btnSuggest.FlatAppearance.BorderColor = Color.DarkGreen;
            _btnSuggest.Click += BtnSuggest_Click;
            _grpAI.Controls.Add(_btnSuggest);

            // Auto-play controls
            var lblAuto = new Label
            {
                Text = "Compras automáticas:",
                Location = new Point(15, 175),
                AutoSize = true,
                Font = new Font("Arial", 9)
            };
            _grpAI.Controls.Add(lblAuto);

            _numAutoSteps = new NumericUpDown
            {
                Location = new Point(165, 172),
                Size = new Size(60, 25),
                Minimum = 1,
                Maximum = 20,
                Value = 5
            };
            _grpAI.Controls.Add(_numAutoSteps);

            _btnAuto = new Button
            {
                Text = "🤖 Ejecutar Auto-Play",
                Location = new Point(240, 167),
                Size = new Size(165, 35),
                BackColor = Color.Gold,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            _btnAuto.FlatAppearance.BorderColor = Color.DarkGoldenrod;
            _btnAuto.Click += BtnAuto_Click;
            _grpAI.Controls.Add(_btnAuto);

            // Strategy controls
            var lblStrategy = new Label
            {
                Text = "Objetivo de cookies:",
                Location = new Point(15, 225),
                AutoSize = true,
                Font = new Font("Arial", 9)
            };
            _grpAI.Controls.Add(lblStrategy);

            _numStrategyTarget = new NumericUpDown
            {
                Location = new Point(145, 222),
                Size = new Size(80, 25),
                Minimum = 100,
                Maximum = 1000000,
                Value = 1000,
                DecimalPlaces = 0
            };
            _grpAI.Controls.Add(_numStrategyTarget);

            _btnStrategy = new Button
            {
                Text = "📊 Calcular Estrategia",
                Location = new Point(240, 217),
                Size = new Size(165, 35),
                BackColor = Color.LightCoral,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            _btnStrategy.FlatAppearance.BorderColor = Color.DarkRed;
            _btnStrategy.Click += BtnStrategy_Click;
            _grpAI.Controls.Add(_btnStrategy);

            // Log area
            var lblLog = new Label
            {
                Text = "Registro de actividad:",
                Location = new Point(15, 265),
                AutoSize = true,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            _grpAI.Controls.Add(lblLog);

            _txtLog = new TextBox
            {
                Location = new Point(15, 285),
                Size = new Size(390, 65),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                BackColor = Color.White,
                Font = new Font("Consolas", 8)
            };
            _grpAI.Controls.Add(_txtLog);

            // Log text box at bottom
            var lblMainLog = new Label
            {
                Text = "📋 Registro de Eventos:",
                Location = new Point(20, 500),
                AutoSize = true,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            Controls.Add(lblMainLog);

            var txtMainLog = new TextBox
            {
                Location = new Point(20, 525),
                Size = new Size(855, 120),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                BackColor = Color.White,
                Font = new Font("Consolas", 9)
            };
            Controls.Add(txtMainLog);
            _txtLog = txtMainLog; // Use main log
        }

        private void CreateProducerButtons()
        {
            int yPos = 5;
            foreach (var producer in _engine.Producers)
            {
                var panel = new Panel
                {
                    Location = new Point(5, yPos),
                    Size = new Size(370, 65),
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.White
                };

                var lblName = new Label
                {
                    Text = $"{producer.Name} ({producer.Id})",
                    Location = new Point(5, 5),
                    Font = new Font("Arial", 10, FontStyle.Bold),
                    AutoSize = true
                };
                panel.Controls.Add(lblName);

                var lblInfo = new Label
                {
                    Location = new Point(5, 25),
                    Size = new Size(250, 35),
                    Font = new Font("Arial", 8),
                    Text = $"Cantidad: 0\nCosto: {producer.CurrentCost:F2} | CPS: +{producer.ProductionPerSecond:F2}"
                };
                lblInfo.Name = $"lbl_{producer.Id}";
                panel.Controls.Add(lblInfo);

                var btnBuy = new Button
                {
                    Text = "Comprar",
                    Location = new Point(265, 15),
                    Size = new Size(95, 40),
                    BackColor = Color.LightGreen,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Arial", 9, FontStyle.Bold),
                    Tag = producer.Id
                };
                btnBuy.FlatAppearance.BorderColor = Color.Green;
                btnBuy.Click += BtnBuyProducer_Click;
                panel.Controls.Add(btnBuy);

                _panelProducers.Controls.Add(panel);
                yPos += 70;
            }
        }

        private void InitializeTimer()
        {
            _gameTimer = new System.Windows.Forms.Timer
            {
                Interval = 100 // Update every 100ms
            };
            _gameTimer.Tick += GameTimer_Tick;
            _gameTimer.Start();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            // Advance game time by 0.1 seconds
            _engine.Advance(0.1);
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            _lblCookies.Text = $"Cookies: {_engine.Cookies:F2}";
            _lblCPS.Text = $"CPS (Cookies por Segundo): {_engine.TotalCPS:F2}";

            // Update producer info
            foreach (var producer in _engine.Producers)
            {
                var lblInfo = _panelProducers.Controls.Find($"lbl_{producer.Id}", true).FirstOrDefault() as Label;
                if (lblInfo != null)
                {
                    lblInfo.Text = $"Cantidad: {producer.Quantity}\nCosto: {producer.CurrentCost:F2} | CPS: +{producer.ProductionPerSecond:F2}";
                }
            }
        }

        private void BtnClick_Click(object sender, EventArgs e)
        {
            _engine.Click();
            LogMessage("👆 Click manual: +1 cookie");
            UpdateDisplay();
        }

        private void BtnWait_Click(object sender, EventArgs e)
        {
            double seconds = (double)_numWaitSeconds.Value;
            _engine.Advance(seconds);
            LogMessage($"⏩ Tiempo avanzado: {seconds}s. Producido: {(_engine.TotalCPS * seconds):F2} cookies");
            UpdateDisplay();
        }

        private void BtnBuyProducer_Click(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is string producerId)
            {
                var producer = _engine.Producers.FirstOrDefault(p => p.Id == producerId);
                if (producer == null) return;

                double cost = producer.CurrentCost;
                if (_engine.TryBuy(producerId))
                {
                    LogMessage($"✅ Comprado: {producer.Name} (Costo: {cost:F2}, Total: {producer.Quantity})");
                    UpdateDisplay();
                }
                else
                {
                    LogMessage($"❌ No hay suficientes cookies para comprar {producer.Name} (Necesitas: {cost:F2})");
                }
            }
        }

        private void BtnSuggest_Click(object sender, EventArgs e)
        {
            var solverName = _currentSolver is GreedySolver ? "Greedy" : "Dynamic Programming";
            LogMessage($"🤔 Consultando IA ({solverName})...");

            var suggestion = _currentSolver.SuggestNextPurchase(_engine);
            if (suggestion == null)
            {
                LogMessage("💭 IA: No hay sugerencias disponibles. Necesitas más cookies o hacer click.");
            }
            else
            {
                double timeToAfford = 0;
                if (_engine.Cookies < suggestion.CurrentCost && _engine.TotalCPS > 0)
                {
                    timeToAfford = (suggestion.CurrentCost - _engine.Cookies) / _engine.TotalCPS;
                }

                var msg = $"💡 IA sugiere: {suggestion.Name}\n" +
                          $"   Costo: {suggestion.CurrentCost:F2} | CPS: +{suggestion.ProductionPerSecond:F2}";
                if (timeToAfford > 0)
                {
                    msg += $"\n   Tiempo para ahorrar: {timeToAfford:F1}s";
                }
                LogMessage(msg);
            }
        }

        private void BtnAuto_Click(object sender, EventArgs e)
        {
            int steps = (int)_numAutoSteps.Value;
            var solverName = _currentSolver is GreedySolver ? "Greedy" : "Dynamic Programming";
            LogMessage($"🤖 Iniciando auto-play ({steps} compras) con {solverName}...");

            for (int i = 0; i < steps; i++)
            {
                var suggestion = _currentSolver.SuggestNextPurchase(_engine);
                if (suggestion == null)
                {
                    // Try to wait and accumulate
                    double timeToNext = CalculateTimeToNextPurchase();
                    if (timeToNext > 0 && timeToNext < 1000)
                    {
                        _engine.Advance(timeToNext);
                        LogMessage($"   ⏳ Esperando {timeToNext:F1}s para ahorrar...");
                        suggestion = _currentSolver.SuggestNextPurchase(_engine);
                    }
                }

                if (suggestion == null)
                {
                    LogMessage($"   ⚠️ Paso {i + 1}/{steps}: No hay más compras viables.");
                    break;
                }

                if (_engine.TryBuy(suggestion.Id))
                {
                    LogMessage($"   ✅ Paso {i + 1}/{steps}: IA compró {suggestion.Name} (Costo: {suggestion.CurrentCost:F2})");
                }
            }

            LogMessage($"🏁 Auto-play completado.");
            UpdateDisplay();
        }

        private void BtnStrategy_Click(object sender, EventArgs e)
        {
            double target = (double)_numStrategyTarget.Value;
            var solverName = _currentSolver is GreedySolver ? "Greedy" : "Dynamic Programming";
            LogMessage($"📊 Calculando estrategia con {solverName} para alcanzar {target:F0} cookies...");

            var strategy = _currentSolver.CalculateOptimalStrategy(_engine, target);
            if (strategy.Length == 0)
            {
                LogMessage("❌ No se encontró estrategia viable.");
            }
            else
            {
                LogMessage($"✅ Estrategia encontrada ({strategy.Length} compras):");
                for (int i = 0; i < Math.Min(strategy.Length, 10); i++)
                {
                    LogMessage($"   {i + 1}. Comprar {strategy[i]}");
                }
                if (strategy.Length > 10)
                {
                    LogMessage($"   ... y {strategy.Length - 10} compras más.");
                }
            }
        }

        private void SolverRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (_rbGreedy.Checked)
            {
                _currentSolver = _greedySolver;
                LogMessage("🔄 Cambiado a Greedy Solver (rápido, heurístico)");
            }
            else if (_rbDynamic.Checked)
            {
                _currentSolver = _dynamicSolver;
                LogMessage("🔄 Cambiado a Dynamic Programming Solver (óptimo)");
            }
        }

        private double CalculateTimeToNextPurchase()
        {
            var cheapest = _engine.Producers.OrderBy(p => p.CurrentCost).FirstOrDefault();
            if (cheapest == null || _engine.TotalCPS <= 0) return 0;

            double needed = cheapest.CurrentCost - _engine.Cookies;
            if (needed <= 0) return 0;

            return needed / _engine.TotalCPS;
        }

        private void LogMessage(string message)
        {
            if (_txtLog.InvokeRequired)
            {
                _txtLog.Invoke(new Action(() => LogMessage(message)));
                return;
            }

            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            _txtLog.AppendText($"[{timestamp}] {message}\r\n");
            _txtLog.ScrollToCaret();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _gameTimer?.Stop();
            _gameTimer?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
