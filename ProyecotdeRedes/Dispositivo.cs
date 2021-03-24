using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ProyecotdeRedes
{
    class Dispositivo
    {
        //Puerto []  puertos;

        Dispositivo[] dispositivosConectados; 

        protected string name;

        Bit bitsalida; 

        public Dispositivo (string name , int cantidaddepuertos )
        {
            this.name = name;
            this.dispositivosConectados = new Dispositivo[cantidaddepuertos];
        }

        public Bit BitdeSalida
        {
            get => this.bitsalida;
            set => this.bitsalida = value;
        }

        public int NumerodePuertos
        {
            get => this.dispositivosConectados.Length; 
        }

        public Dispositivo this [int i]
        {
            get => this.dispositivosConectados[i];
            set => this.dispositivosConectados[i]  = value;
        }

        public string Name
        {
            get => this.name;
            set => this.name = value; 
        }

        public IEnumerable<Dispositivo> DispositivosConectados
        {
            get => this.dispositivosConectados; 
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

        public Bit HuboColicion(Dispositivo dispositivodesdeelquesepregunta)
        {
            foreach (var item in this.DispositivosConectados)
            {
                if (item != null ||  item.Equals(dispositivodesdeelquesepregunta))
                {
                    continue; 
                }

                Bit bitparapreguntar = item.HuboColicion(this);

                if (bitparapreguntar == Bit.none)
                {
                    continue;
                }

                if (bitparapreguntar != dispositivodesdeelquesepregunta.BitdeSalida)
                {
                    return bitparapreguntar == Bit.uno ? Bit.cero : Bit.uno; 
                }
            }

            return dispositivodesdeelquesepregunta.BitdeSalida; 
        }
    }
}
