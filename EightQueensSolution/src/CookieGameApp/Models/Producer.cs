using System;

namespace CookieGameApp.Models
{
    /// <summary>
    /// Productor genérico que genera "cookies" por segundo.
    /// </summary>
    public abstract class Producer
    {
        public string Id { get; }
        public string Name { get; }
        public double BaseCost { get; }
        public double CostMultiplier { get; }
        public double ProductionPerSecond { get; }
        public int Quantity { get; private set; }

        protected Producer(string id, string name, double baseCost, double costMultiplier, double pps)
        {
            Id = id;
            Name = name;
            BaseCost = baseCost;
            CostMultiplier = costMultiplier;
            ProductionPerSecond = pps;
            Quantity = 0;
        }

        public double CurrentCost => Math.Round(BaseCost * Math.Pow(CostMultiplier, Quantity), 2);

        public double TotalProduction => Quantity * ProductionPerSecond;

        public void Buy()
        {
            Quantity++;
        }
    }
}
