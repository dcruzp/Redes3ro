using ProyecotdeRedes.Auxiliaries;
using ProyecotdeRedes.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using Action = ProyecotdeRedes.Component.Action;
using Byte = ProyecotdeRedes.Component.Byte;

namespace ProyecotdeRedes.Devices
{
    public class Puerto:IPuertos
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
        /// Esto es para poner los bit de salida que hay en esta computadora 
        /// </summary>
        Bit _outBit;


        /// <summary>
        /// Esto es para representar los bits de entrada por el puerto 
        /// </summary>
        //Queue<Bit> queueinput;


        /// <summary>
        /// Esto es una cola para representar los bit de salidas por el puerto 
        /// </summary>
        Queue<Bit> queueoutput; 


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
        
        /// <summary>
        /// Esta es la dirección Mac del dispositivo que esta conectada
        /// a este dispositivo
        /// </summary>
        public string DirMac
        {
            get => this._dirMac;
        }

        public bool PutMacDirection (string dirMac)
        {
            if (!CheckMetods.CheckIsOkDirMac(dirMac))
                return false;

            this._dirMac = dirMac;
            return true; 
        }

        //public void DesconectarElPuerto()
        //{
        //    //this.puertoalqueestaconectado = null;
        //    this._outBit = Bit.none;
        //    this.estaConectado = false;

        //    LimpiarEntradas(); 
        //}

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
            this.queueoutput = new Queue<Bit>();
            this._history = new List<OneBitPackage>(); 

            entradas = new bool[Enum.GetNames(typeof(Bit)).Length];
        }


        public void SendData (List<Bit> datatosend)
        {
            foreach (var item in datatosend)
            {
                this.queueoutput.Enqueue(item);
            }
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
        /// el time_sending me representa el tiempo en milisegundos que 
        /// se ha estado transmitiendo el bit que esta al principio de la 
        /// cola y que debe permanecer transmitiendose una cantidad de milisegundos 
        /// igual al signal_time para que sea considerado como enviado 
        /// </summary>
        private int time_sending = 0;



        private List<OneBitPackage> _history;

        private int time_received = 0;

        private int time_received_byte = 0; 

        private Bit AuxInBit = Bit.none;


        /// <summary>
        /// Esto es para actualizar y hacer todo las acciones 
        /// para procesar los datos que se tienen hasta el momento 
        /// </summary>
        public void UpdateInBit()
        {
            this.InBit = this.GiveMeInBit();

            if (this.InBit == Bit.none)
            {
                this.AuxInBit = Bit.none;
                this.time_received = 0;
                this.time_received_byte = 0; 
                return;
            }

            if (this.AuxInBit == Bit.none)
            {
                this.AuxInBit = this.InBit;
            }

            if (this.AuxInBit == this.InBit)
            {
                this.time_received++;
            }
            else
            {
                this.AuxInBit = this.InBit;
                this.time_received = 0;
                this.time_received_byte = 0; 
            }

            if (this.time_received >= Program.signal_time)
            {
                OneBitPackage bitreceived = new OneBitPackage(
                    Program.current_time,
                    Action.Received,
                    this.InBit);

                

                this._history.Add(bitreceived);

                time_received_byte++; 

                if (time_received_byte == 8 )
                {

                    OneBytePackage oneBitPackage = BuildBytePackage();

                    
                    
                    Console.WriteLine($"{this.id_puerto} received the byte: {oneBitPackage.ToString()} in time: {Program.current_time}");
                    time_received_byte = 0;

                    this.DispPertenece.BytesReceives.Add(oneBitPackage);
                    this.DispPertenece.ProcessDataReceived();
                }
                                          

                this.time_received = 0;
                this.AuxInBit = Bit.none;
            }
        }


        /// <summary>
        /// Est oes para contruir una instancia de OneBytePackage con los 
        /// ultimos 8 bits recibidos por este puerto. Como el Byte es contruido 
        /// en el instant en que se llama a la funcion entonces es asume que el 
        /// tiempo recepcion del Byte es el momento actual cuando se llama al metodo
        /// </summary>
        /// <returns></returns>
        private OneBytePackage BuildBytePackage ()
        {
            Bit[] bits = this._history.TakeLast(8).Select(x => x.Bit).ToArray();

            Byte @byte = new Byte(bits);

            OneBytePackage oneBytePackage = new OneBytePackage();
            oneBytePackage.Byte = @byte;
            oneBytePackage.portreceived = this.id_puerto;
            oneBytePackage.time_received = Program.current_time;

            return oneBytePackage;
        }

        /// <summary>
        /// Este metodo es para actualizar el bit de la salida del 
        /// puerto. Este metodo mantine el bit de salida 10 ms en 
        /// la salida y cuando se termina de transmitir este bit 
        /// entonces se procede a enviar el proximo en la cola de 
        /// salida
        /// </summary>
        public void UpdateOutBit()
        {

            if (this.queueoutput.Count == 0 || this._cable==null)
            {
                this.OutBit = Bit.none;
            }
            else
            {

                //if (this.tiempoesperandoparavolveraenviar > 0)
                //{
                //    this.tiempoesperandoparavolveraenviar--;
                //    this.bitsalida = Bit.none;
                //    return;
                //}

                this.OutBit = queueoutput.Peek();

                time_sending++;

                if (time_sending >= Program.signal_time)
                {
                    this.time_sending = 0;

                    OneBitPackage oneBitPackage = new OneBitPackage(
                                                                    time: Program.current_time,
                                                                    action: Action.Send,
                                                                    bit: this.OutBit,
                                                                    actionResult: ActionResult.Ok);

                    string salida = this.id_puerto + " ---> " +  oneBitPackage.ToString(); 

                    Console.WriteLine(salida);

                    _history.Add(oneBitPackage);           
                    
                    this.queueoutput.Dequeue();
                }
            }
        }

       
       
        public Bit GiveMeOutBit()
        {
            return this.OutBit;
        }

        public Bit GiveMeInBit()
        {
            if (this._cable == null) return Bit.none; 

            Puerto otherPort  = this._cable.puerto1.Equals(this) ?
                this._cable.puerto2:
                this._cable.puerto1;
            return otherPort.GiveMeOutBit();
        }

        public Cable Cable {
            set => this._cable = value;
        }
    }
}
