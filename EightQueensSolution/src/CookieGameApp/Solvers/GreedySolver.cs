using System;
using System.Collections.Generic;
using System.Linq;
using CookieGameApp.Models;

namespace CookieGameApp.Solvers
{
    /// <summary>
    /// Solver que usa algoritmo Greedy con heurística de eficiencia.
    /// La heurística calcula cuánto CPS se gana por cookie invertida,
    /// considerando también el tiempo de recuperación de la inversión.
    /// Este es un algoritmo de IA basado en búsqueda heurística.
    /// </summary>
    public sealed class GreedySolver : ICookieSolver
    {
        public Producer? SuggestNextPurchase(GameEngine engine)
        {
            if (engine == null) throw new ArgumentNullException(nameof(engine));

            // Buscar el productor con mejor eficiencia que podemos comprar
            var affordable = engine.Producers
                .Where(p => engine.Cookies >= p.CurrentCost)
                .ToList();

            if (!affordable.Any()) return null;

            // Heurística: Eficiencia = CPS ganado / costo
            // Esto favorece compras que dan mejor retorno de inversión
            return affordable
                .OrderByDescending(p => CalculateEfficiency(p, engine))
                .First();
        }

        public string[] CalculateOptimalStrategy(GameEngine engine, double targetCookies, double timeLimit = 3600)
        {
            if (engine == null) throw new ArgumentNullException(nameof(engine));

            var strategy = new List<string>();
            var simulation = CloneEngine(engine);
            double elapsedTime = 0;

            // Simular el juego hasta alcanzar el objetivo o el tiempo límite
            while (simulation.Cookies < targetCookies && elapsedTime < timeLimit)
            {
                // Encontrar el mejor productor que podríamos comprar eventualmente
                var bestProducer = FindBestProducerToSaveFor(simulation);
                if (bestProducer == null) break;

                // Calcular tiempo necesario para comprarlo
                double timeToAfford = CalculateTimeToAfford(simulation, bestProducer);
                
                if (elapsedTime + timeToAfford > timeLimit) break;

                // Avanzar el tiempo hasta poder comprarlo
                simulation.Advance(timeToAfford);
                elapsedTime += timeToAfford;

                // Comprarlo
                if (simulation.TryBuy(bestProducer.Id))
                {
                    strategy.Add(bestProducer.Id);
                }
            }

            return strategy.ToArray();
        }

        /// <summary>
        /// Calcula la eficiencia de un productor usando una heurística compuesta.
        /// Considera: CPS por costo, tiempo de recuperación, y escalabilidad futura.
        /// </summary>
        private double CalculateEfficiency(Producer producer, GameEngine engine)
        {
            double cost = producer.CurrentCost;
            double cpsGain = producer.ProductionPerSecond;
            
            // Heurística principal: CPS ganado por cookie gastada
            double basicEfficiency = cpsGain / cost;

            // Factor de tiempo de recuperación (ROI - Return On Investment)
            // Menor tiempo de recuperación = mejor
            double currentCPS = engine.TotalCPS > 0 ? engine.TotalCPS : 1;
            double recoveryTime = cost / currentCPS;
            double timeFactor = 1.0 / (1.0 + recoveryTime / 100.0); // normalizado

            // Factor de escalabilidad: productores más caros suelen ser más eficientes a largo plazo
            double scaleFactor = Math.Log(cpsGain + 1);

            // Combinar todas las heurísticas
            return basicEfficiency * timeFactor * scaleFactor;
        }

        /// <summary>
        /// Encuentra el mejor productor considerando el estado futuro del juego.
        /// </summary>
        private Producer? FindBestProducerToSaveFor(GameEngine engine)
        {
            var producers = engine.Producers.ToList();
            if (!producers.Any()) return null;

            // Evaluar todos los productores, incluso los que no podemos comprar aún
            return producers
                .OrderByDescending(p => CalculateEfficiency(p, engine))
                .First();
        }

        /// <summary>
        /// Calcula cuánto tiempo se necesita para poder comprar un productor.
        /// </summary>
        private double CalculateTimeToAfford(GameEngine engine, Producer producer)
        {
            double cost = producer.CurrentCost;
            double currentCookies = engine.Cookies;
            double cps = engine.TotalCPS;

            if (currentCookies >= cost) return 0;
            if (cps <= 0) return double.MaxValue; // imposible sin producción

            return (cost - currentCookies) / cps;
        }

        /// <summary>
        /// Clona el motor del juego para simulaciones.
        /// </summary>
        private GameEngine CloneEngine(GameEngine original)
        {
            // Crear nuevos productores con el mismo estado
            var clonedProducers = original.Producers.Select(p => CloneProducer(p)).ToList();
            var cloned = new GameEngine(clonedProducers);
            
            // Usar reflexión para establecer las cookies (ya que no hay setter público)
            var cookiesProperty = typeof(GameEngine).GetProperty("Cookies");
            cookiesProperty?.SetValue(cloned, original.Cookies);

            return cloned;
        }

        private Producer CloneProducer(Producer original)
        {
            // Crear una instancia del mismo tipo
            Producer clone;
            if (original is Cursor)
                clone = new Cursor();
            else if (original is Grandma)
                clone = new Grandma();
            else
                throw new NotSupportedException($"Tipo de productor no soportado: {original.GetType().Name}");

            // Replicar las compras
            for (int i = 0; i < original.Quantity; i++)
            {
                clone.Buy();
            }

            return clone;
        }
    }
}
