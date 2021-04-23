using System;
using System.Collections.Generic;
using System.Text;

namespace ProyecotdeRedes
{
    class Puerto
    {
        string id_puerto;        
        int numero_puerto;

        string puertoalqueestaconectado;

        Dispositivo _dispositivoConectado;
        Bit bitdesalida;

        bool[] entradas;

        bool estaConectado;

        public string PuertoAlQueEstaConnectado
        {
            get => this.puertoalqueestaconectado;
            set => this.puertoalqueestaconectado = value;
        }

        public int NumeroPuertoAlQueEstaConectado
        {
            get =>  (int.Parse(this.puertoalqueestaconectado.Split('_')[1]) -1);
        } 

        public void DesconectarElPuerto()
        {
            this.puertoalqueestaconectado = null;
            this._dispositivoConectado = null;
            this.bitdesalida = Bit.none;
            this.estaConectado = false;

            LimpiarEntradas(); 
        }


        public Puerto (string id_puerto , int numero_puerto)
        {
            this.id_puerto = id_puerto;
            this.bitdesalida = Bit.none;
            this.numero_puerto = numero_puerto;

            entradas = new bool[Enum.GetNames(typeof(Bit)).Length];
        }


        public bool EstaConectadoAOtroDispositivo
        {
            get => this.estaConectado;
            set => this.estaConectado = value; 
        }
        public string ID_Puerto
        {
            get => this.id_puerto;
        }

        public int Numero_Puerto
        {
            get => this.numero_puerto; 
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

        public bool [] Entradas
        {
            get => this.entradas; 
        }

        public void LimpiarEntradas()
        {
            for (int i = 0; i < this.entradas.Length; i++)
                this.entradas[i] = false; 
        }

        public void RecibirUnBit (Bit bit)
        {
            this.entradas[(int)bit] = true; 
        }
    }
}
