using System;
using System.Collections.Generic;
using System.Text;
using ProyecotdeRedes.Devices;
using ProyecotdeRedes.Component;

namespace ProyecotdeRedes.Component
{

    public class Cable : ICable
    {
        Puerto _puerto1;
        Puerto _puerto2;

        Bit _bit1;
        Bit _bit2;


        public Cable()
        {
            this._puerto1 = null;
            this._puerto2 = null;
            this._bit1 = Bit.none;
            this._bit2 = Bit.none;
        }

        public Puerto puerto1 { get => this._puerto1; set => this._puerto1 = value; }
        public Puerto puerto2 { get => this._puerto2; set => this._puerto2 = value; }
        public Bit bit1 { get => this._bit1; set => this._bit1 = value;  }
        public Bit bit2 { get => Bit.none; set => this._bit2 = Bit.none; }
    }
}
