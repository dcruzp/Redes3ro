using ProyecotdeRedes.Auxiliaries;
using ProyecotdeRedes.Component;
using ProyecotdeRedes.Devices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProyecotdeRedes
{
  class Program
  {
    /// <summary>
    /// tiempo que demora un bit transmitiéndose por un canal 
    /// </summary>
    public static uint signal_time = 10;

    /// <summary>
    /// mili-segundo actual , por el que va ejecutándose el programa
    /// </summary>
    public static uint current_time = 0;

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
    public static Queue<Instruccion> instrucciones;

    /// <summary>
    /// Lista de dispositivos que actualmente en el entorno 
    /// El tamaño de la sita va creciendo a medida que se ejecute 
    /// correctamente una instrucción de create 
    /// </summary>
    public static List<Device> dispositivos;


    static void Main(string[] args)
    {
      RunAplication();

      //IP ip1 = new IP("10.0.0.1", System.Globalization.NumberStyles.None);
      //IP ip2 = new IP("10.0.0.2", System.Globalization.NumberStyles.None);
      //IP ip3 = new IP("10.0.0.3", System.Globalization.NumberStyles.None);

      //Dictionary<IP, List<String>> keyValuePairs = new Dictionary<IP, List<string>>();

      //keyValuePairs.Add(ip1, new List<string>() { "AAAAAAAAAAA", "BBBBBBBBBB000011" });
      //keyValuePairs.Add(ip2, new List<string>() { "DDDDDDDDDDD" });
      //keyValuePairs.Add(ip3, new List<string>() { "CCCCCCCCCCCC" });



      //var datatosend = keyValuePairs[ip2];


      //foreach (var item in datatosend)
      //{
      //  Console.WriteLine(item);
      //}

    }


    public static void RunAplication()
    {
      dispositivos = new List<Device>();


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

        //Console.WriteLine($"CURRENT TIME : {Program.current_time} mili-second");


        //Ejecutar las instrucciones que corresponden a ejecutarse en el 
        //mili-segundo actual que están en la cola de instrucciones
        foreach (var item in ProximasInstruccionesEjecutar(current_time))
        {
          EjecutarInstruccion(item);
        }

        //Actualizar el bit que hay en la salida de cada uno 
        //de los puertos de todos los dispositivos que existen 
        foreach (var item in dispositivos)
        {
          item.UpdateSendingBit();

        }


        //Actualizar la entrada y procesar la informacion por cada 
        //uno de los puertos de cada uno de los dispositivos que existen 
        foreach (var item in dispositivos)
        {
          item.updateDataReceived();
        }

        //Aumentar el tiempo global en 1 
        current_time = current_time + 1;
      }


      foreach (var item in dispositivos)
      {
        item.WriteDataInFile();
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

      string[] instruccionpartida = _instruccion.Split(" ");

      if (instruccionpartida.Length < 1)
        EnviromentActions.LanzarExepciondeCasteo(instruccion);

      if (instruccionpartida[0][0] == '#')
        return;

      uint tiempodelainstruccion;

      if (!UInt32.TryParse(instruccionpartida[0], out tiempodelainstruccion))
      {
        throw new FormatException($"no tiene un formato válido '{instruccionpartida[0]}' para ser el tiempo de una instruccion ");
      }

      if (instruccionpartida.Length < 2)
        EnviromentActions.LanzarExepciondeCasteo(instruccion);

      InstructionType tipoinstruccion = AuxiliaryFunctions.GiveMeTheInstruction(instruccionpartida[1]);

      if (tipoinstruccion == InstructionType.create)
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

          Hub hub = new Hub(name, (int)cantidaddepuertos, Program.dispositivos.Count);
          Program.dispositivos.Add(hub);

        }
        else if (instruccionpartida[2] == "host")
        {
          cantidaddepuertos = 1;

          Host computadora = new Host(name, Program.dispositivos.Count);
          Program.dispositivos.Add(computadora);
        }

        else if (instruccionpartida[2] == "switch")
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

      else if (tipoinstruccion == InstructionType.connect)
      {
        if (instruccionpartida.Length < 4)
          EnviromentActions.LanzarExepciondeCasteo(instruccion);

        string port1 = instruccionpartida[2];
        string port2 = instruccionpartida[3];

        Device disp1;
        Device disp2;

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



        Port p1 = disp1.DameElPuerto(numeroport1);
        Port p2 = disp2.DameElPuerto(numeroport2);

        Cable cable = new Cable();

        EnviromentActions.ConnectPortsByCable(cable,
                                              puerto1: p1,
                                              puerto2: p2);


      }

      else if (tipoinstruccion == InstructionType.send)
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

        Device[] comp = disp.ToArray();
        if (comp.Length != 1)
        {
          throw new Exception("no se encontró el dispositivo");
        }

        Host computadora = comp[0] as Host;

        List<Bit> paquetedebits = new List<Bit>();

        foreach (var item in data)
        {
          paquetedebits.Add((Bit)int.Parse(item.ToString()));
        }

        computadora.send(paquetedebits);
      }

      else if (tipoinstruccion == InstructionType.disconnect)
      {
        if (instruccionpartida.Length < 4)
          EnviromentActions.LanzarExepciondeCasteo(instruccion);

        string port1 = instruccionpartida[2];
        string port2 = instruccionpartida[3];

        Device dispositivo1 = dispositivos.Where(x => x.Name == instruccionpartida[2].Split('_')[0]).FirstOrDefault();
        Device dispositivo2 = dispositivos.Where(x => x.Name == instruccionpartida[3].Split('_')[0]).FirstOrDefault();

        if (dispositivo1 == null)
          throw new InvalidCastException($"El puerto {port1} al que se esta tratando de acceder no existe ");

        if (dispositivo2 == null)
          throw new InvalidCastException($"El puerto {port2} al que se esta tratando de acceder no existe ");

        int numeropuerto1 = int.Parse(port1.Split('_')[1]) - 1;
        int numeropuerto2 = int.Parse(port2.Split('_')[1]) - 1;

        Port p1 = dispositivo1.DameElPuerto(numeropuerto1);
        Port p2 = dispositivo2.DameElPuerto(numeropuerto2);

        p1.Cable = null;
        p2.Cable = null;
      }

      else if (tipoinstruccion == InstructionType.mac)
      {
        if (instruccionpartida.Length < 4)
        {
          throw new InvalidCastException($"La instruccion mac '{_instruccion}' no tiene un formato valido");
        }

        Device disp = dispositivos.Where(x => x.Name == instruccionpartida[2]).FirstOrDefault();

        Host comp = null;

        if (disp is Host)
          comp = disp as Host;

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

      else if (tipoinstruccion == InstructionType.send_frame)
      {
        if (instruccionpartida.Length < 5)
        {
          throw new InvalidCastException($"La instruccion mac '{_instruccion}' no tiene un formato valido");
        }


        Device disp = dispositivos.Where(x => x.Name == instruccionpartida[2]).FirstOrDefault();

        Host comp = null;

        if (disp is Host)
          comp = disp as Host;

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

      else if (tipoinstruccion == InstructionType.ip)
      {
        if (instruccionpartida.Length < 5)
        {
          throw new InvalidCastException($"La instruccion mac '{_instruccion}' no tiene un formato valido");
        }

        var hostname = instruccionpartida[2].Split(':',' ').FirstOrDefault(); 

        Device disp = dispositivos.Where(x => x.Name == hostname).FirstOrDefault();

        Host _host = null;

        if (disp is Host)
          _host = disp as Host;

        if (_host is null)
        {
          throw new NullReferenceException($"No se puede encontrar el Host '{instruccionpartida[2]}' en los dispositivos actuales");
        }

        var ipaddress = instruccionpartida[3];
        var mask = instruccionpartida[4];

        _host.TakeIpAndMaskAddres(ipaddress, mask); 

      }

      else if (tipoinstruccion == InstructionType.send_packet)
      {
        if (instruccionpartida.Length < 5)
        {
          throw new InvalidCastException($"La instruccion send_packet '{_instruccion}' no tiene un formato valido");
        }

        var hostname = instruccionpartida[2].Split(':', ' ').FirstOrDefault();

        Device disp = dispositivos.Where(x => x.Name == hostname).FirstOrDefault();

        Host _host = disp as Host;

        if (_host is null)
        {
          throw new NullReferenceException($"No se puede encontrar el Host '{instruccionpartida[2]}' en los dispositivos actuales");
        }

        IP _ip = new IP(instruccionpartida[3], System.Globalization.NumberStyles.None);

        var data = instruccionpartida[4];

        _host.send_packet(_ip.ToString(), data); 
      }
    }


    /// <summary>
    /// Este enumerable devuelve todas las instrucciones que se va a ejecutar en 
    /// un tiempo determinado 
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private static IEnumerable<Instruccion> ProximasInstruccionesEjecutar(uint time)
    {
      while (Program.instrucciones.Count > 0 && Program.instrucciones.Peek().Time <= time)
      {
        yield return Program.instrucciones.Dequeue();
      }
    }

  }
}
