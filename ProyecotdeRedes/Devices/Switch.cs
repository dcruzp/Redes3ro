using ProyecotdeRedes.Auxiliaries;
using ProyecotdeRedes.Devices;
using System.Collections.Generic;

namespace ProyecotdeRedes
{
  class Switch : Device
  {
    public Switch(string name, int cantidaddepuertos, int indice) : base(name, cantidaddepuertos, indice)
    {
      this.name = name;
      this.cantidaddepuertos = cantidaddepuertos;
      this.indice = indice;
    }

    public override void ProcessDataReceived()
    {
      base.ProcessDataReceived();

      if (currentBuildInFrame.FullData)
      {
        _history.Add(currentBuildInFrame);

        string port = BytesReceives[BytesReceives.Count - 1].portreceived;

        Port ptReceived = ports[int.Parse(port.Split('_')[1])];

        string dirMacFromDataReceived = AuxiliaryFunctions.FromByteDataToHexadecimal(currentBuildInFrame.MacOut);

        ptReceived.PutMacDirection(dirMacFromDataReceived);

        string dirMacHostIn = AuxiliaryFunctions.FromByteDataToHexadecimal(currentBuildInFrame.MacIn);

        Port ptToSend = GimePortWithDirMac(dirMacHostIn);

        if (ptToSend == null || dirMacHostIn == "FFFF")
        {
          List<Bit> datatosend = currentBuildInFrame.GetAllDataFrame();

          foreach (var item in ports)
          {
            if (item.Equals(ptReceived))
              continue;

            item.SendData(datatosend);
          }
        }
        else
        {
          ptToSend.SendData(currentBuildInFrame.GetAllDataFrame());
        }
        currentBuildInFrame = null;
      }
    }


    public Port GimePortWithDirMac(string dirMac)
    {
      foreach (var item in ports)
      {
        if (item.DirMac == dirMac)
        {
          return item;
        }
      }
      return null;
    }
  }
}
