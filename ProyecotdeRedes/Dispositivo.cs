using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ProyecotdeRedes
{
    class Dispositivo
    {
        Puerto _puerto;

        protected string name; 

        public Dispositivo (string name)
        {
            this.name = name;
            this.puerto = new Puerto(); 
        }

        public Puerto puerto
        {
            get => this._puerto;
            set => this._puerto = value;
        }

        public void EscribirInformacionEnELFichero()
        {
            //File.Create("output/" + this.name + ".txt");
            //FileInfo fileInfo = new FileInfo("output/" + this.name + ".txt"); 
            //StreamWriter streamWriter = fileInfo.AppendText();
            //streamWriter.WriteLine("esto es un texto para escribir");
            //streamWriter.Close();

            //GenerarTXT(); 

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
