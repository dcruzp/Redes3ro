using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ProyecotdeRedes
{
    class Dispositivo
    {
        protected Puerto[] puertos; 

        protected string name;       

        protected Bit bitsalida;
        protected Bit bitentrada;

        protected int indice;
        protected int cantidaddepuertos; 

        public Dispositivo (string name , int cantidaddepuertos , int indice )
        {
            this.name = name;            
            this.bitsalida = Bit.none;
            this.bitentrada = Bit.none;
            this.indice = indice;
            this.cantidaddepuertos = cantidaddepuertos;             

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
           
        }

        public Bit BitdeEntrada
        {
            get => this.bitentrada;
            set => this.bitentrada = value; 
        }

        public int NumerodePuertos
        {
            get => this.cantidaddepuertos; 
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


        public void recibirUnBit (int puerto, Bit bit)
        {            
            this.puertos[puerto].RecibirUnBit(bit); 
        }

        public bool HuboUnaColision()
        {
            this.bitentrada = Bit.none;
            bool cero = false , uno = false;

            foreach (var item in this.PuertosConectados)
            {
                if (item.Entradas[(int)Bit.cero]) cero = true;
                if (item.Entradas[(int)Bit.uno]) uno = true; 
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

            if (hubocolision)
            {
                LimpiarLosParametrosDeEntrada();
                return;
            }

            if (this.bitentrada == Bit.none)
            {
                LimpiarLosParametrosDeEntrada(); 
                return;
            }

            StringBuilder salida = new StringBuilder(); 

            for (int i = 0; i < this.cantidaddepuertos; i++)
            {
                if (this.puertos[i] == null || !this.puertos[i].EstaConectadoAOtroDispositivo) continue; 

                if (this.puertos[i].Entradas[(int)Bit.cero] || this.puertos[i].Entradas[(int)Bit.uno])
                {
                    salida.Append(string.Format("{0} {1} receive {2} \n", Program.current_time, this.Name + $"_{i + 1}", (int)this.bitentrada));
                }
                else
                {
                    salida.Append(string.Format("{0} {1} send {2} \n", Program.current_time, this.Name + $"_{i + 1}", (int)this.bitentrada)); 
                }
            }

            while (salida.Length>1 && salida[salida.Length - 1] == '\n')
                salida.Remove(salida.Length - 1, 1);

            EscribirEnLaSalida(salida.ToString()); 
            

            LimpiarLosParametrosDeEntrada();
        }

        protected void LimpiarLosParametrosDeEntrada()
        {

            foreach (var item in this.PuertosConectados)
            {
                item.LimpiarEntradas();
            }

            this.BitdeEntrada = Bit.none; 
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append($"{this.name} \n");

            stringBuilder.Append($"Bit de Salida:\t {(int)this.BitdeSalida}\n");

            stringBuilder.Append($"Bit de Entradas:\n");

            foreach (var item in this.PuertosConectados)            
            {
                for (int j = 0; j < item.Entradas.Length; j++)
                {
                    stringBuilder.Append($"{ (item.Entradas[j] == true ?"T" : "F")}  ");
                }
                stringBuilder.Append(Environment.NewLine);
            }

            stringBuilder.Append(Environment.NewLine);

            return stringBuilder.ToString();
        }

    }
}
