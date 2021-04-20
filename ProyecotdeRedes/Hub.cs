using System;
using System.Collections.Generic;
using System.Text;

namespace ProyecotdeRedes
{
    class Hub:Dispositivo 
    {

        Bit[] bitsdeentradas; 
        public Hub(string name ,int indice):base (name , 4,indice)
        {
            this.bitsdeentradas = new Bit[4];
        }

        public Hub (string name , int numerodepuertos , int indice):base(name , numerodepuertos,indice)
        {
            if (numerodepuertos < Program.cantidadminimadepuertosdeunhub || numerodepuertos > Program.cantidadmaximadepuertosdeunhub)
            {
                throw new IndexOutOfRangeException("No se puede tener un hub con menos de 4 puertos o mas de 8 "); 
            }
            this.bitsdeentradas = new Bit[numerodepuertos];
        }

        public Bit [] BitsDeEntrada
        {
            get => this.bitsdeentradas;
        }

        //public void ActualizarlaEntrada()
        //{
        //    this.BitdeSalida = Bit.none;
        //    bool hubounacolicion = false; 
        //    for (int i = 0; i < this.bitsdeentradas.Length; i++)
        //    {
        //        bitsdeentradas[i] = Bit.none;

        //        if (this[i] == null) continue;

        //        if (this[i] is Computadora)
        //        {
        //            Computadora comp = this[i] as Computadora;

        //            if (comp.BitdeSalida == Bit.none)
        //            {
        //                continue;
        //            }
        //            else
        //            {
        //                this.bitsdeentradas[i] = comp.BitdeSalida; 
        //            }
        //            continue; 
        //        }



        //        bool[] mask = new bool[Program.dispositivos.Count];
        //        mask[this.Indice] = true;

        //        Queue<Dispositivo> queue = new Queue<Dispositivo>();
        //        queue.Enqueue(this[i]);
        //        mask[this[i].Indice] = true;

        //        Dispositivo curr;

        //        while (queue.Count>0)
        //        {
        //            curr = queue.Dequeue();

        //            foreach (var item in curr.DispositivosConectados)
        //            {
        //                if (mask[item.Indice]) continue; 

        //                if (item is Computadora)
        //                {
        //                    Computadora comp = item as Computadora;
        //                    if (comp.BitdeSalida == Bit.none)
        //                    {
        //                        continue;
        //                    }
        //                    if (bitsdeentradas[i] == Bit.none)
        //                    {
        //                        bitsdeentradas[i] = comp.BitdeSalida;
        //                    }
        //                    else if (comp.BitdeSalida != bitsdeentradas[i])
        //                    {
        //                        hubounacolicion = true;
        //                    }
        //                }
        //                queue.Enqueue(item);
        //            }

        //        }   
        //    }

        //    if (hubounacolicion  || Haydiferentesbitdeentrada())
        //    {
        //        for (int i = 0; i < this.bitsdeentradas.Length; i++)
        //        {
        //            this.bitsdeentradas[i] = Bit.none; 
        //        }
        //    }
        //}

        //private bool Haydiferentesbitdeentrada ()
        //{
        //    Bit bit = Bit.none;
        //    foreach (var item in this.bitsdeentradas)
        //    {

        //        if (item != Bit.none)
        //        {
        //            if (bit == Bit.none) bit = item;
        //            else if (bit != item) return true; 
        //        }
        //    }
        //    this.BitdeSalida = bit; 
        //    return false; 
        //}
    }
}
