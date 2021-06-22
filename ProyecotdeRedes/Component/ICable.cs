using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProyecotdeRedes.Devices;

namespace ProyecotdeRedes.Component
{
    public interface ICable
    {
        public Puerto puerto1 { get; set; }
        public Puerto puerto2 { get; set; }


        public Bit bit1 { get; set; }

        public Bit bit2 { get; set; }




    }
}
