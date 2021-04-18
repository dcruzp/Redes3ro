using System;
using System.Collections.Generic;
using System.Text;

namespace ProyecotdeRedes
{
    class Computadora:Dispositivo
    {
        Queue<Bit> porenviar;
        
        bool [] recibidos; 

        uint tiempoEnviando;
        uint tiempoEnElQuEmpezoAEnviar;

        uint tiempoesperandoparavolveraenviar; 

        public Computadora(string name ,int indice) : base(name ,1, indice)
        {
            this.tiempoEnviando = 0;
            this.tiempoEnElQuEmpezoAEnviar = 0;
            this.tiempoEnElQuEmpezoAEnviar = 0; 
            this.porenviar = new Queue<Bit>();
            this.recibidos = new bool[Enum.GetNames(typeof(Bit)).Length]; 
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
            return this[0] == null; 
        }

        public void ActualizarElBitDeSalida()
        {
            if (this[0] == null) return; 

            if (this.porenviar.Count == 0)
                this.BitdeSalida = Bit.none;
            else
            {
               
                if (this.tiempoesperandoparavolveraenviar > 0)
                {
                    this.tiempoesperandoparavolveraenviar--;
                    this.BitdeSalida = Bit.none; 
                    return;
                }

                this.BitdeSalida = porenviar.Peek();

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

                foreach (var item in current.DispositivosConectados)
                {
                    if(mask[item.Indice]) continue;


                    int puertoporelqueestaconectado = item.PuertoPorElQueEstaConectado(current);

                    if (puertoporelqueestaconectado != -1)
                        item.recibirUnBit(puertoporelqueestaconectado, this.bitsalida);

                    //if(item is Computadora)
                    //{
                    //    Computadora comp = item as Computadora;

                    //    comp.recibirUnBit(0,this.bitsalida); 
                    //}

                    mask[item.Indice] = true;
                    queue.Enqueue(item); 
                }
            }
        }
    }
}
