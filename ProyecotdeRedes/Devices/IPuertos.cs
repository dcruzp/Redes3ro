using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyecotdeRedes.Devices
{
    public interface IPuertos
    {
        public void UpdateOutBit();
        public void UpdateInBit();
        public Bit GiveMeOutBit();
        public Bit GiveMeInBit(); 
    }
}
