using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyecotdeRedes
{
    class Switch : Dispositivo
    {
        string[] direcionesMac; 

        public Switch(string name, int cantidaddepuertos, int indice) : base(name, cantidaddepuertos, indice)
        {
            this.name = name;
            this.cantidaddepuertos = cantidaddepuertos;
            this.indice = indice;
            this.direcionesMac = new string[cantidaddepuertos]; 
        }
    }
}
