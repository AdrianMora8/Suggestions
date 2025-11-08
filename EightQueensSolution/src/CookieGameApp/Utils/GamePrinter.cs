using System;
using System.Linq;
using CookieGameApp.Models;

namespace CookieGameApp.Utils
{
    public sealed class GamePrinter
    {
        public void PrintStatus(GameEngine engine)
        {
            Console.WriteLine($"Cookies: {Math.Round(engine.Cookies,2)} | CPS: {Math.Round(engine.TotalCPS,2)}");
            Console.WriteLine("Producers:");
            foreach (var p in engine.Producers)
            {
                Console.WriteLine($" - {p.Name} (id: {p.Id}): qty={p.Quantity}, cost={p.CurrentCost}, produces={p.ProductionPerSecond}/u");
            }
        }

        public void PrintSuggestion(Producer suggestion, GameEngine engine)
        {
            Console.WriteLine("╔═══════════════════════════════════════╗");
            Console.WriteLine("║    SUGERENCIA DE IA                  ║");
            Console.WriteLine("╚═══════════════════════════════════════╝");
            Console.WriteLine($"Comprar: {suggestion.Name} ({suggestion.Id})");
            Console.WriteLine($"Costo: {Math.Round(suggestion.CurrentCost, 2)} cookies");
            Console.WriteLine($"Producción: +{suggestion.ProductionPerSecond} CPS");
            Console.WriteLine($"Cantidad actual: {suggestion.Quantity}");
            
            if (engine.Cookies >= suggestion.CurrentCost)
            {
                Console.WriteLine("✓ Puedes comprarlo AHORA");
                Console.WriteLine($"Comando: buy {suggestion.Id}");
            }
            else
            {
                double deficit = suggestion.CurrentCost - engine.Cookies;
                double timeNeeded = engine.TotalCPS > 0 ? deficit / engine.TotalCPS : 0;
                Console.WriteLine($"✗ Necesitas {Math.Round(deficit, 2)} cookies más");
                if (timeNeeded > 0)
                {
                    Console.WriteLine($"Tiempo estimado: {Math.Round(timeNeeded, 1)} segundos");
                    Console.WriteLine($"Comando: wait {Math.Round(timeNeeded, 1)}");
                }
                else
                {
                    Console.WriteLine("Necesitas hacer click o esperar con productores.");
                }
            }
            
            // Calcular eficiencia
            double efficiency = suggestion.ProductionPerSecond / suggestion.CurrentCost;
            Console.WriteLine($"Eficiencia: {Math.Round(efficiency, 4)} CPS por cookie");
        }
    }
}
