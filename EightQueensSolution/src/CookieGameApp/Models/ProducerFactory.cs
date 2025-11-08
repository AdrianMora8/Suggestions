using System.Collections.Generic;

namespace CookieGameApp.Models
{
    public static class ProducerFactory
    {
        public static IReadOnlyList<Producer> CreateDefaultProducers()
        {
            return new Producer[]
            {
                new Cursor(),
                new Grandma()
            };
        }
    }
}
