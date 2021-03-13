﻿using System;
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


        public Computadora(string name) : base(name)
        {
            this.tiempoEnviando = 0;
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

        public void IntentarEnviar()
        {
            if (porenviar.Count == 0) return;

            Bit bitleido = this.puerto.DispositivoConectado.puerto.BitdeSalida;
            Bit bitparaenviar = this.porenviar.Peek();

            Bit xor = Bits.XOR(bitleido, bitparaenviar); 

            if (xor == Bit.cero)
            {
                //Hubo colicion
                this.puerto.BitdeSalida = Bit.none;
            }
            else
            {
                //No hubo colicion
                if (tiempoEnviando > Program.signal_time)
                {
                    this.tiempoEnviando = 0;
                    this.tiempoEnElQuEmpezoAEnviar = Program.current_time;
                    this.porenviar.Dequeue(); 
                }
                this.puerto.BitdeSalida = porenviar.Peek();
            }
        }
        
        public void Recibir()
        {
            this.recibidos.Add(this.puerto.DispositivoConectado.puerto.BitdeSalida);
        }
    }
}