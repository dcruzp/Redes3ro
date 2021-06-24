using ProyecotdeRedes.Auxiliaries;
using ProyecotdeRedes.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        /// Esta es la dirección Max representada en hexadecimal 
        /// </summary>
        string direccionMax;


        
        

       

        /// <summary>
        /// esto es para determinar el tiempo que ha estado la
        /// computadora sin enviar información producto de una 
        /// colisión que detecto anteriormente
        /// </summary>
        uint tiempoesperandoparavolveraenviar; 

        public Computadora(string name ,int indice) : base(name ,1, indice)
        {           
            this.porenviar = new Queue<Bit>();
            this.direccionMax = null;
        }


        /// <summary>
        /// Esto es para darle una dirección Mac a la computadora 
        /// El parámetro es un string hexadecimal de 4 dígitos
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
                throw new InvalidCastException($"No se pudo asignar la dirección Mac {dirMac} a la computadora {this.name}"); 
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
           
        }
        
        public void send_frame (string mac , string data)
        {
            var packagetosend = new List<Bit>();

            var dirmaxin = string.IsNullOrEmpty(mac) ? "FFFF" : mac;

            var dirmacout = string.IsNullOrEmpty(this.direccionMax) ? "FFFF" : this.direccionMax;

            var lenghtdata = (data.Length / 2).ToString("X");

            while (lenghtdata.Length < 2) lenghtdata = "0" + lenghtdata;

            packagetosend = AuxiliaryFunctions.ConvertToListOfBitHexadecimalSequence(dirmaxin, dirmacout, lenghtdata, "00", data);

            //packagetosend.AddRange(packagetosend);

            foreach (var item in puertos)
            {
                item.SendData(packagetosend);
            }

            Console.WriteLine($"Enviado por la computadora '{this.name}' el " +
                $"paquete {AuxiliaryFunctions.FromByteDataToHexadecimal(packagetosend)}");


        }


       

       
        /// <summary>
        /// Este método se llama cuando hubo una instrucción 
        /// send para el envío de un paquete de bits
        /// </summary>
        /// <param name="paquete"></param>
        public void send(List<Bit> paquete)
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
            //if (this.puertos[0] == null || !this.puertos[0].EstaConectadoAOtroDispositivo)
            //    return false; 
            
            //ActualizarElBitDeSalida();
            EnviarElBitQueHayEnLaSalidaALasDemasComputadoras();

            return true; 
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
                    Dispositivo dispconectado = item.giveMeDisposotivoConectado;
                    if (mask[dispconectado.Indice]) continue;

                    int puertoporelqueestaconectado = item.NumeroPuertoAlQueEstaConectado;
                   
                    //dispconectado.recibirUnBit(puertoporelqueestaconectado, this.bitsalida); 
                    
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

            
            //if (this.BitdeSalida != Bit.none && hubocolision)
            //{
            //    //this._sendAndReceived.Enqueue(new OneBitPackage(
            //    //                                                 Program.current_time,
            //    //                                                 Action.Send,
            //    //                                                 this.BitdeSalida,
            //    //                                                 actionResult: ActionResult.Collision));
                
            //    Actualizar();
            //    return; 
            //}
            //else if (this.BitdeSalida != Bit.none)
            //{
            //    //this._sendAndReceived.Enqueue(new OneBitPackage(
            //    //                                                 Program.current_time,
            //    //                                                 Action.Send,
            //    //                                                 this.BitdeSalida,
            //    //                                                 ActionResult.Ok));

                
            //}
            
            //if (this.BitdeEntrada != Bit.none)
            //{
            //    //this._sendAndReceived.Enqueue(new OneBitPackage (
            //    //                                                Program.current_time,
            //    //                                                Action.Received);
            //}

        }


        /// <summary>
        /// Esto es para imprimir los bit que recibió
        /// el dispositivo
        /// </summary>
        //public void PrintReceivedBits()
        //{
        //    StringBuilder stringBuilder = new StringBuilder(); 
            
        //    //Console.Write($"{this.name.ToUpper()}\n");

        //    foreach (var item in _sendAndReceived)
        //    {
        //        stringBuilder.Append(item.ToString());
        //        stringBuilder.Append(Environment.NewLine); 
        //        //Console.WriteLine(item.ToString());
        //    }

        //    EscribirEnLaSalida(stringBuilder.ToString());
        //    //Console.WriteLine(stringBuilder.ToString());

        //    //Console.WriteLine();
        //}

        public override void ProcessDataReceived()
        {
            base.ProcessDataReceived();

            if (currentBuildInFrame.FullData)
            {
                _history.Add(currentBuildInFrame);
                WriteDataReceivedInOutput(currentBuildInFrame);
                currentBuildInFrame = null;
            }
        }

        private void WriteDataReceivedInOutput(DataFramePackage package)
        {
            uint time_received = package.TimeReceived;
            string pcout = AuxiliaryFunctions.FromByteDataToHexadecimal(package.MacOut);
            string data = AuxiliaryFunctions.FromByteDataToHexadecimal(package.Data);

            string dataFrame = package.ToString();

            this.EscribirEnLaSalida(dataFrame , this.name + "_data.txt");
        }
    }
}
