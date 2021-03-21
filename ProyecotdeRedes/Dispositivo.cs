using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ProyecotdeRedes
{
    class Dispositivo
    {
        Puerto []  puertos;

        protected string name; 

        public Dispositivo (string name , int cantidaddepuertos )
        {
            this.name = name;
            this.puertos = new Puerto[cantidaddepuertos];
            for (int i = 0; i < cantidaddepuertos; i++)
            {
                this.puertos[i] = new Puerto(); 
            }
        }

        public int NumerodePuertos
        {
            get => this.puertos.Length; 
        }

        public Puerto this [int i]
        {
            get => this.puertos[i];
            set => this.puertos[i]  = value;
        }

        public string Name
        {
            get => this.name;
            set => this.name = value; 
        }

        public Bit BitDeSalida (Bit bitentrada , Dispositivo dispositivodesdeelquesellama)
        {          
            foreach (var item in this.puertos)
            {
                if (dispositivodesdeelquesellama.Equals(item.DispositivoConectado)) continue;
                if (item.DispositivoConectado == null) continue; 
                Bit aux = item.DispositivoConectado.BitDeSalida(bitentrada,this); 
                if (Bits.HuboColicion(bitentrada, aux))
                    return aux;
            }
            return bitentrada;
        }

        public void EscribirEnLaSalida(string recibo)
        {

            string rutaCompleta = Path.Join(DirectorioDeSalida(),this.name + ".txt");
           
            using (StreamWriter mylogs = File.AppendText(rutaCompleta))         //se crea el archivo
            {
                //se adiciona alguna información y la fecha

                mylogs.WriteLine(recibo);

                mylogs.Close();
            }
        }


        string DirectorioDeSalida()
        {
            var CurrentDirectory = Environment.CurrentDirectory;
            var parent = Directory.GetParent(Directory.GetParent(Directory.GetParent(CurrentDirectory).FullName).FullName);
            return Path.Join(parent.FullName, "output");

        }
    }
}
