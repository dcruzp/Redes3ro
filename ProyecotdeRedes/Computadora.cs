using System;
using System.Collections.Generic;
using System.Text;

namespace ProyecotdeRedes
{
    class Computadora:Dispositivo
    {
        Queue<Bit> porenviar;
        
        uint tiempoEnviando;
        uint tiempoEnElQuEmpezoAEnviar;

        uint tiempoesperandoparavolveraenviar; 

        public Computadora(string name ,int indice) : base(name ,1, indice)
        {
            this.tiempoEnviando = 0;
            this.tiempoEnElQuEmpezoAEnviar = 0;
            this.porenviar = new Queue<Bit>();
        }

        public void Actualizar()
        {
            this.tiempoesperandoparavolveraenviar = (uint)new Random().Next(5,50);
            Console.WriteLine($"{this.name}  va a esperar {this.tiempoesperandoparavolveraenviar} para volver a enviar un dato");
            this.tiempoEnviando = 0;
        }
        
       

        public void send(Bit [] paquete)
        {
            foreach (var item in paquete)
            {
                porenviar.Enqueue(item);
            }
        }

        public bool NoEstaConectada()
        {
            // return this[0] == null;
            return this.PuertosConectados == null; 
        }

        public bool EnviarInformacionALasDemasComputadoras()
        {
            //if (this.dispositivosConectados[0] == null)
            //    return false; 
            
            ActualizarElBitDeSalida();
            EnviarElBitQueHayEnLaSalidaALasDemasComputadoras();

            return true; 
        }

        public void ActualizarElBitDeSalida()
        {
            //if (this[0] == null) return;

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
