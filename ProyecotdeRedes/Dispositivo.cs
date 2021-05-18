using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ProyecotdeRedes
{
    class Dispositivo
    {
        /// <summary>
        /// Esto es para representar los puertos que 
        /// que tiene el dispositivo.
        /// </summary>
        protected Puerto[] puertos; 


        /// <summary>
        /// Este es el nombre que tiene el dispositivo 
        /// </summary>
        protected string name;       


        /// <summary>
        /// Esto es para representar el bit de salida del dispositivo 
        /// inicialmente este es None
        /// </summary>
        protected Bit bitsalida;


        /// <summary>
        /// Esto es para representar el bit de entrada del dispositivo
        /// inicialmente este es None
        /// </summary>
        protected Bit bitentrada;


        /// <summary>
        /// Este es para saber en el montículo del programa inicial donde 
        /// se almacenan los dispositivos, que indice tiene el en ese array 
        /// </summary>
        protected int indice;


        /// <summary>
        /// Esto es para representar la cantidad de puertos que tiene el dispositivo
        /// </summary>
        protected int cantidaddepuertos; 



        /// <summary>
        /// Este el constructor de un dispositivo en general 
        /// Como es obvio todo dispositivo tiene un nombre , un indice 
        /// y la  cantidad de puertos que este va a tener
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cantidaddepuertos"></param>
        /// <param name="indice"></param>
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

        /// <summary>
        /// Este es publico y solo es de consulta esta propiedad
        /// por lo que un dispositivo una vez es creado no se puede 
        /// destruir , ni eliminar de la lista 
        /// </summary>
        public int Indice
        {
            get => this.indice;
        }


        /// <summary>
        /// solo es de consulta externa , es decir este valor no se puede 
        /// modificar desde fiera de la clase o de sus descendientes 
        /// </summary>
        public Bit BitdeSalida
        {
            get => this.bitsalida;
           
        }


        /// <summary>
        /// Este es para poder acceder al bit de entrada del 
        /// dispositivo y poder cambiar su valor , esto esta mal diseñado 
        /// pero se puede modificar con unos pocos cambios 
        /// </summary>
        public Bit BitdeEntrada
        {
            get => this.bitentrada;
            set => this.bitentrada = value; 
        }


        /// <summary>
        /// para saber el numero de puertos del dispositivo 
        /// </summary>
        public int NumerodePuertos
        {
            get => this.cantidaddepuertos; 
        }


        /// <summary>
        /// Este campo es solo de lectura , para saber el nombre
        /// del dispositivo. 
        /// </summary>
        public string Name
        {
            get => this.name;
        }

        /// <summary>
        /// Retorna la instancia del Puerto que corresponde al indice 
        /// que se pasa por el id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Puerto DameElPuerto(int id)
        {
            if (id >= this.cantidaddepuertos || id < 0) throw new IndexOutOfRangeException("El dispositivo no tiene el puerto que se le especifica");

            return this.puertos[id]; 
        }


        /// <summary>
        /// Esto retorna todos los puertos (en orden ) que 
        /// tienen conectado algún dispositivo
        /// </summary>
        public IEnumerable<Puerto> PuertosConectados
        {
            get
            {
                foreach (var item in this.puertos)
                    if (item.EstaConectadoAOtroDispositivo)
                        yield return item;
            }
        }

      
        /// <summary>
        /// Esto crea un fichero si no existe , y si existe lo abre y 
        /// escribe lo que se le pasa por el parámetro indicado en recibo
        /// El txt que se crea o abre corresponde al dispositivo que es instanciado 
        /// por esta clase , (nombredeldispositivo .txt)
        /// </summary>
        /// <param name="recibo"></param>
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

        /// <summary>
        /// Esto retorna el path del directorio de salida donde se va a escribir 
        /// donde se van a crear los ficheros para escribir la salidas correspondientes
        /// </summary>
        /// <returns></returns>
        string DirectorioDeSalida()
        {
            var CurrentDirectory = Environment.CurrentDirectory;
            var parent = Directory.GetParent(Directory.GetParent(Directory.GetParent(CurrentDirectory).FullName).FullName);
            return Path.Join(parent.FullName, "output");
        }


        /// <summary>
        /// Esto escribe el bit que se pasa por el parámetro Bit 
        /// en el puerto que se indica en el parámetro puerto
        /// </summary>
        /// <param name="puerto"></param>
        /// <param name="bit"></param>
        public void recibirUnBit (int puerto, Bit bit)
        {            
            this.puertos[puerto].RecibirUnBit(bit); 
        }


        /// <summary>
        /// Esto detecta si en los bits de entradas hubo mas de 
        /// un bit que fue recibido , es decir se recibieron 
        /// bit diferentes de algunas computadoras
        /// </summary>
        /// <returns></returns>
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

            //if (this is Computadora && bitentrada!= Bit.none /*&&  bitentrada != bitsalida*/) return true; 
            //else return false; 
            return false; 
        }


        /// <summary>
        /// Esto chequea si hubo colisión y escribe en las salidas los 
        /// valores correspondientes 
        /// </summary>
        public virtual void ProcesarInformacionDeSalidaYDeEntrada()
        {
            bool hubocolision = HuboUnaColision();

            if (hubocolision || this.bitentrada == Bit.none)  return;
            

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
           
        }


        /// <summary>
        /// esto limpia el array de bit recibido por las computadoras en un 
        /// mili segundo determinado 
        /// </summary>
        public void LimpiarLosParametrosDeEntrada()
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
