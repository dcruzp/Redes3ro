﻿using System;
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
