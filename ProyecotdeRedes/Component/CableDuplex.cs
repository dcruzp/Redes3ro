using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProyecotdeRedes.Devices;

namespace ProyecotdeRedes.Component
{
    public class CableDuplex : ICable
    {
        Port _puerto1;
        Port _puerto2;

        Bit _bit1;
        Bit _bit2;

        public CableDuplex() 
        {
            this._puerto1 = null;
            this._puerto2 = null;
            this._bit1 = Bit.none;
            this._bit2 = Bit.none; 
        }

        public Port puerto1 { get => this._puerto1; set => this._puerto1 = value; }
        public Port puerto2 { get => this._puerto2; set => this._puerto2 = value; }
        public Bit bit1 { get => this._bit1; set => this._bit1 = value; }
        public Bit bit2 { get => this._bit2; set => this._bit2 = value; }
    }
}
