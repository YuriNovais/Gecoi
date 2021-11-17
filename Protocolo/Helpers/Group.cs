using System.Collections.Generic;
using System.Linq;

namespace Protocolo.Helpers
{
    public class Group<K, T>
    {
        public K Key;

        public IEnumerable<T> Values;

        public int Quantidade
        {
            get
            {
                return Values.Count();
            }
        }
    }
}