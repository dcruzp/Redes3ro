using System;
using System.Reflection;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace ProyecotdeRedes
{
    

    class Program
    {
        public static uint signal_time = 10 ;
        public static uint current_time = 0 ;
        public static uint tiempomaximo = 1000;

        public static  List<Instruccion> instrucciones;
        public static List<Dispositivo> dispositivos; 
        static void Main(string[] args)
        {
            Computadora cp1 = new Computadora("cp1");
            Computadora cp2 = new Computadora("cp2");

            Conectar(cp1, cp2);

            LimpiarDirectoriodeSalida();

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

            //CargarInstrucciones(); 

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

        public static void CargarInstrucciones ()
        {
            instrucciones = new List<Instruccion>(); 

            var CurrentDirectory = Environment.CurrentDirectory;
            var parent = Directory.GetParent(Directory.GetParent(Directory.GetParent(CurrentDirectory).FullName).FullName);

            var script = Path.Join(parent.FullName, "input", "script.txt" );

            if (File.Exists(script))
            {
                IEnumerable<string> line = File.ReadLines(script);
                //Console.WriteLine(String.Join(Environment.NewLine, line));

                foreach (var item in line)
                {
                    var instruccion = new Instruccion(item);
                    instrucciones.Add(instruccion);
                    EjecutarInstruccion(instruccion);
                }
            }
            else
            {
                throw new NullReferenceException("no existe el fichero script"); 
            }
        }

        public static void EjecutarInstruccion(Instruccion instruccion)
        {
            string _instruccion = instruccion.instruccion;

            string [] instruccionpartida = _instruccion.Split(" ");

            //Console.WriteLine(String.Join(Environment.NewLine , instruccionpartida));

            if (instruccionpartida.Length < 1)
                LanzarExepciondeCasteo(instruccion);

            uint tiempodelainstruccion;

            if (! UInt32.TryParse(instruccionpartida[0],out tiempodelainstruccion))
            {
                throw new FormatException("el tiempo de la instrucción no tiene un formato válido ");
            }

            if (instruccionpartida.Length < 2)
                LanzarExepciondeCasteo(instruccion);

            TipodeInstruccion tipoinstruccion ; 

            switch(instruccionpartida[1])
            {
                case "create":
                    tipoinstruccion = TipodeInstruccion.create;
                    break;
                case "connect":
                    tipoinstruccion = TipodeInstruccion.connect;
                    break;
                case "send":
                    tipoinstruccion = TipodeInstruccion.send;
                    break;
                case "disconnect":
                    tipoinstruccion = TipodeInstruccion.disconnect;
                    break;
                default:
                    throw new InvalidCastException("el tipo de instruccion no es valida");
            }

            if (tipoinstruccion == TipodeInstruccion.create)
            {
                if (instruccionpartida.Length < 4)
                    LanzarExepciondeCasteo(instruccion);

                string name = instruccionpartida[3];

                uint cantidaddepuertos = 1; 

                if (instruccionpartida[2] == "hub")
                {
                    if (instruccionpartida.Length < 5)
                    {
                        LanzarExepciondeCasteo(instruccion);
                    }

                    if (!UInt32.TryParse(instruccionpartida[4], out cantidaddepuertos))
                    {
                        throw new FormatException("La cantidad de puertos de la instrucción no tiene un formato válido");
                    }

                    if (cantidaddepuertos < 4 || cantidaddepuertos > 8)
                    {
                        throw new IndexOutOfRangeException("la cantidad de puertos para un hub no son validos"); 
                    }
                }
                else if (instruccionpartida[2] == "host")
                {
                    cantidaddepuertos = 1; 
                }
            }

            else if (tipoinstruccion == TipodeInstruccion.connect)
            {
                if (instruccionpartida.Length < 4)
                    LanzarExepciondeCasteo(instruccion);

                string port1 = instruccionpartida[2];
                string port2 = instruccionpartida[3]; 
            }

            else if (tipoinstruccion == TipodeInstruccion.send)
            {
                if (instruccionpartida.Length < 4)
                    LanzarExepciondeCasteo(instruccion);

                string host = instruccionpartida[2];
                string data = instruccionpartida[3];

                if (!Esbinarioelstring(data))
                {
                    throw new InvalidCastException($"La informacion '{data}' que se quiere enviar no tiene un formato correcto ");
                }
            }

            else if (tipoinstruccion == TipodeInstruccion.disconnect)
            {
                if (instruccionpartida.Length < 3)
                    LanzarExepciondeCasteo(instruccion);

                string port = instruccionpartida[2];
            }

            //Console.WriteLine(tipoinstruccion);

            //Console.WriteLine(tiempodelainstruccion);

            Console.WriteLine("tiempo -> {0}  isntruccion -> {1}" , tiempodelainstruccion ,tipoinstruccion );
            
        }

        static void LanzarExepciondeCasteo(Instruccion instruccion)
        {
            throw new InvalidCastException($"La instruccion '{instruccion.instruccion}' no tiene un formato válido ");
        }

        static bool Esbinarioelstring (string numero)
        {
            foreach (var item in numero)
            {
                if(item != '0' && item != '1')
                {
                    return false; 
                }
            }
            return true; 
        }
    }
}
