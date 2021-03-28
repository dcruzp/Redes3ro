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
        Bit bitentrada;

        int indice; 

        public Dispositivo (string name , int cantidaddepuertos , int indice )
        {
            this.name = name;
            this.dispositivosConectados = new Dispositivo[cantidaddepuertos];
            this.bitsalida = Bit.none;
            this.bitentrada = Bit.none;
            this.indice = indice; 
        }


        public int Indice
        {
            get => this.indice;
        }

        public Bit BitdeSalida
        {
            get => this.bitsalida;
            set => this.bitsalida = value;
        }

        public Bit BitdeEntrada
        {
            get => this.bitentrada;
            set => this.bitentrada = value; 
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

            //se crea el archivo si no existe y lo abre y ya existe 
            using (StreamWriter mylogs = File.AppendText(rutaCompleta))      
            {
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
