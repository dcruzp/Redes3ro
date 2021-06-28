using ProyecotdeRedes.Auxiliaries;
using ProyecotdeRedes.Component;
using System;
using System.Collections.Generic;

namespace ProyecotdeRedes
{
  class Host : Device
  {
    /// <summary>
    /// Esto es una cola para almacenar los bits que quedan 
    /// por enviar aun. Si la cola esta vacía es que no quedan 
    /// bit por enviar. 
    /// </summary>
    //Queue<Bit> porenviar;





    /// <summary>
    /// Esta es la dirección Max representada en hexadecimal 
    /// </summary>
    string direccionMax;







    /// <summary>
    /// esto es para determinar el tiempo que ha estado la
    /// computadora sin enviar información producto de una 
    /// colisión que detecto anteriormente
    /// </summary>
    uint tiempoesperandoparavolveraenviar;

    public Host(string name, int indice) : base(name, 1, indice)
    {
      //this.porenviar = new Queue<Bit>();
      direccionMax = null;
    }


    /// <summary>
    /// Esto es para darle una dirección Mac a la computadora 
    /// El parámetro es un string hexadecimal de 4 dígitos
    /// </summary>
    /// <param name="dirMac"></param>
    public void PutMacDirection(string dirMac)
    {
      if (direccionMax == null && CheckMetods.CheckIsOkDirMac(dirMac))
      {
        direccionMax = dirMac;
      }
      else
      {
        throw new InvalidCastException($"No se pudo asignar la dirección Mac {dirMac} a la computadora {name}");
      }
    }


    /// <summary>
    /// send_frame procesa todos los datos necesarios 
    /// para armar el contenido de un frame yenviarlos 
    /// por el puerto.  
    /// </summary>
    /// <param name="mac">Este parametro es la data de la 
    /// direccion mac en formato hexadecimal. Es la direccion 
    /// mac de la computadora a la que se le va a enviar la data</param>
    /// <param name="data">Este parametro es la data que se va a 
    /// enviar en el frame, en formato hexadecimal </param>
    public void send_frame(string mac, string data)
    {

      var dirmacin = string.IsNullOrEmpty(mac) ? "FFFF" : mac;

      var dirmacout = string.IsNullOrEmpty(direccionMax) ? "FFFF" : direccionMax;

      var lenghtdata = AuxiliaryFunctions.GiveMeLenghtInHexadecimal(data);


      var sumdatahex = AuxiliaryFunctions.SumOfDataInHex(data);

      var lenghtsumdatahex = AuxiliaryFunctions.GiveMeLenghtInHexadecimal(sumdatahex);

      var packagetosend = AuxiliaryFunctions.ConvertToListOfBitHexadecimalSequence(dirmacin, dirmacout, lenghtdata, lenghtsumdatahex, data, sumdatahex);


      foreach (var item in ports)
      {
        item.SendData(packagetosend);
      }

      Console.WriteLine($"Enviado por la computadora '{name}' el " +
          $"paquete {AuxiliaryFunctions.FromByteDataToHexadecimal(packagetosend)}");

    }





    /// <summary>
    /// Este método se llama cuando hubo una instrucción 
    /// send para el envío de un paquete de bits
    /// </summary>
    /// <param name="paquete"></param>
    public void send(List<Bit> pakage)
    {
      foreach (var item in ports)
      {
        item.SendData(pakage);
      }
    }



    public override void ProcessDataReceived()
    {
      base.ProcessDataReceived();

      if (currentBuildInFrame.FullData)
      {
        //currentBuildInFrame.CheckIsOkData(); 
        _history.Add(currentBuildInFrame);
        WriteDataReceivedInOutput(currentBuildInFrame);
        currentBuildInFrame = null;
      }
    }

    private void WriteDataReceivedInOutput(DataFramePackage package)
    {
      uint time_received = package.TimeReceived;
      string pcout = AuxiliaryFunctions.FromByteDataToHexadecimal(package.MacOut);
      string data = AuxiliaryFunctions.FromByteDataToHexadecimal(package.Data);

      string dataFrame = package.ToString();

      EscribirEnLaSalida(dataFrame, name + "_data.txt");
    }
  }
}
