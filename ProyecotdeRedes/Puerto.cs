using System;
using System.Collections.Generic;
using System.Text;

namespace ProyecotdeRedes
{
    class Puerto
    {
        Dispositivo _dispositivoConectado;

        uint numerodelPuerto;

        Bit bitdesalida; 

        public Puerto ()
        {
            this.numerodelPuerto = 1;
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
