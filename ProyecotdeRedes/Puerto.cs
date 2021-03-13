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
        }

        public Bit BitdeSalida
        {
            get => this.bitdesalida;
            set => this.bitdesalida = value;
        }
        public Dispositivo DispositivoConectado
        {
            get => this.DispositivoConectado;
            set => this.DispositivoConectado = value; 
        }


    }
}
