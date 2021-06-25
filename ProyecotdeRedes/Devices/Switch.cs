using ProyecotdeRedes.Auxiliaries;
using ProyecotdeRedes.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyecotdeRedes
{
    class Switch:Dispositivo
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

                string port =  BytesReceives[BytesReceives.Count - 1].portreceived;
                
                Puerto ptReceived = this.puertos[int.Parse(port.Split('_')[1])];

                string dirMacFromDataReceived = AuxiliaryFunctions.FromByteDataToHexadecimal(currentBuildInFrame.MacOut);

                ptReceived.PutMacDirection(dirMacFromDataReceived);

                string dirMacHostIn = AuxiliaryFunctions.FromByteDataToHexadecimal(currentBuildInFrame.MacIn); 

                Puerto ptToSend = GimePortWithDirMac(dirMacHostIn);

                if (ptToSend == null || dirMacHostIn == "FFFF") 
                {
                    List<Bit> datatosend = currentBuildInFrame.GetAllDataFrame();

                    foreach (var item in this.puertos)
                    {
                        if (item.Equals(ptReceived)) continue;

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


        public Puerto GimePortWithDirMac (string dirMac)
        {
            foreach (var item in this.puertos)
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
