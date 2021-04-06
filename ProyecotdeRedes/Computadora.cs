using System;
using System.Collections.Generic;
using System.Text;

namespace ProyecotdeRedes
{
    class Computadora:Dispositivo
    {
        Queue<Bit> porenviar;
        List<Bit> recibidos; 

        uint tiempoEnviando;
        uint tiempoEnElQuEmpezoAEnviar;
        uint tiempoesperandoparavolveraenviar; 

        public Computadora(string name ,int indice) : base(name ,1, indice)
        {
            this.tiempoEnviando = 0;
            this.tiempoEnElQuEmpezoAEnviar = 0;
            this.tiempoEnElQuEmpezoAEnviar = 0; 
            this.porenviar = new Queue<Bit>();
            this.recibidos = new List<Bit>();
        }

        public void Actualizar()
        {
            this.tiempoesperandoparavolveraenviar = (uint)new Random().Next(2 , 20);
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

        public void ActualizarElBitDeSalida()
        {
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

        public void Recibir()
        {
            //if (this[0].DispositivoConectado.BitdeSalida != Bit.none)
            //{
            //    EscribirEnLaSalida(string.Format("{0} {1} receive {2}", Program.current_time, this.name, (int)this[0].DispositivoConectado[0].BitdeSalida));
            //}
        }

        
    }
}
