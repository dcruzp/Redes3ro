using System;
using System.Collections.Generic;
using System.Text;

namespace ProyecotdeRedes
{
    class Puerto
    {
        Dispositivo _dispositivoConectado;
        Bit bitdesalida; 

        public Puerto ()
        {            
            this.bitdesalida = Bit.none; 
        }

        public Bit BitdeSalida
        {
            get => this.bitdesalida;
            set => this.bitdesalida = value;
        }
        public Dispositivo DispositivoConectado
        {
            get => this._dispositivoConectado;
            set => this._dispositivoConectado = value; 
        }
    }
}
