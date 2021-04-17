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
        /// <summary>
        /// tiempo que demora un bit transmitiéndose por un canal 
        /// </summary>
        public static uint signal_time = 10 ;

        /// <summary>
        /// mili-segundo actual , por el que va ejecutándose el programa
        /// </summary>
        public static uint current_time = 0 ;

        /// <summary>
        /// Tiempo máximo que puede corres el programa (la medida de tiempo es como mili-segundos)
        /// </summary>
        public static uint tiempo_maximo = 1000000;


        /// <summary>
        /// Cantidad mínima de puertos que puede tener un hub
        /// </summary>
        public static int cantidadminimadepuertosdeunhub = 4;

        /// <summary>
        /// Cantidad máxima de puertos que puede tener un hub
        /// </summary>
        public static int cantidadmaximadepuertosdeunhub = 8; 




        /// <summary>
        /// Cola de instrucciones que son cargadas al principio del programa 
        /// para después ser ejecutadas según llegue su momento 
        /// </summary>
        public static  Queue<Instruccion> instrucciones;

        /// <summary>
        /// Lista de dispositivos que actualmente en el entorno 
        /// El tamaño de la sita va creciendo a medida que se ejecute 
        /// correctamente una instrucción de create 
        /// </summary>
        public static List<Dispositivo> dispositivos; 
        
        
        
        static void Main(string[] args)
        {
            CorrerlaAplicacion(); 

            //EscribirlaInformacionenConsola(); 
        }

        public static void EscribirlaInformacionenConsola()
        {
            Console.ReadKey(); 
            Console.Clear();
            Console.WriteLine($"TIME : {Program.current_time}");
           // Console.ReadKey();
            
        }
        

        public static void CorrerlaAplicacion ()
        {
            dispositivos = new List<Dispositivo>();


            //Esto limpia el directorio de la salida (es decir borra todos los ficheros que hay 
            //en el directorio '/output') para que en la ejecución no se vayan a sobre escribir 
            //sobre ficheros ya existentes 
            LimpiarDirectoriodeSalida();



            //Esta es para cargar todos las instrucciones que hay en el fichero 'script.txt' 
            //para almacenarlos en memoria , todas las instrucciones que hay en el fichero quedan 
            //almacenadas en instrucciones , ordenadas por el tiempo de ejecución de la instrucción 
            //de forma ascendente, para que una vez hallan sido ejecutadas salgan de la cola.  
            CargarInstrucciones();



            //Este métodos es para configurar todo el entorno del programa ,como signal_time , cantidad
            //máxima de mili-segundos que debe correr el programa , etc 
            Configurar();


            //Este es el ciclo principal para correr las instrucciones y hacer el envió de 
            //información entre todos los host que están conectados.
            while (current_time < tiempo_maximo)
            {

                //Console.WriteLine($"TIME : {Program.current_time} millisecond");
                //EscribirlaInformacionenConsola(); 

                //Ejecutar las instrucciones que corresponden a ejecutarse en el 
                //mili-segundo actual que están en la cola de instrucciones ; 
                foreach (var item in ProximasInstruccionesEjecutar(current_time))
                {
                    EjecutarInstruccion(item);
                }


                foreach (var item in dispositivos.Where(e => e is Computadora))
                {
                    (item as Computadora).ActualizarElBitDeSalida();
                }

                foreach (var item in dispositivos.Where(e => e is Computadora))
                {
                    Computadora comp = item as Computadora;

                    if (comp.BitdeSalida == Bit.none || comp.NoEstaConectada()) continue;

                    bool hubocolicion = HuboUnaColicion(comp);

                    if (hubocolicion)
                    {
                        comp.Actualizar();
                        string textoaescribirenlasalidas = string.Format("{0} {1} send {2} collision", Program.current_time, comp.Name, (int)comp.BitdeSalida);
                        comp.EscribirEnLaSalida(textoaescribirenlasalidas);
                        Console.WriteLine(textoaescribirenlasalidas);

                    }
                    else
                    {
                        string textoaescribirenlasalidas = string.Format("{0} {1} send {2} OK", Program.current_time, comp.Name, (int)comp.BitdeSalida); 
                        comp.EscribirEnLaSalida(textoaescribirenlasalidas);
                        Console.WriteLine(textoaescribirenlasalidas);
                    }
                }

                foreach (var item in dispositivos.Where(e => e is Computadora))
                {
                    Computadora comp = item as Computadora;
                    ActualizarelBitdeEntrada(comp);
                    if (comp.BitdeEntrada != Bit.none)
                        comp.EscribirEnLaSalida(string.Format("{0} {1} receive {2} ", Program.current_time, comp.Name, (int)comp.BitdeEntrada));
                }

                foreach (var item in dispositivos.Where(e => e is Hub))
                {
                    Hub hub = item as Hub;
                    hub.ActualizarlaEntrada();
                    if (hub.BitdeSalida != Bit.none)
                    {
                        for (int i = 0; i < hub.BitsDeEntrada.Length; i++)
                        {
                            if (hub[i] == null) continue;

                            if (hub.BitsDeEntrada[i] == hub.BitdeSalida)
                            {
                                hub.EscribirEnLaSalida(string.Format("{0} {1} receive {2} ", Program.current_time, hub.Name + $"_{i + 1}", (int)hub.BitdeSalida));
                            }
                            else
                            {
                                hub.EscribirEnLaSalida(string.Format("{0} {1} send {2} ", Program.current_time, hub.Name + $"_{i + 1}", (int)hub.BitdeSalida));
                            }
                        }
                    }
                }

                current_time++;                
            }

            //Console.WriteLine($"Signal_time {Program.signal_time}\nCantidad mínima de puertos que puede tener un hub:{Program.cantidadminimadepuertosdeunhub} \nCantidad mínima de puertos que puede tener un hub:{Program.cantidadmaximadepuertosdeunhub}");
        }




        public static void PonerConfiguracionPorDefecto ()
        {
            Program.cantidadminimadepuertosdeunhub = 4;
            Program.cantidadminimadepuertosdeunhub = 8;
            Program.signal_time = 10;
            Program.tiempo_maximo = 1000000; 
        }

        public static void Configurar()
        {
            var CurrentDirectory = Environment.CurrentDirectory;
            var parent = Directory.GetParent(Directory.GetParent(Directory.GetParent(CurrentDirectory).FullName).FullName);

            string rutaCompleta = Path.Join(parent.FullName, "config.txt");

            if (! File.Exists(rutaCompleta))
            {
                PonerConfiguracionPorDefecto();
                Console.WriteLine($"Advertencia: El fichero 'config.txt' no existe\n las configuraciones que se van a poner son las que hay por defecto");
                return;                
            }

            var variable = File.ReadLines(rutaCompleta);


            foreach (var item in variable)
            {
                if (item.Length < 1) continue; 

                string[] configuracionpartida = item.Split(':');
              
                if (configuracionpartida.Length < 2) throw new InvalidCastException($"la configuración {item} no tiene el formato correcto"); 

                switch(configuracionpartida[0])
                {
                    case "max_cantidad_milisegundos":
                        UInt32 _tiempo_maximo; 
                        if (UInt32.TryParse(configuracionpartida[1],out _tiempo_maximo))
                        {
                            Program.tiempo_maximo = _tiempo_maximo; 
                        }
                        else
                        {
                            throw new InvalidCastException($"el numero para asignarle al tiempo_maximo: '{configuracionpartida[1]} no es valido '"); 
                        }
                        break; 

                    case "signal_time":
                        int _signal_time;
                        if (Int32.TryParse(configuracionpartida[1], out _signal_time))
                        {
                            Program.signal_time = (uint)_signal_time; 
                        }
                        else
                        {
                            throw new InvalidCastException($"el numero para asignarle el _signal_time '{configuracionpartida[1]}' no tiene el formato correcto"); 
                        }
                        break;
                    case "numero_puertos_hub":
                        string[] extremosdelintervalo = configuracionpartida[1].Split('-');

                        if (extremosdelintervalo.Length<2)
                        {
                            throw new InvalidCastException($"No tiene el formato correcto los intervalos '{extremosdelintervalo}'");
                        }

                        int min, max;

                        if (! int.TryParse(extremosdelintervalo[0], out min))
                        {
                            throw new InvalidCastException($"El extremo {extremosdelintervalo[0]} no es un numero valido "); 
                        }
                        if (!int.TryParse(extremosdelintervalo[1], out max))
                        {
                            throw new InvalidCastException($"El extremo {extremosdelintervalo[1]} no es un numero valido ");
                        }

                        if (sonvalidoslacantidaddepuertosdeunhub(min,max))
                        {
                            Program.cantidadminimadepuertosdeunhub = min;
                            Program.cantidadmaximadepuertosdeunhub = max; 
                        }
                        break; 

                }
            }
        }

        public static bool sonvalidoslacantidaddepuertosdeunhub(int a , int b)
        {
            if (a < 4 )
            {
                throw new InvalidCastException("Un hub no puede tener menos de 4 puertos "); 
            }
            if (b<=a)
            {
                throw new InvalidCastException("La cantidad máxima de puertos no puede ser menor o igual que la cantidad mínima de puertos");
            }
            return true; 
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
            
            var directorio = Directory.GetParent(Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).FullName).FullName);

            var directoriodelfichero = Path.Join(directorio.FullName, "input", "script.txt" );

            if (File.Exists(directoriodelfichero))
            {
                IEnumerable<Instruccion> lines = from inst in File.ReadLines(directoriodelfichero)
                                            orderby int.Parse(inst.Split(' ')[0]) ascending
                                            select new Instruccion(inst);

                instrucciones = new Queue<Instruccion>(lines); 
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
                if (instruccionpartida.Length < 4)
                    LanzarExepciondeCasteo(instruccion);

                string port1 = instruccionpartida[2];
                string port2 = instruccionpartida[3]; 

                Dispositivo dispositivo1 = dispositivos.Where(x => x.Name == instruccionpartida[2].Split('_')[0]).FirstOrDefault();
                Dispositivo dispositivo2 = dispositivos.Where(x => x.Name == instruccionpartida[3].Split('_')[0]).FirstOrDefault();

                if (dispositivo1 == null)
                    throw new InvalidCastException($"El puerto {port1} al que se esta tratando de acceder no existe ");

                if (dispositivo2 == null)
                    throw new InvalidCastException($"El puerto {port2} al que se esta tratando de acceder no existe ");

                dispositivo1[int.Parse(port1.Split('_')[1])-1] = null;
                dispositivo2[int.Parse(port2.Split('_')[1])-1] = null; 

            }
            
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

        private static IEnumerable<Instruccion> ProximasInstruccionesEjecutar (uint time)
        {
            while(Program.instrucciones.Count > 0 && Program.instrucciones.Peek().Time <= time)
            {
                yield return Program.instrucciones.Dequeue(); 
            }
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
