using System;
using System.Collections.Generic;
using System.Text;

namespace ProyecotdeRedes
{
    class Hub:Dispositivo 
    {
        
        public Hub(string name ,int indice):base (name , 4,indice)
        {
            
        }

        public Hub (string name , int numerodepuertos , int indice):base(name , numerodepuertos,indice)
        {
            if (numerodepuertos < 4 || numerodepuertos > 8)
            {
                throw new IndexOutOfRangeException("No se puede tener un hub con menos de 4 puertos o mas de 8 "); 
            }
            
        }

       
    }
}
