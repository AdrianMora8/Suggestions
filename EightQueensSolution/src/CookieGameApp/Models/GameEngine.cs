using System;
using System.Collections.Generic;
using System.Linq;

namespace CookieGameApp.Models
{
    /// <summary>
    /// Motor sencillo del juego: lleva la cantidad de cookies y la lista de productores.
    /// No usa temporizadores reales; dispone de Advance(seconds) para simular la producción.
    /// </summary>
    public sealed class GameEngine
    {
        public double Cookies { get; private set; }
        public IReadOnlyList<Producer> Producers { get; }

        public GameEngine(IEnumerable<Producer> producers)
        {
            Producers = producers.ToArray();
            Cookies = 0;
        }

        public void Click()
        {
            Cookies += 1; // cada click da 1 cookie
        }

        public bool CanBuy(Producer p)
        {
            return Cookies >= p.CurrentCost;
        }

        public bool TryBuy(string producerId)
        {
            var p = Producers.FirstOrDefault(x => x.Id.Equals(producerId, StringComparison.OrdinalIgnoreCase));
            if (p == null) return false;
            var cost = p.CurrentCost;
            if (Cookies < cost) return false;
            Cookies -= cost;
            p.Buy();
            return true;
        }

        public double TotalCPS => Producers.Sum(p => p.TotalProduction);

        public void Advance(double seconds)
        {
            if (seconds <= 0) return;
            Cookies += TotalCPS * seconds;
        }
    }
}
