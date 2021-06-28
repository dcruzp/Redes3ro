using ProyecotdeRedes.Auxiliaries;
using ProyecotdeRedes.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using Action = ProyecotdeRedes.Component.Action;
using Byte = ProyecotdeRedes.Component.Byte;

namespace ProyecotdeRedes.Devices
{
  public class Port : IPuertos
  {
    /// <summary>
    /// esto es para representar le id que tiene el puerto en
    /// el dispositivo al que pertenece 
    /// </summary>
    string port_id;


    /// <summary>
    /// Este es para representar el nombre del puerto
    /// con el id correspondiente
    /// </summary>
    int port_number;



    /// <summary>
    /// Esto es para poner los bit de salida que hay en esta computadora 
    /// </summary>
    Bit _outBit;


    /// <summary>
    /// Esto es una cola para representar los bit de salidas por el puerto 
    /// </summary>
    Queue<Bit> queueoutput;


    Bit _inBit;

    Device _divice_belongs;


    string _dirMac;

    /// <summary>
    /// Esto es para saber que bits se han recibido de alguna 
    /// computadora en especifico en un mili segundo determinado
    /// </summary>
    //bool[] entradas;


    /// <summary>
    /// esto es para saber si el dispositivo esta conectado con algún 
    /// otro dispositivo 
    /// </summary>
    //bool estaConectado;


    /// <summary>
    /// Esto te desconecta el puerto actual de cualquier otro puerto 
    /// al que se encuentre conectado 
    /// </summary>

    protected ICable _cable;

    /// <summary>
    /// Esta es la dirección Mac del dispositivo que esta conectada
    /// a este dispositivo
    /// </summary>
    public string DirMac
    {
      get => _dirMac;
    }

    public bool PutMacDirection(string dirMac)
    {
      if (!CheckMetods.CheckIsOkDirMac(dirMac))
        return false;

      _dirMac = dirMac;
      return true;
    }


    public int PortNumber
    {
      get => port_number;
    }

    /// <summary>
    /// este es el constructor del puerto con su nombre y su id
    /// </summary>
    /// <param name="id_puerto"></param>
    /// <param name="numero_puerto"></param>
    public Port(string id_puerto, int numero_puerto, Device dispositivo)
    {
      port_id = id_puerto;
      _outBit = Bit.none;
      port_number = numero_puerto;
      _cable = null;
      _divice_belongs = dispositivo;
      queueoutput = new Queue<Bit>();
      _history = new List<OneBitPackage>();


    }


    public void SendData(List<Bit> datatosend)
    {
      foreach (var item in datatosend)
      {
        queueoutput.Enqueue(item);
      }
    }

    public Device DispPertenece
    {
      get => _divice_belongs;
    }

    public List<OneBitPackage> GiveMeHistory
    {
      get => _history;
    }

    public bool ConnectCableToPort(Cable cable)
    {
      if (_cable != null)
      {
        return false;
      }
      _cable = cable;
      return true;
    }



    public string ID_Puerto
    {
      get => port_id;
    }


    public Bit OutBit
    {
      get => _outBit;
      set => _outBit = value;
    }


    public Bit InBit
    {
      get => _inBit;
      set => _inBit = value;
    }


    /// <summary>
    /// el time_sending me representa el tiempo en milisegundos que 
    /// se ha estado transmitiendo el bit que esta al principio de la 
    /// cola y que debe permanecer transmitiendose una cantidad de milisegundos 
    /// igual al signal_time para que sea considerado como enviado 
    /// </summary>
    private int time_sending = 0;

    private List<OneBitPackage> _history;

    private int time_received = 0;

    private int time_received_byte = 0;

    private Bit AuxInBit = Bit.none;


    /// <summary>
    /// Esto es para actualizar y hacer todo las acciones 
    /// para procesar los datos que se tienen hasta el momento 
    /// </summary>
    public void UpdateInBit()
    {
      InBit = GiveMeInBit();

      if (InBit == Bit.none)
      {
        AuxInBit = Bit.none;
        time_received = 0;
        time_received_byte = 0;
        return;
      }

      if (AuxInBit == Bit.none)
      {
        AuxInBit = InBit;
      }

      if (AuxInBit == InBit)
      {
        time_received++;
      }
      else
      {
        AuxInBit = InBit;
        time_received = 0;
        time_received_byte = 0;
      }

      if (time_received >= Program.signal_time)
      {
        OneBitPackage bitreceived = new OneBitPackage(
            Program.current_time,
            Action.Received,
            InBit,
            port: port_id);


        _history.Add(bitreceived);

        time_received_byte++;

        if (time_received_byte == 8)
        {

          OneBytePackage oneBitPackage = BuildBytePackage();

          Console.WriteLine($"{port_id} received the byte: {oneBitPackage.ToString()} in time: {Program.current_time}");
          time_received_byte = 0;

          DispPertenece.BytesReceives.Add(oneBitPackage);
          DispPertenece.ProcessDataReceived();
        }


        time_received = 0;
        AuxInBit = Bit.none;
      }
    }


    /// <summary>
    /// Est oes para contruir una instancia de OneBytePackage con los 
    /// ultimos 8 bits recibidos por este puerto. Como el Byte es contruido 
    /// en el instant en que se llama a la funcion entonces es asume que el 
    /// tiempo recepcion del Byte es el momento actual cuando se llama al metodo
    /// </summary>
    /// <returns></returns>
    private OneBytePackage BuildBytePackage()
    {
      Bit[] bits = _history.TakeLast(8).Select(x => x.Bit).ToArray();

      Byte @byte = new Byte(bits);

      OneBytePackage oneBytePackage = new OneBytePackage();
      oneBytePackage.Byte = @byte;
      oneBytePackage.portreceived = port_id;
      oneBytePackage.time_received = Program.current_time;

      return oneBytePackage;
    }

    /// <summary>
    /// Este metodo es para actualizar el bit de la salida del 
    /// puerto. Este metodo mantine el bit de salida 10 ms en 
    /// la salida y cuando se termina de transmitir este bit 
    /// entonces se procede a enviar el proximo en la cola de 
    /// salida
    /// </summary>
    public void UpdateOutBit()
    {
      if (queueoutput.Count == 0 || _cable == null)
      {
        OutBit = Bit.none;
      }
      else
      {

        //if (this.tiempoesperandoparavolveraenviar > 0)
        //{
        //    this.tiempoesperandoparavolveraenviar--;
        //    this.bitsalida = Bit.none;
        //    return;
        //}

        OutBit = queueoutput.Peek();

        time_sending++;

        if (time_sending >= Program.signal_time)
        {
          time_sending = 0;

          OneBitPackage oneBitPackage = new OneBitPackage(
                                                          time: Program.current_time,
                                                          action: Action.Send,
                                                          bit: OutBit,
                                                          actionResult: ActionResult.Ok,
                                                          port: port_id);

          string salida = port_id + " ---> " + oneBitPackage.ToString();

          Console.WriteLine(salida);

          _history.Add(oneBitPackage);

          queueoutput.Dequeue();
        }
      }
    }

    public Bit GiveMeOutBit()
    {
      return OutBit;
    }

    public Bit GiveMeInBit()
    {
      if (_cable == null)
        return Bit.none;

      Port otherPort = _cable.puerto1.Equals(this) ?
          _cable.puerto2 :
          _cable.puerto1;
      return otherPort.GiveMeOutBit();
    }

    public Cable Cable
    {
      set => _cable = value;
    }
  }
}
