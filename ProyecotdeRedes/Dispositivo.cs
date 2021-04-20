using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ProyecotdeRedes
{
    class Dispositivo
    {

        protected Dispositivo[] dispositivosConectados;

        protected Puerto[] puertos; 

        protected string name;
        protected bool[,] entradas; 

        protected Bit bitsalida;
        protected Bit bitentrada;

        protected int indice;
        protected int cantidaddepuertos; 

        public Dispositivo (string name , int cantidaddepuertos , int indice )
        {
            this.name = name;
            this.dispositivosConectados = new Dispositivo[cantidaddepuertos];
            this.bitsalida = Bit.none;
            this.bitentrada = Bit.none;
            this.indice = indice;
            this.cantidaddepuertos = cantidaddepuertos; 
            this.entradas = new bool[this.cantidaddepuertos, Enum.GetNames(typeof(Bit)).Length];

            this.puertos = new Puerto[cantidaddepuertos];

            for (int i = 0; i < cantidaddepuertos; i++)
            {
                this.puertos[i] = new Puerto(this.name + $"_{i}" , i); 
            }
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

        public Puerto DameElPuerto(int id)
        {
            if (id >= this.cantidaddepuertos || id < 0) throw new IndexOutOfRangeException("El dispositivo no tiene el puerto que se le especifica");

            return this.puertos[id]; 
        }

        public IEnumerable<Puerto> PuertosConectados
        {
            get
            {
                foreach (var item in this.puertos)
                    if (item.EstaConectadoAOtroDispositivo)
                        yield return item;
            }
        }

        public IEnumerable<Dispositivo> DispositivosConectados
        {
            get
            {
                foreach (var item in this.puertos)                
                    if (item.DispositivoConectado != null) 
                        yield return item.DispositivoConectado;
            }
        }

        public void EscribirEnLaSalida(string recibo)
        {
            string rutaCompleta = Path.Join(DirectorioDeSalida(),this.name + ".txt");

            //se crea el archivo si no existe y lo abre si ya existe 
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

        public int PuertoPorElQueEstaConectado(Dispositivo disp)
        {
            for (int i = 0; i < this.dispositivosConectados.Length; i++)
            {
                if (disp.Equals(dispositivosConectados[i])) return i; 
            }
            return -1; 
        }


        public void recibirUnBit (int puerto, Bit bit)
        {
            this.entradas[puerto, (int)bit] = true;
        }

        public bool HuboUnaColision()
        {
            this.bitentrada = Bit.none;

            bool cero = false , uno = false; 
            
            for (int i = 0; i < this.cantidaddepuertos; i++)
            {
                if (entradas[i,(int)Bit.uno]) uno =true ;
                if (entradas[i, (int)Bit.cero]) cero = true; 
            }
            if (uno && cero) return true;
            else if (uno) this.bitentrada = Bit.uno;
            else if (cero) this.bitentrada = Bit.cero;

            if (this is Computadora && bitentrada!= Bit.none && bitentrada != bitsalida) return true; 
            else return false; 
        }

        public virtual void ProcesarInformacionDeSalidaYDeEntrada()
        {
            bool hubocolision = HuboUnaColision();

            if (hubocolision) return;

            if (this.bitentrada == Bit.none) return; 

            for (int i = 0; i < this.cantidaddepuertos; i++)
            {
                if (this.entradas[i, (int)Bit.cero]) EscribirEnLaSalida(string.Format("{0} {1} receive {2} ", Program.current_time, this.Name + $"_{i + 1}", (int)Bit.cero));
                else if (this.entradas[i, (int)Bit.uno]) EscribirEnLaSalida(string.Format("{0} {1} receive {2} ", Program.current_time, this.Name + $"_{i + 1}", (int)Bit.uno));
                else EscribirEnLaSalida(string.Format("{0} {1} send {2} ", Program.current_time, this.Name + $"_{i + 1}", (int)this.BitdeEntrada));
            }

            LimpiarLosParametrosDeEntrada();
        }

        protected void LimpiarLosParametrosDeEntrada()
        {
            for (int i = 0; i < this.entradas.GetLength(0); i++)
            {
                for (int j = 0; j < this.entradas.GetLength(1); j++)
                {
                    this.entradas[i, j] = false; 
                }
            }
            this.BitdeEntrada = Bit.none; 
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append($"{this.name} \n");

            stringBuilder.Append($"Bit de Salida:\t {(int)this.BitdeSalida}\n");

            stringBuilder.Append($"Bit de Entradas:\n"); 
            for (int i = 0; i < this.entradas.GetLength(0); i++)
            {
                for (int j = 0; j < this.entradas.GetLength(1); j++)
                {

                    stringBuilder.Append($"{ (this.entradas[i, j] == true ?"T" : "F")}  ");
                }
                stringBuilder.Append(Environment.NewLine);
            }


            stringBuilder.Append(Environment.NewLine);

            return stringBuilder.ToString();
        }

    }
}
