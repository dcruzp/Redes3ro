using ProyecotdeRedes.Component;
using ProyecotdeRedes.Devices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ProyecotdeRedes
{
  public class Device
  {
    /// <summary>
    /// Esto es para representar los puertos que 
    /// que tiene el dispositivo.
    /// </summary>
    protected Port[] ports;


    /// <summary>
    /// Este es el nombre que tiene el dispositivo 
    /// </summary>
    protected string name;


    /// <summary>
    /// Esto es para representar el bit de salida del dispositivo 
    /// inicialmente este es None
    /// </summary>
    //protected Bit bitsalida;


    /// <summary>
    /// Esto es para representar el bit de entrada del dispositivo
    /// inicialmente este es None
    /// </summary>
    protected Bit bitentrada;


    /// <summary>
    /// Este es para saber en el montículo del programa inicial donde 
    /// se almacenan los dispositivos, que indice tiene el en ese array 
    /// </summary>
    protected int indice;


    /// <summary>
    /// Esto es para representar la cantidad de puertos que tiene el dispositivo
    /// </summary>
    protected int cantidaddepuertos;




    /// <summary>
    /// Este el constructor de un dispositivo en general 
    /// Como es obvio todo dispositivo tiene un nombre , un indice 
    /// y la  cantidad de puertos que este va a tener
    /// </summary>
    /// <param name="name"></param>
    /// <param name="cantidaddepuertos"></param>
    /// <param name="indice"></param>
    public Device(string name, int cantidaddepuertos, int indice)
    {
      this.name = name;
      //this.bitsalida = Bit.none;
      bitentrada = Bit.none;
      this.indice = indice;
      this.cantidaddepuertos = cantidaddepuertos;

      ports = new Port[cantidaddepuertos];

      for (int i = 0; i < cantidaddepuertos; i++)
      {
        ports[i] = new Port(this.name + $"_{i}", i, this);
      }


      BytesReceives = new List<OneBytePackage>();

      _history = new List<DataFramePackage>();
    }

    /// <summary>
    /// Este es publico y solo es de consulta esta propiedad
    /// por lo que un dispositivo una vez es creado no se puede 
    /// destruir , ni eliminar de la lista 
    /// </summary>
    public int Indice
    {
      get => indice;
    }



    /// <summary>
    /// para saber el numero de puertos del dispositivo 
    /// </summary>
    public int NumerodePuertos
    {
      get => cantidaddepuertos;
    }


    /// <summary>
    /// Este campo es solo de lectura , para saber el nombre
    /// del dispositivo. 
    /// </summary>
    public string Name
    {
      get => name;
    }

    /// <summary>
    /// Retorna la instancia del Puerto que corresponde al indice 
    /// que se pasa por el id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Port DameElPuerto(int id)
    {
      if (id >= cantidaddepuertos || id < 0)
        throw new IndexOutOfRangeException("El dispositivo no tiene el puerto que se le especifica");

      return ports[id];
    }




    /// <summary>
    /// Esto crea un fichero si no existe , y si existe lo abre y 
    /// escribe lo que se le pasa por el parámetro indicado en recibo
    /// El txt que se crea o abre corresponde al dispositivo que es instanciado 
    /// por esta clase , (nombredeldispositivo .txt)
    /// </summary>
    /// <param name="recibo"></param>
    public void EscribirEnLaSalida(string recibo, string filename = null)
    {
      string fileName = filename == null ? name + ".txt" : filename;

      string rutaCompleta = Path.Join(DirectorioDeSalida(), fileName);

      //se crea el archivo si no existe y lo abre si ya existe 
      using (StreamWriter mylogs = File.AppendText(rutaCompleta))
      {
        mylogs.WriteLine(recibo);

        mylogs.Close();
      }
    }

    /// <summary>
    /// Esto retorna el path del directorio de salida donde se va a escribir 
    /// donde se van a crear los ficheros para escribir la salidas correspondientes
    /// </summary>
    /// <returns></returns>
    string DirectorioDeSalida()
    {
      var CurrentDirectory = Environment.CurrentDirectory;
      var parent = Directory.GetParent(Directory.GetParent(Directory.GetParent(CurrentDirectory).FullName).FullName);
      return Path.Join(parent.FullName, "output");
    }






    /// <summary>
    /// Este método pone el bit de salida que le corresponde
    /// es ese mili segundo estar en la salida . Este método se 
    /// llama una sola vez en cada mili segundo de ejecución del 
    /// programa
    /// </summary>
    public void UpdateSendingBit()
    {
      foreach (var item in ports)
      {
        item.UpdateOutBit();
      }
    }


    public List<OneBytePackage> BytesReceives { get; set; }


    public void updateDataReceived()
    {
      foreach (var item in ports)
      {
        item.UpdateInBit();
      }
    }


    protected DataFramePackage currentBuildInFrame;

    public List<DataFramePackage> _history { get; set; }


    public virtual void ProcessDataReceived()
    {
      if (currentBuildInFrame == null)
      {
        currentBuildInFrame = new DataFramePackage();
      }

      currentBuildInFrame.InsertNextByte(BytesReceives[BytesReceives.Count - 1].Byte);

      if (currentBuildInFrame.FullData)
      {
        string salida = name + " ===>> " + currentBuildInFrame.ToString();
        Console.WriteLine(salida);
      }
    }

    public void WriteDataInFile()
    {
      EscribirEnLaSalida(ToString(), name + ".txt");
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();

      var history = new List<OneBitPackage>();

      foreach (var port in ports)
      {
        history.AddRange(port.GiveMeHistory);
      }


      foreach (var item in history.OrderBy(x => x.Time))
      {
        stringBuilder.AppendLine(item.ToString());

      }

      return stringBuilder.ToString();
    }

  }
}
