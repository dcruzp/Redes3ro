using System;
using System.Collections.Generic;
using System.Text;

namespace ProyecotdeRedes
{
    class Dispositivo
    {
        Puerto _puerto;

        string name; 

        public Dispositivo (string name)
        {
            this.name = name; 
        }

        public Puerto puerto
        {
            get => this._puerto;
        }
    }
}
