using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets._Scripts.Characters
{
    public interface IDamageable
    {
        public int MaxHealth { get; }
        public int CurrentHealth { get; }
    }
}
