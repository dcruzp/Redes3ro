using ProyecotdeRedes.Auxiliaries;
using ProyecotdeRedes.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using Action = ProyecotdeRedes.Component.Action;

namespace ProyecotdeRedes
{
    class Computadora:Dispositivo
    {
        /// <summary>
        /// Esto es una cola para almacenar los bits que quedan 
        /// por enviar aun. Si la cola esta vacía es que no quedan 
        /// bit por enviar. 
        /// </summary>
        Queue<Bit> porenviar;
        

        /// <summary>
        /// Este es el tiempo que el bit que esta enviándose 
        /// ha estado transmitiéndose a las demás computadoras. 
        /// </summary>
        uint tiempoEnviando;


        /// <summary>
        /// Esta es la direccion Max representada en hexadecimal 
        /// </summary>
        string direccionMax;


        /// <summary>
        /// Datos que se han recibido 
        /// </summary>
        Queue<OneBitPackage> _sendAndReceived; 


        uint tiempoEnElQuEmpezoAEnviar;

        Bit bitReceived;

        uint timeReceivedTheInputBit; 

        /// <summary>
        /// esto es para determinar el tiempo que ha estado la
        /// computadora sin enviar información producto de una 
        /// colisión que detecto anteriormente
        /// </summary>
        uint tiempoesperandoparavolveraenviar; 

        public Computadora(string name ,int indice) : base(name ,1, indice)
        {
            this.tiempoEnviando = 0;
            this.tiempoEnElQuEmpezoAEnviar = 0;
            this.porenviar = new Queue<Bit>();
            this.direccionMax = null;
            this._sendAndReceived = new Queue<OneBitPackage>();
            this.bitReceived = Bit.none;
            this.timeReceivedTheInputBit = 0; 
        }


        /// <summary>
        /// Esto es para darle una direccion Mac a la computadora 
        /// El paramentro es un string hexadecimal de 4 digitos
        /// </summary>
        /// <param name="dirMac"></param>
        public void PutMacDirection (string dirMac)
        {
            if (direccionMax == null && CheckMetods.CheckIsOkDirMac(dirMac))
            {
                this.direccionMax = dirMac;
            }
            else
            {
                throw new InvalidCastException($"No se pudo asignar la direccion Mac {dirMac} a la computadora {this.name}"); 
            }
        }

        /// <summary>
        /// Este método se llama cuando se detecta una colisión 
        /// Esto es para darle un tiempo random a la computadora 
        /// para que espere anted de volver a enviar un bit
        /// </summary>
        public void Actualizar()
        {
            this.tiempoesperandoparavolveraenviar = (uint)new Random().Next(5,50);
            Console.WriteLine($"{this.name}  va a esperar {this.tiempoesperandoparavolveraenviar} para volver a enviar un dato");
            this.tiempoEnviando = 0;
        }
        
        public void send_frame (string mac , string data)
        {
            var packagetosend = new List<Bit>();

            var dirmaxin = string.IsNullOrEmpty(mac) ? "FFFF" : mac;

            var dirmacout = string.IsNullOrEmpty(this.direccionMax) ? "FFFF" : this.direccionMax;

            var lenghtdata = (data.Length / 2).ToString("X");

            while (lenghtdata.Length < 2) lenghtdata = "0" + lenghtdata;

            packagetosend = PackageToSend(dirmaxin, dirmacout, lenghtdata, "00", data);

            foreach (var item in packagetosend)
            {
                this.porenviar.Enqueue(item); 
            }
        }




        /// <summary>
        /// Esto retorna dado dos string que representa las direccion mac 
        /// y la data a enviar en formato hexadecimal y retorna una lista 
        /// de bits donde esta representado el frame que se quiere enviar  
        /// </summary>
        /// <param name="mac"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private List<Bit>  PackageToSend (params string [] datainHex)
        {
            var package = new List<Bit>();

            foreach (var item in datainHex)
            {
                package.AddRange(AuxiliaryFunctions.convertFromHexStrToBitArray(item));
            }

            return package; 
        }

        public void updateDataReceived ()
        {
            if (this.bitentrada == Bit.none)
            {
                this.bitReceived = Bit.none;
                this.timeReceivedTheInputBit = 0;
                return; 
            }

            if (this.bitReceived == Bit.none) 
                this.bitReceived = this.bitentrada;

            if (this.bitReceived == this.bitentrada)
            {
                this.timeReceivedTheInputBit++;
            }
            else
            {
                this.bitReceived = this.bitentrada;
                this.timeReceivedTheInputBit = 0; 
            }
            
            if (this.timeReceivedTheInputBit == Program.signal_time)
            {
                OneBitPackage bitreceived = new OneBitPackage(
                    Program.current_time,
                    ActionResult.Received,
                    this.bitentrada);

                this._sendAndReceived.Enqueue(bitreceived);
                this.timeReceivedTheInputBit = 0;
                this.bitReceived = Bit.none; 
            }
        }


       
        /// <summary>
        /// Este método se llama cuando hubo una instrucción 
        /// send para el envío de un paquete de bits
        /// </summary>
        /// <param name="paquete"></param>
        public void send(Bit [] paquete)
        {
            foreach (var item in paquete)
            {
                porenviar.Enqueue(item);
            }
        }


        /// <summary>
        /// Este método se llama inicialmente antes de cualquier otro 
        /// llamado en un mili segundo , y lo que hace básicamente es determinar 
        /// que bit se va a enviar y Enviárselo a las demás computadoras que 
        /// están conectadas en la red que se encuentra la de esta instancia
        /// </summary>
        /// <returns></returns>
        public bool EnviarInformacionALasDemasComputadoras()
        {
            if (this.puertos[0] == null || !this.puertos[0].EstaConectadoAOtroDispositivo)
                return false; 
            
            ActualizarElBitDeSalida();
            EnviarElBitQueHayEnLaSalidaALasDemasComputadoras();

            return true; 
        }


        /// <summary>
        /// Este método pone el bit de salida que le corresponde
        /// es ese mili segundo estar en la salida . Este método se 
        /// llama una sola vez en cada mili segundo de ejecución del 
        /// programa
        /// </summary>
        public void ActualizarElBitDeSalida()
        {
            if (this.puertos[0] == null || !this.puertos[0].EstaConectadoAOtroDispositivo)  return;

            if (this.porenviar.Count == 0)
            {
                this.bitsalida = Bit.none;
            }
            else
            {

                if (this.tiempoesperandoparavolveraenviar > 0)
                {
                    this.tiempoesperandoparavolveraenviar--;
                    this.bitsalida = Bit.none;
                    return;
                }

                this.bitsalida = porenviar.Peek();

                tiempoEnviando++;

                if (tiempoEnviando >= Program.signal_time)
                {
                    this.tiempoEnviando = 0;
                    this.tiempoEnElQuEmpezoAEnviar = Program.current_time;
                    this.porenviar.Dequeue();
                }
            }
        }


        /// <summary>
        /// Este método se llama después de haber llamado al método
        ///  ActualizarElBitDeSalida() para que este pueda se enviado 
        ///  con el procedimiento de este método a las demás computadoras 
        ///  que están conectadas a la que representa esta instancia, 
        ///  (aquí lo que se usa es un bfs para enviar el bit a cada computadora)
        /// </summary>
        public void EnviarElBitQueHayEnLaSalidaALasDemasComputadoras()
        {
            Queue<Dispositivo> queue = new Queue<Dispositivo>();
            bool[] mask = new bool[Program.dispositivos.Count];
            mask[this.indice]= true; 
            queue.Enqueue(this);

            Dispositivo current; 

            while(queue.Count>0)
            {
                current = queue.Dequeue();

                foreach (var item in current.PuertosConectados)
                {
                    Dispositivo dispconectado = item.DispositivoConectado;
                    if (mask[dispconectado.Indice]) continue;

                    int puertoporelqueestaconectado = item.NumeroPuertoAlQueEstaConectado;
                   
                    dispconectado.recibirUnBit(puertoporelqueestaconectado, this.bitsalida); 
                    
                    mask[dispconectado.Indice] = true;
                    queue.Enqueue(dispconectado); 
                }
            }
        }

        /// <summary>
        /// Este método es llamado para una vez que se establecieron las
        /// salidas y entradas de datos a esta computadora puedan ser 
        /// procesados estos datos y determinar si hubo una colisión 
        /// y escribir en la salida del dispositivo los datos 
        /// de salida  correspondiente a esta computadora.
        /// </summary>
        public override void ProcesarInformacionDeSalidaYDeEntrada()
        {
            if (this.puertos[0] == null || !this.puertos[0].EstaConectadoAOtroDispositivo)
                return;

            bool hubocolision = HuboUnaColision();

            
            if (this.BitdeSalida != Bit.none && hubocolision)
            {
                this._sendAndReceived.Enqueue(new OneBitPackage(
                                                                 Program.current_time,
                                                                 Action.Send,
                                                                 this.BitdeSalida));
                //EscribirEnLaSalida(string.Format("{0} {1} send {2} collision", Program.current_time, this.Name, (int)this.BitdeSalida));
                Actualizar();
                return; 
            }
            else if (this.BitdeSalida != Bit.none)
            {

                //EscribirEnLaSalida(string.Format("{0} {1} send {2} Ok", Program.current_time, this.Name, (int)this.BitdeSalida));
            }
            
            if (this.BitdeEntrada != Bit.none)
            {
                //EscribirEnLaSalida(string.Format("{0} {1} receive {2} Ok", Program.current_time, this.Name, (int)this.BitdeEntrada));
            }

        }


        public void PrintReceivedBits ()
        {
            Console.Write($"\n{this.name.ToUpper()} ");

            foreach (var item in _sendAndReceived)
            {
                Console.WriteLine(item.ToString());
            }

            Console.WriteLine();
        }
    }
}
