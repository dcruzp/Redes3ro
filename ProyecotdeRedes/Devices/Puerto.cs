using System;
using System.Collections.Generic;
using System.Text;

namespace ProyecotdeRedes
{
    class Puerto
    {

        /// <summary>
        /// esto es para representar le id que tiene el puerto en
        /// el dispositivo al que pertenece 
        /// </summary>
        string id_puerto;   
        

        /// <summary>
        /// Este es para representar el nombre del puerto
        /// con el id correspondiente
        /// </summary>
        int numero_puerto;


        /// <summary>
        /// Este es el nombre del puerto al que esta conectado 
        /// </summary>
        string puertoalqueestaconectado;


        /// <summary>
        /// Esta es para tener la instancia del dispositivo que 
        /// esta conectado por este puerto 
        /// </summary>
        Dispositivo _dispositivoConectado;
        
        

        /// <summary>
        /// Esto es para poner los bit de salida que hay en esta computadora 
        /// </summary>
        Bit bitdesalida;


        /// <summary>
        /// Esto es para saber que bits se han recibido de alguna 
        /// computadora en especifico en un mili segundo determinado
        /// </summary>
        bool[] entradas;


        /// <summary>
        /// esto es para saber si el dispositivo esta conectado con algún 
        /// otro dispositivo 
        /// </summary>
        bool estaConectado;



        public string PuertoAlQueEstaConnectado
        {
            get => this.puertoalqueestaconectado;
            set => this.puertoalqueestaconectado = value;
        }


        /// <summary>
        /// Esto te retorna el indice del puerto al que esta conectado 
        /// el dispositivo por el puerto actual 
        /// </summary>
        public int NumeroPuertoAlQueEstaConectado
        {
            get =>  (int.Parse(this.puertoalqueestaconectado.Split('_')[1]) -1);
        } 


        /// <summary>
        /// Esto te desconecta el puerto actual de cualquier otro puerto 
        /// al que se encuentre conectado 
        /// </summary>
        public void DesconectarElPuerto()
        {
            this.puertoalqueestaconectado = null;
            this._dispositivoConectado = null;
            this.bitdesalida = Bit.none;
            this.estaConectado = false;

            LimpiarEntradas(); 
        }

        /// <summary>
        /// este es el constructor del puerto con su nombre y su id
        /// </summary>
        /// <param name="id_puerto"></param>
        /// <param name="numero_puerto"></param>
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


        /// <summary>
        /// Esto retorna las entradas que tiene el dispositivo 
        /// por el puerto actual en un mili segundo determinado
        /// </summary>
        public bool [] Entradas
        {
            get => this.entradas; 
        }


        /// <summary>
        /// Este pone todas las entradas como si no hubiera recibido 
        /// ningún bit de información 
        /// </summary>
        public void LimpiarEntradas()
        {
            for (int i = 0; i < this.entradas.Length; i++)
                this.entradas[i] = false; 
        }


        /// <summary>
        /// Esto actualiza y pone en las entradas el bit que se recibio
        /// </summary>
        /// <param name="bit"></param>
        public void RecibirUnBit (Bit bit)
        {
            this.entradas[(int)bit] = true; 
        }
    }
}
