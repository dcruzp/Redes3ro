using System;
using System.Collections.Generic;
using System.Text;

namespace ProyecotdeRedes
{
    public enum Bit
    {
        cero = 0,
        uno = 1,
        none = 2 
    }

    class Bits
    {
        public static Bit XOR(Bit bit1, Bit bit2)
        {
            if (bit1 == bit2) return Bit.cero;
            else return Bit.uno;
        }
    }
}
