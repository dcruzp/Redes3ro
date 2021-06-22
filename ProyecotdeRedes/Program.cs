using System;
using System.Reflection;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using ProyecotdeRedes.Auxiliaries;
using ProyecotdeRedes.Component;
using ProyecotdeRedes.Devices;

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
            //RunAplication();


            //foreach (var item in Program.dispositivos.Where(x => x is Computadora))
            //{
            //    Computadora comp = item as Computadora;

            //    if (comp is null) continue;

            //    comp.PrintReceivedBits();
            //}




            TestMethod(); 

        }


        public static void TestMethod ()
        {
            Switch @switch = new Switch("sw",4,0);

            Computadora computadora1 = new Computadora("pc1",1);
            Computadora computadora2 = new Computadora("pc2", 2);

            Cable cable1 = new Cable();
            Cable cable2 = new Cable(); 

            Puerto pc1 = computadora1.DameElPuerto(0);
            Puerto pc2 = computadora2.DameElPuerto(0);
            Puerto ps1 = @switch.DameElPuerto(0);
            Puerto ps2 = @switch.DameElPuerto(1);

            //pc.EstaConectadoAOtroDispositivo = true;
            //ps1.EstaConectadoAOtroDispositivo = true;

            EnviromentActions.ConnectPortsByCable(cable1, pc1, ps1);
            EnviromentActions.ConnectPortsByCable(cable2, pc2, ps2);

            computadora1.PutMacDirection("A5B3");
            computadora2.PutMacDirection("A5B4");

            while(current_time < Program.tiempo_maximo)
            {
                SendPackages(computadora1);

                computadora1.UpdateSendingBit();
                @switch.UpdateSendingBit();

                computadora1.updateDataReceived();
                @switch.updateDataReceived();

                current_time++; 
            }
        }


        private static void SendPackages(Computadora comp)
        {
            if (current_time == 1)
            {
                comp.send_frame("A5B4", "A6F43400FFB34E");
            }
        }



        public static void RunAplication()
        {
            dispositivos = new List<Dispositivo>();


            //Esto limpia el directorio de la salida (es decir borra todos los ficheros que hay 
            //en el directorio '/output') para que en la ejecución no se vayan a sobre escribir 
            //sobre ficheros ya existentes 
            EnviromentActions.LimpiarDirectoriodeSalida();


            //Esta es para cargar todos las instrucciones que hay en el fichero 'script.txt' 
            //para almacenarlos en memoria , todas las instrucciones que hay en el fichero quedan 
            //almacenadas en instrucciones , ordenadas por el tiempo de ejecución de la instrucción 
            //de forma ascendente, para que una vez hallan sido ejecutadas salgan de la cola.  
            EnviromentActions.CargarInstrucciones();


            //Este métodos es para configurar todo el entorno del programa ,como signal_time , cantidad
            //máxima de mili-segundos que debe correr el programa , etc 
            EnviromentActions.Configurar();


            //Este es el ciclo principal para correr las instrucciones y hacer el envió de 
            //información entre todos los host que están conectados.
            while (current_time < tiempo_maximo)
            {

                Console.WriteLine($"CURRENT TIME : {Program.current_time} mili-second");
               

                //Ejecutar las instrucciones que corresponden a ejecutarse en el 
                //mili-segundo actual que están en la cola de instrucciones ; 
                foreach (var item in ProximasInstruccionesEjecutar(current_time))
                {
                    EjecutarInstruccion(item);
                }
              
                //Actualizar el bit de salida de cada computadora para después 
                //enviar el bit que esta en la salida a cada uno de los 
                //Dispositivos a los que esta conectado la Computadora
                foreach (var item in dispositivos.Where(e => e is Computadora))
                {
                    Computadora comp = item as Computadora; 
                    comp.EnviarInformacionALasDemasComputadoras(); 
                }

                //verifica las entrada por cada dispositivo y chequea su hubo 
                //una colisión, y escribe en la salida (en su txt correspondiente )
                //la salida que este dispositivo tiene. 
                foreach (var item in dispositivos)
                {
                    item.ProcesarInformacionDeSalidaYDeEntrada();
                    
                    //----------------esto es un prueba -----------------------

                    Computadora comp =  item as Computadora; 
                    if(comp != null)
                    {
                        comp.updateDataReceived();
                    }

                    //--------------------------------------------------------

                    //item.LimpiarLosParametrosDeEntrada();

                }

                //Aumentar el contador de mili-segundos para pasar a procesar 
                //el próximo mili-segundo para ejecutar las instrucciones
                current_time++;                
            }
        }


        
        /// <summary>
        /// Este método ejecuta una instrucción en especifico y chequea 
        /// que tenga la sintaxis correcta , ante cualquier error esta da una excepción 
        /// identificando que pudo haber sucedido
        /// </summary>
        /// <param name="instruccion"></param>
        public static void EjecutarInstruccion(Instruccion instruccion)
        {
            string _instruccion = instruccion.instruccion;

            string [] instruccionpartida = _instruccion.Split(" ");

            if (instruccionpartida.Length < 1)
                EnviromentActions.LanzarExepciondeCasteo(instruccion);

            uint tiempodelainstruccion;

            if (! UInt32.TryParse(instruccionpartida[0],out tiempodelainstruccion))
            {
                throw new FormatException($"no tiene un formato válido '{instruccionpartida[0]}' para ser el tiempo de una instruccion ");
            }

            if (instruccionpartida.Length < 2)
                EnviromentActions.LanzarExepciondeCasteo(instruccion);

            TipodeInstruccion tipoinstruccion = AuxiliaryFunctions.GiveMeTheInstruction(instruccionpartida[1]); 

            if (tipoinstruccion == TipodeInstruccion.create)
            {
                if (instruccionpartida.Length < 4)
                    EnviromentActions.LanzarExepciondeCasteo(instruccion);

                string name = instruccionpartida[3];

                uint cantidaddepuertos = 1; 

                if (instruccionpartida[2] == "hub")
                {
                    if (instruccionpartida.Length < 5)
                    {
                        EnviromentActions.LanzarExepciondeCasteo(instruccion);
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

                else if (instruccionpartida[2] =="switch")
                {
                    if (instruccionpartida.Length < 5)
                    {
                        EnviromentActions.LanzarExepciondeCasteo(instruccion);
                    }

                    if (!UInt32.TryParse(instruccionpartida[4], out cantidaddepuertos))
                    {
                        throw new FormatException($"La cantidad de puertos '{instruccionpartida[4]}' de la instrucción no tiene un formato válido");
                    }

                    if (cantidaddepuertos < Program.cantidadminimadepuertosdeunhub || cantidaddepuertos > Program.cantidadmaximadepuertosdeunhub)
                    {
                        throw new IndexOutOfRangeException("la cantidad de puertos para un hub no son validos");
                    }

                    Switch _switch = new Switch(name, (int)cantidaddepuertos, Program.dispositivos.Count);
                    Program.dispositivos.Add(_switch);
                }
            }

            else if (tipoinstruccion == TipodeInstruccion.connect)
            {
                if (instruccionpartida.Length < 4)
                    EnviromentActions.LanzarExepciondeCasteo(instruccion);

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

              

                Puerto p1 = disp1.DameElPuerto(numeroport1);
                Puerto p2 = disp2.DameElPuerto(numeroport2);

                Cable cable = new Cable();

                EnviromentActions.ConnectPortsByCable(cable, 
                                                      puerto1: p1, 
                                                      puerto2: p2);
                

                p1.EstaConectadoAOtroDispositivo = true;
                p2.EstaConectadoAOtroDispositivo = true; 

            }

            else if (tipoinstruccion == TipodeInstruccion.send)
            {
                if (instruccionpartida.Length < 4)
                    EnviromentActions.LanzarExepciondeCasteo(instruccion);

                string host = instruccionpartida[2];
                string data = instruccionpartida[3];

                if (!CheckMetods.esBinariaLaCadena(data))
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

                computadora.send(paquetedebits); 
            }

            else if (tipoinstruccion == TipodeInstruccion.disconnect)
            {
                if (instruccionpartida.Length < 4)
                    EnviromentActions.LanzarExepciondeCasteo(instruccion);

                string port1 = instruccionpartida[2];
                string port2 = instruccionpartida[3]; 

                Dispositivo dispositivo1 = dispositivos.Where(x => x.Name == instruccionpartida[2].Split('_')[0]).FirstOrDefault();
                Dispositivo dispositivo2 = dispositivos.Where(x => x.Name == instruccionpartida[3].Split('_')[0]).FirstOrDefault();

                if (dispositivo1 == null)
                    throw new InvalidCastException($"El puerto {port1} al que se esta tratando de acceder no existe ");

                if (dispositivo2 == null)
                    throw new InvalidCastException($"El puerto {port2} al que se esta tratando de acceder no existe ");

                int numeropuerto1 = int.Parse(port1.Split('_')[1]) -1;
                int numeropuerto2 = int.Parse(port2.Split('_')[1]) -1;

                Puerto p1 = dispositivo1.DameElPuerto(numeropuerto1);
                Puerto p2 = dispositivo2.DameElPuerto(numeropuerto2);

                p1.DesconectarElPuerto();
                p2.DesconectarElPuerto(); 
                
            }
            
            else if (tipoinstruccion  ==  TipodeInstruccion.mac)
            {
                if (instruccionpartida.Length < 4 )
                {
                    throw new InvalidCastException($"La instruccion mac '{_instruccion}' no tiene un formato valido"); 
                }

                Dispositivo disp = dispositivos.Where(x => x.Name == instruccionpartida[2]).FirstOrDefault();

                Computadora comp = null;

                if (disp is Computadora) comp = disp as Computadora;
                
                if (comp is null)
                {
                    throw new NullReferenceException($"No se puede encontrar el Host '{instruccionpartida[2]}' en los dispositivos actuales"); 
                }

                string dirMac = instruccionpartida[3]; 

                if (!CheckMetods.CheckIsOkDirMac(dirMac))
                {
                    throw new InvalidCastException($"La instruccion Mac '{dirMac}' no tiene la sintaxis correcta "); 
                }

                comp.PutMacDirection(dirMac); 
            }

            else if (tipoinstruccion == TipodeInstruccion.send_frame)
            {
                if (instruccionpartida.Length < 5)
                {
                    throw new InvalidCastException($"La instruccion mac '{_instruccion}' no tiene un formato valido");
                }


                Dispositivo disp = dispositivos.Where(x => x.Name == instruccionpartida[2]).FirstOrDefault();

                Computadora comp = null;

                if (disp is Computadora) comp = disp as Computadora;

                if (comp is null)
                {
                    throw new NullReferenceException($"No se puede encontrar el Host '{instruccionpartida[2]}' en los dispositivos actuales");
                }

                string dirMacToSend = instruccionpartida[3];
                string dataToSend = instruccionpartida[4];

                if (!CheckMetods.CheckIsOkDirMac(dirMacToSend))
                {
                    throw new InvalidCastException($"La instruccion send_frame '{dirMacToSend}' no tiene la sintaxis correcta ");
                }

                if (!CheckMetods.CheckStrContainOnlyHexadecimalCharacters(dataToSend))
                {
                    throw new InvalidCastException($"La instruccion send_frame '{dataToSend}' no contiene los datos a enviar en formato hexadecimal");
                }

                comp.send_frame(dirMacToSend, dataToSend);
                  
            }
        }


        /// <summary>
        /// Este enumerable devuelve todas las instrucciones que se va a ejecutar en 
        /// un tiempo determinado 
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private static IEnumerable<Instruccion> ProximasInstruccionesEjecutar (uint time)
        {
            while(Program.instrucciones.Count > 0 && Program.instrucciones.Peek().Time <= time)
            {
                yield return Program.instrucciones.Dequeue(); 
            }
        }
       
    }
}
