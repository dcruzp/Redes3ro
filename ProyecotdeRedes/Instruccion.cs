using System;
using System.Collections.Generic;
using System.Text;

namespace ProyecotdeRedes
{
     
    class Instruccion
    {
        string _instruccion;

        public Instruccion (string instruccion )
        {
            this._instruccion = instruccion; 
        }

        public String instruccion
        {
            get => this._instruccion; 
        }
    }
}
