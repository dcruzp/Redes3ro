using System;
using System.Reflection;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Linq; 

namespace ProyecotdeRedes
{
    

    class Program
    {
        public static uint signal_time = 10 ;
        public static uint current_time = 0 ;
        public static uint tiempomaximo = 10000;

        public static  Queue<Instruccion> instrucciones;
        public static List<Dispositivo> dispositivos; 
        static void Main(string[] args)
        {
            dispositivos = new List<Dispositivo>(); 

            LimpiarDirectoriodeSalida();

            CargarInstrucciones();

            
            while (current_time < tiempomaximo)
            {
                var instruccions = from instr in instrucciones
                                   where uint.Parse(instr.instruccion.Split(' ')[0]) == current_time
                                   select instr;

                foreach (var item in instruccions)
                {
                    EjecutarInstruccion(item); 
                }

                foreach (var item in dispositivos)
                {
                    if (item is Computadora)
                    {
                        Computadora comp = item as Computadora;

                        comp.IntentarEnviar();
                        comp.Recibir(); 
                    }
                }
                if (current_time % signal_time == 0)
                {
                    Console.WriteLine("Pasaron " + signal_time + " mili-segundos");
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
                throw new Exception("La Salida no existe , para borrar el contenido dentro del directorio"); 
            }
             
        }

        public static void Conectar(string  port1, string port2)
        {
            var auxdisp = from dispositivo in dispositivos
                          where dispositivo.Name == port1.Split('_')[0]
                          select dispositivo;

            if (!auxdisp.Any()) throw new Exception($"No se encontró el dispositivo '{port1.Split('_')[0]}'");

            Dispositivo disp1 = auxdisp.ToArray()[0];

            auxdisp = from dispositivo in dispositivos
                      where dispositivo.Name == port2.Split('_')[0]
                      select dispositivo;

            if (!auxdisp.Any()) throw new Exception($"No se encontró el dispositivo '{port2.Split('_')[0]}'");

            Dispositivo disp2 = auxdisp.ToArray()[0]; 

            disp1[int.Parse(port1.Split('_')[1])-1].DispositivoConectado = disp2;
            disp2[int.Parse(port2.Split('_')[1])-1].DispositivoConectado = disp1;
        }

        public static void CargarInstrucciones()
        {
            instrucciones = new Queue<Instruccion>(); 

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
                    instrucciones.Enqueue(instruccion);
                    //EjecutarInstruccion(instrucción);
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
                    throw new InvalidCastException("el tipo de instrucción no es valida");
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

                    Hub hub = new Hub(name ,(int)cantidaddepuertos);
                    Program.dispositivos.Add(hub); 

                }
                else if (instruccionpartida[2] == "host")
                {
                    cantidaddepuertos = 1;

                    Computadora computadora = new Computadora(name);
                    Program.dispositivos.Add(computadora); 
                }
            }

            else if (tipoinstruccion == TipodeInstruccion.connect)
            {
                if (instruccionpartida.Length < 4)
                    LanzarExepciondeCasteo(instruccion);

                string port1 = instruccionpartida[2];
                string port2 = instruccionpartida[3];

                Dispositivo disp1;
                Dispositivo disp2;

                IEnumerable<Dispositivo> items  = from dispositivo in dispositivos
                        where dispositivo.Name == port1.Split('_')[0]
                        select dispositivo;

                if (items.ToArray().Length != 1 )
                {
                    throw new Exception("el Dispositivo no se encontró por el nombre"); 
                }
                disp1 = items.ToArray()[0];


                items = from dispositivo in dispositivos
                        where dispositivo.Name == port2.Split('_')[0]
                        select dispositivo;

                if (items.ToArray().Length != 1)
                {
                    throw new Exception("el Dispositivo no se encontró por el nombre");
                }

                disp2 = items.ToArray()[0];

                Conectar(port1, port2); 
            }

            else if (tipoinstruccion == TipodeInstruccion.send)
            {
                if (instruccionpartida.Length < 4)
                    LanzarExepciondeCasteo(instruccion);

                string host = instruccionpartida[2];
                string data = instruccionpartida[3];

                if (!Esbinarioelstring(data))
                {
                    throw new InvalidCastException($"La información '{data}' que se quiere enviar no tiene un formato correcto ");
                }

                var disp = from dispositivo in dispositivos
                           where dispositivo.Name == host.Split('_')[0]
                           select dispositivo;

                Dispositivo[] comp = disp.ToArray(); 
                if(comp.Length != 1)
                {
                    throw new Exception("no se encontró el dispositivo "); 
                }

                Computadora computadora = comp[0] as Computadora;

                List<Bit> paquetedebits = new List<Bit>();

                foreach (var item in data)
                {
                    paquetedebits.Add((Bit)int.Parse(item.ToString())); 
                }

                computadora.send(paquetedebits.ToArray()); 
            }

            else if (tipoinstruccion == TipodeInstruccion.disconnect)
            {
                if (instruccionpartida.Length < 3)
                    LanzarExepciondeCasteo(instruccion);

                string port = instruccionpartida[2];
            }

            //Console.WriteLine(tipoinstruccion);

            //Console.WriteLine(tiempodelainstruccion);

            //Console.WriteLine("tiempo -> {0}  instrucción -> {1}" , tiempodelainstruccion ,tipoinstruccion );
            
        }

        static void LanzarExepciondeCasteo(Instruccion instruccion)
        {
            throw new InvalidCastException($"La instrucción '{instruccion.instruccion}' no tiene un formato válido ");
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
