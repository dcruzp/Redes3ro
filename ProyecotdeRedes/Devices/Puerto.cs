using ProyecotdeRedes.Component;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProyecotdeRedes
{
    public class Puerto
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
        /// Esto es para poner los bit de salida que hay en esta computadora 
        /// </summary>
        Bit _outBit;


        Bit _inBit; 


        Dispositivo _dispPertenece;


        string _dirMac; 

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

       

        /// <summary>
        /// Esto te retorna el indice del puerto al que esta conectado 
        /// el dispositivo por el puerto actual 
        /// </summary>
        public int NumeroPuertoAlQueEstaConectado
        {
            get => this.giveMePuertoConectado.numero_puerto;
        }

        /// <summary>
        /// Esto te desconecta el puerto actual de cualquier otro puerto 
        /// al que se encuentre conectado 
        /// </summary>

        protected ICable _cable;  
        
        public string DirMac
        {
            get => this._dirMac;
            set => this._dirMac = value; 
        }

        public void DesconectarElPuerto()
        {
            this.puertoalqueestaconectado = null;
            this._outBit = Bit.none;
            this.estaConectado = false;

            LimpiarEntradas(); 
        }

        public int NumeroPuerto
        {
            get => this.numero_puerto;
        }

        /// <summary>
        /// este es el constructor del puerto con su nombre y su id
        /// </summary>
        /// <param name="id_puerto"></param>
        /// <param name="numero_puerto"></param>
        public Puerto (string id_puerto , int numero_puerto,Dispositivo dispositivo)
        {
            this.id_puerto = id_puerto;
            this._outBit = Bit.none;
            this.numero_puerto = numero_puerto;
            this._cable = null;
            this._dispPertenece = dispositivo;

            entradas = new bool[Enum.GetNames(typeof(Bit)).Length];
        }

        public Dispositivo DispPertenece
        {
            get => this._dispPertenece;
        }

        public Puerto giveMePuertoConectado
        {
            get
            {
                return _cable.puerto1.Equals(this) ?
                    _cable.puerto2 : _cable.puerto1;
            }
        }

        public Dispositivo giveMeDisposotivoConectado
        {
            get
            {
                Puerto puertoconectado = _cable.puerto1.Equals(this) ?
                    _cable.puerto2 : _cable.puerto1;

                return puertoconectado._dispPertenece;
            }

        }

        public bool ConnectCableToPort (Cable cable)
        {
            if (_cable != null )
            {
                return false; 
            }
            _cable = cable;
            return true; 
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

        public Bit OutBit
        {
            get => this._outBit;
            set => this._outBit = value;
        }


        public Bit InBit
        {
            get => this._inBit;
            set => this._inBit = value; 
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
        /// Esto actualiza y pone en las entradas el bit que se recibió
        /// </summary>
        /// <param name="bit"></param>
        public void RecibirUnBit (Bit bit)
        {
            this.entradas[(int)bit] = true; 
        }
    }
}
