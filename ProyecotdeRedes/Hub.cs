using System;
using System.Collections.Generic;
using System.Text;

namespace ProyecotdeRedes
{
    class Hub:Dispositivo 
    {
        int numeroDePuertos; 
        public Hub(string name ):base (name)
        {
            this.numeroDePuertos = 4; 
        }
    }
}
