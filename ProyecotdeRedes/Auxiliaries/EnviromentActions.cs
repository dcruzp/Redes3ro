using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using ProyecotdeRedes.Auxiliaries;
using ProyecotdeRedes.Component;

namespace ProyecotdeRedes
{
    public class EnviromentActions
    {
        /// <summary>
        /// Esto es para poner la configuracion por defecto , 
        /// que se pone de la manera que esta descrita en el 
        /// metodo
        /// </summary>
        public static void PonerConfiguracionPorDefecto()
        {
            Program.cantidadminimadepuertosdeunhub = 4;
            Program.cantidadminimadepuertosdeunhub = 8;
            Program.signal_time = 10;
            Program.tiempo_maximo = 1000000;
        }



        /// <summary>
        /// esto es para leer del fichero config.txt que se 
        /// encuentra en este mismo directorio en que esta este proyecto 
        /// este pone todos los parametros establecido que se encuentran en
        /// el fichero 
        /// </summary>
        public static void Configurar()
        {
            var CurrentDirectory = Environment.CurrentDirectory;
            var parent = Directory.GetParent(Directory.GetParent(Directory.GetParent(CurrentDirectory).FullName).FullName);

            string rutaCompleta = Path.Join(parent.FullName, "config.txt");

            if (!File.Exists(rutaCompleta))
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

                switch (configuracionpartida[0])
                {
                    case "max_cantidad_milisegundos":
                        UInt32 _tiempo_maximo;
                        if (UInt32.TryParse(configuracionpartida[1], out _tiempo_maximo))
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

                        if (extremosdelintervalo.Length < 2)
                        {
                            throw new InvalidCastException($"No tiene el formato correcto los intervalos '{extremosdelintervalo}'");
                        }

                        int min, max;

                        if (!int.TryParse(extremosdelintervalo[0], out min))
                        {
                            throw new InvalidCastException($"El extremo {extremosdelintervalo[0]} no es un numero valido ");
                        }
                        if (!int.TryParse(extremosdelintervalo[1], out max))
                        {
                            throw new InvalidCastException($"El extremo {extremosdelintervalo[1]} no es un numero valido ");
                        }

                        if (CheckMetods.sonvalidoslacantidaddepuertosdeunhub(min, max))
                        {
                            Program.cantidadminimadepuertosdeunhub = min;
                            Program.cantidadmaximadepuertosdeunhub = max;
                        }
                        break;

                }
            }
        }



        /// <summary>
        /// esto borra todos los ficheros que se encuentran en el directorio 
        /// output y se llama al principio de la ejecución del programa , antes de 
        /// entrar en el ciclo principal , y se hace para barrar todos los fichero que 
        /// se podían haber generado previamente en la ejecución de programa en un momento 
        /// anterior
        /// </summary>
        public static void LimpiarDirectoriodeSalida()
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



        /// <summary>
        /// Esto es para cargar las instrucciones del fichero script.txt que 
        /// se encuentra en el diretorio input/ en el directorio donde se encuentra
        /// esta solucion . 
        /// </summary>
        public static void CargarInstrucciones()
        {
            //Program.instrucciones = new Queue<Instruccion>();

            var directorio = Directory.GetParent(Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).FullName).FullName);

            var directoriodelfichero = Path.Join(directorio.FullName, "input", "script.txt");

            if (File.Exists(directoriodelfichero))
            {
                IEnumerable<Instruccion> lines = from inst in File.ReadLines(directoriodelfichero)
                                                 where !string.IsNullOrEmpty(inst) 
                                                 orderby int.Parse(inst.Split(' ')[0]) ascending
                                                 select new Instruccion(inst);

                Program.instrucciones = new Queue<Instruccion>(lines);
            }
            else
            {
                throw new NullReferenceException("no existe el fichero script");
            }
        }


        public static void ConnectPortsByCable(Cable cable , Puerto puerto1 , Puerto puerto2)
        {
            cable.puerto1 = puerto1;
            cable.puerto2 = puerto2;

            puerto1.ConnectCableToPort(cable);
            puerto2.ConnectCableToPort(cable);
        }


        public static void LanzarExepciondeCasteo(Instruccion instruccion)
        {
            throw new InvalidCastException($"La instrucción '{instruccion.instruccion}' no tiene un formato válido ");
        }
    }
}
