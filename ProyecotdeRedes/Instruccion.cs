using System;
using System.Collections.Generic;
using System.Text;

namespace ProyecotdeRedes
{
     
    class Instruccion
    {
        uint time; 
        string _instruccion;

        public Instruccion (string instruccion )
        {
            this._instruccion = instruccion;
            uint.TryParse(instruccion.Split(' ')[0] , out time); 
        }

        public String instruccion
        {
            get => this._instruccion; 
        }

        public uint Time
        {
            get => this.time;
        }
    }
}
