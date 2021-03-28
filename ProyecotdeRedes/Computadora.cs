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

        public Computadora(string name ,int indice) : base(name ,1 , indice)
        {
            this.tiempoEnviando = 0;
            this.tiempoEnElQuEmpezoAEnviar = 0;
            this.tiempoEnElQuEmpezoAEnviar = 0; 
            this.porenviar = new Queue<Bit>();
            this.recibidos = new List<Bit>();
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
                this.BitdeSalida = porenviar.Peek();
                //EscribirEnLaSalida(string.Format("{0} {1} send {2} OK", Program.current_time, this.name, (int)this.BitdeSalida));

                tiempoEnviando++;

                if (tiempoEnviando >= Program.signal_time)
                {
                    this.tiempoEnviando = 0;
                    this.tiempoEnElQuEmpezoAEnviar = Program.current_time;
                    this.porenviar.Dequeue();
                    //this.puerto.BitdeSalida = Bit.none; 
                }
            }
        }

        public void IntentarEnviar()
        {
            if (porenviar.Count == 0)
            {
                this[0].BitdeSalida = Bit.none; 
                return;
            }

            if (this.tiempoesperandoparavolveraenviar > 0 )
            {
                this.tiempoesperandoparavolveraenviar--;
                return; 
            }
            
            
            if (Program.HuboUnaColicion(this))
            {
                //Hubo colisión
                EscribirEnLaSalida(string.Format("{0} {1} send {2} collision", Program.current_time, this.name, (int)this.BitdeSalida));
                this[0].BitdeSalida = Bit.none;
                this.tiempoEnviando = 0;
                
                this.tiempoesperandoparavolveraenviar = (uint)new Random().Next(20, 50); 
            }
            else
            {
                //No hubo colisión
                this.BitdeSalida = porenviar.Peek();
                EscribirEnLaSalida(string.Format("{0} {1} send {2} OK", Program.current_time, this.name, (int)this.BitdeSalida));

                tiempoEnviando++;

                if (tiempoEnviando >= Program.signal_time)
                {
                    this.tiempoEnviando = 0;
                    this.tiempoEnElQuEmpezoAEnviar = Program.current_time;
                    this.porenviar.Dequeue();
                    //this.puerto.BitdeSalida = Bit.none; 
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
