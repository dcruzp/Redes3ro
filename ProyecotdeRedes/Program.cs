using System;
using System.Reflection;
using System.IO;
using System.Threading; 

namespace ProyecotdeRedes
{
    

    class Program
    {
        public static uint signal_time = 10 ;
        public static uint current_time = 0 ;
        public static uint tiempomaximo = 1000; 
        static void Main(string[] args)
        {
            Computadora cp1 = new Computadora("cp1");
            Computadora cp2 = new Computadora("cp2");

            Conectar(cp1, cp2);

            LimpiarDirectoriodeSalida();

            //cp1.EscribirInformacionEnELFichero();

            //string path = Assembly.GetEntryAssembly().Location;

            //string path1 = AppDomain.CurrentDomain.;
            //Console.WriteLine(path);
            //Console.WriteLine(path1);

            //var directory = Directory.GetCurrentDirectory();

            //Console.WriteLine(directory);


            //var CurrentDirectory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            //Console.WriteLine(CurrentDirectory);

            //var CurrentDirectory = Environment.CurrentDirectory;
            //var parent =  Directory.GetParent(Directory.GetParent(Directory.GetParent(CurrentDirectory).FullName).FullName);


            //Console.WriteLine(Path.Join(parent.FullName , "output"));

            Bit[] paquete = { (Bit)1, (Bit)0, (Bit)1, (Bit)1, (Bit)0 };

            cp1.send(paquete);

            while (current_time < tiempomaximo)
            {
                cp1.IntentarEnviar();
                cp2.IntentarEnviar();
                cp1.Recibir(); 
                cp2.Recibir();

                if (current_time % signal_time == 0)
                {
                    Console.WriteLine("Pasaron " + signal_time + " milisegundos");
                }
                current_time++;
            }

        }

        public  static void LimpiarDirectoriodeSalida()
        {
            var CurrentDirectory = Environment.CurrentDirectory;
            var parent = Directory.GetParent(Directory.GetParent(Directory.GetParent(CurrentDirectory).FullName).FullName);

            var salida = Path.Join(parent.FullName, "output");

            if (Directory.Exists(salida))
            {
                foreach (var item in Directory.GetFiles(salida))
                {
                    File.Delete(item); 
                }
            }
            else
            {
                throw new Exception("La Salida no existe , para borrar el contenodo dentro del directorio"); 
            }
             
        }

        public static void Conectar(Dispositivo disp1, Dispositivo disp2)
        {
            disp1.puerto.DispositivoConectado = disp2;
            disp2.puerto.DispositivoConectado = disp1;
        }

    }
}
