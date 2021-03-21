using System;
using System.Collections.Generic;
using System.Text;

namespace ProyecotdeRedes
{
    class Hub:Dispositivo 
    {
        
        public Hub(string name ):base (name , 4)
        {
            
        }

        public Hub (string name , int numerodepuertos):base(name , numerodepuertos)
        {
            if (numerodepuertos < 4 || numerodepuertos > 8)
            {
                throw new IndexOutOfRangeException("No se puede tener un hub con menos de 4 puertos o mas de 8 "); 
            }
            
        }

       
    }
}
