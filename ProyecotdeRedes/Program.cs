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
        public static uint tiempomaximo = 1000;

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

                foreach (var item in dispositivos.Where(e=>e is Computadora))
                {
                    Computadora comp = item as Computadora;
                    comp.ActualizarElBitDeSalida();
                }

                foreach (var item in dispositivos.Where(e => e  is Computadora))
                {
                    Computadora comp = item as Computadora;

                    if (comp.BitdeSalida == Bit.none) continue;
                   
                    bool hubocolicion = HuboUnaColicion(comp);

                   
                    if (hubocolicion)
                    {
                        comp.Actualizar();
                        comp.EscribirEnLaSalida(string.Format("{0} {1} send {2} collision", Program.current_time, comp.Name, (int)comp.BitdeSalida));
                    }
                    else
                    {
                        comp.EscribirEnLaSalida(string.Format("{0} {1} send {2} OK", Program.current_time, comp.Name, (int)comp.BitdeSalida));
                    }
                }

                foreach (var item in dispositivos.Where(e => e is Computadora))
                {
                    Computadora comp = item as Computadora;
                    ActualizarelBitdeEntrada(comp);
                    if (comp.BitdeEntrada != Bit.none)
                        comp.EscribirEnLaSalida(string.Format("{0} {1} receive {2} ", Program.current_time, comp.Name, (int)comp.BitdeEntrada));
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
                throw new FormatException($"no tiene un formato válido '{instruccionpartida[0]}' para ser el tiempo de una instruccion ");
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
                    throw new InvalidCastException($" '{instruccionpartida[1]}' no ese un tipo de instrucción valida");
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
                        throw new FormatException($"La cantidad de puertos '{instruccionpartida[4]}' de la instrucción no tiene un formato válido");
                    }

                    if (cantidaddepuertos < 4 || cantidaddepuertos > 8)
                    {
                        throw new IndexOutOfRangeException("la cantidad de puertos para un hub no son validos"); 
                    }

                    Hub hub = new Hub(name ,(int)cantidaddepuertos,Program.dispositivos.Count);
                    Program.dispositivos.Add(hub); 

                }
                else if (instruccionpartida[2] == "host")
                {
                    cantidaddepuertos = 1;

                    Computadora computadora = new Computadora(name ,Program.dispositivos.Count);
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

                disp1 = dispositivos.Where(disp => disp.Name.Contains(port1.Split('_').FirstOrDefault())).FirstOrDefault();

                if (disp1 == null)
                {
                    throw new KeyNotFoundException($"No hay ningún dispositivo cuyo nombre sea {port1.Split('_')}");
                }

                disp2 = dispositivos.Where(disp => disp.Name.Contains(port2.Split('_').FirstOrDefault())).FirstOrDefault();


                if (disp2 == null)
                {
                    throw new KeyNotFoundException($"No hay ningún dispositivo cuyo nombre sea {port2.Split('_')}");
                }

                int numeroport1 = int.Parse(port1.Split('_')[1]) - 1;
                int numeroport2 = int.Parse(port2.Split('_')[1]) - 1;

                disp1[numeroport1] = disp2;
                disp2[numeroport2] = disp1; 
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


        public static bool  HuboUnaColicion (Dispositivo disp )
        {
            Queue<Dispositivo> cola = new Queue<Dispositivo>();
            bool[] mask = new bool[Program.dispositivos.Count];
            mask[disp.Indice] = true; 
            cola.Enqueue(disp);

            Dispositivo current;

            while(cola.Count > 0 )
            {
                current = cola.Dequeue(); 

                foreach (var item in current.DispositivosConectados)
                {
                    if (item is null) continue; 

                    int indice = item.Indice;
                    if (mask[indice]) continue;

                    if (item is Computadora && item.BitdeSalida != Bit.none && item.BitdeSalida != disp.BitdeSalida)
                    {
                        return true; 
                    }

                    mask[indice] = true;

                    cola.Enqueue(item); 
                }
            }
            return false; 
        }


        public static bool ActualizarelBitdeEntrada(Dispositivo disp)
        {
            disp.BitdeEntrada = Bit.none;
            Queue<Dispositivo> cola = new Queue<Dispositivo>();
            bool[] mask = new bool[Program.dispositivos.Count];
            mask[disp.Indice] = true;
            cola.Enqueue(disp);

            Dispositivo current;

            while (cola.Count > 0)
            {
                current = cola.Dequeue();

                foreach (var item in current.DispositivosConectados)
                {
                    if (item is null) continue;

                    int indice = item.Indice;
                    if (mask[indice]) continue;

                    if (item is Computadora)
                    {
                        Computadora computadora = item as Computadora; 

                        if (computadora.BitdeSalida != Bit.none)
                        {
                            if (disp.BitdeEntrada == Bit.none)
                            {
                                disp.BitdeEntrada = computadora.BitdeSalida;
                            }
                            else if (computadora.BitdeSalida != disp.BitdeEntrada)
                            {
                                disp.BitdeEntrada = Bit.none; 
                                return false; 
                            }
                        }
                    }

                    mask[indice] = true;

                    cola.Enqueue(item);
                }
            }
            return true;
        }
    }
}
