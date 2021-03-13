using System;

namespace ProyecotdeRedes
{
    

    class Program
    {
        public static uint signal_time;
        public static uint current_time; 

        static void Main(string[] args)
        {
            Computadora cp1 = new Computadora("cp1");
            Computadora cp2 = new Computadora("cp2");

            Conectar(cp1, cp2); 

            

            Console.WriteLine("Hello World!");
        }

        public static void Conectar(Dispositivo disp1, Dispositivo disp2)
        {
            disp1.puerto.DispositivoConectado = disp2;
            disp2.puerto.DispositivoConectado = disp1;
        }
    }
}
