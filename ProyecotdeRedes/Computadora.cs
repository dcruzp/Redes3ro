using System;
using System.Collections.Generic;
using System.Text;

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


        uint tiempoEnElQuEmpezoAEnviar;


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

        public void ActualizarElBitDeSalida()
        {
            if (this.puertos[0] == null || !this.puertos[0].EstaConectadoAOtroDispositivo) 
                return;

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

        public override void ProcesarInformacionDeSalidaYDeEntrada()
        {
            if (this.puertos[0] == null || !this.puertos[0].EstaConectadoAOtroDispositivo)
                return;

            bool hubocolision = HuboUnaColision();

            
            if (this.BitdeSalida != Bit.none && hubocolision)
            {
                EscribirEnLaSalida(string.Format("{0} {1} send {2} collision", Program.current_time, this.Name, (int)this.BitdeSalida));
                Actualizar();
                base.LimpiarLosParametrosDeEntrada(); 
                return; 
            }
            else if (this.BitdeSalida != Bit.none)
            {
                EscribirEnLaSalida(string.Format("{0} {1} send {2} Ok", Program.current_time, this.Name, (int)this.BitdeSalida));
            }
            
            if (this.BitdeEntrada != Bit.none)
            {
                EscribirEnLaSalida(string.Format("{0} {1} receive {2} Ok", Program.current_time, this.Name, (int)this.BitdeEntrada));
            }

            base.LimpiarLosParametrosDeEntrada();
        }
    }
}
