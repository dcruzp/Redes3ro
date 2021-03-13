using System;
using System.Collections.Generic;
using System.Text;

namespace ProyecotdeRedes
{
    class Hub:Dispositivo 
    {
        int numb_ports; 
        public Hub()
        {
            this.numb_ports = 4; 
        }
    }
}
