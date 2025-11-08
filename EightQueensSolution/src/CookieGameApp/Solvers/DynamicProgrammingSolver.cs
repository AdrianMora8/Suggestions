using System;
using System.Collections.Generic;
using System.Linq;
using CookieGameApp.Models;

namespace CookieGameApp.Solvers
{
    /// <summary>
    /// Solver que usa programación dinámica para encontrar la estrategia óptima.
    /// Este algoritmo de IA considera todos los estados posibles y encuentra
    /// la secuencia de compras que maximiza el CPS en un tiempo dado.
    /// Utiliza memoización para optimizar el cálculo.
    /// </summary>
    public sealed class DynamicProgrammingSolver : ICookieSolver
    {
        private readonly Dictionary<string, (double maxCPS, List<string> purchases)> _memo;

        public DynamicProgrammingSolver()
        {
            _memo = new Dictionary<string, (double, List<string>)>();
        }

        public Producer? SuggestNextPurchase(GameEngine engine)
        {
            if (engine == null) throw new ArgumentNullException(nameof(engine));

            // Usar estrategia dinámica para los próximos 3 pasos
            var strategy = CalculateOptimalStrategy(engine, engine.Cookies * 2, 300);
            
            if (strategy.Length == 0) return null;

            // Retornar el primer productor de la estrategia
            var nextId = strategy[0];
            return engine.Producers.FirstOrDefault(p => p.Id == nextId);
        }

        public string[] CalculateOptimalStrategy(GameEngine engine, double targetCookies, double timeLimit = 3600)
        {
            if (engine == null) throw new ArgumentNullException(nameof(engine));

            _memo.Clear();

            // Estado inicial
            var initialState = new GameState
            {
                Cookies = engine.Cookies,
                ProducerQuantities = engine.Producers.ToDictionary(p => p.Id, p => p.Quantity),
                TimeElapsed = 0
            };

            var result = FindOptimalPath(initialState, engine.Producers.ToList(), targetCookies, timeLimit);
            return result.purchases.ToArray();
        }

        /// <summary>
        /// Encuentra el camino óptimo usando programación dinámica con memoización.
        /// </summary>
        private (double maxCPS, List<string> purchases) FindOptimalPath(
            GameState state, 
            List<Producer> producers, 
            double targetCookies, 
            double timeLimit)
        {
            // Caso base: alcanzamos el objetivo o el límite de tiempo
            if (state.Cookies >= targetCookies || state.TimeElapsed >= timeLimit)
            {
                double currentCPS = CalculateCPS(state, producers);
                return (currentCPS, new List<string>());
            }

            // Verificar memo
            string stateKey = state.GetKey();
            if (_memo.TryGetValue(stateKey, out var cached))
            {
                return cached;
            }

            double bestCPS = CalculateCPS(state, producers);
            List<string> bestPurchases = new List<string>();

            // Probar comprar cada productor
            foreach (var producer in producers)
            {
                double cost = CalculateCost(producer, state.ProducerQuantities[producer.Id]);
                
                // Si no podemos comprar, calcular tiempo para ahorrar
                double timeToWait = 0;
                if (state.Cookies < cost)
                {
                    double cps = CalculateCPS(state, producers);
                    if (cps <= 0) continue; // no podemos generar más cookies
                    
                    timeToWait = (cost - state.Cookies) / cps;
                    if (state.TimeElapsed + timeToWait > timeLimit) continue;
                }

                // Crear nuevo estado después de la compra
                var newState = new GameState
                {
                    Cookies = state.Cookies + (CalculateCPS(state, producers) * timeToWait) - cost,
                    ProducerQuantities = new Dictionary<string, int>(state.ProducerQuantities),
                    TimeElapsed = state.TimeElapsed + timeToWait
                };
                newState.ProducerQuantities[producer.Id]++;

                // Recursión con límite de profundidad
                if (bestPurchases.Count < 10) // limitar profundidad para evitar explosión combinatoria
                {
                    var (futureCPS, futurePurchases) = FindOptimalPath(newState, producers, targetCookies, timeLimit);
                    
                    if (futureCPS > bestCPS)
                    {
                        bestCPS = futureCPS;
                        bestPurchases = new List<string> { producer.Id };
                        bestPurchases.AddRange(futurePurchases);
                    }
                }
            }

            var result = (bestCPS, bestPurchases);
            _memo[stateKey] = result;
            return result;
        }

        /// <summary>
        /// Calcula el CPS actual dado un estado del juego.
        /// </summary>
        private double CalculateCPS(GameState state, List<Producer> producers)
        {
            double total = 0;
            foreach (var producer in producers)
            {
                int quantity = state.ProducerQuantities[producer.Id];
                total += quantity * producer.ProductionPerSecond;
            }
            return total;
        }

        /// <summary>
        /// Calcula el costo de un productor dado cuántos ya se han comprado.
        /// </summary>
        private double CalculateCost(Producer producer, int currentQuantity)
        {
            return Math.Round(producer.BaseCost * Math.Pow(producer.CostMultiplier, currentQuantity), 2);
        }

        /// <summary>
        /// Representa un estado del juego para la programación dinámica.
        /// </summary>
        private class GameState
        {
            public double Cookies { get; set; }
            public Dictionary<string, int> ProducerQuantities { get; set; } = new();
            public double TimeElapsed { get; set; }

            public string GetKey()
            {
                // Crear clave única para memoización
                var quantities = string.Join(",", ProducerQuantities.OrderBy(kvp => kvp.Key).Select(kvp => $"{kvp.Key}:{kvp.Value}"));
                return $"{Math.Round(Cookies, 0)}|{quantities}|{Math.Round(TimeElapsed, 0)}";
            }
        }
    }
}
