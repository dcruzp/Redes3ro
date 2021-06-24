using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyecotdeRedes.Component
{
    public class OneBytePackage
    {
        public Byte Byte { get; set; }
        public uint time_received { get; set;  } 
        public string portreceived { get; set; }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            
            stringBuilder.Append(Auxiliaries.AuxiliaryFunctions.FromByteDataToHexadecimal(Byte.GiveMeBits.ToList()));

            return stringBuilder.ToString(); 
        }
    }
}
